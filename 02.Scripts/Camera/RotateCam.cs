using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCam : MonoBehaviour, IBeginDragHandler, IDragHandler
{

    public Transform Pivot;
    float camSpeed = 0.4f;

    Vector3 beginPos;
    Vector3 draggingPos;
    float xAngle;
    float yAngle;
    float xAngletmp;
    float yAngletmp;

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginPos = eventData.position;

        xAngletmp = xAngle;
        yAngletmp = yAngle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggingPos = eventData.position;

        yAngle = yAngletmp + (draggingPos.x - beginPos.x) * 180 / Screen.width * camSpeed;
        xAngle = xAngletmp - (draggingPos.y - beginPos.y) * 90 / Screen.height * camSpeed;

        if (xAngle > 30)
            xAngle = 30;
        if (xAngle < -28)
            xAngle = -28;

        Pivot.rotation = Quaternion.Euler(Pivot.rotation.x + xAngle, Pivot.rotation.y + yAngle, 0.0f);
    }

}
