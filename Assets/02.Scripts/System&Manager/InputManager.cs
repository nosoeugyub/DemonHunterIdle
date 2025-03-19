using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 단축키를 저장하고 가져올 수 있도록 관리
/// </summary>
public class InputManager : MonoSingleton<InputManager>
{
    [HideInInspector] public bool isShowHotKey;
    [HideInInspector] public KeyCode closeKey = KeyCode.Escape;
    [HideInInspector] public KeyCode speedUpKey = KeyCode.Period;
    [HideInInspector] public KeyCode speedDownKey = KeyCode.Comma;
    [HideInInspector] public KeyCode maxResourceKey = KeyCode.F;

    private List<KeyCode> currentHoldKey = new(); //현재 유저가 누르고 있는 키

    private void Reset()
    {
        closeKey = KeyCode.Escape;
        speedUpKey = KeyCode.Period;
        speedDownKey = KeyCode.Comma;
        maxResourceKey = KeyCode.F;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e == null || e.keyCode == KeyCode.None) return;
        
        if (e.type == EventType.KeyDown)
        {
            if (Input.GetKeyDown(e.keyCode))
            {
                if(!currentHoldKey.Contains(e.keyCode))
                {
                    currentHoldKey.Add(e.keyCode);
                }
            }
        }
        else if (e.type == EventType.KeyUp)
        {
            if (Input.GetKeyUp(e.keyCode))
            {
                if (currentHoldKey.Contains(e.keyCode))
                {
                    currentHoldKey.Remove(e.keyCode);
                }
            }
        }
    }
    /// <summary>
    /// 현재 이 키가 눌리고 있는 상태인지 유무 반환
    /// </summary>
    public bool IsHoldKey(KeyCode e)
    {
        return currentHoldKey.Contains(e);
    }
}
