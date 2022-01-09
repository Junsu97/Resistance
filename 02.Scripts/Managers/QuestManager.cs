using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class QuestManager : Singleton<QuestManager>
{
    public Dictionary<int, Quest> allQuests;

    public QuestContainer questContainer;

    public GameObject choiceUI;
    public GameObject questUI;
    public Transform questSlotContainer;

    [Header("Quest Info Panel")]
    public GameObject questInfoPanel;
    public TextMeshProUGUI questNameText;
    public VerticalLayoutGroup questInfoLayout;
    public Transform questTaskLayout;
    public GameObject questTaskprefab;
    public Transform questRewardLayout;
    public GameObject questRewardPrefab;
    public TextMeshProUGUI questInfoDesc;

    private void Awake()
    {
        allQuests = new Dictionary<int, Quest>();
    }
    public void questChecker()
    {
        if(GameManager.Instance.npcList != null)
        {
            Dictionary<int, GameObject> npcDic = GameManager.Instance.npcList;

            foreach(Quest quest in questContainer.activeQuests.Values)
            {
                if(quest.questStatus == Quest.QuestStatus.Ready)
                {
                    if(npcDic.ContainsKey(quest.startNpcId))
                    {
                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(true);//UI
                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().HaveQuestIcon.SetActive(true);//Minimap

                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().QuestionMark.SetActive(false);//UI
                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().ClearQuestIcon.SetActive(false); //Minimap
                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().NormalIcon.SetActive(false);
                    }
                }

                else if(quest.questStatus == Quest.QuestStatus.Proceding)
                {
                    bool isGoal = true;
                    foreach(Task task  in quest.tasks)
                    {
                        if(task.taskType == Task.TaskType.Item)
                        {
                            Debug.Log("Item");
                            task.currentCount = InventoryManager.Instance.inventory.GetItemAmount(task.targetId);
                        }

                        if(task.goalCount > task.currentCount)
                        {
                            isGoal = false;
                            break;
                        }
                        if(npcDic.ContainsKey(quest.endNpcId))
                        {
                            if(isGoal)
                            {
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().HaveQuestIcon.SetActive(false);

                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().QuestionMark.SetActive(true);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().ClearQuestIcon.SetActive(true);
                            }
                            else
                            {
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().QuestionMark.SetActive(false);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().HaveQuestIcon.SetActive(false);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().ClearQuestIcon.SetActive(false);

                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().NormalIcon.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ShowNpcQuestList(int npcId)
    {
        for(int i = 0; i < choiceUI.transform.childCount -2; i++)
        {
            ObjectPoolingManager.Instance.InsertQueue(choiceUI.transform.GetChild(i).gameObject, "questChoice");
        }
        // ������ ����Ʈ ����
        foreach(Quest quest in questContainer.activeQuests.Values)
        {
            switch(quest.questStatus)
            {
                case Quest.QuestStatus.Ready:
                    if(quest.startNpcId == npcId)
                    {
                        AddQuestChoiceSlot(quest);
                    }
                    break;
                case Quest.QuestStatus.Proceding:
                    if(quest.endNpcId == npcId)
                    {
                        bool isGoal = true;
                        foreach(Task task in quest.tasks)
                        {
                            if(task.taskType == Task.TaskType.Item)
                            {
                                task.currentCount = InventoryManager.Instance.inventory.GetItemAmount(task.targetId);
                            }

                            if(task.goalCount > task.currentCount)
                            {
                                isGoal = false;
                                break;
                            }
                        }
                        if(isGoal)
                        {
                            AddQuestChoiceSlot(quest);
                        }
                    }
                    break;
            }
        }
    }
    public void AddQuestChoiceSlot(Quest quest)
    {
        // ������ ����Ʈ�� ����, �� ����Ʈ ID�� �ش� ������Ʈ�� ���� QuestChoice�� ����.
        // ����� ID�� �����ʿ� ��ϵ� Ŭ�� �̺�Ʈ�� �߻��� ��, �ش� ID�� �˸��� ����Ʈ�� �ҷ���
        GameObject questChoiceObj = ObjectPoolingManager.Instance.GetQueue("questChoice");
        questChoiceObj.transform.SetParent(choiceUI.transform);
        questChoiceObj.transform.SetAsFirstSibling();
        questChoiceObj.transform.localScale = new Vector3(1, 1, 1);
        
        questChoiceObj.GetComponent<QuestChoice>().quest = quest;
        questChoiceObj.GetComponentInChildren<Text>().text = quest.questName;

        questChoiceObj.GetComponent<Button>().onClick.AddListener(() => { choiceUI.transform.parent.GetComponent<NpcUI>().OnClickQuestBT(questChoiceObj.GetComponent<QuestChoice>());});
    }
    public void QuestClear(int questId)
    {
        Quest quest = questContainer.activeQuests[questId];

        foreach(Task task in quest.tasks)
        {
            if(task.taskType == Task.TaskType.Item)
            {
                InventoryManager.Instance.inventory.ReduceItem(task.targetId, task.goalCount);
            }
        }

        questContainer.completedQuests[questId] = quest;
        questContainer.activeQuests.Remove(questId);

        // linkedQuestId = 0 �ΰ��, ���� ����Ʈ�� �������� �ʴ� ����̴�.
        if(quest.linkedQuestId > 0)
        {
            questContainer.activeQuests[quest.linkedQuestId] = allQuests[quest.linkedQuestId];
            questChecker();
        }

        // ������ ������ ������ ��� ������ ���� & ��庸�� ����
        if(quest.rewardList.Length != 0)
        {
            foreach(Reward reward in quest.rewardList)
            {
                GameManager.Instance.playerStatement.inventory.AddItem(DBManager.Instance.itemDict[reward.itemId], reward.amount);
            }
        }
        GameManager.Instance.playerStatement.inventory.AddGold(quest.rewardGold);
    }

    public void EndQuestTalk(int questId)
    {
        Quest quest = questContainer.activeQuests[questId];

        switch(quest.questStatus)
        {
            case Quest.QuestStatus.Ready:
                GameManager.Instance.npcList[quest.startNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                GameManager.Instance.npcList[quest.startNpcId].GetComponent<NpcInfo>().QuestionMark.SetActive(false);
                GameManager.Instance.npcList[quest.startNpcId].GetComponent<NpcInfo>().HaveQuestIcon.SetActive(false);
                GameManager.Instance.npcList[quest.startNpcId].GetComponent<NpcInfo>().ClearQuestIcon.SetActive(false);

                quest.questStatus = Quest.QuestStatus.Proceding;
                break;
            case Quest.QuestStatus.Proceding:
                GameManager.Instance.npcList[quest.endNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                GameManager.Instance.npcList[quest.endNpcId].GetComponent<NpcInfo>().QuestionMark.SetActive(false);
                GameManager.Instance.npcList[quest.endNpcId].GetComponent<NpcInfo>().HaveQuestIcon.SetActive(false);
                GameManager.Instance.npcList[quest.endNpcId].GetComponent<NpcInfo>().ClearQuestIcon.SetActive(false);

                quest.questStatus = Quest.QuestStatus.Completed;
                QuestClear(questId);
                break;
        }
        questChecker();
    }

    public void TargetEnemyKilled(int targetId)
    {
        foreach(Quest quest in questContainer.activeQuests.Values)
        {
            foreach(Task task in quest.tasks)
            {
                if(task.taskType == Task.TaskType.Hunt && task.targetId == targetId)
                {
                    task.currentCount++;
                    questChecker();
                }
            }
        }
    }

    public void ShowProgressQuest()
    {
        while(questSlotContainer.childCount>0)
        {
            ObjectPoolingManager.Instance.InsertQueue(questSlotContainer.GetChild(0).gameObject, "questSlot");
        }
        foreach(Quest quest  in questContainer.activeQuests.Values)
        {
            if(quest.questStatus == Quest.QuestStatus.Proceding)
            {
                GameObject slotObj = ObjectPoolingManager.Instance.GetQueue("questSlot");
                slotObj.transform.SetParent(questSlotContainer);
                slotObj.transform.localScale = new Vector3(1f, 1f, 1f);

                EventTrigger eventTrigger = slotObj.GetComponent<EventTrigger>();

                EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((data) => { OnSlotPointerDown((PointerEventData)data); });
                eventTrigger.triggers.Add(pointerDown);

                slotObj.GetComponent<QuestSlot>().questId = quest.questId;
                slotObj.GetComponentInChildren<TextMeshProUGUI>().text = quest.questName;
            }
        }
        questUI.SetActive(true);
    }
    private void OnSlotPointerDown(PointerEventData data)
    {
        int questId = data.pointerEnter.GetComponentInParent<QuestSlot>().questId;
        Quest quest = questContainer.activeQuests[questId];

        for(int i = 0; i< questTaskLayout.childCount; i++)
        {
            Destroy(questTaskLayout.GetChild(i).gameObject);
        }
        for(int i =0; i< questRewardLayout.childCount; i++)
        {
            Destroy(questRewardLayout.GetChild(i).gameObject);
        }

        questInfoPanel.SetActive(true);
        questNameText.text = quest.questName;

        foreach(Task task in quest.tasks)
        {
            GameObject taskObj = Instantiate(questTaskprefab, questTaskLayout);
            taskObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = task.goalText;

            if(task.goalCount > 0)
            {
                taskObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Mathf.Clamp(task.currentCount, 0, task.goalCount) + "/" + task.goalCount;
                taskObj.transform.GetChild(1).gameObject.SetActive(true);
            }

            taskObj.SetActive(true);
        }
        foreach (Reward item in quest.rewardList)
        {
            GameObject rewardObj = Instantiate(questRewardPrefab, questRewardLayout);
            rewardObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DBManager.Instance.itemDict[item.itemId].itemName;
            rewardObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = " X " + item.amount;
            rewardObj.SetActive(true);
        }

        // ���� ���
        if (quest.rewardGold != 0)
        {
            GameObject rewardObj = Instantiate(questRewardPrefab, questRewardLayout);
            rewardObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Gold";
            rewardObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = " X " + quest.rewardGold;
            rewardObj.SetActive(true);
        }

        questInfoDesc.text = quest.description;

        Canvas.ForceUpdateCanvases();
        questInfoLayout.SetLayoutVertical();
    }
}
