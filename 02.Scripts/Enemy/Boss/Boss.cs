using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
  
    [Header("Attack")]
    public GameObject attackArea;
    public GameObject attackDetectArea;
    public GameObject attackEffect, attackEffect_2, attackEffect_3;
    [Header("Attack2")]
    public GameObject attack2Effect, attack2Effect_2;
    [Header("Slash")]
    public GameObject slash;
    public GameObject SlashDetectArea;
    [Header("Skill_1")]
    public GameObject Skill_1Effect_1;
    public GameObject Skill_1Effect_2;
    public GameObject Skill_1Effect_3;
    public GameObject Skill_1Effect_4;
    public GameObject Skill_1Area_1;
    public GameObject Skill_1Area_2;
    public GameObject Skill_1Area_3;
    public GameObject Skill_1Area_4;
    
    [Header("Skill_2")]
    public GameObject Skill_2StartEffect;
    public GameObject Skill_2Effect;
    public GameObject Skill_2DetectArea;
    [Header("Paring")]
    public GameObject ParingEffect;
    [Header("Audio")]
    public AudioClip attackClip;
    public AudioClip skillClip;
    public AudioClip paringClip;
    public AudioClip stepClip;


    private EnemyData enemyData;

    public float skillDamage, skillSpeed,skillRange, Skill_2Damage, slashSpeed,slashDamage;

    protected override void OnEnable()
    {
        base.OnEnable();

        IState attack = new BossAttack(this);
        IState attack_2 = new BossAttack_2(this);
        IState skill1 = new BossSkill_1(this);
        IState skill2 = new BossSkill_2(this);
        IState paring = new BossParing(this);
        IState counterAttack = new CounterAttack(this);

        stateDic[EnemyState.Attack] = attack;
        stateDic[EnemyState.Attack2] = attack_2;
        stateDic[EnemyState.Paring] = paring;
        stateDic[EnemyState.Counterattack] = counterAttack;
        stateDic[EnemyState.Skill1] = skill1;
        stateDic[EnemyState.Skill2] = skill2;

        //targetObj = GameManager.Instance.player.gameObject;
        PlayerDetectArea.SetActive(true);
        attackDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += AttackDetectAreaStay;
        attackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += BossAttackArea;

        Enemy enemy = transform.GetComponent<Enemy>();
        enemyData = DBManager.Instance.enemyDict[enemyId];
        SetEnemy(enemy, enemyData);
    }

    private void SetEnemy(Enemy enemy, EnemyData enemyData)
    {
        enemy.enemyType = enemyData.enemyType;
        enemy.enemyId = enemyData.enemyId;
        enemy.name = enemyData.enemyName;
        enemy.itemNo = enemyData.itemNo;
        enemy.dropChance = enemyData.dropChance;
        enemy.maxAmount = enemyData.maxAmount;
        enemy.exp = enemyData.exp;

        enemy.level = enemyData.level;
        enemy.baseMaxHealth = enemyData.maxHealth;
        enemy.health = enemy.baseMaxHealth;
        enemy.baseDamage = enemyData.damage;

        enemy.enemySlider.gameObject.SetActive(false);
        enemy.targetObj = null;

        enemy.dead = false;
    }

    protected class BossAttack : IState
    {
        private Boss owner;
        private NavMeshAgent nav;
        public BossAttack(Boss owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("BossAttackCoroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }
    protected class BossAttack_2 : IState
    {
        private Boss owner;
        private NavMeshAgent nav;
        public BossAttack_2(Boss owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("BossAttack_2Corountine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected class BossSkill_1 : IState
    {
        private Boss owner;
        private NavMeshAgent nav;
        public BossSkill_1(Boss owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("BossSkill_1Coroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }
    protected class BossSkill_2 : IState
    {
        private Boss owner;
        private NavMeshAgent nav;
        public BossSkill_2(Boss owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("BossSkill_2Coroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected class BossParing : IState
    {
        private Boss owner;
        private NavMeshAgent nav;
        public BossParing(Boss owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("BossParingCoroutin");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    protected class CounterAttack : IState
    {
        private Boss owner;
        private NavMeshAgent nav;
        public CounterAttack(Boss owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("CounterAttackCoroutine");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }
   

    private void AttackDetectAreaStay(Collider col)
    {
        if (col.gameObject == targetObj && !dead)
        {
            if (stateMachine.CurrentState == stateDic[EnemyState.Chase] || stateMachine.CurrentState == stateDic[EnemyState.Idle]) 
            {
                if(!ani.GetCurrentAnimatorStateInfo(0).IsName("GetHit") && ani.GetInteger("Attack") == 0)
                {
                    int rand = Random.Range((int)EnemyState.Attack, (int)EnemyState.Skill2 + 1);
                    stateMachine.SetState(stateDic[(EnemyState)rand]);
                    Debug.Log(ani.GetInteger("Attack"));
                }
                else
                {
                    return;
                }
            }
            else if(stateMachine.CurrentState == stateDic[EnemyState.Paring])
            {
                return;
            }
        }
    }
    
    private void BossAttackArea(Collider col)
    {
        if(col.tag == "Player" || col.tag == "DodgePlayer")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            damageable.OnDamage(baseDamage, Vector3.zero, Vector3.zero);
        }
    }
    



    IEnumerator BossAttackCoroutine()
    {
        transform.LookAt(targetObj.transform);
        ani.SetInteger("Attack",1);
        yield return new WaitForSeconds(0.35f);
        attackArea.SetActive(true);
        attackEffect.SetActive(true);
        audioSource.PlayOneShot(attackClip, 0.5f);

        yield return new WaitForSeconds(0.28f);
        attackEffect.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.73f);
        transform.LookAt(targetObj.transform);
        attackEffect_2.SetActive(true);
        attackArea.SetActive(true);
        audioSource.PlayOneShot(attackClip, 0.5f);

        yield return new WaitForSeconds(0.2f);
        attackEffect_2.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.87f);
        transform.LookAt(targetObj.transform);
        attackArea.SetActive(true);
        attackEffect_3.SetActive(true);
        audioSource.PlayOneShot(attackClip, 0.5f);

        yield return new WaitForSeconds(0.2f);
        attackEffect_3.SetActive(false);
        attackArea.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        ani.SetInteger("Attack", 0);
        //yield return new WaitForSeconds(0.6f);
        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }

    }

    IEnumerator BossAttack_2Corountine()
    {
        transform.LookAt(targetObj.transform);
        ani.SetInteger("Attack", 2);
        yield return new WaitForSeconds(0.6f);
        attackArea.SetActive(true);
        attack2Effect.SetActive(true);
        audioSource.PlayOneShot(attackClip, 0.5f);

        yield return new WaitForSeconds(0.8f);
        attackArea.SetActive(false);
        attack2Effect.SetActive(false);

        yield return new WaitForSeconds(0.14f);
        transform.LookAt(targetObj.transform);
        attackArea.SetActive(true);
        attack2Effect_2.SetActive(true);
        audioSource.PlayOneShot(attackClip, 0.5f);

        yield return new WaitForSeconds(0.86f);
        attack2Effect_2.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.13f);
        transform.LookAt(targetObj.transform);
        attackArea.SetActive(true);
        attack2Effect_2.SetActive(true);
        audioSource.PlayOneShot(attackClip, 0.5f);

        yield return new WaitForSeconds(0.97f);
        attackArea.SetActive(false);
        attack2Effect_2.SetActive(false);
        ani.SetInteger("Attack", 0);

        yield return new WaitForSeconds(0.1f);
       // yield return new WaitForSeconds(0.6f);
        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }

    }

    IEnumerator BossSkill_1Coroutine()
    {

        Skill_1 skill_1 = Skill_1Area_1.GetComponent<Skill_1>();
        skill_1.skill_1Range = this.skillRange;
        skill_1.skill_1Speed = this.skillSpeed;
        skill_1.Skill_1Damage = this.skillDamage;

        Skill_1 skill_2 = Skill_1Area_2.GetComponent<Skill_1>();
        skill_2.skill_1Range = this.skillRange;
        skill_2.skill_1Speed = this.skillSpeed;
        skill_2.Skill_1Damage = this.skillDamage;

        Skill_1 skill_3 = Skill_1Area_3.GetComponent<Skill_1>();
        skill_3.skill_1Range = this.skillRange;
        skill_3.skill_1Speed = this.skillSpeed;
        skill_3.Skill_1Damage = this.skillDamage;

        Skill_1 skill_4 = Skill_1Area_4.GetComponent<Skill_1>();
        skill_4.skill_1Range = this.skillRange;
        skill_4.skill_1Speed = this.skillSpeed;
        skill_4.Skill_1Damage = this.skillDamage;

        transform.LookAt(targetObj.transform);
        ani.SetInteger("Attack", 3);

        Skill_1Area_1.SetActive(true);
        Skill_1Area_2.SetActive(true);
        Skill_1Area_3.SetActive(true);
        Skill_1Area_4.SetActive(true);

        yield return new WaitForSeconds(0.6f);
        Skill_1Effect_1.SetActive(true);
        Skill_1Effect_2.SetActive(true);
        Skill_1Effect_3.SetActive(true);
        Skill_1Effect_4.SetActive(true);
        audioSource.PlayOneShot(skillClip, 2.5f);
        yield return new WaitForSeconds(0.35f);
        Skill_1Area_1.GetComponent<SphereCollider>().enabled = true;
        Skill_1Area_2.GetComponent<SphereCollider>().enabled = true;
        Skill_1Area_3.GetComponent<SphereCollider>().enabled = true;
        Skill_1Area_4.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        Skill_1Effect_1.SetActive(false);
        Skill_1Area_1.SetActive(false);
        Skill_1Effect_2.SetActive(false);
        Skill_1Area_2.SetActive(false);
        Skill_1Effect_3.SetActive(false);
        Skill_1Area_3.SetActive(false);
        Skill_1Effect_4.SetActive(false);
        Skill_1Area_4.SetActive(false);
        ani.SetInteger("Attack", 0);
        if(targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
    }
    IEnumerator BossSkill_2Coroutine()
    {
        Skill_2DetectArea.GetComponent<Skill_2>().skillDamage = this.Skill_2Damage;
        transform.LookAt(targetObj.transform);
        ani.SetInteger("Attack",4);
        Skill_2StartEffect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        Skill_2Effect.SetActive(true);
        yield return new WaitForSeconds(0.08f);
        Skill_2DetectArea.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        Skill_2StartEffect.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Skill_2Effect.SetActive(false);
        Skill_2DetectArea.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        ani.SetInteger("Attack", 0);

        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
    }
    IEnumerator BossParingCoroutin()
    {
        transform.LookAt(targetObj.transform);
        ani.SetTrigger("Paring");
        ParingEffect.SetActive(true);
        yield return new WaitForSeconds(0.18f);
        ParingEffect.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
    }
    
    IEnumerator CounterAttackCoroutine()
    {
        transform.LookAt(targetObj.transform);
        audioSource.PlayOneShot(paringClip, 1f);
        ani.SetTrigger("ParingAttack");
        SlashDetectArea.GetComponent<Slash>().slashSpeed = this.slashSpeed;
        SlashDetectArea.GetComponent<Slash>().slashDamage = this.slashDamage;
        yield return new WaitForSeconds(0.04f);
        slash.SetActive(true);
        SlashDetectArea.GetComponent<Slash>().isFire = true;
        SlashDetectArea.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        slash.SetActive(false);
        SlashDetectArea.SetActive(false);
        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
    }
   
}
