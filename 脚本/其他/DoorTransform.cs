using UnityEngine;

public class DoorTransform : MonoBehaviour
{
    [Header("双向传送点(门的子物体)")]
    public Transform outsidePoint;
    public Transform insidePoint;

    [Header("射线设置")]
    public float rayDistance = 2.5f;
    public LayerMask doorLayer;

    [Header("交互提示")]
    public GameObject interactTip;

    [Header("传送冷却")]
    public float cdTime = 0.8f;

    private bool isLookDoor;
    private float cdTimer;

    private CharacterController playerCC;
    private Transform playerTrans;

    void Update()
    {
        if (cdTimer > 0)
            cdTimer -= Time.deltaTime;

        CheckRayInteract();

        if (isLookDoor && cdTimer <= 0 && Input.GetKeyDown(KeyCode.E))
        {
            TeleportToOther();
        }
    }

    void CheckRayInteract()
    {
        isLookDoor = false;

        if (Camera.main == null)
        {
            interactTip?.SetActive(false);
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, rayDistance, doorLayer))
        {
            isLookDoor = true;
            interactTip?.SetActive(true);
        }
        else
        {
            interactTip?.SetActive(false);
        }
    }

    void TeleportToOther()
    {
        cdTimer = cdTime;

        // 找到玩家
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null || outsidePoint == null || insidePoint == null) return;

        // 缓存组件
        playerTrans = player.transform;
        playerCC = player.GetComponent<CharacterController>();

        // 必须先禁用 CharacterController 才能瞬移
        if (playerCC != null)
            playerCC.enabled = false;

        // 判断位置
        float disOut = Vector3.Distance(playerTrans.position, outsidePoint.position);
        float disIn = Vector3.Distance(playerTrans.position, insidePoint.position);

        // 传送
        if (disOut < disIn)
        {
            playerTrans.SetPositionAndRotation(insidePoint.position, insidePoint.rotation);
        }
        else
        {
            playerTrans.SetPositionAndRotation(outsidePoint.position, outsidePoint.rotation);
        }

        // 传送完立刻启用控制器
        if (playerCC != null)
            playerCC.enabled = true;
    }
}