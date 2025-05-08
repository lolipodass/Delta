using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [field: SerializeField] public GameObject PauseMenu { get; private set; }

    private bool isPaused = false;
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

        if (PauseMenu == null)
        {
            Debug.LogError("PauseManager requires a PauseMenu GameObject reference!");
            enabled = false;
            return;
        }

        var Input = FindAnyObjectByType<PlayerInput>();
        if (Input != null)
        {
            Debug.Log("PauseManager found PlayerInput.");
            Input.actions.FindAction("Pause").performed += PauseCallback;
        }
        else
        {
            Debug.LogError("PauseManager did not find PlayerInput!");
        }
    }

    public void OnDestroy()
    {
        var Input = FindAnyObjectByType<PlayerInput>();
        if (Input != null)
        {
            Input.actions.FindAction("Pause").performed -= PauseCallback;
        }
    }

    public void PauseCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        timeScale = Time.timeScale;
        Time.timeScale = 0;
        PauseMenu.SetActive(true);

    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = timeScale;
        PauseMenu.SetActive(false);
    }

}
