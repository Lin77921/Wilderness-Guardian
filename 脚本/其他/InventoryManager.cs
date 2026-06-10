using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // 单例实例
    public static InventoryManager Instance;

    [Header("物品栏配置")]
    public int inventorySize = 27; // 总物品栏格子数（含快捷栏）
    public int hotbarSize = 9;     // 快捷栏格子数

    // 物品栏数据
    private List<InventorySlotData> inventorySlots;

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化物品栏数据
        InitInventory();
    }

    // 初始化物品栏
    private void InitInventory()
    {
        inventorySlots = new List<InventorySlotData>();
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(new InventorySlotData());
        }

        // 同步快捷栏UI（关联HotbarSystem）
        SyncHotbarUI();
    }

    // 添加物品到物品栏
    public bool AddItem(ItemData item, int count)
    {
        if (item == null || count <= 0)
            return false;

        // 1. 先尝试堆叠到已有物品格子
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlotData slot = inventorySlots[i];
            if (slot.item == item && slot.count < item.maxStack)
            {
                int addCount = Mathf.Min(count, item.maxStack - slot.count);
                slot.count += addCount;
                count -= addCount;

                // 更新UI
                UpdateSlotUI(i);

                if (count <= 0)
                    return true;
            }
        }

        // 2. 堆叠失败，找空格子
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlotData slot = inventorySlots[i];
            if (slot.item == null)
            {
                int addCount = Mathf.Min(count, item.maxStack);
                slot.item = item;
                slot.count = addCount;
                count -= addCount;

                // 更新UI
                UpdateSlotUI(i);

                if (count <= 0)
                    return true;
            }
        }

        // 3. 没有空格子，拾取失败
        return false;
    }

    // 更新单个格子UI
    private void UpdateSlotUI(int slotIndex)
    {
        // 区分快捷栏和背包栏
        if (slotIndex < hotbarSize)
        {
            HotbarSystem hotbar = FindAnyObjectByType<HotbarSystem>();
            if (hotbar != null && hotbar.hotbarSlots.Length > slotIndex)
            {
                InventorySlotData slotData = inventorySlots[slotIndex];
                hotbar.hotbarSlots[slotIndex].UpdateSlot(slotData.item, slotData.count);
            }
        }
        // 扩展：这里可以添加背包UI的更新逻辑
    }

    // 同步快捷栏UI
    private void SyncHotbarUI()
    {
        HotbarSystem hotbar = FindAnyObjectByType<HotbarSystem>();
        if (hotbar == null) return;

        for (int i = 0; i < hotbarSize && i < hotbar.hotbarSlots.Length; i++)
        {
            InventorySlotData slotData = inventorySlots[i];
            hotbar.hotbarSlots[i].UpdateSlot(slotData.item, slotData.count);
        }
    }

    // 物品栏格子数据结构
    [System.Serializable]
    public class InventorySlotData
    {
        public ItemData item;
        public int count;

        public InventorySlotData()
        {
            item = null;
            count = 0;
        }
    }
}