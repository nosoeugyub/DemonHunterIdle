using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-10-28
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬슬롯 상태 관리 클래스
/// /// </summary>
public abstract class SkillSlotState 
{
    protected BattleSkillUi skillUi; // Skill UI 참조

    public SkillSlotState(BattleSkillUi skillUi)
    {
        this.skillUi = skillUi;
    }

    public abstract void Enter(); // 상태에 진입할 때 호출
    public abstract void Update(); // 상태 업데이트
    public abstract void Exit(); // 상태에서 나갈 때 호출
}
