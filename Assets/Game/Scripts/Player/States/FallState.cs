using UnityEngine;

public class FallState : PlayerBaseState
{
    public FallState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        //idk if this is needed
        player.ReleaseJumpButton();
        if (player.YVelocity > 0f && player.JumpTime > player.MinimalJumpTime && player.JumpTime < PlayerMovementSFM.maxJumpTime)
        {
            Debug.Log("1");
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
        if (player.YVelocity > 0f && player.JumpTime > player.MinimalJumpTime && player.JumpTime < PlayerMovementSFM.maxJumpTime)
        {
            Debug.Log("2");
            player.JumpCut();
            return;
        }
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.IsHoldJumpButton && (player.LastWallTouchTime > 0 || player.ExtraJumpCountLeft > 0))
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

    }
    public override float HorizontalSpeedMultiplayer => player.AirControlFactor;

}
