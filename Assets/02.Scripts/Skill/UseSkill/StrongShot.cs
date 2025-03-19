using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-22
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 정조준일격 스킬
/// </summary>
public class StrongShot : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수

    [Header("투사체 개수")]
    [SerializeField] int StrongShotNumber = 1;

    [Header("애니메이션 적용 시간")]
    [SerializeField] private float animationApplyTime  = 1f;

    [Header("발사체 발사 간격")]
    [SerializeField] private float CastingInterval = 1f;
    #endregion
    private StrongShotData strongShotData;
    private DamageInfo strongShotDamageInfo = null;

    private int projectileId = 0; //중복 타격 방지용

    public override void Init_Skill()
    {
        hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, false);
        hunter.SetAnimationSpeed(1);
        hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
        hunter.UnBlockJoystickMove();
        isMotion = false;
    }

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        Shot();
    }
    /// <summary>
    /// 투사체 발사 애니메이션 실행
    /// </summary>
    private void Shot()
    {
        isMotion = true;
        if (animationApplyTime > 0) //애니메이션이 실제 플레이 되었을 시에만 fsm 코드 호출
        {
            hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, true, (int)Utill_Enum.SkillAnimationType.StrongShot);
            hunter.ChangeFSMState(Utill_Enum.HunterStateType.Skill);

            float baseAnimationLength = 1.0f; // 기본 애니메이션 길이 
            float animationSpeed = (1 / animationApplyTime) / baseAnimationLength;

            // 애니메이션 속도 설정
            hunter.SetSkillAnimationSpeed(animationSpeed);
            hunter.BlockJoystickMove();
            hunter.BlockChangeAnimation();
        }
        else
        {
            StartCoroutine(SpawnStrongShotProjectile());
        }
    }

    /// <summary>
    /// 투사체 스폰, 대기 후 state 전환
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnStrongShotProjectile()
    {
        strongShotDamageInfo = new(0, strongShotData.attackdamagetype[0] == Utill_Enum.AttackDamageType.Magic, hunter);
        //타입정하기
        var skillposlist = SpawnSkillArea(SkillCastingPositionType);
        Vector3 SpawnEffectPos = skillposlist.pos;
        SpawnEffectPos.y = hunter.FireTransform.position.y;

        if (animationApplyTime > 0) //애니메이션이 실제 플레이 되었을 시에만 fsm 코드 호출
        {
            hunter.UnBlockChangeAnimation();
            hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, false);
            hunter.SetAnimationSpeed(1);
            hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
            hunter.UnBlockJoystickMove();
        }
        SoundManager.Instance.PlayAudio(SkillName);
        var delay = new WaitForSeconds(CastingInterval);

        //스킬 투사체 소환 
        for (int i = 0; i < StrongShotNumber; i++)
        {
            Projectile projectile = ObjectPooler.SpawnFromPool(Tag.StrongArrow, hunter.FireTransform.position).GetComponent<Projectile>();
            projectile.gameObject.SetActive(false);
            projectile.transform.position = SpawnEffectPos;
            Vector3 targetPos = projectile.transform.position + projectile.transform.forward * 5f;
            if (TargetDirection == false)
            {
                targetPos = SpawnEffectPos;
                projectile.transform.eulerAngles = Utill_Standard.Vector3Zero;
            }
            projectile.SetProjectileWithoutTarget(hunter, strongShotDamageInfo, targetPos, hunter.SearchLayer, projectileId,strongShotData);
            projectile.gameObject.SetActive(true);
            projectileId = projectileId < 1150 ? projectileId + 1 : 1100; //일정 id 넘어가면 초기화
            yield return delay;
        }
        isMotion = false;
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
        strongShotData = BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount) as StrongShotData;
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        strongShotData = (StrongShotData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.StrongShot, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, strongShotData.CHGValue_UseMP); //마나감소
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
