using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public enum GameState
{
    MENU,
    SPAWNERS,
    START,
    GAMEPLAY
}

public class GameManager : MonoBehaviour
{
    private GameState state = GameState.MENU;
    private GameObject[] anchors;
    public GameObject zombi;
    private bool spawn = true;
    private GameObject camera;
    public LayerMask enemyLayer;

    private void OnEnable()
    {
        CanvasActivator.Instance.OnCanvasActivated += CheckOnCanvas;
    }

    private void OnDisable()
    {
        CanvasActivator.Instance.OnCanvasActivated -= CheckOnCanvas;
    }


    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        switch(state)
        {
            case GameState.MENU:
                break;

            case GameState.SPAWNERS:
                AnchorPlacement();
                break;

            case GameState.START:
                break;

            case GameState.GAMEPLAY:
                GameplayLoop();
                break;

        }
    }

    private void AnchorPlacement()
    {
        ARAnchorPlacer anchorPlacer = GetComponent<ARAnchorPlacer>();
        if (!anchorPlacer.isActiveAndEnabled)
        {
            anchorPlacer.enabled = true;
        }


        anchors = GameObject.FindGameObjectsWithTag("Anchor");
        if (anchors.Length == 3)
        {
            ChangeState(GameState.START);
            anchorPlacer.enabled = false;
        }
    }


    private void GameplayLoop()
    {
        anchors = GameObject.FindGameObjectsWithTag("Anchor");
        if (anchors.Length == 3)
        {
            if (spawn)
            {
                for (int i = 0; i < anchors.Length; i++)
                {
                    GameObject z = Instantiate(zombi);
                    z.transform.position = anchors[i].transform.position;
                }
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
                    ZombieScript enemyScript = hit.collider.gameObject.GetComponentInParent<ZombieScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.HeadShoot(); // Llama a la función pública TakeDamage en el script
                    }
                }
                else if (hit.collider.gameObject.name.Contains("Body"))
                {
                    ZombieScript enemyScript = hit.collider.gameObject.GetComponentInParent<ZombieScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.BodyShoot(); // Llama a la función pública TakeDamage en el script
                    }
                }
            }
        }
    }

    private void CheckOnCanvas(bool activated, string name)
    {
        switch(name)
        {
            case "MainMenu":
                if (!activated)
                {
                    ChangeState(GameState.SPAWNERS);
                }
                break;
            case "Start":
                break;
            case "Gameplay":
                if (activated)
                {
                    ChangeState(GameState.GAMEPLAY);
                }
                break;
        }
    }

    private void ChangeState(GameState gamestate)
    {
        state = gamestate;


        switch (state)
        {
            case GameState.MENU:
                break;

            case GameState.SPAWNERS:
                break;

            case GameState.START:
                CanvasActivator.Instance.SetActiveCanvas("Start", true);
                break;

            case GameState.GAMEPLAY:
                break;

        }
    }
}
