using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemySlider : MonoBehaviour
{
    public Slider hpSlider;
    public bool isBoss;
    public TextMeshProUGUI bossNameText;

    private void OnEnable()
    {
        hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        if(!isBoss)
        {
            this.transform.LookAt(2 * transform.position - GameManager.Instance.cam.transform.position);
        }
    }

    public void SetSliderValue(Enemy enemy)
    {
        hpSlider.maxValue = enemy.baseMaxHealth;
        hpSlider.value = enemy.health;
    }
}
