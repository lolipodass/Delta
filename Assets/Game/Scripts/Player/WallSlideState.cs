using UnityEngine;

public class WallSlideState : PlayerBaseState
{
    public WallSlideState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        if (player.LastJumpPressedTime > 0f && player.HasWallJump && player.IsHoldJumpButton)
        {
            player.ReleaseJumpButton();
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
    public override void LogicUpdate()
    {
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (!player.IsTouchWall)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        if (player.IsHoldJumpButton && player.HasWallJump)
        {
            player.ReleaseJumpButton();
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
    public override float VerticalSpeedMultiplayer => player.WallSlideSpeed;
}