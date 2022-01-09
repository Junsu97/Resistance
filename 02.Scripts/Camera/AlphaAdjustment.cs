using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaAdjustment : MonoBehaviour
{
    List<MeshRenderer> alphalist = new List<MeshRenderer>();
    Transform player;
    public bool Level_3;
    private void Awake()
    {
    }
    void Start()
    {
        player = GameManager.Instance.player.transform;

        StartCoroutine(YieldAlphalist(0.2f));
    }

    IEnumerator YieldAlphalist(float time)
    {
        WaitForSeconds upTime = new WaitForSeconds(time);

        while(Level_3)
        {
            if(alphalist.Count > 0)
            {
                foreach(var item in alphalist)
                {
                    Color clr = item.material.color;
                    clr.a = 1f;
                    item.material.color = clr;
                }
                alphalist.Clear();
            }

            Ray ray = new Ray(transform.position, player.position - transform.position);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            for(int i =0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag != "Terrain" && hits[i].transform.tag != "Player" && hits[i].transform.tag != "Enemy")
                {
                    MeshRenderer mesh = hits[i].collider.GetComponent<MeshRenderer>();
                    Color clr = mesh.material.color;
                    clr.a = 0.2f;
                    mesh.material.color = clr;

                    alphalist.Add(mesh);
                }
            }
            yield return upTime;
        }
    }
}
