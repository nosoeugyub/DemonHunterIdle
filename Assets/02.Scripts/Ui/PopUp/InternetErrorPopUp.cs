using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                             
/// 클래스용도 : 인터넷 에러 팝업 UI 관리            
/// </summary>
public class InternetErrorPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] private Button quitBtn; 
    [SerializeField] private Button retryBtn; //재시도 버튼
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
 
    private bool isInit = false;
    private void Start()
    {
        if (!isInit)
            Initialize();
    }
    public void Initialize()
    {
        LanguageSetting();
        OnClickEventSetting();
        isInit = true;
    }

    /// <summary>
    /// 언어 세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Alert");
        contentText.text = LocalizationTable.Localization("Message_CheckInternet");

        quitBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Exit");
        retryBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Retry");

        LocalizationTable.languageSettings += LanguageSetting;
    }
    private void OnClickEventSetting()
    {
        quitBtn.onClick.AddListener(Quit);
        retryBtn.onClick.AddListener(Retry);
    }

    /// <summary>
    /// 뒤끝에 서버 연결되어있는지 콜백으로 확인 후 성공시 재연결
    /// </summary>
    public void Retry()
    {
        BackEnd.Backend.Utils.GetServerStatus((callback) =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("재연결 성공");
                StreamingReader.SaveAllData();
                Hide();
            }
            else
            {
                Debug.Log("재연결 실패");
                if (callback.GetErrorCode() == "HttpRequestException")
                    Show();
            }
        });

        BackendManager.Instance.IsLocal = false;
    }
    public void Quit()
    {
        StreamingReader.SaveAllData();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
    }

}
