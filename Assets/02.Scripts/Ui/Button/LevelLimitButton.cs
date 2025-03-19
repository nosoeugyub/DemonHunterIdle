using NSY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-10-11
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 레벨 제한 버튼
/// </summary>
public class LevelLimitButton : MonoBehaviour
{
    [SerializeField]
    private Button levelLimitButton;
    [SerializeField]
    private TextMeshProUGUI levelLimitText;

    private void Awake()
    {
        levelLimitButton.onClick.AddListener(OnLevelLimitClick);
    }

    /// <summary>
    /// 레벨제한 버튼 세팅 (버튼이 보여질 때마다 호출되어야함)
    /// </summary>
    /// <param name="subClass">레벨제한 체크할 헌터</param>
    /// <param name="limitLevel">레벨 제한 (동렙일시 레벨제한 풀림)</param>
    public void SettingLimitLevelButton(SubClass subClass,int limitLevel)
    {
        int level = GameDataTable.Instance.User.HunterLevel[(int)subClass];
        
        if(limitLevel > level ) //레벨제한 걸림
        {
            levelLimitButton.gameObject.SetActive(true);
            levelLimitText.text = string.Format(LocalizationTable.Localization("Button_LevelLimit"),limitLevel);
        }
        else
            levelLimitButton.gameObject.SetActive(false);
    }

    private void OnLevelLimitClick()
    {
        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_LevelLimit"));
    }
}
