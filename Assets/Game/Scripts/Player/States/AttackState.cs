using UnityEngine;
public class AttackState : PlayerBaseState, IAttackHandler
{
    public AttackState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override bool CanRotate => false;
    int IAttackHandler.Damage => (int)(player.StandAttackConfig.DamageMultiplier * Stats.Stats.Damage);
    Vector2 IAttackHandler.Position => player.AttackCheckPos.position;
    Vector2 IAttackHandler.Size => player.StandAttackConfig.Size;

    private float attackTime = 0;

    public override void Enter()
    {
        player.animator.SetTrigger("Attack");
        attackTime = player.StandAttackConfig.AttackTime;
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
    }
    public override void Exit()
    {
        player.animator.ResetTrigger("Attack");
    }


}