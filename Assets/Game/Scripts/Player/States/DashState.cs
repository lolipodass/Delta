using UnityEngine;

public class DashState : PlayerBaseState
{
    public DashState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        dashTime = player.DashTime;
    }

    private float dashTime = 0;
    public override void PhysicsUpdate()
    {
        dashTime -= Time.deltaTime;
        if (dashTime <= 0f)
        {
            stateMachine.ChangeState(player.idleState);
        }
        player.Dash();
    }

    // public override float HorizontalSpeedMultiplayer => 0.5f;
}