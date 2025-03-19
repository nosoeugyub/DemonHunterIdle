using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-39
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : ItemValue 구조체 
/// </summary>

public struct ItemValue
{

    private Grade grade;                        //장비 등급
    public Grade GetGrade => grade;


    private EquipmentType equipmentType;        //장비 부위
    public new EquipmentType GetType => equipmentType;

    private string itemName;                         //장비 이름
    public string GetName => itemName;

    private Optiontype optionType;                  //장비 옵션 타입 (고정 Or 랜덤)
    public Optiontype GetOptionType => optionType;


    public Option option;                     //장비 옵션

    private double minValue;                        //최소 밸류
    public double MinValue => minValue;

    private double maxValue;                        //최대 밸류
    public double MaxValue => maxValue;

    private double finalValue;
    public double FinalValue => finalValue;

    private double valueUnit;                       //몇 배수로 오르는지 
    public double Unit => valueUnit;

    private double[] weightValue;                   //나오는 확률
    public double WeightValue(int index) => weightValue[index];

    public ItemValue(Grade grade, EquipmentType equipmentType, string itemName, Optiontype optionType, Option option,
        double minValue, double maxValue, double finalvalue, double valueUnit, double[] weightValue)
    {
        this.grade = grade;
        this.equipmentType = equipmentType;
        this.itemName = itemName;
        this.optionType = optionType;
        this.option = option;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.valueUnit = valueUnit;
        this.weightValue = weightValue;
        this.finalValue = finalvalue;
    }

   
}
