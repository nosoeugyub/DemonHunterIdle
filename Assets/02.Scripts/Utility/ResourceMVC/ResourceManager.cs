using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// /// </summary>
using static Utill_Enum;
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :재화 컨틀롤러 
/// /// </summary>
public class ResourceManager : MonoSingleton<ResourceManager>
{
    public ResourceView ResourceView;


    public List<ResourceTableData> ResourceModels;





    private void Awake()
    {
        ResourceModels = new List<ResourceTableData> ();

        GameEventSystem.GamePluseGold_GameEventHandler_Event += PlusGold;
        GameEventSystem.GameMinusGold_GameEventHandler_Event += MinusGold;


        GameEventSystem.Resource_Update_Event += Update_Resourcetxt; //현재 재화 표기

        GameEventSystem.GamePluseDia_GameEventHandler_Event += PlusDia;
        GameEventSystem.GameMinusDia_GameEventHandler_Event += MinusDia;
    }

    private void Update_Resourcetxt()
    {
        UpdateReousrce(); //ui 최상단
        ItemDrawer.Instance.itemDrawerview.Update_Resource();//모루 팝업
        UIManager.Instance.gachaPopUp.Update_Resource(); // 소환 팝업
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(InputManager.Instance.maxResourceKey))
        {
            int dia = ResourceManager.Instance.Get_MaximumResource(Utill_Enum.Resource_Type.Dia);
            int gold = ResourceManager.Instance.Get_MaximumResource(Utill_Enum.Resource_Type.Gold);

            ResourceManager.Instance.SetGold(Utill_Enum.Resource_Type.Gold, gold);
            ResourceManager.Instance.SetDia(Utill_Enum.Resource_Type.Dia, dia);
        }
    }
#endif

    #region 공통
    //재화가 들어올떄 max 재화를 넘었는가 를 따지는 함수 
    public int Get_MaximumResource(Utill_Enum.Resource_Type resource_Type)
    {
        int ischeck = ResourceTableData.Get_maxCurrencyValue(ResourceModels, resource_Type);
        return ischeck;

    }
    //재화를 사용이가능한지 = 사용햇을때 -가 되는지안되는지
    public bool CheckResource(Utill_Enum.Resource_Type resource_Type , int count )
    {
        bool ischeck = ResourceTableData.CheckMiuns_Resource(ResourceModels, resource_Type, count);
        return ischeck;

    }
 


    //유저 타입에다라 재화 빼기
   public void Minus_ResourceType(Utill_Enum.Resource_Type resource_Type, int count)
   {
        if (CheckResource(resource_Type , count))
        {
            ResourceTableData.Minus_Resource(ResourceModels, resource_Type, count);
        }
   }

    public void Pluse_ResourceType(Utill_Enum.Resource_Type resource_Type, int count)
    {
        ResourceTableData.Pluse_Resource(ResourceModels, resource_Type, count);
    }

    #endregion

    #region 골드
    public int GetGold()
    {
        int temp = 0;
        temp = ResourceTableData.Get_CurrencyValue(ResourceModels , Utill_Enum.Resource_Type.Gold);
        return temp;
    }

    public void SetGold(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        ResourceTableData.Set_Resource(ResourceModels, Resourcetpye, Value);
        UpdateReousrce();
    }


    public void PlusGold(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        ResourceTableData.Plus_Resource(ResourceModels, Resourcetpye,  Value);
        UpdateReousrce();

    }

    private void MinusGold(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        ResourceTableData.Minus_Resource(ResourceModels, Resourcetpye, Value);
        UpdateReousrce();
    }

    #endregion

    #region 다이아
    public int GetDia()
    {
        int temp = 0;
        temp = ResourceTableData.Get_CurrencyValue(ResourceModels, Utill_Enum.Resource_Type.Dia);
        return temp;
    }

    public void SetDia(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        ResourceTableData.Set_Resource(ResourceModels, Resourcetpye, Value);
        UpdateReousrce();
    }


    public void PlusDia(Utill_Enum.Resource_Type currencytpye, int Value)
    {
        ResourceTableData.Plus_Resource(ResourceModels, currencytpye, Value);
        UpdateReousrce();
    }

    private void MinusDia(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        ResourceTableData.Minus_Resource(ResourceModels, Resourcetpye, Value);
        UpdateReousrce();
    }
    #endregion

    public void UpdateReousrce()
    {
        for (int i = 0; i < ResourceModels.Count; i++) 
        {
            ResourceView.Update_Resource(ResourceModels[i].ResourceType.ToString() , ResourceModels[i].UserHasValue);
        }
    }
}
