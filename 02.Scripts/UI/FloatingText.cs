using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI floating;
    private void OnEnable()
    {
        floating = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        this.transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }

    public void DestroyObj(float time)
    {
        StartCoroutine(DestroyObjCoroutine(time));
    }

    private IEnumerator DestroyObjCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolingManager.Instance.InsertQueue(gameObject, "floatingText");
    }
}
