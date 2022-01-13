using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform player;
 
    void LateUpdate()
    {
        this.transform.position = player.position + new Vector3(0, 50f, 0);
    }
}
