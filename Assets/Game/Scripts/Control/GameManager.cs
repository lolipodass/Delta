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

#if UNITY_EDITOR
            string previousScenePath = EditorPrefs.GetString(PreviousScenePathKey, "");
            if (!string.IsNullOrEmpty(previousScenePath))
            {
                SceneLoader.LoadScene(previousScenePath);
            }
            else
            {
                SceneLoader.LoadMenu();
            }

#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        SceneLoader.Instance.StartNewGame("FirstLevel");
    }

    public void SetPlayer(GameObject player)
    {
        Player = player;
    }

}
