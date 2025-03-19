using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자 : 2024-07-30
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 게스트 로그인 워닝 팝업 관리
/// </summary>
public class GuestWarningPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI contentText = null;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
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
    private void OnClickEventSetting()
    {
        yesBtn.onClick.AddListener(() =>
        {
            Hide();
            SoundManager.Instance.PlayAudio("UIClick");
            LoginManager.Instance.GuestLogin();
        });
        noBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Hide();
        });
    }

    /// <summary>
    /// 언어세팅
    /// </summary>
    public void LanguageSetting()
    {
        contentText.text = LocalizationTable.Localization("Message_GuestAccountInfo");
        titleText.text = LocalizationTable.Localization("Title_Alert");

        LocalizationTable.languageSettings += LanguageSetting;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


}
