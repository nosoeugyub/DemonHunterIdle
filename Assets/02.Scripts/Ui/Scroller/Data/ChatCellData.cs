
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 채팅 셀을 구성하는데 필요한 데이터
/// </summary>
public class ChatCellData
{
    //셀의 타입은 무엇인지
    public ChatCellType cellType;

    public int ranking;             //채팅 작성한 유저의 순위
    public string nickName;         //채팅 작성한 유저의 닉네임
    public string message;          //채팅의 내용  

    //(채팅 알림) 채팅 알림 타입 
    public ChatBroadcastType alertType = ChatBroadcastType.None;
    
    //채팅 셀의 크기
    public float cellSize;          
}