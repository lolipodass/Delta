using UnityEngine;
using System.Collections;
public class IdleState : PlayerBaseState
{
    public IdleState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {

        if (player.LastJumpPressedTime > 0f && player.IsHoldJumpButton)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
    public override void LogicUpdate()
    {
        if (player.IsHoldJumpButton && player.LastJumpPressedTime > 0f)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.MoveInput != 0f)
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }
        if (player.IsHoldCrouchButton)
        {
            stateMachine.ChangeState(player.crouchState);
            return;
        }
        if (!player.IsGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
}
