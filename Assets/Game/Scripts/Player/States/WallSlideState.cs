using UnityEngine;

public class WallSlideState : PlayerBaseState
{
    public WallSlideState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }


    private bool isHoldOnEnterJumpButton = false;
    public override void Enter()
    {
        isHoldOnEnterJumpButton = player.IsHoldJumpButton;
        if (player.LastJumpPressedTime > 0f && player.IsHoldJumpButton && player.HasWallJump && player.IsHoldJumpButton)
        {
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
        if (player.IsHoldJumpButton && player.HasWallJump && !isHoldOnEnterJumpButton)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        isHoldOnEnterJumpButton = player.IsHoldJumpButton;
    }
    public override void PhysicsUpdate()
    {
        if (!player.IsTouchWall)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
    public override float VerticalSpeedMultiplayer
    {
        get
        {
            if (!player.IsTouchBackWall)
            {
                return player.WallSlideSpeed;
            }
            return base.VerticalSpeedMultiplayer;
        }
    }
}