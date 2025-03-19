using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-10
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 헌터 구매에 관련된 UI 처리 (지금은 구매 버튼 관련 처리만 하는중)                                           
/// </summary>
public class HunterPurchaseView : MonoBehaviour
{
    public Button HunterBuyBtn;
    private void Awake()
    {
        LanguageSetting();
    }
    private void OnEnable()
    {
        SetUIHunterBuy();
    }

    /// <summary>
    /// 언어세팅
    /// </summary>
    public void LanguageSetting()
    {
        HunterBuyBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Buuton_Buy");
        LocalizationTable.languageSettings += LanguageSetting;
    }

    /// <summary>
    /// 헌터 구매 버튼 UI 처리
    /// </summary>
    public void SetUIHunterBuy()
    {
        if (HunterPurchaseSystem.Instance.IsHunterPurchase())
        {
            HunterBuyBtn.GetComponent<Basic_Prefab>().SetTypeButton(Utill_Enum.ButtonType.DeActive);
        }
        else
        {
            HunterBuyBtn.GetComponent<Basic_Prefab>().SetTypeButton(Utill_Enum.ButtonType.Active);
        }

        if (GameDataTable.Instance.User.currentHunter == 0)
        {
            HunterBuyBtn.gameObject.SetActive(false);
        }
        else
        {
            HunterBuyBtn.gameObject.SetActive(true);
        }
    }
}
