using UnityEngine;

public class DashState : PlayerBaseState
{
    public DashState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        dashTime = Stats.Stats.DashTime;
    }
    public override bool CanRotate => false;
    public override bool CanHurt => false;
    private float gravity;
    public override void Enter()
    {
        dashTime = Stats.Stats.DashTime;
        player.AnimationDash = true;
        gravity = player.rb.gravityScale;
        player.rb.gravityScale = 0f;
        player.rb.linearVelocityY = 0f;
    }

    private float dashTime = 0;

    public override void PhysicsUpdate()
    {
        dashTime -= Time.deltaTime;
        if (dashTime <= 0f)
        {
            player.timeDashCooldown = Stats.Stats.DashCooldown;
            stateMachine.ChangeState(player.idleState);
        }
        player.rb.linearVelocityX = player.isFacingRight ? Stats.Stats.DashForce : -Stats.Stats.DashForce;
        if (player.ButtonAttack && player.IsGrounded)
        {
            stateMachine.ChangeState(player.dashAttackState);
            return;
        }
    }

    public override void Exit()
    {
        player.AnimationDash = false;
        player.rb.gravityScale = gravity;
    }


    // public override float HorizontalSpeedMultiplayer => 0.5f;
}
