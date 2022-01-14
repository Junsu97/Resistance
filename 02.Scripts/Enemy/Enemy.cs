using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingObjects
{
    public EnemyData.EnemyType enemyType;
    public int enemyId;
    public int itemNo;
    public float dropChance;
    public int maxAmount;
    public int exp;

    public float rotateSpeed = 2f;
    public float paringChance;

    // [HideInInspector]
    public GameObject targetObj;
    [SerializeField]
    public int wayPointIndex;
    public GameObject[] wayPoints;
    public NavMeshAgent nav;
    public EnemySlider enemySlider;

    public StateMachine stateMachine;
    public Dictionary<EnemyState, IState> stateDic = new Dictionary<EnemyState, IState>();

    //[HideInInspector]
    public EnemySpawn enemySpawn;

    public GameObject PlayerDetectArea;
    protected Animator ani;
    protected AudioSource audioSource;
    public AudioClip hitClip;

    protected virtual void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

        nav = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        IState idle = new EnemyIdle(this);
        IState chase = new EnemyChase(this);
        IState die = new EnemyDie(this);
        IState patrol = new EnemyPatrol(this);
        ani.SetBool("Dead", dead);
        stateDic[EnemyState.Patrol] = patrol;
        PlayerDetectArea.SetActive(true);
        PlayerDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += PlayerDetectAreaInStay;
        stateMachine = new StateMachine(idle);
        
        stateDic[EnemyState.Idle] = idle;
        stateDic[EnemyState.Chase] = chase;
        stateDic[EnemyState.Die] = die;

        SetwayPoints();
    }
    protected void Update()
    {
        if (!dead)
        {
            stateMachine.DoOperateUpdate();
        }
    }
    public enum EnemyState
    {
        Idle,
        Patrol,
        Paring,
        Counterattack,
        Chase,
        Attack,
        Attack2,
        Skill1,
        Skill2,
        Die
    };

    protected class EnemyIdle : IState
    {
        private Enemy owner;
        public EnemyIdle(Enemy owner)
        {
            this.owner = owner;
        }
        public void OperateEnter()
        {
        }

        public void OperateUpdate()
        {
            if(owner.targetObj != null)//&& !owner.targetObj.GetComponent<LivingObjects>().dead)
            {
                owner.stateMachine.SetState(owner.stateDic[EnemyState.Chase]);
            }
            else if(owner.targetObj == null && owner.enemyType != EnemyData.EnemyType.boss)
            {
                owner.stateMachine.SetState(owner.stateDic[EnemyState.Patrol]);
            }
            else if(owner.enemyType == EnemyData.EnemyType.boss)
            {
                owner.stateMachine.SetState(owner.stateDic[EnemyState.Chase]);
            }
        }

        public void OperateExit()
        {
        }
    }

    protected class EnemyChase : IState
    {
        private Enemy owner;
        private NavMeshAgent nav;
        private Animator ani;
        public EnemyChase(Enemy owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
            this.ani = owner.ani;
        }

        public void OperateEnter()
        {
            nav.isStopped = false;
            ani.SetFloat("Move", 1f);
        }

        public void OperateExit()
        {
            ani.SetFloat("Move", 0f);
        }

        public void OperateUpdate()
        {
            if (owner.targetObj == null || owner.targetObj.GetComponent<LivingObjects>().dead)
            {
                if(owner.enemyType != EnemyData.EnemyType.boss)
                {
                    owner.stateMachine.SetState(owner.stateDic[EnemyState.Patrol]);
                }
                else if(owner.enemyType == EnemyData.EnemyType.boss)
                {
                    owner.stateMachine.SetState(owner.stateDic[EnemyState.Idle]);
                }
            }
            else
            {
                TraceTarget();
            }
        }
        private void TraceTarget()
        {
            Vector3 dir = owner.targetObj.transform.position - owner.transform.position;
            nav.SetDestination(owner.targetObj.transform.position);

            if(nav.remainingDistance <= nav.stoppingDistance)
            {
                owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * owner.rotateSpeed);
            }
        }
    }
    

    protected class EnemyPatrol : IState
    {
        private Enemy owner;
        private NavMeshAgent nav;
        private Animator ani;
        private GameObject[] wayPoints;
        private int wayPointIndex;
        public EnemyPatrol(Enemy owner)
        {
            this.owner = owner;
            this.nav = owner.GetComponent<NavMeshAgent>();
            this.ani = owner.GetComponent<Animator>();
            this.wayPointIndex = owner.wayPointIndex;
            this.wayPoints = owner.wayPoints;
        }

        public void OperateEnter()
        {
            //owner.transform.rotation = Quaternion.Euler(wayPoints[wayPointIndex].transform.position);
            if(!owner.dead)
            {
                nav.isStopped = false;
                wayPointsDir();
                owner.enemySlider.gameObject.SetActive(false);
                owner.health = owner.baseMaxHealth;
                nav.SetDestination(wayPoints[wayPointIndex].transform.position);
                ani.SetFloat("Move", 1f);
            }
          
        }

        public void OperateExit()
        {
            wayPointIndex = Random.Range(0, wayPoints.Length - 1);
        }

        public void OperateUpdate()
        {
            ToChase();
            NextPatrol();
        }
        private void ToChase()
        {
            if(owner.targetObj != null && !owner.targetObj.GetComponent<LivingObjects>().dead && !owner.dead)
            {
                owner.stateMachine.SetState(owner.stateDic[EnemyState.Chase]);
            }
        }
        private void NextPatrol()
        {
            float dis = Vector3.Distance(owner.transform.position, wayPoints[wayPointIndex].transform.position);
            if(dis <= 7f && !owner.dead)
            {
                ani.SetFloat("Move", 0);
                wayPointIndex = Random.Range(0, wayPoints.Length);
                nav.SetDestination(owner.transform.position);
                float time = Random.Range(0.5f, 5.5f);
                owner.StartCoroutine(SetNextPatrol(time));
            }
        }
        private void wayPointsDir()
        {
            Vector3 dir = wayPoints[wayPointIndex].transform.position - owner.transform.position;
            dir.y = 0f;
            Quaternion rot = Quaternion.LookRotation(dir);
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, rot, Time.deltaTime * owner.rotateSpeed);
        }
        IEnumerator SetNextPatrol(float time)
        {
            yield return new WaitForSeconds(time);
            nav.SetDestination(wayPoints[wayPointIndex].transform.position);
            owner.stateMachine.SetState(owner.stateDic[EnemyState.Patrol]);
            wayPointsDir(); // 다음 웨이포인트의 방향을 바라보기
            OperateEnter();
        }
    }

    protected class EnemyDie : IState
    {
        private Enemy owner;
        private Animator ani;
        public EnemyDie(Enemy owner)
        {
            this.owner = owner;
            this.ani = owner.GetComponent<Animator>();
        }

        public void OperateEnter()
        {
            ani.SetTrigger("Die");
            ani.ResetTrigger("GetHit");
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }

    private void SetwayPoints()
    {
        wayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
    }

    private void PlayerDetectAreaInStay(Collider col)
    {
        if(col.tag == "Player" || col.tag == "DodgePlayer")
        {
            if(!col.GetComponent<LivingObjects>().dead)
            {
                targetObj = col.gameObject;
            }
        }
        else if(col == null)
        {
            TraceExit();
        }
    }

    private void TraceExit()
    {
        targetObj = null;
    }
    private bool ParingChance()
    {
        float rand = Random.Range(1f, 100f);
        return rand <= paringChance;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal, bool isCrit = false)
    {
       
        if(!dead && stateMachine.CurrentState != stateDic[EnemyState.Paring]) // 플레이어의 공격을 받는 상황이라면
        {
            base.OnDamage(damage, hitPoint, hitNormal, isCrit);

            audioSource.PlayOneShot(hitClip, 0.4f);
            GameManager.Instance.cam.ShakeCam(0.1f, 0.2f);
            if (enemyType == EnemyData.EnemyType.boss) 
            {
                enemySlider.isBoss = true;
                enemySlider.bossNameText.text = name;
                if (targetObj.GetComponent<PlayerStatement>().currentState == PlayerStatement.State.Attack)
                {
                    if (ParingChance())
                    {
                        stateMachine.SetState(stateDic[EnemyState.Paring]);
                    }
                }
            }
            else
            {
                if (stateMachine.CurrentState != stateDic[EnemyState.Attack])
                {
                    ani.SetTrigger("GetHit");
                    Debug.Log(this.transform.name);
                }
            }

            enemySlider.gameObject.SetActive(true);
            enemySlider.SetSliderValue(this);

            GameObject effect = ObjectPoolingManager.Instance.GetQueue("hitEffect");
            effect.transform.position = hitPoint;
            effect.GetComponent<ParticleSystem>().Play();

            StartCoroutine(DestroyEffect(effect, 2f));
        }
        else if (stateMachine.CurrentState == stateDic[EnemyState.Paring] && !dead)
        {
            stateMachine.SetState(stateDic[EnemyState.Counterattack]);
        }

        if (!dead && targetObj == null)
        {
            targetObj = GameManager.Instance.player.gameObject;
        }
    }
    public override void Die()
    {
        base.Die();
        if(enemyType != EnemyData.EnemyType.boss)
        {
            stateMachine.SetState(stateDic[EnemyState.Die]);
            DropItem();
            nav.enabled = false;
            GameManager.Instance.GetExp(exp);
            QuestManager.Instance.TargetEnemyKilled(enemyId);

            // Enemy가 죽으면 enemySpawn에 등록된 시간이 지난 후 리스폰된다.
            enemySpawn.Respawn(transform.parent.transform, enemySpawn.respawnTime);
            StartCoroutine(DestroyEnemy(4f));
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Die]);
            DropItem();
            nav.enabled = false;
            GameManager.Instance.GetExp(exp);
            StartCoroutine(DestroyBoss(this.transform.gameObject, 4f));
            QuestManager.Instance.TargetEnemyKilled(enemyId);
            Boss boss = this.GetComponent<Boss>();
            boss.attackDetectArea.SetActive(false);
        }
        targetObj = null;
        PlayerDetectArea.SetActive(false);
        ani.SetFloat("Move", 0f);
        ani.SetBool("Dead", dead);
    }
    private void DropItem()
    {
        int rollDrop = Random.Range(1, 100 + 1);

        if(rollDrop <= dropChance)
        {
            int rollAmount = Random.Range(1, maxAmount + 1);

            if(enemyType != EnemyData.EnemyType.boss)
            {
                GameObject dropItem = ObjectPoolingManager.Instance.GetQueue("dropItem");
                dropItem.GetComponent<ItemObject>().SetItem(itemNo, rollAmount);

                dropItem.transform.position = this.transform.position;
                dropItem.GetComponent<ItemObject>().RandomeForce();
                dropItem.GetComponent<ItemObject>().StartCoroutine("DestroyItemObj", 10f);
            }
            else
            {
                GameObject dropItem = ObjectPoolingManager.Instance.GetQueue("BossdropItem");
                dropItem.GetComponent<ItemObject>().SetItem(itemNo, rollAmount);

                dropItem.transform.position = this.transform.position;
                dropItem.GetComponent<ItemObject>().RandomeForce();
                dropItem.GetComponent<ItemObject>().StartCoroutine("DestroyItemObj", Mathf.Infinity);
            }
        }
    }

    public IEnumerator DestroyEffect(GameObject effect, float time)
    {
        WaitForSeconds ws = new WaitForSeconds(time);
        yield return ws;

        ObjectPoolingManager.Instance.InsertQueue(effect, "hitEffect");
    }

    public IEnumerator DestroyEnemy(float time)
    {
        WaitForSeconds ws = new WaitForSeconds(time);
        yield return ws;

        ObjectPoolingManager.Instance.InsertQueue(this.gameObject, enemySpawn.enemyData.enemyName);
    }
    public IEnumerator DestroyBoss(GameObject boss,float time)
    {
        WaitForSeconds ws = new WaitForSeconds(time);
        yield return ws;
        boss.SetActive(false);
    }

}
