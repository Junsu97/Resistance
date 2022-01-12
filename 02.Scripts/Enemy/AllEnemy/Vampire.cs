using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vampire : Enemy
{
    public GameObject attackArea, attackDetectArea;
    public GameObject AttackEffect;
    protected class VampireAttack : IState
    {
        private Vampire owner;
        private NavMeshAgent nav;
        public VampireAttack(Vampire owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("VampireAttackCoroutine");
            owner.transform.LookAt(owner.targetObj.transform);
        }

        public void OperateExit()
        {
            nav.isStopped = false;
        }

        public void OperateUpdate()
        {
            if(owner.dead)
            {
                owner.StopCoroutine("VampireAttackCoroutine");
            }
        }

    }
    protected class VampireParing : IState
    {
        private Vampire owner;
        public VampireParing(Vampire owner)
        {
            this.owner = owner;
        }
        public void OperateEnter()
        {
        }

        public void OperateExit()
        {
        }

        public void OperateUpdate()
        {
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        AttackEffect.SetActive(false);
        attackArea.SetActive(false);
        attackDetectArea.SetActive(true);
        IState attack = new VampireAttack(this);
        stateDic[EnemyState.Attack] = attack;
        IState paring = new VampireParing(this);
        stateDic[EnemyState.Paring] = paring;

        attackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += VampireAttackArea;
        attackDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += AttackDetectAreaStay;
    }
    private void VampireAttackArea(Collider col)
    {
        if(col.tag == "Player" || col.tag == "DodgePlayer")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            damageable.OnDamage(baseDamage, Vector3.zero, Vector3.zero);
        }
    }
    private void AttackDetectAreaStay(Collider col)
    {
        if (col.tag == "Player" || col.tag == "DodgePlayer")
        {
            if (!dead)
            {
                if (!targetObj.GetComponent<LivingObjects>().dead)
                {
                    if (stateMachine.CurrentState == stateDic[EnemyState.Chase] || stateMachine.CurrentState == stateDic[EnemyState.Idle])
                    {
                        stateMachine.SetState(stateDic[EnemyState.Attack]);
                    }
                }
              
            }
        }
    }

    IEnumerator VampireAttackCoroutine()
    {
        ani.Play("Attack");
        yield return new WaitForSeconds(1.06f);
        AttackEffect.SetActive(true);
        attackArea.SetActive(true);
        yield return new WaitForSeconds(1.04f);
        AttackEffect.SetActive(false);
        attackArea.SetActive(false);

        float x = Random.Range(1f, 1.5f);
        yield return new WaitForSeconds(x);

        if (targetObj != null)
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
        else if (targetObj == null)
        {
            stateMachine.SetState(stateDic[EnemyState.Patrol]);
        }
        
    }
}
