using UnityEngine;
public class DeathState : PlayerBaseState
{
    public DeathState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override bool CanRotate => false;
    public override bool CanHurt => false;
    public override void Enter()
    {
        player.animator.Play("Base.death");
        player.animator.SetBool("isDead", true);
    }
    public override void PhysicsUpdate()
    {
        player.rb.linearVelocityX = 0f;
    }
    public override void Exit()
    {
        player.animator.SetBool("isDead", false);
        player.animator.Play("Base.idle");
    }

}