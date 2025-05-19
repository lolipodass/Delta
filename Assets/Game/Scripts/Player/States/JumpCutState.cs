using UnityEngine;

public class JumpCutState : PlayerBaseState
{
    public JumpCutState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    protected override string AnimationName => "uptofall";
    public override void Enter()
    {
        PlayAnimation();
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
}