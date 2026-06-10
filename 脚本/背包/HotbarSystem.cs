using UnityEngine;

public class HotbarSystem : MonoBehaviour
{
    public static HotbarSystem Instance;

    public InventorySlot[] hotbarSlots;
    public int selectedIndex = 0;

    [Header("丢弃的物品预设")]
    public GameObject dropItemPrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateSelected();
    }

    void Update()
    {
        HandleInput();
        HandleDiscard(); // 👈 加丢弃监听
    }

    void HandleInput()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedIndex = i;
                UpdateSelected();
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            selectedIndex += scroll > 0 ? -1 : 1;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, hotbarSlots.Length - 1);
            UpdateSelected();
        }
    }

    // ======================
    // Q 丢弃（只允许照片）
    // ======================
    void HandleDiscard()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InventorySlot slot = hotbarSlots[selectedIndex];

            if (slot.currentItem == null) return;

            // 👮‍♂️ 只有照片能丢（canDiscard = true）
            if (!slot.currentItem.canDiscard)
            {
                Debug.Log("该物品无法丢弃");
                return;
            }

            // 丢弃
            DropItem(slot.currentItem);
            // 清空格子
            slot.ClearSlot();
        }
    }

    // 生成掉落物品
    void DropItem(ItemData item)
    {
        if (dropItemPrefab == null) return;

        Vector3 dropPos = transform.position + transform.forward * 2f;
        GameObject drop = Instantiate(dropItemPrefab, dropPos, Quaternion.identity);

        PickupItem pickup = drop.GetComponent<PickupItem>();
        if (pickup != null)
        {
            pickup.itemData = item;
            pickup.count = 1;
        }

        Debug.Log("已丢弃：" + item.itemName);
    }

    void UpdateSelected()
    {
        foreach (var slot in hotbarSlots)
            slot.SetSelected(false);
        hotbarSlots[selectedIndex].SetSelected(true);
    }

    public bool AddItem(ItemData item, int count)
    {
        if (item == null) return false;

        foreach (var slot in hotbarSlots)
        {
            if (slot.currentItem == item && slot.currentCount < item.maxStack)
            {
                slot.UpdateSlot(item, slot.currentCount + count);
                return true;
            }
        }

        foreach (var slot in hotbarSlots)
        {
            if (slot.currentItem == null)
            {
                slot.UpdateSlot(item, count);
                return true;
            }
        }
        return false;
    }
}