using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class PlayerEditor : EditorWindow
{
    PlayerData playerData = new PlayerData();
    bool isGet = false;
    Vector2 scrollPos;

    [MenuItem("Custom/PlayerEditor")]
    static void Init()
    {
        PlayerEditor playerEditor = (PlayerEditor)EditorWindow.GetWindow(typeof(PlayerEditor));
        playerEditor.Show();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if(GUILayout.Button("Load Player Data"))
        {
            LoadPlayerData();
            isGet = true;
        }

        EditorGUI.BeginDisabledGroup(isGet == false);
        playerData.level = int.Parse(EditorGUILayout.TextField("Level", playerData.level.ToString()));
        playerData.baseMaxHealth = float.Parse(EditorGUILayout.TextField("Base Max Health", playerData.baseMaxHealth.ToString()));
        playerData.baseMaxMana = float.Parse(EditorGUILayout.TextField("Base Max Mana", playerData.baseMaxMana.ToString()));
        playerData.baseManaRegeneration = float.Parse(EditorGUILayout.TextField("Base Mana Regen", playerData.baseManaRegeneration.ToString()));
        playerData.baseDamage = float.Parse(EditorGUILayout.TextField("Base Damage", playerData.baseDamage.ToString()));
        playerData.baseCriticalChance = float.Parse(EditorGUILayout.TextField("Base Critical Chance", playerData.baseCriticalChance.ToString()));
        playerData.health = float.Parse(EditorGUILayout.TextField("Health", playerData.health.ToString()));
        playerData.mana = float.Parse(EditorGUILayout.TextField("Mana", playerData.mana.ToString()));
        playerData.gold = int.Parse(EditorGUILayout.TextField("Gold", playerData.gold.ToString()));
        if (GUILayout.Button("Save Player Data"))
        {
            SavePlayerData();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndScrollView();
    }

    private void LoadPlayerData()
    {
        string fileName = "playerData.json";
        string jsonPath = Application.persistentDataPath;
        Debug.Log(jsonPath);
        if(!File.Exists(jsonPath+"/"+fileName))
        {
            Debug.Log("?????? ???????? ????????. ?????? ???????? ?????? ?????? ??????????????.");
        }
        string jsonData = File.ReadAllText(jsonPath + "/" + fileName);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
    }

    private void SavePlayerData()
    {
        string fileName = "playerData.json";
        string jsonPath = Application.persistentDataPath;
        string jsonData = JsonUtility.ToJson(playerData, true);
        Debug.Log(jsonPath);
        File.WriteAllText(jsonPath + "/" + fileName, jsonData);

        if(Application.isPlaying)
        {
            DBManager.Instance.LoadPlayerDataFromJson();
        }
    }
}
