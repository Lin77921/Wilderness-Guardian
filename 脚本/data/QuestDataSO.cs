using UnityEngine;

[CreateAssetMenu(fileName = "任务数据", menuName = "任务/任务配置")]
public class QuestDataSO : ScriptableObject
{
    [Header("任务信息")]
    public string questName;       //任务名字
    [TextArea(3, 5)]
    public string questTip;        //任务提示

    [Header("任务类型")]
    public bool needNpc = true;    //是否需要NPC触发（false表示只显示UI的任务）

    [Header("NPC配置")]
    public string npcTag;          //触发任务的NPC标签
    public TextAsset dialogueData; //对话内容CSV

    [Header("UI配置")]
    public MonoBehaviour dialogueUI; //关联的对话UI脚本

    [Header("任务状态（程序自动管理）")]
    public bool isCompleted = false; //任务完成标志
}