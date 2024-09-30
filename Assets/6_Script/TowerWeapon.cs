using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState // 타워 상태
{
    SearchTarget,   // 적을 찾기
    AttactToTarget  // 적을 공격 중
}

public class TowerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // 발사체 프리펩
    [SerializeField] private Transform spawnPoint; // 발사체 생성 위치
    [SerializeField] private float attackRate = 0.5f; // 발사 간격
    [SerializeField] private float attackRange = 2.0f; // 발사체의 생성 범위
    [SerializeField] private float attackDamage = 1.0f; // 발사체 공격력
    private WeaponState weaponState = WeaponState.SearchTarget; // 타워 상태 저장 변수
    private Transform attackTarget = null; // 공격 목표

    /// <summary>
    /// 타워 생성 후 초기화로 반드시 한번 호출
    /// </summary>
    public void Init()
    {
        // 적 찾기 상태로 초기화
        ChangeState(WeaponState.SearchTarget);
    }

    private void ChangeState(WeaponState newState)
    {
        // 이름을 코루틴 파라미터로 설정할 수 있는 것을 이용하여 현재 코루틴 종료
        StopCoroutine(weaponState.ToString());
        // 상태 바꾸기
        weaponState = newState;
        // 새로운 코루틴 시작
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        // 공격 중이면
        if (attackTarget != null)
        {
            // 타워 돌리기 (위를 향하고 있는 스프라이트이므로 up)
            transform.up = attackTarget.position - transform.position;
        }
    }

    private IEnumerator SearchTarget()
    {
        while(true)
        {
            // 가장 가까운 거리를 찾기 위해 가장 큰 수로 먼저 초기화
            float closestDistance = Mathf.Infinity;
            // 리스트에 있는 모든 오브젝트 검사
            foreach (var item in EnemyManager.instance.EnemyList)
            {
                // 두 오브젝트 사이의 거리를 측정
                float distance = Vector3.Distance(item.transform.position, transform.position);
                // 공격 사정 거리 안에 있으면서 가장 긴 거리보다 작으면
                if ((distance <= attackRange) && (distance <= closestDistance))
                {
                    // 현재 거리를 최단 거리로 지정
                    closestDistance = distance;
                    // 공격 목표를 현재 오브젝트로 지정
                    attackTarget = item.transform;
                }
            }
            // 공격 목표가 null이 아니면
            if (attackTarget != null)
            {
                // 공격 상태로 변경
                ChangeState(WeaponState.AttactToTarget);
            }
            // 코루틴으로 계속 적 찾기
            yield return null;
        }
    }

    private IEnumerator AttactToTarget()
    {
        while(true)
        {
            // 공격 목표가 사라지면
            if (attackTarget == null)
            {
                // 적 찾기 상태로 변경
                ChangeState(WeaponState.SearchTarget);
                // 바로 while문 빠져 나오기
                break;
            }
            // 적과의 거리 측정
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            // 거리가 공격범위를 벗어나면
            if (distance > attackRange)
            {
                // 공격목표를 없애고
                attackTarget = null;
                // 적 찾기 상태로 변경
                ChangeState(WeaponState.SearchTarget);
                // 바로 while문 빠져 나오기
                break;
            }
            // 발사간격만큼 기다린 후 다시 공격
            yield return new WaitForSeconds(attackRate);
            // 발사체 생성
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        // 발사체프리펩에서 발사체 생성
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, 
            Quaternion.identity, transform);

        // 발사체에 공격목표 지정하면서 공격력도 전달
        clone.GetComponent<Projectile>().SetTarget(attackTarget, attackDamage);
    }
}
