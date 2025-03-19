using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 약점포착 스킬
/// </summary>
public class WeaknessDetection : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("치명타 확률 버프 증가 수치")]
    [SerializeField] int criChance = 20;
    [Header("치명타 확률 버프 최대 스택")]
    [SerializeField] int maxBuffStack = 5;
    #endregion

    private bool canReciveBuff = false; //현재 이 버프를 받을 수 있는지
    private int curBuffStack = 0;

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (!isInit) //스킬 초기화
        {
            hunter.OnDodgeAction -= AfterDodge;
            hunter.OnDodgeAction += AfterDodge;
            isInit = true;
            canReciveBuff = true;
            curBuffStack = 0;
        }
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
    public override void Init_Skill()
    {
        isInit = false; 
        if(hunter!=null)
            hunter.OnDodgeAction -= AfterDodge;
        curBuffStack = 0;
    }
    private void AfterDodge(Hunter hunter)
    {
        if (!canReciveBuff)
            return;
        SoundManager.Instance.PlayAudio(skillName);
        ObjectPooler.SpawnFromPool(Tag.WeaknessDetectionFx, hunter.EffectOffset + hunter.transform.position);
        StartCoroutine(Buff());
    }
    private IEnumerator Buff()
    {
        curBuffStack++;
        if(curBuffStack == maxBuffStack)
            canReciveBuff = false;
        ApplayBuff();
        yield return new WaitForSeconds(SkillDuration);
        RemoveBuff();
        curBuffStack = 0;
        canReciveBuff = true;
        StopAllCoroutines();
    }
    private void ApplayBuff()
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, criChance, Utill_Enum.Upgrade_Type.CriChance);
    }
    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, criChance * curBuffStack, Utill_Enum.Upgrade_Type.CriChance);
    }

}
