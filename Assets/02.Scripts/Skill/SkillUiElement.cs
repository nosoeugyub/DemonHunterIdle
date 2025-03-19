using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬 슬롯 스크립트
/// /// </summary>
public class SkillUiElement : MonoBehaviour
{
    public Utill_Enum.Grade Grade;

    [SerializeField]private string skillName;
    public string SkillName
    {
        get { return skillName; }
        set { skillName = value; }  
    }
     private string spriteName;
    public string SpriteName
    {
        get { return spriteName; }
        set { spriteName = value; }
    }
    [SerializeField]private int rowindex;
    public int Rowindex
    {
        get { return rowindex; }
        set { rowindex = value; }
    }
    [SerializeField]private int clickindex;
    public int Clickindex
    {
        get { return clickindex; }
        set { clickindex = value; }
    }
     private ISkill _m_Skills;
    public ISkill m_Skills
    {
        get { return _m_Skills;}
        set { _m_Skills = value;}
    }
   [SerializeField] private bool _isEquip = false;
    public bool isEquip
    {
        get { return _isEquip; }
        set { _isEquip = value; }
    }

    [SerializeField] private int index;
    public int Index
    {
        get { return index; }
        set { index = value; }
    }
    public SetSpriteColor setspritecolor;
    public GameObject ActiveOverlay;
     private TextMeshProUGUI SkillCoolDownText; //쿨타임시간
    public TextMeshProUGUI _Option_nametxt; //스킬 이름
    public TextColorSet textcolorset;

    [SerializeField] private TextMeshProUGUI upgradeLevelText; //아이콘에 붙어있는 텍스트

    [SerializeField] private Image SkillIcon; //배경 스킬 이미지 

     private Image SkillImageFilld; //스킬 쿨타임돌아가는이미지
    //버튼 눌렀을때...
    [Header("Button")]
    public Basic_Prefab UseSkillBtn;
    public Button ClickSkillBtn;

    public Button skillInfoBtn;

    private float Cooltime = 0;

    private void Start()
    {
        GameEventSystem.GameUpdateSkillUIElement_GameEventHandler_Event += UpdateSkillUi;
        skillInfoBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.skillInfoPopUp.Show();
            GameEventSystem.Send_GameShowSkillInfo_GameEventHandler_Event(m_Skills);
        });
    }

    //그냥 다 null로 초기화..
    public void init()
    {
        SkillIcon.sprite = null;
    }

    public void DailySkillUi_init()
    {
        SpriteName = skillName;
        clickindex = 0;
    }

    public void NullSKillUi()
    {
        Cooltime = 0;
        SkillCoolDownText.text = "";
        SkillIcon.sprite = null;
        SkillImageFilld.sprite = null;

        // 스프라이트 알파값 0으로 설정
        Color backSkillImageColor = SkillIcon.color;
        backSkillImageColor.a = 0f;
        SkillIcon.color = backSkillImageColor;

        Color skillImageFillColor = SkillImageFilld.color;
        skillImageFillColor.a = 0f;
        SkillImageFilld.color = skillImageFillColor;
    }



    public virtual void UpdateSkillUi()
    {
        Sprite sprite = null;
        sprite = Utill_Standard.GetUiSprite(SpriteName);
        SkillIcon.sprite = sprite;

        if(m_Skills != null)
            upgradeLevelText.text ="+"+ m_Skills._upgradeAmount.ToString();
        //장착했을때
        if (isEquip)
        { 
            //버튼 비활성화
            UseSkillBtn.SetTypeButton(Utill_Enum.ButtonType.DeActive);
            //장착테두리 표시
            ActiveOverlay.gameObject.SetActive(true);
        }
        else
        {
            //버튼 활성화
            UseSkillBtn.SetTypeButton(Utill_Enum.ButtonType.Active);
            //장착테두리 제거
            ActiveOverlay.gameObject.SetActive(false);
        }
    }

    IEnumerator CoolDown()
    {
        float temp_cool = Cooltime;
        SkillImageFilld.fillAmount = 0;

        while (temp_cool < 0) 
        {
            temp_cool -= 1;
            SkillImageFilld.fillAmount = 1.0f / temp_cool;
            yield return Utill_Standard.WaitTimeFixedUpdate;
        }
        yield return null;
    }


    public void setgradesprite()
    {
        setspritecolor._Image.gameObject.SetActive(true);
        setspritecolor.SetColorOptionGradeSprite(Grade);
    }

    public void Desc_GradeSkill(List<Hunter> Hunters, string SKillName, string skillvalue , Hunter SpesicalHunter = null )
    {
        string ClassName;
        StringBuilder sb = new StringBuilder();
        if (SpesicalHunter != null)
        {
            if (SpesicalHunter.name == "Hunter_Archer")
            {
                ClassName = "궁수";
            }
            else if (SpesicalHunter.name == "Hunter_Guardian")
            {
                ClassName = "수호자";
            }
            else
            {
                ClassName = "마법사";
            }
            // StringBuilder를 사용하여 텍스트 포맷팅
           
            sb.AppendLine(ClassName);
            // GetOptionColor 및 GetOptionValueColor 사용하여 색상 적용
            string skillColors = ColorUtility.ToHtmlStringRGBA(textcolorset.GetOptionColor);
            string valueColors = ColorUtility.ToHtmlStringRGBA(textcolorset.GetOptionValueColor);

            sb.AppendLine($"<color=#{skillColors}>{SKillName}</color>");
            sb.AppendLine($"<color=#{valueColors}>{skillvalue}</color>");

            // TextMeshProUGUI 컴포넌트에 텍스트 설정
            _Option_nametxt.text = sb.ToString();
            return;
        }



        if (Hunters.Count >= 3)
        {
            ClassName = "전체";
        }
        else
        {
            if (Hunters[0].name  == "Hunter_Archer" )
            {
                ClassName = "궁수";
            }
            else if(Hunters[0].name == "Hunter_Guardian")
            {
                ClassName = "수호자";
            }
            else
            {
                ClassName = "마법사";
            }
        }
        // StringBuilder를 사용하여 텍스트 포맷팅
        sb.AppendLine(ClassName);
        // GetOptionColor 및 GetOptionValueColor 사용하여 색상 적용
        string skillColor = ColorUtility.ToHtmlStringRGBA(textcolorset.GetOptionColor);
        string valueColor = ColorUtility.ToHtmlStringRGBA(textcolorset.GetOptionValueColor);

        sb.AppendLine($"<color=#{skillColor}>{SKillName}</color>");
        sb.AppendLine($"<color=#{valueColor}>{skillvalue}</color>");

        // TextMeshProUGUI 컴포넌트에 텍스트 설정
        _Option_nametxt.text = sb.ToString();


    }
}
    