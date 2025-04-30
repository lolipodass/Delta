using UnityEngine;

public class WallSlideState : PlayerBaseState
{
    public WallSlideState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }


    private bool isHoldOnEnterJumpButton = false;
    public override void Enter()
    {
        isHoldOnEnterJumpButton = player.ButtonJump;
        if (player.TimeLastJumpPressed > 0f && player.ButtonJump && player.HasWallJump && player.ButtonJump)
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
        if (player.ButtonJump && player.HasWallJump && !isHoldOnEnterJumpButton)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        isHoldOnEnterJumpButton = player.ButtonJump;
    }
    public override void PhysicsUpdate()
    {
        if (!player.IsTouchWall)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        Movement();
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