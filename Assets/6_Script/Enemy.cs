using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f; // 이동 속도
    [SerializeField] private float maxHP = 5.0f; // 최대 체력
    private bool isDie; // 사망 상태
    private int currentIndex; // 현재 경로 인덱스
    private float currentHP; // 현재 체력
    private Animator anim; // 애니메이션 제어용 애니메이터
    private EnemyManager emi;

    public float MaxHP => maxHP; // 최대 체력 프로퍼티
    public float CurrentHP => currentHP; // 현재 체력 프로퍼티

    /// <summary>
    /// 적을 생성한 후 반드시 처음에 한번 호출해서 적을 초기화
    /// </summary>
    public void Init()
    {
        emi = EnemyManager.instance;
        // 인덱스는 0으로 시작
        currentIndex = 0;
        // 위치는 시작인덱스에서 해당하는 경로 위치로 지정
        transform.position = emi.Waypoints[currentIndex].position;
        // 애니메이터 연결
        anim = GetComponent<Animator>();
        // 현재 체력을 최대치로 초기화
        currentHP = maxHP;
        // 살아 있는 상태로 시작
        isDie = false;
    }

    private void Update()
    {
        // 이동지점 배열 인덱스 0 부터 배열크기 -1까지
        if (currentIndex < emi.Waypoints.Length)
        {
            // 현재위치를 frame처리시간비율로 계산한 속도만큼 옮겨줌
            transform.position = Vector3.MoveTowards(transform.position,
                emi.Waypoints[currentIndex].position, moveSpeed * Time.deltaTime);

            // 현재위치가 이동지점의 위치라면 배열 인덱스 +1하여 다음 포인트로 이동하도록.
            if (Vector3.Distance(emi.Waypoints[currentIndex].position, transform.position) == 0f)
                currentIndex++;
        }
        else
        {
            // 목료에 도달했으면 삭제
            OnDie();
        }
    }

    public void OnDie()
    {
        // 매니저에서 삭제 처리
        EnemyManager.instance.DestroyEnemy(this);
    }

    public void TakeDamage(float damage)
    {
        // 죽은 상태면 더 이상 데미지를 받지 않도록 리턴
        if (isDie) return;
        // 데미지만큼 현재 체력 감소
        currentHP -= damage;
        // 체력이 0 이하인지 검사
        if (currentHP <= 0)
        {
            // 죽은 상태로 만들고 
            isDie = true;
            // 삭제 처리
            OnDie();
        }
        else // 아니면
        {
            // 피격 애니메이션 실행
            anim.SetTrigger("HIT");
        }
    }
}
