using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDeActiveState : SkillSlotState
{
    public SkillDeActiveState(BattleSkillUi _skillUi) : base(_skillUi) { }

    public override void Enter()
    {
        // 마나 부족 UI 업데이트
        skillUi.ActiveSetUi(skillUi.m_SubClass);
    }

    public override void Update()
    {
        //마나 부족 로직
    }

    public override void Exit()
    {
        
    }
}
