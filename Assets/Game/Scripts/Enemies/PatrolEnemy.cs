using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
    [Header("Movement")]
    [SerializeField] private float JumpCooldown = 0.5f;
    [SerializeField] private float JumpForce = 4f;
    [SerializeField] private float JumpTriggerDistance = 0.5f;
    [SerializeField] private float GroundRaycastSize = 0.8f;
    [SerializeField] private LayerMask GroundLayerMask = -1;
    private float jumpCooldownTimer;

    protected override bool CanAttack() => false;
    protected override bool CanChase() => false;
    protected override float GetAttackRange() => 0;

    protected override void UpdateTimers()
    {
        base.UpdateTimers();

        jumpCooldownTimer -= Time.fixedDeltaTime;
    }
    protected override void HandleAttackState() { }
    protected override void HandleMovementSpecific()
    {
        if (direction.y > JumpTriggerDistance && IsGrounded() && jumpCooldownTimer <= 0f)
        {
            jumpCooldownTimer = JumpCooldown;
            rb.linearVelocityY = JumpForce;
        }
    }

    protected override bool IsGrounded()
    {
        var groundCheck = Physics2D.Raycast(transform.position, Vector2.down, GroundRaycastSize, GroundLayerMask);
        return groundCheck.collider != null;
    }

    protected override void InitializeSpecific() { }

    protected override void OnDestroySpecific() { }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Debug.DrawRay(transform.position, Vector2.down * GroundRaycastSize, IsGrounded() ? Color.green : Color.red);
    }
}
