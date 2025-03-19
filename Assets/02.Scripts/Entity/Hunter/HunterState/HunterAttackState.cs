using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 공격하는 상태 구현
/// </summary>
public class HunterAttackState : HunterState
{
    protected Rigidbody rigid;  //참조 줄이기 위해 저장함

    protected List<IEnemyDamageAble> nearEnemyList = new();
    protected DamageInfo basicDamageInfo;
    protected NormalAttackStats hunterAttackStat;

    protected int projectileId = 0;
    public HunterAttackState(Hunter hunter) : base(hunter)
    {
        rigid = hunter.Rigid;

        basicDamageInfo = new(hunter.Power, hunter.IsMagicAtk,hunter);
        hunterAttackStat = StatManager.Instance.GetHunterNormalAttackStat(hunter._UserStat.SubClass);
    }

    public override void Enter()
    {
        GameStateMachine.Instance.ChangeState(new CombatState()); //전투시작


        rigid.velocity = Utill_Standard.Vector3Zero;
        rigid.constraints = rigid.constraints | RigidbodyConstraints.FreezeRotationY;
        hunter.NavMeshAgent.velocity = Utill_Standard.Vector3Zero;
        hunter.NavMeshAgent.enabled = false;
        hunter.AttackSpeed = HunterStat.AttackSpeedResult(hunter._UserStat);
    }
    public override void Update()
    {
        Rotate();
        //Game.Debbug.Debbuger.Debug(angle.ToString());
    }
    public override void Exit()
    {
        hunter.NavMeshAgent.enabled = true;
        rigid.constraints = rigid.constraints ^ RigidbodyConstraints.FreezeRotationY;
        rigid.velocity = Utill_Standard.Vector3Zero;
        hunter.MoveVec = Vector3.forward;
    }

    /// <summary>
    /// 공격 가능 여부
    /// </summary>
    public virtual bool CheckAttack()
    {
        return false;
    }
    /// <summary>
    /// 실제 공격 로직
    /// </summary>
    public virtual void OnAttackAnimation()
    {
    }

    /// <summary>
    /// 가장 가까운 상대 바라봄
    /// </summary>
    private void Rotate()
    {
        Vector2 forward = new Vector2(hunter.transform.position.z, hunter.transform.position.x);
        Vector2 steeringTarget = Vector2.zero;
        if (nearEnemyList.Count > 0)
        {
            steeringTarget = new Vector2(nearEnemyList[0].ObjectTransform.position.z, nearEnemyList[0].ObjectTransform.position.x);
        }

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        hunter.transform.eulerAngles = Vector3.up * angle;
    }
}
