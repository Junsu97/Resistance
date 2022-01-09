using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_1 : MonoBehaviour
{
    public float Skill_1Damage = 0f;

    public float skill_1Range = 0f;
    public float skill_1Speed = 0f;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        GetComponent<SphereCollider>().enabled = false;
    }
    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(skill_1Range, skill_1Range, skill_1Range), Time.deltaTime * skill_1Speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "DodgePlayer")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();

            damageable.OnDamage(Skill_1Damage, Vector3.zero, Vector3.zero);
        }
    }
}
