using UnityEngine;

[CreateAssetMenu(fileName = "MovementInfo", menuName = "Scriptable Objects/PlayerBaseInfo")]
public class PlayerBaseInfo : ScriptableObject
{
    public float Damage;
    public float InvincibilityAfterHit;
    public float MaxSpeed;
    public float AirControlFactor;
    public float MaxFallSpeed;
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

}
