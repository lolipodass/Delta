
using Unity.Mathematics;

public enum PlayerStatType
{
    MaxHP,
    Damage,
    InvincibilityAfterHit,
    Speed,
    AirControlFactor,
    FallSpeed,
    JumpForce,
    AirJumpForce,
    ExtraJumpCount,
    HasWallSlide,
    WallSlideSpeed,
    HasWallJump,
    HasDash,
    DashTime,
    DashForce,
    DashCooldown,
}

public enum ModifierType
{
    Additive,        // Add to base value (for example, +20 damage = add 20 to damage)
    Multiplicative,  // Multiply base value (for example, x2 damage = multiply damage by 2)
    Override,        // Replace base value (select max from ovveride max or override min)
    UnlockAbility,      // Unlock ability
}

[System.Serializable]
public struct UpgradeModifier
{
    public static int MinPriority = -99;
    public static int MaxPriority = 99;
    public PlayerStatType Type;
    public ModifierType ModType;
    public float Value;
    public int Priority;
    public string SourceID;

    public UpgradeModifier(PlayerStatType type, ModifierType modType, float value, string sourceID = "", int priority = 0)
    {
        Type = type;
        ModType = modType;
        Value = value;
        SourceID = sourceID;

        Priority = math.clamp(priority, MinPriority, MaxPriority);
    }
}