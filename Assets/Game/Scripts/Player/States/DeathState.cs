using UnityEngine;
public class DeathState : PlayerBaseState
{
    public DeathState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override bool CanRotate => false;
    public override bool CanHurt => false;
    private float deathTime = 0f;
    public override void Enter()
    {
        deathTime = player.PlayerStats.Stats.DeathTime;
        player.animator.SetTrigger("Death");
    }
    public override void PhysicsUpdate()
    {
        deathTime -= Time.deltaTime;
        if (deathTime <= 0f)
        {
            player.StateMachine.ChangeState(player.idleState);
            return;
        }
        player.rb.linearVelocityX = 0f;
    }
    public override void Exit()
    {
        player.animator.ResetTrigger("Death");
    }

}