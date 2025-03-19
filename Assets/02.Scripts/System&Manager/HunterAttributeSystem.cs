using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using System;

/// <summary>
/// 작성일자   : 2024-07-11
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 속성 시스템 관리 
/// </summary>
public class HunterAttributeSystem : MonoSingleton<HunterAttributeSystem>
{
    public HunterAttributeView Attributeview;
    public List<ObscuredInt[]> Attributes = new List<ObscuredInt[]>();




    /// <summary>
    /// 게임 처음 시작시 속성값 바인딩
    /// </summary>
    public void binding_attribute(List<ObscuredInt[]> _Attribute)
    {
        Attributes = _Attribute;
    }
  
    /// <summary>
    /// 초기화
    /// </summary>
    public void Init_AttributeBtn(int index)
    {
        //기존의 더해줬던 스텟만큼 빼야함
        HunterAttributeData.MinuseUserAttributeUpgrda(DataManager.Instance.Hunters[index].Orginstat, GameDataTable.Instance.User);

        //그리고 스텟 초기화
        HunterAttributeData.Attribute_init(GameDataTable.Instance.User , index);

        Utill_Enum.SubClass subclass = (Utill_Enum.SubClass)index;

        //ui 표기 
        Attributeview.UpdateUi_Attribute(Attributes[index] , subclass);

        SoundManager.Instance.PlayAudio("UseBuffBook");
    }

    /// <summary>
    /// 첫번째 속성 버튼 터치시
    /// </summary>
    public void PluseAttribute_01(int hunterindex , bool istoogle)
    {
        int currentHunter = hunterindex;
        //스텟증가
        HunterAttributeData.Pluse_AttributeStat(GameDataTable.Instance.User, 1 , currentHunter , istoogle);
        
        //속성값 계산 
        HunterAttributeData.UserAttributeUpgrda01(DataManager.Instance.Hunters[currentHunter].Orginstat,GameDataTable.Instance.User , currentHunter , istoogle);
        //ui 표기 
        Utill_Enum.SubClass subclass = (Utill_Enum.SubClass)hunterindex;
        Attributeview.UpdateUi_Attribute(Attributes[currentHunter] , subclass);
    }

    /// <summary>
    /// 두번째 속성 버튼 터치시
    /// </summary>
    public void PluseAttribute_02(int hunterindex , bool istoogle)
    {
        int currentHunter = hunterindex;

        //스텟증가
        HunterAttributeData.Pluse_AttributeStat(GameDataTable.Instance.User, 2, currentHunter , istoogle);

        //속성값 계산 
        HunterAttributeData.UserAttributeUpgrda02(DataManager.Instance.Hunters[currentHunter].Orginstat, GameDataTable.Instance.User , currentHunter , istoogle);
        //ui 표기 
        Utill_Enum.SubClass subclass = (Utill_Enum.SubClass)hunterindex;
        Attributeview.UpdateUi_Attribute(Attributes[currentHunter] , subclass);
    }

    /// <summary>
    /// 세번째 속성 버튼 터치시
    /// </summary>
    public void PluseAttribute_03(int hunterindex , bool istoogle)
    {
        int currentHunter = hunterindex;

        //스텟증가
        HunterAttributeData.Pluse_AttributeStat(GameDataTable.Instance.User, 3, currentHunter , istoogle);

        //속성값 계산 
        HunterAttributeData.UserttributeUpgrda03(DataManager.Instance.Hunters[currentHunter].Orginstat, GameDataTable.Instance.User, currentHunter , istoogle);
        //ui 표기 
        Utill_Enum.SubClass subclass = (Utill_Enum.SubClass)currentHunter;
        Attributeview.UpdateUi_Attribute(Attributes[currentHunter] , subclass);
    }

    
}
