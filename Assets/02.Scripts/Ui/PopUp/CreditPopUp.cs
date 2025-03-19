using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자 : 2024-05-28
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 만든이 팝업
/// </summary>
public class CreditPopUp : MonoBehaviour, IPopUp
{
    [Header("Image & Text")]
    [SerializeField] private TextMeshProUGUI titleText = null;
    
    public int ClickCount = 0;
    private bool isInit = false;

    private void Start()
    {
        if (!isInit)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        LanguageSetting();
        isInit = true;
    }

    /// <summary>
    /// 언어 세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Credit");
        LocalizationTable.languageSettings += LanguageSetting;

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
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
