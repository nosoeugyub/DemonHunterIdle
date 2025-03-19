using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자 : 2024-07-30
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 게임 종료 팝업 UI 관리
/// </summary>
public class ExitPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private Button forceQuitBtn; //강제종료 버튼

    public int ClickCount = 0;

    public void Start()
    {
        yesBtn.onClick.AddListener(Quit);
        noBtn.onClick.AddListener(() => Close());

        //에디터 환경에서만 강제종료 버튼 보이게 설정
#if UNITY_EDITOR
        forceQuitBtn.gameObject.SetActive(true); 
#else
        forceQuitBtn.gameObject.SetActive(false);
#endif
        LanguageSetting();
    }
    public void Quit()
    {
        // 애플리케이션 종료
        Application.Quit();

        // Unity 에디터에서 실행 중인 경우 플레이 모드 중지
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// 언어세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Exit");
        contentText.text = LocalizationTable.Localization("Message_Exit");
        
        yesBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Exit_Yes");
        noBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Exit_No");
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
            //뒷배경도 ON.
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
