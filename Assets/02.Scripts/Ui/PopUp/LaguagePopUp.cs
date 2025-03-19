using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 작성일자 : 2024-07-30
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 언어 변경 팝업 UI 관리
/// </summary>
public class LaguagePopUp : MonoBehaviour, IPopUp
{
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] Button koreaBtn = null;
    [SerializeField] Button englishBtn = null;
    private bool isInit = false;
    private void Awake()
    {
        if (!isInit)
        {
            Initialize();
        }
    }
    public void Initialize()
    {
        LanguageSetting();
        OnClickEventSetting();
        isInit = true;
    }

    /// <summary>
    /// 버튼 기능 세팅
    /// </summary>
    private void OnClickEventSetting()
    {
        koreaBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            LocalizationTable.SetLanguage(ELanguage.KR); 
        });
        englishBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            LocalizationTable.SetLanguage(ELanguage.EN);
        });

    }

    /// <summary>
    /// 언어세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Language");
        koreaBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Korea");
        englishBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_English");
        LocalizationTable.languageSettings += LanguageSetting;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public int ClickCount = 0;
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
