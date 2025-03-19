using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 작성일자   : 2024-07-11
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 속성 업그레이드 올라가는 능력치 데이터                                                   
/// </summary>
public class HunterAttributeData
{
    public Utill_Enum.Hunter_Attribute Type;
    public float AttackSpeed;
    public float AttackRange;
    public float MoveSpeed;
    public float PhysicalPower;
    public float MagicPower;
    public float PhysicalPowerDefense;
    public float Hp;
    public float Mp;


    /// <summary>
    /// 속성 1번 버튼에 해당하는 능력치 가지고 오기
    /// </summary>
    public static List<Tuple<string, float>> GetAttributer_value1(Dictionary<Utill_Enum.Hunter_Attribute, HunterAttributeData> data_dic, int index)
    {
        List<Tuple<string, float>> options = new List<Tuple<string, float>>();

        // 인덱스에 따른 Hunter_Property 값 매핑

        Utill_Enum.Hunter_Attribute hunter_Attribute = index switch
        {
            0 => Utill_Enum.Hunter_Attribute.Archer_Attribute01,
            1 => Utill_Enum.Hunter_Attribute.Guardian_Attribute01,
            2 => Utill_Enum.Hunter_Attribute.Mage_Attribute01,
            _ => Utill_Enum.Hunter_Attribute.Archer_Attribute01 // 기본값
        };

        // 딕셔너리에서 데이터를 찾은 경우만 처리
        if (data_dic.TryGetValue(hunter_Attribute, out HunterAttributeData data))
        {
            // 속성 값을 이름과 함께 튜플로 추가
            var properties = new List<Tuple<string, float>>
        {
            Tuple.Create("PhysicalPower", data.PhysicalPower),
            Tuple.Create("AttackSpeed", data.AttackSpeed),
            Tuple.Create("AttackRange", data.AttackRange),
            Tuple.Create("MoveSpeed", data.MoveSpeed),
            Tuple.Create("MagicPower", data.MagicPower),
            Tuple.Create("PhysicalPowerDefense", data.PhysicalPowerDefense),
            Tuple.Create("HP", data.Hp),
            Tuple.Create("MP", data.Mp)
        };

            // 값이 0보다 큰 항목만 리스트에 추가
            options.AddRange(properties.Where(tuple => tuple.Item2 > 0));
        }

        return options;
    }
    public static List<Tuple<string, float>> GetAttributer_value2(Dictionary<Utill_Enum.Hunter_Attribute, HunterAttributeData> data_dic, int index)
    {
        List<Tuple<string, float>> options = new List<Tuple<string, float>>();

        // 인덱스에 따른 Hunter_Property 값 매핑

        Utill_Enum.Hunter_Attribute hunter_attribute = index switch
        {
            0 => Utill_Enum.Hunter_Attribute.Archer_Attribute02,
            1 => Utill_Enum.Hunter_Attribute.Guardian_Attribute02,
            2 => Utill_Enum.Hunter_Attribute.Mage_Attribute02,
            _ => Utill_Enum.Hunter_Attribute.Archer_Attribute01 // 기본값
        };

        // 딕셔너리에서 데이터를 찾은 경우만 처리
        if (data_dic.TryGetValue(hunter_attribute, out HunterAttributeData data))
        {
            // 속성 값을 이름과 함께 튜플로 추가
            var properties = new List<Tuple<string, float>>
        {
            Tuple.Create("PhysicalPower", data.PhysicalPower),
            Tuple.Create("AttackSpeed", data.AttackSpeed),
            Tuple.Create("AttackRange", data.AttackRange),
            Tuple.Create("MoveSpeed", data.MoveSpeed),
            Tuple.Create("MagicPower", data.MagicPower),
            Tuple.Create("PhysicalPowerDefense", data.PhysicalPowerDefense),
            Tuple.Create("HP", data.Hp),
            Tuple.Create("MP", data.Mp)
        };

            // 값이 0보다 큰 항목만 리스트에 추가
            options.AddRange(properties.Where(tuple => tuple.Item2 > 0));
        }

        return options;
    }
    public static List<Tuple<string, float>> GetAttributer_value3(Dictionary<Utill_Enum.Hunter_Attribute, HunterAttributeData> data_dic, int index)
    {
        List<Tuple<string, float>> options = new List<Tuple<string, float>>();

        // 인덱스에 따른 Hunter_Property 값 매핑

        Utill_Enum.Hunter_Attribute hunter_Attribute = index switch
        {
            0 => Utill_Enum.Hunter_Attribute.Archer_Attribute03,
            1 => Utill_Enum.Hunter_Attribute.Guardian_Attribute03,
            2 => Utill_Enum.Hunter_Attribute.Mage_Attribute03,
            _ => Utill_Enum.Hunter_Attribute.Archer_Attribute03 // 기본값
        };

        // 딕셔너리에서 데이터를 찾은 경우만 처리
        if (data_dic.TryGetValue(hunter_Attribute, out HunterAttributeData data))
        {
            // 속성 값을 이름과 함께 튜플로 추가
            var properties = new List<Tuple<string, float>>
        {
            Tuple.Create("PhysicalPower", data.PhysicalPower),
            Tuple.Create("AttackSpeed", data.AttackSpeed),
            Tuple.Create("AttackRange", data.AttackRange),
            Tuple.Create("MoveSpeed", data.MoveSpeed),
            Tuple.Create("MagicPower", data.MagicPower),
            Tuple.Create("PhysicalPowerDefense", data.PhysicalPowerDefense),
            Tuple.Create("HP", data.Hp),
            Tuple.Create("MP", data.Mp)
        };

            // 값이 0보다 큰 항목만 리스트에 추가
            options.AddRange(properties.Where(tuple => tuple.Item2 > 0));
        }

        return options;
    }

    //데이터 초기화 함수
    public static void Attribute_init(User data , int index)
    { 
       
        int total = data.HunterAttribute[index][0];
        int array01value = data.HunterAttribute[index][1];
        int array02value = data.HunterAttribute[index][2];
        int array03value = data.HunterAttribute[index][3];

        //0으로 초기화
        data.HunterAttribute[index][1] = 0;
        data.HunterAttribute[index][2] = 0;
        data.HunterAttribute[index][3] = 0;

        int totalStatPoints = AttributeAllocationData.InitializeStatPoints(GameDataTable.Instance.AttributeAllocationData, GameDataTable.Instance.User.HunterLevel[index]);

        Debug.Log("재설정 값  " + totalStatPoints);
        data.HunterAttribute[index][0] = totalStatPoints;
    }


    //총 속성에 스텟 주기 (레벨업시)
    public static void Plus_TotalStat(User data, int value, int index)
    {
        data.HunterAttribute[index][0] += value;
    }

    //총 속성 값 감소 및 인덱스에 해당하는 속성 배열 값 증가
    public static void Pluse_AttributeStat(User data , int index ,int currenthunterindex , bool istoogle)
    {
        int currentHunter = currenthunterindex;
        //if (data.HunterAttribute[currentHunter][0] < 1)
        //{
        //    return;
        //}

        //토탈에서 하나감소
        if (data.HunterAttribute[currentHunter][0] <= 0)
        {
            //속성 갯수가 부족한 알림떠야함
            return;
        }

        if (istoogle) //10배 버튼눌렀다면
        {
            if (data.HunterAttribute[currentHunter][0] - 10 < 0)
            {
                //속성갯수가 안되다는 알림떠야함
                return;
            }
            data.HunterAttribute[currentHunter][0] -= 10;
            //해당 배열에서 하나증가
            data.HunterAttribute[currentHunter][index] += 10;
        }
        else
        {
            data.HunterAttribute[currentHunter][0] -= 1;
            //해당 배열에서 하나증가
            data.HunterAttribute[currentHunter][index] += 1;
        }
        
    }
   
    #region 게임 처음시작시 유저데이터 내 속성값 바탕으로 유저 능력치 증가
    public static void UserAttributeUpgrda(HunterStat stat, User data)
    {
        //헌터의 총 갯수
        for (int i = 0; i < GameDataTable.Instance.User.HunterPurchase.Length; i++)
        {
            //유저가 구매한 헌터 는 true 반환
            if (GameDataTable.Instance.User.HunterPurchase[i] == true)
            {
                //첫번째 유저 프로퍼티의 값가져오기
                var AttributeList1 = HunterAttributeData.GetAttributer_value1(GameDataTable.Instance.HunterAttraibuteDataDic , i);
                var AttributeList2 = HunterAttributeData.GetAttributer_value2(GameDataTable.Instance.HunterAttraibuteDataDic, i);
                var AttributeList3 = HunterAttributeData.GetAttributer_value3(GameDataTable.Instance.HunterAttraibuteDataDic, i);

                //유저의 프로퍼티 레벨구하기
                float attributeMultiplier1 = data.GetAttribute(i)[1];
                //유저의 프로퍼티 레벨에 곱하고 연산하기
                float attributeMultiplier2 = data.GetAttribute(i)[2];
                //유저의 프로퍼티 레벨에 곱하고 연산하기
                float attributeMultiplier3 = data.GetAttribute(i)[3];


                // 유저의 스탯을 업데이트 (각 스탯 값을 곱셈하여 적용)
                stat.PhysicalPower += GetStatValue(AttributeList1, "PhysicalPower") * attributeMultiplier1;
                stat.MagicPower += GetStatValue(AttributeList1, "MagicPower") * attributeMultiplier1;
                stat.AttackSpeed += GetStatValue(AttributeList1, "AttackSpeed") * attributeMultiplier1;
                stat.MoveSpeed += GetStatValue(AttributeList1, "MoveSpeed") * attributeMultiplier1;
                stat.AttackRange += GetStatValue(AttributeList1, "AttackRange") * attributeMultiplier1;
                stat.PhysicalPowerDefense += GetStatValue(AttributeList1, "PhysicalPowerDefense") * attributeMultiplier1;
                stat.HP += GetStatValue(AttributeList1, "HP") * attributeMultiplier1;
                stat.MP += GetStatValue(AttributeList1, "MP") * attributeMultiplier1;

                // 유저의 스탯을 업데이트 (각 스탯 값을 곱셈하여 적용)
                stat.PhysicalPower += GetStatValue(AttributeList2, "PhysicalPower") * attributeMultiplier2;
                stat.MagicPower += GetStatValue(AttributeList2, "MagicPower") * attributeMultiplier2;
                stat.AttackSpeed += GetStatValue(AttributeList2, "AttackSpeed") * attributeMultiplier2;
                stat.MoveSpeed += GetStatValue(AttributeList2, "MoveSpeed") * attributeMultiplier2;
                stat.AttackRange += GetStatValue(AttributeList2, "AttackRange") * attributeMultiplier2;
                stat.PhysicalPowerDefense += GetStatValue(AttributeList2, "PhysicalPowerDefense") * attributeMultiplier2;
                stat.HP += GetStatValue(AttributeList2, "HP") * attributeMultiplier2;
                stat.MP += GetStatValue(AttributeList2, "MP") * attributeMultiplier2;


                // 유저의 스탯을 업데이트 (각 스탯 값을 곱셈하여 적용)
                stat.PhysicalPower += GetStatValue(AttributeList3, "PhysicalPower") * attributeMultiplier3;
                stat.MagicPower += GetStatValue(AttributeList3, "MagicPower") * attributeMultiplier3;
                stat.AttackSpeed += GetStatValue(AttributeList3, "AttackSpeed") * attributeMultiplier3;
                stat.MoveSpeed += GetStatValue(AttributeList3, "MoveSpeed") * attributeMultiplier3;
                stat.AttackRange += GetStatValue(AttributeList3, "AttackRange") * attributeMultiplier3;
                stat.PhysicalPowerDefense += GetStatValue(AttributeList3, "PhysicalPowerDefense") * attributeMultiplier3;
                stat.HP += GetStatValue(AttributeList3, "HP") * attributeMultiplier3;
                stat.MP += GetStatValue(AttributeList3, "MP") * attributeMultiplier3;
            }
        }

    }
    #endregion

    #region 초기화시 유저 능력치 증가된 값들 초기화 
    public static void MinuseUserAttributeUpgrda(HunterStat stat, User data)
    {
        //헌터의 총 갯수
        for (int i = 0; i < GameDataTable.Instance.User.HunterPurchase.Length; i++)
        {
            //유저가 구매한 헌터 는 true 반환
            if (GameDataTable.Instance.User.HunterPurchase[i] == true)
            {
                //첫번째 유저 프로퍼티의 값가져오기
                var attributeList1 = HunterAttributeData.GetAttributer_value1(GameDataTable.Instance.HunterAttraibuteDataDic, i);
                var AttributeList2 = HunterAttributeData.GetAttributer_value2(GameDataTable.Instance.HunterAttraibuteDataDic, i);
                var AttributeList3 = HunterAttributeData.GetAttributer_value3(GameDataTable.Instance.HunterAttraibuteDataDic, i);

                //유저의 프로퍼티 레벨에 곱하고 연산하기 + 1 해주는이유는 0 이 총속성이라서
                float attributeMultiplier1 = data.GetAttribute(i)[1];
                //유저의 프로퍼티 레벨에 곱하고 연산하기
                float attributeMultiplier2 = data.GetAttribute(i)[2];
                //유저의 프로퍼티 레벨에 곱하고 연산하기
                float attributeMultiplier3 = data.GetAttribute(i)[3];


                // 유저의 스탯을 업데이트 (각 스탯 값을 곱셈하여 적용)
                stat.PhysicalPower -= GetStatValue(attributeList1, "PhysicalPower") * attributeMultiplier1;
                stat.MagicPower -= GetStatValue(attributeList1, "MagicPower") * attributeMultiplier1;
                stat.AttackSpeed -= GetStatValue(attributeList1, "AttackSpeed") * attributeMultiplier1;
                stat.MoveSpeed -= GetStatValue(attributeList1, "MoveSpeed") * attributeMultiplier1;
                stat.AttackRange -= GetStatValue(attributeList1, "AttackRange") * attributeMultiplier1;
                stat.PhysicalPowerDefense -= GetStatValue(attributeList1, "PhysicalPowerDefense") * attributeMultiplier1;
                stat.HP -= GetStatValue(attributeList1, "Hp") * attributeMultiplier1;
                stat.MP -= GetStatValue(attributeList1, "Mp") * attributeMultiplier1;

                // 유저의 스탯을 업데이트 (각 스탯 값을 곱셈하여 적용)
                stat.PhysicalPower -= GetStatValue(AttributeList2, "PhysicalPower") * attributeMultiplier2;
                stat.MagicPower -= GetStatValue(AttributeList2, "MagicPower") * attributeMultiplier2;
                stat.AttackSpeed -= GetStatValue(AttributeList2, "AttackSpeed") * attributeMultiplier2;
                stat.MoveSpeed -= GetStatValue(AttributeList2, "MoveSpeed") * attributeMultiplier2;
                stat.AttackRange -= GetStatValue(AttributeList2, "AttackRange") * attributeMultiplier2;
                stat.PhysicalPowerDefense -= GetStatValue(AttributeList2, "PhysicalPowerDefense") * attributeMultiplier2;
                stat.HP -= GetStatValue(AttributeList2, "Hp") * attributeMultiplier2;
                stat.MP -= GetStatValue(AttributeList2, "Mp") * attributeMultiplier2;


                // 유저의 스탯을 업데이트 (각 스탯 값을 곱셈하여 적용)
                stat.PhysicalPower -= GetStatValue(AttributeList3, "PhysicalPower") * attributeMultiplier3;
                stat.MagicPower -= GetStatValue(AttributeList3, "MagicPower") * attributeMultiplier3;
                stat.AttackSpeed -= GetStatValue(AttributeList3, "AttackSpeed") * attributeMultiplier3;
                stat.MoveSpeed -= GetStatValue(AttributeList3, "MoveSpeed") * attributeMultiplier3;
                stat.AttackRange -= GetStatValue(AttributeList3, "AttackRange") * attributeMultiplier3;
                stat.PhysicalPowerDefense -= GetStatValue(AttributeList3, "PhysicalPowerDefense") * attributeMultiplier3;
                stat.HP -= GetStatValue(AttributeList3, "Hp") * attributeMultiplier3;
                stat.MP -= GetStatValue(AttributeList3, "Mp") * attributeMultiplier3;
            }
        }
    }
    #endregion
    
    #region  첫번째 속성 강화 했을때 추가되는 능력치
    public static void UserAttributeUpgrda01(HunterStat stat, User data , int hunterindex , bool isx10toogle)
    {
        int currentHuner = hunterindex;

        //첫번째 유저 프로퍼티의 값가져오기
        var Attribute01 = HunterAttributeData.GetAttributer_value1(GameDataTable.Instance.HunterAttraibuteDataDic, currentHuner);

        if (isx10toogle)
        {
            //유저의 프로퍼티 레벨에 곱하고 연산하기
            stat.PhysicalPower += GetStatValue(Attribute01, "PhysicalPower") * 10;
            stat.MagicPower += GetStatValue(Attribute01, "MagicPower") * 10;
            stat.AttackSpeed += GetStatValue(Attribute01, "AttackSpeed") * 10;
            stat.MoveSpeed += GetStatValue(Attribute01, "MoveSpeed") * 10;
            stat.AttackRange += GetStatValue(Attribute01, "AttackRange") * 10;
            stat.PhysicalPowerDefense += GetStatValue(Attribute01, "PhysicalPowerDefense") * 10;
            stat.HP += GetStatValue(Attribute01, "HP") * 10;
            stat.MP += GetStatValue(Attribute01, "MP") * 10;
        }
        else
        {
            //유저의 프로퍼티 레벨에 곱하고 연산하기
            stat.PhysicalPower += GetStatValue(Attribute01, "PhysicalPower");
            stat.MagicPower += GetStatValue(Attribute01, "MagicPower");
            stat.AttackSpeed += GetStatValue(Attribute01, "AttackSpeed");
            stat.MoveSpeed += GetStatValue(Attribute01, "MoveSpeed");
            stat.AttackRange += GetStatValue(Attribute01, "AttackRange");
            stat.PhysicalPowerDefense += GetStatValue(Attribute01, "PhysicalPowerDefense");
            stat.HP += GetStatValue(Attribute01, "HP");
            stat.MP += GetStatValue(Attribute01, "MP");
        }
        
    }
    #endregion

    #region  두번째 속성 강화 했을때 추가되는 능력치
    public static void UserAttributeUpgrda02(HunterStat stat, User data , int hunterindex, bool isx10toogle)
    {
        int currentHuner = hunterindex;

        //두번째 유저 프로퍼티의 값가져오기
        var Attribute02 = HunterAttributeData.GetAttributer_value2(GameDataTable.Instance.HunterAttraibuteDataDic , currentHuner);
        //유저의 프로퍼티 레벨에 곱하고 연산하기
        if (isx10toogle)
        {
            //유저의 프로퍼티 레벨에 곱하고 연산하기
            stat.PhysicalPower += GetStatValue(Attribute02, "PhysicalPower") * 10;
            stat.MagicPower += GetStatValue(Attribute02, "MagicPower") * 10;
            stat.AttackSpeed += GetStatValue(Attribute02, "AttackSpeed") * 10;
            stat.MoveSpeed += GetStatValue(Attribute02, "MoveSpeed") * 10;
            stat.AttackRange += GetStatValue(Attribute02, "AttackRange") * 10;
            stat.PhysicalPowerDefense += GetStatValue(Attribute02, "PhysicalPowerDefense") * 10;
            stat.HP += GetStatValue(Attribute02, "HP") * 10;
            stat.MP += GetStatValue(Attribute02, "MP") * 10;
        }
        else
        {
            //유저의 프로퍼티 레벨에 곱하고 연산하기
            stat.PhysicalPower += GetStatValue(Attribute02, "PhysicalPower");
            stat.MagicPower += GetStatValue(Attribute02, "MagicPower");
            stat.AttackSpeed += GetStatValue(Attribute02, "AttackSpeed");
            stat.MoveSpeed += GetStatValue(Attribute02, "MoveSpeed");
            stat.AttackRange += GetStatValue(Attribute02, "AttackRange");
            stat.PhysicalPowerDefense += GetStatValue(Attribute02, "PhysicalPowerDefense");
            stat.HP += GetStatValue(Attribute02, "HP");
            stat.MP += GetStatValue(Attribute02, "MP");
        }
    }
    #endregion

    #region  세번째 속성 강화 했을때 추가되는 능력치
    public static void UserttributeUpgrda03(HunterStat stat, User data , int hunterindex, bool isx10toogle)
    {
        int currentHuner = hunterindex;

        //세번째 유저 프로퍼티의 값가져오기
        var Proproty03 = HunterAttributeData.GetAttributer_value3(GameDataTable.Instance.HunterAttraibuteDataDic, currentHuner);

        //능력치값더해주기
        if (isx10toogle)
        {
            //유저의 프로퍼티 레벨에 곱하고 연산하기
            stat.PhysicalPower += GetStatValue(Proproty03, "PhysicalPower") * 10;
            stat.MagicPower += GetStatValue(Proproty03, "MagicPower") * 10;
            stat.AttackSpeed += GetStatValue(Proproty03, "AttackSpeed") * 10;
            stat.MoveSpeed += GetStatValue(Proproty03, "MoveSpeed") * 10;
            stat.AttackRange += GetStatValue(Proproty03, "AttackRange") * 10;
            stat.PhysicalPowerDefense += GetStatValue(Proproty03, "PhysicalPowerDefense") * 10;
            stat.HP += GetStatValue(Proproty03, "HP") * 10;
            stat.MP += GetStatValue(Proproty03, "MP") * 10;
        }
        else
        {
            //유저의 프로퍼티 레벨에 곱하고 연산하기
            stat.PhysicalPower += GetStatValue(Proproty03, "PhysicalPower");
            stat.MagicPower += GetStatValue(Proproty03, "MagicPower");
            stat.AttackSpeed += GetStatValue(Proproty03, "AttackSpeed");
            stat.MoveSpeed += GetStatValue(Proproty03, "MoveSpeed");
            stat.AttackRange += GetStatValue(Proproty03, "AttackRange");
            stat.PhysicalPowerDefense += GetStatValue(Proproty03, "PhysicalPowerDefense");
            stat.HP += GetStatValue(Proproty03, "HP");
            stat.MP += GetStatValue(Proproty03, "MP");
        }
    }
    #endregion


    // 특정 속성 값을 추출하는 함수
    public static float GetStatValue(List<Tuple<string, float>> attributes, string attributeName)
    {
        var attribute = attributes.FirstOrDefault(p => p.Item1 == attributeName);
        return attribute != null ? attribute.Item2 : 0f; // 해당 속성이 없으면 0 반환
    }
}



