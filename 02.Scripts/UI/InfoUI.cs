using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUI : MonoBehaviour
{
    public Image weapon, weaponDefault, helmet, helmetDefault;
    public TextMeshProUGUI level, exp, hp, addHp, mp, addMp, damage, addDamage, manaRegen, addManaRegen, criticalCance, addcriticalChance;
    public Slider expSlider;

    private void OnEnable()
    {
        addHp.gameObject.SetActive(false);
        addMp.gameObject.SetActive(false);
        addDamage.gameObject.SetActive(false);
        addcriticalChance.gameObject.SetActive(false);
        addManaRegen.gameObject.SetActive(false);

        SetIcons();
        SetStatus();
    }

    private void Update()
    {
        mp.text = (int)GameManager.Instance.playerStatement.mana + "/" + GameManager.Instance.playerStatement.totalMaxMana;
    }

    public void SetIcons()
    {
        weapon.gameObject.SetActive(false);
        helmet.gameObject.SetActive(false);

        weaponDefault.gameObject.SetActive(true);
        helmetDefault.gameObject.SetActive(true);

        foreach(Item item in GameManager.Instance.playerStatement.equipList.Values)
        {
            switch(item.itemType)
            {
                case Item.ItemType.weapon:
                    weapon.sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);
                    weaponDefault.gameObject.SetActive(false);
                    weapon.gameObject.SetActive(true);
                    break;
                case Item.ItemType.helmet:
                    helmet.sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);
                    helmetDefault.gameObject.SetActive(false);
                    helmet.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void SetStatus()
    {
        PlayerStatement playerStatement = GameManager.Instance.playerStatement;

        level.text = playerStatement.level.ToString();
        float reamainExp =
            playerStatement.level == 1 ? (float)playerStatement.exp / DBManager.Instance.playerStatDict[playerStatement.level].exp : (float)(playerStatement.exp - DBManager.Instance.playerStatDict[playerStatement.level - 1].exp) / (float)(DBManager.Instance.playerStatDict[playerStatement.level].exp - DBManager.Instance.playerStatDict[playerStatement.level - 1].exp);

        expSlider.value = reamainExp;
        exp.text = (reamainExp * 100f) + "%";

        if(playerStatement.totalMaxHealth != playerStatement.baseMaxHealth)
        {
            addHp.gameObject.SetActive(true);
        }
        if (playerStatement.totalMaxMana != playerStatement.baseMaxMana)
        {
            addMp.gameObject.SetActive(true);
        }

        if (playerStatement.totalDamage != playerStatement.baseDamage)
        {
            addDamage.gameObject.SetActive(true);
        }

        if (playerStatement.totalCriticalChance != playerStatement.baseCriticalChance)
        {
            addcriticalChance.gameObject.SetActive(true);
        }

        if (playerStatement.totalManaRegen != playerStatement.baseManaRegeneration)
        {
            addManaRegen.gameObject.SetActive(true);
        }

        hp.text = playerStatement.health + " / " + playerStatement.totalMaxHealth;
        addHp.text = "(+" + (playerStatement.totalMaxHealth - playerStatement.baseMaxHealth) + ")";

        mp.text = (int)playerStatement.mana + " / " + playerStatement.totalMaxMana;
        addMp.text = "(+" + (playerStatement.totalMaxMana - playerStatement.baseMaxMana) + ")";

        damage.text = playerStatement.totalDamage.ToString();
        addDamage.text = "(+" + (playerStatement.totalDamage - playerStatement.baseDamage) + ")";

        criticalCance.text = playerStatement.totalCriticalChance.ToString() + "%";
        addcriticalChance.text = "(+" + (playerStatement.totalCriticalChance - playerStatement.baseCriticalChance) + ")";

        manaRegen.text = playerStatement.totalManaRegen.ToString();
        addManaRegen.text = "(+" + (playerStatement.totalManaRegen - playerStatement.baseManaRegeneration) + ")";
    }
}
