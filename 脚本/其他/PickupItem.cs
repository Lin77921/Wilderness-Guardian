using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData itemData;
    public int count = 1;
    public GameObject pickTip;
    private bool canPick = false;

    void Update()
    {
        if (canPick && Input.GetKeyDown(KeyCode.E))
        {
            Pick();
        }
    }

    void Pick()
    {
        if (HotbarSystem.Instance != null && itemData != null)
        {
            bool success = HotbarSystem.Instance.AddItem(itemData, count);
            if (success)
            {
                if (pickTip != null) pickTip.SetActive(false);

                // ======================
                // 【新增】拾取成功 → 显示物品栏
                // ======================
                HotbarShowHide hotbarShow = FindObjectOfType<HotbarShowHide>();
                if (hotbarShow != null)
                {
                    hotbarShow.ShowHotbarOnPickup();
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPick = true;
            if (pickTip != null) pickTip.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPick = false;
            if (pickTip != null) pickTip.SetActive(false);
        }
    }
}