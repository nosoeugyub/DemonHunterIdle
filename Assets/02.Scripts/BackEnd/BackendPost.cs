using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
/// <summary>
/// 작성일자   : 2024-07-16 
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 우편 정보 제목, 내용, 날짜                                      
/// </summary>
public class Post
{
    public bool isCanReceive = false;

    public string title; // 우편 제목
    public string content; // 우편 내용
    public string inDate; // 우편 inDate
    public string sender; // (로컬 우편)  우편 출처
    public string localMailKey; //(로컬 우편) 저장된 우편의 키 값 

    // string은 우편 아이템 이름, int는 갯수
    public Dictionary<string, int> postReward = new Dictionary<string, int>();

    public override string ToString()
    {
        string result = string.Empty;
        result += $"title : {title}\n";
        result += $"content : {content}\n";
        result += $"inDate : {inDate}\n";

        if (isCanReceive)
        {
            result += "우편 아이템\n";

            foreach (string itemKey in postReward.Keys)
            {
                result += $"| {itemKey} : {postReward[itemKey]}개\n";
            }
        }
        else
        {
            result += "지원하지 않는 우편 아이템입니다.";
        }

        return result;
    }
}

