using UnityEngine;
public class DeathState : PlayerBaseState
{
    public DeathState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override bool CanRotate => false;
    public override bool CanHurt => false;
    protected override string AnimationName => "death";
    public override void Enter()
    {
        Debug.Log("Enter DeathState");
        PlayAnimation();
        player.animator.SetBool("isDead", true);
    }
    public override void PhysicsUpdate()
    {
        player.rb.linearVelocityX = 0f;
    }
    public override void Exit()
    {
        player.animator.SetBool("isDead", false);
    }

}