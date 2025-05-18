using UnityEngine;

public class FallState : PlayerBaseState
{
    public FallState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    private bool isHoldOnEnterJumpButton = false;
    public override void Enter()
    {
        isHoldOnEnterJumpButton = player.ButtonJump;
    }
    public override void LogicUpdate()
    {

        if (player.IsTouchWall && Stats.Stats.HasWallSlide)
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.ButtonJump && CanJump())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        isHoldOnEnterJumpButton = player.ButtonJump;
    }
    private bool CanJump()
    {
        return !isHoldOnEnterJumpButton && ((player.TimeLastWallTouch > 0 && Stats.Stats.HasWallJump) || player.ExtraJumpCountLeft > 0);
    }
    public override float HorizontalSpeedMultiplayer => Stats.Stats.AirControlFactor;

}
