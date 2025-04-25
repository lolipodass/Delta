public class MoveState : PlayerBaseState
{
    public MoveState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

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
        if (player.ButtonJump && player.TimeLastJumpPressed > 0f)
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
        if (player.ButtonCrouch)
        {
            stateMachine.ChangeState(player.crouchState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
    }
}