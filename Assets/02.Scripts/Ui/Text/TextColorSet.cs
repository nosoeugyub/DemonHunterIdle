using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 작성일자 : 2024-07-30
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 텍스트 컬러관리
/// </summary>
public class TextColorSet : MonoBehaviour
{
    [SerializeField] Utill_Enum.Text_Color selectColor;
    public Utill_Enum.Text_Color SelectColor
    {
        get { return selectColor; }
        set { selectColor = value;  }
    }

    [Header("기본텍스트 색상")]
    public Color DefaultColor1;
    [Header("초록 계열")]
    [SerializeField] Color Green1;
    [Header("흰색 계열")]
    [SerializeField] Color White1;
    [Header("회색 계열")]
    [SerializeField] Color Gray1;
    [Header("회색 계열 2")]
    [SerializeField] Color Gray2;
    [Header("노란 계열")]
    [SerializeField] Color Yellow1;
    [Header("빨간 계열")]
    [SerializeField] Color Red1;


    [Header("옵션 종류 색깔")]
    [SerializeField] Color OptionColor;
    [Header("옵션 값 색깔")]
    [SerializeField] Color OptionValueColor;


    public Color GetGreen1 => Green1;
    public Color GetWhite1 => White1;
    public Color GetGray1 => Gray1;
    public Color GetGray2 => Gray2;
    public Color GetYellow1 => Yellow1;
    public Color GetRed1 => Red1;

    public Color GetOptionColor => OptionColor;
    public Color GetOptionValueColor => OptionValueColor;

    [SerializeField] TextMeshProUGUI _textMeshProUGUI;
    public TextMeshProUGUI textMeshProUGUI
    {
        get { return _textMeshProUGUI; }
        set { _textMeshProUGUI = value;}
    }


    public bool isLevelDiffecnt;
    private void Awake()
    {
        if (!isLevelDiffecnt)
        {
            TextColor(SelectColor);
        }
        
    }

    public void TextColor(Utill_Enum.Text_Color color)
    {
        switch (color)
        {
            case Utill_Enum.Text_Color.Default:
                textMeshProUGUI.color = DefaultColor1;
                break;
            case Utill_Enum.Text_Color.Red:
                textMeshProUGUI.color = Red1;
                break;
            case Utill_Enum.Text_Color.Yellow:
                textMeshProUGUI.color = Yellow1;
                break;
            case Utill_Enum.Text_Color.Gray:
                textMeshProUGUI.color = Gray1;
                break;
            case Utill_Enum.Text_Color.Gray2:
                textMeshProUGUI.color = Gray2;
                break;
            case Utill_Enum.Text_Color.Green:
                textMeshProUGUI.color = Green1;
                break;
            case Utill_Enum.Text_Color.White:
                textMeshProUGUI.color = White1;
                break;
            case Utill_Enum.Text_Color.OptionColor:
                textMeshProUGUI.color = OptionColor;
                break;
            case Utill_Enum.Text_Color.OptionValueColor:
                textMeshProUGUI.color = OptionValueColor;
                break;
        }
    }
}
