using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemValue;
using static Utill_Enum;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.TextCore.Text;
using static Item;

/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : Item 구조체 아이템이 가지고 있는 정보들                              
/// </summary>


public struct Item
{
    /// <summary>
    /// 장비 아이템 구조체
    /// </summary>
    public struct EquipmentItem  
    {
        public EquipmentType type; //장비 타입
        public Option[] randomOptions; //랜덤 옵션 결정된값
        public ObscuredBool isEquip; //장착여부

        public EquipmentItem(EquipmentType type, Option[] options, bool isEquip)
        {
            this.type = type;
            randomOptions = options;
            this.isEquip = isEquip;
        }
    }

    public SubClass character; //어떤 헌터 아이템인지

    ObscuredInt num; //아이템 개수
    ObscuredInt maxCount; //아이템 최대 개수

    public EquipmentItem equipmentItem;  // 장비 아이템 객체 나타내는 변수

    private Grade grade; //아이템 등급
    public Grade GetGrade => grade;

    private ObscuredString itemName; //아이템 이름 
    public ObscuredString GetName { get => itemName; set => itemName = value; }


    private ObscuredString backgroundName; //배경 이름
    public ObscuredString GetBackgroundName => backgroundName;

    private Optiontype[] optionTypes; 
    public Optiontype[] GetOptionTypes => optionTypes;
    public Optiontype[] SetOptionTypes
    {
        set { optionTypes = value; }
    }

    #region  고정값
    private List<Option> options; // 고정 옵션
    public List<Option> GetOptions => options;

    private List<ObscuredDouble> fixedValues; // 고정 옵션 수치 

    public int[] FixedOptionMinValue;
    public int[] FixedOptionMaxValue;
    public int[] FixedOptionFinalValue;
    public float[] FixedOptionValueUnit;
    public int[] FixedOptionWeightValue;

    public Option[] HoldOption;
    public int[] HoldOptionValue;

    public List<ObscuredDouble> FixedValues
    {
        get { return fixedValues; }
        set { fixedValues = value; }
    }

    private List<ObscuredDouble> fixedValuesPercent; // 고정 옵션 퍼센트
    public List<ObscuredDouble> FixedValuesPercent
    {
        get { return fixedValuesPercent; }
        set { fixedValuesPercent = value; }
    }

    #endregion

    #region 랜덤값

    private List<Option> selectRandOptions; //랜덤옵션중에 RandomOptionCnt 개수만큼만 선택된 랜덤옵션
    public List<Option> SelectRandOptions
    {
        get { return selectRandOptions; }
        set { selectRandOptions = value; }
    }

    private List<ObscuredDouble> randomValues; //랜덤옵션 수치
    public List<ObscuredDouble> RandomValues
    {
        get { return randomValues; }
        set { randomValues = value; }
    }

    private List<ObscuredDouble> randomValuesPercent;  //랜덤옵션 퍼센트
    public List<ObscuredDouble> RandomValuesPercent
    {
        get { return randomValuesPercent; }
        set { randomValuesPercent = value; }
    }
    #endregion


    public Utill_Enum.Resource_Type SaleResourceType1;
    public int[] SaleResourceCount1;

    /// <summary>
    /// 아이템 생성자 CSV 읽을때 불림
    /// </summary>
    public Item(SubClass character, Grade grade, string itemName, string backgroundName, List<Option> options ,int[] _FixedOptionMinValue , int[] _FixedOptionMaxValue , int[] _FixedOptionFinalValue ,
                float[] _FixedOptionValueUnit, int[] _FixedOptionWeightValue, Option[] _Holdoptions , int[] _HoldOptionValue, Utill_Enum.Resource_Type _saleResourceType1, int[] _SaleResourceCount1)
    {
        this.character = character;
        this.grade = grade;
        this.itemName = itemName;
        this.backgroundName = backgroundName;
        this.options = options; //options;
        this.optionTypes = new Optiontype[0];
     
        fixedValues = new List<ObscuredDouble>();
        fixedValuesPercent = new List<ObscuredDouble>();

        selectRandOptions = new List<Option>();
        randomValues  = new List<ObscuredDouble>();
        randomValuesPercent = new List<ObscuredDouble>();



     FixedOptionMinValue = _FixedOptionMinValue;
     FixedOptionMaxValue = _FixedOptionMaxValue;
     FixedOptionFinalValue = _FixedOptionFinalValue;
     FixedOptionValueUnit = _FixedOptionValueUnit;
     FixedOptionWeightValue = _FixedOptionWeightValue;

    HoldOption = _Holdoptions;
     HoldOptionValue = _HoldOptionValue;


    equipmentItem = new EquipmentItem();
        num = 1;
        maxCount = 1;
        SaleResourceType1 = _saleResourceType1;
        SaleResourceCount1 = _SaleResourceCount1;
    }

    //장비 아이템 설정하여 새로운 장비 아이템 객체 생성
    public void SetEquipmentItem(EquipmentType type, Option[] options,bool isEquip)
                => equipmentItem = new EquipmentItem(type, options, isEquip);


    /// <summary>
    /// 서버나 로컬에서 받아온 데이터 바탕으로 아이템 정보 세팅 및 생성
    /// </summary>
    /// <param 아이템 이름="_name"></param>
    /// <param 아이템 개수="_num"></param>
    /// <param 고정 옵션 리스트="_FO"></param>
    /// <param 고정 옵션 퍼센트 리스트="_FOVP"></param>
    /// <param 랜덤 옵션 리스트="_RO"></param>
    /// <param 랜덤 옵션 퍼센트 리스트="_ROVP"></param>
    /// <param 장착 여부="_isEquip"></param>
    public Item(string _name, int _num, List<Option> _FO, List<ObscuredDouble> _FOVP, List<Option> _RO, List<ObscuredDouble> _ROVP, bool _isEquip)
    {
        itemName = _name;
        this = InventoryManager.Instance.GetItem(itemName);

        num = _num;
        equipmentItem.isEquip = _isEquip;

        options = _FO;
        fixedValuesPercent = _FOVP;
        
        
        selectRandOptions = _RO;
        randomValuesPercent = _ROVP;
        

        (grade, backgroundName) = GetGradeAndBackground(itemName);
    }

    


    /// <summary>
    /// 아이템 이름을 바탕으로 아이템 등급과 배경 반환
    /// </summary>
    private (Grade, string) GetGradeAndBackground(string itemName)
    {
        Grade itemGrade = Grade.None;
        string itemBackground = "";

        if (string.IsNullOrEmpty(itemName))
        {
            return (itemGrade, itemBackground);
        }

        if (itemName.Contains("Rare"))
        {
            itemGrade = Grade.Rare;
            itemBackground = "Rare"; // 예시 배경 이름
        }
        else if (itemName.Contains("Normal"))
        {
            itemGrade = Grade.Normal;
            itemBackground = "Normal"; // 예시 배경 이름
        }

        return (itemGrade, itemBackground);
    }

    public bool Null()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 로컬 세이브 형식 
    /// 이름, 고정옵션, 고정옵션 값, 랜덤으로 골라진 랜덤옵션, 랜덤 옵션 값, 개수, 장착여부
    /// </summary>
    public override string ToString()
    {
        return 
            $"Item:{{Name: {itemName}, " +
            $"FO: {Utill_Standard.ListToJson(options)}, " +
            $"FOVP: {Utill_Standard.ListToJson(fixedValuesPercent)}, " +
            $"RO: {Utill_Standard.ListToJson(selectRandOptions)}, " +
            $"ROVP: {Utill_Standard.ListToJson(randomValuesPercent)}, " +
            $"Num: {num}, " +
            $"IsEquip: {equipmentItem.isEquip}}}";
    }

    /// <summary>
    /// 암호화
    /// </summary>
    public void RandomizeKey_Inventory()
    {
        if (fixedValues != null)
        {
            for (int i = 0; i < fixedValues.Count; i++)
            {
                fixedValues[i].RandomizeCryptoKey();
            }
        }
        if (randomValuesPercent != null)
        {
            for (int i = 0; i < randomValuesPercent.Count; i++)
            {
                randomValuesPercent[i].RandomizeCryptoKey();
            }
        }
        if (itemName != null)
        {
            itemName.RandomizeCryptoKey();
        }

        num.RandomizeCryptoKey();
        maxCount.RandomizeCryptoKey();
        equipmentItem.isEquip.RandomizeCryptoKey();
    }
}
