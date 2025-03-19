using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 망치 스킬
/// </summary>
public class HammerSummon : BaseSkill
{ 
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정 변수

    [Header("망치 피격시 스턴 지속 시간")]
    [SerializeField] private float StunDuration;

    [Header("망치 피해범위")]
    [SerializeField] private float DamageRadius;

    [Header("스킬모션 이후 데미지 적용 딜레이")]
    [SerializeField] private float ImpactDelay;

    [Header("스킬모션 총 재생 시간")]
    [SerializeField] private float animationApplyTime = 1f;

    [Header("이펙트사이즈")]
    [SerializeField] private float EffectSize;




    #endregion


    private DamageInfo hammerDamageInfo = null;
    private Coroutine currentCoroutine;

    private HammerSummonData hammersummondata;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        Hammer();
    }

    public void Hammer()
    {
        isMotion = true;
        hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, true, (int)Utill_Enum.SkillAnimationType.HammerSummon);
        hunter.ChangeFSMState(Utill_Enum.HunterStateType.Skill);

    }
    public IEnumerator UseHammerSummon()
    {
        if (hunter.CanGetDamage) // 이 변수로 플레이어 상태를 조율함
        { 
            var skillposlist = SpawnSkillArea(SkillCastingPositionType);
            Vector3 SpawnEffectPos = skillposlist.pos;
            GameObject fx = ObjectPooler.SpawnFromPool(skillName, Vector3.zero);
            fx.transform.position = SpawnEffectPos;

            fx.transform.localScale = Utill_Standard.vector3One * CalculateMeteorSize(DamageRadius);
            // ImpactDelay 동안 대기
            // yield return new WaitForSeconds(ImpactDelay);
            hammerDamageInfo = new DamageInfo(0, skillDamageType == Utill_Enum.AttackDamageType.Physical, hunter);
            SoundManager.Instance.PlayAudio(skillName);
            ApplyDamage(SpawnEffectPos);
            if (StunDuration > 0)
            {
                ApplyStun(SpawnEffectPos);
            }
            yield return null;
        }
    }

    public override void Init_Skill()
    {
        hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, false);
        hunter.SetAnimationSpeed(1);
        hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
        hunter.UnBlockJoystickMove();
        isMotion = false;
        UnEquipWeapon();
    }



    public void EquipWeapon()
    {
        hunter.GetCostumeStlyer.SetPartOfCostume("GuardianHammer");
    }

    public void UnEquipWeapon()
    {
        hunter.GetCostumeStlyer.SetPartOfCostume(HunteritemData.GetHunteritemData(GameDataTable.Instance.HunterItem.Guardian, Utill_Enum.EquipmentType.Weapon).Name);

        hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, false);
        hunter.SetAnimationSpeed(1);
        hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
        isMotion = false;
    }

    private void ApplyDamage(Vector3 impactPosition)
    {
        // 데미지 반경 내의 모든 적 탐지
        Collider[] hitColliders = Physics.OverlapBox(impactPosition, new Vector3(DamageRadius, 0.1f, DamageRadius), Quaternion.identity, hunter.SearchLayer);
        foreach (var hitCollider in hitColliders)
        {
            IDamageAble target = hitCollider.GetComponent<IDamageAble>();
            if (target != null && target.CanGetDamage)
            {
                HunterStat.CalculateSkillDamage(hunter._UserStat, (target as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, hammerDamageInfo,hammersummondata);
                target.Damaged(hammerDamageInfo); // 데미지 적용
            }
        }
    }

    private void ApplyStun(Vector3 impactPosition)
    {
        // 스턴 반경 내의 모든 적 탐지
        Collider[] hitColliders = Physics.OverlapBox(impactPosition, new Vector3(DamageRadius, 0.1f, DamageRadius), Quaternion.identity, LayerMask.GetMask("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            Enemy target = hitCollider.GetComponent<Enemy>();
            if (target != null && target.CanGetDamage)
            {
                target.ApplyStatusEffect(new StunStatusEffect(Utill_Enum.Debuff_Type.Stun, StunDuration));
            }
        }
    }

    public float CalculateMeteorSize(float range)
    {
        float size = EffectSize * range;
        return size;
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }


    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        hammersummondata = (HammerSummonData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.HammerSummon, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, hammersummondata.CHGValue_UseMP); //마나감소
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
