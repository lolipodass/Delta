using System;
using UnityEditor;
using UnityEngine;

public class GameManager : PersistSingleton<GameManager>
{
    public GameObject Player { get; private set; }

    private const string PreviousScenePathKey = "PlayFromZeroScene_PreviousScenePath";

    protected override void Awake()
    {
        base.Awake();

        SceneLoader.LoadMenu();
        SceneLoader.Instance.OnFirstSceneLoaded += OnFirstSceneLoaded;
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

    private bool isNewGame = false;
    void OnFirstSceneLoaded()
    {
        if (isNewGame)
        {
            SaveLoadManager.Instance.SaveGame();
            isNewGame = false;
        }
        StartGameplayLogic();
    }
    public void HandlePlayerDeath()
    {
        SaveLoadManager.Instance.SaveGame();
        Player.GetComponent<PlayerSFM>().Restart();
        MovePlayerToSavePoint();
    }

    public void MovePlayerToSavePoint()
    {
        if (CheckObject(Player, "Player")) return;
        Player.transform.position = SaveLoadManager.Instance.GameData.playerStatsDataSave.SavePointPosition;
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
    private bool CheckObject<T>(T ob, string name)
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
