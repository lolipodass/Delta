using System.Collections;
using UnityEngine;

public class Destruct : MonoBehaviour
{
    public HealthComponent healthComponent;
    [SerializeField] private float respawnDelay = 5.0f;

    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Collider2D objectCollider;
    // [SerializeField] private List<MonoBehaviour> componentsToDisable;

    private Coroutine _respawnCoroutine;
    private bool isDead = false;

    void Start()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
            healthComponent.OnDamage += onHealthChange;
        }
        else
        {
            Debug.LogError("HealthComponent not assigned in Destruct script on GameObject: " + gameObject.name);
        }

        if (objectRenderer == null) Debug.LogWarning("Renderer not assigned in Destruct script on " + gameObject.name);
        if (objectCollider == null) Debug.LogWarning("Collider2D not assigned in Destruct script on " + gameObject.name);
    }

    private void onHealthChange(int amount)
    {
        // Можно убрать, если не нужно для отладки
        // Debug.Log($"Health changed by {amount} on {gameObject.name}");
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        if (_respawnCoroutine != null)
        {
            StopCoroutine(_respawnCoroutine);
            SetObjectActiveState(true);
        }
        _respawnCoroutine = StartCoroutine(RespawnCoroutine());


        SetObjectActiveState(false);
    }

    private IEnumerator RespawnCoroutine()
    {
        Debug.Log($"{gameObject.name} - Starting respawn timer...");
        yield return new WaitForSeconds(respawnDelay);

        Debug.Log($"{gameObject.name} - Respawning now!");

        if (healthComponent != null)
        {
            healthComponent.ResetHealth();
        }

        SetObjectActiveState(true);

        isDead = false;
        _respawnCoroutine = null;
    }

    private void SetObjectActiveState(bool isActive)
    {
        if (objectRenderer != null) objectRenderer.enabled = isActive;
        if (objectCollider != null) objectCollider.enabled = isActive;

        // Отключить/включить другие компоненты, если нужно
        // foreach (var component in componentsToDisable)
        // {
        //     if (component != null) component.enabled = isActive;
        // }

        // Если вам все же нужно полностью деактивировать объект ПОСЛЕ задержки,
        // а не просто скрыть/сделать неинтерактивным, можно было бы сделать так:
        // gameObject.SetActive(isActive);
        // Но в вашем исходном коде деактивация была ДО задержки, что и вызывало проблему.
        // Если вы хотите, чтобы объект ИСЧЕЗ на время респавна, используйте этот метод.
    }


    void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= HandleDeath;
            healthComponent.OnDamage -= onHealthChange;
        }
        if (_respawnCoroutine != null)
        {
            StopCoroutine(_respawnCoroutine);
        }
    }
}