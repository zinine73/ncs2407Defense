using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP; // 유저 체력
    [SerializeField] private TextMeshProUGUI textGold; // 골드
    
    private void Update()
    {
        // 체력 표시
        textPlayerHP.text = $"{PlayerManager.instance.CurrentHP}/{PlayerManager.instance.MaxHP}";
        // 골드 표시
        textGold.text = $"{PlayerManager.instance.CurrentGold}";
    }
}
