using System.Collections.Generic;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-11
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 아이템 가챠시 필요한 데이터
/// </summary>
public class ItemGachaData
{
    public int Index; //1부터 시작
    public string RewardName; //장비는 등급이름으로만 저장되어있음
    public float Prob; //확률
    public int Count;
    public string FrameEffect;
    public bool ChatBroadcast;

    //csv에서 불러오지 않음
    public bool IsResource = false;
    public string ItemRewardFullName = string.Empty; //아이템 형식일 경우 서브클래스+등급+이름 조합한 이름

}