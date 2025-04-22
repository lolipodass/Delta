public class MoveState : PlayerBaseState
{
    public MoveState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

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
        if (player.MoveInput == 0f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.IsHoldJumpButton && player.LastJumpPressedTime > 0f)
        {
            stateMachine.ChangeState(player.jumpState);
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

    public override void PhysicsUpdate()
    {
    }
}