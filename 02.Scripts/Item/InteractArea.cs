using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractArea : MonoBehaviour
{
    public Action<Collider> CollisionEnterEvent;
    public Action<Collider> CollisionExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            CollisionEnterEvent(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            CollisionExitEvent(other);
        }
    }
}
