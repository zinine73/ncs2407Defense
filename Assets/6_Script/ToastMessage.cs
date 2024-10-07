using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ToastType // 메시지 종류 설정
{
    Money,  // 골드 모자랄 때
    Build   // 건설 불가능할 때
}

public class ToastMessage : MonoBehaviour
{
    private TextMeshProUGUI toastMsg;
    private TMPAlpha tmpAlpha;

    private void Start()
    {
        toastMsg = GetComponent<TextMeshProUGUI>();
        tmpAlpha = GetComponent<TMPAlpha>();    
    }

    /// <summary>
    /// type에 따른 토스트메시지 출력
    /// </summary>
    /// <param name="type">메시지 종류</param>
    public void ShowToast(ToastType type)
    {
        switch (type)
        {
            case ToastType.Money:
                toastMsg.text = "Not enough money";
                break;

            case ToastType.Build:
                toastMsg.text = "Invalid build tower";
                break;
        }
        tmpAlpha.FadeOut();
    }
}
