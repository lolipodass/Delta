using UnityEngine;

public class FlyEnemy : BaseEnemy
{
    private float originalGravityScale;

    protected override void InitializeSpecific()
    {
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f; // Disable gravity
    }

    protected override void Move()
    {
        if (currentPath == null)
        {
            moveDirection = 0;
            return;
        }

        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            moveDirection = 0;
            return;
        }

        Vector2 direction = (Vector2)currentPath.vectorPath[currentWaypoint] - rb.position;

        float currentSpeed = (currentState == EnemyState.Chase) ? chaseSpeed : patrolSpeed;
        moveDirection = direction.x;

        rb.linearVelocity = direction * currentSpeed;

        UpdatePathPoint();
    }


    protected override bool CanAttack() => false;
    protected override float GetAttackRange() => 0f;
    protected override bool IsGrounded() => false;

    protected override void HandleAttackState()
    {
        // Flying scouts don't attack, transition to chase
        TransitionToState(EnemyState.Chase);
    }

    protected override void EnterNewState(EnemyState newState)
    {
        base.EnterNewState(newState);

        if (newState == EnemyState.Stunned || newState == EnemyState.Dead)
        {
            rb.linearVelocityY = 0;
        }
    }

    protected override void OnDestroySpecific()
    {
        rb.gravityScale = originalGravityScale;
    }


    protected override bool CanChase() => true;

    protected override void HandleMovementSpecific() { }
}
