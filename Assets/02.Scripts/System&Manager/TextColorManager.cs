using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 코드에서 사용해야 될 색상들을 제공
/// </summary>
public class TextColorManager : MonoSingleton<TextColorManager>
{
    [Header("인게임 관련")]
    [Header("-오류 표시 알림의 색상")]
    [SerializeField]
    private Color notificationChatColor = Color.white;
    [Header("-경험치 색상")]
    [SerializeField]
    private Color ExpColor = Color.white;
    [Header("-스테이지 바 색상")]
    [SerializeField]
    private Color StagebarColor = Color.white;
    [Header("-정보 상세창 수치 맥스시 색상")]
    [SerializeField]
    private Color StatMaxColor = Color.white;


    [Space(40)]

    [Header("채팅 관련")]
    [Header("-유저 순위의 색상")]
    [SerializeField]
    private Color chatRankColor = Color.white;
    [Header("-타인의 닉네임 / 채팅 색상")]
    [SerializeField]
    private Color defaultChatColor = Color.white;
    [Header("-자신의 닉네임 / 채팅 색상")]
    [SerializeField]
    private Color myChatColor = Color.white;
    [Header("-운영자 채팅 색상")]
    [SerializeField]
    private Color consoleChatColor = Color.white;
    [Header("-알림 색상 1")]
    [SerializeField]
    private Color chatBroadcastColor1 = Color.white;
    [Header("-알림 색상 2")]
    [SerializeField]
    private Color chatBroadcastColor2 = Color.red;


    [Space(40)]

    [Header("등급별 글자 색상")]
    public Color Normal;
    public Color Superior;
    public Color Rare;
    public Color Unique;
    public Color Epic;
    public Color Hero;
    public Color Ancient;
    public Color Abyssal;
    public Color Legendary;
    public Color Mythic;
    public Color Celestial;
    public Color Primordial;
    public Color Absolute;






    public Color ChatRankColor => chatRankColor;
    public Color DefaultChatColor => defaultChatColor;
    public Color MyChatColor => myChatColor;
    public Color ConsoleChatColor => consoleChatColor;
    public Color ChatBroadcastColor1 => chatBroadcastColor1;
    public Color ChatBroadcastColor2 => chatBroadcastColor2;
    public Color NotificationChatColor => notificationChatColor;
    public Color SendExpColor => ExpColor;
    public Color SendStageColor => StagebarColor;
    public Color SendStatMaxColor => StatMaxColor;

}
