using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 재화 시각적 셋팅
/// /// </summary>

public class ResourceView : MonoBehaviour
{

    [SerializeField] private Image Resource_img_Gold;
    [SerializeField] private Image Resource_img_Dia;

    [SerializeField] private TextMeshProUGUI Resource_Value_Gold;
    [SerializeField] private TextMeshProUGUI Resource_value_Dia;



    public void Update_Resource(string name , int Value)
    {
        switch (name) 
        {
            case "Gold":
                GoldUpdate(Value);
                break;
            case "Dia":
                DiaUpdate(Value);
                break;
        }
    }

    public void GoldUpdate(int value)
    {
        //max재화 판단
        int ckeckMaxvalue = ResourceManager.Instance.Get_MaximumResource(Utill_Enum.Resource_Type.Gold);
        bool ischeck = ConstraintsData.CheckMaxValue(ckeckMaxvalue, value);

        //글씨 색상
        string textColorCode = ColorCodeData.GetTextColor(GameDataTable.Instance.ColorCodeDataDic, Tag.TEXT_RED1);
        Color color;
        Color WhiteColor = Color.white;
        if (ischeck)//최대값이랑 같거나 넘었으면?
        {
            //표기
            string checkmaxvaluestr = Utill_Math.FormatCurrency(ckeckMaxvalue);
            Resource_Value_Gold.text = checkmaxvaluestr.ToString();
            //색상 변경
            if (ColorUtility.TryParseHtmlString(textColorCode, out color))
            {
                Resource_Value_Gold.color = color; 
            }
               
        }
        else
        {
            //표기
            string valuestr = Utill_Math.FormatCurrency(value);
            Resource_Value_Gold.text = valuestr.ToString();
            Resource_Value_Gold.color = WhiteColor;
        }
        
    }
    public void DiaUpdate(int value)
    {
        //max재화 판단
        
        int ckeckMaxvalue = ResourceManager.Instance.Get_MaximumResource(Utill_Enum.Resource_Type.Dia);
        bool ischeck = ConstraintsData.CheckMaxValue(ckeckMaxvalue, value);


        //글씨 색상
        string textColorCode = ColorCodeData.GetTextColor(GameDataTable.Instance.ColorCodeDataDic, Tag.TEXT_RED1);
        Color color;
        Color WhiteColor = Color.white;
        if (ischeck)//최대값이랑 같거나 넘었으면?
        {
            //표기
            string ckeckMaxvaluestr = Utill_Math.FormatCurrency(ckeckMaxvalue);
            Resource_value_Dia.text = ckeckMaxvaluestr.ToString();
            //색상 변경
            if (ColorUtility.TryParseHtmlString(textColorCode, out color))
            {
                Resource_value_Dia.color = color;
            }

        }
        else
        {
            //표기
            string valuestr = Utill_Math.FormatCurrency(value);
            Resource_value_Dia.text = valuestr.ToString();
            Resource_value_Dia.color = WhiteColor;
        }
    }

}
