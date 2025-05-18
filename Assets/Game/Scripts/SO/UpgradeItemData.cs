using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItem", menuName = "Items/Upgrade Item")]
public class UpgradeItemData : ScriptableObject
{
    public string ID;
    public string Name;
    public Sprite Icon;
    [TextArea] public string Description;

    public List<UpgradeModifier> modifiersToApply = new();
}