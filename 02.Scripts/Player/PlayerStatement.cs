using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatement : LivingObjects
{
    [Header("Total Stats")]
    public float totalMaxHealth;
    public float totalMaxMana;
    public float totalManaRegen;
    public float totalDamage;
    public float totalCriticalChance;
    public int exp;

    [Header("Character Sliders")]
    public Slider healthSlier;
    public Slider manaSlider;

    [Header("Potion Buttons")]
    public Button hpPotionbt;
    public Image hpPotionCool_I;
    public TextMeshProUGUI hpPotionCool_T;
    public float hpPotionCool = 20f;
    public TextMeshProUGUI hpPotionAmount_T;
    private bool canHpPotion = true;

    public Button mpPotionbt;
    public Image mpPotionCool_I;
    public TextMeshProUGUI mpPotionCool_T;
    public float mpPotionCool = 20f;
    public TextMeshProUGUI mpPotionAmount_T;
    private bool canMpPotion = true;

    [Header("Animation")]
    public Animator playerAnim;

    [Header("Inventory")]
    public Inventory inventory;
    public Dictionary<Item.ItemType, Item> equipList = new Dictionary<Item.ItemType, Item>();

    [Header("EquipParts")]
    public GameObject In_Dungeon;
    public GameObject In_Village;
    public GameObject Holster;
    public GameObject head;
    public enum State
    {
        Idle,
        Move,
        Dodge,
        Attack,
        Attack2,
        Attack3,
        Attack4,
        Skill
    }

    public State currentState;

    private void Update()
    {
        DBManager.Instance.SavePlayerData();
    }
    protected void OnEnable()
    {
        foreach(Item equipItem in equipList.Values)
        {
            ApplyItemModel(equipItem);
        }
        if(health <= 0)
        {
            dead = true;
            Die();
        }
        currentState = State.Idle;
        StartCoroutine(ManaRegeneration());
        PotionTextRefresh();
    }

    public void SetComponents()
    {
        playerAnim = GameManager.Instance.player.ani;

        hpPotionbt.onClick.AddListener(OnHealthPotionBTClicked);
        mpPotionbt.onClick.AddListener(OnManaPotionBTClicked);
        PotionTextRefresh();
    }
    // 플레이어 스텟 설정
    protected void SetPlayerStatus(int level, float baseMaxHealth, float baseMaxMana, float health, float mana, float baseManaRegen, float baseDamage)
    {
        base.SetStatus(level, baseMaxHealth, baseMaxMana, health, mana, baseManaRegen, baseDamage);

        totalDamage = baseDamage;
        totalMaxHealth = baseMaxHealth;
        totalMaxMana = baseMaxMana;
        totalManaRegen = baseManaRegen;
    }

    public void ResetPlayerState()
    {
        RestoreHealth(1f);
        RestoreMana(1f);
        dead = false;
        playerAnim.SetTrigger("Reset");
    }
    
    // 장착 아이템을 스텟에 적용
    public void ApplyItems()
    {
        totalDamage = baseDamage;
        totalCriticalChance = baseCriticalChance;
        totalManaRegen = baseManaRegeneration;
        totalMaxHealth = baseMaxHealth;
        totalMaxMana = baseMaxMana;
        foreach(Item equipItem in equipList.Values)
        {
            totalDamage += equipItem.damage;
            totalCriticalChance += equipItem.criticalChance;
            totalManaRegen += equipItem.manaRegeneration;
            totalMaxMana += equipItem.mana;
            totalMaxHealth += equipItem.health;
        }

        if(health > totalMaxHealth)
        {
            health = totalMaxHealth;
        }
        if(mana > totalMaxMana)
        {
            mana = totalMaxMana;
        }

        healthSlier.maxValue = totalMaxHealth;
        manaSlider.maxValue = totalMaxMana;
    }
    
    public void EquipItem(Item selectedItem)
    {
        if(equipList.ContainsKey(selectedItem.itemType))
        {
            UnEquip(selectedItem);
        }

        inventory.RemoveItem(selectedItem);
        selectedItem.isEquip = true;

        equipList.Add(selectedItem.itemType, selectedItem);
        if(playerAnim.isActiveAndEnabled)
        {
            ApplyItemModel(selectedItem);
        }
        ApplyItems();
        InventoryManager.Instance.RefreshInvenItem();
    }
    private void ApplyItemModel(Item item)
    {
        switch(item.itemParts)
        {
            case Item.ItemParts.head:
                head.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case Item.ItemParts.RH:
                Holster.transform.GetChild(item.itemNo).gameObject.SetActive(true);
                In_Dungeon.transform.GetChild(item.itemNo).gameObject.SetActive(true);
                In_Village.transform.GetChild(item.itemNo).gameObject.SetActive(true);
                break;
        }
    }

    public void UnEquip(Item item)
    {
        Item.ItemType type = item.itemType;

        equipList[type].isEquip = false;
        inventory.itemList.Add(equipList[type]);

        switch (type)
        {
            case Item.ItemType.weapon:
                Holster.transform.GetChild(equipList[type].itemNo).gameObject.SetActive(false);
                In_Dungeon.transform.GetChild(equipList[type].itemNo).gameObject.SetActive(false);
                In_Village.transform.GetChild(equipList[type].itemNo).gameObject.SetActive(false);
                break;
            case Item.ItemType.helmet:
                head.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
        equipList.Remove(type);

        ApplyItems();
        InventoryManager.Instance.RefreshInvenItem();
    }
    IEnumerator ManaRegeneration()
    {
        while(true)
        {
            if(mana > totalMaxMana)
            {
                mana = totalMaxMana;
            }
            else
            {
                if(!dead)
                {
                    mana += totalManaRegen * Time.deltaTime;
                    manaSlider.value = mana;
                }
            }

            yield return null;
        }
    }

    public override void RestoreHealth(float amount = 0.2f)
    {
        base.RestoreHealth(amount);

        health += totalMaxHealth * amount;
        if(health > totalMaxHealth)
        {
            health = totalMaxHealth;
        }
        healthSlier.value = health;
    }

    public override void RestoreMana(float amount = 0.2f)
    {
        base.RestoreMana(amount);

        mana += totalMaxMana * amount;

        if(mana > totalMaxMana)
        {
            mana = totalMaxMana;
        }
        manaSlider.value = mana;
    }

    public void ReduceMana(float amount)
    {
        mana -= amount;
        manaSlider.value = mana;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDir, bool isCrit = false)
    {
        if(!dead && transform.tag == "Player")
        {
            base.OnDamage(damage, hitPoint, hitDir);
            GameManager.Instance.cam.ShakeCam(0.1f, 0.1f);
            healthSlier.value = health;
        }
        else if(transform.tag == "DodgePlayer")
        {
            PlayerAttack attack = GameManager.Instance.player.transform.GetComponent<PlayerAttack>();
            attack.SPSkill();
        }

    }

    public override void Die()
    {
        base.Die();
        
        playerAnim.SetTrigger("Die");
        GameManager.Instance.DieEnable();
    }

    public void OnHealthPotionBTClicked()
    {
        if(!dead && canHpPotion && inventory.GetItemAmount(3000) > 0)
        {
            canHpPotion = false;
            
            inventory.ReduceItem(3000, 1);
            PotionTextRefresh();

            RestoreHealth();
            StartCoroutine(CoolTimeHealthPotion());
        }
    }

    protected IEnumerator CoolTimeHealthPotion()
    {
        float tmp = hpPotionCool;

        hpPotionCool_I.gameObject.SetActive(true);
        hpPotionCool_T.gameObject.SetActive(true);
        WaitForFixedUpdate wf = new WaitForFixedUpdate();
        while(tmp > 0)
        {
            tmp -= Time.deltaTime;

            hpPotionCool_I.fillAmount = tmp / hpPotionCool;
            hpPotionCool_T.text = ((int)(tmp)).ToString();

            yield return wf;
        }

        hpPotionCool_I.gameObject.SetActive(false);
        hpPotionCool_T.gameObject.SetActive(false);

        canHpPotion = true;
    }

    public void OnManaPotionBTClicked()
    {
        if(!dead && canMpPotion && inventory.GetItemAmount(3001) > 0)
        {
            canMpPotion = false;
            inventory.ReduceItem(3001, 1);

            PotionTextRefresh();
            RestoreMana();

            StartCoroutine(CoolTimeManaPotion());
        }
    }
    protected IEnumerator CoolTimeManaPotion()
    {
        WaitForFixedUpdate wf = new WaitForFixedUpdate();

        float tmp = mpPotionCool;
        mpPotionCool_I.gameObject.SetActive(true);
        mpPotionCool_T.gameObject.SetActive(true);

        while(tmp > 0)
        {
            tmp -= Time.deltaTime;

            mpPotionCool_I.fillAmount = tmp / mpPotionCool;
            mpPotionCool_T.text = ((int)(tmp)).ToString();

            yield return wf;
        }

        mpPotionCool_I.gameObject.SetActive(false);
        mpPotionCool_T.gameObject.SetActive(false);
    }

    public void PotionTextRefresh()
    {
        hpPotionAmount_T.text = inventory.GetItemAmount(3000) == -1 ? "0" : inventory.GetItemAmount(3000).ToString();
        mpPotionAmount_T.text = inventory.GetItemAmount(3001) == -1 ? "0" : inventory.GetItemAmount(3001).ToString();
    }
}
