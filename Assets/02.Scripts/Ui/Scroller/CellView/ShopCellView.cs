using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;


/// </summary>
/// 작성일자   : 2024-08-28
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 상점 팝업 cell 
/// </summary>
public class ShopCellView :  EnhancedScrollerCellView
{
    public Image GoldShopImg;
    public TextMeshProUGUI GoldValueText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI DiscountText;


    public Button buybtn;
    ShopGoldCellData shopdata;
    private void Awake()
    {
        buybtn.onClick.AddListener( delegate { BuyButton(); });
    }

    ShopPopUpController shoppopupcontroller;

    string iosnumber;
    string Aosnumber;

    StringBuilder strbr = new StringBuilder();


    private void BuyButton() //구매버튼 선택했을때
    {
        SoundManager.Instance.PlayAudio("UIClick");
        Debug.Log(" 누른 데이터 는" + shopdata);
        if (shopdata.Type == Utill_Enum.ShopType.Resource)
        {
            UIManager.Instance.loadingPopUp.Show(true);
            PopUpSystem.Instance.ShopPopup.Purchase(shopdata);
        }
    }

    public void SetData(ShopGoldCellData data)
    {
        shopdata = data;
        Debug.Log("데이터 할당" + shopdata);

        strbr.Clear();
        //데이터 값에따라 uiview 대비
        Sprite itemSprite = Utill_Standard.GetItemSprite(data.SpriteName);
        GoldShopImg.sprite = itemSprite;

        string goldvalue = Utill_Math.FormatCurrency(data.Count);
        data.totalamount = data.Count;
        strbr.Append(goldvalue);
        //만약 percent가 있다면? 추가 금화
        if (data.AddPercent > 0)
        {
            int addpercentvalue = data.Count * data.AddPercent / 100; // 수정: Percent 값을 이용한 추가 금화 계산
            TextColorSet textColorSet = GoldValueText.GetComponent<TextColorSet>();

            // 줄바꿈 및 추가 금화 표시
            strbr.AppendLine();
            strbr.Append(" + ");
            strbr.Append(Utill_Math.FormatCurrency(addpercentvalue));
            // 빨간색 글씨로 추가 금화의 퍼센트 표시
            strbr.AppendLine();
            strbr.Append(string.Format(("<color=#{0}>"),ColorUtility.ToHtmlStringRGB(textColorSet.GetRed1)));  // 빨간색 시작 태그
            strbr.Append(Utill_Math.FormatCurrency(data.AddPercent));
            strbr.Append("%</color>");    // 빨간색 종료 태그

            data.totalamount += addpercentvalue;
        }
        GoldValueText.text = strbr.ToString();
        strbr.Clear();

        //버튼ui
        if (data.Discount > 0)//할인이 붙었을 때
        {
            strbr.AppendLine();
            strbr.Append(data.Discount.ToString());
            strbr.Append("%"); 
            TextColorSet textColorSet = DiscountText.GetComponent<TextColorSet>();
            PriceText.text = string.Format(LocalizationTable.Localization($"Button_Shop_{data.OriginalPrice}_{data.Discount}_{data.DiscountedPrice}"), ColorUtility.ToHtmlStringRGB(textColorSet.GetRed1));
        }
        else //할인이 안붙었을때
        {
            DiscountText.text = ""; //할인이 없음으로
            PriceText.text = LocalizationTable.Localization($"Button_Shop_{data.OriginalPrice}_{data.Discount}_{data.DiscountedPrice}");
        }
        
    }
}
