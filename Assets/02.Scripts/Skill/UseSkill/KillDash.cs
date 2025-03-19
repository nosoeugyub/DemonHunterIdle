using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-10-16
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 즉살돌진 스킬
/// </summary>
public class KillDash : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("이동속도 퍼센트 증가 수치")]
    [SerializeField]
    private float moveSpeed = 100f;
    #endregion

    private KillDashData killDashData;
    private float curBuffAmount = 0; //예외사항 막기 위한 현재 버프 양
    private float curPassiveBuffAmount = 0; //예외사항 막기 위한 현재 패시브 버프 양
    private Coroutine killDashBuffCoroutine = null;
    private GameObject killDashFx = null;

    public override event Action<float, bool> OnActiveSecondChanged;
    
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if(!isInit)
        {
            isInit = true;
            hunter.OnCoupDamageAction -= KillDashStart;
            hunter.OnCoupDamageAction += KillDashStart;
            hunter.OnCriDamageAction -= KillDashStart;
            hunter.OnCriDamageAction += KillDashStart;
            AddPassiveBuff();
        }
    }

    public override void Init_Skill()
    {
        RemovePassiveBuff();
        isInit = false;
        hunter.OnCoupDamageAction -= KillDashStart;
        hunter.OnCriDamageAction -= KillDashStart;
        if (curBuffAmount > 0)
            RemoveBuff();
    }

    private void KillDashStart(Hunter hunter)
    {
        if(killDashBuffCoroutine != null)
        {
            StopCoroutine(killDashBuffCoroutine);
            if(curBuffAmount > 0)
                RemoveBuff();
        }
        SoundManager.Instance.PlayAudio(Tag.BloodSurge);
        killDashBuffCoroutine = StartCoroutine(KillDashBuff(killDashData.CHGValue_SkillDuration));
    }

    private IEnumerator KillDashBuff(float time)
    {
        RemoveBuff();
        AddBuff();
        if(killDashFx == null)
        {
            killDashFx = ObjectPooler.SpawnFromPool(Tag.KillDashEffect, Vector3.zero);
            killDashFx.transform.SetParent(hunter.transform);
            killDashFx.transform.localPosition = Vector3.zero;
        }
        OnActiveSecondChanged?.Invoke(time, true);
        yield return new WaitForSeconds(time);
        RemoveBuff();
        OnActiveSecondChanged?.Invoke(0, false);
        if(killDashFx != null)
        {
            killDashFx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
            killDashFx.gameObject.SetActive(false);
            killDashFx = null;
        }
    }

    private void AddBuff()
    {
        if (curBuffAmount > moveSpeed)
            RemoveBuff();
        HunterStat.UseSkillAddUserStat(hunter._UserStat, moveSpeed, Utill_Enum.Upgrade_Type.MoveSpeedPercent);
        curBuffAmount += moveSpeed;
    }

    private void RemoveBuff()
    {
        if (curBuffAmount <= 0)
            return;
        while(curBuffAmount > 0) //0이 될 때까지..
        {
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, moveSpeed, Utill_Enum.Upgrade_Type.MoveSpeedPercent);
            curBuffAmount -= moveSpeed;
        }
    }

    private void AddPassiveBuff()
    {
        if (curPassiveBuffAmount > killDashData.CHGValue_CriChance)
            RemovePassiveBuff();
        HunterStat.UseSkillAddUserStat(hunter._UserStat, killDashData.CHGValue_CriChance, Utill_Enum.Upgrade_Type.CriChance);
        HunterStat.UseSkillAddUserStat(hunter._UserStat, killDashData.CHGValue_CoupChance, Utill_Enum.Upgrade_Type.CoupChance);
        curPassiveBuffAmount += killDashData.CHGValue_CriChance; 
    }
    private void RemovePassiveBuff()
    {
        if (curPassiveBuffAmount <= 0)
            return;
        while(curPassiveBuffAmount > 0)
        {
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, killDashData.CHGValue_CriChance, Utill_Enum.Upgrade_Type.CriChance);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, killDashData.CHGValue_CoupChance, Utill_Enum.Upgrade_Type.CoupChance);
            curPassiveBuffAmount -= killDashData.CHGValue_CriChance;
        }
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
        killDashData = BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount) as KillDashData;
    }
    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        killDashData = BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount) as KillDashData;
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, killDashData.CHGValue_UseMP); //마나감소
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
