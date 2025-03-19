using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGoldCellData //상점
{
    public int index;
    public string IOS;
    public string Aos;
    public string SpriteName;
    public Utill_Enum.ShopType Type;
    public string Goods;
    public int Count;
    public int AddPercent;

    public int OriginalPrice;
    public int Discount;
    public int DiscountedPrice;
    
    public int TBC;// 마일리지
    public string ShopDesc;// 상품설명

    public int totalamount;


    public static ShopGoldCellData FindShopData(Dictionary<int, ShopGoldCellData> ShopCurrencyGolddataDic , string goodsname)
    {
        ShopGoldCellData tempdata = new ShopGoldCellData();
        // 딕셔너리의 모든 값들을 순회하며 찾습니다.
        foreach (var kvp in ShopCurrencyGolddataDic)
        {
            ShopGoldCellData data = kvp.Value;

            // Aos 값이 주어진 goodsname과 일치하면 해당 데이터를 반환합니다.
            if (data.Aos == goodsname)
            {
                return data;
            }
        }

        // 일치하는 데이터가 없을 경우 null을 반환합니다.
        return null;
    }

}
