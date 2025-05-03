using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    private float timeScale;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PauseGame()
    {
        Debug.Log("Paused game.");
        timeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Debug.Log("Resumed game.");
        Time.timeScale = timeScale;
    }

}
