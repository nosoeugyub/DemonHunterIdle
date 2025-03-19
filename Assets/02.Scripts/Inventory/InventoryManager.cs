using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;
using CodeStage.AntiCheat.ObscuredTypes;
using BackEnd;
using static UnityEditor.Progress;
/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : itemCells 관리 및 아이템 생성 및 삭제                                      
/// </summary> 
public class InventoryManager : MonoSingleton<InventoryManager>
{
    [SerializeField] private ItemCell ItemCellPrefab; // 아이템 셀 프리팹
    [SerializeField] private Transform ScrollContent; //아이템 셀 생성할 위치

    [SerializeField] private int MaxInventoryCount = 30; //최대 아이템 셀 수

    public List<ItemCell> itemCells = new List<ItemCell>(); // 생성된 아이템 셀을 저장할 리스트

    public Dictionary<EquipmentType, Item> LoadDataDic = new Dictionary<EquipmentType, Item>(); //로드한 내용 저장

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        GameDataTable.Instance.InventoryList[0] = new List<Item>();
        GameDataTable.Instance.InventoryList[1] = new List<Item>();
        GameDataTable.Instance.InventoryList[2] = new List<Item>();

        for (int i = 0; i < MaxInventoryCount; i++) // 시작시 최대 아이템 셀 수 만큼 셀 생성
        {
            // 100개 넘어가면 SetActive로 수정 더 많아지면 재사용으로 수정해야함 
            ItemCell newItemCell = Instantiate(ItemCellPrefab, ScrollContent);
            newItemCell.transform.SetParent(ScrollContent, false);

            itemCells.Add(newItemCell);
        }
    }
    public void SettingEquipInfo()
    {
        GameDataTable.Instance.InventoryList[GameDataTable.Instance.User.currentHunter] =
        itemCells
        .Where(cell =>
        {
            bool isOccupied = cell.IsOccupied();
            return isOccupied;
        })
        .Select(cell =>
        {
            return cell.item;
        })
        .ToList();
    }

    /// <summary>
    /// 모든 itemCell 정보 초기화 아이템 삭제
    /// </summary>
    public void InitItemCell()
    {
        for (int i = 0; i < itemCells.Count; i++) // 시작시 최대 아이템 셀 수 만큼 셀 생성
        {
            itemCells[i].DeleteItemInfo();
        }
    }

    /// <summary>
    /// GameDataTable에 InventoryList 정보 가져와서 아이템 셀들 설정
    /// </summary>
    public void SettingItemCell()
    {
        // 현재 헌터 인덱스 가져오기
        int currentHunterIndex = GameDataTable.Instance.User.currentHunter;

        // 현재 헌터의 인벤토리 아이템 리스트 가져오기
        var inventoryItems = GameDataTable.Instance.InventoryList[currentHunterIndex];

        // itemCells에 아이템 정보 설정
        for (int i = 0; i < inventoryItems.Count && i < itemCells.Count; i++)
        {
            itemCells[i].SetItemInfo(inventoryItems[i]);
        }
    }
    
    
    /// <summary>
    /// 인벤토리 암호화
    /// </summary>
    public void RandomizeKey_Inventory()
    {
        if (itemCells.Count > 0) // 인벤토리 초기화가 되었는지 체크
        {
            foreach (var itemCell in itemCells)
            {
                itemCell.item.RandomizeKey_Inventory();
            }
        }
    }
    
    /// <summary>
    ///  EquipmentList csv에 존재하는 모든 아이템 생성
    /// </summary>
    public void CreateAllItems()
    {
        var currentHunter = GameDataTable.Instance.User.currentHunter;
        SubClass desiredCharacter = SubClass.Archer; // 기본적으로 아처로 초기화

        // currentHunter에 따라 원하는 캐릭터 설정
        if (currentHunter == 0)
        {
            desiredCharacter = SubClass.Archer;
        }
        else if (currentHunter == 1)
        {
            desiredCharacter = SubClass.Guardian;
        }
        else if (currentHunter == 2)
        {
            desiredCharacter = SubClass.Mage;
        }

        if (GameDataTable.Instance.EquipmentList.TryGetValue(desiredCharacter, out var characterDictionary))
        {
            foreach (var item in characterDictionary.Values)
            {
                AddItem(item, false);
            }
        }
    }

    /// <summary>
    /// 인벤토리 모든 아이템 삭제
    /// </summary>
    public void AllDeleteItemCells()
    {
        foreach (var itemCellList in itemCells)
        {
            itemCellList.DeleteItemInfo();
        }
        GameDataTable.Instance.InventoryList[GameDataTable.Instance.User.currentHunter].Clear();
    }

    /// <summary>
    /// 인덱스로 아이템 삭제
    /// </summary>
    public void DeleteItem(int listIndex, int itemIndex)
    {
        //내부 필요시 작성
    }

    /// <summary>
    /// 고정옵 값과 고정옵 퍼센트 설정
    /// </summary>
    public Tuple<List<ObscuredDouble>, List<ObscuredDouble>> SetFixedOptionValues(Item item)
    {
        List<Option> options = item.GetOptions;
        List<ObscuredDouble> valueList = new List<ObscuredDouble>();
        List<ObscuredDouble> valuePercentList = new List<ObscuredDouble>();

        for (int i = 0; i < options.Count; i++)
        {
            double minValue = item.FixedOptionMinValue[i]; // 최소 값
            double maxValue = minValue;
            double unit = 0;

            maxValue = item.FixedOptionMaxValue[i]; // 최대 값
            unit = item.FixedOptionValueUnit[i];         // 단위

            double minwegiht = item.FixedOptionWeightValue[0];
            double maxwegight = 0;
            if (item.FixedOptionWeightValue.Length > 1)
                maxwegight = item.FixedOptionWeightValue[1];
            else
                maxwegight = minwegiht;

            // 가능한 값 생성
            List<double> possibleValues = new List<double>();
            List<double> weights = new List<double>();
            List<double> weightspersent = new List<double>();

            if (item.FixedOptionMaxValue.Count() >= i + 1)
            {
                //index값구하기
                for (double k = minValue; k <= maxValue; k += unit)
                {
                    possibleValues.Add(k);
                }
            }


            // 가중치 계산 (MaxIndex 사용)
            for (int j = 0; j < possibleValues.Count; j++)
            {
                double maxindex = possibleValues.Count - 1;
                double tempWeight = maxwegight - ((maxwegight - minwegiht) * j) / maxindex;
                weights.Add(tempWeight);
            }



            // 총 가중치 계산
            double totalWeight = 0f;
            foreach (var weight in weights)
            {
                totalWeight += weight;
            }

            //확률구하기
            foreach (var weight in weights)
            {
                weightspersent.Add(totalWeight / weight);
            }

            if(possibleValues.Count > 0)
            {
                // 확률에 따라 해당 possibleValues 배열에서 랜덤값 구하기
                // 랜덤 값 생성: 0에서 총 가중치 사이의 랜덤 값
                double randomValue = UnityEngine.Random.Range(0f, (float)totalWeight);
                double cumulativeWeight = 0f;
                double selectedValue = possibleValues[0];
                
                // 가중치에 따라 랜덤 값 선택
                for (int m = 0; m < weights.Count; m++)
                {
                    cumulativeWeight += weights[m];
                    if (randomValue <= cumulativeWeight)
                    {
                        selectedValue = possibleValues[m];
                        break;
                    }
                }
                
                // 선택된 값을 저장
                valueList.Add(selectedValue);
                
                // 퍼센트 값 계산
                double normalizedValue = (selectedValue - minValue) / (maxValue - minValue);
                valuePercentList.Add(normalizedValue);
            }
        }

        return new Tuple<List<ObscuredDouble>, List<ObscuredDouble>>(valueList, valuePercentList);
    }


    /// <summary>
    /// 퍼센트 값으로 수치 공식 내버리기
    /// </summary>
    public List<Double> ChangeFixedOptionValues(List<string> Options , List<double> Persents , string itemname )
    {
        List<Double> tempresult = new List<double>();
        List<Option> options = new List<Option>();

        Item item = GetItem(itemname);

        for (int i = 0; i < Options.Count; i++)
        {
            Option option = CSVReader.ParseEnum<Option>(Options[i]);
            options.Add(option);

            double minValue = item.FixedOptionMinValue[i]; // 최소 값
            double maxValue = 0; // 최대 값
            maxValue = item.FixedOptionFinalValue[i];
            double unit = 0;
            unit = item.FixedOptionValueUnit[i];         // 단위

            //1차 
            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;
            //1차
            temp_1 = maxValue - minValue;
            //2차
            if (Persents.Count() >= i + 1)
                temp_2 = Persents[i] * temp_1;
            else
                temp_2 = temp_1;
            //3차
            temp_3 = minValue + temp_2;
            //4차
            temp_4 = temp_3 / unit;
            //5차 소숫점에서 반올림
            temp_5 = Math.Round(temp_4);
            //6차
            temp_6 = temp_5 * unit;

            tempresult.Add(temp_6);
        }

        return tempresult;
    }

    /// <summary>
    /// 가챠로 뽑은 아이템의 고정옵을 세팅
    /// </summary>
    /// <param name="Options"></param>
    /// <param name="itemname"></param>
    /// <returns></returns>
    public List<Double> GachaChangeFixedOptionValues(List<string> Options, string itemname)
    {
        List<Double> tempresult = new List<double>();
        List<Option> options = new List<Option>();

        Item item = GetItem(itemname);

        for (int i = 0; i < Options.Count; i++)
        {
            tempresult.Add(item.FixedOptionMinValue[i]); //min value기준으로 세팅함
        }

        return tempresult;
    }

    /// <summary>
    /// 전체 랜덤옵션중 RandomOptionCnt 개수만큼만 리스트 반환
    /// </summary>
    private List<Option> SetRandomOptions(Option[] options, int randomOptionCnt)
    {
        options = options.OrderBy(x => UnityEngine.Random.value).ToArray();
        return options.Take(randomOptionCnt).ToList();
    }

    
    /// <summary>
    /// 아이템 추가 서버에서 받아온 데이터로 생성되는것과 인게임 내에서 생성되는것 모두 이 함수에서 처리
    /// </summary>
    public void AddItem(Item item, bool isInit)
    {
        foreach (var itemCell in itemCells)
        {
            if (isInit) // 서버에서 받아온 데이터로 아이템 생성
            {
                if (!itemCell.IsOccupied())
                {
                    itemCell.SetItemInfo(item);

                    if (item.equipmentItem.isEquip)
                    {
                        LoadDataDic.Add(item.equipmentItem.type, item);

                        HunterStat.AddChangOptionToStat(NSY.DataManager.Instance.Hunters[(int)item.character].Orginstat, item);
                    }

                    UpdateItemCell();

                    break; // 아이템을 추가했으므로 루프 종료
                }
            }
            else // 인게임에서 아이템 획득
            {
                if (!itemCell.IsOccupied())
                {
                    Item setCompleteItem = SetItemValue(item);
                    itemCell.SetItemInfo(setCompleteItem);
                    GameDataTable.Instance.InventoryList[GameDataTable.Instance.User.currentHunter].Add(setCompleteItem);
                    return;
                }
            }

            // isInit이 true일 때 아이템이 추가되면 루프를 종료합니다.
            if (isInit && item.equipmentItem.isEquip)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 고정옵 랜덤옵 관련 리스트 반환한것들 받아서 아이템 값 설정
    /// </summary>
    public Item SetItemValue(Item item)
    {
        Tuple<List<ObscuredDouble>, List<ObscuredDouble>> fixedTuple = SetFixedOptionValues(item);

        item.FixedValues = fixedTuple.Item1;
        item.FixedValuesPercent = fixedTuple.Item2;

        item.SelectRandOptions = SetRandomOptions(item.equipmentItem.randomOptions, GameDataTable.Instance.ItemRandomOptionCnt[item.GetName]);

        return item;
    }
    
    /// <summary>
    /// 모든 아이템 셀 토글 텍스트 업데이트
    /// </summary>
    public void UpdateItemCell()
    {
        foreach (var itemCell in itemCells)
        {
            // ItemCell 업데이트
            itemCell.ToggleEquipText();
        }
    }

    /// <summary>
    /// 모든 아이템을 가지고 있는 딕셔너리에서 매개변수와 같은 이름의 아이템이 있다면 반환  
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Item GetItem(string name)
    {
        foreach (var characterDict in GameDataTable.Instance.EquipmentList.Values)
        {
            if (characterDict.TryGetValue(name, out var item))
            {
                return item;
            }
        }
        return new Item(); // name에 해당하는 Item을 찾지 못한 경우
    }


}
