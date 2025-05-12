using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [field: SerializeField]
    public int MaxHealth { get; private set; }
    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        private set =>
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }
    private bool isDead = false;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnDamage;
    public event Action<int> OnHeal;
    public event Action OnDeath;
    void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
        CurrentHealth -= damage;
        try
        {
            OnDamage?.Invoke(damage);
            OnHealthChanged?.Invoke(CurrentHealth);
            if (CurrentHealth <= 0)
            {
                isDead = true;
                Debug.Log("isDead");
                OnDeath?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void Heal(int heal)
    {
        if (isDead)
            return;
        CurrentHealth += heal;

        try
        {
            OnHeal?.Invoke(heal);
            OnHealthChanged?.Invoke(CurrentHealth);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void ResetHealth()
    {
        Debug.Log(MaxHealth);
        isDead = false;
        CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(MaxHealth);
    }
    public void SetMaxHealth(int maxHealth)
    {
        MaxHealth = maxHealth;
    }
}