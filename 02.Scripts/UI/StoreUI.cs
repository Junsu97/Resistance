using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreUI : MonoBehaviour
{
    [Header("Trader")]
    public Trader trader;

    [Header("Item Slot Containers")]
    public Transform buySlotContainer;
    public Transform sellSlotContainer;
    public ScrollRect contentScroll;

    [Header("Item Slots")]
    public GameObject buySlot;
    public GameObject sellSlot;

    [Header("Tab Focusses")]
    public GameObject storeTabFocus;
    public GameObject itemTabFocus;

    [Header("Initial Focu Transform")]
    public Transform buyTabTr;
    public Transform weaponTabTr;

    [Header("Current Tab Datas")]
    public GameObject currentStoreTab;
    string currentStoreTabName = "Buy";
    public GameObject currentItemTab;
    string currentTabName = "Weapon";

    [Header("Player Gold")]
    public TextMeshProUGUI GoldText;

    [Header("Item Infomation Datas")]
    public GameObject itemInfo;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDamageText;
    public TextMeshProUGUI itemCriticalText;
    public TextMeshProUGUI itemHealthText;
    public TextMeshProUGUI itemManaText;
    public TextMeshProUGUI itemManaRegenText;
    public TextMeshProUGUI itemDescText;

    public Button sellBT;
    public TextMeshProUGUI sellPriceText;

    private Item selectedItem;

    private void OnEnable()
    {
        OnStoreTabClick(buyTabTr.gameObject);
        OnItemTabClick(weaponTabTr.gameObject);

        RefreshStore();
    }

    public void RefreshGoldText()
    {
        GoldText.text = string.Format("{0:#,0}", GameManager.Instance.playerStatement.inventory.gold);
    }

    public void OnStoreTabClick(GameObject tab)
    {
        // 활성화된 탭으 색 변경
        currentStoreTab.GetComponent<TextMeshProUGUI>().color = Color.white;
        tab.GetComponent<TextMeshProUGUI>().color = new Color(246f / 255f, 225f / 255f, 156f / 255f);

        // 탭 포커스를 활성화 탭에 위치
        storeTabFocus.transform.SetParent(tab.transform);
        storeTabFocus.transform.localPosition = new Vector3(0f, storeTabFocus.transform.localPosition.y,0f);

        currentStoreTab = tab;
        currentStoreTabName = tab.name;

        RefreshStore();
    }

    public void OnItemTabClick(GameObject tab)
    {
        // 0 : 비활성화 아이콘 1 : 활성화 아이콘
        currentItemTab.transform.GetChild(0).gameObject.SetActive(true);
        currentItemTab.transform.GetChild(0).gameObject.SetActive(false);

        tab.transform.GetChild(0).gameObject.SetActive(false);
        tab.transform.GetChild(1).gameObject.SetActive(true);

        // 탭 포커스를 활성화 탭에 위치
        itemTabFocus.transform.SetParent(tab.transform);
        itemTabFocus.transform.localPosition = new Vector3(0f, itemTabFocus.transform.localPosition.y, 0f);

        currentItemTab = tab;
        currentTabName = tab.name;

        RefreshStore();
    }

    public void RefreshStore()
    {
        RefreshGoldText();
        contentScroll.verticalNormalizedPosition = 1.0f;

        for(int i = 0; i < buySlotContainer.childCount; i++)
        {
            Destroy(buySlotContainer.GetChild(i).gameObject);
        }
        for(int i = 0; i < sellSlotContainer.childCount; i++)
        {
            Destroy(sellSlotContainer.GetChild(i).gameObject);
        }

        if(currentStoreTabName == "Buy")
        {
            contentScroll.content = buySlotContainer.GetComponent<RectTransform>();

            RefreshStoreToBuy();
        }

        else
        {
            contentScroll.content = sellSlotContainer.GetComponent<RectTransform>();
            RefreshStoretoSell();
        }
    }

    public void RefreshStoreToBuy()
    {
        buySlotContainer.gameObject.SetActive(true);
        sellSlotContainer.gameObject.SetActive(false);

        Item.ItemType tabType = 0;
        switch(currentTabName)
        {
            case "Waeapon":
                tabType = Item.ItemType.weapon;
                break;
            case "Potion":
                tabType = Item.ItemType.consumable;
                break;
            case "Helmet":
                tabType = Item.ItemType.helmet;
                break;
        }

        // 현재 상호작용중인 상인 등록
        trader = GameManager.Instance.npcList[GameManager.Instance.currentInteractId].GetComponent<Trader>();

        foreach(Item item in trader.storeItemList.Values)
        {
            if(item.itemType == tabType)
            {
                GameObject slotObj = Instantiate(buySlot, buySlotContainer);

                slotObj.GetComponent<ItemSlot>().item = item;

                if (item.damage != 0)
                {
                    slotObj.transform.Find("StatTexts/Damage").GetComponent<TextMeshProUGUI>().text = "Damage : +" + item.damage;
                    slotObj.transform.Find("StatTexts/Damage").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/Damage").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                }
                if (item.criticalChance != 0)
                {
                    slotObj.transform.Find("StatTexts/Critical").GetComponent<TextMeshProUGUI>().text = "Critical Chance : +" + item.criticalChance;
                    slotObj.transform.Find("StatTexts/Critical").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/Critical").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                }
                if(item.health != 0)
                {
                    slotObj.transform.Find("StatTexts/Health").GetComponent<TextMeshProUGUI>().text = "Health : +" + item.health;
                    slotObj.transform.Find("StatTexts/Health").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/Health").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                }
                if (item.mana != 0)
                {
                    slotObj.transform.Find("StatTexts/Mana").GetComponent<TextMeshProUGUI>().text = "Mana : +" + item.health;
                    slotObj.transform.Find("StatTexts/Mana").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/Mana").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                }
                if (item.manaRegeneration != 0)
                {
                    slotObj.transform.Find("StatTexts/ManaRegen").GetComponent<TextMeshProUGUI>().text = "ManaRegeneration : +" + item.manaRegeneration;
                    slotObj.transform.Find("StatTexts/ManaRegen").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("StatTexts/ManaRegen").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                }


                slotObj.transform.Find("ItemBG/ItemImg").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);
                slotObj.transform.Find("Price/PriceText").GetComponent<TextMeshProUGUI>().text = string.Format("{0:#,0}", item.price);

                // 매개변수가 있는 메소드이므로 델리게이트를 통해 리스너 등록
                // selectedItem 변수에 아이템을 저장하는 방식으로는 델리게이트 사용안해도됨
                slotObj.transform.Find("BuyBT").GetComponent<Button>().onClick.AddListener(delegate { OnBuyButtonClicked(item); });

                slotObj.SetActive(true);
            }
        }
    }

    public void RefreshStoretoSell()
    {
        buySlotContainer.gameObject.SetActive(false);
        sellSlotContainer.gameObject.SetActive(true);

        Item.ItemType tabType = 0;
        switch(currentTabName)
        {
            case "Weapon":
                tabType = Item.ItemType.weapon;
                break;
            case "Potion":
                tabType = Item.ItemType.consumable;
                break;
            case "Helmet":
                tabType = Item.ItemType.helmet;
                break;
        }
        foreach(Item item in GameManager.Instance.playerStatement.equipList.Values)
        {
            if(item.itemType == tabType)
            {
                GameObject slotObj = Instantiate(sellSlot, sellSlotContainer);
                slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);

                slotObj.GetComponent<ItemSlot>().item = item;
                slotObj.GetComponent<ItemSlot>().itemClicked = OnSellItemClicked;

                slotObj.transform.Find("IsEquip").gameObject.SetActive(true);
                slotObj.transform.Find("IsAmount").gameObject.SetActive(false);
                slotObj.SetActive(true);
            }
        }

        foreach(Item item in GameManager.Instance.playerStatement.inventory.GetItemList())
        {
            if(item.itemType == tabType)
            {
                GameObject slotObj = Instantiate(sellSlot, sellSlotContainer);
                slotObj.transform.Find("Image").GetComponent<Image>().sprite = SpriteManager.Instance.LoadItemImage(item.itemNo);


                slotObj.GetComponent<ItemSlot>().item = item;
                slotObj.GetComponent<ItemSlot>().itemClicked = OnSellItemClicked;

                slotObj.transform.Find("IsEquip").gameObject.SetActive(false);

                if(tabType == Item.ItemType.consumable || tabType == Item.ItemType.etc)
                {
                    slotObj.transform.Find("IsAmount").GetComponent<TextMeshProUGUI>().text = item.amount.ToString();
                    slotObj.transform.Find("IsAmount").gameObject.SetActive(true);
                }
                else
                {
                    slotObj.transform.Find("IsAmount").gameObject.SetActive(false);
                }
                slotObj.SetActive(true);
            }
        }
    }

    private void OnBuyButtonClicked(Item item)
    {
        if(item.price <= GameManager.Instance.playerStatement.inventory.gold)
        {
            GameManager.Instance.playerStatement.inventory.AddItem(item, 1);
            GameManager.Instance.playerStatement.inventory.AddGold(-item.price);
            GameManager.Instance.playerStatement.PotionTextRefresh();

            RefreshStore();
        }
    }

    public void OnSellItemClicked(Item item)
    {
        selectedItem = item;
        itemNameText.text = item.itemName;
        itemDescText.text = item.itemDescription;

        itemDamageText.gameObject.SetActive(false);
        itemCriticalText.gameObject.SetActive(false);
        itemHealthText.gameObject.SetActive(false);
        itemManaText.gameObject.SetActive(false);
        itemManaRegenText.gameObject.SetActive(false);

        if (item.damage != 0)
        {
            itemDamageText.text = "Damage : +" + item.damage;
            itemDamageText.gameObject.SetActive(true);
        }

        if (item.criticalChance != 0)
        {
            itemCriticalText.text = "Critical Chance : +" + item.criticalChance + "%";
            itemCriticalText.gameObject.SetActive(true);
        }

        if (item.health != 0)
        {
            itemHealthText.text = "Health : +" + item.health;
            itemHealthText.gameObject.SetActive(true);
        }

        if (item.mana != 0)
        {
            itemManaText.text = "Mana : +" + item.mana;
            itemManaText.gameObject.SetActive(true);
        }

        if (item.manaRegeneration != 0)
        {
            itemManaRegenText.text = "ManaRegen : +" + item.manaRegeneration;
            itemManaRegenText.gameObject.SetActive(true);
        }
        

        sellPriceText.text = string.Format("{0:#,0}", item.price * 0.7f);
        sellBT.gameObject.SetActive(true);
        itemInfo.SetActive(true);
    }

    public void OnSellBTClicked()
    {
        if(selectedItem.isEquip)
        {
            GameManager.Instance.playerStatement.UnEquip(selectedItem);
        }

        if(selectedItem.itemType == Item.ItemType.weapon)
        {
            GameManager.Instance.playerStatement.inventory.RemoveItem(selectedItem);
            GameManager.Instance.playerStatement.ApplyItems();
        }
        else
        {
            GameManager.Instance.playerStatement.inventory.ReduceItem(selectedItem.itemNo, 1);
            GameManager.Instance.playerStatement.PotionTextRefresh();
        }

        GameManager.Instance.playerStatement.inventory.AddGold((int)(selectedItem.price * 0.7f));
        RefreshStore();

        itemInfo.SetActive(false);
    }
}
