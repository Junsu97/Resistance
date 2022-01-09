using System;
using TMPro;
using UnityEngine;

public abstract class LivingObjects : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    public int level;
    public float baseMaxHealth;     // ���̽� ����
    public float baseMaxMana;
    public float baseManaRegeneration;
    public float baseDamage;
    public float baseCriticalChance;
    public float health; //���� ü��
    public float mana;  // ���� ����

    public bool dead;

    public Transform damageTextTr;


    protected virtual void SetStatus(int level,float baseMaxHealth,float baseMaxMana,float health,float mana, float baseManaRegeneration, float baseDamage)
    {
        this.level = level;
        this.baseMaxHealth = baseMaxHealth;
        this.baseMaxMana = baseMaxMana;
        this.health = health;
        this.mana = mana;
        this.baseManaRegeneration = baseManaRegeneration;
        this.baseDamage = baseDamage;
        dead = false;
    }
    // ������
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal, bool isCrit = false)
    {
        GameObject damageText = ObjectPoolingManager.Instance.GetQueue("floatingText");
        if(!dead)
        {
            if (isCrit == true)
            {
                damage *= 1.5f;
                damageText.GetComponent<TextMeshProUGUI>().color = Color.red;
                Debug.Log("LivingOBJ ũ��Ƽ��");
            }
            else
            {
                damageText.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }        
        health -= damage;
        damageText.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        damageText.transform.position = damageTextTr.position;
        damageText.transform.SetParent(damageTextTr);
        damageText.GetComponent<FloatingText>().DestroyObj(2.0f);

        if(health <= 0 && !dead)
        {
            Die();
            this.transform.tag = "Dead";
        }
    }
    // ü��ȸ��
    public virtual void RestoreHealth(float amount)
    {
        if (dead) { return; }
    }
    public virtual void RestoreMana(float amount)
    {
        if (dead) { return; }
    }
    // ���
    public virtual void Die()
    {
        dead = true;
    }
}
