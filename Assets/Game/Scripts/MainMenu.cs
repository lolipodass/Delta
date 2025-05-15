using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void PlayNewGamePressed()
    {
        GameManager.Instance.CreateNewGame();
    }
    public void ContinuePressed()
    {
        GameManager.Instance.StartGame();
    }
    public void SettingsPressed()
    {
        SceneLoader.LoadSettings();
    }
    public void ExitPressed()
    {
        SceneLoader.QuitGame();
    }
}
