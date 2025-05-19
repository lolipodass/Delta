using TMPro;
using UnityEngine;

public class GameplayUIManager : MonoSingleton<GameplayUIManager>
{
    public TextMeshProUGUI HealthText;
    private HealthComponent _healthComponent;

    public void Start()
    {
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("HealthUIManager: PlayerStats component not found!");
            enabled = false;
            return;
        }
        if (playerStats.Health == null)
        {
            Debug.LogError("HealthUIManager: HealthComponent component not found!");
            enabled = false;
            return;
        }
        _healthComponent = playerStats.Health;
        _healthComponent.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(_healthComponent.CurrentHealth);
    }
    public void UpdateHealthUI(int hp)
    {
        HealthText.text = "HP: " + hp;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_healthComponent != null)
        {
            _healthComponent.OnHealthChanged -= UpdateHealthUI;
        }
    }
}
