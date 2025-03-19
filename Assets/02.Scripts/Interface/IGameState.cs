using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :상태 인터페이스 
/// </summary>

public interface IGameState 
{
    void Enter();
    void Exit();
    void Update();

}
