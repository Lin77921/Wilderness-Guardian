using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestPanel : MonoBehaviour
{
    [Header("UI 组件")]
    public Text questNameText;
    public Text questTipText;

    [Header("动画配置")]
    public float fadeDuration = 0.3f;
    public float stayDuration = 3f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        SetVisible(false);
    }

    void Update()
    {
        if (QuestSystem.Instance == null || !QuestSystem.Instance.IsGameStarted)
            return;

        QuestDataSO currentQuest = QuestSystem.Instance.GetCurrentQuestData();
        if (currentQuest != null)
        {
            UpdateQuestInfo(currentQuest);
        }
    }

    void UpdateQuestInfo(QuestDataSO quest)
    {
        if (questNameText != null)
            questNameText.text = quest.questName;

        if (questTipText != null)
            questTipText.text = quest.questTip;
    }

    public void Show()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInOut());
    }

    IEnumerator FadeInOut()
    {
        // 淡入
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1);
        SetVisible(true);

        // 停留
        yield return new WaitForSeconds(stayDuration);

        // 淡出
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0);
        SetVisible(false);

        fadeCoroutine = null;
    }

    public void ShowPermanent()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        SetVisible(true);
        SetAlpha(1);
    }

    public void Hide()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        SetVisible(false);
        SetAlpha(0);
    }

    void SetVisible(bool visible)
    {
        if (questNameText != null)
            questNameText.enabled = visible;

        if (questTipText != null)
            questTipText.enabled = visible;
    }

    void SetAlpha(float alpha)
    {
        if (questNameText != null)
        {
            Color color = questNameText.color;
            color.a = alpha;
            questNameText.color = color;
        }

        if (questTipText != null)
        {
            Color color = questTipText.color;
            color.a = alpha;
            questTipText.color = color;
        }
    }
}