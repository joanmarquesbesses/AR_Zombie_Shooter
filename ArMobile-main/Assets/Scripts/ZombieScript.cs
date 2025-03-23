using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Rotate(0f, 180f, 0f);
        transform.position += -transform.forward * Time.deltaTime * 0.1f;
    }
}
