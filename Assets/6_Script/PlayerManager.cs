using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; // 싱글턴 인스턴스
    [SerializeField] private float maxHP = 20.0f; // 최대 체력
    [SerializeField] private int currentGold = 100; // 현재 골드
    [SerializeField] private GameObject gameoverUI; // 게임오버 표시
    [SerializeField] private Image imageRed; // 데미지 받았을 때 화면 깜박임
    private float currentHP; // 현재 체력

    public float MaxHP => maxHP; // 최대 체력 프로퍼티
    public float CurrentHP => currentHP;// 현재 체력 프로퍼티
    public int CurrentGold // 현재 골드 프로퍼티
    {
        get => currentGold;
        set => currentGold = Mathf.Max(0, value); // 음수를 적는 경우 대비
    }

    private void Awake()
    {
        // 싱글턴 인스턴스 연결
        if (instance == null) instance = this;
    }

    private void Start()
    {
        // 처음 시작하면 게임오버 표시는 꺼주고
        gameoverUI.SetActive(false);
        // 현재 체력은 최대체력으로 초기화
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        // 데미지 양만큼 체력을 감소시키고
        currentHP -= damage;
        // 돌아가는 코루틴이 있다면 멈추고
        StopCoroutine(HitAlphaAnimation());
        // 체력이 0 이하라면
        if (currentHP <= 0)
        {
            // 게임오버 표시하고
            gameoverUI.SetActive(true);
            // 화면 깜박임 도중일 수 있으므로 깜박이는 image의 알파값을 0으로 만든다
            Color color = imageRed.color;
            color.a = 0.0f;
            imageRed.color = color;
            // 게임 시간을 멈춰서 게임 진행이 안되게 한다
            Time.timeScale = 0;
        }
        else // 아니라면
        {
            // 화면 깜박이는 코루틴 실행
            StartCoroutine(HitAlphaAnimation());
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 이미지의 컬러값을 얻어와서
        Color color = imageRed.color;
        // 알파값만 40%로 설정
        color.a = 0.4f;
        imageRed.color = color;
        // 알파값이 0 이상이면
        while (color.a >= 0.0f)
        {
            // 조금씩 알파값을 감소시킨다
            color.a -= Time.deltaTime;
            imageRed.color = color;
            // 코루틴 반복
            yield return null;
        }
    }
}
