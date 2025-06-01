using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Destruct : SavableObject
{
    private HealthComponent healthComponent;
    private bool isBroken = false;

    protected override void Awake()
    {
        base.Awake();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDamage += OnDamage;
        healthComponent.OnDeath += OnDeath;
    }

    public override int CaptureState()
    {
        return isBroken ? 1 : 0;
    }
    public override void RestoreState(int state)
    {
        isBroken = state == 1;
        UpdateState();
    }

    private void OnDamage(int damage)
    {
        //animation 
    }
    private void OnDeath()
    {
        if (!isBroken)
        {
            isBroken = true;
            UpdateState();
            FileSaveManager.Instance.SaveElement(this);

            Debug.Log($"Wall {Id} is now broken.");
        }
    }

    private void UpdateState()
    {
        gameObject.SetActive(!isBroken);
    }

    private void Start()
    {
        UpdateState();
    }
}