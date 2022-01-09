using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Trader : MonoBehaviour
{
    string josnPath;
    public string storeItemListFileName;
    public Dictionary<int, Item> storeItemList = new Dictionary<int, Item>();

    private void OnEnable()
    {
        josnPath = Application.persistentDataPath;
        LoadStoreItemList();
    }

    public void LoadStoreItemList()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + storeItemListFileName);
        ItemList tmp = JsonUtility.FromJson<ItemList>(jsonData.ToString());

        for(int i = 0; i < tmp.itemList.Count; i++)
        {
            storeItemList[tmp.itemList[i].itemNo] = tmp.itemList[i];
        }
    }
}
