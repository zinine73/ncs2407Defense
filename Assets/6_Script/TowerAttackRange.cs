using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    private void Start()
    {
        OffAttackRange();
    }

    /// <summary>
    /// 공격범위 이미지 설정
    /// </summary>
    /// <param name="position">이미지가 보여져야 할 위치</param>
    /// <param name="range">공격 범위 값</param>
    public void OnAttackRange(Vector3 position, float range)
    {
        // 이미지를 켠다
        gameObject.SetActive(true);
        // 지름을 구하고
        float diameter = range * 2.0f;
        // 지름 크기만큼 키우고
        transform.localScale = Vector3.one * diameter;
        // 위치 지정
        transform.position = position;
    }

    /// <summary>
    /// 공격 범위 이미지를 끈다
    /// </summary>
    public void OffAttackRange()
    {
        // 이미지를 끈다
        gameObject.SetActive(false);
    }
}
