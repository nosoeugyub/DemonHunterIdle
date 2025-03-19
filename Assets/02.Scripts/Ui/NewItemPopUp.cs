using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-09-02
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 새로운 아이템나오는 팝업창
/// </summary>
public class NewItemPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItmeNameText;
    [SerializeField] TextMeshProUGUI ItemLevelCount;
    [SerializeField] TextMeshProUGUI ItmeNameLevel;
    [SerializeField] TextMeshProUGUI FiexdName;
    [SerializeField] TextMeshProUGUI ItemSelectBtnText;
    [SerializeField] TextMeshProUGUI ItemSellBtnText;
    [SerializeField] List<TextMeshProUGUI> ItemOptionList;
    [SerializeField] List<TextMeshProUGUI> ItemOptionValueList;

    [SerializeField] TextMeshProUGUI SellDesc;
    [SerializeField] TextMeshProUGUI SellAmountText;

    [SerializeField] Image itemImageBG;
    [SerializeField] Image itemImage;
    [SerializeField] Image SellImage;

    [SerializeField] TextColorSet ItemLevelColor;
    [SerializeField] TextColorSet ItemLevelDiffentColor;

    List<string> fixedoption1name;
     List<string> fixedvalue;

    public Button ItemSellBtn;
    public Button ItemSelectBtn;


    

    public void OnEnable()
    {
        ItemSellBtn.onClick.AddListener(delegate { OnClickItemSellBtn(); });
        ItemSelectBtn.onClick.AddListener(delegate { OnClickItemSeleBtn(); });
    }

    private void OnClickItemSellBtn()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        GameEventSystem.Send_Click_ItemSellDrawer_Event();
    }

    private void OnClickItemSeleBtn()
    {
        SoundManager.Instance.PlayAudio("Equipment");
        GameEventSystem.Send_Click_ItemSelectDrawer_Event();
    }

    public void ShowBtn(int leveldiffent)
    {
        if (leveldiffent <= -1) //레벨차가 안나는경우
        {
            ItemLevelCount.gameObject.SetActive(false);
        }
        else
        {
            ItemLevelDiffentColor.TextColor(Utill_Enum.Text_Color.Red);
            ItemLevelCount.gameObject.SetActive(true);
            ItemLevelCount.text = "↑" + leveldiffent.ToString();
           
        }

            ItemSellBtn.gameObject.SetActive(true);
            ItemSelectBtn.gameObject.SetActive(true);

        //판매가 이미지가리게
        SellImage.gameObject.SetActive(false);
    }

    public void HideBtn()
    {
        ItemLevelCount.gameObject.SetActive(false);
        ItemSellBtn.gameObject.SetActive(false);
        ItemSelectBtn.gameObject.SetActive(false);
        //판매가 이미지가리게
        SellImage.gameObject.SetActive(true);
        SellDesc.text = SellDesc.text.Replace("결정", "판매");
    }

    public void Show()
    {
        gameObject.SetActive(true);
        PopUpSystem.Instance.CanUndoByESC = false;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        PopUpSystem.Instance.CanUndoByESC = true;
    }

    public void resultMessage(string message , Utill_Enum.Resource_Type resourcetype, int resuceramount)
    {
        SellDesc.text = message;
        SellImage.sprite = Utill_Standard.GetItemSprite(resourcetype.ToString());
        SellAmountText.text = resuceramount.ToString();
    }

   public void SellPopupSet(int sellamonut , Utill_Enum.Resource_Type type)
    {
        SellImage.sprite = Utill_Standard.GetItemSprite(type.ToString());
        SellAmountText.text = sellamonut.ToString();

    }

    public void SettingNewItem(HunteritemData _data)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string gradespritename = "";
        ItemLevelColor.TextColor(Utill_Enum.Text_Color.Green);
        Color itemdrawercolor = Color.white;
        switch (_data.DrawerGrade)
        {
            case DrawerGrade.None:
                break;
            case DrawerGrade.Normal:
                gradespritename = Tag.ItemDrawer_Normal;
                itemdrawercolor = TextColorManager.Instance.Normal;
                break;
            case DrawerGrade.Superior:
                gradespritename = Tag.ItemDrawer_Superior;
                itemdrawercolor = TextColorManager.Instance.Superior;
                break;
            case DrawerGrade.Rare:
                gradespritename = Tag.ItemDrawer_Rare;
                itemdrawercolor = TextColorManager.Instance.Rare;
                break;
            case DrawerGrade.Unique:
                gradespritename = Tag.ItemDrawer_Unique;
                itemdrawercolor = TextColorManager.Instance.Unique;
                break;
            case DrawerGrade.Epic:
                gradespritename = Tag.ItemDrawer_Epic;
                itemdrawercolor = TextColorManager.Instance.Epic;
                break;
            case DrawerGrade.Hero:
                gradespritename = Tag.ItemDrawer_Hero;
                itemdrawercolor = TextColorManager.Instance.Hero;
                break;
            case DrawerGrade.Ancient:
                gradespritename = Tag.ItemDrawer_Ancient;
                itemdrawercolor = TextColorManager.Instance.Ancient;
                break;
            case DrawerGrade.Abyssal:
                gradespritename = Tag.ItemDrawer_Abyssal;
                itemdrawercolor = TextColorManager.Instance.Abyssal;
                break;
        }

        stringBuilder.Clear();
        string itemname = _data.Part.ToString();
        string grade = _data.DrawerGrade.ToString();
        string hexColor = ColorUtility.ToHtmlStringRGBA(itemdrawercolor); // 색상을 HTML 형식의 문자열로 변환
        stringBuilder.Append($"<color=#{hexColor}>");
        stringBuilder.Append(LocalizationTable.Localization(_data.Class.ToString()));
        stringBuilder.Append(LocalizationTable.Localization("Grade_" + grade));
        stringBuilder.Append(LocalizationTable.Localization("Equipment_" + itemname));
        stringBuilder.Append("</color>"); // 색상 끝
        ItmeNameText.text = stringBuilder.ToString();

        //고정 텍스트
        FiexdName.text = LocalizationTable.Localization("Fixed");

        //선택 텍스트 / 판매 텍스트
        ItemSelectBtnText.text = LocalizationTable.Localization("Sell");
        ItemSellBtnText.text = LocalizationTable.Localization("Select");
        //BG 및 아이템 이미지
        //아이템 이름 / 등급 이미지 할당
        Sprite backGroundSprite = Utill_Standard.GetItemSprite(_data.ItemGrade.ToString());
        Sprite itemSprite = Utill_Standard.GetItemSprite(_data.Name);


        itemImageBG.sprite = backGroundSprite;
        itemImage.sprite = itemSprite;


        //옵션및 값
        int optioncount = _data.FixedOptionTypes.Count;

        fixedoption1name = new List<string>(optioncount);
        fixedvalue = new List<string>(optioncount);

        for (int i = 0; i < optioncount; i++)
        {
            ItemOptionList[i].text = "";
            ItemOptionValueList[i].text = "";
        }




        if (optioncount > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                ItemOptionList[i].gameObject.SetActive(false);
                ItemOptionValueList[i].gameObject.SetActive(false);
            }

            //옵션
            for (int i = 0; i < optioncount; i++)
            {
                ItemOptionList[i].gameObject.SetActive(true);
                fixedoption1name.Add(_data.FixedOptionTypes[i]);
                ItemOptionList[i].text = LocalizationTable.Localization(fixedoption1name[i]);
            }


            //레벨
         
            for (int i = 0; i < optioncount; i++)
            {
                ItemOptionValueList[i].gameObject.SetActive(true);
                fixedvalue.Add(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, fixedoption1name[i], (float)_data.FixedOptionValues[i]));
                ItemOptionValueList[i].text = LocalizationTable.Localization(fixedvalue[i]);

               
            }

            //레벨평균
            StringBuilder stb = new StringBuilder();
            stb.Clear();
            string itemLevelstr = Utill_Math.FormatCurrency(_data.TotalLevel);
            stb.Append("+");
            stb.Append(itemLevelstr);
            ItmeNameLevel.text = stb.ToString();
        }
    }


 

}
