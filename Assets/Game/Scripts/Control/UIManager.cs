using UnityEngine;
using PrimeTween;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [field: SerializeField] public GameObject PauseMenu { get; private set; }
    [field: SerializeField] public GameObject GameplayUI { get; private set; }
    [field: SerializeField] public GameObject DeathUI { get; private set; }
    [field: SerializeField] public GameObject SaveUI { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        if (!CheckObject(PauseMenu, "PauseMenu")) return;
        if (!CheckObject(GameplayUI, "GameplayUI")) return;
        if (!CheckObject(DeathUI, "DeathUI")) return;
        if (!CheckObject(SaveUI, "SaveUI")) return;
    }

    public void ShowSaveUI()
    {
        SaveUI.SetActive(true);
        if (SaveUI.TryGetComponent<TextMeshProUGUI>(out var text))
        {
            Tween.Alpha(text, 1f, 0.5f);
        }
    }
    public void HideSaveUI()
    {
        if (SaveUI.TryGetComponent<TextMeshProUGUI>(out var text))
        {
            Tween.Alpha(text, 0f, 0.5f).OnComplete(target: SaveUI, ui => ui.SetActive(false));
        }
    }

    public void ShowDeathUI()
    {
        DeathUI.SetActive(true);
        var image = DeathUI.GetComponentInChildren<Image>();
        if (image != null)
        {
            Tween.Alpha(image, 1f, 1f, Ease.InOutCubic);
        }
    }
    public void HideDeathUI()
    {
        var image = DeathUI.GetComponentInChildren<Image>();
        if (image != null)
        {
            Tween.Alpha(image, 0f, 0.8f, Ease.InOutCubic).OnComplete(target: DeathUI, ui => ui.SetActive(false));
        }
    }
    public void ShowGameplayUI()
    {
        GameplayUI.SetActive(true);
    }
    public void HideGameplayUI()
    {
        GameplayUI.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
    }
    public void HidePauseMenu()
    {
        PauseMenu.SetActive(false);
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
