using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;
using BackEnd;
using EnhancedUI.EnhancedScroller;
using System.Linq;
using System;
using NSY;
using Google.Protobuf.WellKnownTypes;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 헌터 업그레이트 뷰관련
/// /// </summary>
public class HunterUpGradeView : EnhancedScrollerCellView
{
    [SerializeField] private int slotindex;
    public int Slotindex
    {
        get { return slotindex; }
        set { slotindex = value; }  
    }



    [SerializeField] private Utill_Enum.Upgrade_Type upgradeType;
    public Utill_Enum.Upgrade_Type UpgradeType
    {
        get { return upgradeType; }
        set { upgradeType = value; }
    }
    Dictionary<string, int> userdata;

    public TextMeshProUGUI StatText;
    public TextMeshProUGUI StatLevel;
    public TextMeshProUGUI StatCurrentlevel;
    public TextMeshProUGUI Nextlevel;
    public TextMeshProUGUI UpgradeName;
    [SerializeField] private TextMeshProUGUI upgradevaluetext;

    public Utill_Enum.SubClass character;
    public TextMeshProUGUI Upgradevaluetext
    {
        get
        {
            return upgradevaluetext;
        }
        set
        {
            upgradevaluetext = value;
        }
    }
    public TextMeshProUGUI UpgradeLevelLimt;

    public TextColorSet leveltxt;
    public TextColorSet Nextleveltxt;
   

    public Image StatGaugebar;
    public Image CurrneyImg;


    public GameObject DeActiveLevelObj;
   [SerializeField]  private Button _UpgradeBtn;
    public Button UpgradeBtn
    {
        get { return _UpgradeBtn; }
        set { _UpgradeBtn = value; }
    }

    [SerializeField] Basic_Prefab _upgradebtn;
    public Basic_Prefab upgradebtn
    {
        get { return _upgradebtn; }
        set { _upgradebtn = value; }
    }


    public TextMeshProUGUI DeActiveTxt;
    public TextColorSet DeActvieTxtColor;



    [SerializeField] private bool isNoBuy = false;
    public bool IsNoBuy
    {
        get
        {
            return isNoBuy;
        }    
        set
        { isNoBuy = value;}
    }
    [SerializeField] private bool isLock = false;
    public bool IsLock
    {
        get
        {
            return isLock;
        }
        set
        { isLock = value; }
    }

    [SerializeField] private int buygold;
    public int BuyGold
    {
        get { return buygold; }
        set { buygold = value; }
    }
    [SerializeField] private int buydia;
    public int Buydia
    {
        get { return buydia; }
        set { buydia = value; }
    }
    [SerializeField]
    private int buyLevel;
    public int BuyLevel
    {
        get { return buyLevel; }
        set { buyLevel = value; }
    }

    void Awake()
    {
        GameEventSystem.GameCheckResource_GameEventHandler_Event += CheackingCurrency;
    }
    public void SetButtonFunc()
    {
        int index = (int)character;
        UpgradeBtn.onClick.AddListener(delegate { UpgradeClickBtn(GameDataTable.Instance.User.GetUpgradeList(index), UpgradeType, slotindex); });

    }

    private void CheackingCurrency(int level, int Gold, int Dia)
    {
        int index = (int)character;

        //데이터 파싱
        userdata = GameDataTable.Instance.User.GetUpgradeList(index)[Slotindex];


        //재화 검열
        bool isbuy = Hunter_UpgradeData.Check_IsBuyCell(userdata, Gold, Dia);
        if (!isbuy)
        {
            upgradebtn.SetTypeButton(ButtonType.DeActive);
            UpgradeBtn.interactable = false;
        }
        else if (isbuy)
        {
            upgradebtn.SetTypeButton(ButtonType.Active);
            UpgradeBtn.interactable = true;
        }


        //레벨 제한
        // 업그레이드 타입을 문자열로 변환
        string firstKey = userdata.Keys.FirstOrDefault(); // 첫 번째 key 가져오기
        int firstValue = userdata[firstKey]; // 첫 번째 key에 해당하는 값 가져오기
        Hunter_UpgradeData data = GameDataTable.Instance.HunterUpgradeDataDic[firstValue];

        //레벨 제한 구하기
        int limitlevel = data.LevelLimit;

        if (level >= limitlevel) 
        {
            DeActiveLevelObj.SetActive(false);
        }
        else
        {
            DeActiveLevelObj.SetActive(true);
        }
    }

    private bool CheckLevelLimit(int level)
    {
        return BuyLevel > level;
    }

    private bool CheckCurrencyLimit(int Gold, int Dia)
    {
        return BuyGold > Gold || buydia > Dia;
    }



    private void OnEnable()
    {
        int index = (int)character;
        int level = GameDataTable.Instance.User.HunterLevel[index];
        int gold = ResourceManager.Instance.GetGold();
        int Dia = ResourceManager.Instance.GetDia();
        GameEventSystem.Send_GameCheckResource_GameEventHandler(level, gold, Dia);
        
        CheackingCurrency(level , gold ,Dia);
    }
    //업그레이ㅡㄷ 버튼눌렀승ㄹ때
    private void UpgradeClickBtn(List<Dictionary<string, int>> hunterUpgradeList, Upgrade_Type upgradeType , int buttonindex)
    {
        //데이터 파싱
        userdata = hunterUpgradeList[buttonindex];
        // 업그레이드 타입을 문자열로 변환
        string firstKey = userdata.Keys.FirstOrDefault(); // 첫 번째 key 가져오기
        int firstValue = userdata[firstKey]; // 첫 번째 key에 해당하는 값 가져오기
        Hunter_UpgradeData data = GameDataTable.Instance.HunterUpgradeDataDic[firstValue];

        //업그레이드 할수있는 상황인지 먼저 체크 해야함 
        //재화및 맥스레벨일때 검사
        int index = (int)character;
        int levelbefore = GameDataTable.Instance.User.HunterLevel[index];
        int goldbefore = ResourceManager.Instance.GetGold();
        int Diabefore = ResourceManager.Instance.GetDia();


        bool ismaxs = Hunter_UpgradeData.Upgrade_MaxLevelCheck(hunterUpgradeList, upgradeType);
        bool isbuys = Hunter_UpgradeData.Check_IsBuyCell(userdata, goldbefore, Diabefore);
        if (ismaxs || !isbuys)
        {
            upgradebtn.SetTypeButton(ButtonType.DeActive);
            UpgradeBtn.interactable = false;
            return;
        }
        else if (isbuys && !ismaxs)
        {
            upgradebtn.SetTypeButton(ButtonType.Active);
            UpgradeBtn.interactable = true;
        }


        //업글레이드 버튼을 눌렀을때
        Hunter_UpgradeData.Upgrade_UserStat(hunterUpgradeList, UpgradeType, character);

        //재화감소
        ResourceManager.Instance.Minus_ResourceType(data.ResourceType, data.ResourceCount);



        //Cell ui갱신
        Update_UpgradeSlot(hunterUpgradeList[buttonindex]);
        SoundManager.Instance.PlayAudio("UpgradeClick");
        //재화및 맥스레벨일때 검사
        int level = GameDataTable.Instance.User.HunterLevel[index];
        int gold = ResourceManager.Instance.GetGold();
        int Dia = ResourceManager.Instance.GetDia();
        GameEventSystem.Send_GameCheckResource_GameEventHandler(level, gold, Dia);

        bool ismax = Hunter_UpgradeData.Upgrade_MaxLevelCheck(hunterUpgradeList, upgradeType);
        bool isbuy = Hunter_UpgradeData.Check_IsBuyCell(userdata, gold, Dia);
        if (ismax || !isbuy)
        {
            upgradebtn.SetTypeButton(ButtonType.DeActive);
            UpgradeBtn.interactable = false;
            return;
        }
        else if(isbuy && !ismax)
        {
            upgradebtn.SetTypeButton(ButtonType.Active);
            UpgradeBtn.interactable = true;
        }

        
    }
    //해당 슬롯을 업글레이드 했으면  업데이트 해야함
    public void Update_UpgradeSlot(Dictionary<string, int> userdata)
    {
         
        //기본 텍스트부터 바인딩
        string firstKey = userdata.Keys.FirstOrDefault(); // 첫 번째 key 가져오기
        int firstValue = userdata[firstKey]; // 첫 번째 key에 해당하는 값 가져오기
       

        //기본 텍스트부터 바인딩
        SetUpgradeLevl(firstValue); //레벨정해주기
        //레벨에따른 능력치값 셋팅
        SetStatValue(firstValue);
        //레벨에 따른 재화 이미지 및 재화량 셋팅
        var gamedata = GameDataTable.Instance.HunterUpgradeDataDic[firstValue]; //현재 레벨에 맞는 업그레이드 데이터
        SetCurrencyValue(gamedata.ResourceCount, gamedata.ResourceType);
    }


    //스크롤할때마다 갱신해주는 로직
    public void UpdateCell(Dictionary<string, int> userdata , Dictionary<int, Hunter_UpgradeData> upgradedata, int index)
    {
        Slotindex = index;

        //기본 텍스트부터 바인딩
        string firstKey = userdata.Keys.FirstOrDefault(); // 첫 번째 key 가져오기
        StatText.text = LocalizationTable.Localization(firstKey); //텍스트 이름 정해주기

        int firstValue = userdata[firstKey]; // 첫 번째 key에 해당하는 값 가져오기
        SetUpgradeLevl(firstValue); //레벨정해주기

        UpgradeType = CSVReader.ParseEnum<Utill_Enum.Upgrade_Type>(firstKey);

       //레벨에따른 능력치값 셋팅
       SetStatValue(firstValue);
        //레벨에 따른 재화 이미지 및 재화량 셋팅
        var gamedata = upgradedata[firstValue]; //현재 레벨에 맞는 업그레이드 데이터
        SetCurrencyValue(gamedata.ResourceCount , gamedata.ResourceType);
        ////랭귀지 셋팅
        //LanguageSetting();
    }


    public void SetUpgradeLevl(int level)
    {
        int tempgrade = Hunter_UpgradeData.Upgrade_GetMaxLevel();
        if (level >= tempgrade)
        {
            StatGaugebar.fillAmount =1;
            leveltxt.TextColor(Text_Color.Red);
        }
        StatLevel.text = "Lv. " + level.ToString();
                
        //게이지바도 증가
        float guage = (float)level / (float)tempgrade;
        StatGaugebar.fillAmount = Mathf.Clamp01(guage);
    }



    public void SetStatValue(int _currentSkillLevel )
    {
        int templevel = _currentSkillLevel;
        //현재레벨의 다음레벨이 만렙 가져오기
        int _MaxStatLevel = Hunter_UpgradeData.Upgrade_GetMaxLevel();

        //현재 레벨에대한 능력치 가져오기 
        Hunter_UpgradeData data = GameDataTable.Instance.HunterUpgradeDataDic[templevel];
        //현재 레벨 할당
        int index = (int)character;
        float currentvalue = Hunter_UpgradeData.Upgrade_GetLevelStatVAlue(GameDataTable.Instance.User.GetUpgradeList(index), UpgradeType, _currentSkillLevel);
        StatCurrentlevel.text = StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, UpgradeType.ToString(), currentvalue);
        //다음 레벨 조건검사
        if (_currentSkillLevel + 1 >= _MaxStatLevel)
        {
            templevel = _MaxStatLevel;
        }
        else
        {
            templevel = _currentSkillLevel + 1;
        }
        //다음 레벨에대한 능력치 가져오기 
        Hunter_UpgradeData Nextdata = GameDataTable.Instance.HunterUpgradeDataDic[templevel];

        //현재 레벨이 만렙일경우
        if (_currentSkillLevel >= _MaxStatLevel)//업그레이드   만렙일때
        {
            _currentSkillLevel = _MaxStatLevel;
            //다음레벨
            Nextlevel.text = Tag.MAX;
            Nextleveltxt.TextColor(Utill_Enum.Text_Color.Red);
        }
        else
        {
            //다음레벨할당
            index = (int)character;
            float Nextvalue = Hunter_UpgradeData.Upgrade_GetLevelStatVAlue(GameDataTable.Instance.User.GetUpgradeList(index), UpgradeType, templevel);
            Nextlevel.text = StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, UpgradeType.ToString(), Nextvalue);
            Nextleveltxt.TextColor(Utill_Enum.Text_Color.Green);
        }
    }

     public void SetCurrencyValue(int Value, Utill_Enum.Resource_Type tpye)
     {
        int CurrentuserValue = 0;
        switch (tpye)
        {
            case Resource_Type.Gold:
                CurrentuserValue = ResourceManager.Instance.GetGold();
                //이미지도 변환
                CurrneyImg.sprite = Utill_Standard.GetItemSprite(Tag.Gold);
                break;
            case Resource_Type.Dia:
                CurrentuserValue = ResourceManager.Instance.GetDia();
                //이미지도 변환
                CurrneyImg.sprite = Utill_Standard.GetItemSprite(Tag.Dia);
                break;
        }

        Upgradevaluetext.text = Value.ToString();

        //살수있는지없는지 판단하여 글자색깔바꿈
        if (CurrentuserValue < Value)
        {
            
            upgradebtn.SetTypeButton(Utill_Enum.ButtonType.DeActive);
            UpgradeBtn.interactable = false;
        }
        else
        {
            upgradebtn.SetTypeButton(Utill_Enum.ButtonType.Active);
            UpgradeBtn.interactable = true;
        }


    }

    public void LanguageSetting()
    {
        //랭귀지 셋팅을 해줘야함 아직 작업안함
    }
}
