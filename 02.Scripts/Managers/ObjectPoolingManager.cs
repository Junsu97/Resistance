using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    [System.Serializable]
    public class ObjectInfo
    {
        public GameObject prefab;
        public Queue<GameObject> queue = new Queue<GameObject>();
        public string key;
        public int amount;
    }

    public ObjectInfo[] objectInfo;
    public Dictionary<string, Queue<GameObject>> objPoolDictionary = new Dictionary<string, Queue<GameObject>>();

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < objectInfo.Length; i++)
        {
            Init(objectInfo[i]);
        }
    }

    private void Init(ObjectInfo objectInfo)
    {
        objPoolDictionary.Add(objectInfo.key, objectInfo.queue);

        for (int i = 0; i < objectInfo.amount; i++)
        {
            GameObject clone = Instantiate(objectInfo.prefab, gameObject.transform);
            objectInfo.queue.Enqueue(clone);
            clone.SetActive(false);
        }
    }

    // 함수를 호출할 때 키 값을 패러미터로 호출하여 모든 큐마다 함수를 구현할 필요 없이 키값을 통해 구분하도록 한다. (딕셔너리의 활용)
    public void InsertQueue(GameObject queueObj, string key)
    {
        objPoolDictionary[key].Enqueue(queueObj);
        queueObj.transform.SetParent(gameObject.transform);
        queueObj.SetActive(false);
    }

    // 함수를 호출할 때 키 값을 패러미터로 호출하여 모든 큐마다 함수를 구현할 필요 없이 키값을 통해 구분하도록 한다. (딕셔너리의 활용)
    public GameObject GetQueue(string key, bool isActive = true)
    {
        GameObject queueObj = objPoolDictionary[key].Dequeue();
        queueObj.SetActive(isActive);
        return queueObj;
    }
}
