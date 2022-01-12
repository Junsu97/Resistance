using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public List<PooledObject> objectPool = new List<PooledObject>(); //풀에 담을 프리팹
    private void Awake()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            objectPool[i].Initialize(transform);
        }
    }

    public bool PushToPool(string itemName, GameObject item, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        
        if (pool == null)
        {
            return false;
        }
        if(item == null)
        {
            Debug.LogWarning("ㅇㅇㅇㅇㅇ");
        }
        item.transform.position = Vector3.zero;
        item.transform.rotation = Quaternion.identity;
      //  item.transform.SetParent(this.transform);
        pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }
    public GameObject PopFromPool(string itemName, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
        {
            return null;

        }
        return pool.PopFromPool(parent);
    }
    public PooledObject GetPoolItem(string itemName)
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (objectPool[i].poolItemName.Equals(itemName))
            {
                return objectPool[i];
            }
        }

        Debug.LogWarning("There's no matched pool list.");
        return null;
    }
}
