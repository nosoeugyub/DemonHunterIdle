using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 움직이는 상태 구현
/// </summary>
public class HunterMoveState : HunterState
{
    protected bool isDestNull = false;
    protected NavMeshAgent navmeshAgent = null; // navMeshAgent 참조 줄이기 위해 저장함

    /// <summary>
    /// 상태 초기화 함수
    /// </summary>
    public HunterMoveState(Hunter hunter) : base(hunter) 
    {
        navmeshAgent = hunter.NavMeshAgent;
    }

    /// <summary>
    /// 움직임 제한 유무에 따라 최초 목적지 설정
    /// </summary>
    public override void Enter()
    {
        hunter.NavMeshAgent.enabled = true;
        SetDist();
    }

    /// <summary>
    /// 이동 상태일 때 Update에서 실행될 함수   
    /// </summary>
    public override void Update()
    {
        if (IdleModeRestCycleSystem.Instance.IsRestMode()) //휴식중이라면 움직일 수 없음.
        {
            IdleModeRestCycleSystem.Instance.SetHunterStateUsingSequence(hunter);
        }

        List<IDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IDamageAble>(hunter.transform.position, hunter.SearchRadius, hunter.SearchLayer);
        
        if (damageable.Contains(hunter))
            damageable.Remove(hunter);

        damageable.RemoveAll(x => !x.CanGetDamage);

        //IDamageAble attackAble = damageable.FindAll((target) => (target.ObjectTransform.position - transform.position).sqrMagnitude <= attackableRadius);
        IDamageAble closestObject = damageable
        .OrderBy(obj => Vector3.Distance(hunter.transform.position, obj.ObjectTransform.position)) // 거리순으로 정렬
        .FirstOrDefault(); // 첫 번째 요소 반환

        Rotate();
        
        // 가장 가까운 오브젝트가 있을 경우
        if (closestObject != null)
        {
            float closestDistance = Vector3.Distance(hunter.transform.position, closestObject.ObjectTransform.position);

            if (closestDistance <= hunter._UserStat.AttackRange)
            {
                hunter.ChangeFSMState(HunterStateType.Attack);
                return;
            }

            //다음 목적지 설정
            if (navmeshAgent.isOnNavMesh)
                navmeshAgent.SetDestination(closestObject.ObjectTransform.position);
            return;
        }
        // 목표 위치에 도착했는지 확인
        else if (navmeshAgent.isOnNavMesh && !navmeshAgent.pathPending && !navmeshAgent.hasPath /*<= navmeshAgent.stoppingDistance*/)
        {
            //다음 목적지 설정
            SetDist();
        }

        //목적지 지정에 실패했다면 계속 시도
        if(isDestNull)
        {
            SetDist();
        }
    }
    public override void Exit()
    {
        hunter.Rigid.velocity = Utill_Standard.Vector3Zero;
        if (navmeshAgent.isOnNavMesh)
            navmeshAgent.SetDestination(hunter.transform.position);
        navmeshAgent.velocity = Utill_Standard.Vector3Zero;
    }

    protected void Rotate()
    {
        Vector2 forward = new Vector2(hunter.transform.position.z, hunter.transform.position.x);
        Vector2 steeringTarget = new Vector2(hunter.NavMeshAgent.steeringTarget.z, hunter.NavMeshAgent.steeringTarget.x);

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        hunter.transform.eulerAngles = Vector3.up * angle;
    }
    protected virtual void SetDist()
    {
        if (!navmeshAgent.isOnNavMesh) return;
        if (GroundCreatSystem.Instance.NextArrivePos() == null)
        {
            isDestNull = true;
            return;
        }
        if (hunter.IsLimitMove)
            navmeshAgent.SetDestination(Vector3.forward * hunter.LimitGroundZ.y);
        else
            navmeshAgent.SetDestination(GroundCreatSystem.Instance.NextArrivePos().position + Vector3.forward * 5f + (hunter.XAxisOffset * Vector3.right));
        isDestNull = false;
    }
}
