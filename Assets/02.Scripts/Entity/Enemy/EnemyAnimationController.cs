using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;
/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적의 애니메이션을 제어
/// </summary>
public class EnemyAnimationController
{
    protected static int attackAnimationHash = Animator.StringToHash("Attack");
    protected static int deathAnimationHash = Animator.StringToHash("Death");
    protected static int idleAnimationHash = Animator.StringToHash("Idle");
    protected static int walkAnimationHash = Animator.StringToHash("Walk");
    
    protected Animator animator;
    public EnemyAnimationController(Animator animator)
    {
        this.animator = animator;
    }

    /// <summary>
    /// 애니메이터의 속도 조절
    /// </summary>
    public void SetSpeed(float speed)
    {
        animator.speed = speed;
    }

    /// <summary>
    /// 스테이트에 맞는 애니메이션 실행
    /// </summary>
    /// <param name="isStartState">스테이트 중지/시작 여부</param>
    public void SetAnimationWithState(EnemyStateType state, bool isStartState = true)
    {
        bool isTrigger = false;
        int playHash = idleAnimationHash;
        switch (state)
        {
            case EnemyStateType.Move:
                playHash = walkAnimationHash;
                break;
            case EnemyStateType.Die:
                isTrigger = true;
                playHash = deathAnimationHash;
                break;
            case EnemyStateType.Attack:
                isTrigger = true;
                playHash = attackAnimationHash;
                break;
            case EnemyStateType.Idle: //idle애니메이션
                {
                    playHash = idleAnimationHash;
                    animator.Play(playHash);
                    //SetTrigger나 SetBool해줄 필요 없어 return
                    return;
                }
            case EnemyStateType.Debuff: //바로멈추는거
                {
                    animator.enabled = !isStartState;
                    return;
                }
        }

        if (isTrigger)
            animator.SetTrigger(playHash);
        else
            animator.SetBool(playHash, isStartState);
    }
}
