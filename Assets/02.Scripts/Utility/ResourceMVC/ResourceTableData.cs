using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 재화 데이터 모델
/// /// </summary>
public class ResourceTableData
{
    public Utill_Enum.Resource_Type ResourceType;
    public int StartCount;
    public int MaxValue;

    //동적데이터
    private int userHasValue;//유저가 현재 가지고 있는 갯수
    public int UserHasValue
    {
        get { return userHasValue; } set {  userHasValue = value; }
    }

    public ResourceTableData() { }






    //원하는 ID를 기반으로 재화가져오기
    public static void Get_Resource()
    {

    }

    //원하는 재화의 maximum가져오는 함수
    public static int Get_maxCurrencyValue(List<ResourceTableData> list, Utill_Enum.Resource_Type type)
    {
        int temp_value = 0;
        ResourceTableData model = list.Find(c => c.ResourceType == type);
        temp_value = model.MaxValue;
        if(model.MaxValue < 0) //최대치가 음수라면
        {
            return 999999999;
        }
        return temp_value;
    }

    //원하는 재화의 갯수가 얼마있는지 알아오는 함수
    public static int Get_CurrencyValue(List<ResourceTableData> list , Utill_Enum.Resource_Type type  )
    {
        int temp_value = 0;
        ResourceTableData model = list.Find(c => c.ResourceType == type);
        temp_value = model.userHasValue;
        return temp_value;
    }

    //재화 전체 초기화
    public static void Init_AllResource()
    {

    }

    //원하는 재화 초기화
    public static void Init_SelectResource(int crrencyid)
    {

    }


    public static void Set_Resource(List<ResourceTableData> models, Utill_Enum.Resource_Type resourcetype, int value)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].ResourceType == resourcetype)
            {
                models[i].userHasValue = value;
            }
        }
    }
    //원하는 재화에 일정량 더해줌 
    public static void Plus_Resource(List<ResourceTableData>  models ,Utill_Enum.Resource_Type resourcetype , int value)
    {

        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].ResourceType == resourcetype)
            {
                models[i].userHasValue += value;
            }
        }
    }

    //원하는 재화에 일정량 감소
    public static void Minus_Resource(List<ResourceTableData> models, Utill_Enum.Resource_Type resourcetype, int value)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].ResourceType == resourcetype)
            {
                models[i].userHasValue -= value;
                if (models[i].userHasValue <= 0)
                {
                    models[i].userHasValue = 0;
                }
            }
        }
    }

    //원하는 재화에 일정량 증가
    public static void Pluse_Resource(List<ResourceTableData> models, Utill_Enum.Resource_Type resourcetype, int value)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].ResourceType == resourcetype)
            {
                models[i].userHasValue += value;
                //맥스 재화 검토
               int maxcount = GameDataTable.Instance.ResourceTableDic[resourcetype].MaxValue;
                if (models[i].userHasValue >= maxcount)
                {
                    models[i].userHasValue = maxcount;
                }
            }
        }
    }


    public static bool CheckMiuns_Resource(List<ResourceTableData> models, Utill_Enum.Resource_Type resourcetype, int value)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].ResourceType == resourcetype)
            {
                int currentvalue = models[i].userHasValue;
                int temp = currentvalue - value;
                if (temp < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;

    }
}
