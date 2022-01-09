using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MessageInOut : MonoBehaviour
{
    private TextMeshProUGUI message;
    private void Awake()
    {
        message = this.GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        StartCoroutine(textAlpha());
    }
    IEnumerator textAlpha()
    {
        float alpha = 1f;
        WaitForSeconds ws = new WaitForSeconds(0.01f);
        while(this.gameObject.activeInHierarchy == true)
        {
            alpha -= (Time.deltaTime * 0.6f);
            yield return ws;
            message.color = new Color(1,0,0,alpha);
            if(message.color.a < 0.5f)
            {
                message.gameObject.SetActive(false);
            }
        }
    }
}
