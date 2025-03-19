using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 수호자 광분 스킬
/// </summary>
public class RageAttack :BaseSkill
{
    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;
    GameObject rapidFireFx;
    public override event Action<float, bool> OnActiveSecondChanged;
    RangeAttackData rangeattackdata;
    public float skillcount = 0;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(RageAttackBuff());
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public override void Init_Skill()
    {
        SoundManager.Instance.PlayAudio("RageAttack_Off");
        if (rapidFireFx != null)
        {
            rapidFireFx.gameObject.SetActive(false);
            rapidFireFx = null;
        }

        if (skillcount > 0)
        {
            skillcount = 0;
            RemoveBuff();
            OnActiveSecondChanged?.Invoke(0, false);
        }
    }



    IEnumerator RageAttackBuff()
    {
        SoundManager.Instance.PlayAudio(skillName);
        rapidFireFx = ObjectPooler.SpawnFromPool(Tag.RageAttack, hunter.transform.position);
        // x 회전값을 90도로 설정
        rapidFireFx.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        rapidFireFx.gameObject.SetActive(true);
        rapidFireFx.transform.SetParent(hunter.transform);
        skillcount = 1;
        ApplyBuff();
        OnActiveSecondChanged?.Invoke(rangeattackdata.CHGValue_SkillDuration, true);
        yield return new WaitForSeconds(rangeattackdata.CHGValue_SkillDuration);
        OnActiveSecondChanged?.Invoke(0, false);
        skillcount = 0;
        rapidFireFx.gameObject.SetActive(false);
        rapidFireFx = null;
        RemoveBuff();
        SoundManager.Instance.PlayAudio("RageAttack_Off");
        currentCoroutine = null;
    }
    private void ApplyBuff()
    {
        //공속 버프
        HunterStat.UseSkillAddUserStat(hunter._UserStat, rangeattackdata.CHGValue_AttackSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
        //이속 버프
        HunterStat.UseSkillAddUserStat(hunter._UserStat, rangeattackdata.CHGValue_MoveSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }

    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, rangeattackdata.CHGValue_AttackSpeedPercent , Utill_Enum.Upgrade_Type.AttackSpeedPercent);

        HunterStat.UseSkillMinusUserStat(hunter._UserStat, rangeattackdata.CHGValue_MoveSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        rangeattackdata = (RangeAttackData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.RageAttack, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, rangeattackdata.CHGValue_UseMP); //마나감소
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
