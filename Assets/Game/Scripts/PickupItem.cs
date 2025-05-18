using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    [field: SerializeField] private UpgradeItemData Data;


    private void Start()
    {
        if (Data == null)
        {
            Debug.LogError("PickupItem requires a Data field!");
            enabled = false;
            return;
        }
        var image = GetComponent<SpriteRenderer>();
        image.sprite = Data.Icon;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(Data);
            Destroy(gameObject);
        }
    }
    private void OnValidate()
    {
        if (Data != null)
        {
            var image = GetComponent<SpriteRenderer>();
            image.sprite = Data.Icon;
        }
    }
}
