using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType // 타워 종류
{
    Gun,        // 유도탄
    Laser       // 레이저
}

public enum WeaponState // 타워 상태
{
    SearchTarget,   // 적을 찾기
    TryAttactGun,   // 적을 유도탄으로 공격 중
    TryAttackLaser  // 레이저 공격 중
}

public class TowerWeapon : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private TowerTemplate towerTemplate; // 타워 정보
    [SerializeField] private Transform spawnPoint; // 발사체 생성 위치
    [SerializeField] private WeaponType weaponType; // 무기 속성 설정

    [Header("Gun")]
    [SerializeField] private GameObject projectilePrefab; // 발사체 프리펩

    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer; // 레이저로 사용되는 선
    [SerializeField] private Transform hitEffect; // 타격 효과

    private WeaponState weaponState = WeaponState.SearchTarget; // 타워 상태 저장 변수
    private Transform attackTarget = null; // 공격 목표
    private SpriteRenderer spriteRenderer; // 타워 이미지 출력 렌더러
    private int level = 0; // 타워 레벨

    #region Property
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite; // 레벨에 따른 타워 이미지 프로퍼티
    public int Level => level + 1; // 타워 레벨 프로퍼티
    public float Damage => towerTemplate.weapon[level].damage; // 공격력 프로퍼티
    public float Rate => towerTemplate.weapon[level].rate; // 발사 간격 프로퍼티
    public float Range => towerTemplate.weapon[level].range; // 생성범위 프로퍼티
    public int CostUpgrade => Level < MaxLevel ? towerTemplate.weapon[level + 1].cost : 0; // 업그레이드 비용 프로퍼티
    public int CostSell => towerTemplate.weapon[level].sell; // 판매 비용 프로퍼티
    public int MaxLevel => towerTemplate.weapon.Length; // 최대 레벨
    #endregion Property

    /// <summary>
    /// 타워 생성 후 초기화로 반드시 한번 호출
    /// </summary>
    public void Init()
    {
        // 이미지 변경용 렌더러 연결
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private Transform FindClosestAttackTarget()
    {
        // 가장 가까운 거리를 찾기 위해 가장 큰 수로 먼저 초기화
        float closestDistance = Mathf.Infinity;
        // 리스트에 있는 모든 오브젝트 검사
        foreach (var item in EnemyManager.instance.EnemyList)
        {
            // 두 오브젝트 사이의 거리를 측정
            float distance = Vector3.Distance(item.transform.position, transform.position);
            // 공격 사정 거리 안에 있으면서 가장 긴 거리보다 작으면
            if ((distance <= towerTemplate.weapon[level].range) && (distance <= closestDistance))
            {
                // 현재 거리를 최단 거리로 지정
                closestDistance = distance;
                // 공격 목표를 현재 오브젝트로 지정
                attackTarget = item.transform;
            }
        }
        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        // 대상이 있는지 검사
        if (attackTarget == null)
        {
            return false;
        }
        // 적과의 거리 측정
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        // 거리가 공격범위를 벗어나면
        if (distance > towerTemplate.weapon[level].range)
        {
            // 공격목표를 없애고
            attackTarget = null;
            return false;
        }
        return true;
    }

    private IEnumerator SearchTarget()
    {
        while(true)
        {
            // 현재 타워에 가장 가까이 있는 공격 대상 탐색
            attackTarget = FindClosestAttackTarget();

            // 공격 목표가 null이 아니면
            if (attackTarget != null)
            {
                // 무기 상태에 따른 공격 상태로 변경
                if (weaponType == WeaponType.Gun)
                {
                    ChangeState(WeaponState.TryAttactGun);
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }
            // 코루틴으로 계속 적 찾기
            yield return null;
        }
    }

    private IEnumerator TryAttactGun()
    {
        while(true)
        {
            // 타겟이 공격 가능한지 검사
            if (IsPossibleToAttackTarget() == false)
            {
                // 적 찾기 상태로 변경
                ChangeState(WeaponState.SearchTarget);
                // 바로 while문 빠져 나오기
                break;
            }
            // 발사간격만큼 기다린 후 다시 공격
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
            // 발사체 생성
            SpawnProjectile();
        }
    }

    private void SetAttackLaser(bool value)
    {
        // 레이저인 경우에만 해당
        if (weaponType != WeaponType.Laser) return;
        // 레이저 선 켜고 끄기
        lineRenderer.gameObject.SetActive(value);
        // 타격 효과 켜고 끄기
        hitEffect.gameObject.SetActive(value);
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 타격효과 활성화
        SetAttackLaser(true);

        while (true)
        {
            // 타겟을 공격하는게 가능한지 검사
            if (IsPossibleToAttackTarget() == false)
            {
                // 레이저, 레이저 타격효과 비활성화
                SetAttackLaser(false);
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 레이저 공격
            SpwanLaser();

            yield return null;
        }
    }

    private void SpwanLaser()
    {
        // 선의 시작 지점
        lineRenderer.SetPosition(0, spawnPoint.position);
        // 선의 목표 지점
        lineRenderer.SetPosition(1, new Vector3(attackTarget.position.x, attackTarget.position.y, 0) + Vector3.back);
        // 타격효과 위치 설정
        hitEffect.position = new Vector3(attackTarget.position.x, attackTarget.position.y - 0.2f, 0);
        // 적 체력 감소 (1초에 damage만큼 감소)
        attackTarget.GetComponent<Enemy>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
    }

    private void SpawnProjectile()
    {
        // 발사체프리펩에서 발사체 생성
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, 
            Quaternion.identity, transform);

        // 발사체에 공격목표 지정하면서 공격력도 전달
        clone.GetComponent<Projectile>().SetTarget(attackTarget, towerTemplate.weapon[level].damage);
    }

    /// <summary>
    /// 타워가 업그레이드 가능한지 검사하고 가능하면 업그레이드
    /// </summary>
    /// <returns>업그레이드 가능 여부</returns>
    public bool Upgrade()
    {
        // 가진 돈이 현재 레벨보다 1큰 비용보다 적은지 검사
        if (PlayerManager.instance.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            // 실패 리턴
            return false;
        }

        // 레벨 올리고
        level++;
        // 스프라이트 이미지도 바꾸고
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // 골드에서 건설 비용 차감하고
        PlayerManager.instance.CurrentGold -= towerTemplate.weapon[level].cost;
        // 무기 속성이 레이저면
        if (weaponType == WeaponType.Laser)
        {
            // 레벨에 따라 레이저의 굵기 결정
            lineRenderer.startWidth = 0.05f + level * 0f;
            lineRenderer.endWidth = 0.05f;
        }
        // 성공 리턴
        return true;
    }

    /// <summary>
    /// 타워 판매한 비용을 플레이어에 추가하고 타워 지우기
    /// </summary>
    public void Sell()
    {
        // 판매 비용 추가하고
        PlayerManager.instance.CurrentGold += towerTemplate.weapon[level].sell; 
        // 타워 오브젝트 지우고
        Destroy(gameObject);
    }
}
