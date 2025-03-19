using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적이 움직이는 상태 구현
/// </summary>
public class EnemyMoveState : EnemyState
{
    NavMeshAgent navmeshAgent = null; // navMeshAgent 참조 줄이기 위해 저장함
    IHunterDamageAble target = null;
 
    public EnemyMoveState(Enemy enemy) : base(enemy)
    {
        navmeshAgent = enemy.NavMeshAgent;
    }

    public override void Enter()
    {
        target = (IHunterDamageAble)enemy.Target;
    }

    public override void FixedUpdate()
    {
        if (enemy._EnemyStat == null) return;

        List<IHunterDamageAble> damageable = FindDamageableTargets();
        
        //타겟이 존재한다면
        if(enemy.Target != null && enemy.Target.CanGetDamage)
        {
            damageable.Clear();
            damageable.Add(target);
        }
        else
        {
            enemy.Target = target; //타겟이 없거나, 죽었다면 현재 탐색한 타겟을 넣어줌
            
            // 탐지된 객체가 없으면 대기 상태로 전이
            if (damageable.Count == 0)
            {
                enemy.ChangeFSMState(EnemyStateType.Idle);
                return;
            }
        }

        // 공격 가능한 객체가 있으면 공격 상태로 전이
        if (IsAnyTargetInRange(damageable))
        {
            enemy.ChangeFSMState(EnemyStateType.Attack);
            return;
        }
        // 네비메쉬에 매치되어있으면 다음 목적지 설정
        if (navmeshAgent.isOnNavMesh && target != null)
        {
            navmeshAgent.SetDestination(target.ObjectTransform.position);
        }

        Rotate();
    }

    public override void Exit()
    {
        enemy.Rigid.velocity = Utill_Standard.Vector3Zero;
        target = null;

        if (navmeshAgent.isOnNavMesh)
        {
            navmeshAgent.velocity = Utill_Standard.Vector3Zero;
        }
    }

    private List<IHunterDamageAble> FindDamageableTargets()
    {
        //개별 탐지 이동/공격 하려면 주석 해제
        List<IHunterDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(enemy.transform.position, enemy.SearchRadius, enemy.SearchLayer);
        //단체 탐지 이동/공격 하려면 주석 해제
        //List<IHunterDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(transform.position, 15, searchLayer);

        // 탐지된 객체 중 공격 가능한 객체 필터링
        for (int i = 0; i < damageable.Count; i++)
        {
            if (!damageable[i].CanGetDamage)
                damageable.Remove(damageable[i]);
        }

        if(target == null && damageable.Count != 0)
        {
            target = damageable
            .OrderBy(obj => Vector3.Distance(enemy.transform.position, obj.ObjectTransform.position)) // 거리순으로 정렬
            .FirstOrDefault();
        }

        return damageable;
    }

    /// <summary>
    /// 공격 범위 내의 객체 필터링
    /// </summary>
    private bool IsAnyTargetInRange(List<IHunterDamageAble> targets)
    {
        return targets.Any(target => Vector3.Distance(enemy.transform.position, target.ObjectTransform.position) <= enemy._EnemyStat.AttackRange);
    }

    /// <summary>
    /// navMeshAgent를 이용해 움직인 값으로 방향을 구해 조절하는 함수  
    /// </summary>
    private void Rotate()
    {
        Vector2 forward = new Vector2(enemy.transform.position.z, enemy.transform.position.x);
        Vector2 steeringTarget = new Vector2(navmeshAgent.steeringTarget.z, navmeshAgent.steeringTarget.x);

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        enemy.transform.eulerAngles = Vector3.up * angle;
    }
}
