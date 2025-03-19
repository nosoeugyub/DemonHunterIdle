using Game.Debbug;
using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 가시덫 스킬
/// </summary>
public class SpikeTrap : BaseSkill
{
   
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("투사체 개수")]
    [SerializeField] int SpikeTrapNumber = 1;
    [Header("투사체 사이의 각도")]
    [SerializeField] float shotAngle = 30;
    [Header("장판 범위")]
    [SerializeField] float DamageRadius = 2f;
    [Header("장판 기간")]
    [SerializeField] float aoeDuration = 2f;
    [Header("장판 진입시 이동속도 퍼센트 감소 수치")]
    [SerializeField] int speedPercentDebuff = 50;
    [Header("장판 진입시  물리 방어도 퍼센트 디버프")]
    [SerializeField] int defensePercentDebuff = 20;
    #endregion
    

    public int SpeedDebuffAmount => speedPercentDebuff;
    public int DefenseDebuffAmount => defensePercentDebuff;
    public float AoeDuration => aoeDuration;


    private DamageInfo spikeTrapDamageInfo = null;
    private int projectileId = 0; //중복 타격 방지용
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        //스킬 데미지 정보 세팅
        spikeTrapDamageInfo = new(0, skillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);
        //타입정하기
        var skillposlist = SpawnSkillArea(SkillCastingPositionType);
        Vector3 SpawnEffectPos = skillposlist.pos;
        SpawnEffectPos.y = hunter.FireTransform.position.y;

        //스킬 투사체 소환 
        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(Tag.SpikeTrap, hunter.FireTransform, SpikeTrapNumber + hunter._UserStat.SpikeTrapNumber, shotAngle);
        for (int i = 0; i < projectileList.Count; i++)
        {
            projectileList[i].gameObject.SetActive(false);
            projectileList[i].transform.position = SpawnEffectPos;
            Vector3 targetPos = projectileList[i].transform.position + projectileList[i].transform.forward * 5f;
            if(TargetDirection== false)
            {
                targetPos = SpawnEffectPos;
                projectileList[i].transform.eulerAngles = Utill_Standard.Vector3Zero;
            }
            projectileList[i].SetProjectileWithoutTarget(hunter, spikeTrapDamageInfo, targetPos, hunter.SearchLayer, projectileId);
            projectileList[i].OnAttackAction -= SpawnTrap;
            projectileList[i].OnAttackAction += SpawnTrap;
            projectileList[i].gameObject.SetActive(true);
        }
        projectileId = projectileId < 1150 ? projectileId + 1 : 1100; //일정 id 넘어가면 초기화
        SoundManager.Instance.PlayAudio(skillName);
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }


    private void SpawnTrap(List<IDamageAble> targets,Projectile projectile)
    {
       if (targets.Count <= 0) return;
        if (targets[0] is IEnemyDamageAble)
        {
            Enemy enemy = (targets[0] as IEnemyDamageAble).DamageAbleEnemy;
            GameObject fx = ObjectPooler.SpawnFromPool(Tag.SpikeTrapAOE, enemy.transform.position);
            fx.transform.position = new Vector3(enemy.transform.position.x, 1, enemy.transform.position.z);
            fx.transform.localScale = Vector3.one * (DamageRadius + hunter._UserStat.SpikeTrapDamageRadius);
            SoundManager.Instance.PlayAudio(Tag.SpikeTrapAOE);
        }
    }

}
