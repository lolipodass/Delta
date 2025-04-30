using UnityEngine;
public class AttackState : PlayerBaseState
{
    public AttackState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    private float attackTime = 0;
    // private bool releaseButton = false;

    public override void Enter()
    {
        player.animator.SetTrigger("Attack"); attackTime = player.AttackTime;
    }
    public override void PhysicsUpdate()
    {
        player.rb.linearVelocityX = 0;
        attackTime -= Time.deltaTime;
        if (attackTime <= 0f)
        {
            player.StateMachine.ChangeState(player.idleState);
            return;
        }
        // if (releaseButton && player.ButtonAttack)
        // {
        //     player.StateMachine.ChangeState(player.attackState);
        //     Debug.Log("doubleAttack");
        //     return;
        // }
        // if (!player.ButtonAttack)
        //     releaseButton = true;
    }
    public override void Exit()
    {
        player.animator.ResetTrigger("Attack");
    }
    public override bool CanRotate => false;
}