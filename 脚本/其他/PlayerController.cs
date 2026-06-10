using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float walkSpeed = 3f;
    public float sneakSpeed = 1.5f;
    public float runSpeed = 5.5f;
    public float jumpHeight = 2f;
    public float gravity = -25f;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isSneaking = false;
    private bool isRunning = false;

    private Transform mainCamera;
    private Animator anim;

    // 记录上一帧位置，用于判断实际移动
    private Vector3 lastPos;

    [Header("===== 上车配置 =====")]
    public GameObject car;                // 车辆物体
    public Camera playerCamera;           // 玩家主相机
    public Camera carCamera;              // 车载相机
    public Transform carExitPoint;         // 下车出生点位(车辆子物体)
    public GameObject playerPrefab;       // 玩家预制体(用于下车重生)
    // 注意：下车 G 键检测已移至 CarControl.cs，因为上车后 PlayerController 会被销毁

    private bool isNearCar = false;       // 是否靠近车辆触发区
    private bool isInCar = false;         // 是否正在车内

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
        anim = GetComponent<Animator>();
        lastPos = transform.position;

        // 初始状态：车载相机关闭
        if (carCamera != null)
            carCamera.enabled = false;
    }

    void Update()
    {
        // 上下车按键检测
        CheckEnterCarInput();

        // 仅不在车内时，执行角色原有逻辑
        if (!isInCar)
        {
            Move();
            Jump();
            Sneak();
            ApplyGravity();
            UpdateAnim();
            lastPos = transform.position;
        }
    }

    #region 原有角色移动逻辑（完全未修改）
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            targetAngle = NormalizeAngle(targetAngle);
            float cameraYaw = mainCamera.eulerAngles.y;
            targetAngle += cameraYaw;

            float currentYaw = transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(currentYaw, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float currentSpeed;
            if (isSneaking)
                currentSpeed = sneakSpeed;
            else if (isRunning)
                currentSpeed = runSpeed;
            else
                currentSpeed = walkSpeed;

            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            turnSmoothVelocity = 0f;
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }

    void Jump()
    {
        bool canJump = controller.isGrounded
                     || Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Sneak()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSneaking = !isSneaking;
        }

        // 按 Shift 切换奔跑（点击切换，非按住）
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = !isRunning;
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnim()
    {
        // 计算角色本帧实际移动距离
        float moveDistance = Vector3.Distance(transform.position, lastPos);
        float animSpeed = 0f;

        // 移动距离大于极小值 = 真正在行走
        if (moveDistance > 0.001f)
        {
            // 区分 走/跑/潜行 不同动画速度
            if (isSneaking)
                animSpeed = 0.5f;
            else if (isRunning)
                animSpeed = 1.5f;
            else
                animSpeed = 1f;
        }

        anim.SetFloat("Speed", animSpeed);
    }

    public bool IsSneaking() => isSneaking;
    #endregion

    #region 上下车新增逻辑
    // 检测F按键上车（G键下车已由 CarControl 处理）
    void CheckEnterCarInput()
    {
        // F 上车：靠近车辆 + 不在车内
        if (Input.GetKeyDown(KeyCode.F) && isNearCar && !isInCar)
        {
            EnterCar();
        }
    }

    // 上车逻辑
    void EnterCar()
    {
        isInCar = true;
        Debug.Log("=== 成功上车 ===");

        // 强制切换相机
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }
        if (carCamera != null)
        {
            carCamera.gameObject.SetActive(true);
            carCamera.enabled = true;
            // 确保车载相机渲染优先级高于其他相机
            carCamera.depth = 10;
        }

        if (car != null)
        {
            CarControl carCtrl = car.GetComponent<CarControl>();
            // 把下车所需引用传给 CarControl
            carCtrl.playerCamera  = playerCamera;
            carCtrl.carCamera     = carCamera;
            carCtrl.carExitPoint  = carExitPoint;
            carCtrl.playerPrefab  = playerPrefab;
            carCtrl.StartControl(); // 显式激活控制，而非依赖 enabled
        }

        Destroy(gameObject);
    }

    // 注意：下车逻辑已移至 CarControl.cs（因为上车后本脚本被销毁，无法接收输入）

    // 进入车辆触发区
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarTrigger") && !isInCar)
        {
            isNearCar = true;
            Debug.Log("按 F 上车");
        }
    }

    // 离开车辆触发区
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CarTrigger"))
        {
            isNearCar = false;
        }
    }
    #endregion
}