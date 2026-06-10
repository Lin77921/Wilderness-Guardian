using UnityEngine;

public class PlayerCameraInput : MonoBehaviour
{
    [Header("相机道具")]
    public ItemData cameraItem;
    [Header("拍照相机物体")]
    public PhotoCameraSystem photoCam;

    void Update()
    {
        // 仅按下F判定
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckUseCamera();
        }
    }

    void CheckUseCamera()
    {
        HotbarSystem hotbar = FindObjectOfType<HotbarSystem>();
        if (hotbar == null) return;

        // 获取当前选中格子
         int selectIdx = hotbar.selectedIndex;
        InventorySlot curSlot = hotbar.hotbarSlots[selectIdx];

        // 判断当前选中物品是不是相机
        if (curSlot.currentItem != cameraItem)
        {
            Debug.Log("未选中相机道具，无法使用！");
            return;
        }

        // 选中相机，切换相机状态
        if (photoCam.gameObject.activeSelf)
        {
            photoCam.CloseCamera();
        }
        else
        {
            photoCam.OpenCamera();
        }
    }
}