using System.Collections;
using System.Collections.Generic;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적이 멈춰있는 상태 구현
/// </summary>
public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.NavMeshAgent.enabled = false;
    }

    public override void Update()
    {
        if(enemy.Target != null) //타겟이 있다면 이동 스테이트로 전환
        {
            enemy.ChangeFSMState(EnemyStateType.Move);
            return;
        }
        //탐색 성공했다면
        if (IsAnyTargetInRange())
        {
            //개별 탐지 이동/공격 하려면 주석 해제
            enemy.ChangeFSMState(EnemyStateType.Move);
            return;
        }
        //만약 이상한 위치에 있다면?
        if(enemy.gameObject.transform.position.y < 0)
        {
            enemy.EnemyDie(); //강제로 사망처리
        }
    }

    public override void Exit()
    {
        enemy.NavMeshAgent.enabled = true;
    }

    /// <summary>
    /// 탐색 가능 범위에 적이 들어왔는지 탐색
    /// </summary>
    /// <returns>탐색 성공 여부</returns>
    private bool IsAnyTargetInRange()
    {
        List<IHunterDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(enemy.transform.position, enemy.SearchRadius, enemy.SearchLayer);

        for (int i = 0; i < damageable.Count; i++)
        {
            if (!damageable[i].CanGetDamage)
                damageable.Remove(damageable[i]);
        }

        return damageable.Count > 0;
    }
}
