using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 회원탈퇴 팝업 UI 처리                                                  
/// </summary>

public class WithdrawalPopUp : MonoBehaviour, IPopUp
{
    public int ClickCount = 0;
    [SerializeField] TextMeshProUGUI titleText = null; //제목 텍스트
    [SerializeField] TextMeshProUGUI contentText = null; //내용 텍스트
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

    /// <summary>
    /// 버튼 눌렀을때 행동 설정
    /// </summary>
    private void OnClickEventSetting()
    {
        yesBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Backend.BMember.WithdrawAccount();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        noBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Hide();
        });
    }

    /// <summary>
    /// 언어 세팅
    /// </summary>
    public void LanguageSetting()
    {
        contentText.text = LocalizationTable.Localization("Message_WithdrawAccountInfo");
        titleText.text = LocalizationTable.Localization("Title_Alert");

        LocalizationTable.languageSettings += LanguageSetting;
    }
    public void Close()
    {
        gameObject.SetActive(false);
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
