using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerCallback : MonoBehaviour
{
    public Action<Collider> CollisionEnterEvent;
    public Action<Collider> CollisionStayEvent;
    public Action<Collider> CollisionExtiEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if(CollisionEnterEvent != null)
        {
            CollisionEnterEvent(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(CollisionStayEvent != null)
        {
            CollisionStayEvent(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(CollisionExtiEvent != null)
        {
            CollisionExtiEvent(other);
        }
    }
}
