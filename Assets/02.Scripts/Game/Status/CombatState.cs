using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 전투 상태
/// </summary>
public class CombatState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.SetStatusMsg("전투");
        // 전투 상태에 진입할 때 필요한 초기화 작업을 수행합니다.
        //장착한 헌터의 쿨다운 재시작
        List<Hunter> tmpHunterList = DataManager.Instance.GetEquippedHunters();

        for (int i = 0; i < tmpHunterList.Count; i++)
        {
            SkillManager.Instance.ReStartAllHunterSkill(DataManager.Instance.GetSubClassUsingHunter(tmpHunterList[i]));
        }
        Game.Debbug.Debbuger.Debug("Enter Combat State");
    }

    public void Update()
    {
        // 전투 상태에서의 게임 로직을 업데이트합니다.
    }

    public void Exit()
    {
        Game.Debbug.Debbuger.Debug("Exiting Combat State");
        // 전투 상태를 종료할 때 필요한 정리 작업을 수행합니다.
    }
}
