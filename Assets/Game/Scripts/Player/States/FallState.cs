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
        if (player.ButtonJump && !isHoldOnEnterJumpButton && (player.TimeLastWallTouch > 0 || player.ExtraJumpCountLeft > 0))
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        isHoldOnEnterJumpButton = player.ButtonJump;
    }
    public override float HorizontalSpeedMultiplayer => player.AirControlFactor;

}
