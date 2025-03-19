using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShortageState : SkillSlotState
{
    public ManaShortageState(BattleSkillUi _skillUi) : base(_skillUi) { }

    public override void Enter()
    {
        // 마나 부족 UI 업데이트
        skillUi.ManaShortageImage.gameObject.SetActive(true);
    }

    public override void Update()
    {
        // 마나 부족 로직
        Debug.Log("마나 감지 호출되나..?");
    }

    public override void Exit()
    {
        skillUi.ManaShortageImage.gameObject.SetActive(false);
        // 상태 종료 시 처리
    }
}
