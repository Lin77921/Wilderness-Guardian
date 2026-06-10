using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    [Header("目标玩家")]
    public Transform player;
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0);

    [Header("鼠标设置")]
    public float mouseSensitivity = 2.5f;
    public float minVerticalAngle = -35f;
    public float maxVerticalAngle = 60f;

    public static bool canControlCamera = true;

    private float mouseX;
    private float mouseY;
    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
        mouseX = transform.eulerAngles.y;
        mouseY = transform.eulerAngles.x;
    }

    // 外部调用：重置相机视角（下车重生后调用）
    public void ResetView()
    {
        mouseX = transform.eulerAngles.y;
        mouseY = 0f; // 第一人称：平视前方
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
    }

    void LateUpdate()
    {
        if (player == null || !canControlCamera) return;

        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);

        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        mainCam.position = player.position + targetOffset;
        mainCam.LookAt(mainCam.position + transform.forward);
    }
}