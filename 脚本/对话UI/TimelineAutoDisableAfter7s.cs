using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections; // 关键修复：加上协程引用

public class TimelineAndDialogueFlow : MonoBehaviour
{
    [Header("Timeline 相关")]
    public PlayableDirector timelineDirector;
    public Camera animationCamera;

    [Header("黑屏对话 UI")]
    public Canvas blackScreenDialogueCanvas;
    public CanvasGroup fadePanelGroup;
    public Text dialogueText;

    [Header("玩家对象")]
    public GameObject player;

    [Header("设置")]
    public float maxDuration = 7f;
    public string dialogue = "动画结束了，按P继续...";
    public float textSpeed = 0.05f;
    public float fadeDuration = 0.8f;

    private bool isTimelineDone = false;
    private bool isDialogueOpen = false;
    private bool playerUnlocked = false;
    private float timer = 0f;
    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;
    bool isPlaying = false;
    void Start()
    {
        if (timelineDirector == null)
            timelineDirector = GetComponent<PlayableDirector>();

        LockPlayer(true);
        blackScreenDialogueCanvas.enabled = false;
        fadePanelGroup.alpha = 0f;
        dialogueText.text = "";
    }

    void Update()
    {
        if (!isTimelineDone)
        {
            timer += Time.deltaTime;

            if (timelineDirector.state != PlayState.Playing)
            {
                FinishTimeline();
                return;
            }

            if (timer >= maxDuration)
            {
                FinishTimeline();
                return;
            }
        }
        else
        {
            if (isPlaying!=true &&Input.GetKeyDown(KeyCode.P))
            {
                ToggleDialogue();
            }
        }
    }

    void FinishTimeline()
    {
        isTimelineDone = true;

        timelineDirector.Stop();
        timelineDirector.enabled = false;
        if (animationCamera != null)
            animationCamera.enabled = false;

        blackScreenDialogueCanvas.enabled = true;
        isDialogueOpen = true;
        fadeCoroutine = StartCoroutine(FadeToBlack());
    }

    void ToggleDialogue()
    {
        if (!isDialogueOpen)
        {
            blackScreenDialogueCanvas.enabled = true;
            isDialogueOpen = true;
            fadeCoroutine = StartCoroutine(FadeToBlack());
        }
        else
        {
            isDialogueOpen = false;
            isPlaying = true;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToTransparent());
        }
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanelGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadePanelGroup.alpha = 1f;

        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator FadeToTransparent()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanelGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadePanelGroup.alpha = 0f;

        blackScreenDialogueCanvas.enabled = false;
        dialogueText.text = "";

        LockPlayer(false);
        playerUnlocked = true;
    }

    IEnumerator TypeText()
    {
        foreach (char c in dialogue.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void LockPlayer(bool lockPlayer)
    {
        if (player == null) return;

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = !lockPlayer;

        var playerController = player.GetComponent<MonoBehaviour>();
        if (playerController != null)
            playerController.enabled = !lockPlayer;
    }
}