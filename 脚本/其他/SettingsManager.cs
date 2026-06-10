using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("面板")]
    public GameObject settingsPanel;

    [Header("声音")]
    public AudioMixer audioMixer;
    public Slider slider_BGM;
    public Slider slider_SFX;

    [Header("显示")]
    public Toggle toggle_FullScreen;
    public Dropdown dropdown_Quality;
    public Dropdown dropdown_Resolution;

    private Resolution[] resolutions;

    void Start()
    {
        settingsPanel.SetActive(false);
        InitSettings();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    void InitSettings()
    {
        resolutions = Screen.resolutions;
        dropdown_Resolution.ClearOptions();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string opt = $"{resolutions[i].width}×{resolutions[i].height}";
            dropdown_Resolution.options.Add(new Dropdown.OptionData(opt));
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResIndex = i;
        }
        dropdown_Resolution.value = currentResIndex;
        dropdown_Resolution.RefreshShownValue();

        dropdown_Quality.value = QualitySettings.GetQualityLevel();
        dropdown_Quality.RefreshShownValue();
        toggle_FullScreen.isOn = Screen.fullScreen;

        slider_BGM.value = PlayerPrefs.GetFloat("BGM", 0.75f);
        slider_SFX.value = PlayerPrefs.GetFloat("SFX", 0.75f);
    }

    // 绑定给 BGM滑块
    public void SetBGM(float vol)
    {
        if (audioMixer == null) return;
        float v = Mathf.Max(vol, 0.001f);
        audioMixer.SetFloat("BGM", Mathf.Log10(v) * 20);
        PlayerPrefs.SetFloat("BGM", vol);
    }

    // 绑定给 音效滑块
    public void SetSFX(float vol)
    {
        if (audioMixer == null) return;
        float v = Mathf.Max(vol, 0.001f);
        audioMixer.SetFloat("SFX", Mathf.Log10(v) * 20);
        PlayerPrefs.SetFloat("SFX", vol);
    }

    public void SetFullScreen(bool isOn)
    {
        Debug.Log("全屏设置被调用了：" + isOn);
        Screen.fullScreen = isOn;
    }

    public void SetQuality(int index)
    {
        Debug.Log("画质设置被调用了：" + index);
        QualitySettings.SetQualityLevel(index);
    }

    public void SetResolution(int index)
    {
        Debug.Log("分辨率设置被调用了：" + index);
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}