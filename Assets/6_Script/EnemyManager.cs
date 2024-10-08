using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; // 싱글톤 인스턴스
    [SerializeField] private GameObject enemyHPSliderPrefab; // 체력을 나타내는 프리펩
    [SerializeField] private Transform canvasTransform; // UI를 표시할 캔버스의 tf
    [SerializeField] private Transform[] waypoints; // 이동 위치 배열
    private Wave currentWave; // 현재 웨이브 정보
    private int currentEnemyCount; // 현재 남은 적 수
    private List<Enemy> enemyList; // 생성된 적 리스트
    private bool isRunMode; // 런모드 발동 여부

    #region Property
    public List<Enemy> EnemyList => enemyList; // 생성된 적 리스트 프로퍼티
    public Transform[] Waypoints => waypoints; // 이동위치배열 프로퍼티
    public int CurrentEnemyCount => currentEnemyCount; // 현재 남은 저 수 프로퍼티
    public int MaxEnemyCount => currentWave.maxEnemyCount; // 현재 웨이브 적 수
    #endregion Property
    
    private void Awake()
    {
        if (instance == null) instance = this; // 싱글톤 인스턴스 연결
    }

    private void Start()
    {
        // 생성된 적 리스트 초기화
        enemyList = new List<Enemy>();
    }

    /// <summary>
    /// 웨이브 시작
    /// </summary>
    /// <param name="wave">웨이브 정보</param>
    public void StartWave(Wave wave)
    {
        // 현재 웨이브 정보 전달
        currentWave = wave;
        // 현재 웨이브 최대 적 수를 현재 남은 적 수로 지정
        currentEnemyCount = currentWave.maxEnemyCount;
        // 런모드는 아닌걸로 시작
        isRunMode = false;
        // 코루틴 실행
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        // 생성한 적 숫자
        int spawnEnemyCount = 0;
        // 웨이브 정보에 있는 최대 생성 숫자에 도달할 때까지
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            int enemyIndex;
            if (currentWave.isRandom)
            {
                // 웨이브 정보에 있는 적 종류 중 랜덤으로 생성
                enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            }
            else
            {
                // 고정인 경우 카운트가 곧 인덱스
                enemyIndex = spawnEnemyCount;
            }
            // 적 프레팝으로 오브젝트를 생성하고 Enemy 스크립트 연결
            Enemy enemy = Instantiate(currentWave.enemyPrefabs[enemyIndex], transform).GetComponent<Enemy>();
            // 적 초기화
            enemy.Init();
            // 적을 리스트에 넣기
            enemyList.Add(enemy);
            // 적 체력 슬라이드 표시
            SpawnEnemyHPSlider(enemy);
            // 생성한 적 숫자 증가
            spawnEnemyCount++;
            // 런모드가 발동된 상태면
            if (isRunMode)
            {
                // 새로 생성되는 적에게 바로 런모드 적용
                enemy.StartRunMode();
            }
            // 생성시간 기다렸다 다음 적 생성
            float waitTime = currentWave.isRandom ? currentWave.spawnTime : currentWave.spawnTimeStatic[enemyIndex];
            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 적 삭제 처리
    /// </summary>
    /// <param name="enemy">삭제해야할 오브젝트</param>
    /// <param name="gold">goal도착이 아닌 경우 추가할 골드</param>
    /// <param name="isArrivedGoal">goal 도착 여부</param>
    public void DestroyEnemy(Enemy enemy, int gold, bool isArrivedGoal)
    {
        // goal에 도착한 것이냐
        if (isArrivedGoal)
        {
            // 플레이어에게 데미지
            PlayerManager.instance.TakeDamage(1);
        }
        else // 아니면
        {
            // 골드 증가
            PlayerManager.instance.CurrentGold += gold;
        }
        // 현재 적 수에서 하나 감소
        currentEnemyCount--;
        // 웨이브 정보에 있는 런모드 발동 조건 검사
        if (currentWave.runMode == currentEnemyCount)
        {
            // 런모드 발동
            isRunMode = true;
            // 생성된 적에게 런모드 적용
            foreach (var item in enemyList)
            {
                item.StartRunMode();
            }
        }
        // 적 리스트에서 지정한 적 지우기
        enemyList.Remove(enemy);
        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(Enemy enemy)
    {
        // 슬라이더 클론 생성
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab, canvasTransform);
        // 크기 지정
        sliderClone.transform.localScale = Vector3.one;
        // 위치 지정
        sliderClone.GetComponent<SliderPosAuto>().Setup(enemy.transform);
        // 값 지정
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy);
    }
}
