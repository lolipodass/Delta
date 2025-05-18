using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    [field: SerializeField] private UpgradeItemData Data;

    private void Awake()
    {
        if (Data == null)
        {
            Debug.LogError("PickupItem requires a Data field!");
            enabled = false;
            return;
        }
        var image = GetComponent<SpriteRenderer>();
        image.sprite = Data.Icon;
        Tween.ShakeLocalPosition(transform, duration: 1f, strength: new Vector3(0, 0.1f, 0), cycles: -1);
        // Tween.PositionY(transform, end0.01f, duration: 0.5f, cycles: -1, cycleMode: CycleMode.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(Data);
            Tween.Scale(transform, 1, 0f, duration: 0.5f).OnComplete(target: transform, ui => Destroy(gameObject));
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
