using UnityEngine;
using static GameDataSave;

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsManager Stats { get; private set; }
    [field: SerializeField] public HealthComponent Health { get; private set; }
    [SerializeField] private PlayerBaseInfo _playerConfig;
    public void Awake()
    {
        if (_playerConfig == null)
        {
            Debug.LogError("PlayerStats requires a basePlayerConfig SO reference!");
            enabled = false;
            return;
        }
        Stats = new PlayerStatsManager(_playerConfig);
        Health = GetComponent<HealthComponent>();
    }

    public PlayerStatsDataSave GetSaveData()
    {
        return new PlayerStatsDataSave()
        {
            activePlayerModifiers = Stats.Modifiers,
            HP = Health.CurrentHealth,
        };
    }
}
