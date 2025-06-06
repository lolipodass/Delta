using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class SceneLoader : PersistSingleton<SceneLoader>
{
    public event Action<string> OnSceneLoaded;
    public event Action OnGameplayUILoaded;
    public event Action OnFirstSceneLoaded;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    [SerializeField] private string gameplayUISceneName = "UI";
    public void StartNewGame(string initialRoomSceneName)
    {
        StartCoroutine(LoadInitialScenesAsync(initialRoomSceneName));
    }

    private IEnumerator LoadInitialScenesAsync(string initialRoomSceneName)
    {
        Debug.Log($"Start loading initial scene: {initialRoomSceneName}");

        yield return SceneManager.LoadSceneAsync(initialRoomSceneName, LoadSceneMode.Single);
        // loadedRoomScenes.Clear();
        // loadedRoomScenes.Add(initialRoomSceneName);

        Debug.Log($"Scene loaded: {initialRoomSceneName}");
        OnFirstSceneLoaded?.Invoke();
        OnSceneLoaded?.Invoke(initialRoomSceneName);

        if (!SceneManager.GetSceneByName(gameplayUISceneName).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(gameplayUISceneName, LoadSceneMode.Additive);
            Debug.Log($"Scene loaded: {gameplayUISceneName} (Additive)");
            OnSceneLoaded?.Invoke(gameplayUISceneName);
            OnGameplayUILoaded?.Invoke();
        }

        Debug.Log("Initial game scenes loaded.");
    }


    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public static void LoadSceneAdditive(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public static void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public static void LoadPause()
    {
        SceneManager.LoadScene("Pause");
    }
    public static void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public static void BackToMainMenu()
    {
        LoadMenu();
        // loadedRoomScenes.Clear();
        Debug.Log("Returned to Main Menu.");
    }

    public static void LoadSettings()
    {
        LoadSceneAdditive("Settings");
    }
    public static void UnloadSettings()
    {
        UnloadScene("Settings");
    }
    public static void QuitGame()
    {
        Debug.Log("Quitting game.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
