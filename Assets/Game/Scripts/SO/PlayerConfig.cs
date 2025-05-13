using UnityEngine;

[CreateAssetMenu(fileName = "MovementInfo", menuName = "Scriptable Objects/PlayerBaseInfo")]
public class PlayerBaseInfo : ScriptableObject
{
    public float Damage;
    public float InvincibilityAfterHit;
    public float MaxSpeed;
    public float AirControlFactor;
    public float MaxFallSpeed;
    [Header("Jump")]
    public float jumpForce;
    public float airJumpForce;
    public int ExtraJumpCount;
    public float CoyoteTime;
    public float JumpBufferTime;
    public float MinimalJumpTime;
    [Header("Wall")]
    public bool HasWallSlide;
    public float WallSlideSpeed;
    public bool HasWallJump;
    public float WallJumpYForce;
    public float WallJumpXForce;
    [Header("Dash")]
    public bool HasDash;
    public float DashTime;
    public float DashForce;
    public float DashCooldown;

    public float GetNumericStat(PlayerStatType type)
    {
        return type switch
        {
            PlayerStatType.MaxHP => 100,
            PlayerStatType.Damage => Damage,
            PlayerStatType.InvincibilityAfterHit => InvincibilityAfterHit,
            PlayerStatType.Speed => MaxSpeed,
            PlayerStatType.AirControlFactor => AirControlFactor,
            PlayerStatType.FallSpeed => MaxFallSpeed,
            PlayerStatType.JumpForce => jumpForce,
            PlayerStatType.AirJumpForce => airJumpForce,
            PlayerStatType.ExtraJumpCount => ExtraJumpCount,
            PlayerStatType.WallSlideSpeed => WallSlideSpeed,
            PlayerStatType.DashTime => DashTime,
            PlayerStatType.DashForce => DashForce,
            PlayerStatType.DashCooldown => DashCooldown,
            _ => 0,
        };
    }
    public bool GetBooleanStat(PlayerStatType type)
    {
        return type switch
        {
            PlayerStatType.HasWallSlide => HasWallSlide,
            PlayerStatType.HasWallJump => HasWallJump,
            PlayerStatType.HasDash => HasDash,
            _ => false,
        };
    }
}