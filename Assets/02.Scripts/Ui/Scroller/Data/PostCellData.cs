using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 우편 셀 데이터 집합                             
/// </summary>
public class PostCellData
{
    public int index;
    public PostType postType; // 누가 보낸건지 우편 타입 결정
    public string inDate;     // 보낸 날짜
    public string title;      // 우편 제목
    public string senderName; // 우편 받는 사람
    public string itemName;   // 우편으로 받은 아이템 네임
    public int itemCount;     // 우편으로 받은 아이템 개수
    public string remainTime;

    public Sprite itemSprite;
    public Sprite itemBackgroundSprite;

    public bool buttonDisable;
}
