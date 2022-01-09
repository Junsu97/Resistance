using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_2 : MonoBehaviour
{   
    public float skillDamage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "DodgePlayer")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.OnDamage(skillDamage, Vector3.zero, Vector3.zero);
        }
    }
}
