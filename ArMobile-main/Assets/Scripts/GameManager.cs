using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] anchors;
    public GameObject zombi;
    private bool spawn = true;
    private GameObject camera;
    public LayerMask enemyLayer;
    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        anchors = GameObject.FindGameObjectsWithTag("Anchor");
        if(anchors.Length == 3)
        {
            if (spawn)
            {
                Instantiate(zombi);
                Instantiate(zombi, anchors[1].transform);
                Instantiate(zombi, anchors[2].transform);
                spawn = false;
            }
        }

        if (Pointer.current.press.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (hit.collider.gameObject.name.Contains("Head"))
                {
                    ZombieScript zparent = hit.collider.gameObject.GetComponentInParent<ZombieScript>();
                    if (zparent != null)
                    {
                        zparent.HeadShoot(); // Llama a la función pública TakeDamage en el script
                    }
                    Debug.Log("Head");
                } else if (hit.collider.gameObject.name.Contains("Body"))
                {
                    ZombieScript enemyScript = hit.collider.gameObject.GetComponent<ZombieScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.BodyShoot(); // Llama a la función pública TakeDamage en el script
                    }
                    Debug.Log("Body");
                }
            } else
            {
                Debug.Log("Miss");
            }
        }
    }
}
