using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 채팅 셀 뷰
/// </summary>
public class ChatCellView : EnhancedScrollerCellView
{
    [SerializeField]
    private TMP_Text chatText;
    private ChatTextContext chatTextContext;

    public TMP_Text ChatText => chatText;
    public ChatCellData ChatCellData { get; private set; }

    //최근 채팅 보여주는 셀인지 여부
    public bool IsRecentChat => cellIdentifier == "RecentChat";

    public void SetData(ChatCellData data)
    {
        ChatCellData = data;
        if(chatTextContext == null )
            chatTextContext = new ChatTextContext();

        switch (data.cellType)
        {
            case ChatCellType.Notification:
                chatTextContext.SetStrategy(new NotificationChatStrategy());
                break;
            case ChatCellType.Alarm:
                chatTextContext.SetStrategy(new ChatBroadcastChatStrategy());
                break;
            case ChatCellType.Console:
                chatTextContext.SetStrategy(new ConsoleChatStrategy());
                break;
            case ChatCellType.Chat:
                if(data.message == null || data.message == string.Empty) //채팅으로 보낸 셀이 아닌 그냥 메시지가 없는 초기화 상태
                    chatTextContext.SetStrategy(new NoneChatStrategy());
                else if (data.nickName == BackendManager.myNickname)
                    chatTextContext.SetStrategy(new MyChatStrategy());
                else
                    chatTextContext.SetStrategy(new DefaultChatStrategy());
                break;
        }

        chatTextContext.ExecuteStrategy(this);
    }
}