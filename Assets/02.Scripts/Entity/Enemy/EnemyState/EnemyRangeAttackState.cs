using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적이 원거리 공격을 하는 상태 구현
/// </summary>
public class EnemyRangeAttackState : EnemyAttackState
{
    public EnemyRangeAttackState(Enemy enemy) : base(enemy)
    {
    }

    public override bool CheckAttack()
    {
        float AttackableRadius = enemy._EnemyStat.AttackRange;
        nearTargetList = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(enemy.transform.position, AttackableRadius, enemy.SearchLayer);

        for (int i = 0; i < nearTargetList.Count; i++)
        {
            if (!nearTargetList[i].CanGetDamage)
                nearTargetList.Remove(nearTargetList[i]);
        }
        if (nearTargetList.Count <= 0)
        {
            return false;
        }


        nearTargetList = nearTargetList
        .OrderBy(obj => (obj.ObjectTransform.position - enemy.transform.position).sqrMagnitude) // 거리순으로 정렬
        .ToList();

        return true;
    }
    /// <summary>
    /// 실제 공격 로직(투사체 발사)
    /// </summary>
    public override void OnAttackAnimation()
    {
        if (nearTargetList.Count <= 0)
        {
            enemy.ChangeFSMState(EnemyStateType.Move);
            return;
        }
        bool IsMagicAtk = enemy._EnemyStat.AttackDamageType == Utill_Enum.AttackDamageType.Physical ? false : true;
        basicDamageInfo = new(enemy.Power, IsMagicAtk, enemy);
        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(Tag.EnemyTestProjectile, enemy.FireTransform, 1, 15);

        for (int i = 0; i < projectileList.Count; i++)
        {
            LinearProjectile tmpPrg = projectileList[i] as LinearProjectile;
            tmpPrg.SetMoveSpeed(enemy._EnemyStat.ProjectileSpeed);

            //temp.SetDir(Vector3.forward);
            tmpPrg.SetProjectileWithTarget(enemy, basicDamageInfo, nearTargetList[0], enemy.SearchLayer, 0); 
        }
    }
}
