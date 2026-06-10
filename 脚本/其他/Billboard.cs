using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("告示牌内容")]
    public SignData signData;

    [Header("交互提示")]
    public GameObject interactTipUI;

    public SignData GetSignData()
    {
        return signData;
    }

    public void ShowInteractTip(bool show)
    {
        if (interactTipUI != null)
            interactTipUI.SetActive(show);
    }
}