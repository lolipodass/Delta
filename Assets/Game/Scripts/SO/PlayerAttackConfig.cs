using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackConfig", menuName = "Scriptable Objects/PlayerAttackConfig")]
public class PlayerAttackConfig : ScriptableObject
{
    public float AttackTime;
    public float AttackCooldown;
    public float InvincibilityAfterHit;
    public float DamageMultiplier;
    public Vector2 Size;
}
