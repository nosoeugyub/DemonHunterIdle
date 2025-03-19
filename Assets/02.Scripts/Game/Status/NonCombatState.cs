using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 전투 상태
/// </summary>
public class NonCombatState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.SetStatusMsg("비전투");
        // 비전투 상태에 진입할 때 필요한 초기화 작업을 수행합니다.

        //헌터 움직임 범위 제한 해제
        for(int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            DataManager.Instance.Hunters[i].UnLimitHunterPos();
        }

        ////장착한 헌터의 쿨다운 정지
        //List<Hunter> tmpHunterList = DataManager.Instance.GetEquippedHunters();

        //for (int i = 0; i < tmpHunterList.Count; i++)
        //{
        //    SkillManager.Instance.PauseAllHunterSkill(DataManager.Instance.GetSubClassUsingHunter(tmpHunterList[i]));
        //}

        //휴식 조건 충족했다면
        if(IdleModeRestCycleSystem.Instance.IsWaitForRest && (GameEventSystem.CurBossSqenunce == BossSqeunce.None || GameEventSystem.CurBossSqenunce == BossSqeunce.Die))
        {
            //휴식 시작
            GameEventSystem.GameRestModeSequence_GameEventHandler_Event(IdleModeRestCycleSequence.Cinematic);
        }



        Game.Debbug.Debbuger.Debug("Enter Non-Combat State");
    }

    public void Update()
    {
        // 비전투 상태에서의 게임 로직을 업데이트합니다.
    }

    public void Exit()
    {
        Game.Debbug.Debbuger.Debug("Exiting Non-Combat State");
        // 비전투 상태를 종료할 때 필요한 정리 작업을 수행합니다.

        //헌터 움직임 범위 제한 시작
        for(int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            DataManager.Instance.Hunters[i].LimitHunterPos(GroundCreatSystem.Instance.Groundlist[1], GameManager.Instance.hunterLimitMinZ, GameManager.Instance.hunterLimitMaxZ);
        }
    }
}
