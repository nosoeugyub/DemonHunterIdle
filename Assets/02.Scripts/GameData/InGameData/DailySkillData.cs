using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 요일스킬등급데이터
/// </summary>
public class DailySkillData 
{
    public string Key;
    public float NoramlValue;
    public float Superior;
    public float Rare;
    public float Unique;
    public float Epic;

    //등급만 나오는 함수
    public static Utill_Enum.Grade GetRandom_Gradeskill(Dictionary<string, DailySkillData> dic)
    {
        if (!dic.ContainsKey("GradeProb"))
        {
            Debug.LogError($"Key '{"GradeProb"}' not found in dictionary.");
            return Utill_Enum.Grade.Normal; // 기본값 반환
        }

        DailySkillData skillData = dic["GradeProb"];

        // 난수 생성
        System.Random random = new System.Random();
        float randValue = (float)random.NextDouble() * 100f; // 0부터 100 사이의 난수

        // 누적 확률 계산
        float cumulativeProbability = 0f;

        // 각 등급별 확률을 기준으로 결정
        cumulativeProbability += skillData.NoramlValue;
        if (randValue < cumulativeProbability)
            return Utill_Enum.Grade.Normal;

        cumulativeProbability += skillData.Superior;
        if (randValue < cumulativeProbability)
            return Utill_Enum.Grade.Superior;

        cumulativeProbability += skillData.Rare;
        if (randValue < cumulativeProbability)
            return Utill_Enum.Grade.Rare;

        cumulativeProbability += skillData.Unique;
        if (randValue < cumulativeProbability)
            return Utill_Enum.Grade.Unique;

        cumulativeProbability += skillData.Epic;
        if (randValue < cumulativeProbability)
            return Utill_Enum.Grade.Epic;

        // 기본적으로 Normal 반환
        return Utill_Enum.Grade.Normal;
    }

    //해당스킬이 등급에따라 값이 나오게
    public static float Get_GradeSkillValue(string skillName , Dictionary<string, DailySkillData> dic , Utill_Enum.Grade _grade)
    {
        if (!dic.ContainsKey(skillName))
        {
            Debug.LogError($"Skill '{skillName}' not found in dictionary.");
            return 0f; // 기본값 반환
        }

        DailySkillData skillData = dic[skillName];

        switch (_grade)
        {
            case Utill_Enum.Grade.Normal:
                return skillData.NoramlValue;
            case Utill_Enum.Grade.Superior:
                return skillData.Superior;
            case Utill_Enum.Grade.Rare:
                return skillData.Rare;
            case Utill_Enum.Grade.Unique:
                return skillData.Unique;
            case Utill_Enum.Grade.Epic:
                return skillData.Epic;
            default:
                Debug.LogError($"Invalid grade '{_grade}' provided.");
                return 0f; // 기본값 반환
        }
    }

    public static List<Hunter> Get_BindingHunter(float ArcherValue , float GurdianValue ,float MageValue , float AllValue)
    {
        List<Hunter> hunters = new List<Hunter>();
        // 난수 생성
        System.Random random = new System.Random();
        float randValue = (float)random.NextDouble() * 100f; // 0부터 100 사이의 난수


        // 누적 확률 계산
        float cumulativeProbability = 0f;

        // 각 등급별 확률을 기준으로 결정
        cumulativeProbability += ArcherValue;
        if (randValue < cumulativeProbability)
        {
            hunters.Add(DataManager.Instance.Hunters[0]);
                 return hunters;
        }
           

        cumulativeProbability += GurdianValue;
        if (randValue < cumulativeProbability)
        {
            hunters.Add(DataManager.Instance.Hunters[1]);
            return hunters;
        }


        cumulativeProbability += MageValue;
        if (randValue < cumulativeProbability)
        {
            hunters.Add(DataManager.Instance.Hunters[2]);
            return hunters;
        }


        cumulativeProbability += AllValue;
        if (randValue < cumulativeProbability)
        {
            hunters = (DataManager.Instance.Hunters);
        }


        // 기본적으로 Normal 반환
        return hunters;
    }
}
