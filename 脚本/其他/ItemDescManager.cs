using UnityEngine;
using UnityEngine.UI;

public class ItemDescManager : MonoBehaviour
{
    public static ItemDescManager Instance;

    [Header("详情面板")]
    public GameObject panel;
    public Text itemName;
    public Text itemDesc;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    void Update()
    {
        // 按 H 打开/关闭
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleDesc();
        }
    }

    void ToggleDesc()
    {
        HotbarSystem hotbar = HotbarSystem.Instance;
        if (hotbar == null) return;

        var slot = hotbar.hotbarSlots[hotbar.selectedIndex];

        if (slot.currentItem == null)
        {
            panel.SetActive(false);
            return;
        }

        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            itemName.text = slot.currentItem.itemName;
            itemDesc.text = slot.currentItem.itemDesc;
            panel.SetActive(true);
        }
    }
}