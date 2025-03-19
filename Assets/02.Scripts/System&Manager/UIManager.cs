using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자 : 2024-05-28
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : UI 팝업 관리
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    public SettingPopUp settingPopUp;
    public LaguagePopUp laguagePopUp;
    public OptimizationPopUp optimizationPopUp;
    public CommunityPopUp communityPopUp;
    public CreditPopUp creditPopUp;
    public ExitPopUp exitPopUp;
    public InventoryPopType inventoryPopType;
    public ClickItemPopType clickItemPopType;
    public MailPopUp mailPopUp; 
    public LoadingPopUp loadingPopUp;
    public InternetErrorPopUp internetErrorPopUp;
    public WithdrawalPopUp withdrawalPopUp;
    public PromotionPopUp promotionPopUp;
    public CharacterInfoPopup characterInfoPopup;
    public SkillInfoPopUp skillInfoPopUp;
    public PurchaseAchievementPopUp purchaseAchievementPopUp;
    public GachaPopUp gachaPopUp;
    public GachaDictionaryPopUp gachaDictionaryPopUp;
    public GachaAchievementPopUp gachaAchievementPopUp;
    public ItemDrawerView ItemDrawerview;
    public NewItemPopUp newitempopup;
    public SleepModePopUp sleepModePopUp;
    public GachaItemInfoPopUp gachaItemInfoPopUp;

    [Header("경험치바 색상 만렙시 /평상시")]
    public  Color MaxExpBarColor;
    public Color  DefultExpBarColor;

    [Header("슬라이더 배경색1")]
    public Color SliderBGColor01 = new Color32(102, 102, 102, 255);
    [Header("슬라이더 게이지색1")]
    public Color SliderBarColor01 = new Color32(217, 217, 217, 255);

    [Header("메인하단버튼표시 관리 -StageLevel 도달시 활성화")]
    public int HunterInfo;
    public int HunterUpgrade;
    public int HunterAttribute;
    public int HunterSkill;
    public int Shop;


    private void Awake()
    {
        GameEventSystem.StageLevel_UiDraw_Event += DrawUi;
    }

    private void DrawUi(int stageLevel)
    {
        // stageLevel이 각 조건을 충족하면 해당 UI 활성화
        if (stageLevel >= HunterInfo)
        {
            PopUpSystem.Instance.CharacterInfoPopupBtn.gameObject.SetActive(true);
        }
        else
        {
            PopUpSystem.Instance.CharacterInfoPopupBtn.gameObject.SetActive(false);
        }

        if (stageLevel >= HunterUpgrade)
        {
            PopUpSystem.Instance.CharacterUpgradePopupBtn.gameObject.SetActive(true);
        }
        else
        {
            PopUpSystem.Instance.CharacterUpgradePopupBtn.gameObject.SetActive(false);
        }

        if (stageLevel >= HunterAttribute)
        {
            PopUpSystem.Instance.HunterAttributebtn.gameObject.SetActive(true);
        }
        else
        {
            PopUpSystem.Instance.HunterAttributebtn.gameObject.SetActive(false);
        }

        if (stageLevel >= HunterSkill)
        {
            PopUpSystem.Instance.SkillPopupBtn.gameObject.SetActive(true);
        }
        else
        {
            PopUpSystem.Instance.SkillPopupBtn.gameObject.SetActive(false);
        }

        if (stageLevel >= Shop)
        {
            PopUpSystem.Instance.ShopPopupBtn.gameObject.SetActive(true);
        }
        else
        {
            PopUpSystem.Instance.ShopPopupBtn.gameObject.SetActive(false);
        }
    }
}
