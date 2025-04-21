using System.Collections;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.5f;
    public float attackRange = 0.1f;

    [Header("Attack")]
    public float attackInterval = 1f;
    public int attackDamage = 1;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip spawnSFX;
    public AudioClip[] idleSounds;
    public AudioClip deadSFX;
    public float idleSoundInterval = 5f;

    private Transform target;
    private Animator anim;
    private int lives = 3;
    public int pointsWorth = 5;
    private bool isAttacking = false;

    void Start()
    {
        GameObject camObj = GameObject.FindWithTag("MainCamera");
        if (camObj != null)
            target = camObj.transform;
        else
            Debug.LogError("ZombieScript: No MainCamera found.");

        Transform zombieChild = transform.Find("zombie");
        if (zombieChild != null)
            anim = zombieChild.GetComponent<Animator>();
        else
            Debug.LogError("ZombieScript: No child named 'zombie'.");

        idleSoundInterval = Random.Range(2f, 5f);

        audioSource = GetComponent<AudioSource>();

        if (spawnSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(spawnSFX);
        }

        if (idleSounds.Length > 0 && audioSource != null)
        {
            StartCoroutine(IdleSoundRoutine());
        }
    }

    private IEnumerator IdleSoundRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleSoundInterval);
            if (idleSounds.Length > 0)
            {
                AudioClip randomClip = idleSounds[Random.Range(0, idleSounds.Length)];
                audioSource.PlayOneShot(randomClip);
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            if (isAttacking)
            {
                StopAllCoroutines();
                isAttacking = false;
            }

            transform.LookAt(target);
            transform.Rotate(0f, 180f, 0f);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.position += -transform.forward * Time.deltaTime * 0.1f;
        }
        else if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        if (lives <= 0)
        {
            //GameManager.Instance.AddPoints(pointsWorth);
            audioSource.PlayOneShot(deadSFX);
            Destroy(gameObject);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        while (true)
        {
            if (anim != null) anim.SetTrigger("Shoot");

            GameManager.Instance.TakeDamage(attackDamage);

            yield return new WaitForSeconds(attackInterval);

            if (Vector3.Distance(transform.position, target.position) > attackRange)
                break;
        }
        isAttacking = false;
    }

    public void BodyShoot()
    {
        lives -= 1;
        Debug.Log($"Body shot: lives left = {lives}");
    }

    public void HeadShoot()
    {
        lives -= 2;
        Debug.Log($"Head shot: lives left = {lives}");
    }
}
