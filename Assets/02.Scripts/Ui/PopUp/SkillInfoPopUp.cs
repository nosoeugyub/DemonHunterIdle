using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Utill_Enum;
using NSY;

/// <summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 스킬 정보 보여주는 팝업
/// </summary>
public class SkillInfoPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] private TextMeshProUGUI titleText;
    
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillUpGradeText; //스킬 아이콘에 +n 으로 뜨는 텍스트
    [SerializeField] private TextMeshProUGUI skillUpGradeButtonText; //버튼 텍스트
    [SerializeField] private TextMeshProUGUI skillUpGradeResourceAmountText; //필요 재화 양 텍스트
    [SerializeField] private TextMeshProUGUI coolTimeText;
    [SerializeField] private TextMeshProUGUI attackRangeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI damageFormulaText; //계산공식
    [SerializeField] private List<TextMeshProUGUI> dmgStringTextList;

    [SerializeField] private Transform CHGCellParent;
    [SerializeField] private SkillCHGCell chgCellPrefab;

    [SerializeField] private Image skillImage;
    [SerializeField] private Image skillEvolutionOneImage; //스킬 진화 1이미지 
    [SerializeField] private Image skillEvolutionTwoImage; //스킬 진화 2이미지 

    [SerializeField] private Image upgradeResourceImage;

    [SerializeField] private Button upgradeBtn;
    [SerializeField] private LevelLimitButton levelLimitButton;
    public ScrollRect scrollrect;

    private ISkill curSkill;
    private SubClass curSubClass;
    private int curPresetCount;
    private int ClickCount = 0;
    private bool isInit = false;
    private List<SkillCHGCell> chgCellList = new();
    private Basic_Prefab upgradeButtonBasicPrefab;

    private const string subDataInfoFomat = "{0} : {1}"; //재사용시간,피해범위 형식 코드
    private const int firstSpawnCHGCellNum = 5; //내부적으로 생성할 셀의 양

    public void Close()
    {
    }

    public void Hide()
    {
        ClickCount = 0;
    
        PopUpSystem.Instance.EscPopupListRemove();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
            scrollrect.verticalScrollbar.value = 1.0f;
            if(!isInit)
            {
                upgradeBtn.onClick.AddListener(OnUpgradeSkillClicked);
                upgradeButtonBasicPrefab = upgradeBtn.GetComponent<Basic_Prefab>();
                InstantiateCell(firstSpawnCHGCellNum);
                isInit = true;
            }
        }
        else
        {
            Hide();
        }
    }

    public void SetSkillInfo(ISkill skill,SubClass subClass,int presetCount)
    {
        curSkill = skill;
        curSubClass = subClass;
        curPresetCount = presetCount;

        BaseSkillData skillData = BaseSkillData.FindLevelForData(GetSkillListPerSubClass(subClass), skill.SkillName,skill._upgradeAmount);
        levelLimitButton.SettingLimitLevelButton(subClass,skillData.LevelLimit);

        SetBasicSkillInfo(skill.SkillName);
        SetSkillData(skillData);
        SetSkillCHGInfo(skillData);
        SetSkillDMGInfo(skillData);

        //레이아웃 재설정 코드 강제 호출
        //무거운 작업이지만 Vertical LayoutGroup에 셀 추가시 가끔 위치가 바로 갱신이 안 되는 현상이 있기 때문에 추가함
        //추후 성능 관련 확인 필요
        LayoutRebuilder.ForceRebuildLayoutImmediate(CHGCellParent.GetComponent<RectTransform>());
    }

    private Dictionary<string, List<BaseSkillData>> GetSkillListPerSubClass(SubClass subClass)
    {
        switch (subClass)
        {
            case SubClass.Archer:
                return GameDataTable.Instance.ArcherSkillList;
            case SubClass.Guardian:
                return GameDataTable.Instance.GurdianSkillLiat;
            case SubClass.Mage:
                //return GameDataTable.Instance.MageSkillLiat;
                break;
        }
        return null;
    }
    private Dictionary<string,int> GetUpgradeDicPerSubClass(SubClass subClass)
    {
        switch (subClass)
        {
            case SubClass.Archer:
                return GameDataTable.Instance.User.ArcherSkillDic;
            case SubClass.Guardian:
                return GameDataTable.Instance.User.GurdianSkillDic;
            case SubClass.Mage:
                //return GameDataTable.Instance.User.MageSkillDic;
                break;
        }
        return null;
    }

    /// <summary>
    /// CHG셀 생성
    /// </summary>
    /// <param name="instantiateNum">추가 생성할 값</param>
    private void InstantiateCell(int instantiateNum)
    {
        for(int i = 0; i < instantiateNum;i++)
        {
            SkillCHGCell cell = Instantiate(chgCellPrefab, CHGCellParent).GetComponent<SkillCHGCell>();
            chgCellList.Add(cell);
        }
    }
    
    /// <summary>
    /// 스킬 이름만 있으면 채워지는 UI + 이미지 세팅
    /// </summary>
    private void SetBasicSkillInfo(string skillName)
    {
        skillUpGradeButtonText.text = LocalizationTable.Localization("Button_Upgrade");    
        titleText.text = LocalizationTable.Localization("Title_Skill");
        damageFormulaText.text = LocalizationTable.Localization("DamageFormula");

        skillNameText.text = LocalizationTable.Localization("Skill_Name_" + skillName);
        descriptionText.text = LocalizationTable.Localization("Skill_Info_" + skillName);

        skillImage.sprite = Utill_Standard.GetUiSprite(skillName);
        //임시
        skillEvolutionOneImage.sprite = Utill_Standard.GetUiSprite(skillName);
        skillEvolutionTwoImage.sprite = Utill_Standard.GetUiSprite(skillName);
    }

    /// <summary>
    /// CHG,피해공식 제외한 UI 채워넣기
    /// </summary>
    private void SetSkillData(BaseSkillData skillData)
    {
        curSkill._upgradeAmount = GetUpgradeDicPerSubClass(curSubClass)[curSkill.SkillName];
        skillUpGradeText.text = "+" + Utill_Math.FormatCurrency(curSkill._upgradeAmount);
        skillUpGradeResourceAmountText.text = Utill_Math.FormatCurrency(skillData.ResourceCount);

        if(curSkill.SkillCoolDown > 0) //쿨다운 0초 이상
            coolTimeText.text = string.Format(subDataInfoFomat,LocalizationTable.Localization("SkillCooldown"), curSkill.SkillCoolDown);
        else //없으면 없음으로 표기
            coolTimeText.text = string.Format(subDataInfoFomat,LocalizationTable.Localization("SkillCooldown"), LocalizationTable.Localization("None"));

        if(curSkill.DamageRadius_ > 0)
        {
            attackRangeText.gameObject.SetActive(true);
            attackRangeText.text = string.Format(subDataInfoFomat, LocalizationTable.Localization("DamageRadius"), Utill_Math.FormatCurrency((int)(curSkill.DamageRadius_ * 100)));
        }
        else if(curSkill.SkillRange_ > 0)
        {
            attackRangeText.gameObject.SetActive(true);
            attackRangeText.text = string.Format(subDataInfoFomat, LocalizationTable.Localization("SkillRange"), Utill_Math.FormatCurrency((int)(curSkill.SkillRange_ * 100)));
        }
        else
        {
            attackRangeText.gameObject.SetActive(false);
        }

        if (GameDataTable.Instance.ConstranitsDataDic["MAX_SKILLUPGREADLEVEL"].Value <= curSkill._upgradeAmount)
            upgradeButtonBasicPrefab.SetTypeButton(ButtonType.DeActive);
        else
            upgradeButtonBasicPrefab.SetTypeButton(ButtonType.Active);

        upgradeResourceImage.sprite = Utill_Standard.GetItemSprite(skillData.REsourceType.ToString());
    }

    /// <summary>
    /// 정보창 CHG 값 채워넣기
    /// </summary>
    private void SetSkillCHGInfo(BaseSkillData skillData)
    {
        BaseSkillData nextLevelData = BaseSkillData.FindLevelForData(GetSkillListPerSubClass(curSubClass),curSkill.SkillName,curSkill._upgradeAmount+1);
        if (skillData.CHGValue.Count>chgCellList.Count) //create cell
        {
            InstantiateCell(skillData.CHGValue.Count - chgCellList.Count);
        }

        for(int i = 0; i < chgCellList.Count; i++)
        {
            if (skillData.CHGValue.Count <= i)
            {
                chgCellList[i].gameObject.SetActive(false);
                continue;
            }
            chgCellList[i].gameObject.SetActive(true);
            string curVal = (nextLevelData != null) ? nextLevelData.CHGValue[i] : "";
            chgCellList[i].SetCHGCell(skillData.CHGName[i],skillData.CHGValue[i],curVal);
        }
    }

    /// <summary>
    /// 정보창 하단 피해공식 string 값 채워넣기
    /// </summary>
    private void SetSkillDMGInfo(BaseSkillData skillData)
    {
        if (skillData.DMGValue.Count<=0)
        {
            for (int i = 0; i < dmgStringTextList.Count; i++)
            {
                dmgStringTextList[i].gameObject.SetActive(false);
            }
            damageFormulaText.gameObject.SetActive(false);
            return;
        }
        
        damageFormulaText.gameObject.SetActive(true);
        string damageFormat = LocalizationTable.Localization("SkillDamageFormula");
        HunterStat hunterStat = DataManager.Instance.GetHunterUsingSubClass(curSubClass)._UserStat;
        for (int i = 0; i < dmgStringTextList.Count; i++)
        {
            if (skillData.DMGValue.Count <= i)
            {
                dmgStringTextList[i].gameObject.SetActive(false);
                continue;
            }
            dmgStringTextList[i].gameObject.SetActive(true);

            //계산 위해 현재 스탯 가져옴
            float tmpStat = 0f;
            tmpStat = HunterStat.GetMaximumStat(HunterStat.GetUserStatPerOption(hunterStat, skillData.optionliast[i], true), skillData.optionliast[i].ToString());
            
            //퍼센테이지 계산
            tmpStat *= (skillData.DMGValue[i] * 0.01f);
            string finalValue = Utill_Math.FormatCurrency((int)tmpStat);
            dmgStringTextList[i].text = string.Format(damageFormat, LocalizationTable.Localization(skillData.optionliast[i].ToString()), skillData.DMGValue[i], finalValue, LocalizationTable.Localization("DamageType_" + skillData.attackdamagetype[i].ToString()));
        }
    }

    private void OnUpgradeSkillClicked()
    {
        if (GameDataTable.Instance.ConstranitsDataDic["MAX_SKILLUPGREADLEVEL"].Value <= curSkill._upgradeAmount)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_maimnumlevel"));
            return;
        }
        BaseSkillData skillData = BaseSkillData.FindLevelForData(GetSkillListPerSubClass(curSubClass), curSkill.SkillName, curSkill._upgradeAmount);

        if (!ResourceManager.Instance.CheckResource(skillData.REsourceType,skillData.ResourceCount))
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_NeedMoreResource"));
            return;
        }
        ResourceManager.Instance.Minus_ResourceType(skillData.REsourceType, skillData.ResourceCount);
        SoundManager.Instance.PlayAudio("UpgradeClick");

        GameDataTable.Instance.User.SetSkillNameForUpgrade(GetUpgradeDicPerSubClass(curSubClass), curSkill.SkillName, 1);
        curSkill._upgradeAmount = GetUpgradeDicPerSubClass(curSubClass)[curSkill.SkillName];
        SetSkillInfo(curSkill, curSubClass, curPresetCount);

        GameEventSystem.Send_GameUpdateSkillUIElement_GameEventHandler_Event(); //UI 업데이트
    }
}
