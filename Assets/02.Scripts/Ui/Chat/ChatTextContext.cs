using EnhancedScrollerDemos.Chat;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-08-29
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 아무 정보가 없을 시 포멧을 설정하는 클래스
/// </summary>
public class NoneChatStrategy : IStrategy<ChatCellView>
{
    public void DoAlgorithm(ChatCellView data)
    {
        data.ChatText.alignment = TMPro.TextAlignmentOptions.Left;
        data.ChatText.text = "";
        data.ChatText.color = TextColorManager.Instance.DefaultChatColor;
    }
}

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 다른 유저가 보낸 채팅 셀의 포멧을 설정하는 클래스
/// </summary>
public class DefaultChatStrategy : IStrategy<ChatCellView>
{
    public void DoAlgorithm(ChatCellView data)
    {
        string chat = string.Format(Tag.ChatFormatStr,
            ColorUtility.ToHtmlStringRGB(TextColorManager.Instance.ChatRankColor), //랭크 순위 색
            string.Format(LocalizationTable.Localization("ChatFormat_Rank"),
            (data.ChatCellData.ranking > 0) ? data.ChatCellData.ranking.ToString("D2"):"-"), //랭킹표시
            data.ChatCellData.nickName, //채팅 친 유저
            data.ChatCellData.message //채팅 내용
            );
        data.ChatText.alignment = TMPro.TextAlignmentOptions.Left;
        data.ChatText.text = chat;
        data.ChatText.color = TextColorManager.Instance.DefaultChatColor;
    }
}

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 내가 보낸 채팅 셀의 포멧을 설정하는 클래스
/// </summary>
public class MyChatStrategy : IStrategy<ChatCellView>
{
    public void DoAlgorithm(ChatCellView data)
    {
        string chat = string.Format(Tag.ChatFormatStr,
            ColorUtility.ToHtmlStringRGB(TextColorManager.Instance.ChatRankColor), //랭크 순위 색
            string.Format(LocalizationTable.Localization("ChatFormat_Rank"),
            (data.ChatCellData.ranking > 0) ? data.ChatCellData.ranking.ToString("D2") : "-"), //랭킹표시
            data.ChatCellData.nickName, //채팅 친 유저
            data.ChatCellData.message //채팅 내용
            );
        data.ChatText.alignment = TMPro.TextAlignmentOptions.Left;
        data.ChatText.text = chat;
        data.ChatText.color = TextColorManager.Instance.MyChatColor;
    }
}

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 뒤끝 콘솔에서 보낸 운영자 채팅 셀의 포멧을 설정하는 클래스
/// </summary>
public class ConsoleChatStrategy : IStrategy<ChatCellView>
{
    public void DoAlgorithm(ChatCellView data)
    {
        string chat = string.Format(LocalizationTable.Localization("ChatFormat_Admin"),
            data.ChatCellData.message //채팅 내용
            );
        data.ChatText.alignment = TMPro.TextAlignmentOptions.Left;
        data.ChatText.text = chat;
        data.ChatText.color = TextColorManager.Instance.ConsoleChatColor;
    }
}

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 안내 타입(채팅 최대수 제한 안내 등)의 채팅 셀의 포멧을 설정하는 클래스
/// </summary>
public class NotificationChatStrategy : IStrategy<ChatCellView>
{
    public void DoAlgorithm(ChatCellView data)
    {
        string notification = string.Format(LocalizationTable.Localization("ChatFormat_Notification"), LocalizationTable.Localization(data.ChatCellData.message));

        data.ChatText.alignment = TMPro.TextAlignmentOptions.Left;
        data.ChatText.text = notification;
        data.ChatText.color = TextColorManager.Instance.NotificationChatColor;
    }
}

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 알림타입(유저 희귀 아이템 획득 알림 등)의 채팅 셀의 포멧을 설정하는 클래스
/// </summary>
public class ChatBroadcastChatStrategy : IStrategy<ChatCellView>
{
    public void DoAlgorithm(ChatCellView data)
    {
        data.ChatText.text = data.ChatCellData.message;
        data.ChatText.color = GetChatColorByChatBroadcastType(data.ChatCellData.alertType);
        if (data.IsRecentChat == false)
            data.ChatText.alignment = TMPro.TextAlignmentOptions.Center;
    }

    private Color GetChatColorByChatBroadcastType(ChatBroadcastType alertChatType)
    {
        switch (alertChatType)
        {
            case ChatBroadcastType.Boss_FirstClear:
                return TextColorManager.Instance.ChatBroadcastColor1;
            case ChatBroadcastType.Item_Drop:
                return TextColorManager.Instance.ChatBroadcastColor1;
            case ChatBroadcastType.Gacha:
                return TextColorManager.Instance.ChatBroadcastColor1;
            case ChatBroadcastType.None:
            default:
                return Color.white;
        }
    }
}

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 전략 패턴을 사용하여 채팅 셀의 타입에 따라 각자 포멧을 다르게 지정해줄 수 있도록 연결해주는 클래스
/// </summary>
public class ChatTextContext
{
    private IStrategy<ChatCellView> strategy;

    public ChatTextContext() { }
    public ChatTextContext(IStrategy<ChatCellView> strategy)
    {
        this.strategy = strategy;
    }

    public void SetStrategy(IStrategy<ChatCellView> strategy)
    {
        this.strategy = strategy;
    }

    public void ExecuteStrategy(ChatCellView cellView)
    {
        strategy.DoAlgorithm(cellView);
    }
}