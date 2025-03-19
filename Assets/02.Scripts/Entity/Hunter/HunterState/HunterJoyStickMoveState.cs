using NSY;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 조이스틱에 의해 움직이는 상태 구현
/// </summary>
public class HunterJoyStickMoveState : HunterState
{
    protected FloatingJoystick joyStick;
    public HunterJoyStickMoveState(Hunter hunter) : base(hunter)
    {
        joyStick = hunter.JoyStick;
    }
    public override void Enter()
    {
        hunter.NavMeshAgent.enabled = false;
        hunter.MoveVec = Vector3.forward; 
        hunter.Rigid.interpolation = RigidbodyInterpolation.Interpolate;
    }
    public override void FixedUpdate()
    {
        if (joyStick.isClick == false )  //비전투시에도 조이스틱은안먹히게
        {
            hunter.ChangeFSMState(HunterStateType.Move);
            return;
        }
        if (!hunter.CanMoveJoystick || hunter.GetState() == HunterStateType.Die || GameStateMachine.Instance.CurrentState is NonCombatState)
        {
            hunter.ChangeFSMState(HunterStateType.Move);
            return;
        }

        Move();
    }

    public override void LateUpdate()
    {
        Vector3 newPosition = hunter.transform.position;
        //현재 LimitGround와 LimitXRange중 더 작은 수를 기준으로 고정 x 값을 결정함
        float clampMaxX = (hunter.LimitXRange > hunter.LimitGroundX.y) ? hunter.LimitGroundX.y : hunter.LimitXRange;
        float clampMinX = (-hunter.LimitXRange < hunter.LimitGroundX.x) ? hunter.LimitGroundX.x : -hunter.LimitXRange;

        newPosition.x = Mathf.Clamp(hunter.transform.position.x, clampMinX, clampMaxX);
        newPosition.z = Mathf.Clamp(hunter.transform.position.z, hunter.LimitGroundZ.x, hunter.LimitGroundZ.y);

        hunter.transform.position = newPosition;
    }
    public override void Exit()
    {
        hunter.transform.rotation = Quaternion.identity;
        hunter.Rigid.interpolation = RigidbodyInterpolation.None;
        hunter.Rigid.velocity = Utill_Standard.Vector3Zero; 
        hunter.Rigid.angularVelocity = Vector3.zero;
        hunter.MoveVec = Vector3.forward;
        hunter.NavMeshAgent.enabled = true;
    }

    protected void Move()
    {
        Vector3 moveVec = hunter.MoveVec;
        Rotate();
        float tmpX = hunter.transform.position.x;
        Vector3 newPosition = hunter.transform.position;

        if (moveVec.x + tmpX > hunter.LimitXRange || (hunter.IsLimitMove && moveVec.x + tmpX > hunter.LimitGroundX.y))
            moveVec.x = 0;
        if (moveVec.x + tmpX < -hunter.LimitXRange || (hunter.IsLimitMove && moveVec.x + tmpX < hunter.LimitGroundX.x))
            moveVec.x = 0;

        float tmpZ = hunter.transform.position.z;
        if (hunter.IsLimitMove && moveVec.z + tmpZ > hunter.LimitGroundZ.y)
            moveVec.z = 0;
        if (hunter.IsLimitMove && moveVec.z + tmpZ < hunter.LimitGroundZ.x)
            moveVec.z = 0;
        moveVec.y = 0;

        hunter.Rigid.velocity = moveVec * hunter.GetMoveSpeedByGameState() * 2f;
    }

    private void Rotate()
    {
        if (hunter.GetLastState() == HunterStateType.Whirlwind)
            return;
        var angle = Mathf.Atan2(joyStick.Horizontal, joyStick.Vertical) * Mathf.Rad2Deg;
        hunter.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
