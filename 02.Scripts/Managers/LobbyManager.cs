using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        GameManager.Instance.LoadScene(DBManager.Instance.playerData.currentSceneName);
    }
}
