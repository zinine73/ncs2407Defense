using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f; // 이동 속도
    private int currentIndex; // 현재 경로 인덱스
    private EnemyManager emi;
    public void Init()
    {
        emi = EnemyManager.instance;
        // 인덱스는 0으로 시작
        currentIndex = 0;
        // 위치는 시작인덱스에서 해당하는 경로 위치로 지정
        transform.position = emi.Waypoints[currentIndex].position;
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
}
