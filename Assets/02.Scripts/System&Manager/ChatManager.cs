using BackEndChat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static Utill_Enum;


/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 채팅의 서버 송수신 관련을 제어함
///             뒤끝 채팅 SDK 사용함 https://docs.thebackend.io/sdk-docs/chat/channel
/// </summary>
public class ChatManager : MonoSingleton<ChatManager>, BackEndChat.IChatClientListener
{
    [SerializeField]
    private ChatCellView recentChat = null; //최근 채팅 보여주는 챗 셀

    private ChatClient ChatClient = null;
    private ChatPopUp chatPopUp = null;

    private Dictionary<string,ChannelInfo> currentChannel = new(); //채널 이름 / 채널 인포
    private string curShowChat = ""; //현재 보여주고 있는 채널의 이름

    private StringBuilder alertChatSb = new StringBuilder();

    public ChatCellView RecentChat => recentChat;

    private const string rankIdentifier = "UserRank_";
    private const string notificationIdentifier = "ChatError_";

    void Start()
    {
        chatPopUp = PopUpSystem.Instance.ChatPopUp;
        if (BackendManager.Instance.IsLocal) return;

        ChatClient = new ChatClient(this, "default");
    }

    void Update()
    {
        ChatClient?.Update();
    }

    protected override void OnDestroy()
    {
        ChatClient?.Dispose();
        base.OnDestroy();
    }

    protected override void OnApplicationQuit()
    {
        ChatClient?.Dispose();
        base.OnApplicationQuit();
    }

    /// <summary>
    /// 채팅 알림 추가
    /// </summary>
    /// <param name="chatBroadcastType">알림 타입</param>
    /// <param name="makeSound">사운드 재생 여부</param>
    /// <param name="message">보낼 알림 (포멧은 ChatBroadcastStringGenerator 참고)</param>
    public void SendChatBroadcast(ChatBroadcastType chatBroadcastType, bool makeSound, string message)
    {
        if (BackendManager.Instance.IsLocal) return; //로컬일시 예외처리
        
        alertChatSb.Clear();
        alertChatSb.Append(chatBroadcastType.ToString());
        alertChatSb.Append(makeSound);
        alertChatSb.Append(message);

        string tmpMessage = alertChatSb.ToString();
        
        ChannelInfo tmpInfo = currentChannel[curShowChat];
        ChatClient.SendChatMessage(tmpInfo.ChannelGroup, tmpInfo.ChannelName, tmpInfo.ChannelNumber, tmpMessage);
    }

    /// <summary>
    /// 채팅 메시지만 있으면 현재 서버를 탐지해 채팅을 보내주는 함수
    /// </summary>
    public void ChatMsgWithoutChannelInfo(string message)
    {
        if (BackendManager.Instance.IsLocal) return;
        if (curShowChat == string.Empty) return;
        if (currentChannel.Count <= 0) //현재 들어와 있는 채널이 없다면
            return;
        if (CanSend(message) == false) //메시지가 유효하지 않음
        {
            NotifyError(notificationIdentifier + "CantSendMessage");
            return;
        }
        var myRank = RankManager.Instance.GetLastCheckedMyData(RankType.ClearStageLevel1);
        int rankNum = myRank == null ? -1 : int.Parse(myRank["rows"][0]["rank"].ToString());
        message = $"{rankIdentifier}{rankNum}_{message}";
        ChannelInfo tmpInfo = currentChannel[curShowChat];
        ChatClient.SendChatMessage(tmpInfo.ChannelGroup, tmpInfo.ChannelName, tmpInfo.ChannelNumber, message);
    }

    //여러 채널에 동시에 입장을 한 상태라도 같은 콜백이 호출됩니다.
    //메시지에 있는 채널 정보를 토대로 메시지를 따로 출력하는 로직을 만들어야 합니다.
    #region 서버 콜백
    /// <summary>
    /// 채널 입장시
    /// </summary>
    public void OnJoinChannel(ChannelInfo channelInfo)
    {
        if (currentChannel.ContainsKey(channelInfo.ChannelName)) return;

        //채널 init
        currentChannel.Add(channelInfo.ChannelName, channelInfo);
        curShowChat = channelInfo.ChannelName;

        //채팅 셀 init
        List<ChatCellData> chatCellList = new List<ChatCellData>();
        chatPopUp.ChatScrollController.ResetData();
        //최근 대화내역 불러오기 (불려오는 개수는 콘솔에서 관리함)
        for (int i = 0; i < channelInfo.Messages.Count; i++)
        {
            chatCellList.Add(CreateChatCellData(channelInfo.Messages[i].Message, channelInfo.Messages[i].GamerName, channelInfo.Messages[i].MessageType, false));
        }

        if (channelInfo.Messages.Count > 0)
            recentChat.SetData(chatCellList[chatCellList.Count - 1]);
        else
            recentChat.SetData(new ChatCellData());

        chatPopUp.ChatScrollController.SetData(chatCellList);
        Game.Debbug.Debbuger.Debug(channelInfo.ChannelName + " 입장 성공");
    }
    /// <summary>
    /// 채널 나갈 시 오는 콜백 함수
    /// </summary>
    /// <param name="channelInfo"></param>
    public void OnLeaveChannel(ChannelInfo channelInfo)
    {
        if (!currentChannel.ContainsKey(channelInfo.ChannelName))
            return;
        currentChannel.Remove(channelInfo.ChannelName);
        curShowChat = string.Empty;

        //현재 남은 채팅 싹 비우기
        chatPopUp.ChatScrollController.SetData(new List<ChatCellData>());
        //채널접속 끊김을 표시함
        NotifyError(LocalizationTable.Localization(notificationIdentifier+"LeaveChannel"));
    }

    /// <summary>
    /// 채널에 들어와 있는 상태에서 새 메시지가 왔을 시
    /// </summary>
    public void OnChatMessage(MessageInfo messageInfo)
    {
        ChatCellData tmpChatCellData = CreateChatCellData(messageInfo.Message, messageInfo.GamerName, messageInfo.MessageType, true);

        chatPopUp.ChatScrollController.AddNewRow(tmpChatCellData.cellType, tmpChatCellData.ranking, tmpChatCellData.nickName, tmpChatCellData.message, tmpChatCellData.alertType, keepPosition: messageInfo.GamerName != BackendManager.myNickname);

        recentChat.SetData(tmpChatCellData);
    }

    /// <summary>
    /// 채팅 관련 요청에 대해 성공 응답이 들어옴 (신고/아바타 변경)
    /// https://docs.thebackend.io/sdk-docs/chat/success
    /// </summary>
    public void OnSuccess(SUCCESS_MESSAGE success, object param)
    {
        switch (success)
        {
            case SUCCESS_MESSAGE.UNKNOWN_SUCCESS:
                break;
            case SUCCESS_MESSAGE.REPORT:
                break;
            case SUCCESS_MESSAGE.CHANGE_AVATAR:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 채팅 관련 요청에 대해 실패 응답이 들어옴
    /// https://docs.thebackend.io/sdk-docs/chat/error
    /// </summary>
    public void OnError(ERROR_MESSAGE error, object param)
    {
        switch (error)
        {
            default:
                break;
        }
        NotifyError(notificationIdentifier + error.ToString());
    }

    #region 현재 미사용
    public void OnJoinChannelPlayer(string channelGroup, string channelName, UInt64 channelNumber, string gamerName, string avatar) { }

    public void OnLeaveChannelPlayer(string channelGroup, string channelName, UInt64 channelNumber, string gamerName, string avatar) { }


    public void OnWhisperMessage(WhisperMessageInfo messageInfo) { }

    public void OnHideMessage(MessageInfo messageInfo) { }

    public void OnDeleteMessage(MessageInfo messageInfo) { }
    #endregion

    #endregion

    /// <summary>
    /// 해당 메세지의 내용 체크
    /// </summary>
    /// <returns>보낼 수 있는지 유무</returns>
    private bool CanSend(string message)
    {
        if (message.StartsWith(notificationIdentifier)) //Notification 형식으로 시작하는 채팅을 보내려 한다면
            return false;
        if(TryParseAlertChatType(message) != ChatBroadcastType.None) //채팅알림 형식으로 시작하는 채팅을 보내려 한다면
            return false;

        return true;
    }

    /// <summary>
    /// 이 메세지가 알림타입인지 검사
    /// </summary>
    private ChatBroadcastType TryParseAlertChatType(string message)
    {
        for (int i = 0; i < (int)ChatBroadcastType.END; i++)
        {
            if (message.StartsWith(((ChatBroadcastType)i).ToString()))
            {
                return (ChatBroadcastType)i;
            }
        }
        return ChatBroadcastType.None;
    }

    /// <summary>
    /// 에러 발생시 채팅에 안내를 띄우는 함수
    /// </summary>
    private void NotifyError(string errorMessage)
    {
        chatPopUp.ChatScrollController.AddNewRow(ChatCellType.Notification, 0, "", errorMessage);

        recentChat.SetData(new ChatCellData()
        {
            cellType = ChatCellType.Notification,
            ranking = 0,
            nickName = "",
            message = errorMessage,
            cellSize = 0,
        });
    }

    /// <summary>
    /// 매개 변수들을 기반으로 ChatCellData 반환
    /// </summary>
    /// <param name="makeSoundIfCan">사운드 재생 여부</param>
    /// <returns></returns>
    private ChatCellData CreateChatCellData(string message, string gamerName, MESSAGE_TYPE messageType,bool makeSoundIfCan)
    {
        ChatCellType chatCellType = ChatCellType.Chat;
        ChatBroadcastType alertType = TryParseAlertChatType(message);

        if (alertType != ChatBroadcastType.None) //채팅 알림일 경우
        {
            chatCellType = ChatCellType.Alarm;
            message = message.Split(alertType.ToString())[1];
            bool isMakeSound = message.Contains("True");
            message = message.Replace("True", "").Replace("False", "");

            if (isMakeSound && makeSoundIfCan)
            {
                SoundManager.Instance.PlayAudio(alertType + "Sound");
            }
        }
        else if (messageType == MESSAGE_TYPE.SYSTEM_MESSAGE)
        {
            chatCellType = ChatCellType.Console;
        }

        int ranking = 0;
        string pattern = $@"^{rankIdentifier}(-?\d+)_(.*)$";
        Match match = Regex.Match(message, pattern);

        if (match.Success)
        {
            // 숫자 부분 (1번째 그룹)
            ranking = int.Parse(match.Groups[1].Value);
            // 나머지 문자열 부분 (2번째 그룹)
            message = match.Groups[2].Value;
        }

        return new ChatCellData()
        {
            cellType = chatCellType,
            ranking = ranking,
            nickName = gamerName,
            message = message,
            alertType = alertType,
            cellSize = 0,
        };
    }

}
