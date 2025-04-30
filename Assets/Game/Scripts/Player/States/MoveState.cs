public class MoveState : PlayerBaseState
{
    public MoveState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {

        if (player.TimeLastJumpPressed > 0f && player.ButtonJump)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
    public override void LogicUpdate()
    {
        if (player.ButtonMoveInput == 0f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.ButtonJump)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.ButtonCrouch)
        {
            stateMachine.ChangeState(player.crouchState);
            return;
        }
        if (!player.IsGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        if (player.ButtonDash && player.HasDash && player.CanDash)
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