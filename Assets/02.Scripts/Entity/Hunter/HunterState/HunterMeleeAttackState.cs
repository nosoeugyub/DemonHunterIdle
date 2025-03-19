using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 근거리 공격하는 상태 구현
/// </summary>
public class HunterMeleeAttackState : HunterAttackState
{
    List<IEnemyDamageAble> hitEnemyList = new();
    IEnemyDamageAble target = null;
    public HunterMeleeAttackState(Hunter hunter) : base(hunter)
    {
    }

    public override bool CheckAttack()
    {
        nearEnemyList = Utill_Standard.FindAllObjectsInRadius<IEnemyDamageAble>(hunter.transform.position, hunter._UserStat.AttackRange, hunter.SearchLayer);

        for (int i = 0; i < nearEnemyList.Count; i++)
        {
            if (!nearEnemyList[i].CanGetDamage)
                nearEnemyList.Remove(nearEnemyList[i]);
        }
        if (nearEnemyList.Count <= 0)
        {
            return false;
        }
        if(nearEnemyList.Count > 0)
            target = nearEnemyList[0];
        return true;
    }

    public override void OnAttackAnimation()
    {
        hitEnemyList = Utill_Standard.FindAllObjectsInRadius<IEnemyDamageAble>(hunter.transform.position + (hunter.transform.forward * hunterAttackStat.ApplicationDistance), hunterAttackStat.DamageRadius, hunter.SearchLayer,hunterAttackStat.MaxTarget);
        if (hitEnemyList.Count <= 0)
        {
            return;
        }

        hitEnemyList.Add(target); //첫 타겟은 무조건 hit

        for (int i = 0; i < hitEnemyList.Count; i++)
        {
            if (hitEnemyList[i] == null || !hitEnemyList[i].CanGetDamage) //데미지를 못받거나 null이면
                hitEnemyList.Remove(hitEnemyList[i]);
        }

        // 중복을 제거
        hitEnemyList = hitEnemyList.Distinct().ToList();

        for (int i = 0; i < hunterAttackStat.MaxTarget; i++)
        {
            if (hitEnemyList.Count <= i) return;
            basicDamageInfo.isNormalAttack = true;
            (basicDamageInfo.ReflectRange , basicDamageInfo.isInstanceKillBoss ,basicDamageInfo.damage, basicDamageInfo.addDamage, basicDamageInfo.CoupDamage, basicDamageInfo.CoupAddDamage, basicDamageInfo.isCoup, basicDamageInfo.isCri, basicDamageInfo.isdodge, basicDamageInfo.isStun , basicDamageInfo.isFreezing, basicDamageInfo.isPosion , basicDamageInfo.isSlow , basicDamageInfo.isElectric, basicDamageInfo.isChain) = 
                HunterStat.DamageToPhysical(hunter._UserStat, (hitEnemyList[i].DamageAbleEnemy)._EnemyStat,isNormalAttack:basicDamageInfo.isNormalAttack);
            hitEnemyList[i].Damaged(basicDamageInfo);
        }
    }
}
