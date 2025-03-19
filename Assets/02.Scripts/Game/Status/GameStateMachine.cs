using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 게임상태를관리하는 중계자
/// </summary>
public class GameStateMachine 
{
   

    // 싱글톤 인스턴스
    private static GameStateMachine instance;

    // 인스턴스에 접근하기 위한 프로퍼티
    public static GameStateMachine Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameStateMachine();
            }
            return instance;
        }
    }

    // private 생성자 (외부에서 인스턴스 생성 불가)
    private GameStateMachine() { }

    // 현재 상태를 외부에서 접근할 수 있도록 합니다.
    private IGameState currentState;
    public IGameState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }
  
    public void ChangeState(IGameState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
}
