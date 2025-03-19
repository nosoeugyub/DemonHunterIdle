using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using static Utill_Enum;

public class GachaManager : MonoSingleton<GachaManager>
{
    [Header("아이템 가챠 셀 소환 갯수")]
    [SerializeField]
    private int itemGachaCellCount = 35;

    [Header("아이템 가챠시 소모 재화 수")]
    [SerializeField]
    private int itemGachaResourceAmount = 10;

    [Header("아이템 가챠 일일제한 최대치")]
    [SerializeField]
    private int itemGachaLimitMax = 100;

    [Header("슬롯 생성 시간 조정")]
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float delaySec = 0;

    [Header("특수 아이템 생성 시간 조정")]
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float specialDelaySec = 0;

    [Header("슬롯 뒤집히는 시간 조정")]
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float flipCardSec = 0;

    public int ItemGachaResourceAmount => itemGachaResourceAmount;
    public float DealySec => delaySec;
    public float SpecialDelaySec => specialDelaySec;
    public float FlipCardSec => flipCardSec;
    public int ItemGachaLimitMax => itemGachaLimitMax;
    public int ItemGachaCellCount => itemGachaCellCount;

    private void Awake()
    {
        GameEventSystem.DailyInit_Event_Event += OnTimeChanged;
    }
    private void OnTimeChanged(TimeSpan timeSpan, bool isToday)
    {
        if (isToday) //시간이 바뀌어 하루가 지남
        {
            //현재 보상 획득 여부, 현재 달성도 모두 0으로 초기화함
            for (int i = 0; i < GameDataTable.Instance.User.ArcherDailyGachaLimit.Length; i++)
            {
                GameDataTable.Instance.User.ArcherDailyGachaLimit[i] = 0;
            }
            for (int i = 0; i < GameDataTable.Instance.User.ArcherDailyGachaLimit.Length; i++)
            {
                GameDataTable.Instance.User.GuardianDailyGachaLimit[i] = 0;
            }
            for (int i = 0; i < GameDataTable.Instance.User.ArcherDailyGachaLimit.Length; i++)
            {
                GameDataTable.Instance.User.MageDailyGachaLimit[i] = 0;
            }
            UIManager.Instance.gachaPopUp.SetGachaLimit();
            return;
        }
    }

    /// <summary>
    /// 등급 이름을 기반으로 현재 선택 헌터 / 아이템 이름을 조합하여  아이템 이름을 반환함
    /// </summary>
    public string GetRewardItemName(string curName, EquipmentType equipmentType)
    {
        return $"{(SubClass)GameDataTable.Instance.User.currentHunter.GetDecrypted()}{curName}{equipmentType}";
    }

    /// <summary>
    /// 이 보상이 재화타입인지 검사
    /// </summary>
    public bool IsResource(string rewardName)
    {
        Resource_Type resource = Utill_Standard.StringToEnum<Resource_Type>(rewardName);

        return resource != Resource_Type.None;
    }

    /// <summary>
    /// 현재 목표 등급과 헌터 서브 클래스를 기반으로 가챠에서 나올 아이템의 이름을 세팅해주는 함수
    /// </summary>
    public void SetItemFullName(EquipmentType equipmentType)
    {
        for (int i = 0; i < GameDataTable.Instance.ItemGachaDataDic.Count; i++)
        {
            ItemGachaData curData = GameDataTable.Instance.ItemGachaDataDic[i + 1];
            if (curData.IsResource) continue;
            curData.ItemRewardFullName = GetRewardItemName(curData.RewardName, equipmentType);
        }
    }

    /// <summary>
    /// 가챠에서 나온 데이터를 기반으로 헌터 아이템 데이터를 수정
    /// </summary>
    /// <param name="itemGachaData">가챠를 통해 나온 아이템 데이터</param>
    /// <param name="equipmentType">어떤 부위인지</param>
    public void AddItemCount(ItemGachaData itemGachaData, EquipmentType equipmentType)
    {
        string rewardName = itemGachaData.ItemRewardFullName;
        List<HunteritemData> tmpDataList = new();
        Grade grade = HunteritemData.FindItemGradeForName(rewardName);
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
        HunteritemData data = HunteritemData.GetHunteritemData(tmpDataList, equipmentType);
        int lastEquipCount = data.EquipCountList[(int)grade - 1];
        data.EquipCountList[(int)grade - 1] += itemGachaData.Count;

        HunteritemData.Save_Slot(tmpDataList, data);
        if(lastEquipCount <= 0 && data.EquipCountList[(int)grade - 1]> 0) //보유 갯수가 0개에서 n개로 늘어났다면
        {
            int hunterindex = (int)data.Class;
            //보유 효과 추가
            HunterStat.AddHoldOptionToStat(DataManager.Instance.Hunters[hunterindex].Orginstat, data, grade);
        }
    }

    /// <summary>
    /// 랜덤 확률로 가챠를 뽑아 반환
    /// </summary>
    public ItemGachaData RandomItemGacha(Dictionary<int, ItemGachaData> data)
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float tmpValue = 0f;
        for (int i = 0; i < data.Count; i++)
        {
            tmpValue += data[i + 1].Prob;
            if (randomValue <= tmpValue)
            {
                return data[i + 1];
            }
            Debug.Log(data[i + 1].Prob);
        }
        return data[data.Count];
    }

    public void MergeGachaItem(EquipmentType equipmentType, HunteritemData curEquipData)
    {
        curEquipData.isEquip = false; //기존 장비는 일단 벗김
        GameEventSystem.Send_UnEquipment_EuipSlot(curEquipData);//기존 장비 장착해제 델리게이트
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

        int lastHighItem = -1;
        bool canMerge = false;

        for (int i = 0; i < GameManager.Instance.GachaMergeMax; i++) //현재 보유갯수 재세팅
        {
            int mergeableNum = GameDataTable.Instance.ItemGachaMergeDataDic[i + 1].Count;
            int lastEquipCount = curEquipData.EquipCountList[i];
            //합성 가능
            if (curEquipData.EquipCountList[i] >= mergeableNum)
            {
                int mergeNum = curEquipData.EquipCountList[i] / mergeableNum;
                int leftNum = curEquipData.EquipCountList[i] % mergeableNum;
                curEquipData.EquipCountList[i] = leftNum;
                curEquipData.EquipCountList[i + 1] += mergeNum;
                if(!canMerge)
                {
                    canMerge = true;
                    SoundManager.Instance.PlayAudio("Equipment");
                }
            }
            if (curEquipData.EquipCountList[i] != 0 && i + 1 > lastHighItem)
                lastHighItem = i + 1;
            if(lastEquipCount > 0 && curEquipData.EquipCountList[i] <= 0) // 머지 전에 0이 아니었는데 연산 후 보유갯수가 0이 되었다면
            {
                int hunterindex = (int)curEquipData.Class;
                //보유효과 없앰
                HunterStat.RemoveHoldItemOptionToStat(DataManager.Instance.Hunters[hunterindex].Orginstat, curEquipData, (Grade)(i+1));
            }
            if(lastEquipCount <= 0 && curEquipData.EquipCountList[i] > 0) //머지 후 보유 갯수가 늘어났다면
            {
                int hunterindex = (int)curEquipData.Class;
                //보유 효과 추가
                HunterStat.AddHoldOptionToStat(DataManager.Instance.Hunters[hunterindex].Orginstat, curEquipData, (Grade)(i + 1));
            }
        }

        if (lastHighItem < 0)
        {
            //장착할 아이템이업습니다.
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_GetEqiupmentItem"), SystemNoticeType.Default);
            return;
        }
        //마지막 아이템 장착
        string itemName = GetRewardItemName(GameDataTable.Instance.ItemGachaMergeDataDic[lastHighItem].Grade.ToString(), equipmentType);
        HunterItem.Add_PartItem(tmpDataList, equipmentType, itemName, HunteritemData.FindItemGradeForName(itemName).ToString(), true, "");
        HunteritemData data = HunteritemData.GetHunteritemData(tmpDataList, equipmentType);
        //Item item = GameDataTable.Instance.EquipmentList[(SubClass)GameDataTable.Instance.User.currentHunter.GetDecrypted()][itemName];
        //HunterItem.SetHunterItemDataWithItem(data,item);
        data.FixedOptionValues = InventoryManager.Instance.GachaChangeFixedOptionValues(data.FixedOptionTypes, data.Name);
        data.EquipCountList = curEquipData.EquipCountList;
        GameEventSystem.Send_Equipment_EuipSlot(data);//장착 델리게이트
    }
}
