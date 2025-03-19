using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 뒤끝 시간 관련 VIew
/// </summary>
public class BackendView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI NowTimetxt;
    [SerializeField] TextMeshProUGUI DailyTimetxt;
    [SerializeField] TextMeshProUGUI LocalTimetxt;
    [SerializeField] TextMeshProUGUI PlayerUsingTimetxt;

    private bool hasUpdatedToday; // 오늘 오전 9시에 갱신했는지 여부를 추적
    public void Active(bool _isactive)
    {
        NowTimetxt.gameObject.SetActive(_isactive);
        DailyTimetxt.gameObject.SetActive(_isactive);
        LocalTimetxt.gameObject.SetActive(_isactive);
       // PlayerUsingTimetxt.gameObject.SetActive(_isactive);
    }

    public void GetTime(string _nowtime)
    {
        // 현재 시간을 파싱
        if (DateTime.TryParse(_nowtime, out DateTime nowTime))
        {
            // "00시 00분 00초" 형식으로 텍스트 설정
            NowTimetxt.text = $"서버 시간 {nowTime.Hour:00}시 {nowTime.Minute:00}분 ";
        }
        else
        {
            NowTimetxt.text = "Invalid current time format";
            return; // 잘못된 형식일 경우 함수 종료
        }

        // 오늘 오후 9시까지 남은 시간을 계산
        if (DateTime.TryParse(_nowtime, out DateTime now))
        {
            DateTime ninePM = now.Date.AddHours(GameManager.Instance.dailyResetDateTime); // 오늘 오후 9시
            if (now >= ninePM)
            {
                ninePM = ninePM.AddDays(1); // 다음 날 오후 9시
            }

            TimeSpan timeUntilNinePM;
            if (CheatManager.Instance.DailyResetTimeChange)
            {
                // 치트 모드에서는 N초마다 갱신
                int N = GameManager.Instance.CheatdailyResetDateTime; // N초 설정
                int elapsedSeconds = (int)(now.TimeOfDay.TotalSeconds % N); // 경과한 초
                timeUntilNinePM = TimeSpan.FromSeconds(N - elapsedSeconds); // 남은 시간 계산
            }
            else
            {
                timeUntilNinePM = ninePM - now; // 일반 모드에서는 실제 남은 시간 계산
            }

            // 남은 시간을 반올림하여 초 단위로 계산
            int totalSeconds = (int)Math.Round(timeUntilNinePM.TotalSeconds);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            // "00시 00분 00초" 형식으로 텍스트 설정
            GameEventSystem.Send_DailyInit_Event(timeUntilNinePM, false);
            DailyTimetxt.text = $"오후 9시까지 남은 시간 {hours:00}시 {minutes + 1:00}분 "; // {seconds:00}초
            PlayerUsingTimetxt.text = Utility.TimeFormatDefault(timeUntilNinePM);
        }
        else
        {
            DailyTimetxt.text = "Invalid current time format";
        }
    }

    public void SetLocalTime(string elapsedTime)
    {
        
        // "00시 00분 00초" 형식으로 텍스트 설정
        // 현재 시간을 파싱
        if (DateTime.TryParse(elapsedTime, out DateTime nowlocalTime))
        {
            // "00시 00분 00초" 형식으로 텍스트 설정
            LocalTimetxt.text = $"{nowlocalTime.Hour:00}시간 {nowlocalTime.Minute:00}분 {nowlocalTime.Second:00}초";
        }
        else
        {
            NowTimetxt.text = "Invalid current time format";
        }
       
    }

    public void SetTime(string _nowtime, string localtime)
    {
        // 현재 시간을 파싱
        if (DateTime.TryParse(_nowtime, out DateTime nowTime))
        {
            // "00시 00분 00초" 형식으로 텍스트 설정
            NowTimetxt.text = $"{nowTime.Hour:00}시간 {nowTime.Minute:00}분 {nowTime.Second:00}초";
        }
        else
        {
            NowTimetxt.text = "Invalid current time format";
        }

        // 오늘 오후 9시까지 남은 시간을 계산
        if (DateTime.TryParse(localtime, out DateTime now))
        {
            DateTime ninePM = now.Date.AddHours(GameManager.Instance.dailyResetDateTime); // 오늘 오후 9시
            if (now >= ninePM)
            {
                ninePM = ninePM.AddDays(1); // 다음 날 오후 9시
            }
            TimeSpan timeUntilNinePM = ninePM - now;

            // 남은 시간을 반올림하여 초 단위로 계산
            int totalSeconds = (int)Math.Round(timeUntilNinePM.TotalSeconds);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            // "00시 00분 00초" 형식으로 텍스트 설정
            LocalTimetxt.text = $"{hours:00}시간 {minutes + 1:00}분 "; //{seconds:00}
        }
        else
        {
            LocalTimetxt.text = "Invalid current time format";
        }
    }



    /// <summary>
    /// 매 초마다 오전 9시가 되었는지 확인하고, 오후 9시가 되면 스킬 선택 팝업창을 띄웁니다.
    /// </summary>
    public void CheckTimeForEvent(DateTime now)
    {
        // 유저의 시간 저장
        GameDataTable.Instance.User.UserSaveForTime = now.ToString();

        // 현재 시간의 TimeSpan
        TimeSpan currentTime = now.TimeOfDay;

        if (CheatManager.Instance.DailyResetTimeChange)
        {
            // 치트 모드에서 N초마다 갱신하도록 설정
            int N = GameManager.Instance.CheatdailyResetDateTime; // N초 설정
            if (currentTime.Seconds % N == 0) // 현재 초가 N의 배수일 때
            {
                if (!hasUpdatedToday) // 오늘 갱신하지 않았다면
                {
                    GameDataTable.Instance.User.isDailySkill = false; // 조건에 맞게 데이터 업데이트
                    GameEventSystem.Send_DailyInit_Event(new TimeSpan(), true);
                    hasUpdatedToday = true;
                }
            }
            else
            {
                hasUpdatedToday = false;
            }

            if (GameDataTable.Instance.User.isDailySkill == false)
            {
                ShowSkillSelectionPopup();
            }
        }
        else
        {
            // 오전 9시의 기준 시간
            TimeSpan morningResetTime = new TimeSpan(9, 0, 0); // 오전 9시

            // 오전 9시의 허용 범위 (±2초)
            TimeSpan tolerance = TimeSpan.FromSeconds(2);

            if (currentTime >= morningResetTime - tolerance && currentTime <= morningResetTime + tolerance)
            {
                if (!hasUpdatedToday) // 오늘 오전 9시에 갱신하지 않았다면
                {
                    GameDataTable.Instance.User.isDailySkill = false; // 조건에 맞게 데이터 업데이트
                    GameEventSystem.Send_DailyInit_Event(new TimeSpan(), true);
                    hasUpdatedToday = true;
                }
            }
            else
            {
                // 오전 9시가 지났으므로 다음날 갱신을 위해 상태 초기화
                hasUpdatedToday = false;
            }


            if (GameDataTable.Instance.User.isDailySkill == false)
            {
                ShowSkillSelectionPopup();
            }
        }
    }




    /// <summary>
    /// 유저가 접속할 때 오후 9시가 넘었는지 확인하고, 오후 9시가 넘었다면 기존 스킬을 비활성화하고 스킬 선택 창을 띄웁니다.
    /// 유저가 스킬을 아직 고르지 않았다면 스킬 선택 창을 띄웁니다.
    /// </summary>
    private void CheckTimeForPopupOnStart()
    {
        DateTime now = DateTime.Now;
        DateTime ninePM = now.Date.AddHours(21);

        if (now >= ninePM)
        {
            ninePM = ninePM.AddDays(1);
        }

        if (now >= ninePM.AddDays(-1))
        {
            DisableCurrentSkills();
            ShowSkillSelectionPopup();
        }
        else if (!GameDataTable.Instance.User.isDailySkill)
        {
            ShowSkillSelectionPopup();
        }
    }
    /// <summary>
    /// 스킬 선택 팝업창을 띄웁니다.
    /// </summary>
    private void ShowSkillSelectionPopup()
    {
        PopUpSystem.Instance._DeilyRandomPopUp.Show();
    }

    /// <summary>
    /// 현재 스킬들을 비활성화하는 로직을 작성합니다.
    /// </summary>
    private void DisableCurrentSkills()
    {
        Debug.Log("기존 스킬들이 비활성화되었습니다.");
    }

    /// <summary>
    /// 개발자 버튼을 눌렀을 때 일일 초기화와 함께 스킬 선택 팝업창을 띄웁니다.
    /// </summary>
    private void OnDeveloperButtonClick()
    {
        DailyReset();
        ShowSkillSelectionPopup();
    }

    /// <summary>
    /// 일일 초기화 로직을 작성합니다.
    /// 예: 퀘스트 초기화, 보상 초기화 등
    /// </summary>
    private void DailyReset()
    {
        Debug.Log("일일 초기화가 완료되었습니다.");
        GameDataTable.Instance.User.isDailySkill = false; // 스킬 선택 상태 초기화
    }
    /// <summary>
    /// 유저가 스킬을 선택했을 때 호출되는 함수
    /// </summary>
    public void OnSkillSelected()
    {
        GameDataTable.Instance.User.isDailySkill = true;
       // SkillSelectionPopup.SetActive(false);
        Debug.Log("스킬이 선택되었습니다.");
    }

}
