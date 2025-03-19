using System.Collections.Generic;

/// <summary>
/// 작성일자   : 2024-09-06
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : csv에서 읽어온 일일보상 데이터
/// </summary>
public class DailyMissionData
{
    public int StageLevelMin;
    public int StageLevelMax;

    public Dictionary<Utill_Enum.DailyMissionType, DailyMission> Missions = new();

    public static DailyMissionData GetCurrentDailyMissionData(List<DailyMissionData>data, int stageLevel)
    {
        int tmpStage = 0;
        for(int i = 0; i < data.Count; i++)
        {
            tmpStage += data[i].StageLevelMax;
            if (tmpStage >= stageLevel)
                return data[i];
        }
        return null;
    }

    public static DailyMissionData FindDailyMissionData(List<DailyMissionData> dailyMissionData, string str)
    {
        var tempStr = str.Split('_');

        // 딕셔너리의 모든 값들을 순회하며 찾습니다.
        foreach (var kvp in dailyMissionData)
        {
            if (kvp.StageLevelMax == int.Parse(tempStr[0]))
            {
                return kvp;
            }
        }

        // 일치하는 데이터가 없을 경우 null을 반환합니다.
        return null;
    }
}

public struct DailyMission
{
    public int NeedValue;
    public string ResourceType;
    public int ResourceCount;
}
