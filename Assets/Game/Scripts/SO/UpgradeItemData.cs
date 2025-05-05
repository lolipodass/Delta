using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItem", menuName = "Items/Upgrade Item")]
public class UpgradeItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string itemDescription;

    public List<UpgradeModifier> modifiersToApply = new();
}