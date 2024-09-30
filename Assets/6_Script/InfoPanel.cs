using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP; // 유저 체력
    [SerializeField] private TextMeshProUGUI textGold; // 골드
    [SerializeField] private TextMeshProUGUI textWave; // 웨이브
    [SerializeField] private TextMeshProUGUI textEnemyCount; // 적 수
    [SerializeField] private WaveSystem waveSystem; //웨이브 시스템

    private void Update()
    {
        // 체력 표시
        textPlayerHP.text = $"{PlayerManager.instance.CurrentHP}/{PlayerManager.instance.MaxHP}";
        // 골드 표시
        textGold.text = $"{PlayerManager.instance.CurrentGold}";
        // 웨이브 표시
        textWave.text = waveSystem.GetWaveInfoString();
        // 적 수 표시
        textEnemyCount.text = $"{EnemyManager.instance.CurrentEnemyCount} / {EnemyManager.instance.MaxEnemyCount}";
    }
}
