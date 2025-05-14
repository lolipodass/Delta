using UnityEngine;

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
    public void GoToMenu()
    {
        FileSaveManager.Instance.SaveGame();
        SceneLoader.LoadMenu();
    }
}
