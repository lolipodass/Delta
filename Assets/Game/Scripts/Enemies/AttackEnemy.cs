using System.Collections;
using UnityEngine;

public class AttackEnemy : PatrolEnemy
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackTimeBeforeStart = 0.3f;
    [SerializeField] private float attackAnimBeforeHit = 0.3f;
    [SerializeField] private float attackHitboxActiveDuration = 0.2f;
    [SerializeField] private float attackAnimTimeBeforeEnd = 0.4f;
    [SerializeField] private Collider2D attackHitbox;

    private float attackCooldownTimer;
    private Coroutine attackCoroutine;
    protected override bool CanAttack() => true;
    protected override bool CanChase() => true;
    protected override float GetAttackRange() => attackRange;

    protected override void InitializeSpecific()
    {
        base.InitializeSpecific();
        attackCooldownTimer = attackCooldown;
    }
    protected override void HandleAttackState()
    {
        if (attackCooldownTimer <= 0f && attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(PerformAttackCoroutine());
        }
    }

    protected override void UpdateTimers()
    {
        base.UpdateTimers();

        attackCooldownTimer -= Time.deltaTime;
    }

    private IEnumerator PerformAttackCoroutine()
    {
        attackCooldownTimer = attackCooldown;

        yield return new WaitForSeconds(attackTimeBeforeStart);
        CheckPlayerRange();

        if (CheckPlayerRange()) yield break;

        if (animator != null)
        {
            animator.SetFloat("attack", Random.Range(0f, 1f));
        }

        yield return new WaitForSeconds(attackAnimBeforeHit);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
            yield return new WaitForSeconds(attackHitboxActiveDuration);
            attackHitbox.enabled = false;
        }

        yield return new WaitForSeconds(attackAnimTimeBeforeEnd);

        if (animator != null)
        {
            animator.SetFloat("attack", 0f);
        }

        if (distanceToPlayer > attackRange)
            TransitionToState(EnemyState.Chase);
        else
            TransitionToState(EnemyState.Patrol);

        attackCoroutine = null;
    }

    private bool CheckPlayerRange()
    {
        if (distanceToPlayer >= attackRange)
        {
            TransitionToState(EnemyState.Chase);
            attackCoroutine = null;
            if (animator != null)
                animator.SetFloat("attack", 0f);
            return true;
        }
        return false;
    }

    protected override void ExitCurrentState()
    {
        base.ExitCurrentState();

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            if (animator != null)
                animator.SetFloat("attack", 0f);
        }
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
