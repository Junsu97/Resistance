using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class PlayerAttack : MonoBehaviour
{
    protected Animator ani;
    protected PlayerCtrl player;
    [Header("Attack")]
    public Button AtkBT;
    [Header("Skill")]
    public Button SkillBT;
    public Text counter_S;
    public Image time_S;
    protected float cool_S = 10f;
    public float currentCool_S;
    public bool skill = true;
    [Header("SPSkill")]
    public Image time_SP;
    protected float cool_SP = 40f;
    public bool skill_2 = true;
    public Button SPBT;
    [Header("Dodge")]
    public Button DodgeBT;
    public Text counter_D;
    public Image time_D;
    protected float cool_D = 1.5f;
    public float currentCool_D;
    public bool canDodge = true;

    [Header("Effect")]
    public GameObject atk1;
    public GameObject atk2;
    public GameObject atk3;
    public GameObject atk4;
    public GameObject Skill_1;
    public GameObject Skill_2;
    public GameObject Spark;
    [Header("메세지")]
    public TextMeshProUGUI message;

    [Header("사운드")]
    public AudioClip[] attackClip;
    public AudioClip skillClip, skill2Clip;
    protected AudioSource audioSource;
    //[Header("TimeLine")]
    //public PlayableDirector playableDirector;
    //public TimelineAsset timeline;

    public int atkCount = 0;

    protected bool ComboPossible;
    protected PlayerStatement playerStatement;

    protected List<Skill> skills = new List<Skill>();
    protected Skill[] selectedSkills = new Skill[1];
    public GameObject meleeArea;
    protected virtual void Awake()
    {
        ani = GetComponent<Animator>();
        playerStatement = GameManager.Instance.playerStatement;
        player = GameManager.Instance.player;
        message = GameManager.Instance.message;
        audioSource = GetComponent<AudioSource>();

        AtkBT.onClick.AddListener(InputAtk);
        SkillBT.onClick.AddListener(InputSkill);
        DodgeBT.onClick.AddListener(InputDodge);
        SPBT.onClick.AddListener(InputSPSkill);
    }
    protected void Start()
    {
        SPBT.gameObject.SetActive(false);
        atk1.SetActive(false);
        atk2.SetActive(false);
        atk3.SetActive(false);
        atk4.SetActive(false);
        Skill_1.SetActive(false);
        Skill_2.SetActive(false);
        Spark.SetActive(false);
    }

    public GameObject FindNearestObjectByTag(string tag)
    {
        // 탐색할 오브젝트 목록을 List 로 저장합니다.
        var objects = GameObject.FindGameObjectsWithTag(tag).ToList();

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var neareastObject = objects
            .OrderBy(obj =>
            {
                return Vector3.Distance(transform.position, obj.transform.position);
            })
        .FirstOrDefault();

        return neareastObject;
    }
    /*******************************************/
    public void InputAtk()
    {
        Attack();
    }
    public void InputSkill()
    {
        Skill();
    }
    public void InputDodge()
    {
        Dodge();
    }
    protected virtual void Attack()
    {
    }
    protected virtual void Skill()
    {
    }
    protected virtual void Dodge()
    {
    }
   
    /*************************************************************/
    public bool CalcCrit()
    {
        float rand = Random.Range(1f, 100f);
        return rand <= playerStatement.totalCriticalChance;
    }
    public void comboPossible()
    {
        ComboPossible = true;
    }
    public void Combo()
    {
        if (atkCount == 2)
        {
            ani.Play("Attack_2");
            playerStatement.currentState = PlayerStatement.State.Attack;
        }
        if (atkCount == 3)
        {
            ani.Play("Attack_3");
            playerStatement.currentState = PlayerStatement.State.Attack;
        }
        if (atkCount == 4)
        {
            ani.Play("Attack_4");
            playerStatement.currentState = PlayerStatement.State.Attack;
        }
    }

    public void ComboReset()
    {
        ComboPossible = false;
        atkCount = 0;
        playerStatement.currentState = PlayerStatement.State.Idle;
    }
   
    /*********************************************/
    protected IEnumerator CoolTime_Dodge()
    {
        while (time_D.fillAmount > 0)
        {
            time_D.fillAmount -= 1 * Time.deltaTime / cool_D;
            yield return null;
        }
        canDodge = true;
        yield break;
    }
    protected IEnumerator CoolText_Dodge()
    {
        while (currentCool_D > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currentCool_D -= 1.0f;
            counter_D.text = "" + currentCool_D;
        }
        counter_D.text = "";
        yield break;
    }
    public void DodgeToIdleState()
    {
        playerStatement.currentState = PlayerStatement.State.Idle;
        player.tag = "Player";
    }
    /********************************************************/

    protected IEnumerator CoolTime_Skill()
    {
        while (time_S.fillAmount > 0)
        {
            time_S.fillAmount -= 1 * Time.deltaTime / cool_S;
            yield return null;
        }
        skill = true;
        yield break;
    }
    protected IEnumerator CoolText_Skill()
    {
        while (currentCool_S > 0)
        {
            yield return new WaitForSeconds(1.0f);

            currentCool_S -= 1.0f;
            counter_S.text = "" + currentCool_S;
        }
        counter_S.text = "";
        yield break;
    }
    /**************************************************************/
    public void SPSkill()
    {
        if (skill_2)
        {
            SPBT.gameObject.SetActive(true);
            time_SP.gameObject.SetActive(false);
        }
    }
    protected virtual void InputSPSkill()
    {
    }

    protected IEnumerator CoolTime_SP()
    {
        yield return new WaitForSeconds(cool_SP);
        skill_2 = true;
        Debug.Log("SP스킬 온");
        time_SP.gameObject.SetActive(true);
    }
    /*************************************************************/
    //Effect AnimationEvenet
  
    public void StartEffect_1()
    {
        atk1.SetActive(true);
        audioSource.PlayOneShot(attackClip[0], 1f);
    }
    public void StartEffect_2()
    {
        atk2.SetActive(true);
        audioSource.PlayOneShot(attackClip[1], 1f);
    }
    public void StartEffect_3()
    {
        atk3.SetActive(true);
        audioSource.PlayOneShot(attackClip[2], 1f);
    }
    public void StartEffect_4()
    {
        atk4.SetActive(true);
        audioSource.PlayOneShot(attackClip[3], 1f);
    }
    /**********************************************************************/
    public void EndEffect_1()
    {
        atk1.SetActive(false);
        playerStatement.currentState = PlayerStatement.State.Idle;
    }
    public void EndEffect_2()
    {
        atk2.SetActive(false);
        playerStatement.currentState = PlayerStatement.State.Idle;
    }
    public void EndEffect_3()
    {
        atk3.SetActive(false);
        playerStatement.currentState = PlayerStatement.State.Idle;
    }
    public void EndEffect_4()
    {
        atk4.SetActive(false);
        playerStatement.currentState = PlayerStatement.State.Idle;
    }

    /**********************************************************************/
    public void StartSkill()
    {
        Skill_1.SetActive(true);
        playerStatement.currentState = PlayerStatement.State.Attack;
    }
    public void EndSkill()
    {
        Skill_1.SetActive(false);
        playerStatement.currentState = PlayerStatement.State.Idle;
    }

    public void StartSPSkill()
    {
        Skill_2.SetActive(true);
        playerStatement.currentState = PlayerStatement.State.Attack;
        StartCoroutine("SPSkillArea");
    }
    public void OnSpark()
    {
        Spark.SetActive(true);
    }
    public void OffSpark()
    {
        Spark.SetActive(false);
    }
    public void EndSPSkill()
    {
        //Time.timeScale = 1f;
        playerStatement.currentState = PlayerStatement.State.Idle;
        Skill_2.SetActive(false);
        ani.SetBool("SpecialAttack", false);
    }
}
