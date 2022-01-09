using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public float requireMana;
    public float skillCoolTime;
    public GameObject skillArea;

    public Skill(string skillName,float requireMana, float skillCoolTime,GameObject skillArea)
    {
        this.skillName = skillName;
        this.requireMana = requireMana;
        this.skillCoolTime = skillCoolTime;
        this.skillArea = skillArea;
    }
}
