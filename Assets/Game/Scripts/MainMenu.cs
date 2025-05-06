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
        Debug.Log("Settings pressed");
    }
    public void ExitPressed()
    {
        SceneLoader.QuitGame();
    }
}
