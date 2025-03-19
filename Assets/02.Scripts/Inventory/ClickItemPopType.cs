using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Utill_Enum;
using static Item;
/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 자세히 보기 아이템 팝업 UI 처리 및 아이템 탈부착 처리                                                
/// </summary>
public class ClickItemPopType : MonoBehaviour, IPopUp
{
    public int ClickCount = 0;

    public ItemCell itemCell;

    [SerializeField] 
    private TextMeshProUGUI itemNameText; 
    [SerializeField] 
    private TextMeshProUGUI optionText;
    [SerializeField]
    private Button detachEquipmentButton; //탈부착 버튼

    private EquipmentItemManager equipmentItemManagerInst;
    
    Item item;

    /// <summary>
    /// 아이템 팝업 UI 처리 
    /// </summary>
    /// <param name="item"> 인벤토리에서 자세히 보기 누른 아이템 </param>
    public void Init(Item item)
    {
        this.item = item;
        equipmentItemManagerInst = EquipmentItemManager.Instance;
        int optionCnt = 0;
        optionCnt = item.GetOptions.Count;

        # region 아이템 셀에 아이템 할당 및 UI 처리
        itemCell.SetItemInfo(InventoryPopType.Instance.item);
        itemCell.ToggleEquipText();
     

        itemNameText.text = item.GetName;

        string optionStr = string.Empty;
        
        int index = 0;
        int randIndex = 0;

        optionStr += $"<color=#787878FF>\n고정옵   </color>";

        foreach (var opt in item.GetOptions)
        {
            if (index == 0)
            {
                optionStr += $"{opt.ToString()}  {item.FixedValues[index]}  \n";
            }
            else
            {
                optionStr += $"            {opt.ToString()}  {item.FixedValues[index]}  \n";
            }
            index++;
        }

        optionStr += $"<color=#787878FF>\n \n랜덤옵  </color>";

        foreach (var randOpt in item.SelectRandOptions)
        {
            if (randIndex == 0)
            {
                optionStr += $"{randOpt.ToString()}  {item.RandomValues[randIndex]}  \n";
            }
            else
            {
                optionStr += $"            {randOpt.ToString()}  {item.RandomValues[randIndex]}  \n";
            }
            randIndex++;
        }

        optionText.text = optionStr;
        #endregion

        detachEquipmentButton.onClick.RemoveAllListeners();
        detachEquipmentButton.onClick.AddListener(DetachEquipment);
    }

    /// <summary>
    /// 장착버튼 눌렀을때 실행
    /// </summary>
    private void DetachEquipment()
    {
        EquipmentType equipmentType = item.equipmentItem.type;

        if (itemCell.item.equipmentItem.isEquip) //장착되어 있다면
        {
            itemCell.item.equipmentItem.isEquip = false;  //장착 여부 false로 변경
           // equipmentItemManagerInst.RemoveEquipmentItem(equipmentType, item); // 스탯 마이너스 및 코스튬 처리
        }
        else
        {
            if (!IsMountable()) // 승급 레벨에 따라 장비 장착 불가능하다면 시스템 알림
            {
                SystemNoticeManager.Instance.SystemNotice("SystemNotice_LackOfPromotionLevel", SystemNoticeType.Default);
                return;
            }

            itemCell.item.equipmentItem.isEquip = true; //장착 여부 true로 변경
           // equipmentItemManagerInst.SetEquipmentItem(equipmentType, item); //스탯 설정 및 코스튬 처리
        }

        InventoryPopType.Instance.cell.SetItemInfo(itemCell.item);
        InventoryManager.Instance.SettingEquipInfo();
        InventoryPopType.Instance.cell.ToggleEquipText(); //토글 텍스트 설정
        InventoryPopType.Instance.Hide(); //탈부착시 인벤토리 팝업 끄기
    }

    /// <summary>
    /// 각 헌터 승급 레벨따라 장비 부위 별 장착 가능한지 판단
    /// </summary>
    public bool IsMountable()
    {
        int promotionValue = GameDataTable.Instance.User.HunterPromotion[GameDataTable.Instance.User.currentHunter];

        if (item.equipmentItem.type == EquipmentType.Pet)
        {
            ConstraintsData data = GameDataTable.Instance.ConstranitsDataDic[Tag.PET_UNLOCK];
            return ConstraintsData.CheckMaxValue(data, promotionValue);
        }
        else if (item.equipmentItem.type == EquipmentType.Wing)
        {
            ConstraintsData data = GameDataTable.Instance.ConstranitsDataDic[Tag.WING_UNLOCK];
            return ConstraintsData.CheckMaxValue(data, promotionValue);
        }
        return true;
    }

    public void Close()
    {
    }

    public void Hide()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        ClickCount = 0;

        PopUpSystem.Instance.EscPopupListRemove();

        Item item = new Item();
        InventoryPopType.Instance.item = item;
        this.item = item;

        InventoryPopType.Instance.createBtn.SetActive(true);
        InventoryPopType.Instance.deleteBtn.SetActive(true);

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
