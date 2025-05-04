using UnityEngine;

public class JumpState : PlayerBaseState
{
    public JumpState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        if (player.IsGrounded || player.TimeLastGrounded > 0f)
        {
            player.GroundJump();
            return;
        }
        if (player.IsTouchWall || player.TimeLastWallTouch > 0f)
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
        if (!player.ButtonJump)
        {
            stateMachine.ChangeState(player.jumpCutState);
            return;
        }
        if (YVelocity < 0f)
        {
            player.ReleaseJumpButton();
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

    public override float HorizontalSpeedMultiplayer => player.PlayerConfig.AirControlFactor;

}
