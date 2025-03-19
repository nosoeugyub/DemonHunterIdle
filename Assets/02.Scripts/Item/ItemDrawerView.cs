using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 모루강화 view
/// </summary>
public class ItemDrawerView : MonoBehaviour , IPopUp
{
    #region 컴포넌트
    [SerializeField] TextMeshProUGUI Titletext;
    [SerializeField] TextMeshProUGUI Equiptext;
    [SerializeField] TextMeshProUGUI Drawername;
    [SerializeField] TextMeshProUGUI EtcName;
    [SerializeField] TextMeshProUGUI Button_Equip;

    //숫자는 랭귀지 필요없어서 분리
    [SerializeField] TextMeshProUGUI NeedAmounttext; //필요한 재화 갯수
    [SerializeField] TMP_InputField UseNumtext;
    [SerializeField] TextMeshProUGUI ItemLeveltext;
    [SerializeField] TextMeshProUGUI HaverUSeramounttext; //유저의 현재 다이아갯수

    public Image Drawerimg;
    public Image UserCurrentImg; //유저의현재가지고있는 재화 이미지
    public Image NeddCurrecnyimg; // 필요한 재화이미지


    public Button Equipbtn; //장착/해제 버튼
    public Basic_Prefab EquipBasicbtn;
    public Button Drawerbtn; //추출버튼
    public Button Drawerinfobtn; //모루강화버튼
    public Button Minusebtn; //추출 횟수 --
    public Button Plusbtn; // 추출 횟수 ++ 
    public Button CloseBtn; //닫기버튼 


    [Header("아이템 ui")]
    public GameObject ItemOptionObj;
    public Image BackGroundImage;
    public Image ItemImage;
    [SerializeField] TextMeshProUGUI ItemName;
    [SerializeField] TextMeshProUGUI Fixedtext;
    [SerializeField] List<TextMeshProUGUI> Fixedvaluetxtlist;
    [SerializeField] List<TextMeshProUGUI> valuetxt;
    #endregion


    [SerializeField] TextColorSet ItemLevelColor;














    string itemnamestr;
    public List<string> fixedoption1name;
    public List<string> fixedvalue;
    StringBuilder stringBuilder = new StringBuilder();  
    public HunteritemData _hunteritemdata;
    public int currentCount =0; // 추출횟수
    private void OnEnable()
    {
        Show();
        // Add button event listeners when the object is enabled
        Equipbtn.onClick.AddListener(OnEquipButtonClick);
        Drawerbtn.onClick.AddListener(OnDrawerButtonClick);
        Drawerinfobtn.onClick.AddListener(OnDrawerInfoButtonClick);
        Minusebtn.onClick.AddListener(OnMinusButtonClick);
        Plusbtn.onClick.AddListener(OnPlusButtonClick);
        CloseBtn.onClick.AddListener(Hide);

        //추출중에 일어나야하는 이벤트 / 추출끝나면 일어나야하는 이벤트
        GameEventSystem.Drawer_PlayAnimation_DeActiveFunc_Event += DeAtiveDrawerEvent;
        GameEventSystem.Drawer_PlayAnimation_ActiveFunc_Event += AtiveDrawerEvent;
    }



    private void OnDisable()
    {
        // Remove button event listeners to prevent memory leaks
        Equipbtn.onClick.RemoveListener(OnEquipButtonClick);
        Drawerbtn.onClick.RemoveListener(OnDrawerButtonClick);
        Drawerinfobtn.onClick.RemoveListener(OnDrawerInfoButtonClick);
        Minusebtn.onClick.RemoveListener(OnMinusButtonClick);
        Plusbtn.onClick.RemoveListener(OnPlusButtonClick);
        CloseBtn.onClick.RemoveListener(Hide);

        //추출중에 일어나야하는 이벤트 / 추출끝나면 일어나야하는 이벤트
        GameEventSystem.Drawer_PlayAnimation_DeActiveFunc_Event -= DeAtiveDrawerEvent;
        GameEventSystem.Drawer_PlayAnimation_ActiveFunc_Event -= AtiveDrawerEvent;


        //현재  데이터를 헌터리스트에 저장
        switch (_hunteritemdata.Class)
        {
            case Utill_Enum.SubClass.Archer:
                HunteritemData.Save_Slot(GameDataTable.Instance.HunterItem.Archer, _hunteritemdata);
                break;
            case Utill_Enum.SubClass.Guardian:
                HunteritemData.Save_Slot(GameDataTable.Instance.HunterItem.Guardian, _hunteritemdata);
                break;
            case Utill_Enum.SubClass.Mage:
                HunteritemData.Save_Slot(GameDataTable.Instance.HunterItem.Mage, _hunteritemdata);
                break;
        }

    }

    private void DeAtiveDrawerEvent()
    {
        UseNumtext.interactable = false;
        Equipbtn.interactable = false;
        Drawerinfobtn.interactable = false;
        Minusebtn.interactable = false;
        Plusbtn.interactable = false;
        Drawerbtn.interactable = false;
    }
    private void AtiveDrawerEvent()
    {
        UseNumtext.interactable = true;
        Equipbtn.interactable = true;
        Drawerinfobtn.interactable = true;
        Minusebtn.interactable = true;
        Plusbtn.interactable = true;
        Drawerbtn.interactable = true;
    }


    //캐릭터 바꿨을때 호출 
    public void init_Drawer()
    {
        stringBuilder.Clear();
        _hunteritemdata = null;
        currentCount = 0;
    }

    public void init_drawerCount()
    {
        currentCount = 1;
        UseNumtext.text = currentCount.ToString();
    }

    public void Update_Resource()
    {
        string datstr = Utill_Math.FormatCurrency(ResourceManager.Instance.GetDia());
        HaverUSeramounttext.text = datstr;
    }

    public void View_Darwer()//ui 갱신과함께 view를 보여주는 함수
    {
        Titletext.text = LocalizationTable.Localization("Title_Equipment");

        if (_hunteritemdata.isEquip) //장착 미장착시 ui갱신
        {
            EquipBasicbtn.SetTypeButton(ButtonType.DeActive);
        }
        else
        {
            EquipBasicbtn.SetTypeButton(ButtonType.Active);
        }

        ItemLevelColor.TextColor(Utill_Enum.Text_Color.Green);
        string gradespritename = "";
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

        //아이템
        if (_hunteritemdata.Name == "")
        {
            ItemOptionObj.gameObject.SetActive(false);
        }
        else
        {
            ItemOptionObj.gameObject.SetActive(true);
            //아이템 이름 / 등급 이미지 할당
            Sprite backGroundSprite = Utill_Standard.GetItemSprite(_hunteritemdata.ItemGrade.ToString());
            Sprite itemSprite = Utill_Standard.GetItemSprite(_hunteritemdata.Name);
  

            BackGroundImage.sprite = backGroundSprite;
            ItemImage.sprite = itemSprite;
        
        }


        stringBuilder.Clear();
        //추출기이름
        string grade = hunteritem.DrawerGrade.ToString();
        string parts = _hunteritemdata.Part.ToString();
        string hexColor = ColorUtility.ToHtmlStringRGBA(itemdrawercolor); // 색상을 HTML 형식의 문자열로 변환
        stringBuilder.Append($"<color=#{hexColor}>");
        stringBuilder.Append(LocalizationTable.Localization(hunteritem.Class.ToString()));
        stringBuilder.Append(LocalizationTable.Localization("Grade_" + grade));
        stringBuilder.Append(LocalizationTable.Localization("Equipment_" + parts.ToString()));
        stringBuilder.Append(LocalizationTable.Localization("Drawer"));
        stringBuilder.Append("</color>"); // 색상 끝
        Drawername.text = stringBuilder.ToString();
        stringBuilder.Clear();


        //아이템 이름
        itemnamestr = _hunteritemdata.Name.ToString();
        stringBuilder.Append($"<color=#{hexColor}>");
        stringBuilder.Append(LocalizationTable.Localization(_hunteritemdata.Class.ToString()));
        stringBuilder.Append($"<color=#{hexColor}>{LocalizationTable.Localization("Grade_" + grade)}</color>");
        stringBuilder.Append(LocalizationTable.Localization("Equipment_" + parts.ToString()));
        stringBuilder.Append("</color>"); // 색상 끝
        itemnamestr = stringBuilder.ToString();
        ItemName.text = itemnamestr;
        stringBuilder.Clear();


        //추출기 이미지
        Sprite _itemdarwer = Utill_Standard.GetUiSprite(gradespritename);
        Drawerimg.sprite = _itemdarwer;
        //횟수 카운트 예외사항 추가
        if (currentCount == 0)
        {
            currentCount = 1;
            UseNumtext.text = currentCount.ToString();
        }
        

        //유저의 현재 다이아 갯수
        HaverUSeramounttext.text =Utill_Math.FormatCurrency(ResourceManager.Instance.GetDia());
        UserCurrentImg.sprite = Utill_Standard.GetItemSprite(Tag.Dia);
        //해당 모루의 추출 갯수or타입
        ItemDrawerTableData gradedata = GameDataTable.Instance.ItemDrawerGradeDic[_hunteritemdata.DrawerGrade];
        NeedAmounttext.text = Utill_Math.FormatCurrency(gradedata.DrawerResourceCount);
        NeddCurrecnyimg.sprite = Utill_Standard.GetItemSprite(gradedata.DrawerResourceType.ToString());
        
        LaungugeSetting();
    }


    private int ClickCount = 0;

    public void Init_CurrentCount()
    {
         UseNumtext.text = currentCount.ToString();
    }
    public void Close()
    {
    }

    public void Hide()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        if (ItemDrawer.Instance.IsGachaAnimRun) return;
        HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);
        gameObject.SetActive(false);
        //추출기 꺼지면 dps도 갱신해주기
        PopUpSystem.Instance.CharacterInfoPopup.SetDPS();
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove(); //팝업 종료시 esc리스트에서 제거

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            //뒷배경도 ON.
            PopUpSystem.Instance.EscPopupListAdd(this);//팝업 보여질 시 esc리스트에 추가
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }

    public HunteritemData hunteritem
    {
        get { return _hunteritemdata; }
        set { _hunteritemdata = value;}
            
    }
    public void LaungugeSetting()
    {
        EtcName.text = LocalizationTable.Localization("GettingTopOption");



        Button_Equip.text = LocalizationTable.Localization("Button_Equip");
        Fixedtext.text = LocalizationTable.Localization("Fixed");

        int optioncount = _hunteritemdata.FixedOptionTypes.Count;

        fixedoption1name = new List<string>(optioncount);
        fixedvalue = new List<string>(optioncount);

        for (int i = 0; i < valuetxt.Count; i++)
        {
            Fixedvaluetxtlist[i].text = "";
            valuetxt[i].text = "";
        }


        if (optioncount > 0)
        {
            //옵션
            for (int i = 0; i < optioncount; i++)
            {
                fixedoption1name.Add(_hunteritemdata.FixedOptionTypes[i]);
                Fixedvaluetxtlist[i].text = LocalizationTable.Localization(fixedoption1name[i]);
                
            }
            //퍼센트 값으로 수치값 만들어주기
            _hunteritemdata.FixedOptionValues = InventoryManager.Instance.ChangeFixedOptionValues(_hunteritemdata.FixedOptionTypes, _hunteritemdata.FixedOptionPersent, _hunteritemdata.Name);

            var class_itme = GameDataTable.Instance.EquipmentList[_hunteritemdata.Class];
            foreach (var item in class_itme)
            {
                // class_itme 의 키값중에  classname 와 Gradename 가 함께 포함되어있으면 해당  value 값을 temp저장
                // 클래스 이름과 등급 이름이 포함된 키를 찾기
                if (item.Key.Contains(_hunteritemdata.Class.ToString()) && item.Key.Contains(_hunteritemdata.Name))
                {
                    // 조건에 맞는 아이템을 temp에 저장
                    _hunteritemdata.TotalLevel = HunteritemData.Total_OptionLevel(_hunteritemdata.FixedOptionPersent, item.Value);//평균레벨
                }
            }

            for (int i = 0; i < optioncount; i++)
            {
                fixedvalue.Add(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, fixedoption1name[i], (float)_hunteritemdata.FixedOptionValues[i]));
                valuetxt[i].text = LocalizationTable.Localization(fixedvalue[i]);
            }

            //레벨평균
            string itemLevelstr =Utill_Math.FormatCurrency(_hunteritemdata.TotalLevel);
            stringBuilder.Clear();
            stringBuilder.Append("+");
            stringBuilder.Append(itemLevelstr);
            ItemLeveltext.text = stringBuilder.ToString();
        }
        
    }

    //장착버튼
    private void OnEquipButtonClick()
    {
        SoundManager.Instance.PlayAudio("Equipment");
        //장착/해제 버튼 해당 슬롯의 슬롯아이템 데이터 값만 바뀌는 로직
        if (_hunteritemdata.isEquip == false) // 장착을 안했더라면 현재 장비 장착
        {
            if (_hunteritemdata.Name == "")
            {
                //장착할 아이템이업습니다.
                SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_GetEqiupmentItem"), SystemNoticeType.Default);
                return;
            }

            EquipBasicbtn.SetTypeButton(ButtonType.DeActive);
            _hunteritemdata.isEquip = true;
            GameEventSystem.Send_Equipment_EuipSlot(_hunteritemdata);//장착 델리게이트

        }
        else // 장착을 했더라면 현재 장비 해제
        {
            EquipBasicbtn.SetTypeButton(ButtonType.Active);
            _hunteritemdata.isEquip = false;
            GameEventSystem.Send_UnEquipment_EuipSlot(_hunteritemdata); //해제 델리게이트
        }


    }
    //추출 버튼 
    private void OnDrawerButtonClick()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        //이벤트 호출
        int count =  GameEventSystem.Send_Click_ItemDrawer(currentCount, _hunteritemdata);
        if (currentCount == 1)
        {
            count = 1;
        }
        currentCount = count; // 횟수 리턴
    }
    //강화버튼
    private void OnDrawerInfoButtonClick()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        ItemDrawerPopup.Instance.ShowVIew(_hunteritemdata);
    }

    private void OnMinusButtonClick()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        currentCount -= 1;
        if (currentCount - 1 <= 0)
        {
            currentCount = 0;
        }
        UseNumtext.text = Mathf.Max(0, currentCount).ToString();

    }

    private void OnPlusButtonClick()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        currentCount += 1;
        UseNumtext.text = (currentCount).ToString();
    }
}
