using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnTime; // 생성 시간
    [SerializeField] private Transform[] waypoints; // 이동 위치 배열
    private List<Enemy> enemyList; // 생성된 적 리스트

    public List<Enemy> EnemyList => enemyList; // 생성된 적 리스트 프로퍼티
    public Transform[] Waypoints => waypoints; // 이동위치배열 프로퍼티

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start는 코루틴으로 사용할 수 있다
    IEnumerator Start()
    {
        // 생성된 적 리스트 초기화
        enemyList = new List<Enemy>();
        
        while (true)
        {
            // 적 프레팝으로 오브젝트를 생성하고 Enemy 스크립트 연결
            Enemy enemy = Instantiate(enemyPrefab, transform).GetComponent<Enemy>();
            // 적 초기화
            enemy.Init();
            // 적을 리스트에 넣기
            enemyList.Add(enemy);
            // 생성시간 기다렸다 다음 적 생성
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        // 적 리스트에서 지정한 적 지우기
        enemyList.Remove(enemy);
        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }
}
