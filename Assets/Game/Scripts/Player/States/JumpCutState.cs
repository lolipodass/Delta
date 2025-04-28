using UnityEngine;

public class JumpCutState : PlayerBaseState
{
    public JumpCutState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        if (YVelocity > 0f && player.TimeJump > player.MinimalJumpTime && player.TimeJump < PlayerMovementSFM.maxJumpTime)
        {
            player.rb.linearVelocityY /= 2;
            // return;
        }
        if (YVelocity <= 0f)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
}