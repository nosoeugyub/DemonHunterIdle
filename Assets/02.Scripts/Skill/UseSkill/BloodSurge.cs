using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-10-16
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 혈파 스킬
/// </summary>
public class BloodSurge : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("투사체 개수")]
    [SerializeField] int BloodSurgeNumber = 1;
    [Header("투사체 사이의 각도")]
    [SerializeField] float bloodSurgeAngle = 15;
    [Header("분열되는 투사체 생성 이격거리")]
    [SerializeField] float BloodSurgeDiameter = 0.1f;
    #endregion

    private BloodSurgeData bloodSurgeData;
    private DamageInfo bloodSurgeDamageInfo = null;
    private int projectileId = 2050; //중복 타격 방지용
    private string projectileName = string.Empty; //원래 헌터 투사체의 이름이 무엇이었는지

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (!isInit) //스킬 초기화
        {
            isInit = true;
            hunter.ProjectileName = Tag.BloodSurge;
        }
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
        bloodSurgeData = (BloodSurgeData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount);
    }

    public override void Init_Skill()
    {
        isInit = false;

        if (hunter != null)
        {
            hunter.ProjectileName = hunter.GetDefaultProjectileName(); //평타 투사체 모양 원복
        }
    }

    public void SpawnBloodSurgeEffect(List<IDamageAble> targets, Projectile projectile)
    {
        if (!Utill_Math.Attempt(bloodSurgeData.CHGValue_CastingChance))
            return; //소환 안 함

        //스킬 데미지 정보 세팅
        bloodSurgeDamageInfo = new(0, bloodSurgeData.attackdamagetype[0] == Utill_Enum.AttackDamageType.Magic, hunter);
        SoundManager.Instance.PlayAudio(Tag.BloodSurge);
        //스킬 투사체 소환 
        projectile.transform.position = projectile.transform.position + (projectile.transform.up * BloodSurgeDiameter);
        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(Tag.BloodSurgeEffect, projectile.transform, BloodSurgeNumber + hunter._UserStat.SplitPiercingNumber, bloodSurgeAngle);
        for (int i = 0; i < projectileList.Count; i++)
        {
            projectileList[i].gameObject.SetActive(false);
            //Vector3 targetPos = projectileList[i].transform.position + projectileList[i].transform.forward * 5f;
            projectileList[i].SetProjectileWithoutTarget(hunter, bloodSurgeDamageInfo, Vector3.zero, hunter.SearchLayer, projectileId,bloodSurgeData);

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
        projectileId = projectileId < 3100 ? projectileId + 1 : 3050; //일정 id 넘어가면 초기화
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        bloodSurgeData = (BloodSurgeData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, bloodSurgeData.CHGValue_UseMP); //마나감소
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
