using UnityEngine;

public class HotbarShowHide : MonoBehaviour
{
    [Header("显示时长(秒)")]
    public float showTime = 2f;
    private CanvasGroup canvasGroup;
    private float timer;
    private bool isShow;

    void Awake()
    {
        if (GetComponent<CanvasGroup>() == null)
            gameObject.AddComponent<CanvasGroup>();
        canvasGroup = GetComponent<CanvasGroup>();
        HideHotbar();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            ShowHotbar();
            timer = showTime;
        }

        if (isShow)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                HideHotbar();
            }
        }
    }

    void ShowHotbar()
    {
        isShow = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void HideHotbar()
    {
        isShow = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // ======================
    // 【新增】拾取物品时强制显示物品栏
    // ======================
    public void ShowHotbarOnPickup()
    {
        ShowHotbar();
        timer = showTime;
    }
}