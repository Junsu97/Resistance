using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_2 : MonoBehaviour
{   
    public float skillDamage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "DodgePlayer")
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable.OnDamage(skillDamage, Vector3.zero, Vector3.zero);
        }
    }
}
