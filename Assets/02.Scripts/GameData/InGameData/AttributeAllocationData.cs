using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeAllocationData 
{
    public int[] LevelRange;// 레벨 범위
    public int[] AttributeCount;//속성 포인트 범위

    /// <summary>
    /// 주어진 레벨에 해당하는 속성 포인트 값　반환
    /// </summary>
    /// <param name="data">레벨 범위에 따른 속성 할당 데이터를 포함한 딕셔너리</param>
    /// <param name="level">현재 레벨</param>
    /// <returns>레벨에　맞는　랜덤으로 선택된 속성 포인트 값</returns>
    public static int Get_Attribute_value(Dictionary<int, AttributeAllocationData> data, int level)
    {
        int levelRangeKey = IsWithinRange(data, level);
        int result = 0;

        if (levelRangeKey != -1 && data.ContainsKey(levelRangeKey))
        {
            result = GetRandomValue(data[levelRangeKey]);
        }
        else
        {
            Debug.LogError($"Invalid level range key: {levelRangeKey}. No corresponding data found.");
        }

        return result;
    }

    /// <summary>
    /// 주어진 레벨이 속한 키반환.
    /// </summary>
    public static int IsWithinRange(Dictionary<int, AttributeAllocationData> data, int value)
    {
        if (value >= data[1].LevelRange[0] && value <= data[1].LevelRange[1])
        {
            return 1;
        }
        else if (value >= data[2].LevelRange[0] && value <= data[2].LevelRange[1])
        {
            return 2;
        }
        else if (value >= data[3].LevelRange[0] && value <= data[3].LevelRange[1])
        {
            return 3;
        }
        return -1;
    }

    /// <summary>
    /// 레벨 업 시 얼마나 오를지 랜덤 값　반환
    /// </summary>
    /// <param name="data">속성 할당 데이터</param>
    /// <returns>랜덤으로 선택된 속성 포인트 값</returns>
    private static int GetRandomValue(AttributeAllocationData data)
    {
        // 랜덤 값을 생성하기 위한 범위 설정
        int minValue = data.AttributeCount[0];
        int maxValue = data.AttributeCount[1];
        // 랜덤 값 생성
        System.Random random = new System.Random();
        return random.Next(minValue, maxValue + 1); // maxValue를   포함시키기 위해 +1
    }

    /// <summary>
    /// 처음부터 주어진 레벨까지의 총 스탯 포인트 초기화
    /// </summary>
    /// <param name="data">레벨 범위에 따른 속성 할당 데이터를 포함한 딕셔너리</param>
    /// <param name="level">현재 레벨</param>
    /// <returns>총 스탯 포인트</returns>
    public static int InitializeStatPoints(Dictionary<int, AttributeAllocationData> data, int level)
    {
        Debug.Log("레벨   " + level);
        int totalStatPoints = 0;

        // 미리 계산된 스탯 포인트 합을 저장할 딕셔너리
        Dictionary<int, int> preCalculatedPoints = new Dictionary<int, int>();

        // 각 레벨 범위에 대한 총합 계산
        foreach (var kvp in data)
        {
            int key = kvp.Key;
            AttributeAllocationData allocationData = kvp.Value;

            int levelStart = allocationData.LevelRange[0];
            int levelEnd = allocationData.LevelRange[1];

            int points = 0;
            System.Random random = new System.Random();

            for (int i = levelStart; i <= levelEnd; i++)
            {
                points += random.Next(allocationData.AttributeCount[0], allocationData.AttributeCount[1] + 1);
            }

            preCalculatedPoints[key] = points;
        }

        // 현재 레벨까지의 총합 계산
        for (int i = 2; i <= level; i++)
        {
            int levelRangeKey = IsWithinRange(data, i);

            if (levelRangeKey != -1 && preCalculatedPoints.ContainsKey(levelRangeKey))
            {
                AttributeAllocationData allocationData = data[levelRangeKey];
                totalStatPoints += GetRandomValue(allocationData);
            }
            else
            {
                Debug.LogError($"Invalid level range key: {levelRangeKey} for level {i}. No corresponding data found.");
            }
        }

        return totalStatPoints;
    }
}
