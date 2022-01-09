using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DBManager : Singleton<DBManager>
{
    [Serializable]
    public class Serialization<TKey, TValue>
    {
        //[SerializeField]
        //List<TKey> keys;
        //[SerializeField]
        //List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> GetDictionarty() { return target; }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }
    }
    string jsonPath;

    public PlayerData playerData;
    public TalkDataList talkDataList;

    public Dictionary<int, Item> itemDict;
    public Dictionary<int, PlayerStat> playerStatDict;
    public Dictionary<int, EnemyData> enemyDict;

    private void Awake()
    {
        jsonPath = Application.persistentDataPath;
        talkDataList = new TalkDataList();
        playerData = new PlayerData();

        itemDict = new Dictionary<int, Item>();
        playerStatDict = new Dictionary<int, PlayerStat>();
        enemyDict = new Dictionary<int, EnemyData>();
    }
    private void Start()
    {
        LoadItemListFromJson();
        LoadQuestListFromJson();
        LoadTalkListFromJson();
        LoadExpListFromJson();
        LoadEnemyListFromJson();

        LoadPlayerDataFromJson();
    }
    //���� �÷��̾� �����͸�json���� ����
    public void SavePlayerData(bool isExist = true)
    {
        if(isExist)
        {
            playerData.level = GameManager.Instance.playerStatement.level;
            playerData.health = GameManager.Instance.playerStatement.health;
            playerData.baseMaxHealth = GameManager.Instance.playerStatement.baseMaxHealth;
            playerData.mana = GameManager.Instance.playerStatement.mana;
            playerData.baseMaxMana = GameManager.Instance.playerStatement.baseMaxMana;
            playerData.baseManaRegeneration = GameManager.Instance.playerStatement.baseManaRegeneration;
            playerData.baseDamage = GameManager.Instance.playerStatement.baseDamage;
            playerData.baseCriticalChance = GameManager.Instance.playerStatement.baseCriticalChance;

            playerData.exp = GameManager.Instance.playerStatement.exp;
            playerData.gold = GameManager.Instance.playerStatement.inventory.gold;

            playerData.hasItemList = GameManager.Instance.playerStatement.inventory.GetItemList();
            playerData.equipItemList = GameManager.Instance.playerStatement.equipList.Values.ToList();

            playerData.questContainer = QuestManager.Instance.questContainer;
            playerData.questContainer.DictionaryToList();

            playerData.currentSceneName = GameManager.Instance.currentSceneName;
            playerData.currentPos = GameManager.Instance.player.transform.position;
            playerData.currentRot = GameManager.Instance.player.transform.rotation;
        }
        else
        {
            GameManager.Instance.isRespawn = true;
            playerData.level = 1;
            playerData.baseMaxHealth = playerStatDict[1].health;
            playerData.health = playerData.baseMaxHealth;
            playerData.baseMaxMana = playerStatDict[1].mana;
            playerData.mana = playerData.baseMaxMana;
            playerData.baseManaRegeneration = playerStatDict[1].manaRegeneration;
            playerData.baseDamage = playerStatDict[1].damage;
            playerData.baseCriticalChance = 0f;
            playerData.questContainer.activeQuests[10] = QuestManager.Instance.allQuests[10];
            playerData.questContainer.DictionaryToList();

            playerData.currentSceneName = "Level_1";
        }

        string fileName = "playerData.json";
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(jsonPath + "/" + fileName, jsonData);
    }

    //json ������ �÷��̾� �����ͷκ��� ���ӻ����� �÷��̾� �����͸� �ҷ���
    public void LoadPlayerDataFromJson()
    {
        string fileName = "playerData.json";

        if(!File.Exists(jsonPath + "/"+ fileName))
        {
            SavePlayerData(false);
        }
        string jsonData = File.ReadAllText(jsonPath + "/" + fileName);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);

        GameManager.Instance.playerStatement.level = playerData.level;
        GameManager.Instance.playerStatement.health = playerData.health;
        GameManager.Instance.playerStatement.mana = playerData.mana;

        GameManager.Instance.playerStatement.baseMaxHealth = playerData.baseMaxHealth;
        GameManager.Instance.playerStatement.baseMaxMana = playerData.baseMaxMana;
        GameManager.Instance.playerStatement.baseManaRegeneration = playerData.baseManaRegeneration;
        GameManager.Instance.playerStatement.baseDamage = playerData.baseDamage;
        GameManager.Instance.playerStatement.baseCriticalChance = playerData.baseCriticalChance;

        GameManager.Instance.playerStatement.inventory.itemList = playerData.hasItemList;

        QuestManager.Instance.questContainer = playerData.questContainer;
        QuestManager.Instance.questContainer.ListToDictionary();

        for(int i = 0; i< playerData.equipItemList.Count; i++)
        {
            GameManager.Instance.playerStatement.EquipItem(playerData.equipItemList[i]);
        }

        GameManager.Instance.playerStatement.ApplyItems();

        GameManager.Instance.playerStatement.healthSlier.value = GameManager.Instance.playerStatement.health;
        GameManager.Instance.playerStatement.manaSlider.value = GameManager.Instance.playerStatement.mana;

        GameManager.Instance.playerStatement.exp = playerData.exp;
        GameManager.Instance.playerStatement.inventory.gold = playerData.gold;

        GameManager.Instance.currentSceneName = playerData.currentSceneName;
        GameManager.Instance.currentPos = playerData.currentPos;
        GameManager.Instance.currentRot = playerData.currentRot;
    }

    /* Json ������ ������ ����Ʈ���� ��� ������ �����͸� �ҷ��� allItemList�� �����Ѵ�. 
     * �̹� ��������ü�� itemNo�� ����������, 
     * �˻��� ���̼��� ���� itemNo�� Ű������ �ϴ� ��ųʸ��� ����
     */
    public void LoadItemListFromJson()
    {
        string fileName = "itemList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        ItemList tmp = JsonUtility.FromJson<ItemList>(jsonData.ToString());

        if (tmp.itemList.Count < 1)
        {
            Debug.Log("������ ������ ����");
        }
        for(int i = 0; i< tmp.itemList.Count; i++)
        {
            itemDict[tmp.itemList[i].itemNo] = tmp.itemList[i];
        }
    }

    // Json ������ ����Ʈ ����Ʈ���� ��� ����Ʈ �����͸� �ҷ��� ����
    public void LoadQuestListFromJson()
    {
        string fileName = "QuestList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        QuestList questlist = JsonUtility.FromJson<QuestList>(jsonData.ToString());

        foreach(Quest quest in questlist.quests)
        {
            QuestManager.Instance.allQuests[quest.questId] = quest;
        }
        for(int i = 0; i < QuestManager.Instance.allQuests.Count; i++)
        {
            Debug.Log(QuestManager.Instance.allQuests[10].rewardList);
        }
    }

    /* Json������ ��ȭ ����Ʈ���� ��� ��ȭ �����͸� �ҷ��� talkDataList�� �����Ѵ�.
     * talkManager���� �ش� ��ȭ�� ��ųʸ� ���·� Ȱ���ϱ� ������ talkManager�� talkData����
     * ����Ʈ �����͸� ��ųʸ� ���·� �����Ѵ�.
     */
    public void LoadTalkListFromJson()
    {
        string fileName = "TalkList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        talkDataList = JsonUtility.FromJson<TalkDataList>(jsonData.ToString());

        foreach(TalkData talkData in talkDataList.talkDatas)
        {
            TalkManager.Instance.talkDataList.Add(talkData.id, talkData.scripts.ToArray());
        }
    }

    public void LoadExpListFromJson()
    {
        string fileName = "PlayerStatTable";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        PlayerStatTable tmp = JsonUtility.FromJson<PlayerStatTable>(jsonData.ToString());

        for(int i = 0; i < tmp.playerStatTable.Count; i++)
        {
            playerStatDict[i + 1] = tmp.playerStatTable[i];
        }
    }

    public void LoadEnemyListFromJson()
    {
        string fileName = "EnemyList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        EnemyDataList temp = JsonUtility.FromJson<EnemyDataList>(jsonData.ToString());

        if(temp.enemyList.Count < 1)
        {
            Debug.Log("���� ������ ����");
        }
        for (int i = 0; i < temp.enemyList.Count; i++)
        {
            enemyDict[temp.enemyList[i].enemyId] = temp.enemyList[i];
            Debug.Log(temp.enemyList[i].enemyName);
        }
    }
}
