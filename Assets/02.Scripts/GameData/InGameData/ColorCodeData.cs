using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-06-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 코드이름으로 색찾는 스크립트
/// </summary>
public class ColorCodeData 
{
    public string Name;
    public string Code;


    public static string GetTextColor(Dictionary<string , ColorCodeData> data_dic, string Text)
    {
        ColorCodeData temp = null;
        string result = string.Empty;

        //temp = data_dic[Text];
        //result = temp.Code;
        

        return result;
    }
}
