using UnityEngine;

public class JumpState : PlayerBaseState
{
    public JumpState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    protected override string AnimationName => "jump";
    private int skipFirstFrames = 5;
    public override void Enter()
    {
        skipFirstFrames = 10;
        PlayAnimation();
        if (player.IsGrounded || player.TimeLastGrounded > 0f)
        {
            player.GroundJump();
            return;
        }
        if ((player.IsTouchFrontWall || player.TimeLastWallTouch > 0f) && Stats.Stats.HasWallJump)
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
        if (skipFirstFrames > 0)
        {
            skipFirstFrames--;
            return;
        }
        if (player.ButtonDash && Stats.Stats.HasDash && player.CanDash)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }

    }
    public override void PhysicsUpdate()
    {
        Movement();

        if (skipFirstFrames > 0)
        {
            skipFirstFrames--;
            return;
        }
        if (player.IsGrounded)
        {
            player.ReleaseJumpButton();
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (YVelocity <= 0f)
        {
            player.ReleaseJumpButton();
            stateMachine.ChangeState(player.upToFallState);
            return;
        }

    }

    public override float HorizontalSpeedMultiplayer => Stats.Stats.AirControlFactor;

}
