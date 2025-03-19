using Damageable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 회전돌격을 사용하는 중의 상태를 구현 (데미지/이펙트는 스킬 코드에)
/// </summary>
public class HunterWhirlwindState : HunterState
{
    NavMeshAgent navmeshAgent = null; // navMeshAgent 참조 줄이기 위해 저장함
    private List<IEnemyDamageAble> targets = new();
    private WhirlwindRush whirlwindRush = null;
    private IEnemyDamageAble curTarget = null;
    private int targetIndex = 0; // 현재 타겟 인덱스

    private float targetFindTimer = 0f; //적 탐색 타이머
    private float findTime = 1f; //적 탐색 타이머 초기값
    private bool isFindEnemies = false; //적 탐지중인지

    public HunterWhirlwindState(Hunter hunter) : base(hunter)
    {
        navmeshAgent = hunter.NavMeshAgent;
        whirlwindRush = SkillManager.Instance.whirlwindRush;
    }

    public override void Enter()
    {
        //GameStateMachine.Instance.ChangeState(new CombatState()); //전투시작
        targets = Utill_Standard.FindAllObjectsInRadius<IEnemyDamageAble>(hunter.transform.position, whirlwindRush.EnemySearchRange, hunter.SearchLayer);

        isFindEnemies = false;
    }
    public override void Update()
    {
        // 목표 위치에 도착했는지 확인
        if (navmeshAgent.isOnNavMesh && !navmeshAgent.pathPending && navmeshAgent.remainingDistance <= navmeshAgent.stoppingDistance)
        {
            MoveToNextTarget();
        }

        if (curTarget == null || !curTarget.CanGetDamage)
        {
            MoveToNextTarget();
        }

        if(targets.Count <= 0)
        {
            if(!isFindEnemies)
            {
                isFindEnemies = true;
                findTime = targetFindTimer;
            }
            findTime -= Time.fixedDeltaTime;
            if(findTime <= 0)
            {
                if (navmeshAgent.isOnNavMesh && !navmeshAgent.pathPending && !navmeshAgent.hasPath)
                {
                    MoveForward();
                }
                FindTargets();
                findTime = targetFindTimer;
            }

            //적을 찾았다면
            if(targets.Count != 0)
            {
                //현재 목적지(앞으로 향해 가던 목적지) 삭제
                if (navmeshAgent.isOnNavMesh)
                    navmeshAgent.SetDestination(hunter.transform.position);
            }
        }
    }

    private void MoveToNextTarget()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null || !targets[i].CanGetDamage) //데미지를 못받거나 null이면
            {
                targets.RemoveAt(i);
            }
        }

        if (targets.Count == 0)
        {
            return;
        }

        // 타겟 인덱스 순환
        if (targetIndex >= targets.Count)
        {
            // 타겟 리스트를 다시 거리 순으로 정렬
            targets = targets
                .OrderBy(obj => Vector3.Distance(hunter.transform.position, obj.ObjectTransform.position))
                .ToList();
            targetIndex = 0; // 인덱스 초기화
        }

        curTarget = targets[targetIndex];
        targetIndex++;

        if (navmeshAgent.isOnNavMesh)
        {
            navmeshAgent.SetDestination(curTarget.ObjectTransform.position);
        }
    }

    private void FindTargets()
    {
        var temp = Utill_Standard.FindAllObjectsInRadius<IEnemyDamageAble>(hunter.transform.position, whirlwindRush.EnemySearchRange, hunter.SearchLayer);
        if (temp.Count != 0)
        {
            isFindEnemies = false;
            targets.AddRange(temp);
        }
    }

    //적을 찾지 못했으면 앞으로 이동
    private void MoveForward()
    {
        if (!navmeshAgent.isOnNavMesh) return;
        if (hunter.IsLimitMove)
        {
            navmeshAgent.SetDestination(Vector3.forward * hunter.LimitGroundZ.y);
            return;
        }

        if (GroundCreatSystem.Instance.NextArrivePos() == null)
        {
            return;
        }
        navmeshAgent.SetDestination(GroundCreatSystem.Instance.NextArrivePos().position + Vector3.forward * 5f + (hunter.XAxisOffset * Vector3.right));
    }
}
