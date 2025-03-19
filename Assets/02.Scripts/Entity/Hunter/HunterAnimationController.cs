using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터의 애니메이션을 제어
/// </summary>
public class HunterAnimationController
{
    protected static int attackAnimationHash = Animator.StringToHash("Attack");
    protected static int walkAnimationHash = Animator.StringToHash("Walk");
    protected static int deathAnimationHash = Animator.StringToHash("Death");
    protected static int idleAnimationHash = Animator.StringToHash("Idle");
    protected static int startSkillAnimationHash = Animator.StringToHash("SkillStart");
    protected static int skillAnimationHash = Animator.StringToHash("Skill");
    protected static int skillTypeAnimationHash = Animator.StringToHash("SkillType");
    protected static int restAnimationHash = Animator.StringToHash("Rest");
    protected static int restStartAnimationHash = Animator.StringToHash("RestStart");

    private bool canChangeAnim = true;

    protected Animator animator;
    public HunterAnimationController(Animator animator) 
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
    /// 애니메이션을 바꾸지 못하도록 설정 (스킬때문에 제작)
    /// </summary>
    public void BlockChangeAnimation()
    {
        canChangeAnim = false;
    }
    /// <summary>
    /// 애니메이션을 바꿀 수 있도록 설정 (스킬때문에 제작)
    /// </summary>
    public void UnBlockChangeAnimation()
    {
        canChangeAnim = true;
    }
    /// <summary>
    /// 스테이트에 맞는 애니메이션 실행
    /// </summary>
    /// <param name="value">애니메이션 실행/멈춤 여부</param>
    public void SetAnimationWithState(HunterStateType state, bool value = true, int skillType = 0)
    {
        if (!canChangeAnim) return;
        // Debug.Log("animation state is changing " + state.ToString() + value + Time.time);
        bool isTrigger = false;
        int playHash = idleAnimationHash;
        switch (state)
        {
            case HunterStateType.Move:
            case HunterStateType.JoystickMove:
            case HunterStateType.MoveToRest:
            case HunterStateType.CheatJoystickMove:
                playHash = walkAnimationHash;
                break;
            case HunterStateType.Die:
                isTrigger = true;
                playHash = deathAnimationHash;
                break;
            case HunterStateType.Attack:
                if (animator.GetBool(skillAnimationHash)) //스킬중이라면
                    return;
                isTrigger = true;
                playHash = attackAnimationHash;
                break;
            case HunterStateType.Stop:
                playHash = idleAnimationHash;
                break;
            case HunterStateType.Whirlwind:
            case HunterStateType.Skill:
                animator.SetBool(skillAnimationHash, value);
                if(value)
                {
                    //Debug.Log("skill animation state true" + state.ToString() + Time.time);
                    animator.SetTrigger(startSkillAnimationHash);
                    animator.SetInteger(skillTypeAnimationHash, skillType);
                }
                return;
            case HunterStateType.Resting:
                playHash = restAnimationHash;
                if(value != false)
                    animator.SetTrigger(restStartAnimationHash);
                break;
        }

        if (isTrigger)
        {
            animator.SetTrigger(playHash);
            animator.SetBool(playHash,value);
        }
        else
            animator.SetBool(playHash,value);
    }
}
