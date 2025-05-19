public class MoveState : PlayerBaseState
{
    public MoveState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    private bool isHoldOnEnterJumpButton = false;
    public override void Enter()
    {

        isHoldOnEnterJumpButton = player.ButtonJump;
        if (player.TimeLastJumpPressed > 0f && player.ButtonJump)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
    public override void LogicUpdate()
    {
        if (player.ButtonMoveInput == 0f && XVelocity < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.ButtonJump && player.TimeLastJumpPressed > 0f && !isHoldOnEnterJumpButton)
        {
            stateMachine.ChangeState(player.jumpState);
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
        isHoldOnEnterJumpButton = false;
    }

}