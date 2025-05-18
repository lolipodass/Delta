using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [field: SerializeField] public Image Image { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Name { get; private set; }
    public void SetData(UpgradeItemData data)
    {
        Image.sprite = data.Icon;
        Name.text = data.name;
    }


}
