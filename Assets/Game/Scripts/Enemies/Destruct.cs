using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Destruct : SavableObject
{
    private HealthComponent healthComponent;
    [SerializeField] private Transform breakPosition;
    private bool isBroken = false;
    [SerializeField] private float breakRange = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDamage += OnDamage;
        healthComponent.OnDeath += OnDeath;
        if (breakPosition == null)
        {
            Debug.Log("Destruct: Break position not found");
            enabled = false;
            return;
        }
        breakPosition.gameObject.SetActive(false);

        healthComponent.OnDamageCheck += OnDamageCheck;
    }
    private bool OnDamageCheck(int damage, Vector2 position)
    {
        if (Vector3.Distance(position, breakPosition.position) < breakRange)
        {
            Debug.Log("Destruct: Damage check passed");
            return true;
        }
        return false;
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

    private void OnDamage(int damage, Vector2 position)
    {
        //animation 
    }
    private void OnDeath(Vector2 position)
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
    private void OnDrawGizmosSelected()
    {

        if (breakPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(breakPosition.position, breakRange);
        }

    }
}