using NSY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 툭정 부위의 장착한 아이템의 셀
/// </summary>
public class EquipmentItemCell : MonoBehaviour
{
    //nsy code start
    HunteritemData currenthunteritem;
    public HunteritemData CurrentHunterItem
    {
        get { return currenthunteritem; }
        set { currenthunteritem = value; }
    }
    public EquipmentType equipmentType;
    public Button ClickDrawerBtn;

    public Image ItemImage;
    public Image ItemBgImage;

    public TextMeshProUGUI ItemLevel;
    public TextMeshProUGUI itemName;

    private void OnEnable()
    {
        ClickDrawerBtn.onClick.RemoveAllListeners();
        ClickDrawerBtn.onClick.AddListener(delegate { ClickOnDrawer(equipmentType, CurrentHunterItem); });


       
    }


    //슬롯 초기화
    public void init_Slot()
    {
        ItemImage.gameObject.SetActive(false);
        ItemBgImage.color = Color.black;
        ItemBgImage.sprite = null;
        ItemImage.sprite = null;
        ItemLevel.gameObject.SetActive(false);
    }
    public void SettingImg(string ItemString, string BgSTring , int level)
    {
        itemName.text = LocalizationTable.Localization("Equipment_" + CurrentHunterItem.Part.ToString());
        //아이템 이미지
        Sprite _itemImg = Utill_Standard.GetItemSprite(ItemString);
        ItemImage.gameObject.SetActive(true);
        ItemImage.sprite = _itemImg;

        //아이템 배경 이미지
        Sprite _itemBG = Utill_Standard.GetItemSprite(BgSTring);
        ItemBgImage.color = Color.white;
        ItemBgImage.sprite = _itemBG;

        //레벨
        ItemLevel.gameObject.SetActive(true);
        ItemLevel.text ="+" + Utill_Math.FormatCurrency(level);
    }
    public void init_Slots()
    {
        itemName.text = LocalizationTable.Localization("Equipment_" + CurrentHunterItem.Part.ToString());
        ItemImage.gameObject.SetActive(false);
        ItemBgImage.color = Color.black;

        ItemBgImage.sprite = null;
        ItemImage.sprite = null;

        ItemLevel.text = "";
    }

    public void EquipmentSetting() //장비 장착시키는 함수
    {
        if (CurrentHunterItem.isEquip)
        {
            CostumeStlyer.ChangePartOfCostumeStyle(CurrentHunterItem.Class, CurrentHunterItem);
        }

    }
    /// <summary>
    /// 헌터의 장착 슬롯 클릭 이벤트
    /// </summary>
    /// <param name="equipmentType"></param>
    /// <param name="CurrentHunterItem"></param>
    public void ClickOnDrawer(EquipmentType equipmentType , HunteritemData CurrentHunterItem)
    {
        if (GameDataTable.Instance.User.HunterPurchase[(int)CurrentHunterItem.Class] == false)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_BuyHunterFirst"), SystemNoticeType.Default);
            return;
        }
        SoundManager.Instance.PlayAudio("UIClick");
        ItemDrawer.Instance.OpenITemDrawer(CurrentHunterItem.ItemContainsType , CurrentHunterItem);
    }

    //nsy code end
}
