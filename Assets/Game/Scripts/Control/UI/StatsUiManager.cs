using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUiManager : MonoSingleton<StatsUiManager>
{
    [SerializeField] private TextMeshProUGUI statsUI;

    protected override void Awake()
    {
        base.Awake();
        if (statsUI == null)
        {
            Debug.LogError("StatsUiManager: TextMeshProUGUI reference not found!");
            enabled = false;
            return;
        }
        GameManager.Instance.playerStats.OnModifiersChanged += UpdateStats;
        UpdateStats(null);

    }

    private void UpdateStats(List<UpgradeModifier> modifiers)
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        var stats = GameManager.Instance.playerStats;
        statsUI.text = "Stats:\n" +
         $"HP: {stats.Health.MaxHealth}\n" +
            $"Damage: {stats.Stats.Damage}\n" +
            $"Speed: {stats.Stats.MaxSpeed}\n" +
            $"Dash cooldown: {stats.Stats.DashCooldown}";
    }

}
