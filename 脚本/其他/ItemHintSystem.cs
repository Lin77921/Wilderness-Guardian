using UnityEngine;
using UnityEngine.UI;

public class ItemHintSystem : MonoBehaviour
{
    public static ItemHintSystem Instance;

    [Header("提示文本")]
    public Text hintText;

    [Header("自动隐藏时间（秒）")]
    public float autoHideTime = 5f;

    private HotbarSystem hotbar;
    private float hideTimer;
    private int lastSelectedIndex = -1;

    void Awake()
    {
        Instance = this;
        hintText.gameObject.SetActive(false);
    }

    void Start()
    {
        hotbar = HotbarSystem.Instance;
    }

    void Update()
    {
        if (hotbar == null) return;

        int currentIndex = hotbar.selectedIndex;
        InventorySlot slot = hotbar.hotbarSlots[currentIndex];

        // ======================
        // 选中物品 → 显示提示，并开始 5 秒计时
        // ======================
        if (slot.currentItem != null && currentIndex != lastSelectedIndex)
        {
            ShowHint();
            lastSelectedIndex = currentIndex;
        }

        // ======================
        // 5 秒自动隐藏
        // ======================
        if (hintText.gameObject.activeSelf)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideHint();
            }
        }
    }

    void ShowHint()
    {
        hintText.gameObject.SetActive(true);
        hideTimer = autoHideTime;
    }

    void HideHint()
    {
        hintText.gameObject.SetActive(false);
    }
}