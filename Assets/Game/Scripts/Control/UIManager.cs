using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [field: SerializeField] public GameObject PauseMenu { get; private set; }
    [field: SerializeField] public GameObject GameplayUI { get; private set; }
    [field: SerializeField] public GameObject DeathUI { get; private set; }
    [field: SerializeField] public GameObject SaveUI { get; private set; }
    public void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!CheckObject(PauseMenu, "PauseMenu")) return;
        if (!CheckObject(GameplayUI, "GameplayUI")) return;
        if (!CheckObject(DeathUI, "DeathUI")) return;
        if (!CheckObject(SaveUI, "SaveUI")) return;
    }

    public void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
    }
    public void HidePauseMenu()
    {
        PauseMenu.SetActive(false);
    }
    public void ShowGameplayUI()
    {
        GameplayUI.SetActive(true);
    }
    public void HideGameplayUI()
    {
        GameplayUI.SetActive(false);
    }
    public void ShowDeathUI()
    {
        DeathUI.SetActive(true);
    }
    public void HideDeathUI()
    {
        DeathUI.SetActive(false);
    }
    public void ShowSaveUI()
    {
        SaveUI.SetActive(true);
    }
    public void HideSaveUI()
    {
        SaveUI.SetActive(false);
    }
    private bool CheckObject(GameObject ui, string name)
    {
        if (ui == null)
        {
            Debug.LogError($"UIManager requires a {name} GameObject reference!");
            enabled = false;
            return false;
        }
        return true;
    }
}
