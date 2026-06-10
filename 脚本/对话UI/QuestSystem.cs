using UnityEngine;
using System.Collections;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }

    [Header("任务数据配置")]
    public QuestDataSO[] questDatas;

    [Header("任务面板")]
    public QuestPanel questPanel;

    [Header("任务状态")]
    private int currentQuestIndex = -1;
    private bool isGameStarted = false;
    private bool isDialogueActive = false;

    public bool IsGameStarted => isGameStarted;
    public int CurrentQuestIndex => currentQuestIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (!isGameStarted && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("QuestSystem: P key pressed, starting game");
            StartGame();
        }

        // 按O键重新显示当前任务信息
        if (isGameStarted && Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("QuestSystem: O key pressed, showing current quest");
            ShowCurrentQuestInfo();
        }
    }

    void StartGame()
    {
        if (questDatas == null || questDatas.Length == 0)
        {
            Debug.LogError("QuestSystem: questDatas is null or empty");
            return;
        }

        isGameStarted = true;
        Debug.Log("QuestSystem: Game started, starting quest 0");
        StartQuest(0);
    }

    void StartQuest(int questIndex)
    {
        if (questIndex >= questDatas.Length)
        {
            Debug.LogError($"QuestSystem: questIndex {questIndex} out of range");
            return;
        }

        // 检查前置任务是否完成
        if (questIndex > 0)
        {
            QuestDataSO prevQuest = questDatas[questIndex - 1];
            if (prevQuest != null && !prevQuest.isCompleted)
            {
                Debug.Log($"[QuestSystem] 任务 {questIndex} 前置任务未完成");
                return;
            }
        }

        currentQuestIndex = questIndex;
        QuestDataSO quest = questDatas[questIndex];

        if (quest == null)
        {
            Debug.LogError($"QuestSystem: questDatas[{questIndex}] is null");
            return;
        }

        // 显示任务面板
        if (questPanel != null)
        {
            Debug.Log($"QuestSystem: Showing quest panel for quest {questIndex}");
            questPanel.Show();
        }
        else
        {
            Debug.LogWarning("QuestSystem: questPanel is null");
        }

        // 如果任务需要NPC，激活对话UI
        if (quest.needNpc)
        {
            if (quest.dialogueUI != null)
            {
                quest.dialogueUI.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"[QuestSystem] 任务 {questIndex + 1} 不需要NPC，只显示UI");
        }

        Debug.Log($"[QuestSystem] 开始任务 {questIndex + 1}: {quest.questName}");
    }

    public void NotifyQuestComplete(int questIndex)
    {
        if (questIndex < 0 || questIndex >= questDatas.Length) return;

        QuestDataSO quest = questDatas[questIndex];
        if (quest == null) return;

        quest.isCompleted = true;
        Debug.Log($"[QuestSystem] 任务 {questIndex + 1} 完成: {quest.questName}");

        // 隐藏当前任务UI
        if (quest.dialogueUI != null)
        {
            quest.dialogueUI.gameObject.SetActive(false);
        }

        // 触发下一个任务
        if (questIndex + 1 < questDatas.Length)
        {
            StartQuest(questIndex + 1);
        }
        else
        {
            Debug.Log("[QuestSystem] 所有任务已完成");
        }
    }

    public bool IsQuestCompleted(int questIndex)
    {
        if (questIndex < 0 || questIndex >= questDatas.Length) return false;
        return questDatas[questIndex] != null && questDatas[questIndex].isCompleted;
    }

    public QuestDataSO GetQuestData(int questIndex)
    {
        if (questIndex < 0 || questIndex >= questDatas.Length) return null;
        return questDatas[questIndex];
    }

    public QuestDataSO GetCurrentQuestData()
    {
        return GetQuestData(currentQuestIndex);
    }

    public void SetDialogueActive(bool active)
    {
        isDialogueActive = active;
    }

    void ShowCurrentQuestInfo()
    {
        if (questPanel != null && currentQuestIndex >= 0 && currentQuestIndex < questDatas.Length)
        {
            questPanel.Show();
            Debug.Log($"QuestSystem: Showing quest {currentQuestIndex + 1} info");
        }
        else
        {
            Debug.LogWarning("QuestSystem: questPanel is null or no active quest");
        }
    }
}