using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TalkManager : Singleton<TalkManager>
{
    public Dictionary<int, string[]> talkDataList = new Dictionary<int, string[]>();
    public GameObject npcUI;
    public TextMeshProUGUI talkText;
    [HideInInspector]
    public int talkIndex;
 
    public string GetTalk(int id, int talkIndex)
    {
        if(talkIndex == talkDataList[id].Length)
        {
            return null;
        }
        else
        {
            return talkDataList[id][talkIndex];
        }
    }
    public void Talk(int npcId, int questId = 0, int questIndex = 0)
    {
        string talkData = GetTalk(npcId + questId + questIndex, talkIndex);

        if(talkData == null)
        {
            talkIndex = 0;
            npcUI.SetActive(false);

            if(questId != 0)
            {
                QuestManager.Instance.EndQuestTalk(questId);
            }
            return;
        }

        talkText.text = talkData;
        talkIndex++;
       
    }
}
