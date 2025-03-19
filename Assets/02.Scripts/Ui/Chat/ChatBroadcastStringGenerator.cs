using BackEnd;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-08-26
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 채팅 알림 텍스트 포멧 생성하는 클래스
/// </summary>
public class ChatBroadcastStringGenerator
{
    public static string GenerateBossFirstClearString(int clearBoss)
    {
        if (BackendManager.Instance.IsLocal)
        {
            return string.Empty;
        }
        string tmp = string.Format(LocalizationTable.Localization("ChatBroadcast_FirstKillBoss"), Backend.UserNickName, ColorUtility.ToHtmlStringRGB(TextColorManager.Instance.ChatBroadcastColor2), clearBoss);

        return tmp;
    }
    public static string GenerateBossFirstClearString(string itemName)
    {
        if (BackendManager.Instance.IsLocal)
        {
            return string.Empty;
        }
        string tmp = string.Format(LocalizationTable.Localization("ChatBroadcast_Gacha"), Backend.UserNickName, ColorUtility.ToHtmlStringRGB(TextColorManager.Instance.ChatBroadcastColor2), itemName);

        return tmp;
    }
}
