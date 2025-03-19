using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 야수화 스킬
/// </summary>
public class BeastTransformation : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("야수화시 필요 스택")]
    [SerializeField] int StackCount = 5;
    [Header("치명타 확률 증가 수치")]
    [SerializeField] int criChance = 20;
    [Header("공격속도 퍼센트 증가 수치")]
    [SerializeField] int attackSpeedPercent = 20;
    #endregion

    public override event System.Action<int> OnStackChanged;
    private bool isReciveBuff = false; //현재 이 버프를 받을 수 있는지
    private int curBuffStack = 0;

   
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (!isInit) //스킬 초기화
        {
            isInit = true;
            isReciveBuff = false;
            curBuffStack = 0;
            hunter.OnNormalAttackAction -= IncreaseStack;
            hunter.OnNormalAttackAction += IncreaseStack;
        }
        CheckStack();
    }
    public override void Init_Skill()
    {
        isInit = false;
        isReciveBuff = false;
        curBuffStack = 0;
        hunter.OnNormalAttackAction -= IncreaseStack;
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
   

    private void IncreaseStack(Hunter hunter)
    {
        if(!isReciveBuff) //야수화 상태의 평타는 계산 안 함
        {
            curBuffStack++;
            OnStackChanged?.Invoke(curBuffStack); // 이벤트 호출
        }
        
        if (curBuffStack >= GetStackNum())
            CheckStack();
    }
    private void CheckStack()
    {
        if(curBuffStack >= GetStackNum())
        {
            if (isReciveBuff) //중복 버프 방지
                return;

            float tempmana = HunterStat.MinusMp(hunter._UserStat, SkillUseMP); //마나감소
            if (tempmana == 0)
            {
                return; //마나 사용실패
            }
            else
            {
                //ui까지 변화 적용
                hunter.mpuiview.UpdateHpBar(hunter._UserStat.MP, hunter._UserStat.CurrentMp);
            }

            isReciveBuff = true;
            SoundManager.Instance.PlayAudio(skillName);
            StartCoroutine(Buff());
        }
    }

    private IEnumerator Buff()
    {
        GameObject beastTransformationFx = ObjectPooler.SpawnFromPool(Tag.BeastTransformationFx, hunter.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        beastTransformationFx.transform.SetParent(hunter.transform);
        beastTransformationFx.transform.localPosition = Vector3.zero;
        
        ApplayBuff();
        yield return new WaitForSeconds(SkillDuration);
        RemoveBuff();

        beastTransformationFx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
        beastTransformationFx.gameObject.SetActive(false);
        curBuffStack = 0;
        OnStackChanged?.Invoke(curBuffStack); // 이벤트 호출

        isReciveBuff = false;
    }
    private void ApplayBuff()
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, criChance, Utill_Enum.Upgrade_Type.CriChance);
        HunterStat.UseSkillAddUserStat(hunter._UserStat, attackSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }
    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, criChance, Utill_Enum.Upgrade_Type.CriChance);
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, attackSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }

    private int GetStackNum()
    {
        if((StackCount - hunter._UserStat.BeastTransformationStackCountReduce) < 1)
        {
            Game.Debbug.Debbuger.ErrorDebug("StackCount can't be null");
            return 1;
        }
        return (StackCount - hunter._UserStat.BeastTransformationStackCountReduce);
    }
}
