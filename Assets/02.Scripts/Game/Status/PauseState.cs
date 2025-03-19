using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 일시정지 상태
/// </summary>
public class PauseState : IGameState
{
    public void Enter()
    {
        Debug.Log("Entering Pause State");
        Time.timeScale = 0f; // 게임을 일시정지
    }

    public void Update()
    {
        // 일시정지 상태에서는 게임 로직 업데이트가 필요하지 않습니다.
    }

    public void Exit()
    {
        Debug.Log("Exiting Pause State");
        Time.timeScale = 1f; // 게임 재개
    }
}
