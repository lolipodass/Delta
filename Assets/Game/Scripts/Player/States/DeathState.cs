using UnityEngine;
public class DeathState : PlayerBaseState
{
    public DeathState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetTrigger("Death");
    }
    public override void PhysicsUpdate()
    {

    }
    public override void Exit()
    {
        player.animator.ResetTrigger("Death");
    }
}