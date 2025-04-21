using UnityEngine;

public class FallState : PlayerBaseState
{
    public FallState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.ReleaseJumpButton();
        if (player.YVelocity > 0f && player.jumpTime > player.MinimalJumpTime && player.jumpTime < PlayerMovementSFM.maxJumpTime)
        {
            player.JumpCut();
        }
    }
    public override void LogicUpdate()
    {
        if (player.IsTouchWall && player.HasWallSlide)
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
        if (player.YVelocity > 0f && player.jumpTime > player.MinimalJumpTime && player.jumpTime < PlayerMovementSFM.maxJumpTime)
        {
            player.JumpCut();
            return;
        }
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.IsHoldJumpButton && player.ExtraJumpCountLeft > 0)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

    }
    public override float HorizontalSpeedMultiplayer => player.AirControlFactor;

}
