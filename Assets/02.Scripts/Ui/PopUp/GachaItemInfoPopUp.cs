using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

public class GachaItemInfoPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] TextMeshProUGUI ItmeNameText;
    [SerializeField] TextMeshProUGUI FiexdName;
    [SerializeField] TextMeshProUGUI HoldName;
    [SerializeField] TextMeshProUGUI EquipButtonText;
    [SerializeField] TextMeshProUGUI CloseButtonText;

    [SerializeField] List<TextMeshProUGUI> ItemOptionList;
    [SerializeField] List<TextMeshProUGUI> ItemOptionValueList;

    //보유효과
    [SerializeField] List<TextMeshProUGUI> HoldItemOptionList;
    [SerializeField] List<TextMeshProUGUI> HoldItemOptionValueList;

    [SerializeField] Image itemImageBG;
    [SerializeField] Image itemImage;

    [SerializeField] TextColorSet ItemLevelColor;
    
    [SerializeField] Button equipButton;
    [SerializeField] Button closeButton;

    Basic_Prefab basicPrefab;
    List<string> fixedoption1name;
    List<string> fixedvalue;
    List<string> holdOption1name;
    List<string> holdvalue;
    private int ClickCount = 0;
    private bool isInit = false;
    HunteritemData _ItemData;

    private void Initialize()
    {
        if (isInit) return;
        isInit = true;
        basicPrefab = equipButton.GetComponent<Basic_Prefab>();
        equipButton.onClick.AddListener(OnEquipItem);
        closeButton.onClick.AddListener(Hide);
    }

    /// <summary>
    /// 아이템 장착
    /// </summary>
    public void OnEquipItem()
    {

        if (_ItemData.Name == "")
        {
            //장착할 아이템이업습니다.
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_GetEqiupmentItem"), SystemNoticeType.Default);
            return;
        }

        List<HunteritemData> tmpDataList = new();
        switch (GameDataTable.Instance.User.currentHunter)
        {
            case 0: //궁수
                tmpDataList = GameDataTable.Instance.HunterItem.Archer;
                break;
            case 1: //수호자
                tmpDataList = GameDataTable.Instance.HunterItem.Guardian;
                break;
            case 2: //법사
                tmpDataList = GameDataTable.Instance.HunterItem.Mage;
                break;
        }
        //현재 부위의 최신 HunteritemData
        var curHunterItem = HunteritemData.GetHunteritemData(tmpDataList, _ItemData.Part);
        //미보유 장비인지 확인
        if (curHunterItem.EquipCountList[(int)_ItemData.ItemGrade-1]<=0)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_CannotEquipNotheld"), SystemNoticeType.Default);
            return;
        }

        if (_ItemData.isEquip == false) // 장착을 안했더라면 현재 장비 장착
        {
            SoundManager.Instance.PlayAudio("Equipment");
            basicPrefab.SetTypeButton(ButtonType.DeActive);
            _ItemData.FixedOptionValues = InventoryManager.Instance.GachaChangeFixedOptionValues(_ItemData.FixedOptionTypes, _ItemData.Name);
            _ItemData.isEquip = true;
            HunteritemData.Save_Slot(tmpDataList,_ItemData);
            GameEventSystem.Send_Equipment_EuipSlot(HunteritemData.GetHunteritemData(tmpDataList,_ItemData.Part));//장착 델리게이트

        }
        else // 장착을 했더라면 현재 장비 해제
        {
            SoundManager.Instance.PlayAudio("UnEquipment");
            basicPrefab.SetTypeButton(ButtonType.Active);
            curHunterItem.isEquip = false;
            GameEventSystem.Send_UnEquipment_EuipSlot(curHunterItem); //해제 델리게이트
        }
        Hide();
    }

    public void Close()
    {
    }

    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove(); //팝업 종료시 esc리스트에서 제거

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);//팝업 보여질 시 esc리스트에 추가
            gameObject.SetActive(true);
            Initialize();
        }
        else
        {
            Hide();
        }

    }


    public void SettingNewItem(HunteritemData _data,Item item)
    {
        EquipButtonText.text = LocalizationTable.Localization("Button_Equip");
        CloseButtonText.text = LocalizationTable.Localization("Button_Close");

        HunteritemData hunteritemData = HunteritemData.DeepCopy(_data);
        HunterItem.SetHunterItemDataWithItem(hunteritemData, item);

        _ItemData = hunteritemData;

        StringBuilder stringBuilder = new StringBuilder();
        ItemLevelColor.TextColor(Utill_Enum.Text_Color.Green);
        Color itemdrawercolor = Color.white;
        string itemname = hunteritemData.Part.ToString();
        string grade = item.GetGrade.ToString();
       

        switch (item.GetGrade)
        {
            case Grade.Normal:
                itemdrawercolor = TextColorManager.Instance.Normal;
                break;
            case Grade.Superior:
                itemdrawercolor = TextColorManager.Instance.Superior;
                break;
            case Grade.Rare:
                itemdrawercolor = TextColorManager.Instance.Rare;
                break;
            case Grade.Unique:
                itemdrawercolor = TextColorManager.Instance.Unique;
                break;
            case Grade.Epic:
                itemdrawercolor = TextColorManager.Instance.Epic;
                break;
            case Grade.Hero:
                itemdrawercolor = TextColorManager.Instance.Hero;
                break;
            case Grade.Ancient:
                itemdrawercolor = TextColorManager.Instance.Ancient;
                break;
            case Grade.Abyssal:
                itemdrawercolor = TextColorManager.Instance.Abyssal;
                break;
        }

        if (_data.isEquip == true && _data.ItemGrade == item.GetGrade)
        {
            basicPrefab.SetTypeButton(ButtonType.DeActive);
        }
        else
        {
            basicPrefab.SetTypeButton(ButtonType.Active);
        }
        string hexColor = ColorUtility.ToHtmlStringRGBA(itemdrawercolor); // 색상을 HTML 형식의 문자열로 변환

        stringBuilder.Clear();
        stringBuilder.Append($"<color=#{hexColor}>");
        stringBuilder.Append(LocalizationTable.Localization(hunteritemData.Class.ToString()));
        stringBuilder.Append(LocalizationTable.Localization("Grade_" + grade));
        stringBuilder.Append(LocalizationTable.Localization("Equipment_" + itemname));
        stringBuilder.Append("</color>"); // 색상 끝
        ItmeNameText.text = stringBuilder.ToString();

        //고정 텍스트
        FiexdName.text = LocalizationTable.Localization("Fixed");
        HoldName.text = LocalizationTable.Localization("HoldOption");

        //BG 및 아이템 이미지
        //아이템 이름 / 등급 이미지 할당
        Sprite backGroundSprite = Utill_Standard.GetItemSprite(hunteritemData.ItemGrade.ToString());
        Sprite itemSprite = Utill_Standard.GetItemSprite(hunteritemData.Name);


        itemImageBG.sprite = backGroundSprite;

        if (_data.EquipCountList[(int)item.GetGrade-1] <= 0)
        {
            //아이템셀 이미지를 물음표 이미지로 변경
            itemImage.sprite = Utill_Standard.GetUiSprite("QuestionMark");
            itemImage.color = Color.black;
        }
        else
        {
            itemImage.color = Color.white;
            itemImage.sprite = itemSprite;
        }

        //옵션및 값
        int optioncount = hunteritemData.FixedOptionTypes.Count;

        fixedoption1name = new List<string>(optioncount);
        fixedvalue = new List<string>(optioncount);

        for (int i = 0; i < ItemOptionList.Count; i++)
        {
            ItemOptionList[i].text = "";
            ItemOptionValueList[i].text = "";
        }


        if (optioncount > 0)
        {
            //옵션
            for (int i = 0; i < optioncount; i++)
            {
                fixedoption1name.Add(hunteritemData.FixedOptionTypes[i]);
                ItemOptionList[i].text = LocalizationTable.Localization(fixedoption1name[i]);
            }


            //레벨

            for (int i = 0; i < optioncount; i++)
            {
                fixedvalue.Add(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, fixedoption1name[i], (float)item.FixedOptionMinValue[i]));
                ItemOptionValueList[i].text = LocalizationTable.Localization(fixedvalue[i]);


            }

            //레벨평균
            //StringBuilder stb = new StringBuilder();
            //stb.Clear();
            //string itemLevelstr = _data.TotalLevel.ToString();
            //stb.Append("+");
            //stb.Append(itemLevelstr);
            //ItmeNameLevel.text = stb.ToString();
        }

        int holdOptionCount = hunteritemData.HoldOption.Count;

        holdOption1name = new List<string>(optioncount);
        holdvalue = new List<string>(optioncount);

        for (int i = 0; i < HoldItemOptionList.Count; i++)
        {
            HoldItemOptionList[i].text = "";
            HoldItemOptionValueList[i].text = "";
        }

        if (holdOptionCount > 0)
        {

            //옵션
            for (int i = 0; i < holdOptionCount; i++)
            {
                holdOption1name.Add(item.HoldOption[i].ToString());
                HoldItemOptionList[i].text = LocalizationTable.Localization(holdOption1name[i]);
            }


            //레벨

            for (int i = 0; i < holdOptionCount; i++)
            {
                holdvalue.Add(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, holdOption1name[i], (float)item.HoldOptionValue[i]));
                HoldItemOptionValueList[i].text = LocalizationTable.Localization(holdvalue[i]);
            }
        }
    }
}
