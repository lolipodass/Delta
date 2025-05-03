using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneLoader.LoadMenu();
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
