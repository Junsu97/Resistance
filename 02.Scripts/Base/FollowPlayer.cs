using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform plaeyr;
    private void LateUpdate()
    {
        transform.position = plaeyr.position;
    }
}
