using CodeStage.AntiCheat.ObscuredTypes;
using NSY;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 아이템 장착을 관리하는 매니저
/// </summary>
public class EquipmentItemManager : MonoSingleton<EquipmentItemManager>
{
    [SerializeField]
    private float itemCellSize = 150f;
    [SerializeField]
    private List<EquipmentItemCell> equipmentItemCellList = new List<EquipmentItemCell>();
    private Dictionary<EquipmentType , EquipmentItemCell> equipmentItemCellDictionary = new Dictionary<EquipmentType,EquipmentItemCell>();
    private bool isInit = false;

    //nsycode
    public List<HunteritemData> _currentItemList;//팝업 켰을때 현재 장착중인 데이터를 말함


    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSequence_SendGameEventHandler += GameSequence;

        //장착 / 해제 이벤트 
        GameEventSystem.Equipment_EuipSlot_Event += Euipment_Slot;
        GameEventSystem.UnEquipment_EuipSlot_Event += UnEuipment_Slot;
    }

    private void UnEuipment_Slot(HunteritemData type)
    {
        if (type.Name == string.Empty)
        {
            return;
        }
        EquipmentItemCell tempcell = new EquipmentItemCell();
        tempcell = FindCellData(type);
        //해당 슬롯찾고 해당 슬롯에 이미지 내리기
        tempcell.init_Slot();
        //장비 해제
        CostumeStlyer.RemovePartOfCostumeStyle(type.Class, type.Part);
        ///스텟 적용
        int hunterindex = (int)type.Class;
        HunterStat.RemoveChangOptionToStat(DataManager.Instance.Hunters[hunterindex].Orginstat, type);

    }

    private void Euipment_Slot(HunteritemData type)
    {
        if (type.Name == string.Empty)
        {
            return;
        }

        EquipmentItemCell tempcell = new EquipmentItemCell();
        //해당 슬롯 찾고 해당슬롯에 이미지 띄우기
        tempcell = FindCellData(type);
        string itemgrade = type.ItemGrade.ToString();
        string itemname = type.Name;
        tempcell.SettingImg(itemname, itemgrade, type.TotalLevel);
        //장비 교체 및 장착
        CostumeStlyer.ChangePartOfCostumeStyle(type.Class, type);
        //스텟 적용
        int hunterindex = (int)type.Class;
        HunterStat.AddChangOptionToStat(DataManager.Instance.Hunters[hunterindex].Orginstat, type);
    }
    //헌터 현재 선택한 헌터의 슬롯 찾기 
    public EquipmentItemCell FindCellData(HunteritemData type)
    {
        EquipmentItemCell tempcell = new EquipmentItemCell();
        
        tempcell = equipmentItemCellList.Find(c => c.equipmentType == type.Part);

        return tempcell;
    }

    private bool GameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Enum_GameSequence.InGame:
                isInit = false;
                equipmentItemCellDictionary.Clear();
                return true;
        }
        return true;
    }

    public void InitAllCell(SubClass subClass)
    {
    }
    //헌터들 전체로 슬롯셋팅해주는곳
    public void AllSetCell(SubClass subClass)
    {
    }


    //유저데이터 기반으로 헌터의 슬롯을 셋팅해주는 곳
    public void SettingCellList()
    {
        //현재 위치가 어떤 헌터인지 캐싱 0 헌터  1 가디언 2 메이지 
        int HunterIndex = GameDataTable.Instance.User.currentHunter;
        //해당헌터의 슬롯 바인딩
        switch (HunterIndex)
        {
            case 0:
                BindingHunterSlotData(GameDataTable.Instance.HunterItem.Archer);
                break;
            case 1:
                BindingHunterSlotData(GameDataTable.Instance.HunterItem.Guardian);
                break;
            case 2:
                BindingHunterSlotData(GameDataTable.Instance.HunterItem.Mage);
                break;
            default:
                break;
        }


        //해당 데이터 기반으로 아이템만들어 바인딩
        foreach (var cell in equipmentItemCellList)
        {
            //데이터 가공...
            List<Option> options = new List<Option>();
            List < ObscuredDouble > optionvalue = new List<ObscuredDouble>();   
            for (int i = 0; i < cell.CurrentHunterItem.FixedOptionTypes.Count; i++)
            {
                Option option = CSVReader.ParseEnum<Option>(cell.CurrentHunterItem.FixedOptionTypes[i]);
                options.Add(option);
                optionvalue.Add(cell.CurrentHunterItem.FixedOptionValues[i]);
            }
            if (cell.CurrentHunterItem.Name == "") //아이템이름이 없음 == 저장된게 없음
            {
                continue;
            }
            else
            {
                
            }
        }
    }


    //각 슬롯들을 해당 헌터가 가지고있는 슬롯들의 아이템 데이터로 바인딩 슬롯 타입 유형도 여기서 정해줌
    public void BindingHunterSlotData(List<HunteritemData> hunterdatalist)
    {
        int i = 0;
        foreach (var item in equipmentItemCellList)
        {
            hunterdatalist[i].Part = item.equipmentType;
            if (item.equipmentType == EquipmentType.Pet)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Hat)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Back)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Wing)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Mask)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Necklace)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Earrings)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            if (item.equipmentType == EquipmentType.Cloak)
            {
                hunterdatalist[i].ItemContainsType = 1;
            }
            i++;
        }

 
        foreach (var item in equipmentItemCellList) //해당 지역변수리스트 만들기
        {
            //파츠 위치 맞는 데이터 바인딩
            HunteritemData partcell = hunterdatalist.Find(c => c.Part == item.equipmentType);
            if (partcell != null)
            {
                item.CurrentHunterItem = partcell;

                if (item.CurrentHunterItem.Name != "" && item.CurrentHunterItem.isEquip == true)
                {
                    item.EquipmentSetting();
                    Utill_Enum.Grade grade = HunteritemData.FindItemGradeForName(item.CurrentHunterItem.Name);

                    var class_itme = GameDataTable.Instance.EquipmentList[item.CurrentHunterItem.Class];

                    if(item.CurrentHunterItem.EquipCountList == null) //가챠 아이템 아닐 경우에만
                    {
                        foreach (var hunteritem in class_itme)
                        {
                            // class_itme 의 키값중에  classname 와 Gradename 가 함께 포함되어있으면 해당  value 값을 temp저장
                            // 클래스 이름과 등급 이름이 포함된 키를 찾기
                            if (hunteritem.Key.Contains(item.CurrentHunterItem.Class.ToString()) && hunteritem.Key.Contains(item.CurrentHunterItem.Name))
                            {
                                // 조건에 맞는 아이템을 temp에 저장
                                item.CurrentHunterItem.TotalLevel = HunteritemData.Total_OptionLevel(item.CurrentHunterItem.FixedOptionPersent, hunteritem.Value);//평균레벨
                            }
                        }
                    }
                    item.CurrentHunterItem.ItemGrade = grade;
                    item.SettingImg(item.CurrentHunterItem.Name, grade.ToString(), item.CurrentHunterItem.TotalLevel);
                }
                else
                {
                    item.init_Slots();
                }
                
            }

        }
    }
}