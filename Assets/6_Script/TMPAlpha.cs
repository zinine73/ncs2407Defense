using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPAlpha : MonoBehaviour
{
    [SerializeField] private float lerpTime = 0.5f;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 투명하게 사라지기
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(AlphaLerp(1, 0));
    }

    private IEnumerator AlphaLerp(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;
        while (percent < 1)
        {
            // lerpTime 시간동안 while 반복문 실행
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;
            // 폰트의 투명도를 start에서 end로 변경
            Color color = text.color;
            color.a = Mathf.Lerp(start, end, percent);
            text.color = color;
            yield return null;
        }
    }
}
