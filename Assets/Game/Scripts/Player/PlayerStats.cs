using UnityEngine;
using static GameDataSave;
using static GameDataSave.PlayerStatsDataSave;

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsManager Stats { get; private set; }
    [field: SerializeField] public HealthComponent Health { get; private set; }

    [field: SerializeField] public SavePointInfo LastSavePoint { get; private set; }
    [field: SerializeField] public SavePointInfo SavePoint { get; private set; }
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

    public void SetSavePoint(SavePoint savePoint)
    {
        SavePoint = new(savePoint);
    }
    public void SetSavePoint(string savePointName, Vector3 position)
    {
        SavePoint = new(savePointName, position);
    }
    public void SetSavePoint(SavePointInfo savePointInfo)
    {
        SavePoint = savePointInfo;
    }

    public void Restart()
    {
        Health.ResetHealth();
    }
    public void SetLastSavePoint(SavePoint savePoint)
    {
        if (savePoint == null)
            LastSavePoint = null;
        else
            LastSavePoint = new(savePoint);
    }

    public PlayerStatsDataSave GetSaveData()
    {
        return new PlayerStatsDataSave()
        {
            activePlayerModifiers = Stats.Modifiers,
            HP = Health.MaxHealth,
            savePoint = SavePoint
        };
    }
    public void SetSavedData(PlayerStatsDataSave savedData)
    {
        Stats.SetLoadedModifiers(savedData.activePlayerModifiers);
        Health.SetMaxHealth(savedData.HP);
        SetSavePoint(savedData.savePoint);
    }
}
