using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]

public class Monster : MonoBehaviour
{
    public static Monster instance;

    private float Xpos;
    private float YPos;
    private float ZPos;

    private float Yrot;

    private Vector3 RandomVector;
    private Transform parent;

    [Header("Player"),SerializeField]
    protected PlayerCtrl player;

    [Header("Monster")]
    public Animator m_ani;
    public float Dis;
    public float MaxDis;

    Quaternion rot;
    Vector3 SpawnPoint;
    public int HP;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }
    public void OnEnable()
    {  
        player = GameManager.Instance.player;
        parent = this.transform.root;
        m_ani = this.GetComponent<Animator>();
        SpawnPos();
    }
    public void SpawnPos()
    {
        float tmpX = Random.Range(-16.5f, 14f);
        float tmpZ = Random.Range(-18f, 18f);
        Yrot = Random.Range(0f, 360f);

        Xpos = this.transform.root.position.x + tmpX;
        ZPos = this.transform.root.position.z + tmpZ;
        YPos = this.transform.root.position.y;


        RandomVector = new Vector3(Xpos, YPos, ZPos);
        this.transform.position = RandomVector;
        SpawnPoint = this.transform.position;
        this.transform.rotation = Quaternion.Euler(0f, Yrot, 0f);
    }
  
    private void CheckDis()
    {
        rot = Quaternion.LookRotation(player.transform.position - this.transform.position);
        Dis = Vector3.Distance(player.transform.position, this.transform.position);
        MaxDis = Vector3.Distance(parent.transform.position, this.transform.position);
        if (Dis <= 5f)
        {
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * 0.1f);
        }
        if (MaxDis < 24f && Dis > 5f)
        {
            this.transform.position = SpawnPoint;
        }
    }

    protected void Update()
    {
        if (HP <= 0)
        {
            ObjectPool.Instance.PushToPool(this.gameObject.name, gameObject, parent);
        }
        CheckDis();
    }
}
