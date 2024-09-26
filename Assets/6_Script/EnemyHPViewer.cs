using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPViewer : MonoBehaviour
{
    private Enemy enemy;
    private Slider slider;

    public void Setup(Enemy inEnemy)
    {
        enemy = inEnemy;
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        // 슬라이더 값은 0.0f ~ 1.0f 사이 값으로 지정
        slider.value = enemy.CurrentHP / enemy.MaxHP;
    }
}
