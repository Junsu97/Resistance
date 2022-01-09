using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TransferMap : MonoBehaviour
{
    public string transferMapName;

    private PlayerCtrl thePlayer;
    private JoyStick joy;
    private GameObject can;
    private NavMeshAgent nav;
    private void Start()
    {
        joy = GameManager.Instance.stick; 
        thePlayer = GameManager.Instance.player;
        can = GameManager.Instance.mainCanvas;
        nav = thePlayer.GetComponent<NavMeshAgent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") || other.gameObject.tag.Equals("DodgePlayer"))
        {
            joy.isInput = false;
            nav.enabled = false;
            thePlayer.currentMapName = transferMapName;
            GameManager.Instance.LoadScene(transferMapName);
        }
    }
}
