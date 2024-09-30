using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유니티에디터에서 사용하거나 파일로 저장하기 위해서 시리얼라이즈
[System.Serializable]
public struct Wave
{
    public float        spawnTime;      // 적 생성 주기
    public int          maxEnemyCount;  // 적 최대 숫자
    public GameObject[] enemyPrefabs;   // 적 종류
}

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves; // 웨이브 배열
    private int currentWaveIndex = -1; // 현재 웨이브 인덱스 (0에서 시작해야 해서 초기값은 -1)

    /// <summary>
    /// 현재 인덱스에 해당하는 웨이브 실행
    /// </summary>
    public void StartWave()
    {
        // 적이 없고 웨이브가 남아 있다면 가능
        if ((EnemyManager.instance.EnemyList.Count == 0) && (currentWaveIndex < waves.Length - 1))
        {
            // 웨이브 인덱스를 하나 증가
            currentWaveIndex++;
            // 현재 인덱스에 해당하는 웨이브를 실행
            EnemyManager.instance.StartWave(waves[currentWaveIndex]);
        }
    }

    /// <summary>
    /// [ 현재 웨이브 / 총 웨이브 ] 문자열 얻어오기
    /// </summary>
    /// <returns></returns>
    public string GetWaveInfoString()
    {
        return $"{currentWaveIndex + 1}\n---\n{waves.Length}";
    }
}
