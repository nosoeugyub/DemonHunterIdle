using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Socketio;
using System.Globalization;


/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 서버 로직 컨트롤러
/// </summary>
public class ServerController : MonoSingleton<ServerController>
{

    [SerializeField] BackendView BackEndView;

    [Header("몇 초마다 서버와 로컬 시간을 동기화 할껀지")]
    public float SyncTimeServerToLocal;

    private DateTime startTime;
    private Coroutine syncCoroutine;
    public DateTime LocalTime;


    private void Start()
    {
        // 게임이 시작될 때 로컬 시간을 저장
        startTime = DateTime.Now;

        if (BackendManager.Instance.IsLocal) //로컬이면 동작 안 하도록
            return;

       
        //게임 시작하면 서버시간을 처음바다 로컬 타이머 작동 
        StartServerTimer();
        // 동기화 코루틴 시작 현재는 10분마다
        syncCoroutine = StartCoroutine(SyncTimeWithServer());


        //게임시작하면 오프라인 보상이 먼저나와야함
        OfflineRewardPopupSystem.Instance.Setting_OfflinePopup();

    }

    public void ActivePopUp(bool isActive)
    {
        BackEndView.Active(isActive);
    }
    public void StartTime()
    {
        string name = BackendManager.Get_ServerTime();
        BackEndView.GetTime(name);
    }


    // 서버 시간과 동기화
    private IEnumerator SyncTimeWithServer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(SyncTimeServerToLocal); // 10분 대기

            // 서버 시간 가져오기 (서버 시간 요청 구현 필요)
            string serverTime = BackendManager.Get_ServerTime();

            // 서버 시간과 로컬 시간을 동기화
            if (DateTime.TryParse(serverTime, out DateTime parsedServerTime))
            {
                LocalTime = parsedServerTime;
            }

            // 로컬 시간과 서버 시간을 UI에 업데이트
            BackEndView.SetTime(serverTime, serverTime);
        }
    }


    public void StartLocalTimer()//이건 로컬씬에서 사용하기떄문에...여기서 사용못하고 GameManager쪽에서 사용
    {
        if (BackendManager.Instance.IsLocal == true) //로컬인지
        {

            StartCoroutine(StartLocalTimerCoroutine());
        }

    }

    public void StartServerTimer()
    {
        if (BackendManager.Instance.IsLocal == false) //서버인지
        {
            StartCoroutine(StartServerTimerCoroutine());
        }
    }

    //ex_) "2024-06-12 오후 1:45:32"
    IEnumerator StartLocalTimerCoroutine()
    {
        // 게임 시작할 때 서버 시간 가져오기
        DateTime localTime = DateTime.Now;
        int savetime = 0;
        while (true)
        {
            // 1초가 추가된 시간 계산
            yield return new WaitForSeconds(1.0f); // 1초 대기
            localTime = localTime.AddSeconds(1.0f);
            savetime++;
            if (savetime % 60  == 0)
            {
                int tempgold =  GameDataTable.Instance.User.AverageAcquisitionResource[0];
                int tempexp = GameDataTable.Instance.User.AverageAcquisitionResource[1];

                if (tempgold < User.Gold)
                {
                    GameDataTable.Instance.User.AverageAcquisitionResource[0] = User.Gold;
                }
                GameManager.Instance.SetGold(GameDataTable.Instance.User.AverageAcquisitionResource[0]);
               

                if (tempexp < User.Exp)
                {
                    GameDataTable.Instance.User.AverageAcquisitionResource[1] = User.Exp;
                }
                GameManager.Instance.SetExp(GameDataTable.Instance.User.AverageAcquisitionResource[1]);
                savetime = 0;
            }

            // 시간을 문자열로 포맷해서 출력 (예: "2024-06-12 오후 1:45:32")
            string formattedTime = localTime.ToString("yyyy-MM-dd tt h:mm:ss");
            BackEndView.SetLocalTime(formattedTime);
            // 9시 이벤트 체크
            BackEndView.CheckTimeForEvent(localTime);
            BackEndView.GetTime(formattedTime);
        }
    }

    IEnumerator StartServerTimerCoroutine()
    {

        //게임시작할때 서버시간 가져오기
        string temptime = string.Empty;
        var servertime = Backend.Utils.GetServerTime();
        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime parsedDate = DateTime.Parse(time);
        temptime = parsedDate.ToString();
        int savetime = 0;
        while (true)
        {
            yield return Utill_Standard.WaitTimeOne; // 1초 대기
            string name = BackendManager.Get_ServerTime();
            savetime++;
            if (savetime % 60 == 0)
            {
                int tempgold = GameDataTable.Instance.User.AverageAcquisitionResource[0];
                int tempexp = GameDataTable.Instance.User.AverageAcquisitionResource[1];

                if (tempgold < User.Gold)
                {
                    GameDataTable.Instance.User.AverageAcquisitionResource[0] = User.Gold;
                }
                GameManager.Instance.SetGold(GameDataTable.Instance.User.AverageAcquisitionResource[0]);


                if (tempexp < User.Exp)
                {
                    GameDataTable.Instance.User.AverageAcquisitionResource[1] = User.Exp;
                }
                GameManager.Instance.SetExp(GameDataTable.Instance.User.AverageAcquisitionResource[1]);
                savetime = 0;
            }
            //9시 이벤트...해야함
            BackEndView.CheckTimeForEvent(parsedDate);
            BackEndView.SetLocalTime(temptime);
            BackEndView.GetTime(name);
        }
    }

    public bool isDailySkillCheck()
    {
        // 유저가 최종으로 접속한 시간
         string userLastTimeStr = GameDataTable.Instance.User.UserSaveForTime;

        // 한글 문화권 지정
        CultureInfo culture = new CultureInfo("ko-KR");
        DateTime parsedDate;
        // DateTime 형식 정의
        string format = "yyyy-MM-dd tt h:mm:ss";

        if (userLastTimeStr == "" )
        {
            GameDataTable.Instance.User.isDailySkill = false;
            return false;
        }

        if (BackendManager.Instance.IsLocal == true)
        {
            // 대괄호 제거
            userLastTimeStr = userLastTimeStr.Trim('{', '}');

            if (userLastTimeStr == "")
            {
                GameDataTable.Instance.User.isDailySkill = false;
                return false;
            }

            // '오전' 또는 '오후'과 날짜 사이에 공백 추가
            userLastTimeStr = AddSpaceBeforeTime(userLastTimeStr);

            // 시간와 'PM' 또는 'AM' 사이에 공백 추가
            userLastTimeStr = userLastTimeStr.Insert(13, " ");
        }

        // 문자열을 DateTime으로 변환
        if (!DateTime.TryParseExact(userLastTimeStr, format, culture, DateTimeStyles.None, out  parsedDate))
        {
            return false;
        }



        DateTime referenceTime;
        // 로컬에서 접속했는지 확인
        if (BackendManager.Instance.IsLocal)
        {
            // 로컬 시간 가져오기
            DateTime localTime = DateTime.Now;

            // 오늘 오전 9시를 기준으로 시간 설정
            DateTime todayNinePM = localTime.Date.AddHours(GameManager.Instance.dailyResetDateTime);

            // 만약 로컬 시간이 오전 9시 이전이라면 
            if (localTime < todayNinePM)
            {
                //어제 버프랑 비교해야함해서 어제 오전 9시로 갱신
                todayNinePM = todayNinePM.AddDays(-1);
            }
            referenceTime = todayNinePM;
        }
        else // 서버로 접속했는지 확인
        {
            var serverTimeResult = Backend.Utils.GetServerTime();
            if (serverTimeResult.IsSuccess())
            {
                string serverTimeStr = serverTimeResult.GetReturnValuetoJSON()["utcTime"].ToString();
                DateTime serverTime;
                if (!DateTime.TryParse(serverTimeStr, out serverTime))
                {
                    Debug.LogError("Failed to parse server time.");
                    return false;
                }

                // 오늘 오전 9시를 기준으로 시간 설정
                DateTime todayNinePM = serverTime.Date.AddHours(GameManager.Instance.dailyResetDateTime);

                // 만약 서버 시간이 오전 9시 이전이라면, 갱신 시간은 전날 오전 9시로 설정
                if (serverTime < todayNinePM)
                {
                    todayNinePM = todayNinePM.AddDays(-1);
                }

                referenceTime = todayNinePM;
            }
            else
            {
                Debug.LogError("Failed to get server time.");
                return false;
            }
        }

        // 마지막 접속 시간과 갱신 시간 비교
        if (parsedDate < referenceTime)
        {
            //어제 오전 9시 이후에 스킬 버프를 안받았으면 버프받게끔
            if (GameDataTable.Instance.User.isDailySkill == true)
            {
                GameDataTable.Instance.User.isDailySkill = false;
            }
            return false; // 갱신 시간이 지났다면 false 반환
        }
     
        return true; // 갱신 시간이 지나지 않았다면 true 반환
    }


    private string AddSpaceBeforeTime(string input)
    {
        if (input.Contains("오전") || input.Contains("오후"))
        {
            input = input.Replace("오전", " 오전");
            input = input.Replace("오후", " 오후");
        }
        return input;

        return input;
    }
}
