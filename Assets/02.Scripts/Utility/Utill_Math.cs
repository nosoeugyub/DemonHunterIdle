using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 유틸 수학 관련 클래스
/// /// </summary>
public class Utill_Math 
{
    public static List<T> ChangeListSwap<T>(List<T> list) //리스트 내부 값을 바꾸는 함수
    {

        // 첫 번째 요소를 저장하고 삭제
        T firstElement = list[0];
        // 두 번째 요소를 저장하고 삭제
        //T ScecndElement = list[1];
        list.RemoveAt(0);
        //list.RemoveAt(0);

        // 저장된 첫 번째 요소를 리스트의 끝에 추가
        list.Add(firstElement);
        //list.Add(ScecndElement);

        return list;
    }

    // 확률에 따라 true를 반환하는 함수
    public static bool CalculateProbability(float probability)
    {
        // 0과 1 사이의 랜덤한 값을 생성합니다.
        float randomValue = Random.value;

        // 랜덤한 값이 확률보다 작거나 같으면 true를 반환합니다.
        // 확률이 0일 때는 항상 false가 반환되고, 확률이 1일 때는 항상 true가 반환됩니다.
        return randomValue <= probability;
    }

    public static void MakeStageArray(int _clearstage)
    {
        //데이터 가공
        int clearstage = _clearstage;
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userstageindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = 0;
        if (GameDataTable.Instance.StageTableDic.ContainsKey(userstageindex))
        {
             totalRound = GameDataTable.Instance.StageTableDic[userstageindex].ChapterCycle;
            
        }

        int stageIndex = 1; // 스테이지 인덱스

        foreach (KeyValuePair<int, StageTableData> pair in GameDataTable.Instance.StageTableDic)
        {
            StageTableData stageData = pair.Value;

            // 해당 스테이지의 클리어 상태를 결정
            // 현재 스테이지 인덱스가 클리어 레벨보다 작거나 같거나, 챕터가 클리어 챕터보다 작으면 모두 클리어
            if (stageIndex < userstageindex || stageData.ChapterZone < UserStage.chapter) 
            {
                for (int i = 0; i < stageData.IsSClearStageArray.Length; i++)
                {
                    stageData.IsSClearStageArray[i] = true;
                }

                stageData.IsClearBoss = true; //보스는 깬 것으로 판정함
            }
            else if (stageIndex == userstageindex) // 현재 스테이지 인덱스가 클리어 레벨과 같으면 일부만 클리어
            {
                for (int i = 0; i < userRound; i++)
                {
                    stageData.IsSClearStageArray[i] = true;
                }
                for (int i = userRound; i < stageData.IsSClearStageArray.Length; i++)
                {
                    stageData.IsSClearStageArray[i] = false;
                }
                stageData.IsClearBoss = false; //보스는 못 깬 것으로 판정함
            }
            else // 나머지 스테이지는 모두 미클리어
            {
                for (int i = 0; i < stageData.IsSClearStageArray.Length; i++)
                {
                    stageData.IsSClearStageArray[i] = false;
                }
                stageData.IsClearBoss = false; //보스는 못 깬 것으로 판정함
            }

            stageIndex++;
        }
    }

    //public static int CalculateValue(int n) //스테이지 변환을 위해 사용되는 공용함수
    //{
    //    if (n == 1)
    //    {
    //        return 1;
    //    }
    //    else
    //    {
    //        // 1을 제외한 경우에는 (n-1) / 10 + 1을 반환합니다.
    //        return (n - 1) / 10 + 1;
    //    }
    //}

    //스테이지 라운드 구하는 로직
    public static (int stageindex ,int chapter ,int CurrentStage , int CurrentRound) CalculateStageAndRound(int _currenttotalStage)
    {
        int tempLevel = 1;
        int tempRound = 1;
        int tempChapter = 1;
        int stageindex = 0;
        int tempcount = GameDataTable.Instance.StageTableDic.Count;

        for (int i = 0; i < tempcount; i++)//총스테이지 를 돌면서 
        {
            
            int key = i + 1;
            int CurrentStageCycle = GameDataTable.Instance.StageTableDic[key].ChapterCycle; //한 스테이지에있는 라운드 갯수
            stageindex = key;
            tempRound = _currenttotalStage - CurrentStageCycle; //현재 총스에이지에서 각 라운드를 뺌
            if (tempRound <= 0)
            {
                //해당 키값에 맞는  레벨을 가져옴
                 tempLevel = GameDataTable.Instance.StageTableDic[key].ChapterLevel;
                tempChapter = GameDataTable.Instance.StageTableDic[key].ChapterZone;
                return (stageindex , tempChapter, tempLevel, _currenttotalStage);
            }
            else
            {
                tempLevel = GameDataTable.Instance.StageTableDic[key].ChapterLevel;
                _currenttotalStage = tempRound;

            }
        }

        return (stageindex , tempChapter, tempLevel, tempRound);
    }


    //확률 공용함수
    public static bool Attempt(float chancePercent)
    {
        if (chancePercent < 0f )
        {
            return false;
        }
        else if (chancePercent > 100f)
        {
            return true;
        }

        float roll = Random.Range(0f, 100f);
        return roll < chancePercent;
    }
    //스테이지 최대값을 구하는 함수
    public static int Make_Max_stage(int MaxLevel)
    {
        int temp = 0;
        int count = MaxLevel;
        int value = 0;


        
        for (int i = 0; i < count; i++)
        {

            value += GameDataTable.Instance.StageTableDic[i + 1].ChapterCycle;
        }
        temp = value;
        return temp;
    }


    /// <summary>
    /// 세 Vector3 중간 지점을 계산하는 함수
    /// (카메라 포지션 계산에 사용)
    /// </summary>
    public static Vector3 CalculateMidpoint(Vector3 t1, Vector3 t2, Vector3 t3)
    {
        //계산에 유효한 vector의 갯수 측정
        Vector3[] positionArr = new Vector3[] { t1, t2, t3 };
        Vector3 sum = Vector3.zero;
        int count = 0;

        // 각 Transform의 위치를 확인하여 Vector3.zero가 아닌 위치를 합산
        foreach (Vector3 t in positionArr)
        {
            if (t != Vector3.zero)
            {
                sum += t;
                count++;
            }
        }

        // 각 축의 값을 평균하여 중간 지점을 계산
        Vector3 midpoint = Vector3.zero;
        if (count > 0)
            midpoint = sum / count;

        return midpoint;
    }


    /// <summary>
    /// 특정 배열의 인덱스와 배열의 중앙 인덱스 간의 거리를 기반으로 오프셋을 계산
    /// 헌터 x축 관리/다중 발사 투사체 각도 관리에 사용됨
    /// </summary>
    /// <param name="dist">각 요소마다 벌어진 거리</param>
    /// <param name="idx">반환할 요소의 인덱스</param>
    /// <param name="maxIdx">총 요소의 갯수</param>
    /// <returns></returns>
    public static float GetDistancedValue(float dist, int idx, int maxIdx)
    {
        if (maxIdx <= 0 || idx < 0 || idx >= maxIdx)
        {
            Debug.LogError("Invalid array length or index.");
            return -1;
        }
        float centerIndex = maxIdx % 2 == 0 ? (maxIdx / 2) - 0.5f : maxIdx / 2;

        // 자신의 위치 계산
        float positionOffset = (idx - centerIndex) * dist;

        return positionOffset;
    }

    /// <summary>
    /// Vecotr3의 축의 값을 전부 절댓값으로 변경하는 함수
    /// </summary>
    public static Vector3 AbsVector(Vector3 changeVec)
    {
        changeVec.x = (changeVec.x >= 0) ? changeVec.x : -changeVec.x;
        changeVec.y = (changeVec.y >= 0) ? changeVec.y : -changeVec.y;
        changeVec.z = (changeVec.z >= 0) ? changeVec.z : -changeVec.z;
        return changeVec;
    }

    #region 큰 수 K,M,G와 같은 단위로 축약
    public const long DamageMaxValue = 9000000000000000000;
    public const long KillMaxValue = 999999999999999999;

    private static readonly SortedDictionary<System.Tuple<long, long>, string> abbrevations = new SortedDictionary<System.Tuple<long, long>, string>
    {
        { new System.Tuple<long, long>(1000000,                         1000), "K" },
        { new System.Tuple<long, long>(1000000000,                   1000000), "M" },
        { new System.Tuple<long, long>(1000000000000,             1000000000), "G" },
        { new System.Tuple<long, long>(1000000000000000,       1000000000000), "T" },
        { new System.Tuple<long, long>(1000000000000000000, 1000000000000000), "P" },
    };
    /// <summary>
    /// 큰 수를 K,M,G와 같은 단위로 축약하여 반환
    /// 6자리 기준으로 포맷팅 되어 반환됨 (123 -> 000123)
    /// </summary>
    public static string AbbreviateNumber(long number)
    {
        for (int i = abbrevations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<System.Tuple<long, long>, string> pair = abbrevations.ElementAt(i);
            if (AbsoluteValueOf(number) >= pair.Key.Item1)
            {
                long roundedNumber = number / pair.Key.Item2;
                return string.Format("{0:000000}{1}", roundedNumber, pair.Value);
            }
        }

        return  string.Format("{0:000000}", number);
    }

    /// <summary>
    /// 큰 수를 K,M,G와 같은 단위로 축약하여 반환
    /// </summary>
    public static string AbbreviateNumberWithoutZero(long number)
    {
        for (int i = abbrevations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<System.Tuple<long, long>, string> pair = abbrevations.ElementAt(i);
            if (AbsoluteValueOf(number) >= pair.Key.Item1)
            {
                long roundedNumber = number / pair.Key.Item2;
                return string.Format("{0}{1}", roundedNumber, pair.Value);
            }
        }

        return string.Format("{0}", number);
    }

    private static double AbsoluteValueOf(double number)
    {
        double num = number;
        if (number < 0)
        {
            num = -1 * number;
        }

        return num;
    }
    #endregion



    //재화 단위 천 자리 단위로 표시
    public static string FormatCurrency(int value)
    {
        // 천의 자리마다 쉼표를 넣고 소수점 이하 없이 숫자를 포맷
        return string.Format("{0:N0}", value);
    }
}

