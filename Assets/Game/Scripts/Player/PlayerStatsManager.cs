using System.Collections.Generic;

[System.Serializable]
public class PlayerStatsManager
{
    private PlayerBaseInfo _playerBaseInfo;
    public PlayerBaseInfo PlayerBaseInfo => _playerBaseInfo;

    private List<UpgradeModifier> _modifiers;
    public List<UpgradeModifier> Modifiers => _modifiers;

    public float CoyoteTime => _playerBaseInfo.CoyoteTime;
    public float JumpBufferTime => _playerBaseInfo.JumpBufferTime;
    public float MinimalJumpTime => _playerBaseInfo.MinimalJumpTime;
    public float WallJumpYForce => _playerBaseInfo.WallJumpYForce;
    public float WallJumpXForce => _playerBaseInfo.WallJumpXForce;

    private int _cachedMaxHP;
    private int _cachedDamage;
    private float _cachedInvincibilityAfterHit;
    private float _cachedSpeed;
    private float _cachedAirControlFactor;
    private float _cachedFallSpeed;
    private float _cachedJumpForce;
    private float _cachedAirJumpForce;
    private int _cachedExtraJumpCount;
    private float _cachedWallSlideSpeed;
    private float _cachedDashTime;
    private float _cachedDashForce;
    private float _cachedDashCooldown;

    private bool _cachedHasWallSlide;
    private bool _cachedHasWallJump;
    private bool _cachedHasDash;
    public int MaxHP => _cachedMaxHP;
    public int Damage => _cachedDamage;
    public float InvincibilityAfterHit => _cachedInvincibilityAfterHit;
    public float MaxSpeed => _cachedSpeed;
    public float AirControlFactor => _cachedAirControlFactor;
    public float MaxFallSpeed => _cachedFallSpeed;
    public float JumpForce => _cachedJumpForce;
    public float AirJumpForce => _cachedAirJumpForce;
    public int ExtraJumpCount => _cachedExtraJumpCount;
    public float WallSlideSpeed => _cachedWallSlideSpeed;
    public float DashTime => _cachedDashTime;
    public float DashForce => _cachedDashForce;
    public float DashCooldown => _cachedDashCooldown;

    public bool HasWallSlide => _cachedHasWallSlide;
    public bool HasWallJump => _cachedHasWallJump;
    public bool HasDash => _cachedHasDash;

    public PlayerStatsManager(PlayerBaseInfo playerBaseInfo)
    {
        _playerBaseInfo = playerBaseInfo;
        _modifiers = new List<UpgradeModifier>();
    }

    public void SetPlayerBaseInfo(PlayerBaseInfo playerBaseInfo)
    {
        _playerBaseInfo = playerBaseInfo;
        RecalculateStats();
    }

    public void AddModifier(UpgradeModifier upgradeItem)
    {
        _modifiers.Add(upgradeItem);
        RecalculateStats();
    }
    public void AddModifiers(List<UpgradeModifier> upgradeItems)
    {
        _modifiers.AddRange(upgradeItems);
        RecalculateStats();
    }

    public void RemoveModifiers(string upgradeItemID)
    {
        _modifiers.RemoveAll(x => x.SourceID == upgradeItemID);
        RecalculateStats();
    }
    public void RecalculateStats()
    {
        _cachedHasWallSlide = CalculateBool(PlayerStatType.HasWallSlide);
        _cachedHasWallJump = CalculateBool(PlayerStatType.HasWallJump);
        _cachedHasDash = CalculateBool(PlayerStatType.HasDash);

        _cachedMaxHP = (int)CalculateStats(PlayerStatType.MaxHP);
        _cachedDamage = (int)CalculateStats(PlayerStatType.Damage);
        _cachedInvincibilityAfterHit = CalculateStats(PlayerStatType.InvincibilityAfterHit);
        _cachedSpeed = CalculateStats(PlayerStatType.Speed);
        _cachedAirControlFactor = CalculateStats(PlayerStatType.AirControlFactor);
        _cachedFallSpeed = CalculateStats(PlayerStatType.FallSpeed);
        _cachedJumpForce = CalculateStats(PlayerStatType.JumpForce);
        _cachedAirJumpForce = CalculateStats(PlayerStatType.AirJumpForce);
        _cachedExtraJumpCount = (int)CalculateStats(PlayerStatType.ExtraJumpCount);
        _cachedWallSlideSpeed = CalculateStats(PlayerStatType.WallSlideSpeed);
        _cachedDashTime = CalculateStats(PlayerStatType.DashTime);
        _cachedDashForce = CalculateStats(PlayerStatType.DashForce);
        _cachedDashCooldown = CalculateStats(PlayerStatType.DashCooldown);
    }
    private float CalculateStats(PlayerStatType type)
    {
        float value = _playerBaseInfo.GetNumericStat(type);
        float additiveBonus = 0;
        float multiplicativeBonus = 1;
        float overrideValue = 0;
        int priorityOverride = UpgradeModifier.MinPriority;
        foreach (var modifier in _modifiers)
        {
            if (modifier.Type == type)
            {
                switch (modifier.ModType)
                {
                    case ModifierType.Additive:
                        additiveBonus += modifier.Value;
                        break;
                    case ModifierType.Multiplicative:
                        multiplicativeBonus *= modifier.Value;
                        break;
                    case ModifierType.Override:
                        if (priorityOverride < modifier.Priority)
                            overrideValue = modifier.Value;
                        break;
                }
            }
        }
        if (priorityOverride > UpgradeModifier.MinPriority)
            return overrideValue;
        return value * multiplicativeBonus + additiveBonus;
    }
    private bool CalculateBool(PlayerStatType type)
    {
        bool value = _playerBaseInfo.GetBooleanStat(type);
        int priorityOverride = UpgradeModifier.MinPriority;
        bool overrideValue = false;
        foreach (var modifier in _modifiers)
        {
            if (modifier.Type == type)
            {
                switch (modifier.ModType)
                {
                    case ModifierType.UnlockAbility:
                        if (priorityOverride < modifier.Priority)
                        {
                            priorityOverride = modifier.Priority;
                            overrideValue = modifier.Value > -1;
                        }
                        break;
                }
            }
        }
        return priorityOverride > UpgradeModifier.MinPriority ? overrideValue : value;
    }

    public void SetLoadedModifiers(List<UpgradeModifier> loadedModifiers)
    {
        _modifiers = loadedModifiers ?? new List<UpgradeModifier>();
        RecalculateStats();
    }

}
