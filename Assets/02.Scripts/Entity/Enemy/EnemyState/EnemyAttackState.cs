using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적이 공격을 하는 상태 구현
/// </summary>
public class EnemyAttackState : EnemyState
{
    protected List<IHunterDamageAble> nearTargetList = new();
    protected DamageInfo basicDamageInfo;

    public EnemyAttackState(Enemy enemy) : base(enemy)
    {
        basicDamageInfo = new(enemy.Power,false, enemy);
    }
    public override void Enter()
    {
        enemy.NavMeshAgent.enabled = false;
        enemy.Obstacle.enabled = true;
        if (enemy.NavMeshAgent.isOnNavMesh)
        {
            enemy.NavMeshAgent.SetDestination(enemy.transform.position);
        }
    }
    public override void Update()
    {
        Rotate();
        //Game.Debbug.Debbuger.Debug(angle.ToString());
    }
    public override void Exit()
    {
        enemy.Obstacle.enabled = false;
        enemy.NavMeshAgent.enabled = true;
    }

    /// <summary>
    /// 공격 가능 여부
    /// </summary>
    public virtual bool CheckAttack() { return false; }

    /// <summary>
    /// 실제 공격 로직
    /// </summary>
    public virtual void OnAttackAnimation() { }

    /// <summary>
    /// 가장 가까운 상대 바라봄
    /// </summary>
    protected void Rotate()
    {
        Vector2 forward = new Vector2(enemy.transform.position.z, enemy.transform.position.x);
        Vector2 steeringTarget = Vector2.zero;
        if (nearTargetList.Count > 0)
        {
            steeringTarget = new Vector2(nearTargetList[0].ObjectTransform.position.z, nearTargetList[0].ObjectTransform.position.x);
        }

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        enemy.transform.eulerAngles = Vector3.up * angle;
    }
}