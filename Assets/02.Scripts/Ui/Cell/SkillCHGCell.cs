using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillCHGCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chgOptionNameText;
    [SerializeField] private TextMeshProUGUI chgCurValueText;
    [SerializeField] private TextMeshProUGUI chgNextValueText;

    public void SetCHGCell(string optionName,string curValue, string nextValue)
    {
        chgOptionNameText.text = LocalizationTable.Localization(optionName);

        chgCurValueText.text = curValue.ToString();
        chgNextValueText.text = nextValue.ToString();

        SetCHGValueText(optionName, curValue, chgCurValueText);
        SetCHGValueText(optionName, nextValue, chgNextValueText);
    }

    private void SetCHGValueText(string optionName,string value, TextMeshProUGUI textMesh)
    {
        //스트링형인 경우 그 값 그대로 사용
        if(!float.TryParse(value, out float val)) 
        {
            textMesh.text = value;
            return;
        }

        if(optionName == "CHGValue_SkillDuration")
        {
            textMesh.text = SetFloatString(val);
            return;
        }
        //아무 표기 없이 텍스트만
        if(optionName == "CHGValue_SplitPiercinNumber" || optionName == "CHGValue_UseMP" || optionName == "CHGValue_SkillDuration" ||
            optionName == "CHGValue_ChainLightningNumber" || optionName == "CHGValue_StunDuration" || optionName == "CHGValue_TargetNumber")
        {
            textMesh.text = SetNormalString(value);
            return;
        }
        //퍼센트가 붙은 텍스트
        textMesh.text = SetPercentString(val);
    }
    private string SetFloatString(float num, int digitNum = 1)
    {
        string tempStr = num.ToString($"F{digitNum}");
        tempStr = string.Format("{0:#,###}", tempStr);//천단위 콤마
        return tempStr;
    }
    
    private string SetNormalString(string num)
    {
        string tempStr = string.Format("{0:#,###}", num);//천단위 콤마
        return tempStr;
    }
    
    private string SetPercentString(float num)
    {
        if(num >= 1000 ||  num <= -1000)   
            return Utill_Math.FormatCurrency((int)num)+"%";
        return $"{num}%";
    }
}
