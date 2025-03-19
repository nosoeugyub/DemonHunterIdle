using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using Random = UnityEngine.Random;
using Game.Debbug;
using Damageable;

/// <summary>
/// 작성일자   : 2024-07-22
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 래피드샷 스킬
/// </summary>
public class RapidShot : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("래피드샷 폭발 피해범위")]
    [SerializeField]
    float EffectSize = 2f;
    [Header("투사체 개수")]
    [SerializeField] int RapidShotNumber = 2;


    [Header("기다리는 시간")]
    public float CastingInterval = 1f;
    #endregion

    private DamageInfo rapidShotDamageInfo = null;
    private int projectileId = 0; //중복 타격 방지용
    private Coroutine currentCoroutine;


    RapidShotData rapidshotdata;
    public override float DamageRadius_ { get => EffectSize; set => EffectSize = value; }

    IEnumerator Shot()
    {
        ////나중에 스킬 애니메이션 넣어줘야함 
        //hunter.SetAnimation(Utill_Enum.HunterStateType.Stop, true);
        //hunter.StopHunter();
        //hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);

        //래피드샷 데미지 처리
        float dmg = HunterStat.PhysicalPowerResult(NSY.DataManager.Instance.Hunters[0]._UserStat, true) * (rapidshotdata.CHGValue_DMG_PhysicalPower_Physical / 100);

        rapidShotDamageInfo = new(dmg, rapidshotdata.attackdamagetype[0] == Utill_Enum.AttackDamageType.Magic, hunter);
        //타입정하기
        var skillposlist = SpawnSkillArea(SkillCastingPositionType);

        Vector3 SpawnEffectPos = skillposlist.pos;
        SpawnEffectPos.y = hunter.FireTransform.position.y;

        var delay = new WaitForSeconds(CastingInterval);

        //스킬 투사체 소환 
        for (int i = 0; i < RapidShotNumber; i++)
        {
            Projectile projectile = ObjectPooler.SpawnFromPool(Tag.RapidArrow, hunter.FireTransform.position).GetComponent<Projectile>();
            projectile.gameObject.SetActive(false);
            projectile.transform.position = SpawnEffectPos;
            Vector3 targetPos = projectile.transform.position + projectile.transform.forward * 5f;
            if (TargetDirection == false)
            {
                targetPos = SpawnEffectPos;
                projectile.transform.eulerAngles = Utill_Standard.Vector3Zero;
            }
            projectile.SetProjectileWithoutTarget(hunter, rapidShotDamageInfo, targetPos, hunter.SearchLayer, projectileId, rapidshotdata);
            projectile.gameObject.SetActive(true);
            projectile.OnAttackAction += Explosion; //타격시 범위공격

            projectileId = projectileId < 1450 ? projectileId + 1 : 1500; //일정 id 넘어가면 초기화
            yield return delay;
        }
        SoundManager.Instance.PlayAudio(skillName);
    }
    private void Explosion(List<IDamageAble> targets, Projectile projectile)
    {
        if (targets.Count <= 0) return;
        if (targets[0] is IEnemyDamageAble)
        {
            SoundManager.Instance.PlayAudio(Tag.RapidShot_Explosion);
            
            Enemy enemy = (targets[0] as IEnemyDamageAble).DamageAbleEnemy;
            GameObject fx = ObjectPooler.SpawnFromPool(Tag.RapidExplosion, enemy.transform.position);
            fx.transform.position = new Vector3(enemy.transform.position.x, 1, enemy.transform.position.z);
            fx.transform.localScale = new Vector3(EffectSize, EffectSize, EffectSize);

            List<Enemy> hitEnemies = Utill_Standard.FindEnemiesInArea(fx.transform.position, 0, EffectSize, 360);

            for(int i = 0; i< hitEnemies.Count; i++)
            {
                if (hitEnemies[i].CanGetDamage)
                {
                    HunterStat.CalculateSkillDamage(hunter._UserStat, hitEnemies[i]._EnemyStat, projectile.DamageInfo, rapidshotdata);
                    hitEnemies[i].Damaged(projectile.DamageInfo);
                }
            }
        }
    }
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Shot());
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
         rapidshotdata = (RapidShotData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.RapidShot, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, rapidshotdata.CHGValue_UseMP); //마나감소
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
