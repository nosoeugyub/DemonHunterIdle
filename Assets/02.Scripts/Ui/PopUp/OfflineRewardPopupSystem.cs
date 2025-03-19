using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// </summary>
/// 작성일자   : 2024-09-24
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 오프라인 팝업보상 매니저
/// </summary>
public class OfflineRewardPopupSystem : MonoSingleton<OfflineRewardPopupSystem>
{
    public OfflineRewardView offline_view;

    
    //오프라인 보상
    int gold;
    int exp;
    int dia;
    private void Awake()
    {
        GameEventSystem.OfflineReward_Event += SendOfflineReward;
    }

    private void SendOfflineReward() //오프라인 보상 우편 보내기
    {
        OfflineRewardData goldcelldata = new OfflineRewardData();
        goldcelldata.ResourceType = Utill_Enum.Resource_Type.Gold;
        goldcelldata.Count = gold;
        GameManager.Instance.SendRewards(goldcelldata);

        OfflineRewardData expcelldata = new OfflineRewardData();
        expcelldata.ResourceType = Utill_Enum.Resource_Type.Exp;
        expcelldata.Count = exp;
        GameManager.Instance.SendRewards(expcelldata);

        OfflineRewardData diacelldata = new OfflineRewardData();
        diacelldata.ResourceType = Utill_Enum.Resource_Type.Dia;
        diacelldata.Count = dia;
        GameManager.Instance.SendRewards(diacelldata);

        ClosedOfflinePopup(); //비활성화

    }

    public void Setting_OfflinePopup() //게임 시작시 떠야함
    {
        int temp_minit = MakeRewardTime();


        if (temp_minit > 0) //활성화
        {
            ShowOfflinePopup();
            //ui값 셋팅
            Setting_View(temp_minit);


        }
        else // 비활성화
        {
            ClosedOfflinePopup();
        }
    }

    public int MakeRewardTime() //(최종 종료시간 - 현재 접속시간)
    {
        int temp = 0; // 보상 시간을 저장할 변수
        string nowtime = string.Empty;

        // 유저의 마지막 저장 시간 (최종 종료 시간)
        string UserSaveData = GameDataTable.Instance.User.UserSaveForTime;

        string cleanedSaveData = UserSaveData
      .Replace("{", "")
      .Replace("}", "")
      .Replace("오전", "AM")
      .Replace("오후", "PM");


        DateTime lastSaveTime;
        if (!DateTime.TryParse(cleanedSaveData, out lastSaveTime))
        {
            // 저장된 시간이 유효하지 않으면 함수 종료
            temp = -1;
            return temp;
        }

        // 현재 시간 구하기
        if (BackendManager.Instance.IsLocal) // 로컬이라면
        {
            nowtime = DateTime.Now.ToString(); // 로컬 시스템 시간을 사용
        }
        else // 서버 시간 가져오기
        {
            nowtime = BackendManager.Get_ServerTime();
        }

        DateTime currentTime;
        if (!DateTime.TryParse(nowtime, out currentTime))
        {
            // 서버 시간이 유효하지 않으면 함수 종료
            temp = -2;
            return temp;
        }

        // (최종 종료시간 - 현재 접속시간) 구하기 : 분단위로
        TimeSpan timeDifference = currentTime - lastSaveTime;
        offline_view.timespan = timeDifference;
        int tempSeconed = timeDifference.Seconds;// 초 단위 가져오기


        if (tempSeconed > 0)
        {
            tempSeconed = 1;
        }
        else
        {
            tempSeconed = 0;
        }

        int totalMinutes = (int)timeDifference.TotalMinutes; // 분 단위로 변환
        totalMinutes += tempSeconed;
        // 최소 미접속 시간 설정 (예: 10분 이상)
        int minAllowedTime = GameManager.Instance.OfflineMinRewardTime;
        int maxAllowedTime = GameManager.Instance.OfflineMaxRewardTime;


        // 미접속 최소 적용시간을 넘어야만 보상을 지급함
        temp = totalMinutes;
        if (totalMinutes < minAllowedTime)
        {
            //보상지급 안함
            temp = 0; // 
        }
        else if(totalMinutes >= maxAllowedTime) // 최대적용기간을 sjarlaus
        {
            temp = maxAllowedTime;
        }


        return temp;
    }

    public void ShowOfflinePopup() //오프라인 팝업 열기
    {
        offline_view.Show();
    }

    public void ClosedOfflinePopup() // 오프라인 팝업 닫기
    {
        offline_view.Hide();
    }
   

    public void Setting_View(int minit) //분값으로 ui 데이터 셋팅
    {
        //계수와 값들을 대입하여 계산후 ui보여주기
         gold = minit * GameDataTable.Instance.User.AverageAcquisitionResource[0] * GameManager.Instance.OfflineGoldCoefficient; // 0 골드
         exp = minit * GameDataTable.Instance.User.AverageAcquisitionResource[1] * GameManager.Instance.OfflineExpCoefficient; // 0 골드
         dia = minit * GameManager.Instance.OfflineDiaCoefficient;
        offline_view.Update_OfflineVIew(minit , gold , exp , dia ,GameManager.Instance.OfflineMinRewardTime , GameManager.Instance.OfflineMaxRewardTime);
    }
}
