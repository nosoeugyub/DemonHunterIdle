using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적이 근거리 공격을 하는 상태 구현
/// </summary>
public class EnemyMeleeAttackState : EnemyAttackState
{
    public EnemyMeleeAttackState(Enemy enemy) : base(enemy)
    {
    }
    public override bool CheckAttack()
    {
        nearTargetList = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(enemy.transform.position, enemy._EnemyStat.AttackRange, enemy.SearchLayer);

        for (int i = 0; i < nearTargetList.Count; i++)
        {
            if (!nearTargetList[i].CanGetDamage)
                nearTargetList.Remove(nearTargetList[i]);
        }
        if (nearTargetList.Count <= 0)
        {
            return false;
        }
        return true;
    }
    public override void OnAttackAnimation()
    {
        if (nearTargetList.Count <= 0)
        {
            return;
        }
        (basicDamageInfo.damage, basicDamageInfo.addDamage, basicDamageInfo.isCri, basicDamageInfo.isdodge,basicDamageInfo.isStun , basicDamageInfo.isFreezing , basicDamageInfo.isPosion , basicDamageInfo.isSlow) = EnemyStat.DamageToPhysical(enemy._EnemyStat, (nearTargetList[0].DamageAbleHunter)._UserStat);
        nearTargetList[0].Damaged(basicDamageInfo);
    }
}
