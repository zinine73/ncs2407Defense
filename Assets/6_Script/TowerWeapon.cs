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
    private WeaponState weaponState = WeaponState.SearchTarget; // 타워 상태 저장 변수
    private Transform attackTarget = null; // 공격 목표

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

    void Update()
    {
        
    }

    private IEnumerator SearchTarget()
    {
        while(true)
        {
            // 가장 가까운 거리를 찾기 위해 가장 큰 수로 먼저 초기화
            float closestDistance = Mathf.Infinity;
            // 리스트에 있는 모든 오브젝트 검사
            //EnemyManager.instance.EnemyList
                // 
        }
        yield return null;
    }

    private IEnumerator AttackToTarget()
    {
        while(true)
        {

        }
        yield return null;
    }

    private void SpawnProjectile()
    {
        // 발사체프리펩에서 발사체 생성

        // 발사체에 공격목표 지정

    }
}
