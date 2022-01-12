using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public int enemyId;
    public EnemyData enemyData;
    public Transform[] spawnPoints;
    public float respawnTime = 5f;

    private float Xpos;
    private float Ypos;
    private float Zpos;
    private float Yrot;

    private void Start()
    {
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            Respawn(spawnPoints[i], 0);
        }
    }

    private void SetEnemy(Enemy enemy, EnemyData enemyData)
    {
        enemy.enemyType = enemyData.enemyType;
        enemy.enemyId = enemyData.enemyId;
        enemy.name = enemyData.enemyName;
        enemy.itemNo = enemyData.itemNo;
        enemy.dropChance = enemyData.dropChance;
        enemy.maxAmount = enemyData.maxAmount;
        enemy.exp = enemyData.exp;

        enemy.level = enemyData.level;
        enemy.baseMaxHealth = enemyData.maxHealth;
        enemy.health = enemy.baseMaxHealth;
        enemy.baseDamage = enemyData.damage;

        enemy.enemySlider.gameObject.SetActive(false);
        enemy.targetObj = null;

        enemy.dead = false;
    }

    public void Respawn(Transform spawnPos, float time)
    {
        StartCoroutine(RespawnCoroutine(spawnPos, time));
    }
    private void RandomSpawnPos(Transform spawnPos)
    {
        float tmpX = Random.Range(5f, 5f);
        float tmpz = Random.Range(5f, 5f);
        Yrot = Random.Range(0f, 360f);

        Xpos = spawnPos.position.x + tmpX;
        Zpos = spawnPos.position.z + tmpz;
        Ypos = spawnPos.root.position.y;
    }

    IEnumerator RespawnCoroutine(Transform spawnPos, float time)
    {
        WaitForSeconds ws = new WaitForSeconds(time);

        yield return ws;

        enemyData = DBManager.Instance.enemyDict[enemyId];
        GameObject enemyObj = ObjectPoolingManager.Instance.GetQueue(enemyData.enemyName, false);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.nav.enabled = true;
        enemy.PlayerDetectArea.SetActive(true);
        SetEnemy(enemy, enemyData);
        RandomSpawnPos(spawnPos);
        enemy.wayPointIndex = Random.Range(0, enemy.wayPoints.Length - 1);
        enemy.enemySpawn = this;
        enemy.transform.SetParent(spawnPos);
        enemy.transform.position = new Vector3(Xpos, Ypos, Zpos);
        enemy.transform.rotation = Quaternion.Euler(0f, Yrot, 0f);
        enemy.transform.tag = "Enemy";
        enemyObj.SetActive(true);
    }
}
