using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcUI : MonoBehaviour
{
    public Quest currentQuest;

    public GameObject choiceUI;
    public GameObject talkUI;
    public GameObject storeUI;

    public GameObject talkChoice;
    public GameObject storeChoice;

    public Canvas canvas;
    public Transform camTr;
    public IEnumerator coroutine;

    public void OnEnable()
    {
        choiceUI.SetActive(false);
        talkUI.SetActive(false);
        storeUI.SetActive(false);
    }

    // 선택에 따른 활성.비활성화
    public void SetChoiceUI(bool hasTalk, bool hasStore)
    {
        talkChoice.SetActive(hasTalk);
        storeChoice.SetActive(hasStore);

        choiceUI.SetActive(true);
    }

    public void OnClickStoreBT()
    {
        choiceUI.SetActive(false);
        storeUI.SetActive(true);
    }

    public void OnClickTalkBT()
    {
        TalkManager.Instance.talkIndex = 0;

        currentQuest = null;
        TalkManager.Instance.Talk(GameManager.Instance.currentInteractId);
        
        choiceUI.SetActive(false);
        talkUI.SetActive(true);
        Debug.Log(TalkManager.Instance.talkIndex);
    }
    public void OnClickQuestBT(QuestChoice questChoice)
    {
        TalkManager.Instance.talkIndex = 0;

        currentQuest = questChoice.quest;
        TalkManager.Instance.Talk(GameManager.Instance.currentInteractId, currentQuest.questId, (int)currentQuest.questStatus);

        choiceUI.SetActive(false);
        talkUI.SetActive(true);
    }
    public void OnClickTalkBox()
    {
        if(currentQuest == null)
        {
            TalkManager.Instance.Talk(GameManager.Instance.currentInteractId);
        }
        else
        {
            TalkManager.Instance.Talk(GameManager.Instance.currentInteractId, currentQuest.questId, (int)currentQuest.questStatus);
        }
    }
}
