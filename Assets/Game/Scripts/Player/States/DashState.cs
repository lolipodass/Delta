using UnityEngine;

public class DashState : PlayerBaseState
{
    public DashState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        dashTime = player.DashTime;
    }
    private float gravity;
    public override void Enter()
    {
        dashTime = player.DashTime;
        player.AnimationDash = true;
        gravity = player.rb.gravityScale;
        player.rb.gravityScale = 0;
    }

    private float dashTime = 0;

    public override void PhysicsUpdate()
    {
        dashTime -= Time.deltaTime;
        if (dashTime <= 0f)
        {
            player.timeDashCooldown = player.DashCooldown;
            stateMachine.ChangeState(player.idleState);
        }
        player.rb.linearVelocityX = player.isFacingRight ? player.DashForce : -player.DashForce;
    }

    public override void Exit()
    {
        player.AnimationDash = false;
        player.rb.gravityScale = gravity;
    }
    public override bool CanRotate => false;
    // public override float HorizontalSpeedMultiplayer => 0.5f;
}
