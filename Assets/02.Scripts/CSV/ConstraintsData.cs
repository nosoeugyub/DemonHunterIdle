using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 게임에사용한 최대칙 값들
/// </summary>
public class ConstraintsData
{
    public  string Name;           
    public int Value;
    public float _floatValue;


    //최대를 넘는지 안넘는지 확안하는 함수(스테이지는 이거안씀)
    public static bool CheckMaxValue(ConstraintsData data, int _value)
    {
        bool ischeck= false;
        int temp = 0;
        if (data.Value <= _value)
        {
            ischeck = true;
            return ischeck;
        }

        ischeck = false;
        return ischeck;
    }
    public static bool CheckMaxValue(int data, int _value)
    {
        bool ischeck = false;

        if (data <= _value)
        {
            ischeck = true;
            return ischeck;
        }

        ischeck = false;
        return ischeck;
    }
    //기존값과 테이블의 max값을 비교해서 값 반환하느 함수(스테이지는 이거안씀)
    public static int CheckMaxValueResult(ConstraintsData data   , int _value )
    {
        int temp = 0;
        if (data.Value <= _value)
        {
            temp= data.Value;
            return temp;
        }

        temp = _value;
        return temp;
    }



    //최대스테이지 넘어섯을경우
    public static (int chapter , int stage , int Round , bool isMaxstage) StageCheckMaxValueResult(ConstraintsData data, int Stage, int Round)
    {
        int temp_stage = 0;
        int temp_Round = 0;
        int temp_Chapter = 0;
        bool isMaxStage = false;

        //마지막스테이지의 마지막 라운드를 얻기위해 데이터 가공
        ConstraintsData constraindata = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL];
        int maxlndex = constraindata.Value;

        //마지막스테이지 총합
        int maxclearstage = Utill_Math.Make_Max_stage(maxlndex);
        //유저현재 스테이지
        int usercurrentstage = GameDataTable.Instance.User.ClearStageLevel;

        StageTableData stage = GameDataTable.Instance.StageTableDic[maxlndex];
        temp_stage = stage.ChapterLevel;
        temp_Round = stage.ChapterCycle;
        temp_Chapter = stage.ChapterZone;

        if (maxclearstage < usercurrentstage) 
        {
            isMaxStage = true;
            return (temp_Chapter ,temp_stage, temp_Round, isMaxStage);
        }
        else
        {
            isMaxStage = false;
            return (temp_Chapter , Stage, Round, isMaxStage);
        }
        
    }

}