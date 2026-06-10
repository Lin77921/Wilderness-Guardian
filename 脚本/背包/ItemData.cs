using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemName;
    public Sprite icon;
    [TextArea(2, 5)]
    public string itemDesc;

    [Header("堆叠设置")]
    public bool isStackable = true;
    public int maxStack = 64;

    // 👇 新加：只有照片勾这个
    public bool canDiscard = false;
}