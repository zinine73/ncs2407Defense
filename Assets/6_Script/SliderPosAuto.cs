using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPosAuto : MonoBehaviour
{
    [SerializeField] private Vector3 distance = Vector3.down * 40.0f; // 위치 지정
    private Transform targetTransform; // 붙여야 할 대상
    private RectTransform rectTransform; // 슬라이더의 rect tf;

    /// <summary>
    /// 슬라이더 설정
    /// </summary>
    /// <param name="target">붙여야 할 대상의 transform</param>
    public void Setup(Transform target)
    {
        // 붙여야 할 대상 지정
        targetTransform = target;
        // rt 컴퍼넌트 연결
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // 대상이 사라졌거나 없으면 리턴
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 실제 표시할 좌표 계산
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);

        // 지정한 위치만큼 떨어져서 붙이기
        rectTransform.position = screenPosition + distance;
    }
}
