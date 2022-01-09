using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
public class BossSceneDirector : Singleton<BossSceneDirector>
{
    public GameObject EnemyPool;
    public TimelineAsset BossDirector;
    public PlayableDirector playable;
    public GameObject Boss;
    public GameObject LightningEffect;

    private GameObject MainCam;
    private GameObject BossScenePanel;
    private void Start()
    {
        if(EnemyPool.activeSelf == false)
        {
            EnemyPool.SetActive(true);
        }
        if(Boss.activeSelf == true)
        {
            Boss.SetActive(false);

        }
        MainCam = GameManager.Instance.cam.gameObject;
        BossScenePanel = GameManager.Instance.BossScenePanel;
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.tag == "Player" || col.collider.tag == "DodgePlayer")
        {
            BossScenePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void StartTimeLine()
    {
        BossScenePanel.SetActive(false);
        Time.timeScale = 1f;
        EnemyPool.SetActive(false);
        Boss.SetActive(true);
        Boss.GetComponent<Boss>().enabled = false;
        playable.Play(BossDirector);
        MainCam.gameObject.SetActive(false);
        BGMManager.Instance.PlayBgm("Boss");
        BGMManager.Instance.audioSource.time = 14.91666f;
        BoxCollider box = GetComponent<BoxCollider>();
        box.enabled = false;
    }
    public void ExitBoss()
    {
        Time.timeScale = 1f;
        BossScenePanel.SetActive(false);
    }

    public void EndBossTimeLine()
    {
        playable.Stop();
        playable.gameObject.SetActive(false);
        Boss.GetComponent<Boss>().enabled = true;
        MainCam.SetActive(true);
        LightningEffect.SetActive(false);
    }
}
