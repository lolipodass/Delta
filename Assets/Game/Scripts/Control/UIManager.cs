using UnityEngine;
using PrimeTween;

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

    public void FadeIn(GameObject ui)
    {
        // Tween
        ui.SetActive(true);
    }
    public void FadeOut(GameObject ui)
    {
        ui.SetActive(false);
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
