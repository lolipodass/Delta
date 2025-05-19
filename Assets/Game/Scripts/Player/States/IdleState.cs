
using UnityEngine;
using System.Collections;
public class IdleState : PlayerBaseState
{
    public IdleState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    protected override string AnimationName => "idle";

    public override void Enter()
    {
        PlayAnimation();
        if (player.TimeLastJumpPressed > 0f && player.ButtonJump)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
    public override void LogicUpdate()
    {
        if (player.ButtonJump && player.TimeLastJumpPressed > 0f)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.ButtonMoveInput != 0f)
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }
        if (player.ButtonCrouch && Stats.Stats.HasCrouch)
        {
            stateMachine.ChangeState(player.crouchState);
            return;
        }
        if (!player.IsGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        if (player.ButtonDash && Stats.Stats.HasDash && player.CanDash)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }
        if (player.ButtonAttack)
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }
    }
}
