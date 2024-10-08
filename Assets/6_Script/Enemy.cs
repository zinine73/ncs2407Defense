using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f; // 이동 속도
    [SerializeField] private float maxHP = 5.0f; // 최대 체력
    [SerializeField] private int gold = 10; // 적을 처치했을 때 얻는 골드
    private bool isDie; // 사망 상태
    private int currentIndex; // 현재 경로 인덱스
    private float currentHP; // 현재 체력
    private Animator anim; // 애니메이션 제어용 애니메이터
    private EnemyManager emi; // 자주 사용할 인스턴스 간소화
    private SpriteRenderer spriteRenderer; // 이미지 반전용 SPriteRenderer
    
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
        // SpriteRenderer 연결
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            // 런모드면 더 빠르게
            float fixedSpeed = anim.GetBool("RUN") ? moveSpeed + 2 : moveSpeed;
            
            // 현재위치를 frame처리시간비율로 계산한 속도만큼 옮겨줌
            transform.position = Vector3.MoveTowards(transform.position,
                emi.Waypoints[currentIndex].position, fixedSpeed * Time.deltaTime);

            // 현재 오브젝트가 어느 방향으로 이동하는지 검사
            // MoveTowards 에서 target - current 를 뺀 값의 x 가 0 보다 크냐 작냐로 판단
            Vector3 direction = emi.Waypoints[currentIndex].position - transform.position;

            // 0 보다 크면 오른쪽으로 가는 것이므로 이때 SpriteRender의 FlipX 를 true
            // 위로 올라가는 경우에도 오른쪽을 보게 하자
            spriteRenderer.flipX = (direction.x > 0) || (direction.y > 0);

            // 현재위치가 이동지점의 위치라면 배열 인덱스 +1하여 다음 포인트로 이동하도록.
            if (Vector3.Distance(emi.Waypoints[currentIndex].position, transform.position) == 0f)
                currentIndex++;
        }
        else
        {
            // 목료에 도달했으면 삭제
            OnDie(true);
        }
    }

    /// <summary>
    /// 적이 goal에 도달하거나 체력이 다해 죽을 경우 호출
    /// </summary>
    /// <param name="isArrivedGoal">goal에 도착했는지 여부</param>
    public void OnDie(bool isArrivedGoal = false)
    {
        // 매니저에서 삭제 처리하면서 골드 처리
        EnemyManager.instance.DestroyEnemy(this, gold, isArrivedGoal);
    }

    /// <summary>
    /// 적에게 데미지 주기
    /// </summary>
    /// <param name="damage">주는 데미지 양</param>
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

    /// <summary>
    /// 이 몬스터를 런모드로 만들어서 빠르게 한다
    /// </summary>
    public void StartRunMode()
    {
        anim.SetBool("RUN", true);
    }
}
