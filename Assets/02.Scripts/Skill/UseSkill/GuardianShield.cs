using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// 작성일자   : 2024-07-24
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 수호 스킬
/// </summary>
public class GuardianShield : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]

    #region 특정 변수
    [Header("이펙트사이즈")]
    [SerializeField] private float EffectSize;
    #endregion

    List<Hunter> battleHunters;

    public override event Action<float, bool> OnActiveSecondChanged;
    private Dictionary<Hunter, GameObject> shieldEffects = new Dictionary<Hunter, GameObject>();
    private Coroutine currentCoroutine;
    private GuardianShieldData guardianShieldData;

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            foreach (var battleHunter in battleHunters)
            {
                RemoveShield(battleHunter);
            }

            // 이펙트 삭제
            foreach (var battleHunter in battleHunters)
            {
                RemoveBattleHunterShieldEffect(battleHunter);
            }
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Shield());
    }
    IEnumerator Shield()
    {
        SoundManager.Instance.PlayAudio(skillName);
        OnActiveSecondChanged?.Invoke(guardianShieldData.CHGValue_SkillDuration, true);
        
        battleHunters = DataManager.Instance.GetEquippedHunters();
        battleHunters.Remove(hunter);
        foreach (var battleHunter in battleHunters)
        {
            // 스킬 효과 적용
            ApplyShield(battleHunter);

            ApplyBattleHunterShieldEffect(battleHunter);
        }
        yield return new WaitForSeconds(guardianShieldData.CHGValue_SkillDuration);

        OnActiveSecondChanged?.Invoke(0, false);
        foreach (var battleHunter in battleHunters)
        {
            RemoveShield(battleHunter);
        }

        // 이펙트 삭제
        foreach (var battleHunter in battleHunters)
        {
            RemoveBattleHunterShieldEffect(battleHunter);
        }

        yield return null;
    }

    private void ApplyShield(Hunter targetHunter)
    {
        // 타겟 헌터에 실드 효과 적용
        targetHunter.OnGuardianShieldAction += RedirectDamage;
    }

    private void RemoveShield(Hunter targetHunter)
    {
        // 타겟 헌터에 실드 효과 제거
        targetHunter.OnGuardianShieldAction -= RedirectDamage;
    }

    private void RedirectDamage(Hunter targetHunter, DamageInfo info)
    {
        if (hunter != null && hunter != targetHunter)
        {
            //연출
            ApplyDamagedEffect();
            SoundManager.Instance.PlayAudio(Tag.GuardianShield_Damaged);

            IHunterDamageAble damageAble = hunter.DamageAbleHunter;

            float _damagePercent = guardianShieldData.CHGValue_DamageShieldRate * 0.01f;
            float redirectedDamage = info.damage * _damagePercent;
            float reducedDamage = info.damage - redirectedDamage;

            // 가디언이 대신 받는 피해

            DamageInfo guardianDamageInfo = new DamageInfo(redirectedDamage, info.isMagic, damageAble);
            hunter.Damaged(guardianDamageInfo);

            IHunterDamageAble hunterDamageAble = targetHunter.DamageAbleHunter;
            DamageInfo hunterDamageInfo = new DamageInfo(reducedDamage, info.isMagic, hunterDamageAble);

            targetHunter.OnGuardianShieldAction -= RedirectDamage;
            targetHunter.Damaged(hunterDamageInfo);
            targetHunter.OnGuardianShieldAction += RedirectDamage;
        }


    }

    #region 이펙트

    // 배틀 헌터에게 이펙트 적용
    private void ApplyBattleHunterShieldEffect(Hunter targetHunter)
    {
        GameObject battleEffect = ObjectPooler.SpawnFromPool(Tag.GuardianShield, targetHunter.transform.position);
        battleEffect.transform.SetParent(targetHunter.transform);
        battleEffect.transform.localScale = new Vector3(EffectSize, EffectSize, EffectSize);
        //보호막 이펙트 위치 조정
        battleEffect.transform.localPosition = Vector3.up * 1.2f;
        shieldEffects[targetHunter] = battleEffect;
    }

    // 배틀 헌터 이펙트 제거
    private void RemoveBattleHunterShieldEffect(Hunter targetHunter)
    {
        if (shieldEffects.TryGetValue(targetHunter, out GameObject battleEffect))
        {
            battleEffect.SetActive(false);
            battleEffect.transform.SetParent(null);
        }
    }

    private void ApplyDamagedEffect()
    {
        GameObject effect = ObjectPooler.SpawnFromPool(Tag.GuardianShield_Damaged, hunter.transform.position);
        //타격시 이펙트 위치 조정
        effect.transform.localPosition = hunter.transform.position + Vector3.up * 1.6f;
    }
    #endregion


    public override void Init_Skill()
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            foreach (var battleHunter in battleHunters)
            {
                RemoveShield(battleHunter);
            }

            // 이펙트 삭제
            foreach (var battleHunter in battleHunters)
            {
                RemoveBattleHunterShieldEffect(battleHunter);
            }
            StopCoroutine(currentCoroutine);
        }
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        guardianShieldData = (GuardianShieldData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.GuardianShield, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }

        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, guardianShieldData.CHGValue_UseMP); //마나감소
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
