using UnityEngine;
public class HurtState : PlayerBaseState
{
    public HurtState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override bool CanRotate => false;
    public override bool CanHurt => false;
    private float hurtTime = 0;
    public override void Enter()
    {
        Debug.Log("hurt");
        player.animator.SetTrigger("Hurt");
        hurtTime = Stats.Stats.InvincibilityAfterHit;
    }

    public override void PhysicsUpdate()
    {
        Movement();
        //add back force after hit
        hurtTime -= Time.deltaTime;
        if (hurtTime <= 0f)
        {
            player.StateMachine.ChangeState(player.idleState);
            return;
        }
    }
    public override void Exit()
    {
        player.animator.ResetTrigger("Hurt");
    }

}