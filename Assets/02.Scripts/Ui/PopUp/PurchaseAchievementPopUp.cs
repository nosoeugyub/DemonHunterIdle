using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-09-02
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 과금업적 보상 팝업 (팝업 켜지고 꺼지는 것만 관리)
/// </summary>
public class PurchaseAchievementPopUp : MonoBehaviour, IPopUp
{
    [SerializeField]
    private TMP_Text titleText = null;
    private PurchaseAchievementSystem achievementSystem = null;
    private bool isInit = false;

    public int ClickCount = 0;
    
    public void Initialize()
    {
        titleText.text = LocalizationTable.Localization("Title_PurchaseAchievement");

        achievementSystem = GetComponent<PurchaseAchievementSystem>();
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
                Initialize();
                isInit = true;
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
