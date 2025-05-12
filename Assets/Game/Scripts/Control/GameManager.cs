using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : PersistSingleton<GameManager>
{
    public GameObject Player { get; private set; }
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

            var stats = Player.GetComponent<PlayerStats>();
            var player = Player.GetComponent<PlayerSFM>();
            if (stats.LastSavePoint != null)
            {
                stats.SetSavePoint(Player.GetComponent<PlayerStats>().LastSavePoint);
                player.StateMachine.ChangeState(player.saveState);
                SaveLoadManager.Instance.SaveGame();
                Debug.Log("Saved");
            }
            else
            {
                PauseManager.Instance.TogglePause();
            }
        }
    }
    private bool isNewGame = false;
    void OnFirstSceneLoaded()
    {
        if (isNewGame)
        {
            SaveLoadManager.Instance.CreateNewGame();
            isNewGame = false;
        }

        playerInput = FindAnyObjectByType<PlayerInput>();

        StartGameplayLogic();
        MovePlayerToSavePoint();
    }
    public void HandlePlayerDeath()
    {
        SaveLoadManager.Instance.SaveGame();

        Player.GetComponent<PlayerSFM>().Restart();
        MovePlayerToSavePoint();
    }

    public void MovePlayerToSavePoint()
    {
        if (ObjectIsNull(Player, "Player")) return;
        Player.transform.position = SaveLoadManager.Instance.GameData.player.savePoint.Position;
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

    public void StartGameplayLogic()
    {
        SaveLoadManager.Instance.LoadGame();
    }

    public void ExitGame()
    {
        SaveLoadManager.Instance.SaveGame();
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
