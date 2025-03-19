using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 연타 스킬
/// </summary>
public class ContinuousHit : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("연타시 필요 스택")]
    [SerializeField] int StackCount = 5;
    [Header("연타 최대 개수")]
    [SerializeField] int ContinuouHitNumber = 2;
    [Header("공격속도 퍼센트 증가 수치")]
    [SerializeField] float AttackSpeedPercent = 300;
    #endregion

    public override event System.Action<int> OnStackChanged;
  
    private bool isReciveBuff = false; //현재 이 버프를 받을 수 있는지
    private int curBuffStack = 0;
    private int curContinuousHitAmount = 0; //현재 연타 횟수
    
    private float curBuffAmount = 0f; //현재 버프 부여 양

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (!isInit) //스킬 초기화
        {
            isInit = true;
            isReciveBuff = false;
            curBuffStack = 0;
            curContinuousHitAmount = 0;
            hunter.OnNormalAttackAction -= IncreaseStack;
            hunter.OnNormalAttackAction += IncreaseStack;
        }
        //CheckStack();
    }
    public override void Init_Skill()
    {
        if(curBuffAmount!= 0)
            RemoveBuff();
        isInit = false;
        isReciveBuff = false;
        curBuffStack = 0;
        curContinuousHitAmount = 0;
        hunter.OnNormalAttackAction -= IncreaseStack;
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    private void IncreaseStack(Hunter hunter)
    {
        if (!isReciveBuff) //연타 상태의 평타는 계산 안 함
        {
            curBuffStack++;
            OnStackChanged?.Invoke(curBuffStack); // 이벤트 호출
        }
        else
        {
            OnStackChanged?.Invoke(0); // 이벤트 호출
            curContinuousHitAmount++;
        }

        if (curBuffStack >= GetStackNum())
            CheckStack();

        if (curContinuousHitAmount > ContinuouHitNumber)
            RemoveBuff();
    }
    private void CheckStack()
    {
        if (curBuffStack >= GetStackNum())
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
            ApplayBuff();
        }
    }
    private void ApplayBuff()
    {
        curBuffAmount = (AttackSpeedPercent-100);
        HunterStat.UseSkillAddUserStat(hunter._UserStat, curBuffAmount, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }
    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, curBuffAmount, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
        curBuffStack = 0;
        curContinuousHitAmount = 0;
        curBuffAmount = 0;

        isReciveBuff = false;
    }
    private int GetStackNum()
    {
        if ((StackCount - hunter._UserStat.ContinuousHitStackCountReduce) < 1)
        {
            Game.Debbug.Debbuger.ErrorDebug("StackCount can't be null");
            return 1;
        }
        return (StackCount - hunter._UserStat.ContinuousHitStackCountReduce);
    }
}