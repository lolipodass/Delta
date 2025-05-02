using UnityEngine;

public interface IAttackHandler
{
    protected int Damage { get; }
    protected Vector2 Position { get; }
    protected Vector2 Size { get; }

    void Attack()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(Position, Size, 0f);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Player")) continue;
            if (enemy.TryGetComponent<HealthComponent>(out var healthComponent))
            {
                healthComponent.TakeDamage(Damage);
            }
        }
    }
}