
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
    Override         // Replace base value (for example, change bool value)
}

[System.Serializable]
public struct UpgradeModifier
{
    public PlayerStatType Type;
    public ModifierType ModType;
    public float Value;
    public string SourceID;

    public UpgradeModifier(PlayerStatType type, ModifierType modType, float value, string sourceID = "")
    {
        Type = type;
        ModType = modType;
        Value = value;
        SourceID = sourceID;
    }
}