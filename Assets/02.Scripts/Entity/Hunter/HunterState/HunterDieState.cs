using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 사망한 상태 구현
/// </summary>
public class HunterDieState : HunterState
{
    public HunterDieState(Hunter hunter) : base(hunter)
    {
    }
    public override void Enter()
    {
        hunter.NavMeshAgent.velocity = Utill_Standard.Vector3Zero;
        hunter.NavMeshAgent.enabled = false;
        hunter.Rigid.isKinematic = true;
        hunter.Col.enabled = false;
    }
}
