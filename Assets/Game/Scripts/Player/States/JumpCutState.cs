using UnityEngine;

public class JumpCutState : PlayerBaseState
{
    public JumpCutState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    protected override string AnimationName => "uptofall";
    public override void Enter()
    {
        PlayAnimation();
    }
    public override void LogicUpdate()
    {
        if (player.ButtonJump && CanJump())
        {
            stateMachine.ChangeState(player.jumpState);
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
        if (YVelocity > 0f && player.TimeJump > Stats.Stats.MinimalJumpTime && player.TimeJump < PlayerSFM.maxJumpTime)
        {
            player.rb.linearVelocityY /= 2;
            return;
        }
        if (YVelocity < 0f)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
    private bool CanJump()
    {
        return
        (player.TimeLastWallTouch > 0 && Stats.Stats.HasWallJump) || player.ExtraJumpCountLeft > 0
        || player.TimeLastGrounded > 0;
    }
}