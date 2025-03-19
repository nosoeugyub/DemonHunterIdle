using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-09-12
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 가챠 업적 테이블 데이터
/// </summary>
public class GachaAchievementData
{
    public int index = 0;
    public int totalPurchase = 0;
    public string resourceType;
    public int resourceCount;

    public static GachaAchievementData FindGachaAchievementData(Dictionary<int, GachaAchievementData> gachaAchievementDataDic, string goodsname)
    {
        int tmpTotalPurchase = 0;
        if (int.TryParse(goodsname, out tmpTotalPurchase) == false)
            return null;
        // 딕셔너리의 모든 값들을 순회하며 찾습니다.
        foreach (var kvp in gachaAchievementDataDic)
        {
            GachaAchievementData data = kvp.Value;

            // totalPurchase 값이 주어진 goodsname과 일치하면 해당 데이터를 반환합니다.
            if (data.totalPurchase == tmpTotalPurchase)
            {
                return data;
            }
        }

        // 일치하는 데이터가 없을 경우 null을 반환합니다.
        return null;
    }
}
