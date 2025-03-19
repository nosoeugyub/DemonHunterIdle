using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-09-12
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 가챠업적 보상 팝업 (팝업 켜지고 꺼지는 것만 관리)
/// </summary>
public class GachaAchievementPopUp : MonoBehaviour, IPopUp
{
    [SerializeField]
    private TMP_Text titleText = null;
    private GachaAchievementSystem achievementSystem = null;
    private bool isInit = false;

    public int ClickCount = 0;
    
    public void Initialize()
    {
        titleText.text = LocalizationTable.Localization("Title_GachaAchievement");

        achievementSystem = GetComponent<GachaAchievementSystem>();
        achievementSystem.InitSliders();
    }
    public void Close()
    {
    }

    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove();

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            //뒷배경도 ON.
            PopUpSystem.Instance.EscPopupListAdd(this);

            if (!isInit)
            {
                isInit = true;
                Initialize();
            }
            else
            {
                achievementSystem.RefrashSliders();
                achievementSystem.scrollRect.horizontalNormalizedPosition = 0f;
            }

            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
