using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public float slashDamage = 0f;
    public float slashSpeed = 0f;

    public bool isFire = false;
    void OnEnable()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        isFire = false;
    }

    void Update()
    {
        if(isFire)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * slashSpeed);
            transform.Translate(Vector3.forward * Time.deltaTime * slashSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "DodgePlayer")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();

            damageable.OnDamage(slashDamage, Vector3.zero, Vector3.zero);
        }
    }
}
