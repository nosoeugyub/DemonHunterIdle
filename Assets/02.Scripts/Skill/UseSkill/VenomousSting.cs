using Game.Debbug;
using NSY;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 맹독침 스킬
/// </summary>
public class VenomousSting : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("투사체 개수")]
    [SerializeField] int VenomousStingNumber = 1;
    [Header("투사체 사이의 각도")]
    [SerializeField] float shotAngle = 30;
    [Header("범위공격시 최대 타겟 수(-1이면 사용 안 함)")]
    [SerializeField] int maxTarget = -1;
    [Header("범위공격시 타격 범위")]
    [SerializeField] float DamageRadius = 2f;
    [Header("테스트용 - 스킬 데미지 기준으로 데미지 적용 여부(비활성화시 적의 Hp를 이용해 피해를 줌)")]
    [SerializeField] bool isUseSkillDamage = false;
    [Header("적 hp사용 적용 비율 퍼센트")]
    [SerializeField] float damageHpRatio = 50f;

    private DamageInfo venomousStingDamageInfo = null;
    private int projectileId = 0; //중복 타격 방지용
    #endregion
   

    private Dictionary<Enemy,GameObject> skullEffects = new();

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        //스킬 데미지 정보 세팅
        venomousStingDamageInfo = new(0, skillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);
        //타입정하기
        var skillposlist = SpawnSkillArea(SkillCastingPositionType);
        Vector3 SpawnEffectPos = skillposlist.pos;
        SpawnEffectPos.y = hunter.FireTransform.position.y;

        //스킬 투사체 소환
        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(Tag.VenomousSting, hunter.FireTransform, VenomousStingNumber + hunter._UserStat.VenomousStingNumber, shotAngle);
        for (int i = 0; i < projectileList.Count; i++)
        {
            projectileList[i].gameObject.SetActive(false);
            projectileList[i].transform.position = SpawnEffectPos; 
            Vector3 targetPos = projectileList[i].transform.position + projectileList[i].transform.forward * 5f;
            if (TargetDirection == false)
            {
                targetPos = SpawnEffectPos;
                projectileList[i].transform.eulerAngles = Utill_Standard.Vector3Zero;
            }
            projectileList[i].SetProjectileWithoutTarget(hunter, venomousStingDamageInfo, targetPos, hunter.SearchLayer, projectileId);
            projectileList[i].OnAttackAction -= CorpseExplosion;
            projectileList[i].OnAttackAction += CorpseExplosion;
            projectileList[i].gameObject.SetActive(true);
        }
        projectileId = projectileId < 1100 ? projectileId + 1 : 1050; //일정 id 넘어가면 초기화
        SoundManager.Instance.PlayAudio(skillName);
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
    /// <summary>
    /// 맹독침에 피격시 사망시 범위공격 하도록 Action에 추가
    /// </summary>
    private void CorpseExplosion(List<IDamageAble> targets,Projectile projectile)
    {
        if (targets.Count <= 0) return;
        if(targets[0] is IEnemyDamageAble)
        {
            Enemy enemy = (targets[0] as IEnemyDamageAble).DamageAbleEnemy;
            if (skullEffects.ContainsKey(enemy))
            {
                return;
            }
            enemy.OnDieAction -= RangeAttack;
            enemy.OnDieAction += RangeAttack;

            //해골 이펙트
            GameObject fx = ObjectPooler.SpawnFromPool(Tag.SkullFx,Vector3.zero);
            fx.transform.SetParent(enemy.transform);
            fx.transform.localPosition = enemy.EffectOffset;
            skullEffects.Add(enemy,fx);
        }
    }
    /// <summary>
    /// 사망시 범위공격 시전 함수
    /// </summary>
    private void RangeAttack(Enemy enemy)
    {
        List<IEnemyDamageAble> hitEnemyList = Utill_Standard.FindAllObjectsInRadius<IEnemyDamageAble>(enemy.transform.position, (DamageRadius + hunter._UserStat.VenomousStingDamageRadius), hunter.SearchLayer, maxTarget);

        //해골 이펙트 삭제
        skullEffects[enemy].transform.SetParent(GameManager.Instance.ObjectPooler.transform);
        skullEffects[enemy].SetActive(false);
        skullEffects.Remove(enemy);
        //범위공격 이펙트
        GameObject fx = ObjectPooler.SpawnFromPool(Tag.VenomousStingDamageRadiusAttackFx, enemy.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        fx.transform.position = new Vector3(enemy.transform.position.x, 1, enemy.transform.position.z);
        fx.transform.localScale = (Vector3.one * 0.25f ) * (DamageRadius + hunter._UserStat.VenomousStingDamageRadius); //0.25는 이펙트 크기 보정값
        SoundManager.Instance.PlayAudio(Tag.VenomousStingDamageRadiusAttackFx); //사운드

        if (hitEnemyList.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < hitEnemyList.Count; i++)
        {
            if (hitEnemyList[i] == null || !hitEnemyList[i].CanGetDamage) //데미지를 못받거나 null이면
                hitEnemyList.Remove(hitEnemyList[i]);
        }

        // 중복을 제거
        hitEnemyList = hitEnemyList.Distinct().ToList();
        int maxTargetNum = (maxTarget == -1)?int.MaxValue : maxTarget;


        //스킬 데미지 정보 세팅
        if(isUseSkillDamage)
        {
            venomousStingDamageInfo = new(0, skillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);
        }
        else
            venomousStingDamageInfo = new(enemy._EnemyStat.HP * ( damageHpRatio  * 0.01f) , skillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);

        for (int i = 0; i < maxTargetNum; i++)
        {
            if (hitEnemyList.Count <= i) return;
            if(isUseSkillDamage)
            {
                //HunterStat.CalculateSkillDamage(hunter._UserStat, hitEnemyList[i].DamageAbleEnemy._EnemyStat, venomousStingDamageInfo);
            }
            hitEnemyList[i].Damaged(venomousStingDamageInfo);
        }

    }
 
}
