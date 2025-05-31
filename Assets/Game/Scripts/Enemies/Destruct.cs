using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Destruct : SavableObject
{
    private HealthComponent healthComponent;
    private bool isBroken = false;

    public void Break()
    {
        if (!isBroken)
        {
            isBroken = true;
            UpdateWallVisual();
            Debug.Log($"Wall {Id} is now broken.");
        }
    }

    protected void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDamage += OnDamage;
        healthComponent.OnDeath += OnDeath;
    }

    private void OnDamage(int damage)
    {
        //animation 
    }
    private void OnDeath()
    {
        Break();
    }

    private void UpdateWallVisual()
    {
        gameObject.SetActive(!isBroken);
    }

    private void Start()
    {
        UpdateWallVisual();
    }
}