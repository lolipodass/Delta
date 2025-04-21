public class MoveState : PlayerBaseState
{
    public MoveState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        if (player.MoveInput == 0f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.IsHoldJumpButton)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.IsHoldCrouchButton)
        {
            stateMachine.ChangeState(player.crouchState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
    }
}