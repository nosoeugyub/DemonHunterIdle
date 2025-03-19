using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-04
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 승급 팝업 UI 처리                                                  
/// </summary>

public class PromotionPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] private GameObject LeftObj;
    [SerializeField] private GameObject RightObj;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private TextMeshProUGUI promotionText;
    [SerializeField] private TextMeshProUGUI promotionCurrenyText;

    [SerializeField] private Image currneyImg;

    [SerializeField] private Basic_Prefab basicbtn;
    [SerializeField] private Button promotionBtn;

    public int ClickCount = 0;

    private StringBuilder sb = new();

    void Start()
    {
        titleText.text = LocalizationTable.Localization("Title_Promotion");

        promotionBtn.onClick.AddListener(delegate { OnClickPromotion(); });
    }

    private void OnClickPromotion()
    {
        GameEventSystem.Send_Promotion();
    }

    /// <summary>
    /// 최대 승급일때 ui 처리
    /// </summary>

    public void MaxPromitoinIUi()
    {
        //조건
        sb.Clear();
        sb.AppendLine(LocalizationTable.Localization("Condition"));
        sb.AppendLine("-");
        sb.AppendLine("-");
        sb.AppendLine();
        sb.AppendLine(LocalizationTable.Localization("Reward"));
        sb.AppendLine("-");
        sb.AppendLine("-");
        sb.AppendLine("-");
        sb.AppendLine("-");
        contentText.text = sb.ToString();
        //버튼
        basicbtn.SetTypeButton(ButtonType.DeActive);

    }


    /// <summary>
    /// 승급 팝업 UI 처리
    /// </summary>
    public void RefreshUI()
    {
        int MaxPromotion = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_PROMOTION_LEVEL].Value;
        int currentPromotionLevel = GameDataTable.Instance.User.HunterPromotion[GameDataTable.Instance.User.currentHunter];
        if (MaxPromotion <= currentPromotionLevel) //최대치..
        {
            UIManager.Instance.promotionPopUp.MaxPromitoinIUi();
            return;
        }



        int currentHunter = GameDataTable.Instance.User.currentHunter;

        int key = GameDataTable.Instance.User.HunterPromotion[currentHunter] + 1;

        if (GameDataTable.Instance.PromotionAllocationData.ContainsKey(key))
        {
            promotionBtn.gameObject.SetActive(true);

            var promotionData = GameDataTable.Instance.PromotionAllocationData[key];
            var promotionAbilityData = GameDataTable.Instance.PromotionAbilityData[key];

            string conditionText = "";

            sb.Clear();
            sb.AppendLine(LocalizationTable.Localization("Condition"));
            sb.Append(LocalizationTable.Localization(((SubClass)GameDataTable.Instance.User.currentHunter.GetDecrypted()).ToString()));
            sb.Append(" ");
            sb.Append(string.Format(LocalizationTable.Localization("PromotionLevelCondition"), Utill_Math.FormatCurrency(promotionData.ReqHunterLevel)) );
            sb.AppendLine();
            sb.AppendLine(string.Format(LocalizationTable.Localization("PromotionStageCondition"), Utill_Math.FormatCurrency(promotionData.ReqClearChapterZone)));
            sb.AppendLine();
            sb.AppendLine(LocalizationTable.Localization("Reward"));

            // Add Ability Modifiers
            AppendPromotionAbility("AttackSpeed",  promotionAbilityData.AttackSpeed);
            AppendPromotionAbility("AttackRange", promotionAbilityData.AttackRange);
            AppendPromotionAbility("PhysicalPower", promotionAbilityData.PhysicalPower);
            AppendPromotionAbility("MagicPower", promotionAbilityData.MagicPower);
            AppendPromotionAbility("PhysicalPowerDefense", promotionAbilityData.PhysicalPowerDefense);
            AppendPromotionAbility("HP", promotionAbilityData.Hp);
            AppendPromotionAbility("MP", promotionAbilityData.Mp);
            
            conditionText = sb.ToString();

            contentText.SetText(conditionText);
            int promotion = GameDataTable.Instance.User.HunterPromotion[currentHunter];
            promotionText.SetText(++promotion + LocalizationTable.Localization("Promotion"));

            promotionCurrenyText.SetText(promotionData.ResourceCount.ToString());

            UIManager.Instance.characterInfoPopup.SetPromotionText();

            string currencyString = promotionData.ResourceType;
            if (Enum.TryParse(currencyString, out Resource_Type currencyType))
            {
                currneyImg.sprite = Utill_Standard.GetItemSprite(currencyString);
            }

            basicbtn.SetTypeButton(ButtonType.Active);
        }
        else
        {
            contentText.SetText("최대 승급입니다");
            promotionBtn.gameObject.SetActive(false);
        }
    }

    // Helper function to append ability data to StringBuilder if the value is greater than zero
    private void AppendPromotionAbility(string abilityName, float value)
    {
        if (value > 0)
        {
            sb.AppendLine($"-{LocalizationTable.Localization(abilityName)} { Utill_Math.FormatCurrency((int)value)}");
        }
    }
    public void Close()
    {
        gameObject.SetActive(false);

        HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);
    }

    public void Hide()
    {
        ClickCount = 0;

        HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);

        LeftObj.SetActive(true);
        RightObj.SetActive(true);

        PopUpSystem.Instance.EscPopupListRemove();

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            RefreshUI();
            
            HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(false);

            LeftObj.SetActive(false);
            RightObj.SetActive(false);
            
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
