using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [Header("Main Canvas")]
    public GameObject mainCanvas;
    public GameObject quitPanel;
    public GameObject diePanel;
    public GameObject npcUI;
    public GameObject loadingPanel;
    public Slider loadingProgressBar;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI message;
    public TextMeshProUGUI MapName;
    public Image NameBack;
    public GameObject LastScenePanel;
    public GameObject BossScenePanel;
    private string thisMapName;
    [Header("Player")]
    public PlayerCtrl player;
    public CamPivot cam;
    public Camera mainCam;
    public PlayerStatement playerStatement;
    public Transform SpawnPos;
    public NavMeshAgent p_nav;
    private Animator ani;
    private GameObject In_Dungeon;
    private GameObject In_Village;
    public bool canAttack;
    [HideInInspector]
    public bool isRespawn = false;
    public string prevSceneName;
    public string currentSceneName;
    public Vector3 currentPos;
    public Quaternion currentRot;
    public JoyStick stick;

    [Header("LevelUP UI")]
    public GameObject levelUpObj;
    public TextMeshProUGUI levelText;

    [Header("Npcs Panrent")]
    public GameObject npcObjs;
    public Dictionary<int, GameObject> npcList;
    public int currentInteractId;
  
    void Awake()
    {
        Application.targetFrameRate = 60;
        PlayerInit();
        mainCanvas.SetActive(false);
        npcList = new Dictionary<int, GameObject>();
        ani = player.transform.GetComponent<Animator>();
        In_Dungeon = playerStatement.In_Dungeon;
        In_Village = playerStatement.In_Village;
    }
    private void Start()
    {
        BGMManager.Instance.PlayBgm("Lobby");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void PlayerInit()
    {
        playerStatement = player.GetComponent<PlayerStatement>();
        playerStatement.inventory = new Inventory();
        playerStatement.SetComponents();
        InventoryManager.Instance.SetInven(playerStatement.inventory);
        playerStatement.equipList = new Dictionary<Item.ItemType, Item>();
    }
    public void LoadScene(string nextSceneName)
    {
        loadingProgressBar.value = 0f;
        loadingText.text = "Loading... 0%";
        
        BGMManager.Instance.StopBgm();
        loadingPanel.SetActive(true);
        mainCanvas.SetActive(true);
        p_nav.enabled = false;
        prevSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneCoroutine(nextSceneName));
    }

    public void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        BGMManager.Instance.PlayBgm(currentSceneName);
        CurrentMapName(currentSceneName);
        StartCoroutine(ClosedName());
        if (isRespawn)
        {
            SpawnPos = GameObject.Find("Start").transform;
            isRespawn = false;
        }

        else
        {
            if(prevSceneName == "Lobby")
            {
                SpawnPos = new GameObject().transform;
                SpawnPos.position = currentPos;
                SpawnPos.rotation = currentRot;
            }
            else
            {
                SpawnPos = GameObject.Find(prevSceneName).transform;
            }
        }
        if(currentSceneName == "Level_3")
        {
            mainCam.farClipPlane = 1000f;
        }
        else
        {
            mainCam.farClipPlane = 60f;
        }

        player.transform.position = SpawnPos.position;
        player.transform.rotation = SpawnPos.rotation;
        player.gameObject.SetActive(true);
        p_nav.enabled = true;
        ChangeAnimator();
        npcList.Clear();
        if((npcObjs = GameObject.Find("NPC")) != null)
        {
            for(int i = 0; i < npcObjs.transform.childCount; i++)
            {
                GameObject npcObj = npcObjs.transform.GetChild(i).gameObject;
                npcList[npcObj.GetComponent<NpcInfo>().npcId] = npcObj;
            }

            QuestManager.Instance.questChecker();
        }
        loadingPanel.SetActive(false);
    }
    void ChangeAnimator()
    {
        string SceneName = currentSceneName;
        if (SceneName != null)
        {
            if (SceneName != "Level_1" && SceneName != "Lobby")
            {
                if (ani.runtimeAnimatorController.name == "PlayerVillageAnimator")
                {
                    ani.runtimeAnimatorController = Resources.Load("Player/Animator/PlayerBattleAnimator") as RuntimeAnimatorController;
                }
                if (!In_Dungeon.activeSelf && In_Village.activeSelf)
                {
                    In_Dungeon.SetActive(true);
                    In_Village.SetActive(false);
                    canAttack = true;
                }
            }

            else if (SceneName == "Level_1" || SceneName == "Lobby")
            {
                if (ani.runtimeAnimatorController.name == "PlayerBattleAnimator")
                {
                    ani.runtimeAnimatorController = Resources.Load("Player/Animator/PlayerVillageAnimator") as RuntimeAnimatorController;
                }
                if (In_Dungeon.activeSelf && !In_Village.activeSelf)
                {
                    In_Village.SetActive(true);
                    In_Dungeon.SetActive(false);
                    canAttack = false;
                }
            }
        }
    }
    private void CurrentMapName(string map)
    {

        switch (map)
        {
            case "Level_1":
                thisMapName = "<Village>";
                break;
            case "Level_2":
                thisMapName = "<In front of the wall.>";
                break;
            case "Dungeon_2":
                thisMapName = "<The den of plague>";
                break;
            case "Boss":
                thisMapName = "<Dragon nest>";
                break;
        }
        NameBack.gameObject.SetActive(true);
        MapName.gameObject.SetActive(true);
        MapName.text = thisMapName;
    }


    private IEnumerator ClosedName()
    {
        float Btimer = 0.2f;
        float Ttimer = 1f;
        NameBack.color = new Color(1, 1, 1, 1);
        MapName.color = new Color(0, 0, 0, 1);
        while (NameBack.gameObject.activeInHierarchy)
        {
            yield return null;
            Btimer -= (Time.deltaTime * 0.1f);
            Ttimer -= (Time.deltaTime * 0.1f);

            NameBack.color = new Color(1, 1, 1, Btimer);
            MapName.color = new Color(0, 0, 0, Ttimer);

            if (NameBack.color.a < 0.01f)
            {
                NameBack.gameObject.SetActive(false);
                yield break;
            }
        }
    }
    IEnumerator LoadSceneCoroutine(string nextSceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;
        float timer = 0f;

        while(!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if(op.progress < 0.9f)
            {
                loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, op.progress, timer);
                if(loadingProgressBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, 1f, timer);
                if(loadingProgressBar.value == 1f)
                {
                    op.allowSceneActivation = true;
                }
            }
            loadingText.text = "Loading..." + ((int)(loadingProgressBar.value * 100)).ToString() + "%";
        }
    }

    public void GetExp(int exp)
    {
        playerStatement.exp += exp;

        int count = 0;
        while(playerStatement.level + count < DBManager.Instance.playerStatDict.Count && DBManager.Instance.playerStatDict[playerStatement.level + count].exp <= playerStatement.exp)
        {
            count++;
        }
        if(count != 0)
        {
            LevelUp(count);
        }
    }

    private void LevelUp(int count)
    {
        playerStatement.level += count;
        playerStatement.baseMaxHealth = DBManager.Instance.playerStatDict[playerStatement.level].health;
        playerStatement.baseMaxMana = DBManager.Instance.playerStatDict[playerStatement.level].mana;
        playerStatement.baseManaRegeneration = DBManager.Instance.playerStatDict[playerStatement.level].manaRegeneration;
        playerStatement.baseDamage = DBManager.Instance.playerStatDict[playerStatement.level].damage;

        playerStatement.ApplyItems();

        levelText.text = playerStatement.level.ToString();
        levelUpObj.SetActive(true);

        StartCoroutine(DisappearLevelUpObj());
        Debug.Log("·¹º§¾÷!" + playerStatement.level);
    }

    private IEnumerator DisappearLevelUpObj()
    {
        WaitForSeconds ws = new WaitForSeconds(3f);

        yield return ws;
        levelUpObj.SetActive(false);
    }
    public void EnterBossStage()
    {
        BGMManager.Instance.StopBgm();
        BGMManager.Instance.PlayBgm("Boss");
        BossSceneDirector.Instance.StartTimeLine();
        Enemy[] enemy = FindObjectsOfType<Enemy>();
        for(int i = 0; i < enemy.Length; i++)
        {
            enemy[i].targetObj = null;
        }
    }
    public void ExitBossStage()
    {
        BossSceneDirector.Instance.ExitBoss();
    }
    public void ToVillageBTClicked()
    {
        LoadScene("Level_1");
        LastScenePanel.SetActive(false);
    }
    public void ToGateBTClicked()
    {
        LoadScene("Level_2");
        LastScenePanel.SetActive(false);
    }

    public void DieEnable()
    {
        diePanel.SetActive(true);
        Debug.Log("EndGame");
    }

    public void OnRespawnBTClicked()
    {
        playerStatement.ResetPlayerState();
        player.transform.tag = "Player";
        diePanel.SetActive(false);
        isRespawn = true;
        LoadScene("Level_1");
    }

    public void OnQuitBTClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
