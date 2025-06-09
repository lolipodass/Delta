using TMPro;
using UnityEngine;

public class GameplayUIManager : MonoSingleton<GameplayUIManager>
{
    public TextMeshProUGUI HealthText;
    private PlayerStats stats;

    public void Start()
    {
        if (GameManager.Instance.playerStats != null)
            stats = GameManager.Instance.playerStats;
        else
            stats = FindAnyObjectByType<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError("HealthUIManager: PlayerStats component not found!");
            enabled = false;
            return;
        }
        if (stats.Health == null)
        {
            Debug.LogError("HealthUIManager: HealthComponent component not found!");
            enabled = false;
            return;
        }

        stats.OnStatsChanged += UpdateUI;
        UpdateUI();
    }
    public void UpdateUI()
    {
        HealthText.text = $"Health: {stats.Health.CurrentHealth}\n" +
            $"Score: {stats.Score} ";
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        stats.OnStatsChanged -= UpdateUI;
    }
}
