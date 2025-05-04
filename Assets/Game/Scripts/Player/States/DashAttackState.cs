using UnityEngine;
public class DashAttackState : PlayerBaseState, IAttackHandler
{
    public DashAttackState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    private float attackTime = 0;
    public override bool CanRotate => false;
    public override bool CanHurt => false;
    int IAttackHandler.Damage => (int)(player.DashAttackConfig.DamageMultiplier * player.PlayerConfig.Damage);
    Vector2 IAttackHandler.Position => player.DashAttackCheckPos.position;
    Vector2 IAttackHandler.Size => player.DashAttackConfig.Size;

    public override void Enter()
    {
        player.animator.SetTrigger("Attack");
        attackTime = player.DashAttackConfig.AttackTime;
    }
    public override void PhysicsUpdate()
    {
        float targetXVelocity = 0;
        player.rb.linearVelocityX = Mathf.Lerp(player.rb.linearVelocity.x, targetXVelocity, 4f * Time.fixedDeltaTime);
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