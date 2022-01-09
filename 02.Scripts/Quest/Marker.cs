using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private Transform camTr;

    private void Start()
    {
        camTr = Camera.main.transform.transform;
    }

    void Update()
    {
        Vector3 targetPos = new Vector3(camTr.position.x, transform.position.y, camTr.position.z);
        this.transform.LookAt(targetPos);
        transform.Rotate(0f, 180f, 0f);
    }
}
