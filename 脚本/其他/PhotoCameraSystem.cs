using UnityEngine;
using UnityEngine.UI;

public class PhotoCameraSystem : MonoBehaviour
{
    [Header("移动/视角")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 2f;

    [Header("拍照设置")]
    public Image flashUI;
    public AudioSource shootAudio;
    public ItemData photoItem;

    [Header("玩家设置")]
    public GameObject playerModel; // 把玩家模型拖这里
    public LayerMask photoCullingMask; // 拍照渲染层（取消Player层）

    private Camera mainCam;
    private Camera photoCamera;
    private bool isWork = false;
    private float rotX, rotY;

    private PlayerController playerController;
    private MouseCamera mouseCamera;

    void Awake()
    {
        gameObject.SetActive(false);
        mainCam = Camera.main;
        photoCamera = GetComponent<Camera>();
        playerController = FindObjectOfType<PlayerController>();
        mouseCamera = FindObjectOfType<MouseCamera>();
    }

    void Update()
    {
        if (!isWork) return;

        CameraRotate();
        CameraMove();

        if (Input.GetMouseButtonDown(0))
        {
            TakePhoto();
        }
    }

    public void OpenCamera()
    {
        isWork = true;
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 禁用玩家控制
        if (playerController != null) playerController.enabled = false;
        if (mouseCamera != null) mouseCamera.enabled = false;

        // 相机位置
        transform.position = mainCam.transform.position + mainCam.transform.forward * 1.5f + new Vector3(0, 0.6f, 0);
        transform.rotation = mainCam.transform.rotation;
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        // ----------------------
        // 解决白屏核心！
        // ----------------------
        photoCamera.clearFlags = CameraClearFlags.Skybox;
        photoCamera.cullingMask = photoCullingMask;
        photoCamera.enabled = true;
    }

    public void CloseCamera()
    {
        isWork = false;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 恢复玩家
        if (playerController != null) playerController.enabled = true;
        if (mouseCamera != null) mouseCamera.enabled = true;

        // 强制显示人物
        if (playerModel != null) playerModel.SetActive(true);
    }

    void CameraRotate()
    {
        rotY += Input.GetAxis("Mouse X") * rotateSpeed;
        rotX -= Input.GetAxis("Mouse Y") * rotateSpeed;
        rotX = Mathf.Clamp(rotX, -45f, 45f);
        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }

    void CameraMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = transform.right * h + transform.forward * v;
        dir.y = 0;
        dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void TakePhoto()
    {
        Debug.Log("拍照成功！");

        // ----------------------
        // 拍照时隐藏人物（双保险）
        // ----------------------
        if (playerModel != null)
        {
            playerModel.SetActive(false);
        }

        // 闪光效果
        if (flashUI != null)
        {
            flashUI.gameObject.SetActive(true);
            Invoke(nameof(HideFlash), 0.1f);
        }

        if (shootAudio != null)
        {
            shootAudio.Play();
        }

        // 0.1秒后恢复人物
        Invoke(nameof(ShowPlayer), 0.1f);
        AddPhotoToBag();
    }

    void ShowPlayer()
    {
        if (playerModel != null)
        {
            playerModel.SetActive(true);
        }
    }

    void HideFlash()
    {
        if (flashUI != null)
            flashUI.gameObject.SetActive(false);
    }

    void AddPhotoToBag()
    {
        HotbarSystem hotbar = HotbarSystem.Instance;
        if (hotbar == null || photoItem == null) return;

        foreach (var slot in hotbar.hotbarSlots)
        {
            if (slot.currentItem == null)
            {
                slot.UpdateSlot(photoItem, 1);
                Debug.Log("照片已存入物品栏");
                return;
            }
        }
        Debug.Log("快捷栏已满");
    }
}