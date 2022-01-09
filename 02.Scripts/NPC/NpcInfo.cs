using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfo : MonoBehaviour
{
    [Header("Npc Datas")]
    public int npcId;
    public string npcName;

    [Header("Npc Elements")]
    public bool hasTalk, hasStroe;

    [Header("Npc Detect Player")]
    public GameObject interactArea;

    [Header("Quest Marks")]
    public GameObject QuestionMark;
    public GameObject exclamationMark;

    [Header("MinimapIcon")]
    public GameObject NormalIcon;
    public GameObject HaveQuestIcon;
    public GameObject ClearQuestIcon;

    private void OnEnable()
    {
        interactArea.GetComponent<InteractArea>().CollisionEnterEvent += InteractAreaEnter;
        interactArea.GetComponent<InteractArea>().CollisionExitEvent += InteractAreaExit;
    }

    private void InteractAreaEnter(Collider collider)
    {
        GameManager.Instance.npcUI.SetActive(true);
        GameManager.Instance.currentInteractId = npcId;
        QuestManager.Instance.ShowNpcQuestList(npcId);
        GameManager.Instance.npcUI.GetComponent<NpcUI>().SetChoiceUI(hasTalk, hasStroe);
    }

    private void InteractAreaExit(Collider collider)
    {
        GameManager.Instance.npcUI.SetActive(false);
    }
}
