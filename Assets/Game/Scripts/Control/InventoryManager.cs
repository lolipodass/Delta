using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{

    private UpgradeItemData[] inventory;
    public UpgradeItemData[] Inventory { get => inventory; }

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
        foreach (var item in items)
        {
            itemsRegistry.Add(item.itemID, item);
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
            result.Add(item.itemID);
        }
        return result.ToArray();
    }

    public void LoadInventory(string[] ids)
    {
        inventory = new UpgradeItemData[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            var item = GetByID(ids[i]);
            if (item != null)
            {
                inventory[i] = item;
            }
        }
    }



}
