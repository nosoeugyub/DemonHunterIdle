using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 작성일자   : 2024-07-10
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 헌터 구매 관리                                 
/// </summary>
public class HunterPurchaseSystem : MonoSingleton<HunterPurchaseSystem>
{
    public HunterPurchaseView hunterPurchaseView;

    /// <summary>
    /// 구매 할수 있는지 체크 (지금은 무료로 살 수 있기에 구매여부만 체크중)
    /// </summary>
    public bool IsHunterPurchase()
    {
        return GameDataTable.Instance.User.HunterPurchase[GameDataTable.Instance.User.currentHunter];
    }

    /// <summary>
    /// 구매 버튼 눌렀을때 실행되는 함수
    /// </summary>
    public void OnHunterBuy()
    {
        if (!HunterPurchaseSystem.Instance.IsHunterPurchase())
        {
            GameDataTable.Instance.User.SetHunterPurchase();
        }
        HunterPortraitSystem.Instance.hunterPortraitView.SetHunterPortraitsCell();
        hunterPurchaseView.SetUIHunterBuy();
        //구매한 헌터의 장착 슬롯 업데이트
        if (GameDataTable.Instance.User.currentHunter == 0)
        {
            EquipmentItemManager.Instance.BindingHunterSlotData(GameDataTable.Instance.HunterItem.Archer);
        }
        if (GameDataTable.Instance.User.currentHunter == 1)
        {
            HunterItem.Equipment_UserDataEquipment(GameDataTable.Instance.HunterItem.Guardian);
            EquipmentItemManager.Instance.BindingHunterSlotData(GameDataTable.Instance.HunterItem.Guardian);
        }
        if (GameDataTable.Instance.User.currentHunter == 2)
        {
            EquipmentItemManager.Instance.BindingHunterSlotData(GameDataTable.Instance.HunterItem.Mage);
        }
    }
}
