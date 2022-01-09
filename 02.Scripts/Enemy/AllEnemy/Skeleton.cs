using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Skeleton : Enemy
{
    public GameObject attackArea, attackDetecArea;

    protected class SkeletonAttack : IState
    {
        private Skeleton owner;
        private NavMeshAgent nav;
        public SkeletonAttack(Skeleton owner)
        {
            this.owner = owner;
            this.nav = owner.nav;
        }

        public void OperateEnter()
        {
            nav.isStopped = true;
            owner.StartCoroutine("SkeletonAttackCoroutine");
            LookTarget();
        }

        public void OperateExit()
        {
            if(nav.isStopped == true)
            {
                nav.isStopped = false;
            }
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
    protected class SkeletonParing : IState
    {
        private Skeleton owner;
        public SkeletonParing(Skeleton owner)
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
        attackDetecArea.SetActive(true);
        attackArea.SetActive(false);
        IState attack = new SkeletonAttack(this);
        stateDic[EnemyState.Attack] = attack;
        IState paring = new SkeletonParing(this);
        stateDic[EnemyState.Paring] = paring;

        attackArea.GetComponent<TriggerCallback>().CollisionEnterEvent += SkeletonAttackArea;
        attackDetecArea.GetComponent<TriggerCallback>().CollisionStayEvent += AttackDetectAreaStay;

        if(nav.enabled == false)
        {
            nav.enabled = true;
        }
    }

    private void SkeletonAttackArea(Collider col)
    {
        if(col.tag == "Player" || col.tag == "DodgePlayer")
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            damageable.OnDamage(baseDamage, Vector3.zero, Vector3.zero);
        }
    }

    private void AttackDetectAreaStay(Collider col)
    {
        if(col.tag == "Player" || col.tag == "DodgePlayer")
        {
            if(!dead && !Back)
            {
               if(!targetObj.GetComponent<LivingObjects>().dead && stateMachine.CurrentState != stateDic[EnemyState.Attack] && stateMachine.CurrentState != stateDic[EnemyState.Patrol])
                {
                    stateMachine.SetState(stateDic[EnemyState.Attack]);
                }
            }
        }
    }
   
    IEnumerator SkeletonAttackCoroutine()
    {
        ani.Play("Attack");
        yield return new WaitForSeconds(1.08f);
        attackArea.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(1.8f);

        if(targetObj != null && !Back)
        {
            stateMachine.SetState(stateDic[EnemyState.Chase]);
        }
        else if(targetObj == null)
        {
            stateMachine.SetState(stateDic[EnemyState.Patrol]);
        }
    }
}
