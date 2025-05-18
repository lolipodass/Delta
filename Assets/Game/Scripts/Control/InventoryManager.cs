using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{

    private List<UpgradeItemData> inventory;
    public List<UpgradeItemData> Inventory { get => inventory; }

    private Dictionary<string, UpgradeItemData> itemsRegistry;
    public event Action OnInventoryChanged;
    public event Action OnActiveModifiersChanged;
    public event Action<UpgradeItemData> OnItemAdded;
    public event Action<UpgradeItemData> OnItemRemoved;

    protected override void Awake()
    {
        base.Awake();
        itemsRegistry = new Dictionary<string, UpgradeItemData>();
        UpgradeItemData[] items = Resources.LoadAll<UpgradeItemData>("Items");
        inventory = new();
        foreach (var item in items)
        {
            itemsRegistry.Add(item.ID, item);
        }
    }

    private UpgradeItemData GetByID(string id)
    {
        if (itemsRegistry.TryGetValue(id, out var item))
        {
            return item;
        }
        return null;
    }

    public string[] SaveInventory()
    {
        List<string> result = new();
        foreach (var item in inventory)
        {
            result.Add(item.ID);
        }
        return result.ToArray();
    }

    public void LoadInventory(string[] ids)
    {
        inventory = new(ids.Length);
        for (int i = 0; i < ids.Length; i++)
        {
            var item = GetByID(ids[i]);
            if (item != null)
            {
                inventory[i] = item;
            }
        }
    }

    public void AddItem(UpgradeItemData item)
    {
        inventory.Add(item);
        OnInventoryChanged?.Invoke();
    }
    public void RemoveItem(UpgradeItemData item)
    {
        inventory.Remove(item);
        OnInventoryChanged?.Invoke();
    }
}
