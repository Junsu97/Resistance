using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int itemNo;
    public int amount = 1;

    [HideInInspector]
    public Transform playerTr;
    private float chaseSpeed = 5f;
    private string key;

    private void OnEnable()
    {
        playerTr = GameManager.Instance.player.transform;
        StopCoroutine(ChasePlayer());
    }

    public IEnumerator ChasePlayer()
    {
        while (true)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, playerTr.position, chaseSpeed * Time.deltaTime);
        }
    }

    public IEnumerator DestroyItemObj(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolingManager.Instance.InsertQueue(this.gameObject, key);
    }

    public void SetItem(int itemNo, int amount)
    {
        this.itemNo = itemNo;
        this.amount = amount;
        if(itemNo == 2000)
        {
            key = "BossdropItem";
        }
        else
        {
            key = "dropItem";
        }
    }

    public void RandomeForce()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 50), 250f, Random.Range(0, 50)));
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "DodgePlayer")
        {
            collision.gameObject.GetComponent<PlayerStatement>().inventory.AddItem(DBManager.Instance.itemDict[itemNo], amount);
            if(this.itemNo == 3000 || this.itemNo == 3001)
            {
                GameManager.Instance.playerStatement.PotionTextRefresh();
            }
            else
            {
                Debug.Log("보스아이템");
            }
            StartCoroutine(DestroyItemObj(0f));
        }
    }
}
