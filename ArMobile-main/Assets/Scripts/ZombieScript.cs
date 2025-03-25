using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    private Transform target;
    private int lives = 3;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("MainCamera").transform;
        anim = transform.Find("zombie").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Rotate(0f, 180f, 0f);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        transform.position += -transform.forward * Time.deltaTime * 0.1f;
        if(lives <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void BodyShoot()
    {
        lives -= 1;
        anim.SetTrigger("Shoot");
        Debug.Log("Body");
    }

    public void HeadShoot()
    {
        lives -= 2;
        anim.SetTrigger("Shoot");
    }
}
