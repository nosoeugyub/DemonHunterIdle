using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-06-13
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 에러 상황을 안내하는 팝업                                      
/// </summary>
public class CommonErrorPopUp : MonoBehaviour
{
    private const string ErrorLogFormat = "statusCode:{0}\nerrorCode:{1}\n{2}";
    [SerializeField]
    private TMP_Text errorText;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text confirmButtonText;

    private string errorStr = string.Empty;

    bool canInitUI = false;
    bool needQuitGame = false;

    public bool CanInitUI => canInitUI;
    public string ErrorType { get; private set; }
    private void Start()
    {
        LocalizationTable.languageSettings += LanguageSetting;
    }

    public void UpdateUI()
    {
        if(canInitUI)
        {
            errorText.text = errorStr;
            SettingCommonUI(needQuitGame);
            canInitUI = false;
        }
    }

    public void Close()
    {
        if (SceneManager.GetActiveScene().name.Contains("Login") == false) //로그인 중이 아니면
            StreamingReader.SaveAllData(); //저장
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void Hide()
    {
        BackendErrorManager.Instance.ReturnErrorPopUp(this);
    }

    public void Show()
    {
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
    }

    public IEnumerator AutoQuitGame()
    {
        yield return new WaitForSeconds(BackendErrorManager.Instance.MaintenanceAutoQuitSecond);
        Close();
    }

    /// <summary>
    /// 언어 세팅 함수
    /// </summary>
    private void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Alert");

        confirmButtonText.text = LocalizationTable.Localization("Button_Check");
    }
    /// <summary>
    /// 에러 팝업 세팅
    /// </summary>
    /// <param name="stateObj">에러를 표시할 BackendReturnObject</param>
    /// <param name="needQuitGame">클릭시 true시 게임 종료, false 시 창만 닫힘</param>
    /// <returns> BackendReturnObject가 정상적이었다면 false 반환</returns>
    public bool Setting(BackendReturnObject stateObj, bool needQuitGame = false)
    {
        if (stateObj.IsSuccess())
        {
            Hide();
            return false;
        }
        //핸들러로 위 함수에 들어오는 경우 게임 오브젝트 관련 함수가 작동하지 않음.. 그렇기에 canInitUI라는 불값으로 오브젝트가 켜지고 꺼지는 것을 제어함
        //Show();
        this.needQuitGame = needQuitGame;
        errorStr = string.Format(ErrorLogFormat, stateObj.GetStatusCode(), stateObj.GetErrorCode(), SetErrorLocalization(stateObj.GetMessage()));
        ErrorType = stateObj.GetMessage();
        canInitUI = true;
        Game.Debbug.Debbuger.Debug(errorStr);
        return true;
    }

    /// <summary>
    ///  BackendReturnObject없이 에러 팝업 생성 (포멧 적용)
    ///  포멧은 ErrorLogFormat참고
    /// </summary>
    /// <param name="statusCode">맨 처음 나올 코드(숫자)</param>
    /// <param name="errorCode">두번쨰로 표시될 에러 코드(영자)</param>
    /// <param name="message">에러 코드의 자세한 내용(문자)</param>
    /// <param name="needQuitGame">클릭시 true시 게임 종료, false 시 창만 닫힘</param>
    /// <returns>무조건 true 반환</returns>
    public bool Setting(string statusCode, string errorCode, string message, bool needQuitGame = false)
    {
        this.needQuitGame = needQuitGame;
        //핸들러로 위 함수에 들어오는 경우 게임 오브젝트 관련 함수가 작동하지 않음.. 그렇기에 canInitUI라는 불값으로 오브젝트가 켜지고 꺼지는 것을 제어함
        //Show();
        errorStr = string.Format(ErrorLogFormat, statusCode, errorCode, SetErrorLocalization(message));
        canInitUI = true;
        ErrorType = message;
        Game.Debbug.Debbuger.Debug(errorStr);
        return true;
    }

    /// <summary>
    /// string으로 팝업 생성 (포멧 적용 x)
    /// </summary>
    /// <param name="content">에러 팝업의 내용 (포멧 적용 x)</param>
    /// <param name="needQuitGame">클릭시 true시 게임 종료, false 시 창만 닫힘</param>
    /// <returns>무조건 true 반환</returns>
    public bool Setting(string content,bool needQuitGame = false)
    { 
        //핸들러로 위 함수에 들어오는 경우 게임 오브젝트 관련 함수가 작동하지 않음.. 그렇기에 canInitUI라는 불값으로 오브젝트가 켜지고 꺼지는 것을 제어함
        //Show();
        this.needQuitGame = needQuitGame;
        errorStr = SetErrorLocalization(content);
        ErrorType = content;
        canInitUI = true;
        Game.Debbug.Debbuger.Debug(errorStr);
        return true;
    }

    /// <summary>
    /// 공통적인 UI를 세팅하는 함수
    /// </summary>
    /// <param name="needQuitGame"></param>
    private void SettingCommonUI(bool needQuitGame)
    {
        LanguageSetting();
        Show();
        confirmButton.onClick.RemoveAllListeners();
        if (needQuitGame)
        {
            confirmButton.onClick.AddListener(Close);
            if(errorStr.Contains(LocalizationTable.Localization("Message_Maintenance")))
                StartCoroutine(AutoQuitGame());
        }
        else
            confirmButton.onClick.AddListener(Hide);
    }
    /// <summary>
    /// 필요하다면 에러 텍스트를 Localization 시킴
    /// </summary>
    /// <param name="msg">검사할 string</param>
    /// <returns>필요에 따라 Localization시킨 string 반환</returns>
    private string SetErrorLocalization(string msg)
    {
        if (msg.Contains("blocked user"))
        {
            msg = LocalizationTable.Localization("Message_Blocked");
            needQuitGame = true;
        }

        if (msg.Contains("Message_VersionUpdate"))
        {
            msg = LocalizationTable.Localization("Message_UpdateVersion");
        }

        if(msg.Contains("Precondition Required"))
        {
            msg = LocalizationTable.Localization("Message_Ranking");
        }

        return msg;
    }
}
