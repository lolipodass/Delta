using UnityEngine;

public class JumpState : PlayerBaseState
{
    public JumpState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        if (player.IsGrounded || player.lastGroundedTime > 0f)
        {
            player.GroundJump();
            return;
        }
        if (player.IsTouchWall)
        {
            player.WallJump();
            return;
        }
        else
        {
            player.AirJump();
            return;
        }
    }
    public override void LogicUpdate()
    {
        if (!player.IsHoldJumpButton)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        if (player.YVelocity <= 0f)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        // if (player.LastJumpPressedTime >= player.JumpBufferTime && player.IsHoldJumpButton
        // && (player.IsGrounded || player.IsTouchWall))
        // {
        //     Debug.Log("redundant jump");
        //     stateMachine.ChangeState(player.jumpState);
        //     return;
        // }
    }

    public override float HorizontalSpeedMultiplayer => player.AirControlFactor;

}
