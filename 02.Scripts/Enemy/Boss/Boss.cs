using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
  
    public float paringChance;
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
            nav.isStopped = false;
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
            nav.isStopped = false;
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
            nav.isStopped = false;
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
            nav.isStopped = false;
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
            nav.isStopped = false;
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
            nav.isStopped = false;
        }

        public void OperateUpdate()
        {
        }
    }
   

    private void AttackDetectAreaStay(Collider col)
    {
        if (col.gameObject == targetObj && !dead)
        {
            if (targetObj.GetComponent<PlayerStatement>().currentState == PlayerStatement.State.Attack)
            {
                if (ParingChance())
                {
                    stateMachine.SetState(stateDic[EnemyState.Paring]);
                }
            }

            if (stateMachine.CurrentState == stateDic[EnemyState.Chase])
            {
                int rand = Random.Range((int)EnemyState.Attack, (int)EnemyState.Skill2 + 1);
                stateMachine.SetState(stateDic[(EnemyState)rand]);

                
            }
        }
        else if(dead)
        {
            Die();
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
    

    private bool ParingChance()
    {
        float rand = Random.Range(1f, 100f);
        return rand <= paringChance;
    }

    IEnumerator BossAttackCoroutine()
    {
        transform.LookAt(targetObj.transform);
        ani.SetTrigger("Attack");
        yield return new WaitForSeconds(0.14f);
        attackArea.SetActive(true);
        attackEffect.SetActive(true);
        audioSource.PlayOneShot(attackClip, 1f);

        yield return new WaitForSeconds(0.22f);
        attackEffect.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.7f);
        attackEffect_2.SetActive(true);
        attackArea.SetActive(true);
        audioSource.PlayOneShot(attackClip, 1f);

        yield return new WaitForSeconds(0.2f);
        attackEffect_2.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.87f);
        attackArea.SetActive(true);
        attackEffect_3.SetActive(true);
        audioSource.PlayOneShot(attackClip, 1f);

        yield return new WaitForSeconds(0.2f);
        attackEffect_3.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(1f);
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
        ani.SetTrigger("Attact_2");
        yield return new WaitForSeconds(0.2f);
        attackArea.SetActive(true);
        attack2Effect.SetActive(true);
        audioSource.PlayOneShot(attackClip, 1f);

        yield return new WaitForSeconds(0.8f);
        attackArea.SetActive(false);
        attack2Effect.SetActive(false);

        yield return new WaitForSeconds(0.14f);
        attackArea.SetActive(true);
        attack2Effect_2.SetActive(true);
        audioSource.PlayOneShot(attackClip, 1f);

        yield return new WaitForSeconds(0.86f);
        attack2Effect_2.SetActive(false);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.13f);
        attackArea.SetActive(true);
        attack2Effect_2.SetActive(true);
        audioSource.PlayOneShot(attackClip, 1f);

        yield return new WaitForSeconds(0.97f);
        attackArea.SetActive(false);
        attack2Effect_2.SetActive(false);
        yield return new WaitForSeconds(1f);
        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }

    }
    private void Skill_1Set()
    {
        Skill_1Area_1.GetComponent<Skill_1>().skill_1Range = this.skillRange;
        Skill_1Area_1.GetComponent<Skill_1>().skill_1Speed = this.skillSpeed;
        Skill_1Area_1.GetComponent<Skill_1>().Skill_1Damage = this.skillDamage;

        Skill_1Area_2.GetComponent<Skill_1>().skill_1Range = this.skillRange;
        Skill_1Area_2.GetComponent<Skill_1>().skill_1Speed = this.skillSpeed;
        Skill_1Area_2.GetComponent<Skill_1>().Skill_1Damage = this.skillDamage;

        Skill_1Area_3.GetComponent<Skill_1>().skill_1Range = this.skillRange;
        Skill_1Area_3.GetComponent<Skill_1>().skill_1Speed = this.skillSpeed;
        Skill_1Area_3.GetComponent<Skill_1>().Skill_1Damage = this.skillDamage;

        Skill_1Area_4.GetComponent<Skill_1>().skill_1Range = this.skillRange;
        Skill_1Area_4.GetComponent<Skill_1>().skill_1Speed = this.skillSpeed;
        Skill_1Area_4.GetComponent<Skill_1>().Skill_1Damage = this.skillDamage;
    }
    IEnumerator BossSkill_1Coroutine()
    {
        transform.LookAt(targetObj.transform);
        ani.SetTrigger("Skill");
        Skill_1Set(); // 스킬 데미지 거리 속도 설정
        Skill_1Area_1.SetActive(true);
        Skill_1Area_2.SetActive(true);
        Skill_1Area_3.SetActive(true);
        Skill_1Area_4.SetActive(true);

        yield return new WaitForSeconds(0.14f);
        Skill_1Effect_1.SetActive(true);
        Skill_1Effect_2.SetActive(true);
        Skill_1Effect_3.SetActive(true);
        Skill_1Effect_4.SetActive(true);
        audioSource.PlayOneShot(skillClip, 2f);
        yield return new WaitForSeconds(0.2f);
        Skill_1Area_1.GetComponent<SphereCollider>().enabled = true;
        Skill_1Area_2.GetComponent<SphereCollider>().enabled = true;
        Skill_1Area_3.GetComponent<SphereCollider>().enabled = true;
        Skill_1Area_4.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(0.3f);
        Skill_1Effect_1.SetActive(false);
        Skill_1Area_1.SetActive(false);
        Skill_1Effect_2.SetActive(false);
        Skill_1Area_2.SetActive(false);
        Skill_1Effect_3.SetActive(false);
        Skill_1Area_3.SetActive(false);
        Skill_1Effect_4.SetActive(false);
        Skill_1Area_4.SetActive(false);
        yield return new WaitForSeconds(0.86f);
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
        ani.SetTrigger("Skill_2");
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
                yield return new WaitForSeconds(1f);

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
        yield return new WaitForSeconds(0.96f);
        if (targetObj.GetComponent<LivingObjects>().dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Idle]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
        slash.SetActive(false);
        SlashDetectArea.SetActive(false);
    }
    private void StepClip()
    {
        audioSource.PlayOneShot(stepClip, 0.3f);
    }

}
