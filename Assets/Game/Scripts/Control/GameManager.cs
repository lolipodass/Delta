using System;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player { get; private set; }

    private const string PreviousScenePathKey = "PlayFromZeroScene_PreviousScenePath";

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneLoader.LoadMenu();
            SceneLoader.Instance.OnFirstSceneLoaded += OnFirstSceneLoaded;
#if UNITY_EDITOR
            string previousScenePath = EditorPrefs.GetString(PreviousScenePathKey, "");
            if (!string.IsNullOrEmpty(previousScenePath))
            {
                SceneLoader.Instance.StartNewGame(previousScenePath);
            }
            else
            {
                SceneLoader.LoadMenu();
            }
            return;
#endif
#pragma warning disable CS0162 
            SceneLoader.LoadMenu();
#pragma warning restore CS0162
        }
        else
        {
            Destroy(gameObject);
        }
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
        Player = player;
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
}
