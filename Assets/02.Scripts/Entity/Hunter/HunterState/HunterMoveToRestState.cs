using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-25
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 휴식하러 움직이는 상태 구현
/// </summary>
public class HunterMoveToRestState : HunterMoveState
{
    Vector3 targetPosition = Vector3.zero;
    public HunterMoveToRestState(Hunter hunter) : base(hunter)
    {
    }

    public override void Update()
    {
        Rotate();

        // 목표 위치에 도착했는지 확인
        if (navmeshAgent.isOnNavMesh && !navmeshAgent.pathPending && !navmeshAgent.hasPath /*<= navmeshAgent.stoppingDistance*/)
        {
            //도착하였으면 피로도 시스템에 알림
            IdleModeRestCycleSystem.Instance.HunterArraivalToCamp(hunter._UserStat.SubClass);
            return;
        }

        //목적지 지정에 실패했다면 계속 시도
        if (isDestNull)
        {
            SetDist();
        }
    }

    protected override void SetDist()
    {
        if (!navmeshAgent.isOnNavMesh) return;
        if (GroundCreatSystem.Instance.NextArriveGround() == null)
        {
            isDestNull = true;
            return;
        }
        targetPosition = GroundCreatSystem.Instance.NextArriveGround().IdleModeRestCharacterPos[(int)hunter._UserStat.SubClass].position;
        navmeshAgent.SetDestination(targetPosition);
        isDestNull = false;
    }
}
