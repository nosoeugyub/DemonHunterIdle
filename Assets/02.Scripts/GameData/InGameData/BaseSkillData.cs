using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-10-11
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 기반 스킬 데이터
/// </summary>
public class BaseSkillData 
{
    public string SkillName;
    public int Level; // 레벨
    public Utill_Enum.Resource_Type REsourceType; //강화 소모 타입
    public int ResourceCount; // 강화소모 재화량
    public int LevelLimit;//레벨 제한
    public int CHGValue_UseMP; // 마나소모
    public float CHGValue_SkillDuration; // 지속시간

    public List<string> CHGValue = new List<string>();  //CHG리스트
    public List<float> DMGValue = new List<float>();//DMG 리스트

    public List<string> CHGName = new List<string>();  //CHG 이름 리스트
    public List<string> DMGName = new List<string>();//DMG 이름 리스트


    public  List<Utill_Enum.Option> optionliast = new List<Utill_Enum.Option>(); //해당 스킬의 수치기반
    public List<Utill_Enum.AttackDamageType> attackdamagetype = new List<Utill_Enum.AttackDamageType>(); //해당스킬의 수치타입

    public static List<BaseSkillData> FindSkillDataList(Dictionary<string, List<BaseSkillData>> archerSkillList, string name) //이름으로 데이터 리스트를 반환함
    {
        // 키가 존재하면 해당 값을 반환하고, 없으면 빈 리스트 반환
        return archerSkillList.TryGetValue(name, out var skillData) ? skillData : new List<BaseSkillData>();
    }


    public static BaseSkillData FindLevelForData(Dictionary<string, List<BaseSkillData>> archerSkillList, string name , int Level)//데이터 이름 레벨 넣으면 해당 데이터를 뱉어냄
    {
        BaseSkillData tempdata = new BaseSkillData();
        List<BaseSkillData> data = archerSkillList.TryGetValue(name, out var skillData) ? skillData : new List<BaseSkillData>();
        tempdata  = data[Level];
        return tempdata;
    }
    //아직 다른 헌터 데이터 작업이 안 끝나 임시로 아쳐것만 넣ㅇ줌
    public static Dictionary<string, List<BaseSkillData>> GetSkillListPerSubClass(Utill_Enum.SubClass subClass)
    {
        switch (subClass)
        {
            case Utill_Enum.SubClass.Archer:
                return GameDataTable.Instance.ArcherSkillList;
                break;
            case Utill_Enum.SubClass.Guardian:
                break;
            case Utill_Enum.SubClass.Mage:
                break;
        }
        return null;
    }

    public static float FInd_SkillForChgDmg(string strarray)
    {
        float temp = 0;

        return temp;
    }

    public static List<float> FInd_SkillForChgDmgArray(string[] strarray)
    {
        List<float> temp  = new List<float>();

        return temp;
    }
}
