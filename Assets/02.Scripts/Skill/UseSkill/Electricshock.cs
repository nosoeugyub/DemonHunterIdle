using Game.Debbug;
using Lightning;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 감전 스킬
/// </summary>

public class Electricshock : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("지속데미지 간격")]
    public float TickInterval;
    [Header("감전 줄기 총 개수")]
    public float ElectricshockNumber;
    [Header("1대상 최대 데미지")]
    public int MaxDmgTarget = 1000;
    #endregion

    public override event Action<float, bool> OnActiveSecondChanged;
    private DamageInfo damage = null;
    private ElectricshockData electricshockData = null;
    private Coroutine DamageCoroutine = null;

    private IEnumerator ChangingDefaultArrow()
    {
        //SoundManager.Instance.PlayAudio(skillName);
        OnActiveSecondChanged?.Invoke(electricshockData.CHGValue_SkillDuration, true);
        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);
        DamageCoroutine = StartCoroutine(UseEletricshock());
        yield return new WaitForSeconds(electricshockData.CHGValue_SkillDuration);
        OnActiveSecondChanged?.Invoke(0, false); 
        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);
    }

    IEnumerator UseEletricshock()
    {
        if (damage == null)
        {
            switch (electricshockData.attackdamagetype[0])
            {
                case Utill_Enum.AttackDamageType.Physical:
                    damage = new(0, false, hunter);
                    break;
                case Utill_Enum.AttackDamageType.Magic:
                    damage = new(0, true, hunter);
                    break;
                default:
                    break;
            }
        }

        List<SkillObj> particles = new();
        while (hunter.CanGetDamage) // 이 변수로 플레이어 상태를 조율함 
        {
            // 스킬 영역 생성 및 대상 목록 가져오기
            var skillposlist = SpawnSkillArea(SkillCastingPositionType);
            int targetsHit = 0;

            //감지된 적이 1마리 이상일 시
            if(skillposlist.entitylist.Count > 0)
            {
                CalculateDamage(skillposlist.entitylist.Count);
            }

            foreach (Entity currentTarget in skillposlist.entitylist)
            {
                if (targetsHit >= ElectricshockNumber + hunter._UserStat.ElectricshockNumber)
                    break;

                if (currentTarget != null)
                {
                    // 이펙트와 사운드 처리
                    Vector3 spawnEffectPos = currentTarget.transform.position;
                    SkillObj particle = ObjectPooler.SpawnFromPool(skillName, hunter.transform.position).GetComponent<SkillObj>();
                    Vector3 dir = (currentTarget.transform.position - hunter.transform.position).normalized;
                    particle.SetLinePosition(hunter.transform.position + dir * 0.1f, spawnEffectPos);
                    particles.Add(particle);
                    SoundManager.Instance.PlayAudio(skillName);

                    // 대상에게 피해를 입힙니다.
                    if (currentTarget is Hunter hunterTarget)
                    {
                        hunterTarget.Damaged(damage);
                    }
                    else if (currentTarget is Enemy enemyTarget)
                    {
                        enemyTarget.Damaged(SetDamageInfo(enemyTarget));
                    }

                    // 간격 대기
                    //yield return new WaitForSeconds(TickInterval);

                    targetsHit++;
                }
            }

            float delay = TickInterval;
            yield return new WaitForSeconds(delay);
            for(int i = 0; i < particles.Count; i++)
            {
                particles[i].gameObject.SetActive(false); // 감전 효과 비활성화
            }
            particles.Clear();

            //yield return Utill_Standard.WaitTimeOne; // 모든 대상을 처리한 후 추가 대기
        }
    }


    IEnumerator UnUseSkill()
    {
        // 버프가 끝난 후 공속을 원래대로 되돌립니다.
        //isbuff = false;
        //UserStat.Miuse_AttackSpeed(NSY.DataManager.Instance.Hunter.Orginstat, Attack_Speed_Value);
        yield return null;
    }
    /// <summary>
    /// 데미지기반 : 데미지타입  =  100   x 줄기 / 타겟팅 된 적 수
    /// 위 식을 기반으로 주어야할 데미지기반을 계산함
    /// </summary>
    /// <param name="targetedEnemyCount">타겟팅 된 적 수</param>
    private void CalculateDamage(int targetedEnemyCount)
    {
        //현재 감전 줄기 수
        int electricCount = (int)ElectricshockNumber + (int)hunter._UserStat.ElectricshockNumber;
        //타겟팅 된 적 수(실제로 때릴 적 수)
        int enemyCount = (targetedEnemyCount < electricCount) ? targetedEnemyCount : electricCount; //현재 감전 줄기 수 이상으로 때릴 수 없으니 비교 후 값 넣어줌
        float PhycisDamage = 0;
        float MagicDamage = 0;
        float TrueDamage = 0;

        //최댓값 적용된 스탯
        float stat = HunterStat.GetMaximumStat(HunterStat.GetUserStatPerOption(hunter._UserStat, electricshockData.optionliast[0], true), electricshockData.optionliast[0].ToString());
        //스탯 적용 계수 적용
        stat = stat * (electricshockData.DMGValue[0] * 0.01f);
        switch (electricshockData.attackdamagetype[0])
        {
            case AttackDamageType.Physical:
                PhycisDamage = (stat * electricCount)/ enemyCount;
                break;
            case AttackDamageType.Magic:
                MagicDamage = (stat * electricCount) / enemyCount;
                break;
            case AttackDamageType.PhysicalTrueDamage:
            case AttackDamageType.MagicTrueDamage:
                TrueDamage = (stat * electricCount) / enemyCount;
                break;
        }

        //계산한 값 넣어주기
        damage.PhycisDamageType = PhycisDamage;
        damage.MagaicDamageType = MagicDamage;
        damage.TrueDamageTpye = TrueDamage;
    }


    /// <summary>
    /// 데미지 기반을 바탕으로 현재 적의 수치를 이용해 적용할 데미지를 계산하고, 단일대상 최대 데미지를 적용시킴
    /// </summary>
    /// <param name="enemyTarget">데미지를 줄 타겟</param>
    /// <returns>계산을 마친 DamageInfo</returns>
    private DamageInfo SetDamageInfo(Enemy enemyTarget)
    {
        DamageInfo info = new(0,false,hunter);

        switch (electricshockData.attackdamagetype[0]) //데미지 타입에 따라 계산식 계산
        {
            case AttackDamageType.Physical:
            case AttackDamageType.PhysicalTrueDamage:
                info.isMagic = false;
                (info.ReflectRange, info.isInstanceKillBoss, info.damage, info.addDamage, info.CoupDamage, info.CoupAddDamage, info.isCoup, info.isCri, info.isdodge, info.isStun, info.isFreezing, info.isPosion, info.isSlow, info.isElectric, info.isChain) =
                HunterStat.DamageToPhysical(hunter._UserStat, enemyTarget._EnemyStat, EnemyClass: enemyTarget._EnemyStat.Class, skill: damage.skill, PycisDamage: damage.PhycisDamageType, MagicDamage: damage.MagaicDamageType, TrueDamage: damage.TrueDamageTpye);
                break;
            case AttackDamageType.Magic:
            case AttackDamageType.MagicTrueDamage:
                info.isMagic = true;
                (info.ReflectRange, info.isInstanceKillBoss, info.damage, info.addDamage, info.CoupDamage, info.CoupAddDamage, info.isCoup, info.isCri, info.isdodge, info.isStun, info.isFreezing, info.isPosion, info.isSlow, info.isElectric, info.isChain) =
                HunterStat.DamageToMagic(hunter._UserStat, enemyTarget._EnemyStat, EnemyClass: enemyTarget._EnemyStat.Class, skill: damage.skill, PycisDamage: damage.PhycisDamageType, MagicDamage: damage.MagaicDamageType, TrueDamage: damage.TrueDamageTpye);
                break;
        }

        if (info.damage > MaxDmgTarget)
            info.damage = MaxDmgTarget;
        return info;
    }

    public override void Init_Skill()
    {
        StopCoroutine(ChangingDefaultArrow());
        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);
    }
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(ChangingDefaultArrow());
        StartCoroutine(ChangingDefaultArrow());
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        electricshockData = (ElectricshockData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.Electricshock, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }

        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, electricshockData.CHGValue_UseMP); //마나감소
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
