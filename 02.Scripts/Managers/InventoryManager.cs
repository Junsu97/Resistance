using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : Singleton<InventoryManager>
{
    public Inventory inventory;

    public GameObject invenTabFocus;
    public GameObject equipTabFocus;

    public GameObject currentInvenTab;
    public string currentInvenTabName = "Equipment";
    public GameObject currentEquipTab;
    public string currentEquipTabName = "All";

    public Transform equipTabTransform;
    public Transform allTabTransfrom;

    [Header("UI")]
    public GameObject equipUI;
    public GameObject potionUI;
    public GameObject etcUI;

    [Header("Slot")]
    public GameObject slotPrefab;

    public Transform equipSlotContainer;
    public Transform potionSlotContainer;
    public Transform etcSlotContainer;

    [Header("ItemInfo")]
    public GameObject itemInfo;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemDamageText;
    public TextMeshProUGUI itemCriticalText;
    public TextMeshProUGUI itemHPText;
    public TextMeshProUGUI itemMPText;
    public TextMeshProUGUI itemMPReGeText;

    public GameObject infoButtons;

    public TextMeshProUGUI playerGoldText;

    public Transform itemGetPanel;
    public Queue<GameObject> itemGetSlotQ = new Queue<GameObject>();
    private bool isRunning = false;

    private Item selectItem;

    public void SetInven(Inventory inven)
    {
        this.inventory = inven;
    }

    public void AddItemGetPanel(string itemName, int amount)
    {
        GameObject itemGetSlot = itemGetPanel.GetChild(0).gameObject;
        itemGetSlot.transform.SetAsLastSibling();

        itemGetSlotQ.Enqueue(itemGetSlot);
        itemGetSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = itemName + "X" + amount;
        itemGetSlot.SetActive(true);

        if(!isRunning)
        {
            isRunning = true;
            StartCoroutine("ItemGetSlotDisable");
        }
    }

    IEnumerator ItemGetSlotDisable()
    {
        WaitForSeconds ws = new WaitForSeconds(2f);
        while(itemGetSlotQ.Count > 0)
        {
            yield return ws;

            itemGetSlotQ.Dequeue().SetActive(false);
        }
        isRunning = false;
    }

    public void OnItemClick(Item item)
    {
        selectItem = item;
        itemNameText.text = item.itemName;
        itemDescText.text = item.itemDescription;

        itemDamageText.gameObject.SetActive(false);
        itemCriticalText.gameObject.SetActive(false);
        itemHPText.gameObject.SetActive(false);
        itemMPText.gameObject.SetActive(false);
        itemMPReGeText.gameObject.SetActive(false);

        if(item.itemType > Item.ItemType.etc)
        {
            infoButtons.SetActive(false);
        }
        else
        {
            if(item.isEquip)
            {
                infoButtons.transform.GetChild(0).gameObject.SetActive(false);
                infoButtons.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                infoButtons.transform.GetChild(0).gameObject.SetActive(true);
                infoButtons.transform.GetChild(1).gameObject.SetActive(false);
            }
            infoButtons.SetActive(true);

            if(item.damage != 0)
            {
                itemDamageText.text = "Damage : +" + item.damage;
                itemDamageText.gameObject.SetActive(true);
            }
            if(item.criticalChance != 0)
            {
                itemCriticalText.text = "Critical : +" + item.criticalChance + "%";
                itemCriticalText.gameObject.SetActive(true);
            }
            if(item.health != 0)
            {
                itemHPText.text = "Health : +" + item.health;
                itemHPText.gameObject.SetActive(true);
            }
            if(item.mana != 0)
            {
                itemMPText.text = "Mana : +" + item.mana;
                itemMPText.gameObject.SetActive(true);
            }
            if(item.manaRegeneration != 0)
            {
                itemMPReGeText.text = "ManaRegen : +" + item.manaRegeneration;
                itemMPReGeText.gameObject.SetActive(true);
            }
        }
        itemInfo.SetActive(true);
    }

    public void RefreshGoldText()
    {
        playerGoldText.text = inventory.gold.ToString();
    }

    public void RefreshInvenItem()
    {
        RefreshGoldText();
        for(int i = 0; i < equipSlotContainer.childCount; i++)
        {
            Destroy(equipSlotContainer.GetChild(i).gameObject);
        }
        for(int i = 0; i < potionSlotContainer.childCount; i++)
        {
            Destroy(potionSlotContainer.GetChild(i).gameObject);
        }
        for(int i = 0; i < etcSlotContainer.childCount; i++)
        {
            Destroy(etcSlotContainer.GetChild(i).gameObject);
        }

        if(currentInvenTabName == "Equipment")
        {
            equipUI.SetActive(true);
            potionUI.SetActive(false);
            etcUI.SetActive(false);

            equipSlotContainer.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            if(currentEquipTabName == "All")
            {
                // 장착한 아이템 최상단
                foreach (Item item in GameManager.Instance.playerStatement.equipList.Values)
                {
                    GameObject slotObj = Instantiate(slotPrefab, equipSlotContainer);
                    slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);

                    slotObj.GetComponent<ItemSlot>().item = item;
                    slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClick;

                    slotObj.transform.Find("IsEquip").gameObject.SetActive(true);
                    slotObj.SetActive(true);
                }

                foreach (Item item in inventory.GetItemList())
                {
                    if(item.itemType < Item.ItemType.etc)
                    {
                        GameObject slotObj = Instantiate(slotPrefab, equipSlotContainer);
                        slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);

                        slotObj.GetComponent<ItemSlot>().item = item;
                        slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClick;

                        slotObj.transform.Find("IsEquip").gameObject.SetActive(false);
                        slotObj.SetActive(true);
                    }
                }
            }

            else
            {
                Item.ItemType tabType = 0;

                switch(currentEquipTabName)
                {
                    case "Weapon":
                        tabType = Item.ItemType.weapon;
                        break;
                    case "Helmet":
                        tabType = Item.ItemType.helmet;
                        break;
                }

                //장착한 아이템 최상단
                foreach(Item item in GameManager.Instance.playerStatement.equipList.Values)
                {
                    if(item.itemType == tabType)
                    {
                        GameObject slotObj = Instantiate(slotPrefab, equipSlotContainer);
                        slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);

                        slotObj.GetComponent<ItemSlot>().item = item;
                        slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClick;

                        slotObj.transform.Find("IsEquip").gameObject.SetActive(true);
                        slotObj.SetActive(true);
                    }
                }

                foreach(Item item in inventory.GetItemList())
                {
                    if(item.itemType == tabType)
                    {
                        GameObject slotObj = Instantiate(slotPrefab, equipSlotContainer);
                        slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);

                        slotObj.GetComponent<ItemSlot>().item = item;
                        slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClick;

                        slotObj.transform.Find("IsEquip").gameObject.SetActive(false);
                        slotObj.SetActive(true);
                    }
                }
            }
        }

        else if(currentInvenTabName == "Potion")
        {
            potionUI.SetActive(true);
            etcUI.SetActive(false);
            equipUI.SetActive(false);

            potionSlotContainer.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            foreach(Item item in inventory.GetItemList())
            {
                if(item.itemType == Item.ItemType.consumable)
                {
                    GameObject slotObj = Instantiate(slotPrefab, potionSlotContainer);
                    slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);
                    TextMeshProUGUI amountT = slotObj.transform.Find("IsAmount").GetComponent<TextMeshProUGUI>();
                    amountT.text = item.amount.ToString();
                    amountT.gameObject.SetActive(true);

                    slotObj.GetComponent<ItemSlot>().item = item;
                    slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClick;
                    slotObj.SetActive(true);
                }
            }
        }

        else
        {
            etcUI.SetActive(true);
            potionUI.SetActive(false);
            equipUI.SetActive(false);

            etcSlotContainer.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            foreach(Item item in inventory.GetItemList())
            {
                if(item.itemType == Item.ItemType.etc)
                {
                    GameObject slotObj = Instantiate(slotPrefab, etcSlotContainer);
                    slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);
                    TextMeshProUGUI amountT = slotObj.transform.Find("IsAmount").GetComponent<TextMeshProUGUI>();
                    amountT.text = item.amount.ToString();
                    amountT.gameObject.SetActive(true);

                    slotObj.GetComponent<ItemSlot>().item = item;
                    slotObj.GetComponent<ItemSlot>().itemClicked = OnItemClick;
                    slotObj.SetActive(true);

                    Debug.Log(slotObj.transform.Find("Image").GetComponent<Image>().sprite.name);
                }
            }
        }
    }

    public void ResetInven()
    {
        OnInventoryTabClick(equipTabTransform.gameObject);
        OnEquipTabClick(allTabTransfrom.gameObject);

        RefreshInvenItem();
    }

    public void OnInventoryTabClick(GameObject tab)
    {
        currentInvenTab.GetComponent<TextMeshProUGUI>().color = Color.white;
        tab.GetComponent<TextMeshProUGUI>().color = new Color(246f / 255f, 225f / 255f, 156f / 255f);

        invenTabFocus.transform.SetParent(tab.transform);
        invenTabFocus.transform.localPosition = new Vector3(0f, invenTabFocus.transform.localPosition.y, 0);

        currentInvenTab = tab;
        currentInvenTabName = tab.name;

        RefreshInvenItem();
    }

    public void OnEquipTabClick(GameObject tab)
    {
        if(currentEquipTabName == "All")
        {
            currentEquipTab.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            currentEquipTab.transform.GetChild(0).gameObject.SetActive(true);
            currentEquipTab.transform.GetChild(1).gameObject.SetActive(false);
        }
        if(tab.name == "All")
        {
            tab.GetComponent<TextMeshProUGUI>().color = new Color(246f / 255f, 225f / 255f, 156f / 255f);
        }
        else
        {
            tab.transform.GetChild(0).gameObject.SetActive(false);
            tab.transform.GetChild(1).gameObject.SetActive(true);
        }

        equipTabFocus.transform.SetParent(tab.transform);
        equipTabFocus.transform.localPosition = new Vector3(0, equipTabFocus.transform.localPosition.y, 0);

        currentEquipTab = tab;
        currentEquipTabName = tab.name;

        RefreshInvenItem();
    }

    public void OnEquipBTClicked()
    {
        if(!selectItem.isEquip)
        {
            GameManager.Instance.playerStatement.EquipItem(selectItem);
        }

        itemInfo.SetActive(false);
    }

    public void OnUnequipBTClicked()
    {
        if(selectItem.isEquip)
        {
            GameManager.Instance.playerStatement.UnEquip(selectItem);
        }
        itemInfo.SetActive(false);
    }
}
