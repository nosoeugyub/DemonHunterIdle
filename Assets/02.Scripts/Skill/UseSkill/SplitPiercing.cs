using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 분열관통 스킬
/// </summary>
public class SplitPiercing : BaseSkill
{
    
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("분열되는 개수")]
    [SerializeField] int SplitPiercingNumber = 1;
    [Header("분열되는 투사체 사이의 각도")]
    [SerializeField] float splitAngle = 15;
    [Header("분열된 화살의 데미지 퍼센트")]
    [SerializeField] float SplitDamageRate = 50f;
    [Header("분열후 화살 관통 확률")]
    [SerializeField] float SplitPiercingChance = 30f;
    [Header("분열 확률")]
    [SerializeField] float SplitChance = 80f;
    [Header("분열되는 투사체 기본공격판정여부")]
    [SerializeField] bool BasicAttack = false;
    [Header("분열되는 투사체 생성 이격거리")]
    [SerializeField] float SplitArrowDiameter = 0.1f;
    #endregion

    public override event Action<float, bool> OnActiveSecondChanged;
    private SplitPiercingData splitPiercingData;
    private DamageInfo splitPiercingDamageInfo = null;
    private int projectileId = 2050; //중복 타격 방지용

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ChangingDefaultArrow());
    }


    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
        splitPiercingData = BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount) as SplitPiercingData;
    }
    public override void Init_Skill()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        if (hunter != null)
        {
            hunter.ProjectileName = hunter.GetDefaultProjectileName(); //평타 투사체 모양 원복
            OnActiveSecondChanged?.Invoke(0, false);
        }
    }

    private IEnumerator ChangingDefaultArrow()
    {
        //SoundManager.Instance.PlayAudio(skillName);
        OnActiveSecondChanged?.Invoke(splitPiercingData.CHGValue_SkillDuration, true);
        hunter.ProjectileName = Tag.SplitArrow;
        yield return new WaitForSeconds(splitPiercingData.CHGValue_SkillDuration);
        hunter.ProjectileName = hunter.GetDefaultProjectileName(); //평타 투사체 모양 원복
        OnActiveSecondChanged?.Invoke(0,false);
    }

    public void SpawnFregileArrow(List<IDamageAble> targets, Projectile projectile)
    {
        //분열 확률 통과 못하면
        if (!Utill_Math.Attempt(splitPiercingData.CHGValue_SplitChance))
            return; //소환 안 함

        //스킬 데미지 정보 세팅
        splitPiercingDamageInfo = new(0, projectile.DamageInfo.isMagic, hunter);
        splitPiercingDamageInfo.isNormalAttack = BasicAttack;
        SoundManager.Instance.PlayAudio(Tag.SplitPiercing_Split);
        //스킬 투사체 소환 
        projectile.transform.position = projectile.transform.position + (projectile.transform.up * SplitArrowDiameter);
        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(Tag.FragileArrow, projectile.transform, SplitPiercingNumber + hunter._UserStat.SplitPiercingNumber, splitAngle);
        for (int i = 0; i < projectileList.Count; i++)
        {
            projectileList[i].gameObject.SetActive(false);
            //Vector3 targetPos = projectileList[i].transform.position + projectileList[i].transform.forward * 5f;
            projectileList[i].SetProjectileWithoutTarget(hunter, splitPiercingDamageInfo, Vector3.zero, hunter.SearchLayer, projectileId,splitPiercingData);

            projectileList[i].OnAttackAction -= CheckSplitPiercingChance;
            projectileList[i].OnAttackAction += CheckSplitPiercingChance;
            projectileList[i].AdditionalDamageLogicFunc -= ReduceSplitArrowDamage;
            projectileList[i].AdditionalDamageLogicFunc += ReduceSplitArrowDamage;

            //데미지를 받은 적은 지금 생성하는 화살에 영향을 받지 않도록
            for (int j = 0; j < targets.Count; j++)
            {
                if (targets[j] != null && targets[j] is Enemy)
                    (targets[j] as Enemy).AddProjectileHit(projectileId, 1000);
            }
        }
        for (int i = 0; i < projectileList.Count; i++)
        {
                projectileList[i].gameObject.SetActive(true);
        }
        projectileId = projectileId < 2100 ? projectileId + 1 : 2050; //일정 id 넘어가면 초기화
    }

    /// <summary>
    /// 확률 체크하여 관통 여부 확인
    /// </summary>
    public void CheckSplitPiercingChance(List<IDamageAble> targets, Projectile projectile)
    {
        //확률 체크 통과 못했을 시에는
        if(!Utill_Math.Attempt(splitPiercingData.CHGValue_SplitPiercingChance))
        {
            //삭제
            projectile.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 모든 데미지가 SplitDamageRate 만큼 줄어듦
    /// </summary>
    public DamageInfo ReduceSplitArrowDamage(DamageInfo damageInfo)
    {
        if(damageInfo.damage > 0)
            damageInfo.damage *= (splitPiercingData.CHGValue_SplitDamageRate * 0.01f);
        if(damageInfo.addDamage > 0)
            damageInfo.addDamage *= (splitPiercingData.CHGValue_SplitDamageRate * 0.01f);
        if(damageInfo.CoupDamage > 0)
            damageInfo.CoupDamage *= (splitPiercingData.CHGValue_SplitDamageRate * 0.01f);
        if(damageInfo.CoupAddDamage > 0)
        damageInfo.CoupAddDamage *= (splitPiercingData.CHGValue_SplitDamageRate * 0.01f);
        return damageInfo;
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        splitPiercingData = (SplitPiercingData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.SplitPiercing, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, splitPiercingData.CHGValue_UseMP); //마나감소
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
