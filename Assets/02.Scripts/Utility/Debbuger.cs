using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 게임공용디버거
/// /// </summary>

namespace Game.Debbug
{
    public static class Debbuger
    {
        public static void Debug(string msg)
        {
           
            UnityEngine.Debug.Log(msg);
        }


        public static void ErrorDebug(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }
    }

}

