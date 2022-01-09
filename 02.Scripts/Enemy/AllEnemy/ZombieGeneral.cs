using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZombieGeneral : Enemy
{
    public GameObject attackArea, attackDetectArea;
    
    protected class ZombieGeneralAttack : IState
    {
        private ZombieGeneral owner;
        private NavMeshAgent nav;
        public ZombieGeneralAttack(ZombieGeneral owner)
        {
            this.nav = owner.nav;
            this.owner = owner;
        }
        public void OperateEnter()
        {
            nav.isStopped = true;
            LookTarget();
            owner.StartCoroutine("ZombieGeneralAttackCoroutin");
        }

        public void OperateExit()
        {
            nav.isStopped = false;
        }

        public void OperateUpdate()
        {
        }
        private void LookTarget()
        {
            Vector3 dir = owner.targetObj.transform.position - owner.transform.position;

            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * owner.rotateSpeed);
        }
    }
    protected class ZombieGeneralParing : IState
    {
        private ZombieGeneral owner;
        public ZombieGeneralParing(ZombieGeneral owner)
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
        attackDetectArea.SetActive(true);
        attackArea.SetActive(false);
        IState attack = new ZombieGeneralAttack(this);
        stateDic[EnemyState.Attack] = attack;
        IState paring = new ZombieGeneralParing(this);
        stateDic[EnemyState.Paring] = paring;

        attackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += ZombieGenralAttackArea;
        attackDetectArea.GetComponent<TriggerCallback>().CollisionStayEvent += AttackDetectAreaStay;
    }
    private void ZombieGenralAttackArea(Collider col)
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
            if (!dead && !Back)
            {
                if (!targetObj.GetComponent<LivingObjects>().dead && stateMachine.CurrentState != stateDic[EnemyState.Attack] && stateMachine.CurrentState != stateDic[EnemyState.Patrol])
                {
                    stateMachine.SetState(stateDic[EnemyState.Attack]);
                }
            }
        }
    }

    IEnumerator ZombieGeneralAttackCoroutin()
    {
        ani.Play("Attack");
        yield return new WaitForSeconds(0.24f);
        attackArea.SetActive(true);
        yield return new WaitForSeconds(0.76f);
        attackArea.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        if(targetObj != null)
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
        else
        {
            stateMachine.SetState(stateDic[EnemyState.Patrol]);
        }
    }
}
