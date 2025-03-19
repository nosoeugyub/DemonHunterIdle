using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-08-29
/// 작성자     : 성엽 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 헌터들마다 장착한 아이템 슬롯과 아아템                                          
/// </summary>
public class HunterItem
{
    public List<HunteritemData> Archer;
    public List<HunteritemData> Guardian;
    public List<HunteritemData> Mage;


    public HunterItem()
    {
        Archer = new List<HunteritemData>();
        Guardian = new List<HunteritemData>();
        Mage = new List<HunteritemData>();
    }

    public void init_HunterItem(List<HunteritemData> _Archer, List<HunteritemData> _Guardian, List<HunteritemData> _Mage)
    {
        Archer = _Archer;
        Guardian = _Guardian;
        Mage = _Mage;
    }

    public static void Stat_UserDataEquipment(List<HunteritemData> DataList, HunterStat stat) //유저의 장비들을 체크하여 스텟에 더해주는 로직
    {
        foreach (var item in DataList)
        {
            if (item.EquipCountList != null) //가챠로 뽑은 아이템
            {
                if (item.Name != "")
                {
                    //가챠 전용 고정옵 계산 함수
                    item.FixedOptionValues = InventoryManager.Instance.GachaChangeFixedOptionValues(item.FixedOptionTypes, item.Name);
                    HunterStat.AddChangOptionToStat(stat, item);
                }
                //보유효과 계산
                HunterStat.AddAllHoldOptionToStat(stat, item);
            }
            else if (item.Name != "")
            {
                item.FixedOptionValues = InventoryManager.Instance.ChangeFixedOptionValues(item.FixedOptionTypes, item.FixedOptionPersent, item.Name);
                HunterStat.AddChangOptionToStat(stat, item);
            }
        }
    }

    public static void SetHunterItemDataWithItem(HunteritemData hunteritemData, Item item, List<double> itempersent = null)
    {
        //아이템 데이터 파싱
        hunteritemData.Name = item.GetName;
        hunteritemData.Part = item.equipmentItem.type;
        hunteritemData.Class = item.character;
        hunteritemData.ItemGrade = item.GetGrade;

        //hunterdata.DrawerGrade = (Utill_Enum.DrawerGrade)System.Enum.Parse(typeof(Utill_Enum.DrawerGrade), Drawername);
        if (item.HoldOption.Length > 0)
        {
            hunteritemData.HoldOption = new List<string>();
            hunteritemData.HoldOptionValue = new List<int>(item.HoldOptionValue);
            for (int i = 0; i < item.HoldOption.Length; i++)
            {
                hunteritemData.HoldOption.Add(item.HoldOption[i].ToString());
            }
        }

        hunteritemData.FixedOptionTypes = ItemDrawer.Instance.ChangeStringList(item.GetOptions);

        Tuple<List<ObscuredDouble>, List<ObscuredDouble>> fixedvalue = null;

        //고정옵션 max value 길이가 1 이상이면
        if (item.FixedOptionMaxValue.Length > 1)
        {
            //고정옵 구하는 함수
            fixedvalue = InventoryManager.Instance.SetFixedOptionValues(item);
            hunteritemData.FixedOptionValues = InventoryManager.Instance.ChangeFixedOptionValues(hunteritemData.FixedOptionTypes, ItemDrawer.Instance.ChangedoubleList(fixedvalue.Item2), item.GetName);
        }

        if (itempersent == null)
        {
            itempersent = new List<double> { 0, 0, 0 };
        }
        else
        {
            itempersent = ItemDrawer.Instance.ChangedoubleList(fixedvalue.Item2); //고정옵 퍼센트
        }
        hunteritemData.FixedOptionPersent = itempersent; //고정옵 퍼센트

        if (item.FixedOptionFinalValue.Length > 1)
        {
            hunteritemData.TotalLevel = HunteritemData.Total_OptionLevel(hunteritemData.FixedOptionPersent, item);//평균레벨
        }

    }

    //원하는 헌터의 원하는 부위에 원하는 아이템 셋팅하기
    public static void Add_PartItem(List<HunteritemData> Hunter_data, Utill_Enum.EquipmentType parts, string ItemName,
                                    string GradeName, bool isEquip, string Drawername, List<double> itempersent = null)
    {
        Item temp = new Item(); //type 부위와 , grade 등급에 맞춰서 나와야함
        var class_itme = GameDataTable.Instance.EquipmentList[Hunter_data[0].Class];
        string classname = Hunter_data[0].Class.ToString();
        string Gradename = GradeName;
        string Partname = parts.ToString();
        HunteritemData hunterdata = new HunteritemData();
        // itempersent가 null이면 기본값으로 {0, 0, 0} 할당



        foreach (var item in class_itme)
        {
            // class_itme 의 키값중에  classname 와 Gradename 가 함께 포함되어있으면 해당  value 값을 temp저장
            // 클래스 이름과 등급 이름이 포함된 키를 찾기
            if (item.Key.Contains(classname) && item.Key.Contains(Gradename) && item.Key.Contains(Partname))
            {
                // 조건에 맞는 아이템을 temp에 저장
                temp = item.Value;
                //아이템 데이터 파싱
                hunterdata.Name = temp.GetName;
                hunterdata.Part = temp.equipmentItem.type;
                hunterdata.Class = temp.character;
                hunterdata.ItemGrade = temp.GetGrade;
                if (Drawername != "")
                    hunterdata.DrawerGrade = (Utill_Enum.DrawerGrade)System.Enum.Parse(typeof(Utill_Enum.DrawerGrade), Drawername);
                hunterdata.FixedOptionTypes = ItemDrawer.Instance.ChangeStringList(temp.GetOptions);

                Tuple<List<ObscuredDouble>, List<ObscuredDouble>> fixedvalue = null;
                if (temp.FixedOptionMaxValue.Count() > 1) //데이터에 고정옵 최댓값이 있을때 == 가챠가 아닐때
                {
                    //고정옵 구하는 함수
                    fixedvalue = InventoryManager.Instance.SetFixedOptionValues(temp);
                    hunterdata.FixedOptionValues = InventoryManager.Instance.ChangeFixedOptionValues(hunterdata.FixedOptionTypes, ItemDrawer.Instance.ChangedoubleList(fixedvalue.Item2), temp.GetName);
                }
                else
                {
                    hunterdata.FixedOptionValues = InventoryManager.Instance.GachaChangeFixedOptionValues(hunterdata.FixedOptionTypes, temp.GetName);
                }

                if (itempersent == null)
                {
                    itempersent = new List<double> { 0, 0, 0 };
                }
                else if (fixedvalue != null)
                {
                    itempersent = ItemDrawer.Instance.ChangedoubleList(fixedvalue.Item2); //고정옵 퍼센트
                }
                hunterdata.FixedOptionPersent = itempersent; //고정옵 퍼센트
                if (temp.FixedOptionMaxValue.Count() > 1)
                    hunterdata.TotalLevel = HunteritemData.Total_OptionLevel(hunterdata.FixedOptionPersent, temp);//평균레벨
                else
                    hunterdata.TotalLevel = 0;
                hunterdata.isEquip = isEquip; //장착도 처리
                break; // 첫 번째 매칭 항목만 사용
            }
        }

        for (int i = 0; i < Hunter_data.Count; i++)
        {
            if (Hunter_data[i].Part == hunterdata.Part)
            {
                Hunter_data[i] = hunterdata;
            }
        }
    }

    //해당 장비들 모두 장착 변경
    public static void Equipment_UserDataEquipment(List<HunteritemData> DataList) //유저의 장비들을 체크하여 스텟에 더해주는 로직
    {
        foreach (var item in DataList)
        {
            if (item.Name != "")
            {
                item.isEquip = true;
            }
        }
    }
}
