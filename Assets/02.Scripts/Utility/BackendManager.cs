using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 튀끝 관련 함수를 가져오는곳
/// </summary>
public class BackendManager : MonoSingleton<BackendManager>
{
    public static string myNickname => Backend.UserNickName;
    public static string myInDate => Backend.UserInDate;

    bool isConnectNetwork;
    public bool CheckNetwork => isConnectNetwork;
    
    public bool IsLocal;
    public bool IsEditorUser = false;

    string ownerIndate = null;
    public string OwnerIndate { get => ownerIndate; set{ ownerIndate = value; } }

    private Coroutine checkInternetCoroutine;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private new void OnDestroy()
    {
        if (checkInternetCoroutine != null)
        {
            StopCoroutine(checkInternetCoroutine);
        }
    }
    private void Start()
    {
        checkInternetCoroutine = null;
        //checkInternetCoroutine = StartCoroutine(CheckInternetConnectionCoroutine());
        InvokeRepeating("CheckInternetConnection", 1.0f, 1f );   // n~n초 간격으로 인터넷연결 체크  
    }

    IEnumerator CheckInternetConnectionCoroutine()
    {
        while (true)
        {
            // HTTPS URL을 사용하여 네트워크 요청
            UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");
            request.timeout = 5; // 5초 타임아웃 설정
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                // 네트워크 재시도 팝업 엑티브
               UIManager.Instance.internetErrorPopUp.Show();
                Debug.Log("네트워크 연결 이상 팝업 on");
            }
            else
            {
               // Debug.Log("네트워크 연결 이상 무");

                // 네트워크 연결 정상
            }

            yield return new WaitForSeconds(1f); 
        }
    }


    void CheckInternetConnection()
    {
        if (!IsNetworkAvailable())
        {
            //  네트워크 재시도 팝업 엑티브
            Debug.Log("네트워크 재시도");
            UIManager.Instance.internetErrorPopUp.Show();
        }
        else
        {
        }
    }



    #region 인터넷 체크

    public static bool IsNetworkAvailable()     //	네트워크 가능여부 체크 
    {
        return (getNetworkStatus() != 0);
    }

    public static int getNetworkStatus()        //	네트워크 상태 체크 
    {
        int status = 0;

        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:      //	wifi
                status = 1;
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:    //	3g or lte
                status = 2;
                break;
            case NetworkReachability.NotReachable:                      //	No Connect
                status = 0;
                break;
        }

        return status;
    }

    #endregion

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        if (IsLocal == false)
        {
            Backend.Notification.DisConnect();  //  실시간 알림 서버 연결 해제
        }
    }

    //로그인 & 서버 공용 팝업 띄우기 

    //팝업생성 해당 캔번스안의 ui 자식으로 생성
    public void CreateErrorPopUp(Canvas _canvas , RectTransform _recttransform , float Width, float Height)
    {
        // ErrorPopUp 프리팹을 가져옴
        GameObject errorPopUpPrefab = Utill_Standard.GetPrefab("ErrorPopUp");

        if (errorPopUpPrefab != null)
        {
            // ErrorPopUp을 생성하고 Canvas의 자식으로 설정
            GameObject errorPopUpInstance = Instantiate(errorPopUpPrefab, _recttransform);

            // RectTransform을 설정하여 원하는 위치/크기로 배치
            RectTransform errorPopUpRectTransform = errorPopUpInstance.GetComponent<RectTransform>();

            if (errorPopUpRectTransform != null)
            {
                errorPopUpRectTransform.localPosition = Vector3.zero; // 원하는 위치로 조정
                errorPopUpRectTransform.sizeDelta = new Vector2(Width , Height); // 원하는 크기로 조정
            }
            else
            {
                Debug.LogError("ErrorPopUp prefab does not have RectTransform component.");
            }
        }
        else
        {
            Debug.LogError("ErrorPopUp prefab is not loaded.");
        }
    }
    //에러 메시지 설정
    public void SetErrorMsg(string msg)
    {

    }


    //NSY Code

    //현재 서버시간 가져오기 
    public static string Get_ServerTime()
    {
        string temp_servertime = string.Empty;
        var servertime = Backend.Utils.GetServerTime();

        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();

        DateTime parsedDate = DateTime.Parse(time);
        temp_servertime = parsedDate.ToString();
        return temp_servertime;
    }

    //현재 로컬 시간 가져오기
    public static string Get_LocalTime(DateTime _localdataTime)
    {
        string temp = string.Empty;
        temp = _localdataTime.ToString();
        return temp;
        
    }

    //현재 유저가 이용시간 (얼마나 했는지 가져오기)
    public static string Get_PlayerUsingTime()
    {
        string temp = string.Empty;
        return temp;
    }

    /// 백엔드 서버에 로그 보내는 함수
    public void SendLogToServer(string LogType, string ColumName, string Content, bool isSendLogsEditor = true)
    {

#if UNITY_EDITOR
        if (isSendLogsEditor == false)  //유니티 에디터에서는 일부 로그를 보내지 않도록 설정
        {
            return;
        }
#endif

        if (IsLocal) //로컬 환경에서는 로그를 보내지 않도록 설정
        {
            Debug.Log("로컬 플레이어라 서버에 데이터를 보낼 필요 없습니다");
            return;
        }

        Param param = new Param();                        //     
        param.Add(ColumName, Content);                     //     "소제목"(컬럼명):   "내용"
        Backend.GameLog.InsertLogV2(LogType, param);        //      행동유형 제목    
    }

    //추후 계속 추가될 예정 

}
