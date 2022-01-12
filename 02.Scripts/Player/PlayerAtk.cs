using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtk : PlayerAttack
{
    public float SkillMana, DodgeMana;
    public GameObject meleeAttackArea, JudgmentArea, SPArea;
    public CapsuleCollider cap;
    private Vector3 normalSize;
    private Vector3 atk4Size;

    protected override void Awake()
    {
        base.Awake();
        meleeAttackArea = meleeArea;
        normalSize = new Vector3(1.3f, 1f, 2f);
        atk4Size = new Vector3(1f, 1f, 2.5f);

        meleeArea.GetComponent<TriggerCallback>().CollisionEnterEvent += MeleeAttackDetect;
        JudgmentArea.GetComponent<TriggerCallback>().CollisionEnterEvent += JudgementDetect;
        SPArea.GetComponent<TriggerCallback>().CollisionEnterEvent += SPSkillDetect;
        skills.Add(new Skill("Judgment", SkillMana, 10f, JudgmentArea));


        selectedSkills[0] = skills[0];
    }
    /*************************************************************************/
    protected override void Attack()
    {
        if (!playerStatement.dead && playerStatement.equipList.ContainsKey(Item.ItemType.weapon))//&& (playerStatement.currentState == PlayerStatement.State.Idle || playerStatement.currentState == PlayerStatement.State.Move))
        {
            base.Attack();

            // 플레이어는 로비와 마을에선 공격할 수 없고,사냥터에 있을 때 LINQ를 통해 가장 가까운 몬스터를 찾아 쳐다본다.
            if (GameManager.Instance.currentSceneName != "Level_1")
            {
                if (FindNearestObjectByTag("Enemy") != null)
                {
                    transform.LookAt(FindNearestObjectByTag("Enemy").transform.position);
                }

                if (atkCount == 0)
                {
                    atkCount = 1;
                    ani.Play("Attack_1");
                }
                else //(atkCount != 0)
                {
                    if (ComboPossible)
                    {
                        ComboPossible = false;
                        atkCount += 1;
                    }
                }
                playerStatement.currentState = PlayerStatement.State.Attack;
            }
            else if (!GameManager.Instance.canAttack && message.gameObject.activeInHierarchy == false)
            {
                message.gameObject.SetActive(true);
                message.text = "마을에선 공격할 수 없습니다.";
                message.color = Color.red;
                atkCount = 0;
            }
            if(CalcCrit())
            {
                Debug.Log("크리티컬");
            }
        }
    }
    public void normalActiveArea()
    {
        meleeArea.SetActive(true);
        meleeArea.transform.localScale = normalSize;
    }
    public void Atk4ActiveArea()
    {
        meleeArea.SetActive(true);
        meleeArea.transform.localScale = atk4Size;
    }
    public void UnActiveArea()
    {
        meleeArea.SetActive(false);
    }

    /******************************************************************************/
    protected override void Skill()
    {
        if (skill && GameManager.Instance.canAttack && !playerStatement.dead)//&& playerStatement.equipList.ContainsKey(Item.ItemType.weapon) && (playerStatement.currentState == PlayerStatement.State.Idle || playerStatement.currentState == PlayerStatement.State.Move))
        {
            base.Skill();
            skill = false;
            playerStatement.currentState = PlayerStatement.State.Attack;

            ani.SetTrigger("Skill");
            time_S.fillAmount = 1f;
            StartCoroutine(CoolTime_Skill());

            currentCool_S = cool_S;
            counter_S.text = "" + currentCool_S;
            StartCoroutine(CoolText_Skill());
            StartCoroutine(JudgementArea());
            playerStatement.ReduceMana(selectedSkills[0].requireMana);

            if (FindNearestObjectByTag("Enemy") != null)
            {
                transform.LookAt(FindNearestObjectByTag("Enemy").transform.position);
            }
        }
        else if (!GameManager.Instance.canAttack && message.gameObject.activeInHierarchy == false)
        {
            message.gameObject.SetActive(true);
            message.text = "마을에선 공격할 수 없습니다.";
            message.color = Color.red;
        }
    }
    protected override void InputSPSkill()
    {
        if(skill_2 && GameManager.Instance.canAttack && !playerStatement.dead)
        {
            base.InputSPSkill();
            SPBT.gameObject.SetActive(false);
            GameManager.Instance.cam.SPCam();
            skill_2 = false;
            ani.SetTrigger("SpecialAttack");
            StartCoroutine(SPSkillArea());
            playerStatement.currentState = PlayerStatement.State.Attack;
            player.tag = "Player";
            if (FindNearestObjectByTag("Enemy") != null)
            {
                transform.LookAt(FindNearestObjectByTag("Enemy").transform.position);
            }
        }
        else if (!GameManager.Instance.canAttack && message.gameObject.activeInHierarchy == false)
        {
            message.gameObject.SetActive(true);
            message.text = "마을에선 공격할 수 없습니다.";
            message.color = Color.red;
        }
    }
    /**************************************************************************/
    protected override void Dodge()
    {
        base.Dodge();
        if (canDodge && !playerStatement.dead && playerStatement.currentState != PlayerStatement.State.Attack)
        {
            ani.SetTrigger("DoDodge");
            time_D.fillAmount = 1f;
            StartCoroutine(CoolTime_Dodge());

            currentCool_D = cool_D;
            counter_D.text = "" + currentCool_D;

            StartCoroutine(CoolText_Dodge());
            canDodge = false;
            playerStatement.currentState = PlayerStatement.State.Dodge;
            player.tag = "DodgePlayer";
        }
    }
    /**********************************************************************************/
   
    /**********************************************************************************/
    private void MeleeAttackDetect(Collider col)
    {
        if(col.tag == "Enemy")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            //Vector3 hitPos = col.ClosestPoint(meleeArea.transform.position);
            Vector3 hitPos = col.ClosestPoint(transform.forward);
            damageable.OnDamage(playerStatement.totalDamage, hitPos, Vector3.zero, CalcCrit());
        }
        if(col == null)
        {
            Debug.Log("상대 없음");
        }
    }

    private void JudgementDetect(Collider col)
    {
        if(col.tag == "Enemy")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            Vector3 hitPos = col.ClosestPoint(transform.forward);
            damageable.OnDamage(playerStatement.totalDamage * 1.4f, hitPos, Vector3.zero, CalcCrit());
        }
    }

    private void SPSkillDetect(Collider col)
    {
        if (col.tag == "Enemy")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();

            Vector3 hitPos = col.ClosestPoint(transform.forward);
            damageable.OnDamage(playerStatement.totalDamage * 1f, hitPos, Vector3.zero, CalcCrit());
        }

    }
    private IEnumerator JudgementArea()
    {
        yield return new WaitForSeconds(0.20f);
        JudgmentArea.SetActive(true);
        audioSource.PlayOneShot(skillClip, 1f);
        yield return new WaitForSeconds(0.35f);
        JudgmentArea.SetActive(false);
        playerStatement.currentState = PlayerStatement.State.Idle;

        yield return new WaitForSeconds(0.3f);
        if(Skill_1.activeSelf)
        {
            Skill_1.SetActive(false);
        }
    }
    private IEnumerator SPSkillArea()
    {
        audioSource.PlayOneShot(skill2Clip, 1f);
        GameManager.Instance.canAttack = false;
        cap.enabled = false;
        yield return new WaitForSeconds(1.08f);
        SPArea.SetActive(true);
        audioSource.PlayOneShot(attackClip[2], 1f);

        yield return new WaitForSeconds(0.061f);
        SPArea.SetActive(false);

        yield return new WaitForSeconds(0.079f);
        SPArea.SetActive(true);
        audioSource.PlayOneShot(attackClip[2], 1f);

        yield return new WaitForSeconds(0.04f);
        SPArea.SetActive(false);

        yield return new WaitForSeconds(0.03f);
        SPArea.SetActive(true);
        audioSource.PlayOneShot(attackClip[2], 1f);

        yield return new WaitForSeconds(0.03f);
        SPArea.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.canAttack = true;
        cap.enabled = true;
        playerStatement.currentState = PlayerStatement.State.Idle;

        if (Skill_2.activeSelf)
        {
            Skill_2.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(CoolTime_SP());
    }
    /***********************************************************************************/
}




