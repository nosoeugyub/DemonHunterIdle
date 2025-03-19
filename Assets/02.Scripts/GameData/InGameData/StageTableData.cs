using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스테이지 값들을 불러와서 제어하는 스크립트
/// </summary>
public class StageTableData 
{
    public int StageIndex;
    public int ChapterZone;
    public int ChapterLevel;
    public int ChapterCycle;
    public int GoalKillCount;
    public float[] HPWeight;
    public float[] PhysicalPowerWeight;
    public float[] PhysicalPowerDefenseWeight;
    public float[] SpawnArea;
    public float[] Count1;
    public string[] MobName1;
    public string BossName;
    public string BossTimelimited;
    public float GoldDropChance1;
    public int Gold1;
    public int Exp1;
    public string[] StageMap;

    public float BossHPWeight;
    public float BossPhysicalPowerWeight;
    public float BossPhysicalPowerDefenseWeight;
    public int BossCount;
    public string MobMaterial1;
    public string BossMaterial;


    //로컬에서만 사용될 클리어 변수
    private bool[] _isClearStageArray;
    public bool[] IsSClearStageArray { get { return _isClearStageArray; } set { _isClearStageArray = value; } }

    //보스 클리어 변수
    private bool _isClearBoss;
    public bool IsClearBoss { get { return _isClearBoss; } set { _isClearBoss = value; } }

    #region 스테이지 관련
    public static void Make_IsSClearStageArray(Dictionary<int, StageTableData> userstagetable)
    {
        foreach (StageTableData stagedata in userstagetable.Values)
        {
            stagedata.IsSClearStageArray = new bool[stagedata.ChapterCycle];
        }
    }

    public static bool IsRoundCheck(User userstagetable , int goalkillcount , int Round , int total)
    {
        bool isbosscheck = false;
        int temp_goalcount = goalkillcount / total;
        int goalcount = temp_goalcount * Round;
        if (userstagetable.AccumulateCurrentKillCount >= goalcount)
        {
            isbosscheck= true;
            return isbosscheck;
        }
        
        return isbosscheck;
    }

    public static bool IsBossCheck(User userstagetable, int goalkillcount)
    {
        bool isbosscheck = false;
        if (userstagetable.AccumulateCurrentKillCount >= goalkillcount)
        {
            isbosscheck = true;
            return isbosscheck;
        }

        return isbosscheck;
    }
    #endregion



    #region 레벨에 맞는 몹 보정값 셋팅
    public static float GetHpWeight(StageTableData  stagetable, float MonseterPower , int currentclearstage)//보정값을 적용한 hp
    {
        float temp = 0f;
        float AddDmg = 0;
        float totalAddDmg = 0;
        float totalDmg = 0;

        float isclearstage = currentclearstage;

        temp = MonseterPower;
        AddDmg = stagetable.HPWeight[1] * isclearstage;
        totalAddDmg = AddDmg + stagetable.HPWeight[0];
        totalDmg = temp + (temp * totalAddDmg);


        return totalDmg;
    }

    
    public static float GetPhysicalPower(StageTableData stagetable, float MonseterPower, int currentclearstage)//보정값을 적용한 power
    {
        float temp = 0f;
        float AddDmg = 0;
        float totalAddDmg = 0;
        float totalDmg = 0;

        float isclearstage = currentclearstage;

        temp = MonseterPower;
        AddDmg = stagetable.PhysicalPowerWeight[1] * isclearstage;
        totalAddDmg = AddDmg + stagetable.PhysicalPowerWeight[0];
        totalDmg = temp + (temp * totalAddDmg);


        return totalDmg;
    }

    
    public static float GetPhysicalDefence(StageTableData stagetable, float MonseterPower, int currentclearstage)//보정값을 적용한 방어력
    {
        float temp = 0f;
        float AddDmg = 0;
        float totalAddDmg = 0;
        float totalDmg = 0;

        float isclearstage = currentclearstage;

        temp = MonseterPower;
        AddDmg = stagetable.PhysicalPowerDefenseWeight[1] * isclearstage;
        totalAddDmg = AddDmg + stagetable.PhysicalPowerDefenseWeight[0];
        totalDmg = temp + (temp * totalAddDmg);


        return totalDmg;
    }

    public static string GetStageBossName(Dictionary<int, StageTableData> StageDic , int level)//레벨에따른 보스이름 갖고오기
    {
        string temp_bossname =string.Empty;
        temp_bossname = StageDic[level].BossName;

        return temp_bossname;
    }

    public static float GetBossHpWeight(StageTableData stagetable, float MonseterPower, int currentclearstage)//보정값을 적용한 hp
    {
        return MonseterPower * stagetable.BossHPWeight;
    }

    public static float GetBossPhysicalPower(StageTableData stagetable, float MonseterPower, int currentclearstage)//보정값을 적용한 power
    {
        return MonseterPower * stagetable.BossPhysicalPowerWeight;
    }


    public static float GetBossPhysicalDefence(StageTableData stagetable, float MonseterPower, int currentclearstage)//보정값을 적용한 방어력
    {
        return MonseterPower * stagetable.BossPhysicalPowerDefenseWeight;
    }
    #endregion


    #region 레벨로 값가져오는 것들
    public static int GetTotalGoalKillCount(Dictionary<int, StageTableData> StageDic, int level)//총 목표 킬수 가져오기
    {
        int count = 0;
        count = StageDic[level].GoalKillCount;
        return count;
    }

    public static int GetGoalKillCount(Dictionary<int, StageTableData> StageDic, int level)//한 라운드 킬수 가져오기
    {
        int count = 0;
        count = StageDic[level].GoalKillCount / StageDic[level].ChapterCycle;
        return count;
    }
    #endregion



    #region 50%확률로 지형에 쓰일 텍스쳐 스트링 반환하는 함수
    public static string Get_StringGroundName()
    {
        string temp = string.Empty;

        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userStage = UserStage.CurrentStage;
        //유저라운드에 맞는 스테이지 스트링값 가져오기
        string[] temp_array = GameDataTable.Instance.StageTableDic[UserStage.stageindex].StageMap;

        temp = Utill_Standard.GetRandomString(temp_array);

        return temp;
    }
    #endregion
}
