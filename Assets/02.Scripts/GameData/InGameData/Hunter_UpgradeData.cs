using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 헌터의 업그레이드 데이터 제어하는 스크립트
/// </summary>
public class Hunter_UpgradeData 
{
    public int Level;
    public int LevelLimit;
    public Utill_Enum.Resource_Type ResourceType;
    public int ResourceCount;
    public int PhysicalPower;
    public int MagicPower;
    public int PhysicalPowerDefense;
    public int HP;
    public int MP;
    public float CriChance;
    public float CriDamage;
    public float AttackSpeedPercent;
    public float MoveSpeedPercent;
    public float GoldBuff;
    public float ExpBuff;
    public float ItemBuff;




    //전부  1로 초기화
    public static void Upgrade_init(List<Dictionary<string, int>> userhasupgradedata)
    {
        foreach (var upgradeData in userhasupgradedata)
        {
            var keys = new List<string>(upgradeData.Keys);
            foreach (var key in keys)
            {
                upgradeData[key] = 0;
            }
        }
    }

    //스텟 만렙인지 체크하기
    public static bool Upgrade_MaxLevelCheck(List<Dictionary<string, int>> userhasupgradedata  , Utill_Enum.Upgrade_Type type)
    {
        bool ismax = false;
        // 업그레이드 타입을 문자열로 변환
        string upgradeKey = type.ToString();
        //만렙 체크
        ConstraintsData maxlevel = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_UPGRADELEVEL];

        foreach (var upgradeData in userhasupgradedata)
        {
            // 키가 존재하면 값을 증가시킴
            if (upgradeData.ContainsKey(upgradeKey))
            {
                if (upgradeData[upgradeKey] == maxlevel.Value)
                {
                    ismax = true;
                }
                else
                {
                    ismax = false;
                }
                
            }
        }


        return ismax;
    }
    //업그레이드하기
    public static void Upgrade_UserStat(List<Dictionary<string, int>> userhasupgradedata , Utill_Enum.Upgrade_Type type, Utill_Enum.SubClass character)
    {
        // 업그레이드 타입을 문자열로 변환
        string upgradeKey = type.ToString();

        foreach (var upgradeData in userhasupgradedata)
        {
            // 키가 존재하면 값을 증가시킴
            if (upgradeData.ContainsKey(upgradeKey))
            {
                upgradeData[upgradeKey]++;
                int level = upgradeData[upgradeKey];
                // 스텟 적용해주기
                int index = (int)character;
                HunterStat.UpgradeOneStat_UserStat(DataManager.Instance.Hunters[index].Orginstat, level, type);
                return; // 키를 찾았으므로 메서드를 종료
            }
        }

        // 키를 찾지 못한 경우 새로운 키-값 쌍 추가 (옵션)
        var newUpgradeData = new Dictionary<string, int>();
        newUpgradeData[upgradeKey] = 1;
        userhasupgradedata.Add(newUpgradeData);
    }

    //스텟 만렙 가져오기
    public static int Upgrade_GetMaxLevel()
    {
        int temp = 0;
        temp = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_UPGRADELEVEL].Value;
        return temp;
    }

    //스텟 레벨에대한 능력치 가져오기 
    public static float Upgrade_GetLevelStatVAlue(List<Dictionary<string, int>> userhasupgradedata, Utill_Enum.Upgrade_Type type , int level)
    {
        // 업그레이드 타입을 문자열로 변환
        string upgradeKey = type.ToString();
        float vlaue = 0;

        Hunter_UpgradeData data = GameDataTable.Instance.HunterUpgradeDataDic[level];


        switch (type)
        {
            case Utill_Enum.Upgrade_Type.PhysicalPower:
                vlaue = data.PhysicalPower;
                break;
            case Utill_Enum.Upgrade_Type.MagicPower:
                vlaue = data.MagicPower;
                break;
            case Utill_Enum.Upgrade_Type.PhysicalPowerDefense:
                vlaue = data.PhysicalPowerDefense;
                break;
            case Utill_Enum.Upgrade_Type.HP:
                vlaue = data.HP;
                break;
            case Utill_Enum.Upgrade_Type.MP:
                vlaue = data.MP;
                break;
            case Utill_Enum.Upgrade_Type.CriChance:
                vlaue = data.CriChance;
                break;
            case Utill_Enum.Upgrade_Type.CriDamage:
                vlaue = data.CriDamage;
                break;
            case Utill_Enum.Upgrade_Type.AttackSpeedPercent:
                vlaue = data.AttackSpeedPercent;
                break;
            case Utill_Enum.Upgrade_Type.MoveSpeedPercent:
                vlaue = data.MoveSpeedPercent;
                break;
            case Utill_Enum.Upgrade_Type.GoldBuff:
                vlaue = data.GoldBuff;
                break;
            case Utill_Enum.Upgrade_Type.ExpBuff:
                vlaue = data.ExpBuff;
                break;
            case Utill_Enum.Upgrade_Type.ItemBuff:
                vlaue = data.ItemBuff;
                break;
    
        }

        return vlaue;
    }


    public static bool Check_IsBuyCell(Dictionary<string, int> userdata , int userhasgold, int userhasdia)
    {
        bool isbuy = false;
        int upgradeamount = 0;
        int ResourceAmont = 0;
        string firstKey = userdata.Keys.FirstOrDefault(); // 첫 번째 key 가져오기
        int firstValue = userdata[firstKey]; // 첫 번째 key에 해당하는 값 가져오기
        Hunter_UpgradeData data = GameDataTable.Instance.HunterUpgradeDataDic[firstValue];


        switch (data.ResourceType)
        {
            case Utill_Enum.Resource_Type.Gold:
                ResourceAmont = userhasgold;
                break;
            case Utill_Enum.Resource_Type.Dia:
                ResourceAmont = userhasdia;
                break;
        }

        //현재 업그레이드 해야할 재화
        upgradeamount = data.ResourceCount;

        if (upgradeamount > ResourceAmont)
        {
            isbuy = false;
        }
        else
        {
            isbuy = true;
        }

        return isbuy;
    }
}
