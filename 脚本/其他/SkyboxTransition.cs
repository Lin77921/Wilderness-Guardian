using UnityEngine;

public class SkyboxTransition : MonoBehaviour
{
    [Header("目标天空盒材质")]
    public Material targetSkybox;

    private Material originalSkybox;
    private bool isSwitched = false;

    void Start()
    {
        // 记录一开始的天空盒
        originalSkybox = RenderSettings.skybox;
    }

    void Update()
    {
        // 按下 K 键 瞬间切换
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (targetSkybox == null) return;

            // 切换：原始 ↔ 目标
            if (!isSwitched)
            {
                RenderSettings.skybox = targetSkybox;
            }
            else
            {
                RenderSettings.skybox = originalSkybox;
            }

            isSwitched = !isSwitched;
        }
    }
}