using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    private ObjectPool pool;
    private GameObject item;
    private string thisName;

    [SerializeField]
    private int poolCount;  //Ǯ�� �̸� ��Ƶ� ����
    public int activeCount; // Ȱ��ȭ �� Ǯ�� ������Ʈ�� ����

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        pool = ObjectPool.Instance;
        thisName = this.gameObject.name;
    }

    void Start()
    {
        poolCount = pool.GetPoolItem(thisName).poolCount;
        StartCoroutine(SpawnMonster());
    }

    public void SpawnCondition()//string name)
    {
        item = pool.PopFromPool(thisName);
        
        item.SetActive(true);

        if (item.activeSelf)
        {
            item.transform.SetParent(this.transform);
        }
    }
    private void FixedUpdate()
    {
        activeCount = this.transform.childCount;
    }
    IEnumerator SpawnMonster()
    {
        WaitForFixedUpdate wf = new WaitForFixedUpdate();
        WaitForSeconds ws = new WaitForSeconds(10f);

        do
        {
            for (int i = 0; i < poolCount; i++)
            {
                SpawnCondition();
                yield return wf;
            }
        }
        while (activeCount < poolCount);
        yield return ws;

    }

}
