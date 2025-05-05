using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerStatsManager
{
    public PlayerBaseInfo _playerBaseInfo;

    public List<UpgradeItemData> UpgradeItems { get; private set; }

    public float CoyoteTime => _playerBaseInfo.CoyoteTime;
    public float JumpBufferTime => _playerBaseInfo.JumpBufferTime;
    public float MinimalJumpTime => _playerBaseInfo.MinimalJumpTime;
    public float WallJumpYForce => _playerBaseInfo.WallJumpYForce;
    public float WallJumpXForce => _playerBaseInfo.WallJumpXForce;

    private int _cachedMaxHP;
    private int _cachedDamage;
    private int _cachedInvincibilityAfterHit;
    private float _cachedSpeed;
    private float _cachedAirControlFactor;
    private float _cachedFallSpeed;
    private float _cachedJumpForce;
    private float _cachedAirJumpForce;
    private int _cachedExtraJumpCount;
    private bool _cachedHasWallSlide;
    private float _cachedWallSlideSpeed;
    private bool _cachedHasWallJump;
    private float _cachedWallJumpYForce;
    private float _cachedWallJumpXForce;
    private bool _cachedHasDash;
    private float _cachedDashTime;
    private float _cachedDashForce;
    private float _cachedDashCooldown;
    public int MaxHP => _cachedMaxHP;
    public int Damage => _cachedDamage;
    public int InvincibilityAfterHit => _cachedInvincibilityAfterHit;
    public float Speed => _cachedSpeed;
    public float AirControlFactor => _cachedAirControlFactor;
    public float FallSpeed => _cachedFallSpeed;
    public float JumpForce => _cachedJumpForce;
    public float AirJumpForce => _cachedAirJumpForce;
    public int ExtraJumpCount => _cachedExtraJumpCount;
    public bool HasWallSlide => _cachedHasWallSlide;
    public float WallSlideSpeed => _cachedWallSlideSpeed;
    public bool HasWallJump => _cachedHasWallJump;
    public bool HasDash => _cachedHasDash;
    public float DashTime => _cachedDashTime;
    public float DashForce => _cachedDashForce;
    public float DashCooldown => _cachedDashCooldown;

    public PlayerStatsManager(PlayerBaseInfo playerBaseInfo)
    {
        _playerBaseInfo = playerBaseInfo;
        UpgradeItems = new List<UpgradeItemData>();
    }

    public void AddUpgradeItem(UpgradeItemData upgradeItemData)
    {
        UpgradeItems.Add(upgradeItemData);
        RecalculateStats();
    }

    public void RemoveUpgradeItem(UpgradeItemData upgradeItemData)
    {
        UpgradeItems.Remove(upgradeItemData);
        RecalculateStats();
    }
    public void RecalculateStats()
    {
        _cachedMaxHP = (int)CalculateStats(PlayerStatType.MaxHP);
        _cachedDamage = (int)CalculateStats(PlayerStatType.Damage);
        _cachedInvincibilityAfterHit = (int)CalculateStats(PlayerStatType.InvincibilityAfterHit);
        _cachedSpeed = CalculateStats(PlayerStatType.Speed);
        _cachedAirControlFactor = CalculateStats(PlayerStatType.AirControlFactor);
        _cachedFallSpeed = CalculateStats(PlayerStatType.FallSpeed);
        _cachedJumpForce = CalculateStats(PlayerStatType.JumpForce);
        _cachedAirJumpForce = CalculateStats(PlayerStatType.AirJumpForce);
        _cachedExtraJumpCount = (int)CalculateStats(PlayerStatType.ExtraJumpCount);
        // _cachedHasWallSlide = CalculateStats(PlayerStatType.HasWallSlide);
        _cachedWallSlideSpeed = CalculateStats(PlayerStatType.WallSlideSpeed);
        // _cachedHasWallJump = CalculateStats(PlayerStatType.HasWallJump);
        // _cachedHasDash = CalculateStats(PlayerStatType.HasDash);
        _cachedDashTime = CalculateStats(PlayerStatType.DashTime);
        _cachedDashForce = CalculateStats(PlayerStatType.DashForce);
        _cachedDashCooldown = CalculateStats(PlayerStatType.DashCooldown);
    }
    private float CalculateStats(PlayerStatType type)
    {
        float value = 0;
        foreach (var upgradeItem in UpgradeItems)
        {
            foreach (var modifier in upgradeItem.modifiersToApply)
            {
                if (modifier.Type == type)
                {
                    switch (modifier.ModType)
                    {
                        case ModifierType.Additive:
                            value += modifier.Value;
                            break;
                        case ModifierType.Multiplicative:
                            value *= modifier.Value;
                            break;
                        case ModifierType.Override:
                            value = modifier.Value;
                            break;
                    }
                }
            }
        }
        return value;
    }
    public void SetLoadedModifiers(List<UpgradeItemData> loadedModifiers)
    {
        UpgradeItems = loadedModifiers ?? new List<UpgradeItemData>();
        RecalculateStats();
    }
}
