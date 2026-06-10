using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemCount;
    public Image selectedBg;

    public ItemData currentItem;
    public int currentCount;

    public void UpdateSlot(ItemData item, int count)
    {
        Debug.Log("🔄 刷新格子：" + item);

        currentItem = item;
        currentCount = count;

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            itemCount.text = count > 1 ? count.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentCount = 0;
        itemIcon.enabled = false;
        itemCount.text = "";
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedBg != null)
            selectedBg.enabled = isSelected;
    }
}