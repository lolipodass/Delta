using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameDataSave;
using static GameDataSave.PlayerStatsDataSave;

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsManager Stats { get; private set; }
    [field: SerializeField] public HealthComponent Health { get; private set; }
    [SerializeField] private ObfuscatedInt _score;
    [SerializeField] private ObfuscatedInt _deaths;
    public int Score { get { return _score; } private set { _score = value; } }
    public int Deaths { get { return _deaths; } private set { _deaths = value; } }

    [field: SerializeField] public SavePointInfo LastSavePoint { get; private set; }
    [field: SerializeField] public SavePointInfo SavePoint { get; private set; }

    public event Action<List<UpgradeModifier>> OnModifiersChanged;
    public event Action OnStatsChanged;
    [SerializeField]
    private PlayerBaseInfo _playerConfig;
    public void Awake()
    {
        if (_playerConfig == null)
        {
            Debug.LogError("PlayerStats requires a basePlayerConfig SO reference!");
            enabled = false;
            return;
        }
        LastSavePoint = null;
        Stats = new PlayerStatsManager(_playerConfig);
        Health = GetComponent<HealthComponent>();
        InventoryManager.Instance.OnInventoryChanged += GetInfoFromInventory;
        Health.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(int _, Vector2 __) => OnStatsChanged?.Invoke();

    private void GetInfoFromInventory()
    {
        var modifiers = InventoryManager.Instance.Inventory.Where(x => x.modifiersToApply.Count > 0).SelectMany(x => x.modifiersToApply).ToList();
        Stats.SetLoadedModifiers(modifiers);
        OnModifiersChanged?.Invoke(modifiers);
        OnStatsChanged?.Invoke();
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
            HP = Health.MaxHealth,
            score = Score,
            deaths = Deaths,
            savePoint = SavePoint
        };
    }
    public void SetSavedData(PlayerStatsDataSave savedData)
    {
        Health.SetMaxHealth(savedData.HP);
        Score = savedData.score;
        Deaths = savedData.deaths;
        SetSavePoint(savedData.savePoint);
        GetInfoFromInventory();
    }

    public void AddScore(int score)
    {
        Score += score;
        OnStatsChanged?.Invoke();
    }
    public void AddDeath()
    {
        Deaths++;
        OnStatsChanged?.Invoke();
    }

}
