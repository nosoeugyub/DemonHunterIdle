using System.Collections.Generic;
/// <summary>
/// 작성일자   : 2024-09-06
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 일일보상 셀을 구성하기위해 필요한 데이터
/// </summary>
public class OfflineRewardData 
{
    public Utill_Enum.Resource_Type ResourceType;
    public int Count;

    public static OfflineRewardData FindOfflineRewardData(Dictionary<int, OfflineRewardData> gachaAchievementDataDic, string goodsname)
    {
        // 딕셔너리의 모든 값들을 순회하며 찾습니다.
        foreach (var kvp in gachaAchievementDataDic)
        {
            OfflineRewardData data = kvp.Value;

            // totalPurchase 값이 주어진 goodsname과 일치하면 해당 데이터를 반환합니다.
            if (data.ResourceType.ToString() == goodsname)
            {
                return data;
            }
        }

        // 일치하는 데이터가 없을 경우 null을 반환합니다.
        return null;
    }
}
