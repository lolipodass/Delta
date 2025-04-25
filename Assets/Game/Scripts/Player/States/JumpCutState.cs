public class JumpCutState : PlayerBaseState
{
    public JumpCutState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
    }
    public override void LogicUpdate()
    {
        if (player.YVelocity > 0f && player.TimeJump > player.MinimalJumpTime && player.TimeJump < PlayerMovementSFM.maxJumpTime)
        {
            player.JumpCut();
            // return;
        }
        if (player.YVelocity < 0f)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
}