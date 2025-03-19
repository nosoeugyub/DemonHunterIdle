using Damageable;
using NSY;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-10-25
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : CheatManager-HunterMoveOff시 헌터의 상태 구현
/// 기본로직은 JoystickState와 비슷하나, 상태를 전환하는 조건이 다름
/// </summary>
public class HunterCheatJoyStickMoveMoveState : HunterJoyStickMoveState
{
    public HunterCheatJoyStickMoveMoveState(Hunter hunter) : base(hunter)
    {
    }

    public override void FixedUpdate()
    {

        if(IsFindTarget())
        {
            hunter.ChangeFSMState(HunterStateType.Attack);
            return;
        }

        if (CheatManager.Instance.HunterMoveOff == false)
        {
            hunter.ChangeFSMState(HunterStateType.Move);
            return;
        }

        if (joyStick.isClick == false) 
        {
            return;
        }
        Move();
    }

    /// <summary>
    /// moveState에서 타겟을 찾는 Update문의 코드를 일부 발췌해옴. 
    /// 추후 그 곳의 코드와 이곳의 코드가 다를시 통일해야 오류가 안 날 것임
    /// </summary>
    private bool IsFindTarget()
    {
        List<IDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IDamageAble>(hunter.transform.position, hunter.SearchRadius, hunter.SearchLayer);

        if (damageable.Contains(hunter))
            damageable.Remove(hunter);

        damageable.RemoveAll(x => !x.CanGetDamage);

        IDamageAble closestObject = damageable
       .OrderBy(obj => Vector3.Distance(hunter.transform.position, obj.ObjectTransform.position)) // 거리순으로 정렬
       .FirstOrDefault(); // 첫 번째 요소 반환

        if(closestObject == null)
            return false;

        float closestDistance = Vector3.Distance(hunter.transform.position, closestObject.ObjectTransform.position);

        if (closestDistance <= hunter._UserStat.AttackRange)
        {
            return true;
        }
        return false;
    }
}
