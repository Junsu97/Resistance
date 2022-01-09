using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject inven;
    public GameObject info;
    public GameObject others;

    public void OnOnIfoBTClicked()
    {
        info.SetActive(true);
    }
    public void OnInvenBTClicked()
    {
        InventoryManager.Instance.ResetInven();
        inven.SetActive(true);
    }
    public void OnQuestBTClicked()
    {
        QuestManager.Instance.ShowProgressQuest();
        QuestManager.Instance.questInfoPanel.SetActive(false);
    }

    public void OnQuitBTClicked()
    {
        GameManager.Instance.quitPanel.SetActive(true);
    }
}
