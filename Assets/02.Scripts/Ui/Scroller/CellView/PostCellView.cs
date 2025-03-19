using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;
using BackEnd;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 우편 셀 UI 관리                
/// </summary>
public class PostCellView : EnhancedScrollerCellView
{
    [SerializeField] private PostCellData postCellData; //우편 셀 데이터

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemCntText;
    public TextMeshProUGUI remainTimeText;
    public TextMeshProUGUI senderText;

    public Image itemImage;
    public Image itemBackgroundImage;

    public Button reviceBtn;

    PostType postType;

    /// <summary>
    /// PostCellData 받아와서 데이터 및 UI 세팅
    /// </summary>
    /// <param name="data"></param>
    public void SetData(PostCellData data)
    {
        postCellData = data; //데이터 세팅

        LanguageSetting();

        titleText.text = postCellData.title; //제목
        senderText.text = postCellData.senderName; //보낸이

        itemNameText.text = postCellData.itemName; // 아이템 이름
        if (postCellData.remainTime == "-1") //로컬우편처리
        {
            remainTimeText.text = "\u221E"; //남은 시간 무한대표기
        }
        else
        {
            remainTimeText.text = postCellData.remainTime; //남은 시간
        }
        

        itemImage.sprite = postCellData.itemSprite; //아이템 이미지 
        itemBackgroundImage.sprite = postCellData.itemBackgroundSprite; // 아이템 배경 이미지

        postType = postCellData.postType; 

        if (postCellData.buttonDisable)
        {
            reviceBtn.interactable = false;
        }
        else
        {
            reviceBtn.interactable = true;
            reviceBtn.GetComponent<Basic_Prefab>().SetTypeButton(ButtonType.Active);
        }

        reviceBtn.onClick.RemoveAllListeners(); 

        reviceBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.mailPopUp.ReceiveMail(postType, data.index); //우편 받기
        });
    }

    /// <summary>
    /// 언어 세팅
    /// </summary>
    public void LanguageSetting()
    {
        itemCntText.SetText($"{Utill_Math.FormatCurrency(postCellData.itemCount)}{LocalizationTable.Localization("Count")}");
        reviceBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Recive");
        LocalizationTable.languageSettings += LanguageSetting;
    }
}