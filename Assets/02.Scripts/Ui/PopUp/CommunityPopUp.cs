using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자 : 2024-05-28
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 커뮤니티 팝업 UI 관리
/// </summary>
public class CommunityPopUp : MonoBehaviour, IPopUp
{
    [Header("Image & Text")]
    [SerializeField] private TextMeshProUGUI titleText = null; 

    [Header("Button")]
    [SerializeField] private Button cafeBtn = null; 
    [SerializeField] private Button kakaoBtn = null;

    [SerializeField] private string cafeUrl = null; // 네이버 카페 링크
    [SerializeField] private string kakaoUrl = null; //카카오톡 링크
    
    private bool isInit = false;

    public int ClickCount = 0;

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
        OnClickEventSetting();
        isInit = true;
    }

    /// <summary>
    /// 언어세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Community");

        cafeBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Cafe");
        kakaoBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Kakao");
        LocalizationTable.languageSettings += LanguageSetting;
    }

    /// <summary>
    /// 버튼 눌렀을때 기능 설정
    /// </summary>
    private void OnClickEventSetting()
    {
        cafeBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Application.OpenURL(cafeUrl);
        });
        kakaoBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Application.OpenURL(kakaoUrl);
        });

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
