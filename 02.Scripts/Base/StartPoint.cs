using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;
public class StartPoint : MonoBehaviour
{
    public static StartPoint instance;
    
    public static string thisMapname;

    private Image nameBack;
    private TextMeshProUGUI mapName;

    private Animator ani;

    private void Awake()
    {

    }
    void Start()
    {
        //mapName = GameManager.Instance.MapName;
        //nameBack = GameManager.Instance.NameBack;


        //CurrentMapName(SceneManager.GetActiveScene().name);

        //StartCoroutine(ClosedName());
    }
    
}
