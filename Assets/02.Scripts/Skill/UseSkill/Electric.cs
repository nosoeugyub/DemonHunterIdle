using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 벼락치기 스킬
/// </summary>
public class Electric : BaseSkill
{
    ElectricData electricdata;


    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (!isInit) //스킬 초기화
        {
            isInit = true;
            HunterStat.Plus_ElectricPersent(hunter._UserStat, electricdata.CHGValue_CastingChance , -1); //duration은 사용 안 함
        }
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
    
    public override void Init_Skill()
    {
        isInit = false;
        HunterStat.Miuse_ElectricPersent(hunter._UserStat, electricdata.CHGValue_CastingChance, -1); //duration은 사용 안 함
    }


    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        electricdata = (ElectricData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.Electric, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, electricdata.CHGValue_UseMP); //마나감소
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
