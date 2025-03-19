using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-09-25
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 방치모드 피로도 시스템의 UI를 제어
/// </summary>
public class IdleModeRestCycleUIView : MonoBehaviour
{
    [SerializeField]
    private Image restCycleBar;
    [SerializeField]
    private TextMeshProUGUI restCycleText;
    [SerializeField]
    private TextMeshProUGUI restCycleDebugTimeText;
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color restModeColor;

    public void SetTimeText(bool isRest)
    {
        if (restCycleDebugTimeText.gameObject.activeInHierarchy == false) 
            return;
        if(isRest)
        {
            restCycleDebugTimeText.text = Utill_Math.FormatCurrency((GameManager.Instance.IdleModeRestTime - GameDataTable.Instance.User.IdleModeRestTime));
        }
        else
        {
            restCycleDebugTimeText.text = Utill_Math.FormatCurrency(GameManager.Instance.IdleModeRestCycle - GameDataTable.Instance.User.IdleModeRestCycle);
        }
    }

    public void InitSetRestCycleUI()
    {
        restCycleText.text = LocalizationTable.Localization("IdleModeRestCycleInfo");
        restCycleDebugTimeText.gameObject.SetActive(false);
#if UNITY_EDITOR
        restCycleDebugTimeText.gameObject.SetActive(true);
#endif
    }

    public void SetRestCycleUI(bool isRestMode)
    {
        restCycleText.gameObject.SetActive(isRestMode);
        //휴식모드 여부에 따라 바 색상 바뀌게 하는 로직

        if(isRestMode) //휴식모드
        {
            restCycleBar.color = restModeColor;
            UpdateIdleModeRestTimeBar();
        }
        else
        {
            restCycleBar.color = normalColor;
            UpdateIdleModeRestCycleBar();
        }
    }

    /// <summary>
    /// 피로도를 기반으로 바 UI 갱신
    /// </summary>
    /// <returns>현재 바가 완전히 비었는지</returns>
    public bool UpdateIdleModeRestCycleBar()
    {
        float amount = 1 - ((float)GameDataTable.Instance.User.IdleModeRestCycle / (float)GameManager.Instance.IdleModeRestCycle);
        restCycleBar.fillAmount = amount;  // 슬라이더 바 업데이트
        return amount <= 0;
    }

    /// <summary>
    /// 회복 게이지 기반으로 바 UI 갱신
    /// </summary>
    /// <returns>현재 바가 가득 찼는지</returns>
    public bool UpdateIdleModeRestTimeBar()
    {
        float amount = (float)GameDataTable.Instance.User.IdleModeRestTime / (float)GameManager.Instance.IdleModeRestTime;
        restCycleBar.fillAmount = amount;  // 슬라이더 바 업데이트
        return amount >= 1;
    }
}
