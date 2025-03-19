using NSY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-11
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 가챠를 진행하는 팝업
/// </summary>
public class GachaPopUp : MonoBehaviour, IPopUp
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI gachaNameText;
    [SerializeField]
    private TextMeshProUGUI curResourceText;
    [SerializeField]
    private TextMeshProUGUI useResourceText;
    [SerializeField]
    private TextMeshProUGUI dailyGachaLimitText;
    [SerializeField]
    private TextMeshProUGUI gachaButtonText;
    [SerializeField]
    private TextMeshProUGUI mergeAllButtonText;

    [SerializeField]
    private Button gachaButton; //가챠 소환 버튼
    [SerializeField]
    private Button achievementButton;//가챠 업적 버튼
    [SerializeField]
    private Button dictionaryButton;//가챠 도감 버튼
    [SerializeField]
    private Button Gachexit;//가챠 중지 버튼
    [SerializeField]
    private Button mergeAllButton;//일괄합성/장착 버튼

    [SerializeField]
    private Transform gachaCellParent;
    [SerializeField]
    private Transform gachaMergeItemCellParent;
    [SerializeField]
    private Image resourceImage;
    [SerializeField]
    private Image currentResourceImage;
    [SerializeField]
    private Toggle multipleToggle; //연속 뽑기 토글
    [SerializeField]
    private GachaCell gachaCellPrefab;
    [SerializeField]
    private GachaMergeItemCell gachaMergeItemCellPrefab;

    private Basic_Prefab gachabuttonBasicPrefab; //가챠 소환 버튼 베이직 프리펩

    private List<GachaCell> gachaCellList = new();
    private List<GachaMergeItemCell> gachaMergeItemCellList = new();

    private bool isInit = false;
    private int ClickCount = 0;

    private EquipmentType curPart;
    public EquipmentType CurPart => curPart;
    private ItemGachaData lastItemGachaData; //연속뽑기시 마지막으로 뽑힌 가챠의 데이터

    bool isMultiplied = false; //연속 뽑기인지
    public bool IsContinue = false; //가챠를 뽑는 중인지 (가챠 연출이 나오는 중인지)

    HunteritemData curEquipData; //현재 무엇을 장착했는지
    public HunteritemData CurEquipData => curEquipData;
    private void Awake()
    {
        GameEventSystem.GamePluseDia_GameEventHandler_Event += UpdateResourceText;
        GameEventSystem.GameMinusDia_GameEventHandler_Event += UpdateResourceText;

        GameEventSystem.Gacha_PlayAnimation_ActiveFunc_Event += GachaPlay_StopEvent;
        GameEventSystem.Gacha_PlayAnimation_DeActiveFunc_Event += GachaPlay_StartEvent;
    }



    public void Update_Resource()
    {
        string datstr = Utill_Math.FormatCurrency(ResourceManager.Instance.GetDia());
        curResourceText.text = datstr;
    }

    private void UpdateResourceText(Utill_Enum.Resource_Type Resourcetpye, int Value = 0)
    {
        if (gameObject.activeInHierarchy)
        {
            curResourceText.text = ResourceManager.Instance.GetDia().ToString();
        }
    }
    private void Initialize()
    {
        if (isInit) return;
        InitButtons();

        gachabuttonBasicPrefab = gachaButton.GetComponent<Basic_Prefab>();

        for (int i = 0; i < GachaManager.Instance.ItemGachaCellCount; i++)
        {
            var cell = Instantiate(gachaCellPrefab, gachaCellParent);
            gachaCellList.Add(cell);
        }

        //합성 현황 셀 생성
        for(int i = 0; i < GameManager.Instance.GachaMergeMax; i++)
        {
            var cell = Instantiate(gachaMergeItemCellPrefab, gachaMergeItemCellParent);
            gachaMergeItemCellList.Add(cell);
        }

        mergeAllButtonText.text = LocalizationTable.Localization("Button_GachaMergeAllAndEquip");
        titleText.text = LocalizationTable.Localization("Title_Gacha");
        gachaButtonText.text = LocalizationTable.Localization("Title_Gacha");

        isInit = true;
    }
    private void InitButtons()
    {
        dictionaryButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.gachaDictionaryPopUp.Show();
        });
        achievementButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.gachaAchievementPopUp.Show();
        });
        gachaButton.onClick.AddListener(() => StartGacha());

        multipleToggle.onValueChanged.AddListener((bool isSelected) =>
        {
            isMultiplied = isSelected;
            SoundManager.Instance.PlayAudio("UIClick");
        });
        mergeAllButton.onClick.AddListener(() =>
        {
            GachaManager.Instance.MergeGachaItem(curPart, curEquipData);
            SetMergeCell();
        });

        multipleToggle.isOn = false;
        isMultiplied = false;
    }
    private void AllResetCell()
    {
        for (int i = 0; i < gachaCellList.Count; i++)
        {
            gachaCellList[i].ResetCell(curPart);
        }
    }

    private void SetMergeCell()
    {
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
        for (int i = 0; i < gachaMergeItemCellList.Count; i++)
        {
            string itemName = GachaManager.Instance.GetRewardItemName(GameDataTable.Instance.ItemGachaMergeDataDic[i+1].Grade.ToString(), CurPart);
            Item item = GameDataTable.Instance.EquipmentList[(SubClass)GameDataTable.Instance.User.currentHunter.GetDecrypted()][itemName];
            int tmp = tmpDataList[(int)CurPart-1].EquipCountList[i];
            gachaMergeItemCellList[i].SetItemInfo(item, tmp != 0);
            gachaMergeItemCellList[i].SetEuipNum(tmp, GameDataTable.Instance.ItemGachaMergeDataDic[i+1].Count);
        }
    }


    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        if (IsContinue) return;
        ClickCount = 0;
        HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);

        //가챠창 꺼지면 dps도 갱신해주기
        PopUpSystem.Instance.CharacterInfoPopup.SetDPS();
        PopUpSystem.Instance.EscPopupListRemove();
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
    public void SetGachaLimit()
    {
        if (dailyGachaLimitText == null) return;
        dailyGachaLimitText.text = ($"{GetCurLimit()} / {GachaManager.Instance.ItemGachaLimitMax}");
    }
    public void Setting(EquipmentType parts)
    {
        curPart = parts;
        IsContinue = false;

        resourceImage.sprite = Utill_Standard.GetItemSprite("Dia");
        currentResourceImage.sprite = Utill_Standard.GetItemSprite("Dia");
        curResourceText.text = Utill_Math.FormatCurrency(ResourceManager.Instance.GetDia());
        useResourceText.text = Utill_Math.FormatCurrency(GachaManager.Instance.ItemGachaResourceAmount);
        dailyGachaLimitText.text = ($"{Utill_Math.FormatCurrency(GetCurLimit())} / {Utill_Math.FormatCurrency(GachaManager.Instance.ItemGachaLimitMax)}");

        gachaNameText.text = $"{LocalizationTable.Localization(((SubClass)GameDataTable.Instance.User.currentHunter.GetDecrypted()).ToString())} {LocalizationTable.Localization("Equipment_" + parts.ToString())}";

        Initialize();
        AllResetCell();
        GachaManager.Instance.SetItemFullName(CurPart); //SetMergeCell 전에 실행되어야함
        SetMergeCell();

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

        //현재 장착 데이터
        curEquipData = HunteritemData.GetHunteritemData(tmpDataList, CurPart);
    }


    /// <summary>
    /// EquipmentType을 바탕으로 DailyGachaLimit를 구하기 위한 인덱스 반환
    /// </summary>
    private int GetLimitIndex()
    {
        int tmpIndex = 0;
        switch (curPart)
        {
            case EquipmentType.Pet:
                tmpIndex = 0;
                break;
            case EquipmentType.Hat:
                tmpIndex = 1;
                break;
            case EquipmentType.Cloak:
                tmpIndex = 2;
                break;
            case EquipmentType.Necklace:
                tmpIndex = 3;
                break;
            case EquipmentType.Wing:
                tmpIndex = 4;
                break;
            case EquipmentType.Mask:
                tmpIndex = 5;
                break;
            case EquipmentType.Back:
                tmpIndex = 6;
                break;
            case EquipmentType.Earrings:
                tmpIndex = 7;
                break;
        }
        return tmpIndex;
    }
    private int GetCurLimit()
    {
        switch (GameDataTable.Instance.User.currentHunter)
        {
            case 0: //궁수
                return GameDataTable.Instance.User.ArcherDailyGachaLimit[GetLimitIndex()];
            case 1: //수호자
                return GameDataTable.Instance.User.GuardianDailyGachaLimit[GetLimitIndex()];
            case 2: //법사
                return GameDataTable.Instance.User.MageDailyGachaLimit[GetLimitIndex()];
        }
        return 0;
    }
    private bool CanGacha()
    {
        if (GetCurLimit() >= GachaManager.Instance.ItemGachaLimitMax)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_LimitReached"));
            return false;
        }
        if (ResourceManager.Instance.CheckResource(Resource_Type.Dia, GachaManager.Instance.ItemGachaResourceAmount) == false)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_NeedMoreResource"));
            return false;
        }
        if (isMultiplied && lastItemGachaData != null)
        {
            if (lastItemGachaData.FrameEffect != "")
                return false;
        }

        return true;
    }

    private void StartGacha()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        if (IsContinue)
            return;
        StartCoroutine(Gacha());
    }

    // 각 슬롯들을 뒤집어줌
    IEnumerator Gacha()
    {
        AllResetCell();
        lastItemGachaData = null;

        // 배수 적용이 되있음
        int maxTime = isMultiplied ? 10 : 1;
        IsContinue = true;
        PopUpSystem.Instance.CanUndoByESC = false; //뽑기 중엔 esc단축키 사용불가

        GameEventSystem.Send_Gacha_PlayAnimation_ActiveFunc(); // 10연뽑 나오는동안 중지해야할 기능 이벤트


        for (int i = 0; i < maxTime; i++)
        {
            if (!CanGacha())
                break;

            // 횟수 증가
            dailyGachaLimitText.text = ($"{GetCurLimit() + GachaManager.Instance.ItemGachaCellCount} / {GachaManager.Instance.ItemGachaLimitMax}");

            switch (GameDataTable.Instance.User.currentHunter)
            {
                case 0: //궁수
                    GameDataTable.Instance.User.ArcherDailyGachaLimit[GetLimitIndex()] += GachaManager.Instance.ItemGachaCellCount;
                    GameDataTable.Instance.User.ArcherTotalGacha[curPart.ToString()] += GachaManager.Instance.ItemGachaCellCount;
                    break;
                case 1: //수호자
                    GameDataTable.Instance.User.GuardianDailyGachaLimit[GetLimitIndex()] += GachaManager.Instance.ItemGachaCellCount;
                    GameDataTable.Instance.User.GuardianTotalGacha[curPart.ToString()] += GachaManager.Instance.ItemGachaCellCount;
                    break;
                case 2: //법사
                    GameDataTable.Instance.User.MageDailyGachaLimit[GetLimitIndex()] += GachaManager.Instance.ItemGachaCellCount;
                    GameDataTable.Instance.User.MageTotalGacha[curPart.ToString()] += GachaManager.Instance.ItemGachaCellCount;
                    break;
            }

            ResourceManager.Instance.Minus_ResourceType(Resource_Type.Dia, GachaManager.Instance.ItemGachaResourceAmount);
            GameEventSystem.Send_Resource_Update(); //재화 업데이트
           
            
            int cnt = 0;

            // 결과 보여줌
            foreach (var slot in gachaCellList)
            {
                cnt++;

                ItemGachaData itemGachaData = GachaManager.Instance.RandomItemGacha(GameDataTable.Instance.ItemGachaDataDic);

                slot.Flip(itemGachaData);
                var amount = itemGachaData.Count;
                var itemName = itemGachaData.RewardName;

                //슬롯 다 뒤집힐때까지 기다림
                yield return new WaitUntil(() => slot.isFinalized);

                //재화 여부 판별후 받은 보상을 더해줌
                if (GachaManager.Instance.IsResource(itemName))
                    ResourceManager.Instance.Pluse_ResourceType(Utill_Standard.StringToEnum<Resource_Type>(itemName), amount);
                else
                {
                    GachaManager.Instance.AddItemCount(itemGachaData, CurPart);
                    SetMergeCell();
                }

                if (itemGachaData.ChatBroadcast) //채팅을 보내야할 아이템이라면
                {
                    ChatManager.Instance.SendChatBroadcast(Utill_Enum.ChatBroadcastType.Gacha, true, ChatBroadcastStringGenerator.GenerateBossFirstClearString(itemName));
                }

                if (cnt == gachaCellList.Count) //마지막 셀이 뒤집혔다면
                {
                    if (i < maxTime - 1)
                    {
                        yield return new WaitUntil(() => slot.isFlipDone); //한 타임 다 돌면 마지막 슬롯이 뒤집힐 때까지 대기함
                        AllResetCell();
                    }

                    // 희귀 또는 전설 상자가 나올시 즉시 종료
                    if (isMultiplied && lastItemGachaData != null)
                    {
                        if (lastItemGachaData.FrameEffect != "")
                        {
                            maxTime = 0;
                        }
                    }

                }
            }
        }
        IsContinue = false;
        PopUpSystem.Instance.CanUndoByESC = true;
        GameEventSystem.Send_Gacha_PlayAnimation_DeActiveFunc(); // 10연뽑 나오는동안 중지해제할 기능 이벤트
    }

    private void GachaPlay_StartEvent()
    {
        multipleToggle.interactable = true;
        gachaButton.interactable = true;
        achievementButton.interactable = true;
        dictionaryButton.interactable = true;
        Gachexit.interactable = true;
        mergeAllButton.interactable = true;
        for(int i = 0; i < gachaMergeItemCellList.Count; i++)
        {
            gachaMergeItemCellList[i].SetButtonInteractable(true);
        }
    }

    private void GachaPlay_StopEvent()
    {
        multipleToggle.interactable = false;
        gachaButton.interactable = false;
        achievementButton.interactable = false;
        dictionaryButton.interactable = false;
        Gachexit.interactable = false;
        mergeAllButton.interactable = false;
        for (int i = 0; i < gachaMergeItemCellList.Count; i++)
        {
            gachaMergeItemCellList[i].SetButtonInteractable(false);
        }
    }
}
