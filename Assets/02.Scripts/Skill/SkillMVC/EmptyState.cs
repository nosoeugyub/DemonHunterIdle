using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-10-28
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬슬롯 상태 관리 클래스
/// /// </summary>
public class EmptyState : SkillSlotState
{
    public EmptyState(BattleSkillUi _skillUi) : base(_skillUi) { }

    public override void Enter()
    {
        skillUi.Empty_Slot(); // Initialize empty slot
        skillUi.Skill = null;
    }

    public override void Update()
    {
        // 빈 슬롯 로직
    }

    public override void Exit()
    {
        // 상태 종료 시 처리
    }
}
