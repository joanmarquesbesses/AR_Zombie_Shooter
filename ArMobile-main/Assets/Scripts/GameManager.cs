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
    public static GameManager Instance { get; private set; }

    private GameState state = GameState.MENU;
    private GameObject[] anchors;
    public GameObject zombi;
    public LayerMask enemyLayer;
    private bool spawn = true;
    private GameObject camera;

    public int playerLives = 50;
    public float invulnerabilityTime = 1f;
    private bool canTakeDamage = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        switch (state)
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
        if (anchors.Length == 3 && spawn)
        {
            foreach (var anchor in anchors)
            {
                GameObject z = Instantiate(zombi);
                z.transform.position = anchor.transform.position;
            }
            spawn = false;
        }

        if (Pointer.current.press.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                ZombieScript enemyScript = hit.collider.GetComponentInParent<ZombieScript>();
                if (enemyScript != null)
                {
                    if (hit.collider.gameObject.name.Contains("Head"))
                        enemyScript.HeadShoot();
                    else if (hit.collider.gameObject.name.Contains("Body"))
                        enemyScript.BodyShoot();
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage) return;

        playerLives -= amount;
        Debug.Log($"Player took {amount} damage. Lives remaining: {playerLives}");

        if (playerLives <= 0)
        {
            playerLives = 0;
            GameOver();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invulnerabilityTime);
        canTakeDamage = true;
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        ChangeState(GameState.MENU);
    }

    private void CheckOnCanvas(bool activated, string name)
    {
        switch (name)
        {
            case "MainMenu":
                if (!activated) ChangeState(GameState.SPAWNERS);
                break;
            case "Start":
                if (!activated) ChangeState(GameState.GAMEPLAY);
                break;
            case "Gameplay":
                if (activated) ChangeState(GameState.GAMEPLAY);
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
                CanvasActivator.Instance.SetActiveCanvas("Gameplay", true);
                break;
        }
    }
}