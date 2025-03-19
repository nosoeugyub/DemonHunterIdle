using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-10-28
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬슬롯 상태 관리 클래스
/// /// </summary>
public class UsableState : SkillSlotState
{
    public UsableState(BattleSkillUi _skillUi) : base(_skillUi) { }

    public override void Enter()
    {
        skillUi.SetImageAlpha(skillUi.CooldownOverlay, 0);
        skillUi.StartCooldown();
    }

    public override void Exit()
    {
        skillUi.SetImageAlpha(skillUi.CooldownOverlay, 255);
        skillUi.StopCooldown();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
