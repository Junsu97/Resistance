using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobState : MonoBehaviour
{
    [Header("PatrolWayPoint")]
    public List<Transform> wayPoints = new List<Transform>();
    public int nextIdx;
   // private 

    [Header("Attack")]
    public float AttackRange;
    public float nextAttack = 0f;
    public float AttackRate;
    public bool isAttack = false;

    void Start()
    {
        var group = GameObject.Find("WayPointGroup");

        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);
        }
    }
}
