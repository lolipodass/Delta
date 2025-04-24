using UnityEngine;

public class FallState : PlayerBaseState
{
    public FallState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    private bool isHoldOnEnterJumpButton = false;
    public override void Enter()
    {
        isHoldOnEnterJumpButton = player.IsHoldJumpButton;
    }
    public override void LogicUpdate()
    {

        if (player.IsTouchWall && player.HasWallSlide)
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }


        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.IsHoldJumpButton && !isHoldOnEnterJumpButton && (player.LastWallTouchTime > 0 || player.ExtraJumpCountLeft > 0))
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        isHoldOnEnterJumpButton = player.IsHoldJumpButton;
    }
    public override float HorizontalSpeedMultiplayer => player.AirControlFactor;

}
