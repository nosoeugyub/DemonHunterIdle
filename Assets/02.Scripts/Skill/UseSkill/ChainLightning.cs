using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 연쇄전기 스킬 
/// </summary>
public class ChainLightning : BaseSkill
{

    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정 변수

    [Header("기본 공격시 일정 확률로 발동")]
    public float CastingChance;

    [Header("연쇄되는 최대 개수")]
    public int ChainLightningNumber;

    [Header("연쇄가능한 거리")]
    public float ChainLightningRange;
    #endregion

    private bool isBuffApplied = false;
    ChainLighingData chainlighingdata;
    public override void Init_Skill()
    {
        isBuffApplied = false;
        HunterStat.Miuse_ChainLightningPersent(hunter.Orginstat, CastingChance, SkillDuration);
    }
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(UseChain());
        StartCoroutine(UseChain());
    }
    IEnumerator UseChain()
    {
        if (!isBuffApplied && hunter.CanGetDamage) //이변수로 플레이어 상태를 조율함 
        {
            isBuffApplied = true;
            HunterStat.Plus_ChainLightningPersent(hunter.Orginstat, chainlighingdata.CHGValue_CastingChance , SkillDuration);
            yield return null;
            //N초후 원래대로
        }
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        chainlighingdata = (ChainLighingData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.ChainLightning, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, chainlighingdata.CHGValue_UseMP); //마나감소
        if (tempmana == 0)
        {
            return false; //마나 사용실패
        }
        else
        {
            //ui까지 변화 적용
            DataManager.Instance.Hunters[index].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[index]._UserStat.MP, DataManager.Instance.Hunters[index]._UserStat.CurrentMp);
            return true;
        }
    }
}
