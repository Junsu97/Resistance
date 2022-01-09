using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastScenePotal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "DodgePlayer")
        {
            GameManager.Instance.LastScenePanel.SetActive(true);
        }
    }
}
