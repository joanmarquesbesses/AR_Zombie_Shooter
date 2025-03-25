using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] anchors;
    public GameObject zombi;
    private bool spawn = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        anchors = GameObject.FindGameObjectsWithTag("Anchor");
        if(anchors.Length == 3)
        {
            if (spawn)
            {
                Instantiate(zombi, anchors[0].transform);
                Instantiate(zombi, anchors[1].transform);
                Instantiate(zombi, anchors[2].transform);
                spawn = false;
            }
        }
    }
}
