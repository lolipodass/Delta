using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void PlayPressed()
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
