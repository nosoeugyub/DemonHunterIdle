using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-08-29
/// 작성자     : 성엽 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 각각슬롯들마다의 데이터                                    
/// </summary>
public class HunteritemData 
{
    public Utill_Enum.SubClass Class { get; set; } //캐릭터 종류부분
    public Utill_Enum.EquipmentType Part { get; set; } // 슬롯이 장착될 캐릭터의 부위
    public Utill_Enum.DrawerGrade DrawerGrade { get; set; } // 아이템 추출기 등급
    public string Name { get; set; } // 슬롯에 장착된 아이템 이름
    public bool isEquip { get; set; } // 해당 데이터를 장착했는지 안했는지
    public List<string> FixedOptionTypes { get; set; } // 고정 옵션 종류
    public List<double> FixedOptionPersent { get; set; } // 고정 옵션 퍼센트

    public List<int> EquipCountList { get; set; } //소환에서 보유중인 장비 갯수 리스트
    public List<string> HoldOption { get; set; } // 보유 효과
    public List<int> HoldOptionValue { get; set; } // 보유 효과 값



    // 이 필드는 직렬화에서 제외됩니다 로컬연산하는데 사용 저장/로드와 상관없는 변수입니다.
    [JsonIgnore] 
    public int ItemContainsType { get; set; } // (슬롯 타입)
    [JsonIgnore]
    public Utill_Enum.Grade ItemGrade { get; set; } // 아이템 등급
    [JsonIgnore]
    public List<double> FixedOptionValues { get; set; } // 고정 옵션 값 
    [JsonIgnore]
    public int TotalLevel { get; set; } // 평균 레벨값

   //장비이름으로 해당 등급 찾기

    public static Utill_Enum.Grade FindItemGradeForName(string Itemname)
    {
        // Enum에 있는 등급 이름을 순회
        foreach (Utill_Enum.Grade grade in Enum.GetValues(typeof(Utill_Enum.Grade)))
        {
            // 아이템 이름에 해당 등급 이름이 포함되었는지 확인
            if (Itemname.Contains(grade.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return grade; // 등급을 반환
            }
        }

        // 등급이 발견되지 않으면 기본값 반환 (예: 일반 등급)
        return Utill_Enum.Grade.None; // 또는 적절한 기본값 설정
    }

    /// <summary>
    /// EquipmentType기반으로 특정 부위의 HunterItemData 반환
    /// </summary>
    public static HunteritemData GetHunteritemData(List<HunteritemData> datalist, EquipmentType equipmentType)
    {
        for (int i = 0; i < datalist.Count;i++)
        {
            if (datalist[i].Part == equipmentType)
                return datalist[i];
        }
        return null;
    }

    //현재 장착한 아이템찾기
    public Item SeletItem(List<HunteritemData> datalist, HunteritemData data)
    {
        Item currentitme = new Item();
        for (int i = 0; i < datalist.Count; i++)
        {
            if (datalist[i].Class == data.Class && datalist[i].Part == data.Part)
            {
                currentitme = InventoryManager.Instance.GetItem(datalist[i].Name);
                return currentitme;
            }
        }
        return currentitme;
    }
    //슬롯하나 저장 
    public static void Save_Slot(List<HunteritemData> datalist, HunteritemData data)
    {
        for (int i = 0; i < datalist.Count; i++)
        {
            if (datalist[i].Class == data.Class && datalist[i].Part == data.Part)
            {
                //장착 변수 저장
                datalist[i] = data;
            }
        }
    }

    //기존 부위 아이템 팔고 새데이터로 초기화
    public static void BasicItemSellInit(List<HunteritemData> datalist, HunteritemData data , HunterStat stat)
    {
        Item currentitme = new Item();
        int sellamount = 0;
        for (int i = 0; i < datalist.Count; i++)
        {
            if (datalist[i].Class == data.Class && datalist[i].Part == data.Part)
            {
                //기존아이템이 빈 슬롯인지 체크
                currentitme = InventoryManager.Instance.GetItem(datalist[i].Name);
                if (currentitme.GetName != null)
                {
                    //판매가
                    sellamount = GetRandomValueBetween(currentitme.SaleResourceCount1);

                    //기존 장비는 판매
                    ResourceManager.Instance.Pluse_ResourceType(currentitme.SaleResourceType1, sellamount);

                    //장비 해제
                    CostumeStlyer.RemovePartOfCostumeStyle(datalist[i].Class, datalist[i].Part);
                    ///스텟 적용
                    HunterStat.RemoveChangOptionToStat(stat, datalist[i]);
                }
                //새로운 아이템 바로 장착 변수 저장 && 스텟 적용
                datalist[i] = data;
                HunterStat.AddChangOptionToStat(stat, data);
                datalist[i].isEquip = true;
                return;
            }
        }
    }



    //팔수있는지 없는지 확인하는 함수 
    public static (string SellTpye , Utill_Enum.Resource_Type SellResourcertpye , int sellamount , bool ishighOption) CheckSellItem(List<HunteritemData> datalist, HunteritemData data)
    {
        StringBuilder stbur = new StringBuilder();
        Item currentitme = new Item();
        Item newItem = new Item();
        string SellTpye = string.Empty;
        Utill_Enum.Resource_Type SellResourcertpye = Utill_Enum.Resource_Type.None;
        int sellamount = 0;
        bool ishighOption = false;

        for (int i = 0; i < datalist.Count; i++)
        {
            if (datalist[i].Class == data.Class && datalist[i].Part == data.Part)
            {
                currentitme = InventoryManager.Instance.GetItem(datalist[i].Name);
                newItem = InventoryManager.Instance.GetItem(data.Name);
                SellResourcertpye = currentitme.SaleResourceType1;

                //예외  해당슬롯이 빈슬롯이였으면
                if (currentitme.GetName == null)
                {
                    //새로운 아이템 바로 장착 변수 true
                   
                    ishighOption = true;
                    return (SellTpye , SellResourcertpye, sellamount , ishighOption);
                }



                //조건0 등급이 높다면
                int currentgradeindex = (int)datalist[i].ItemGrade;
                int newitemgradeindex= (int)data.ItemGrade;
                if (currentgradeindex < newitemgradeindex)
                {
                    //판매가
                     //sellamount = GetRandomValueBetween(currentitme.SaleResourceCount1);
                    
                    //기존 장비는 판매
                    //CurrencyManager.Instance.Pluse_ResourceType(currentitme.SaleResourceType1, sellamount);

                    //새로운 아이템 바로 장착 변수 저장
                    //datalist[i] = data;
                    //datalist[i].isEquip = true;
                    ishighOption = true;
                    SoundManager.Instance.PlayAudio("DrawerTopOption");
                    return (SellTpye, SellResourcertpye, sellamount , ishighOption);
                }
                //조건1 기존장비 레벨보다 낮으면 자동으로 판매 (새로운 아이템 판매)
                if (datalist[i].TotalLevel > data.TotalLevel)
                {
                    //새로운 아이템 판매로직
                     sellamount = GetRandomValueBetween(newItem.SaleResourceCount1);
                    //새로운 잗비 장비는 판매
                    ResourceManager.Instance.Pluse_ResourceType(newItem.SaleResourceType1, sellamount);
                    SoundManager.Instance.PlayAudio("ItemSell");

                    //새로운 아이템 데이터 초기화
                    //data.TotalLevel = datalist[i].TotalLevel;

                    //장비 장착 해제이후 스텟도 감소




                    //데이터 구성
                    string tilte = LocalizationTable.Localization("Sell​LowerOptions");
                    string result = LocalizationTable.Localization("Sell");
                    stbur.Clear();
                    stbur.Append(tilte);
                    stbur.AppendLine();
                    stbur.Append(result);
                    SellTpye = stbur.ToString();
                    ishighOption = false;
                    SoundManager.Instance.PlayAudio("DrawerLowerOption");
                    return (SellTpye, SellResourcertpye, sellamount, ishighOption);
                }
                //조건1-2 기존장비 레벨과 같으면 보류 (새로운 아이템으로 교체)
                else if (datalist[i].TotalLevel == data.TotalLevel)
                {
                    //판매가
                    //sellamount = GetRandomValueBetween(currentitme.SaleResourceCount1);
                    //기존 장비는 판매
                    //CurrencyManager.Instance.Pluse_ResourceType(currentitme.SaleResourceType1, sellamount);
                    //새로운 아이템 바로 장착 변수 저장
                    //datalist[i] = data;
                    //datalist[i].isEquip = true;

                    //데이터구성
                    string tilte = LocalizationTable.Localization("SameOption");
                    string result = LocalizationTable.Localization("Decision");
                    stbur.Clear();
                    stbur.Append(tilte);
                    stbur.AppendLine();
                    stbur.Append(result);
                    SellTpye = stbur.ToString();
                    ishighOption = true;
                    SoundManager.Instance.PlayAudio("DrawerTopOption");
                    return (SellTpye, SellResourcertpye, sellamount , ishighOption);
                }
                //조건2 기존장비 레벨보다 높으면 자동으로 장착 (새로운 아이템으로 교체)
                else if (datalist[i].TotalLevel < data.TotalLevel)
                {
                    //판매가
                    //sellamount = GetRandomValueBetween(currentitme.SaleResourceCount1);
                    //기존 장비는 판매
                    //CurrencyManager.Instance.Pluse_ResourceType(currentitme.SaleResourceType1, sellamount);
                    //새로운 아이템 바로 장착 변수 저장
                    // datalist[i] = data;
                    //datalist[i].isEquip = true;
                    SoundManager.Instance.PlayAudio("DrawerTopOption");
                    //데이터구성
                    string tilte = LocalizationTable.Localization("ChangeHightOptions");
                    string result = LocalizationTable.Localization("Decision");
                    stbur.Clear();
                    stbur.Append(tilte);
                    stbur.AppendLine();
                    stbur.Append(result);
                    SellTpye = stbur.ToString();
                    ishighOption = true;
                    return (SellTpye, SellResourcertpye, sellamount , ishighOption);
                }
            }
           
        }
        return (SellTpye, SellResourcertpye, sellamount , ishighOption);
    }








    //장비레벨구하는 함수
    public static int Total_OptionLevel(List<double> list , Item item)
    {
        double totalOptionLevel = 0;
        int optionCount = item.GetOptions.Count;
        int listCount = list.Count;
        // 아이템 이름과 옵션을 미리 캐싱하여 사용
        string itemName = item.GetName;
        // 리스트 인덱스
        int i = 0;
        for (int z = 0; z < optionCount; z++)
        {
            // 연산에 필요한 값들 캐싱
            double finalValue = item.FixedOptionFinalValue[z];
            double maxValue = item.FixedOptionMaxValue[z];

            // 1차 연산
            double tempLevel_1 = finalValue / maxValue;

            // 2차 연산
            double tempLevel_2 = list[i] / tempLevel_1;

            // 3차 연산 (누적)
            totalOptionLevel += tempLevel_2;

            i++; // 리스트 인덱스 증가

            // 리스트의 길이를 넘어서는 경우는 처리 안 함
            if (i >= listCount)
                break;
        }

        // 평균값 계산
        double averageOptionLevel = totalOptionLevel / optionCount;

        // 1000을 곱한 후 소수점 이하 첫째 자리에서 버림
        averageOptionLevel *= 1000.0f;

        // 결과값 정수로 변환
        int resultNumber = (int)Math.Floor(averageOptionLevel);
        return resultNumber;
    }

    public static int GetRandomValueBetween(int[] array)
    {
        int minValue = array[0];
        int maxValue = array[1];

        System.Random rand = new System.Random();
        int randomValue = (int)(rand.NextDouble() * (maxValue - minValue) + minValue);
        return randomValue;
    }
    public static HunteritemData DeepCopy(HunteritemData original)
    {
        HunteritemData copy = new HunteritemData();
        copy.Name = original.Name;
        copy.Part = original.Part;
        copy.Class = original.Class;
        copy.DrawerGrade = original.DrawerGrade;
        copy.ItemGrade = original.ItemGrade;
        copy.FixedOptionTypes = new List<string>(original.FixedOptionTypes); // 리스트의 경우 새롭게 복사
       // copy.FixedOptionValues = new List<double>(original.FixedOptionValues);
        copy.FixedOptionPersent = new List<double>(original.FixedOptionPersent);
        copy.TotalLevel = original.TotalLevel;
        copy.isEquip = original.isEquip;
        // 필요한 다른 필드들도 복사
        if (original.EquipCountList != null)
        {
            copy.EquipCountList = new List<int>(original.EquipCountList);
        }
        if (original.HoldOption != null)
        {
            copy.HoldOption = new List<string>(original.HoldOption);
        }
        if (original.HoldOptionValue != null)
        {
            copy.HoldOptionValue = new List<int>(original.HoldOptionValue);
        }
        return copy;
    }




}
