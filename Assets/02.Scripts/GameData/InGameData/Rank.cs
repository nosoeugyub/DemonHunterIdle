using BackEnd;
using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 랭킹 데이터 구조체
/// </summary>
public class Rank 
{
    //동점자 처리로 인해 소숫점이 필요
    public double ArcherBattlePower;
    public double GuardianBattlePower;
    public double MageBattlePower;

    public double ClearStageLevel;

    //랭킹 추가 항목
    public int ArcherLevel;
    public int GuardianLevel;
    public int MageLevel;

    public Rank()
    { 
    }

    public Rank(double archerBattlePower, double guardanBattlePower, double mageBattlePower, int arcerLevl, int guardianLvel, int mageLevel, double clearStageLevel)
    {
        ArcherBattlePower = archerBattlePower;
        GuardianBattlePower = guardanBattlePower;
        MageBattlePower = mageBattlePower;
        ArcherLevel = arcerLevl;
        GuardianLevel = guardianLvel;
        MageLevel = mageLevel;
        ClearStageLevel = clearStageLevel;
    }

    public static void Save_Rank_Power(Rank rank , User data )
    {
        //동점자 처리를 위한 서버시간 구하기
        BackendReturnObject serverTime = Backend.Utils.GetServerTime();
        DateTime serverDateTime;
        if (!DateTime.TryParse(serverTime.GetReturnValuetoJSON()["utcTime"].ToString(), out serverDateTime))
        {
            Debug.LogError("Failed to parse server time.");
        }
        string serverTimeStr = string.Format("{0:0.yyMMddHHmmss}", serverDateTime);
        double serverTimeNum = double.Parse(serverTimeStr); 

        serverTimeNum = 1 - serverTimeNum; //예전에 달성한 랭크일수록 높은 점수를 줘야함

        if (data.HunterPurchase[0] == true)//헌터 구매 체크
        {
            int curRank = HunterStat.GetCombatPoint(DataManager.Instance.Hunters[0].Orginstat, 0);
            //기존 스코어와 달라졌을 경우
            if ((int)rank.ArcherBattlePower != curRank)
                rank.ArcherBattlePower = curRank + serverTimeNum;
            rank.ArcherLevel = data.HunterLevel[0];
        }
        else
        {
            rank.ArcherBattlePower = 0;
            rank.ArcherLevel = 1;
        }

        if (data.HunterPurchase[1] == true)//헌터 구매 체크
        {
            int curRank = HunterStat.GetCombatPoint(DataManager.Instance.Hunters[1].Orginstat, 1);
            //기존 스코어와 달라졌을 경우
            if ((int)rank.GuardianBattlePower != curRank)
                rank.GuardianBattlePower = curRank + serverTimeNum;
            rank.GuardianLevel = data.HunterLevel[1];
        }
        else
        {
            rank.GuardianBattlePower = 0;
            rank.GuardianLevel = 1;
        }

        if (data.HunterPurchase[2] == true)//헌터 구매 체크
        {
            int curRank = HunterStat.GetCombatPoint(DataManager.Instance.Hunters[2].Orginstat, 2);
            //기존 스코어와 달라졌을 경우
            if ((int)rank.MageBattlePower != curRank)
                rank.MageBattlePower = curRank + serverTimeNum;
            rank.MageLevel = data.HunterLevel[2];
        }
        else
        {
            rank.MageBattlePower = 0;
            rank.MageLevel = 1;
        }

        int curStageRank = data.ClearStageLevel;
        //기존 스코어와 달라졌을 경우
        if ((int)rank.ClearStageLevel != curStageRank)
            rank.ClearStageLevel = curStageRank + serverTimeNum;
    }

    public static void Save_Rank_Level(Rank rank, int archerBattlePower, int guardanBattlePower, int mageBattlePower)
    {
        rank.ArcherLevel = archerBattlePower;
        rank.GuardianLevel = guardanBattlePower;
        rank.MageLevel = mageBattlePower;
    }

}
