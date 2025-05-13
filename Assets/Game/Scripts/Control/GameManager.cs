using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : PersistSingleton<GameManager>
{
    public GameObject Player { get; private set; }
    public PlayerStats playerStats;
    public PlayerSFM playerSFM;
    public PlayerInput playerInput;


    private const string PreviousScenePathKey = "PlayFromZeroScene_PreviousScenePath";

    protected override void Awake()
    {
        base.Awake();

        SceneLoader.LoadMenu();
        SceneLoader.Instance.OnFirstSceneLoaded += OnFirstSceneLoaded;
        SceneLoader.Instance.OnGameplayUILoaded += OnGameplayUILoaded;
#if UNITY_EDITOR
        string previousScenePath = EditorPrefs.GetString(PreviousScenePathKey, "");
        if (!string.IsNullOrEmpty(previousScenePath))
        {
            Debug.Log($"Loading previous scene: {previousScenePath}");
            if (previousScenePath.Contains("MainMenu") || previousScenePath.Contains("Pause"))
                SceneLoader.LoadMenu();
            else
                SceneLoader.Instance.StartNewGame(previousScenePath);
        }
        else
        {
            SceneLoader.LoadMenu();
        }
        return;
#endif
    }
    public void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions.FindAction("Pause").performed -= PauseCallback;
        }
    }

    private void OnGameplayUILoaded()
    {
        if (playerInput != null)
        {
            playerInput.actions.FindAction("Pause").performed += PauseCallback;
        }
    }

    public void PauseCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (ObjectIsNull(Player, "Player"))
                return;

            if (playerStats.LastSavePoint != null)
                SaveManager.Instance.SaveGame();
            else
                PauseManager.Instance.TogglePause();
        }
    }

    private bool isNewGame = false;
    void OnFirstSceneLoaded()
    {
        if (isNewGame)
        {
            FileSaveManager.Instance.CreateNewGame();
            isNewGame = false;
        }

        LoadVariables();
        MovePlayerToSavePoint();
    }
    public void HandlePlayerDeath()
    {
        DeathManager.Instance.HandleDeath();
        FileSaveManager.Instance.SaveGame();
    }

    public void MovePlayerToSavePoint()
    {
        if (ObjectIsNull(Player, "Player")) return;
        Player.transform.position = FileSaveManager.Instance.GameData.player.savePoint.Position;
    }

    public void StartGame()
    {
        SceneLoader.Instance.StartNewGame("FirstLevel");

    }
    public void CreateNewGame()
    {
        SceneLoader.Instance.StartNewGame("FirstLevel");
        isNewGame = true;
    }

    public void SetPlayer(GameObject player)
    {
        if (Player != null)
            Player.GetComponent<PlayerStats>().Health.OnDeath -= HandlePlayerDeath;

        Player = player;
        Player.GetComponent<PlayerStats>().Health.OnDeath += HandlePlayerDeath;
    }

    public void LoadVariables()
    {
        FileSaveManager.Instance.LoadGame();
        playerInput = FindAnyObjectByType<PlayerInput>();
        playerSFM = FindAnyObjectByType<PlayerSFM>();
        playerStats = FindAnyObjectByType<PlayerStats>();

    }

    public void ExitGame()
    {
        FileSaveManager.Instance.SaveGame();
        SceneLoader.BackToMainMenu();
    }
    private bool ObjectIsNull<T>(T ob, string name)
    {
        if (ob == null)
        {
            Debug.LogError($"GameManager requires a {name} GameObject reference!");
            enabled = false;
            return true;
        }
        return false;
    }
}
