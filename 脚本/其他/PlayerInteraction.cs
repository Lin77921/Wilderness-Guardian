using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("弹窗UI")]
    public GameObject signDetailPanel;
    public Image detailImage;
    public Text detailText;

    [Header("射线距离")]
    public float interactDistance = 3f;

    private Billboard currentSign;
    private Rigidbody playerRb;
    private bool originCanMove;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 判空保护，防止面板为空时报错
        if (signDetailPanel != null && signDetailPanel.activeSelf)
            return;

        CheckRay();

        if (currentSign != null && Input.GetKeyDown(KeyCode.E))
        {
            OpenSign(currentSign.signData);
        }
    }

    void CheckRay()
    {
        // 主相机为空直接返回
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Billboard hitSign = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            hitSign = hit.collider.GetComponent<Billboard>();
        }

        if (hitSign != null)
        {
            if (currentSign != hitSign)
            {
                if (currentSign != null)
                    currentSign.ShowInteractTip(false);

                currentSign = hitSign;
                currentSign.ShowInteractTip(true);
            }
        }
        else
        {
            if (currentSign != null)
            {
                currentSign.ShowInteractTip(false);
                currentSign = null;
            }
        }
    }

    void OpenSign(SignData data)
    {
        // 数据为空直接返回，不执行
        if (data == null) return;

        if (currentSign != null)
            currentSign.ShowInteractTip(false);

        // UI 判空
        if (detailImage != null)
            detailImage.sprite = data.detailImage;

        if (detailText != null)
            detailText.text = data.description;

        if (signDetailPanel != null)
            signDetailPanel.SetActive(true);

        // 禁止相机转动（安全调用）
        if (MouseCamera.canControlCamera != null)
            MouseCamera.canControlCamera = false;

        // 锁住玩家物理
        if (playerRb != null)
        {
            originCanMove = !playerRb.isKinematic;
            playerRb.isKinematic = true;
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseSignDetail()
    {
        if (signDetailPanel != null)
            signDetailPanel.SetActive(false);

        currentSign = null;

        // 恢复玩家移动
        if (playerRb != null)
        {
            playerRb.isKinematic = originCanMove;
        }

        StartCoroutine(ResetMouseOnly());
    }

    IEnumerator ResetMouseOnly()
    {
        yield return null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 恢复相机控制（安全）
        if (MouseCamera.canControlCamera != null)
            MouseCamera.canControlCamera = true;
    }
}