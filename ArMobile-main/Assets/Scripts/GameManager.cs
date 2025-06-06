using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public enum GameState
{
    MENU,
    SPAWNERS,
    START,
    GAMEPLAY,
    GAMEOVER
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState state = GameState.MENU;
    private GameObject[] anchors;
    public GameObject zombi;
    public GameObject TombstoneCandles;
    public LayerMask enemyLayer;
    private bool spawn = true;
    private GameObject camera;

    public int playerLives = 5;
    public float invulnerabilityTime = 1f;
    private bool canTakeDamage = true;

    public float spawnInterval = 5f;
    private float spawnTimer;
    private bool initialSpawnDone = false;

    public TMP_Text scoreText;
    public TMP_Text scoreTextShadow;
    public float typeTime = 0.1f;
    private int visualScore = 0;
    private int currentScore = 0;

    private AudioSource audioSource;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

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
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        CanvasActivator.Instance.OnCanvasActivated += CheckOnCanvas;
    }

    private void OnDisable()
    {
        CanvasActivator.Instance.OnCanvasActivated -= CheckOnCanvas;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        spawnTimer = spawnInterval;
        scoreText.text = currentScore.ToString();
        scoreTextShadow.text = currentScore.ToString();
        PlayMusic(menuMusic);
    }

    void Update()
    {
        CheckFreeze();
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
                PlayMusic(gameMusic);
                GameplayLoop();
                break;
        }
    }

    private void CheckFreeze()
    {
        if (CanvasActivator.Instance.IsCanvasActive("PauseMenu") || state == GameState.GAMEOVER)
        {
            Time.timeScale = 0f;
            return;
        }
        else
        {
            Time.timeScale = 1f;
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

        if (anchors.Length == 1)
        {
            GameObject t = Instantiate(TombstoneCandles, anchors[0].transform.position, Quaternion.identity);
            t.transform.LookAt(camera.transform.position);
            t.transform.rotation = Quaternion.Euler(0f, t.transform.rotation.eulerAngles.y, 0f);
            ChangeState(GameState.SPAWNERS);
        }
        else if (anchors.Length == 2)
        {
            GameObject t = Instantiate(TombstoneCandles, anchors[1].transform.position, Quaternion.identity);
            t.transform.LookAt(camera.transform.position);
            t.transform.rotation = Quaternion.Euler(0f, t.transform.rotation.eulerAngles.y, 0f);
            ChangeState(GameState.SPAWNERS);
        }
        else if (anchors.Length == 3)
        {
            GameObject t = Instantiate(TombstoneCandles, anchors[2].transform.position, Quaternion.identity);
            t.transform.LookAt(camera.transform.position);
            t.transform.rotation = Quaternion.Euler(0f, t.transform.rotation.eulerAngles.y, 0f);
            ChangeState(GameState.START);
            anchorPlacer.enabled = false;
        }
    }

    private void GameplayLoop()
    {
        if (CanvasActivator.Instance.IsCanvasActive("PauseMenu"))
        {
            return;
        }

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

        if (!spawn)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawn = true;
                spawnTimer = 0;
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
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                ZombieScript enemyScript = hit.collider.GetComponentInParent<ZombieScript>();
                if (enemyScript != null)
                {
                    if (hit.collider.gameObject.name.Contains("Head"))
                    {
                        enemyScript.HeadShoot();
                        AddPoints(10);

                    }
                    else if (hit.collider.gameObject.name.Contains("Body"))
                    {
                        enemyScript.BodyShoot();
                        AddPoints(5);
                    }
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

    public void AddPoints(int points)
    {
        currentScore += points;
        StartCoroutine(TypeScore(points));
    }


    IEnumerator TypeScore(int pointsToAdd)
    {
        while (visualScore < currentScore)
        {
            visualScore += 1;
            scoreText.text = visualScore.ToString();
            scoreTextShadow.text = visualScore.ToString();
            yield return new WaitForSeconds(typeTime);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        CanvasActivator.Instance.SetActiveCanvas("GameOver", true);
        Time.timeScale = 0f;
        ChangeState(GameState.GAMEOVER);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application Quit");
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