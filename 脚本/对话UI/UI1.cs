using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI1 : MonoBehaviour
{
    [Header("UI 根节点")]
    public GameObject dialoguePanel;
    public Text nameText;
    public Text dialogueText;

    [Header("F键提示")]
    public GameObject pressFTip;

    [Header("交互配置")]
    public float checkRadius = 2f;
    public string playerTag = "Player";

    [Header("打字效果")]
    public float typeSpeed = 0.05f;

    [Header("所属任务索引")]
    public int questIndex = 0;

    private int dialogueIndex = 1;
    private string[] dialogueRows;
    private bool playerNearby = false;
    private bool isTyping = false;
    private bool dialogueActive = false;

    private Coroutine currentTypeCoroutine;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (pressFTip != null)
            pressFTip.SetActive(false);
    }

    void Update()
    {
        if (QuestSystem.Instance == null)
        {
            Debug.Log("UI1: QuestSystem.Instance is null");
            return;
        }

        if (!QuestSystem.Instance.IsGameStarted)
        {
            Debug.Log("UI1: Game not started yet");
            return;
        }

        // 检查当前任务是否匹配
        if (QuestSystem.Instance.CurrentQuestIndex != questIndex)
        {
            return;
        }

        // 检查NPC标签是否匹配当前任务
        if (!IsNpcTagMatched())
        {
            return;
        }

        // 检查玩家是否在附近
        CheckPlayerNearby();

        // 如果玩家不在附近，不处理对话逻辑
        if (!playerNearby)
        {
            // 只有当对话是当前NPC打开的时，才关闭对话面板
            // 避免其他NPC关闭当前对话
            return;
        }

        // 自动从QuestDataSO加载对话数据
        LoadCurrentQuestDialogue();

        // 更新F键提示显示
        UpdateFTip();

        // F键打开对话
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("UI1: F key pressed, opening dialogue");
            if (dialoguePanel == null)
            {
                Debug.LogError("UI1: DialoguePanel 未赋值!");
                return;
            }
            if (!dialoguePanel.activeSelf)
                OpenDialogue();
        }

        // E键加速/下一句
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
                SkipType();
            else
                NextSentence();
        }

        // ESC关闭对话
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogue();
        }
    }

    bool IsNpcTagMatched()
    {
        if (QuestSystem.Instance == null) return false;

        QuestDataSO currentQuest = QuestSystem.Instance.GetCurrentQuestData();
        if (currentQuest == null) return false;

        string npcTag = currentQuest.npcTag;
        bool isMatched = gameObject.CompareTag(npcTag);
        Debug.Log($"UI1: NPC tag={gameObject.tag}, quest npcTag={npcTag}, matched={isMatched}");
        return isMatched;
    }

    void CheckPlayerNearby()
    {
        playerNearby = false;
        Collider[] cols = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (var c in cols)
        {
            if (c != null && c.CompareTag(playerTag))
            {
                playerNearby = true;
                break;
            }
        }
    }

    void UpdateFTip()
    {
        if (pressFTip != null && dialoguePanel != null)
        {
            pressFTip.SetActive(playerNearby && !dialoguePanel.activeSelf);
        }
    }

    void OpenDialogue()
    {
        Debug.Log("UI1: OpenDialogue called");
        dialogueIndex = 1;
        dialogueActive = true;
        bool hasDialogue = ShowDialogueRow();
        Debug.Log($"UI1: hasDialogue = {hasDialogue}");
        if (hasDialogue && dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            QuestSystem.Instance.SetDialogueActive(true);
            Debug.Log("UI1: Dialogue panel activated");
        }
        else
        {
            Debug.LogWarning("UI1: No dialogue data or dialoguePanel is null");
        }
    }

    void NextSentence()
    {
        dialogueIndex++;
        bool hasMore = ShowDialogueRow();

        if (!hasMore)
        {
            CompleteDialogue();
        }
    }

    void CompleteDialogue()
    {
        dialogueActive = false;
        CloseDialogue();

        if (QuestSystem.Instance != null)
        {
            QuestSystem.Instance.NotifyQuestComplete(questIndex);
        }
    }

    void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        dialogueIndex = 1;

        if (currentTypeCoroutine != null)
        {
            StopCoroutine(currentTypeCoroutine);
            currentTypeCoroutine = null;
        }
        isTyping = false;
        dialogueActive = false;

        if (QuestSystem.Instance != null)
            QuestSystem.Instance.SetDialogueActive(false);
    }

    void SkipType()
    {
        if (currentTypeCoroutine != null)
        {
            StopCoroutine(currentTypeCoroutine);
            currentTypeCoroutine = null;
        }
        isTyping = false;

        foreach (var row in dialogueRows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            string[] cells = row.Split(',');
            if (cells.Length < 6) continue;
            if (int.TryParse(cells[0], out int idx) && idx == dialogueIndex)
            {
                if (dialogueText != null) dialogueText.text = cells[4];
                break;
            }
        }
    }

    void LoadCurrentQuestDialogue()
    {
        if (QuestSystem.Instance == null) return;

        QuestDataSO questData = QuestSystem.Instance.GetQuestData(questIndex);
        if (questData != null && questData.dialogueData != null)
        {
            LoadDialogue(questData.dialogueData);
        }
    }

    public void LoadDialogue(TextAsset asset)
    {
        if (asset == null) return;
        dialogueRows = asset.text.Split('\n');
    }

    public bool ShowDialogueRow()
    {
        if (dialogueText == null || nameText == null)
        {
            Debug.LogError("UI1: NameText 或 DialogueText 未赋值!");
            return false;
        }

        foreach (var row in dialogueRows)
        {
            if (string.IsNullOrWhiteSpace(row)) continue;
            string[] cells = row.Split(',');
            if (cells.Length < 6) continue;

            if (int.TryParse(cells[0], out int idx) && idx == dialogueIndex)
            {
                UpdateText(cells[2], cells[4]);

                if (currentTypeCoroutine != null)
                    StopCoroutine(currentTypeCoroutine);

                currentTypeCoroutine = StartCoroutine(TypeSentence(cells[4]));
                return true;
            }
        }
        return false;
    }

    void UpdateText(string name, string text)
    {
        if (nameText != null)
            nameText.text = name;
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (dialogueText == null) yield break;
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        currentTypeCoroutine = null;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}