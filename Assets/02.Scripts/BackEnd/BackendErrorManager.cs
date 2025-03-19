using BackEnd;
using BackEnd.Socketio;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-13
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 뒤끝에서 전달한 에러를 대응하는 매니저                                          
/// </summary>
public class BackendErrorManager : MonoSingleton<BackendErrorManager>
{
    [SerializeField]
    private CommonErrorPopUp commonErrorPopUpPrefab = null;

    [Header("에러팝업이 자동으로 종료되는 초")]
    [SerializeField]
    private float maintenanceAutoQuitSecond = 5f;

    private List<CommonErrorPopUp> commonErrorPopUpList = new();

    private int initialPoolSize = 10; // 초기 생성할 팝업 수
    private int currentAlertNum = 0; //현재 보여주고 있는 팝업 위치
    private bool isInit = false;

    private List<string> curErrorList = new List<string>();
    public List<string> ErrorList { get { return curErrorList; } }
    public  float MaintenanceAutoQuitSecond => maintenanceAutoQuitSecond;
    public void ErrorHandlerInit()
    {
        if (Backend.IsInitialized) //에러 헨들러 등록
        {
            Backend.ErrorHandler.InitializePoll(true);

            Backend.ErrorHandler.OnMaintenanceError = () => //점검에러
            {
                if (SceneManager.GetActiveScene().name.Contains("Login")) return;
                Game.Debbug.Debbuger.Debug("점검 에러 발생!!!");
                SettingPopUp("401", "BadUnauthorizedException", LocalizationTable.Localization("Message_Maintenance"), true);
            };
            Backend.ErrorHandler.OnTooManyRequestError = () => //단시간 내에 너무 많은 요청을 보낼경우 받는 에러
            {
                Game.Debbug.Debbuger.Debug("403 에러 발생!!!");

                SettingPopUp("403", "Forbidden", LocalizationTable.Localization("Message_TooManyRequest"), true);
            };
            Backend.ErrorHandler.OnTooManyRequestByLocalError = () => //위 에러를 받은 후 5분동안 요청을 보내면 받는 에러
            {
                Game.Debbug.Debbuger.Debug("403 로컬 에러 발생!!!");
                SettingPopUp("403", "Forbidden", LocalizationTable.Localization("Message_TooManyRequestByLocalError"), true);
            };
            Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () => //다른 기기에서 로그인 했을 시 받는 에러
            {
                Game.Debbug.Debbuger.Debug("리프레시 불가!!!");
                SettingPopUp("401", "BadUnauthorizedException", LocalizationTable.Localization("Message_OtherDeviceLogin"), true);
            };
            Backend.ErrorHandler.OnDeviceBlockError = () => //디바이스가 차단되었을때 받는 에러
            {
                Game.Debbug.Debbuger.Debug("디바이스 차단 발생");
                SettingPopUp("403", "ForbiddenException", LocalizationTable.Localization("Message_Blocked"), true);
            };
            Backend.Notification.OnServerStatusChanged = (ServerStatusType serverStatusType) => {
                Game.Debbug.Debbuger.Debug(
                    $"[OnServerStatusChanged(서버 상태 변경)]\n" +
                    $"| ServerStatusType : {serverStatusType}\n"

                );

                switch (serverStatusType)
                {
                    case ServerStatusType.Online:
                       
                        break;
                    case ServerStatusType.Offline: //오프라인일때
                        StreamingReader.SaveAllData(); //모두 저장
                        DataManager.Instance.LoadAllServerCsv(); //서버차트 로드
                        break;
                    case ServerStatusType.Maintenance:
                        SettingPopUp("401", "BadUnauthorizedException", LocalizationTable.Localization("Message_Maintenance"), true);
                        break;
                }
            };

            // 초기화 및 팝업 생성
            commonErrorPopUpList = new List<CommonErrorPopUp>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                CommonErrorPopUp popUp = Instantiate(commonErrorPopUpPrefab);
                popUp.transform.SetParent(transform);
                popUp.gameObject.transform.localScale = Vector3.one;
                popUp.gameObject.transform.localPosition = new Vector3(9999, 9999, 9999);
                commonErrorPopUpList.Add(popUp);
            }

            DontDestroyOnLoad(this); //메인씬에서도 작동 되야하기 때문..

            isInit = true;
        }
        
    }

    void Update()
    {
        if (isInit)
        {
            Backend.ErrorHandler.Poll();

            for(int i = 0;i< commonErrorPopUpList.Count;i++)
            {
                if (commonErrorPopUpList[i].CanInitUI)
                {
                    commonErrorPopUpList[i].UpdateUI();
                }
            }
        }
    }

    /// <summary>
    /// 뒤끝 함수를 사용할 시 반환되는 BackendReturnObject의 정보에 에러가 있는지 검사하고, 에러에 대해 대응하는 함수.
    /// </summary>
    /// <param name="func">재시도 진행 할 시 사용할 로직(사용한 뒤끝 함수랑 동일하게)</param>
    /// <param name="ignoreOtherError">반드시 멈춰야하는 오류를 제외한 나머지 오류는 무시할 것인지 (SettingPopUp으로 차후 나머지 오류 대응 필수)</param>
    /// <param name="asyncReturnObj">만약 비동기 함수를 사용했다면 생긴 콜백을 넣는 곳</param>
    /// <returns>재시도까지 거친 BackendReturnObject 반환</returns>
    public BackendReturnObject RetryLogic(Func<BackendReturnObject> func, bool ignoreOtherError = false, BackendReturnObject asyncReturnObj = null)
    {
        BackendReturnObject bro = null;

        if (asyncReturnObj != null) //만약 비동기 함수를 사용하여 이미 BackendReturnObject가 제작되었을 시
            bro = asyncReturnObj;
        else
            bro = func(); //아니라면 함수를 사용하여 값을 넣어줌

        for (int i = 0; i < 2; i++) //재시도까지 진행해줌
        {
            Backend_Result_Type type = CheckError(bro,ignoreOtherError);

            if (type == Backend_Result_Type.ReTry) //재시도 해야한다면
            {
                bro = func(); //함수 실행
                continue;
            }
            return bro;
        }
        return bro;
    }

    /// <summary>
    /// BackendReturnObject를 검사하여 여러 공통 에러를 대응하고, 에러 팝업을 띄워야 한다면 띄워주는 함수
    /// </summary>
    /// <param name="returnObj">검사할 BackendReturnObject</param>
    /// <param name="ignoreOtherError">주요 에러 제외하고 다른 에러들을 무시할지 여부</param>
    /// <returns></returns>
    public Backend_Result_Type CheckError(BackendReturnObject returnObj,bool ignoreOtherError)
    {
        if (returnObj.IsSuccess())
        {
            return Backend_Result_Type.True;
        }
        else
        {
            if (returnObj.IsClientRequestFailError()) // 클라이언트의 일시적인 네트워크 끊김 시
            {
                return Backend_Result_Type.ReTry;
            }
            else if (returnObj.IsServerError()) // 서버의 이상 발생 시
            {
                return Backend_Result_Type.ReTry;
            }
            else if (returnObj.IsBadAccessTokenError())
            {
                bool isRefreshSuccess = RefreshTheBackendToken(3); // 최대 3번 리프레시 실행

                if (isRefreshSuccess)
                {
                    return Backend_Result_Type.ReTry;
                }
                else
                {
                }
            }
            else if(returnObj.IsDeviceBlockError())
            {
                Game.Debbug.Debbuger.Debug("디바이스 차단 발생");
                SettingPopUp("403", "ForbiddenException", LocalizationTable.Localization("Message_Blocked"), true);
                return Backend_Result_Type.False;
            }
            else if (returnObj.IsTooManyRequestError())
            {
                Game.Debbug.Debbuger.Debug("403 에러 발생!!!");
                if(returnObj.GetMessage().Contains("Local"))
                    SettingPopUp("403", "Forbidden", LocalizationTable.Localization("Message_TooManyRequestByLocalError"));
                else
                    SettingPopUp("403", "Forbidden", LocalizationTable.Localization("Message_TooManyRequest"));
                return Backend_Result_Type.False;
            }
            //데이터베이스 할당량 초과
            else if((returnObj.GetStatusCode()=="429")&&returnObj.GetMessage().Contains("ProvisionThroughput"))
            {
                SettingPopUp(returnObj,true);
                return Backend_Result_Type.False;
            }
            //타임아웃 오류
            else if ((returnObj.GetStatusCode() == "408") && returnObj.GetMessage().Contains("timeout"))
            {
                SettingPopUp(returnObj,true);
                return Backend_Result_Type.False;
            }
            //디바이스 정보를 찾지 못했을 경우
            else if ((returnObj.GetErrorCode() == "UndefinedParameterException") && returnObj.GetMessage().Contains("undefined device_unique_id"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            //안드로이드 OS 환경에서 Client(게임)와 Server(뒤끝 콘솔) 간 구글 해시키가 일치하지 않는 경우
            else if ((returnObj.GetErrorCode() == "BadUnauthorizedException") && returnObj.GetMessage().Contains("bad google_hash"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            //Client(게임)와 Server(뒤끝 콘솔) 간 시그니처가 일치하지 않는 경우
            else if ((returnObj.GetErrorCode() == "BadUnauthorizedException") && returnObj.GetMessage().Contains("bad signature"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            //서버와 클라이언트의 시간이 UTC+9(한국시간) 기준 10분 이상 차이가 나는 경우
            else if ((returnObj.GetErrorCode() == "BadUnauthorizedException") && returnObj.GetMessage().Contains("bad client_date"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            //뒤끝 콘솔에서 서버 설정이 테스트 모드인데 10명을 초과하는 계정의 회원가입/로그인 시도를 한 경우
            else if ((returnObj.GetErrorCode() == "ForbiddenException") && returnObj.GetMessage().Contains("Forbidden Active User"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            //개발모드에서 제공량을 모두 소진했을 경우
            else if ((returnObj.GetErrorCode() == "FailedDependency") && returnObj.GetMessage().Contains("Dependency"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            //Access Token이 유효하지 않는 경우 or 유저의 Access Token이 올바르지 않거나 만료된 경우(로그인 후 하루 이상 경과한 경우)
            else if ((returnObj.GetErrorCode() == "BadUnauthorizedException") && returnObj.GetMessage().Contains("bad bad,accessToken"))
            {
                SettingPopUp(returnObj, true);
                return Backend_Result_Type.False;
            }
            if (!ignoreOtherError)
                SettingPopUp(returnObj);
            return Backend_Result_Type.False;
        }
    }

    /// <summary>
    /// 토큰 리프레시 함수
    /// </summary>
    /// <param name="maxRepeatCount">최대 반복실행 횟수</param>
    /// <returns>토큰이 정상적으로 생성되었는지 여부</returns>
    bool RefreshTheBackendToken(int maxRepeatCount)
    {
        if (maxRepeatCount <= 0)
        {
            return false;
        }
        var callback = Backend.BMember.RefreshTheBackendToken();
        if (callback.IsSuccess())
        {
            return true;
        }
        else
        {
            if (callback.IsClientRequestFailError()) // 클라이언트의 일시적인 네트워크 끊김 시
            {
                return RefreshTheBackendToken(maxRepeatCount - 1);
            }
            else if (callback.IsServerError()) // 서버의 이상 발생 시
            {
                return RefreshTheBackendToken(maxRepeatCount - 1);
            }
            else if (callback.IsMaintenanceError()) // 서버 상태가 '점검'일 시
            {
                //점검 팝업창 + 로그인 화면으로 보내기
                return false;
            }
            else if (callback.IsTooManyRequestError()) // 단기간에 많은 요청을 보낼 경우 발생하는 403 Forbbiden 발생 시
            {
                //너무 많은 요청을 보내는 중
                return false;
            }
            else
            {
                //재시도를 해도 액세스토큰 재발급이 불가능한 경우
                //커스텀 로그인 혹은 페데레이션 로그인을 통해 수동 로그인을 진행해야합니다.  
                //중복 로그인일 경우 401 bad refreshToken 에러와 함께 발생할 수 있습니다.  
                //Debug.Log("게임 접속에 문제가 발생했습니다. 로그인 화면으로 돌아갑니다\n" + callback.ToString());
                return false;
            }
        }
    }
    public CommonErrorPopUp GetErrorPopUp()
    {
        CommonErrorPopUp popUp = commonErrorPopUpList[currentAlertNum++];
        if(currentAlertNum == commonErrorPopUpList.Count)
            currentAlertNum = 0;
        return popUp;
    }

    public void ReturnErrorPopUp(CommonErrorPopUp popUp)
    {
        curErrorList.Remove(popUp.ErrorType);
        currentAlertNum--;
        popUp.transform.localPosition = new Vector3(9999, 9999, 9999); // 팝업을 비활성화
    }

    /// <summary>
    /// 에러 팝업을 띄우는 함수, 매개변수는 CommonErrorPopUp Setting확인 바람
    /// </summary>
    public CommonErrorPopUp SettingPopUp(BackendReturnObject stateObj, bool needQuitGame = false)
    {
        if (curErrorList.Contains(stateObj.GetMessage()))
        {
            return null;
        }
        curErrorList.Add(stateObj.GetMessage());

        var popup = GetErrorPopUp();
        popup.Setting(stateObj,needQuitGame);
        return popup;
    }
    /// <summary>
    /// 에러 팝업을 띄우는 함수, 매개변수는 CommonErrorPopUp Setting확인 바람
    /// </summary>
    public CommonErrorPopUp SettingPopUp(string statusCode, string errorCode, string message, bool needQuitGame = false)
    {
        if (curErrorList.Contains(message))
        {
            return null;
        }
        curErrorList.Add(message);
        var popup = GetErrorPopUp();
        popup.Setting(statusCode,errorCode,message,needQuitGame);
        return popup;
    }
    /// <summary>
    /// 에러 팝업을 띄우는 함수, 매개변수는 CommonErrorPopUp Setting확인 바람
    /// </summary>
    public CommonErrorPopUp SettingPopUp(string content, bool needQuitGame = false)
    {
        if(curErrorList.Contains(content))
        {
            return null;
        }
        curErrorList.Add(content);

        var popup = GetErrorPopUp();
        popup.Setting(content, needQuitGame);
        return popup;
    }
}