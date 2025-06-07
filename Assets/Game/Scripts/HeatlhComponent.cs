using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    // public int MaxHealth { get; private set; }
    // private int _currentHealth;
    [field: SerializeField] public ObfuscatedInt MaxHealth { get; private set; }
    private ObfuscatedInt _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        private set =>
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }
    private bool isDead = false;
    public event Action<int, Vector2> OnHealthChanged;
    public event Action<int, Vector2> OnDamage;
    public event Action<int, Vector2> OnHeal;
    public event Action<Vector2> OnDeath;
    public event Func<int, Vector2, bool> OnDamageCheck;
    void Awake()
    {
        if (MaxHealth == null)
        {
            MaxHealth = new ObfuscatedInt(1);
        }
        _currentHealth = new ObfuscatedInt();
        CurrentHealth = MaxHealth;
    }
    public void TakeDamage(int damage, Vector3 attackPosition)
    {
        if (isDead)
            return;
        if (OnDamageCheck != null)
        {
            if (!OnDamageCheck(damage, attackPosition))
            {
                return;
            }
        }
        CurrentHealth -= damage;
        try
        {
            if (CurrentHealth <= 0)
            {
                isDead = true;
                Debug.Log("isDead");
                OnDeath?.Invoke(attackPosition);
            }
            OnDamage?.Invoke(damage, attackPosition);
            OnHealthChanged?.Invoke(CurrentHealth, attackPosition);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void Heal(int heal, Vector3 attackPosition)
    {
        if (isDead)
            return;
        CurrentHealth += heal;

        try
        {
            OnHeal?.Invoke(heal, attackPosition);
            OnHealthChanged?.Invoke(CurrentHealth, attackPosition);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(MaxHealth, Vector2.zero);
    }
    public void SetMaxHealth(int maxHealth)
    {
        MaxHealth = maxHealth;
    }
}