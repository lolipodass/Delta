using UnityEngine;

public class InventoryUIManager : MonoSingleton<InventoryUIManager>
{

    [field: SerializeField] public GameObject GridElement { get; private set; }
    [field: SerializeField] public GameObject ItemPrefab { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        if (!CheckObject(GridElement, "GridElement")) return;
        if (!CheckObject(ItemPrefab, "InventoryItemUI")) return;
    }
    public void Start()
    {
        InventoryManager.Instance.OnInventoryChanged += UpdateInventory;
        UpdateInventory();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InventoryManager.Instance.OnInventoryChanged -= UpdateInventory;
    }
    public void UpdateInventory()
    {
        ClearInventory();
        var inventory = InventoryManager.Instance.Inventory;
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = Instantiate(ItemPrefab, GridElement.transform);
            item.GetComponent<InventoryItemUI>().SetData(inventory[i]);
        }
    }

    public void ClearInventory()
    {
        foreach (Transform child in GridElement.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private bool CheckObject(GameObject ui, string name)
    {
        if (ui == null)
        {
            Debug.LogError($"InventoryUIManager requires a {name} GameObject reference!");
            enabled = false;
            return false;
        }
        return true;
    }

}
