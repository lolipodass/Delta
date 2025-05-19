using PrimeTween;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public float minShakeStrength = 0.001f;
    public float maxShakeStrength = 0.1f;
    public float shakeDuration = 1f;

    public float rotationAngle = 5f;
    public float rotationDuration = 1f;
    [field: SerializeField] private UpgradeItemData Data;
    public bool CanHaveMultiple = false;

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
        Shake();
    }

    private void Start()
    {

        if (InventoryManager.Instance.HasItemInInventory(Data.ID) && !CanHaveMultiple)
        {
            Destroy(gameObject);
        }
    }
    private void Shake()
    {
        var randomShakeY = Random.Range(minShakeStrength, maxShakeStrength);

        Vector3 startRotation = new(0, 0, -rotationAngle);
        Vector3 endRotation = new(0, 0, rotationAngle);

        //dont use sequence here, because cycles bug with setRemainingCycles with cycleMode
        Tween.ShakeLocalPosition(transform,
                        duration: shakeDuration + randomShakeY,
                        strength: new Vector3(0, randomShakeY, 0),
                        cycles: -1);

        Tween.LocalRotation(transform,
                        duration: rotationDuration + randomShakeY,
                        startValue: startRotation,
                        endValue: endRotation,
                        cycleMode: CycleMode.Rewind,
                        cycles: -1);

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(Data);
            AnimationPickup();
        }
    }
    private void AnimationPickup()
    {
        var image = GetComponent<SpriteRenderer>();
        Sequence.Create()
            .Group(Tween.Scale(transform, 1, 0.5f, duration: 0.5f))
            .Group(Tween.Alpha(image, 1, 0f, duration: 0.5f))
            .OnComplete(target: transform, ui => Destroy(gameObject));


    }
    private void OnValidate()
    {
        var image = GetComponent<SpriteRenderer>();

        if (gameObject.scene.name != null)
            image.sprite = Data.Icon;

        //dont catch because can click on error message and find element if data is null
    }
}
