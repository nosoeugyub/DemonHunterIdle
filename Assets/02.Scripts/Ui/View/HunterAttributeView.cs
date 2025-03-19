using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Text;

/// <summary>
/// 작성일자   : 2024-07-11
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 속성 수치 UI 표기                                        
/// </summary>
public class HunterAttributeView : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI[] HunterAttributeNameArray;
    [SerializeField] TextMeshProUGUI[] HunterAttributeBtnNameArray;
    [SerializeField] TextMeshProUGUI[] HunterAttributeDescArray;
    [SerializeField] TextMeshProUGUI[] HunterAttributelevelArray;
    [SerializeField] Button[] HunterAttributeButtonArray;

    [SerializeField] TextMeshProUGUI AttributeTile;
    [SerializeField] TextMeshProUGUI TotalAttributeValue;
    [SerializeField] Button ResetBtn;
    [SerializeField] Image[] HunterAttributerImage;
    public Toggle[] AttributeToogle;

    [SerializeField] Basic_Prefab[] basic_prefab;
    StringBuilder stb = new StringBuilder();
    public int Charactorindex = -1;
    private string optionNameTextColorCode = "";
    private string defaultTextColorCode = "";

    //GameDataTable.Instance.UserData.HunterAttribute[GameDataTable.Instance.UserData.currentHunter]

    public void Init_Toogle()
    {
        for (int i = 0; i < AttributeToogle.Length; i++)
        {
            AttributeToogle[i].isOn = false;
        }
    }

    //언어 및 이미지 설저
    public void SetImageHunter(Utill_Enum.SubClass subclass)
    {
        switch (subclass)
        {
            case Utill_Enum.SubClass.Archer:
                //속성이름 텍스트화
                HunterAttributeNameArray[0].text = LocalizationTable.Localization("NatualPower");
                HunterAttributeNameArray[1].text = LocalizationTable.Localization("DemonPower");
                HunterAttributeNameArray[2].text = LocalizationTable.Localization("HumanPower");

                //이미지도 셋팅
                HunterAttributerImage[0].sprite = Utill_Standard.GetUiSprite("Natural_Power");
                HunterAttributerImage[1].sprite = Utill_Standard.GetUiSprite("Demon_Power");
                HunterAttributerImage[2].sprite = Utill_Standard.GetUiSprite("Human_Power");
                break;
            case Utill_Enum.SubClass.Guardian:
                //속성이름 텍스트화
                HunterAttributeNameArray[0].text = LocalizationTable.Localization("NatualPower");
                HunterAttributeNameArray[1].text = LocalizationTable.Localization("WildPower");
                HunterAttributeNameArray[2].text = LocalizationTable.Localization("HumanPower");


                //이미지도 셋팅
                HunterAttributerImage[0].sprite = Utill_Standard.GetUiSprite("Natural_Power");
                HunterAttributerImage[1].sprite = Utill_Standard.GetUiSprite("Willd_Power");
                HunterAttributerImage[2].sprite = Utill_Standard.GetUiSprite("Human_Power");
                break;
            case Utill_Enum.SubClass.Mage:
                //속성이름 텍스트화
                HunterAttributeNameArray[0].text = LocalizationTable.Localization("NatualPower");
                HunterAttributeNameArray[1].text = LocalizationTable.Localization("DemonPower");
                HunterAttributeNameArray[2].text = LocalizationTable.Localization("HumanPower");
                break;
            default:
                break;
        }
    }

    //속성 값들을 전부 받아와서  표시해주는 함수
    public void UpdateUi_Attribute(ObscuredInt[] Attributearray , Utill_Enum.SubClass subclass)
    {
        AttributeTile.text = LocalizationTable.Localization("Title_Attribute");

        //총속성이 0 개라면 강화버튼들 비활성화처리
        if (Attributearray[0] <= 0)
        {
            for (int i = 0; i < basic_prefab.Length; i++)
            {
                basic_prefab[i].SetTypeButton(Utill_Enum.ButtonType.DeActive); //비활성화 + 터치안되게끔
                HunterAttributeButtonArray[i].interactable = false;
            }
        }
        else
        {
            for (int i = 0; i < basic_prefab.Length; i++)
            {
                basic_prefab[i].SetTypeButton(Utill_Enum.ButtonType.Active); //비활성화 + 터치안되게끔
                HunterAttributeButtonArray[i].interactable = true;
            }
        }




        //속성이름 텍스트화
        SetImageHunter(subclass);


        //버튼 이름 텍스트화
        for (int i = 0; i < HunterAttributeBtnNameArray.Length; i++)
        {
            HunterAttributeBtnNameArray[i].text = LocalizationTable.Localization("Button_Upgrade");
        }

        //속성 설명 텍스트화 + 값 적용
        //개별속성 레벨
        for (int i = 0;i < HunterAttributelevelArray.Length; i++)
        {
            HunterAttributelevelArray[i].text = Attributearray[i + 1].ToString(); // 0 은 토탈 값이라 1부터시작
        }


        //총속성
        stb.Clear();
        stb.Append(LocalizationTable.Localization("TotalAttribute"));
        stb.AppendLine();
        stb.Append(LocalizationTable.Localization(Attributearray[0].ToString()));
        TotalAttributeValue.text = stb.ToString();



        defaultTextColorCode = "#" + ColorUtility.ToHtmlStringRGB(HunterAttributeDescArray[0].GetComponent<TextColorSet>().GetOptionColor);
        optionNameTextColorCode = "#" + ColorUtility.ToHtmlStringRGB(HunterAttributeDescArray[0].GetComponent<TextColorSet>().GetOptionValueColor);

        //데이터 기반으로 출력하기
        var Proproty01 = HunterAttributeData.GetAttributer_value1(GameDataTable.Instance.HunterAttraibuteDataDic, Charactorindex);
        var Proproty02 = HunterAttributeData.GetAttributer_value2(GameDataTable.Instance.HunterAttraibuteDataDic, Charactorindex);
        var Proproty03 = HunterAttributeData.GetAttributer_value3(GameDataTable.Instance.HunterAttraibuteDataDic, Charactorindex);

        stb.Clear();
        //첫번째 글자에 표기
        foreach (var item in Proproty01) 
        {
            string name = item.Item1.ToString();
            float value = item.Item2;
            stb.Append("<color=").Append(optionNameTextColorCode).Append(">");
            stb.Append(LocalizationTable.Localization(name));
            stb.Append("</color>");
            stb.Append("<color=").Append(defaultTextColorCode).Append(">");
            stb.Append(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, name, (value * Attributearray[1])));
            stb.Append("</color>");
            stb.Append("(");
            stb.Append(Tag.UpArrowChar);
            stb.Append(value);
            stb.Append(")");
            stb.AppendLine();
        }
        HunterAttributeDescArray[0].text = stb.ToString();

        stb.Clear();
        //첫번째 글자에 표기
        foreach (var item in Proproty02)
        {
            string name = item.Item1.ToString();
            float value = item.Item2;
            stb.Append("<color=").Append(optionNameTextColorCode).Append(">");
            stb.Append(LocalizationTable.Localization(name));
            stb.Append("</color>");
            stb.Append("<color=").Append(defaultTextColorCode).Append(">");
            stb.Append(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, name, (value * Attributearray[2])));
            stb.Append("</color>");
            stb.Append("(");
            stb.Append(Tag.UpArrowChar);
            stb.Append(value);
            stb.Append(")");
            stb.AppendLine();
        }
        HunterAttributeDescArray[1].text = stb.ToString();

        stb.Clear();
        //첫번째 글자에 표기
        foreach (var item in Proproty03)
        {
            string name = item.Item1.ToString();
            float value = item.Item2;
            stb.Append("<color=").Append(optionNameTextColorCode).Append(">");
            stb.Append(LocalizationTable.Localization(name));
            stb.Append("</color>");
            stb.Append("<color=").Append(defaultTextColorCode).Append(">");
            stb.Append(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, name, (value * Attributearray[3])));
            stb.Append("</color>");
            stb.Append("(");
            stb.Append(Tag.UpArrowChar);
            stb.Append(value);
            stb.Append(")");
            stb.AppendLine();
        }
        HunterAttributeDescArray[2].text = stb.ToString();
    }



}
