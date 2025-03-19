using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-07-21
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 요일스킬 슬롯 스크립트
/// /// </summary>
public class DailySkillUiElement : SkillUiElement
{
    [SerializeField] private Utill_Enum.Grade _option_grade;
    public Utill_Enum.Grade _Option_grade
    {
        get { return _option_grade; }
        set { _option_grade = value; }
    }

    [SerializeField] private float _option_value;
    public float OptionValue
    {
        get { return _option_value; }
        set { _option_value = value; }
    }

    [SerializeField] private string _option_name;
    public string _Option_name
    {
        get { return _option_name; }
        set { _option_name = value; }
    }

    [SerializeField] private TextMeshProUGUI _option_nametxt;
    public TextMeshProUGUI _Daily_Option_nametxt
    {
        get { return _option_nametxt; }
        set { _option_nametxt = value; }
    }



    public SetSpriteColor dailySetspritecolor;//등급별스프라이트색깔

    public override void UpdateSkillUi()
    {
        base.UpdateSkillUi();

        //등급표시 ..


        //등급표시에 따른... 정보표시
    }

    //
    public void SetTextSkillGrade(string ClassName , string SKillName , string skillvalue)
    {
        // StringBuilder를 사용하여 텍스트 포맷팅
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(ClassName);
        sb.AppendLine(SKillName);
        sb.AppendLine(skillvalue);

        // TextMeshProUGUI 컴포넌트에 텍스트 설정
        _Daily_Option_nametxt.text = sb.ToString();
    }
}
