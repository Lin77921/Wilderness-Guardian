using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("车辆参数")]
    public float moveSpeed = 25f;     // 行驶速度（加大）
    public float steerSpeed = 60f;    // 转弯角速度（度/秒，减小）
    public float maxSteerAngle = 45f; // 最大转向角度

    [Header("===== 下车配置 =====")]
    public Camera playerCamera;
    public Camera carCamera;
    public Transform carExitPoint;
    public GameObject playerPrefab;

    private float horizontal;
    private float vertical;
    private bool isControlling = false;

    // 由 PlayerController 上车时调用，激活车辆控制
    public void StartControl()
    {
        isControlling = true;
        Debug.Log("[CarControl] StartControl 已调用，车辆控制已激活");
    }

    void Update()
    {
        if (!isControlling) return;

        horizontal = Input.GetAxis("Horizontal");
        vertical   = Input.GetAxis("Vertical");

        // 调试：按 V 键打印当前输入
        if (Input.GetKeyDown(KeyCode.V))
            Debug.Log($"[CarControl] v={vertical:F2} h={horizontal:F2} pos={transform.position}");

        if (Input.GetKeyDown(KeyCode.G))
            ExitCar();
    }

    void FixedUpdate()
    {
        if (!isControlling) return;

        // 转向（用 transform，不依赖物理）
        if (Mathf.Abs(horizontal) > 0.05f)
        {
            float steer = horizontal * maxSteerAngle;
            transform.Rotate(0f, steer * steerSpeed * Time.fixedDeltaTime, 0f);
        }

        // 前后移动：用 transform.Translate，完全绕过 Rigidbody 约束
        if (Mathf.Abs(vertical) > 0.05f)
        {
            transform.Translate(transform.forward * vertical * moveSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    void ExitCar()
    {
        isControlling = false;
        Debug.Log("=== 下车流程开始 ===");

        // 1. 先恢复玩家相机（确保它成为 Camera.main）
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
            playerCamera.enabled = true;
            // 确保玩家相机渲染优先级高于车载相机
            playerCamera.depth = 11;
            Debug.Log("[CarControl] 玩家相机已恢复");
        }
        else
        {
            Debug.LogWarning("[CarControl] playerCamera 为 null！");
        }

        // 2. 禁用车辆相机
        if (carCamera != null)
        {
            carCamera.enabled = false;
            carCamera.gameObject.SetActive(false);
        }

        // 3. 生成玩家（此时 Camera.main 已是玩家相机）
        if (playerPrefab != null && carExitPoint != null)
        {
            // 用射线检测找到地面高度，避免重生点偏低导致相机过矮
            Vector3 spawnPos = carExitPoint.position;
            if (Physics.Raycast(spawnPos + Vector3.up * 5f, Vector3.down, out RaycastHit groundHit, 10f))
            {
                spawnPos.y = groundHit.point.y;
                Debug.Log($"[CarControl] 射线检测到地面高度: {spawnPos.y:F2}");
            }

            GameObject newPlayer = Instantiate(playerPrefab, spawnPos, carExitPoint.rotation);
            Debug.Log($"[CarControl] 玩家已重生在: {spawnPos}，对象名: {newPlayer.name}");

            // 4. 让 MouseCamera 重新跟随新玩家，并重置视角
            MouseCamera mouseCam = FindAnyObjectByType<MouseCamera>();
            if (mouseCam != null)
            {
                mouseCam.player = newPlayer.transform;
                MouseCamera.canControlCamera = true;
                mouseCam.targetOffset = new Vector3(0f, 3.2f, 0f); // 相机高度翻倍
                mouseCam.ResetView();

                // 调大相机近裁剪面，让玩家模型被裁剪掉（实现第一人称）
                if (Camera.main != null)
                    Camera.main.nearClipPlane = 0.15f;

                Debug.Log("[CarControl] MouseCamera 已重新跟随新玩家并重置视角");
            }
        }
        else
        {
            Debug.LogError("[CarControl] 无法重生玩家！请在 PlayerController 的 Inspector 中检查：");
            if (playerPrefab == null)
                Debug.LogError("[CarControl]   → playerPrefab 未赋值！");
            if (carExitPoint == null)
                Debug.LogError("[CarControl]   → carExitPoint 未赋值！");
        }

        Debug.Log("=== 下车流程结束 ===");
    }
}
