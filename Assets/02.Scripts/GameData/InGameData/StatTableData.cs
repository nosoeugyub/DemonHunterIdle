using System.Collections.Generic;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 유저의 최대스텟 데이터 받아오는곳
/// </summary>
public class StatTableData
{
    public string Key;
    public int MaxValue;

    //표기 관련 변수들
    public string DisplayUnit; //표기시 뒤에 붙을 문자
    public int DisplayDigitCount; //표기시 자릿수
    public float DisplayMultiplier; //표기시 곱해줄 숫자

    public string Desciption;

    //StatMaximumLimitData 이름을 통해 해당 이름의 Maximum 스텟을 가져오기가 가능하다
    public static int GetValue_MaximumData(Dictionary<string, StatTableData> dic_data, string StatMaximumLimitData)
    {
        StatTableData temp = new StatTableData();

        if (dic_data.ContainsKey(StatMaximumLimitData) == false) return 999999999;

        temp = dic_data[StatMaximumLimitData];

        if (temp.MaxValue == -1)
        {
            temp.MaxValue = 999999999;
        }

        return temp.MaxValue;
    }

    /// <summary>
    /// 특정 스탯의 표기 형식을 적용한 값 반환
    /// </summary>
    public static string Get_DisplayString(Dictionary<string, StatTableData> dic_data, string statName, float statValue)
    {
        if (dic_data.TryGetValue(statName, out StatTableData temp))
        {
            float tempValue = statValue;
            string tempStr = string.Empty;

            if (temp.DisplayMultiplier != 0)
            {
                tempValue *= temp.DisplayMultiplier;
            }

            tempStr = tempValue.ToString($"F{temp.DisplayDigitCount}");
            tempStr = string.Format("{0:#,###}", tempStr);//천단위 콤마
            tempStr += temp.DisplayUnit;

            return tempStr;
        }
        Game.Debbug.Debbuger.ErrorDebug($"Can't find {statName} in Data");
        return statValue.ToString();
    }
}
