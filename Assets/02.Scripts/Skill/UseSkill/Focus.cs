using NSY;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 집중 스킬
/// </summary>
public class Focus : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("최종적으로 남을 화살의 갯수")]
    [SerializeField] int finalArrowCount = 1;
    [Header("줄어든 화살 갯수만큼 물리공격력 수치")]
    [SerializeField] float physicalPowerPercent = 100;
    #endregion

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;
    private int arrowCountBeforeSkill = 0; //스킬 사용 전 헌터의 화살 갯수
    private float currentPhysicalPowerBuffAmount = 0; //현재 헌터가 받고있는 물리공격 버프의 양
    private string projectileName = string.Empty; //원래 헌터 투사체의 이름이 무엇이었는지
    private FocusParticle focusFx = null;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(UseFocus());
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    private IEnumerator UseFocus()
    {
        if(hunter.CanGetDamage)
        {
            if (isInit == false)
            {
                InitArrowCount();
                isInit = true;
            }
        }
        else
        {
            ResetFocus();
            isInit = false;
        }
        yield break;
    }

    /// <summary>
    /// 스킬 켜고 초기 1회만 작동
    /// </summary>
    private void InitArrowCount() 
    {
        //단일공격으로
        hunter.IsForcus = true;

    }

    //끝날때 호출
    private void ResetFocus()
    {
        //평소에 쏘는 버젼으로 
        hunter.IsForcus = false;
    }




    public override void Init_Skill()
    {
        ResetFocus();
        isInit = false;
    }
}
