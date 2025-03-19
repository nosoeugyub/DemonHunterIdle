using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 데이터 관련 컨트롤러
/// /// </summary>.
public class ItemDrawerPopupView : MonoBehaviour, IPopUp
{
    [SerializeField] TextMeshProUGUI Titletext;
    [SerializeField] TextMeshProUGUI Upgradetext;
    [SerializeField] TextMeshProUGUI DrawerNanmetext;
    [SerializeField] TextMeshProUGUI Evolutiontext;
    [SerializeField] TextMeshProUGUI NeedEvAmount;
    [SerializeField] TextMeshProUGUI NeedEVtext;
    [SerializeField] TextMeshProUGUI[] NeedEVtextAarray;
    [SerializeField] TextColorSet[] NeedEVtextAarraycolor;
    [SerializeField] TextColorSet NeedEVtextcolor;
    [SerializeField] TextMeshProUGUI SucceedText;

    [SerializeField] Image Eviamge;
    [SerializeField] Image DrawerImage;

    [SerializeField] Basic_Prefab ClickUpgradeDrawerprefab;
    [SerializeField] Button CloseBtn;
    [SerializeField] Button ClickUpgradeDrawer;

    string Drawerpartname;
    string gradespritename;

    StringBuilder str = new StringBuilder();
    HunteritemData _currenthunteritemdata;

    private void Awake()
    {
        ClickUpgradeDrawer.onClick.AddListener(delegate { OnClickDrawerBtn(); });
        CloseBtn.onClick.AddListener(delegate { CloseItemDrawerPopUp(); });
    }

    private void CloseItemDrawerPopUp()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        gameObject.SetActive(false);
    }

    public void MaximumUiSet()
    {
        //진화 버튼 비활성화
        ClickUpgradeDrawerprefab.SetTypeButton(ButtonType.DeActive);

        NeedEVtextcolor.TextColor(Text_Color.Gray);
        //진화조건 비활성화
        for (int i = 0; i < NeedEVtextAarray.Length; i++)
        {
            NeedEVtextAarraycolor[i].TextColor(Text_Color.Gray);
            NeedEVtextAarray[i].text = "-";
        }
    }
    public void NotMaxmuim()
    {
        //진화 버튼 비활성화
        ClickUpgradeDrawerprefab.SetTypeButton(ButtonType.Active);

        NeedEVtextcolor.TextColor(Text_Color.Default);
        //진화조건 비활성화
        for (int i = 0; i < NeedEVtextAarray.Length; i++)
        {
            NeedEVtextAarraycolor[i].TextColor(Text_Color.Gray);
        }
    }


    private void OnClickDrawerBtn()
    {
        GameEventSystem.Send_Click_UpgradeBtnDrawer_Event(_currenthunteritemdata);
    }

    public void SetDarer(bool _condtion1, bool _condtion2, bool _condtion3 , HunteritemData _hunteritemdata)
    {
        //재화 부터 검사 (실시간 재화) 재화가 부족할시에는 리턴해야함
        ItemDrawerTableData Drawerdata = GameDataTable.Instance.ItemDrawerGradeDic[_hunteritemdata.DrawerGrade];
        Upgradetext.text = LocalizationTable.Localization("Button_Upgrade");
        str.Clear();
        str.Append(LocalizationTable.Localization("SuccessProbability"));
        str.Append(Utill_Math.FormatCurrency(Drawerdata.DrawerProb) + "%");
        SucceedText.text = str.ToString();
        str.Clear();

        _currenthunteritemdata = _hunteritemdata;
        Color itemdrawercolor = Color.white;

        switch (_hunteritemdata.DrawerGrade)
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
        string hexColor = ColorUtility.ToHtmlStringRGBA(itemdrawercolor); // 색상을 HTML 형식의 문자열로 변환
        Drawerpartname = _hunteritemdata.Part.ToString();
        //모루이름
        str.Clear();
        str.Append($"<color=#{hexColor}>");
        str.Append(LocalizationTable.Localization(_hunteritemdata.Class.ToString()));
        str.Append(LocalizationTable.Localization("Grade_" + _hunteritemdata.DrawerGrade.ToString()));
        str.Append(LocalizationTable.Localization("Equipment_" + Drawerpartname));
        str.Append(LocalizationTable.Localization("Drawer"));
        str.Append($"<color=#{hexColor}>");
        DrawerNanmetext.text = str.ToString();

        //모루 이미지
        Sprite _itemdarwer = Utill_Standard.GetUiSprite(gradespritename);
        DrawerImage.sprite = _itemdarwer;

        Titletext.text = LocalizationTable.Localization("Title_Equipment");
        NeedEVtext.text = LocalizationTable.Localization("EvolutionCondition");

        Sprite drawersprite = Utill_Standard.GetItemSprite(Drawerdata.DrawerResourceType.ToString());
        Eviamge.sprite = drawersprite;
        NeedEvAmount.text = Utill_Math.FormatCurrency(Drawerdata.DrawerResourceCount);



        var maxiamgradelevel = GameDataTable.Instance.ConstranitsDataDic[Tag.MAXIAM_ITEMDRAWER_GRADE];
        int currentmax = (int)(_hunteritemdata.DrawerGrade);
        if (currentmax >= maxiamgradelevel.Value)//최대 등급일때
        {
            MaximumUiSet();
            return;
        }
        else
        {
            NotMaxmuim();
        }

        //조건 달성

        string condtion1text1 = string.Format(LocalizationTable.Localization("WeaponEquipmentLevelHigher"), Utill_Math.FormatCurrency(Drawerdata.EvolutionReqCurrentEquipmentLvel) );
        string condtion1text2 = string.Format(LocalizationTable.Localization("OtherEquipmentLevels"), Utill_Math.FormatCurrency(Drawerdata.EvolutionReqTotalEquipmentLevl));
        string condtion1text3 =  string.Format(LocalizationTable.Localization("OtherEquipmentGrade"), LocalizationTable.Localization("Grade_" + Drawerdata.EvolutionReqTotalEquipmentGrade));

        string attainment = LocalizationTable.Localization("Attainment");
        string Unattainment = LocalizationTable.Localization("Unattainment");
        if (_condtion1)
        {
            str.Clear();
            str.Append(LocalizationTable.Localization(condtion1text1));
            str.Append("        -");
            str.Append(LocalizationTable.Localization(attainment));
            str.Append("-");
            NeedEVtextAarray[0].text = str.ToString();
            NeedEVtextAarraycolor[0].TextColor(Utill_Enum.Text_Color.White);
        }
        else
        {
            str.Clear();
            str.Append(LocalizationTable.Localization(condtion1text1));
            str.Append("        -");
            str.Append(LocalizationTable.Localization(Unattainment));
            str.Append("-");
            NeedEVtextAarray[0].text = str.ToString();
            NeedEVtextAarraycolor[0].TextColor(Utill_Enum.Text_Color.Gray);
        }


        if (_condtion2)
        {
            str.Clear();
            str.Append(LocalizationTable.Localization(condtion1text2));
            str.Append("        -");
            str.Append(LocalizationTable.Localization(attainment));
            str.Append("-");
            NeedEVtextAarray[1].text = str.ToString();
            NeedEVtextAarraycolor[1].TextColor(Utill_Enum.Text_Color.White);
        }
        else
        {
            str.Clear();
            str.Append(LocalizationTable.Localization(condtion1text2));
            str.Append("        -");
            str.Append(LocalizationTable.Localization(Unattainment));
            str.Append("-");
            NeedEVtextAarray[1].text = str.ToString();
            NeedEVtextAarraycolor[1].TextColor(Utill_Enum.Text_Color.Gray);
        }

        if (_condtion3)
        {
            str.Clear();
            str.Append(LocalizationTable.Localization(condtion1text3));
            str.Append("        -");
            str.Append(LocalizationTable.Localization(attainment));
            str.Append("-");
            NeedEVtextAarray[2].text = str.ToString();
            NeedEVtextAarraycolor[2].TextColor(Utill_Enum.Text_Color.White);
        }
        else
        {
            str.Clear();
            str.Append(LocalizationTable.Localization(condtion1text3));
            str.Append("        -");
            str.Append(LocalizationTable.Localization(Unattainment));
            str.Append("-");
            NeedEVtextAarray[2].text = str.ToString();
            NeedEVtextAarraycolor[2].TextColor(Utill_Enum.Text_Color.Gray);
        }
    }

    private int ClickCount = 0;


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
        PopUpSystem.Instance.EscPopupListAdd(this);//팝업 보여질 시 esc리스트에 추가
        gameObject.SetActive(true);

        ClickCount++;
        if (ClickCount == 1)
        {
            
        }
        else
        {
            //Hide();
        }

    }


}
