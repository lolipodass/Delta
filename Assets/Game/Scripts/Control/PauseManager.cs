using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoSingleton<PauseManager>
{

    private bool isPaused = false;
    private float timeScale;

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void Start()
    {
        if (GameManager.Instance.playerInput != null)
        {
            var pauseAction = GameManager.Instance.playerInput.actions.FindAction("Pause");
            if (pauseAction != null)
            {
                pauseAction.performed -= PauseCallback;
                pauseAction.performed += PauseCallback;
            }
        }
    }

    public void PauseCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameManager.Instance.Player == null)
                return;

            if (GameManager.Instance.playerStats.LastSavePoint != null)
                SaveManager.Instance.SaveGame();
            else
                TogglePause();
        }
    }
    public void PauseGame()
    {
        if (isPaused)
            return;


        isPaused = true;
        timeScale = Time.timeScale;
        Time.timeScale = 0;
        UIManager.Instance.ShowPauseMenu();


    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;


        isPaused = false;
        Time.timeScale = timeScale;
        UIManager.Instance.HidePauseMenu();
    }
    public void LoadSettings()
    {
        SceneLoader.LoadSettings();
    }
    public void GoToMenu()
    {
        FileSaveManager.Instance.SaveGame();
        SceneLoader.LoadMenu();
    }
    protected override void OnDestroy()
    {
        if (GameManager.Instance.playerInput != null)
        {
            GameManager.Instance.playerInput.actions.FindAction("Pause").performed -= PauseCallback;
        }
        base.OnDestroy();
    }
}
