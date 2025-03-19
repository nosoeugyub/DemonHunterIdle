using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using System;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 최대투여량 스킬
/// </summary>
public class MaximumDose : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수

    [Header("최대체력 비례해서 HP 채워주는 퍼센트")]
    public float healPercent;

    [Header("체력차는 간격")]
    [SerializeField] private float TickInterval;


    #endregion

    private Coroutine currentCoroutine;

    GameObject fx;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Dose());
    }
    IEnumerator Dose()
    {
        float _healPercent = healPercent * 0.01f; // healPercent 40일시 40% 증가

        float totalHPToAdd = hunter.MaxHp * _healPercent; // 총 증가시킬 HP 값
        float hpPerSecond = totalHPToAdd / SkillDuration; // 초당 증가시킬 HP 값
        float hpPerInterval = hpPerSecond * TickInterval; // 간격마다 증가시킬 HP 값 (여기서는 interval이 1이므로 hpPerSecond와 같음)

        float elapsedTime = 0f; // 경과 시간 초기화

        while (elapsedTime < SkillDuration)
        {
            if (hunter._UserStat.CurrentHp < hunter.MaxHp)
            {
                EffectSound();
                HunterStat.SetPlusHp(hunter._UserStat, hpPerInterval);
                hunter.hpuiview.UpdateHpBar(hunter._UserStat.HP, hunter._UserStat.CurrentHp);
            }

            // 시간 경과
            elapsedTime += TickInterval;

            // 다음 증가까지 대기
            yield return new WaitForSeconds(TickInterval);
        }

        // 남은 HP 증가량을 한 번 더 추가 (경과 시간 동안의 오차 보정)
        float remainingHP = totalHPToAdd - (hpPerSecond * elapsedTime);
        if (remainingHP > 0)
        {
            if (hunter._UserStat.CurrentHp < hunter.MaxHp)
            {
                EffectSound();
                HunterStat.SetPlusHp(hunter._UserStat, remainingHP);
                hunter.hpuiview.UpdateHpBar(hunter._UserStat.HP, hunter._UserStat.CurrentHp);
            }
        }
    }

    /// <summary>
    /// 이펙트 및 사운드 처리
    /// </summary>
    public void EffectSound()
    {
        fx = ObjectPooler.SpawnFromPool(Tag.MaximumDose, Vector3.zero);
        fx.transform.SetParent(hunter.transform);
        fx.transform.localPosition = hunter.EffectOffset;

        SoundManager.Instance.PlayAudio(skillName);
    }

   
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

}