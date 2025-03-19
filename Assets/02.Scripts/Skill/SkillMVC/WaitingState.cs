using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-10-28
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬슬롯 상태 관리 클래스
/// /// </summary>
public class WaitingState : SkillSlotState
{
    public WaitingState(BattleSkillUi _skillUi) : base(_skillUi) { }

    public override void Enter()
    {
        // 대기 중 UI 업데이트
        skillUi.ManaShortageImage.gameObject.SetActive(false);
        skillUi.Text.text = "대기 중";
    }

    public override void Update()
    {
        // 대기 중 로직
        // 예: 스킬 사용 가능 여부 체크
    }

    public override void Exit()
    {
        // 상태 종료 시 처리
    }
}
