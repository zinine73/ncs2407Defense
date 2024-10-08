using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 유니티에디터에서 사용하거나 파일로 저장하기 위해서 시리얼라이즈
[System.Serializable]
public struct Wave
{
    public bool         isRandom;       // 랜덤인지 아닌지
    public float        spawnTime;      // 적 생성 주기
    public int          maxEnemyCount;  // 적 최대 숫자
    public int          runMode;        // 런모드 발생 시기
    public GameObject[] enemyPrefabs;   // 적 종류
    public float[]      spawnTimeStatic;    // 고정일때 적 생성 주기
}

// JSON 사용을 위한 wrapper class
public class WaveWrapper
{
    public Wave[] waveArray;
}

public class WaveSystem : MonoBehaviour
{
    private string jsonFileName = "waves.json";
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

    // 에디터의 상단 메뉴가 아닌 WaveSystem이 붙어있는 콤포넌트에서 메뉴가 열린다
    [ContextMenu("DefenseTower/Make JSON data", false, 1)]
    public void MakeJSONData()
    {
        // 저장 데이터가 배열인 경우 wrapper class로 한번 감싸주자
        WaveWrapper data = new WaveWrapper();
        data.waveArray = waves;
        
        string jsonData = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.dataPath, jsonFileName);
        File.WriteAllText(path, jsonData);
        Debug.Log("Make JSON data is done.");
    }

    [ContextMenu("DefenseTower/Load from JSON", false, 2)]
    public void LoadFromJSON()
    {
        string path = Path.Combine(Application.dataPath, jsonFileName);
        string jsonData = File.ReadAllText(path);
        
        var json = JsonUtility.FromJson<WaveWrapper>(jsonData);
        waves = json.waveArray;
        Debug.Log("Loaded JSON data from file.");
    }
}
