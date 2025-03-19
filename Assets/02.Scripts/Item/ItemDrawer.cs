using CodeStage.AntiCheat.ObscuredTypes;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using System.Diagnostics.PerformanceData;
using System.Security.Policy;


/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 데이터 관련 컨트롤러
/// </summary>
public class ItemDrawer : MonoSingleton<ItemDrawer>
{
    public ItemDrawerView itemDrawerview;
    [Header("뽑기 파티클 시간")]
    public float particletime = 2.5f;
    [Header("파티클 위치")]
    [SerializeField] Vector3 ItemDrawerEffectPos;

    [Header("결과팝업창 유지 시간")]
    [SerializeField] float ResultItemPopupTime;
    int leveldifference = -1;

    [SerializeField] NewItemPopUp newitmepopup;
    [SerializeField] GameObject ParticaleParent;




    //클레스
    HunteritemData hunterdata;

    public bool IsGachaAnimRun = false;

    private void Awake()
    {
        GameEventSystem.Click_ItemDrawer_Event += Itemdrawsequence; //아이템추출 시퀀스
        GameEventSystem.Click_ItemSelectDrawer_Event += Itemselectdrawer; //상위 아이템 선택
        GameEventSystem.Click_ItemSellDrawer_Event += Itemselldrawer; //상위 아이템 팔기

        //추출중에 일어나야하는 이벤트 / 추출끝나면 일어나야하는 이벤트
        GameEventSystem.Drawer_PlayAnimation_DeActiveFunc_Event += DeAtiveDrawerEvent;
        GameEventSystem.Drawer_PlayAnimation_ActiveFunc_Event += AtiveDrawerEvent;
    }

    private void AtiveDrawerEvent()
    {
        
    }

    private void DeAtiveDrawerEvent()
    {
        
    }

    private void Itemselldrawer()
    {
        StopCoroutine(ItemselldrawerCorutine());
        StartCoroutine(ItemselldrawerCorutine());
    }


    IEnumerator ItemselldrawerCorutine()
    {
        if (hunterdata == null)
        {
            //선택한 아이템이 없습니다.
            yield break;
        }
        Item newItem = InventoryManager.Instance.GetItem(hunterdata.Name);
        //1,아이템 판매
        int sellamount = HunteritemData.GetRandomValueBetween(newItem.SaleResourceCount1);
        ResourceManager.Instance.Pluse_ResourceType(newItem.SaleResourceType1, sellamount);
        //재화텍스트 업데이트
        GameEventSystem.Send_Resource_Update();

        //팝업창 바뀜 버튼은없어지게
        newitmepopup.HideBtn();
        //ui갱신
        newitmepopup.SellPopupSet(sellamount, newItem.SaleResourceType1);

        SoundManager.Instance.PlayAudio("ItemSell");
        yield return new WaitForSeconds(2f);

        //팝업창 꺼지고 
        newitmepopup.Hide();
        //남은 횟수가 있다면 계속돌기
    }

    private void Itemselectdrawer()
    {
        if (hunterdata == null)
        {
            Debug.Log("상위 아이템이 존재하지 않습니다.");
            return;
        }
        int hunterindex = (int)hunterdata.Class;
        HunterStat stat = DataManager.Instance.Hunters[hunterindex].Orginstat;
        //기존 아이템 판매  + 새로운아이템 교체
        switch (hunterdata.Class)
        {
            case Utill_Enum.SubClass.None:
            case Utill_Enum.SubClass.Archer:
                HunteritemData.BasicItemSellInit(GameDataTable.Instance.HunterItem.Archer, hunterdata , stat);
                EquipmentItemManager.Instance.BindingHunterSlotData(GameDataTable.Instance.HunterItem.Archer);
                break;
            case Utill_Enum.SubClass.Guardian:
                HunteritemData.BasicItemSellInit(GameDataTable.Instance.HunterItem.Guardian, hunterdata , stat);
                EquipmentItemManager.Instance.BindingHunterSlotData(GameDataTable.Instance.HunterItem.Guardian);
                break;
            case Utill_Enum.SubClass.Mage:
                HunteritemData.BasicItemSellInit(GameDataTable.Instance.HunterItem.Mage, hunterdata , stat);
                EquipmentItemManager.Instance.BindingHunterSlotData(GameDataTable.Instance.HunterItem.Mage);
                break;
            default:
                break;
        }

        //ui갱신
        newitmepopup.Hide();
        //재화텍스트 업데이트
        GameEventSystem.Send_Resource_Update();

        itemDrawerview._hunteritemdata = hunterdata;//헌터데이터 바인딩
        itemDrawerview.View_Darwer();
    }

    public int DrawCount;
    private int Itemdrawsequence(int Count, HunteritemData hunterdata)
    {
        StopCoroutine(ItemdrawsequenceCorutin(Count, hunterdata));
        StartCoroutine(ItemdrawsequenceCorutin(Count, hunterdata));

        //횟수를 반환해야함
        return DrawCount;
    }

    //아이템 뽑기 시퀀스 : 데이터 생성 -> 데이터 저장 -> 연출(생략가능) -> 자급
    private IEnumerator ItemdrawsequenceCorutin(int Count, HunteritemData _hunterdata)
    {
        hunterdata = HunteritemData.DeepCopy(_hunterdata); //깊은복사
        string classname = hunterdata.Class.ToString();
        string Gradename = hunterdata.DrawerGrade.ToString();
        string Partname = hunterdata.Part.ToString();
        int currentlevel = 0;
        DrawCount = Count;
        //Count 변수가 0 이될때 까지 
        while (DrawCount > 0)
        {
            //재화 부터 검사 (실시간 재화) 재화가 부족할시에는 리턴해야함
            ItemDrawerTableData Drawerdata = GameDataTable.Instance.ItemDrawerGradeDic[hunterdata.DrawerGrade];
           bool checktpye = ResourceManager.Instance.CheckResource(Drawerdata.DrawerResourceType, Drawerdata.DrawerResourceCount);

            //재화가 안되면 리턴
            if (checktpye == false)
            {
                // 재화가 부족합니다. 남은 횟수 반환
                SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_NeedMoreResource"));
                yield break;
            }
            else
            {
                //재화소모
                ResourceManager.Instance.Minus_ResourceType(Drawerdata.DrawerResourceType , Drawerdata.DrawerResourceCount);

                //재화텍스트 업데이트
                GameEventSystem.Send_Resource_Update();
            }


            Item temp = new Item(); //type 부위와 , grade 등급에 맞춰서 나와야함
            var class_itme = GameDataTable.Instance.EquipmentList[hunterdata.Class];
            foreach (var item in class_itme)
            {
                // class_itme 의 키값중에  classname 와 Gradename 가 함께 포함되어있으면 해당  value 값을 temp저장
                // 클래스 이름과 등급 이름이 포함된 키를 찾기
                if (item.Key.Contains(classname) && item.Key.Contains(Gradename) && item.Key.Contains(Partname))
                {
                    // 조건에 맞는 아이템을 temp에 저장
                    temp = item.Value;
                    //아이템 데이터 파싱
                    hunterdata.Name = temp.GetName;
                    hunterdata.Part = temp.equipmentItem.type;
                    hunterdata.Class = temp.character;
                    hunterdata.ItemGrade = temp.GetGrade;
                    hunterdata.FixedOptionTypes = ChangeStringList(temp.GetOptions);
                    //고정옵 구하는 함수
                    var fixedvalue = InventoryManager.Instance.SetFixedOptionValues(temp);
                    hunterdata.FixedOptionValues = InventoryManager.Instance.ChangeFixedOptionValues(hunterdata.FixedOptionTypes, ChangedoubleList(fixedvalue.Item2), temp.GetName);
                    hunterdata.FixedOptionPersent = ChangedoubleList(fixedvalue.Item2); //고정옵 퍼센트
                    currentlevel = hunterdata.TotalLevel;
                   
                  hunterdata.TotalLevel = HunteritemData.Total_OptionLevel(hunterdata.FixedOptionPersent, temp);//평균레벨
                                                                                                                //렙차구하기
                    leveldifference = hunterdata.TotalLevel - _hunterdata.TotalLevel;
                    hunterdata.isEquip = false;//장착하지않음
                    itemDrawerview._hunteritemdata = hunterdata;
                    break; // 첫 번째 매칭 항목만 사용
                }
            }


            //카운트 감소
            DrawCount--;
            if (DrawCount <= 0)
            {
                itemDrawerview.currentCount = 0;
            }
            else
            {
                itemDrawerview.currentCount = DrawCount;
            }

            if (Count == 1)
            {
                itemDrawerview.currentCount = 1;
            }

            itemDrawerview.Init_CurrentCount();
            //뽑기 연출 작동 연출이 다끝나면 
            // 연출 애니메이션 재생 (구체적인 애니메이션 처리 필요)
            yield return StartCoroutine(PlayGachaAnimation());

            //결과 아이템을 가지고 기존아이템과 비교하여 판매 더 높은 등급만 남김
            // +슬롯데이터 저장
            String DescType = string.Empty;
            int saleamount = 0;
            Utill_Enum.Resource_Type resouretype = Utill_Enum.Resource_Type.None;
            bool ishightopiotn = false;
            switch (hunterdata.Class)
            {
                case Utill_Enum.SubClass.Archer:
                   var data = HunteritemData.CheckSellItem(GameDataTable.Instance.HunterItem.Archer, hunterdata);
                    DescType = data.SellTpye;
                    resouretype = data.SellResourcertpye;
                    saleamount = data.sellamount;
                    ishightopiotn = data.ishighOption;
                    break;
                case Utill_Enum.SubClass.Guardian:
                    var data2 = HunteritemData.CheckSellItem(GameDataTable.Instance.HunterItem.Guardian, hunterdata);
                    DescType = data2.SellTpye;
                    resouretype = data2.SellResourcertpye;
                    saleamount = data2.sellamount;
                    ishightopiotn = data2.ishighOption;
                    break;
                case Utill_Enum.SubClass.Mage:
                    var data3 = HunteritemData.CheckSellItem(GameDataTable.Instance.HunterItem.Mage, hunterdata);
                    DescType = data3.SellTpye;
                    resouretype = data3.SellResourcertpye;
                    saleamount = data3.sellamount;
                    ishightopiotn = data3.ishighOption;
                    break;
                default:
                    break;
            }

            

            if (ishightopiotn) // 상위 옵션이나 교체를 해야할때
            {
                //결과팝업창 켜짐
                ShowResultPopup(temp, true, hunterdata, DescType, resouretype, saleamount , ishightopiotn , leveldifference);
                itemDrawerview._hunteritemdata = _hunterdata;
                yield break;
            }
            else //판매를 해야하는상황
            {
                //결과팝업창 
                ShowResultPopup(temp, true, hunterdata, DescType, resouretype, saleamount , ishightopiotn , leveldifference);
                yield return new WaitForSeconds(ResultItemPopupTime);

                ShowResultPopup(temp, false, hunterdata, DescType, resouretype, saleamount , ishightopiotn , leveldifference);
                //ui 갱신   데이터는 원래쓰던걸로
                itemDrawerview._hunteritemdata = _hunterdata;
                itemDrawerview.View_Darwer();// ui셋팅
                //판매를했으니 판매ui업데이트도같이
                if (Count == 1)
                {
                    itemDrawerview.currentCount = 1;
                }
                ResourceManager.Instance.UpdateReousrce();
            }
        }

        yield break;
    }

    // 가챠 애니메이션 코루틴
    private IEnumerator PlayGachaAnimation()
    {
        // 파티클 실행중 일어나야하는 상황
        PopUpSystem.Instance.CanUndoByESC = false; //애니메이션 나오는 중에는 탭을 끄지 못함
        IsGachaAnimRun = true;
        //추출중 일어나면안되는이벤트
        GameEventSystem.Send_Drawer_PlayAnimation_DeActiveFunc();

        var task = RunParticle();
        yield return new WaitUntil(() => task.IsCompleted); //애니메이션 끝나면?
        IsGachaAnimRun = false;
        PopUpSystem.Instance.CanUndoByESC = true;
        //추출중 일어나면안되는이벤트 원상복구 
        GameEventSystem.Send_Drawer_PlayAnimation_ActiveFunc();
    }


    public Task RunParticle()//뽑기할때 특정 사운드  + 특정 이펙트 
    {
        Task task = null;
        SoundManager.Instance.PlayAudio("ItemDrawer");
        task = RunReforgingParticle();
        return task;
    }


    //강화이펙트
    async Task RunReforgingParticle() 
    {
        int time = (int)(particletime * 1000f);
        GameObject gameObject = ObjectPooler.SpawnFromPool("ItemDrawerEffectObj", ItemDrawerEffectPos);
        gameObject.transform.SetParent(ParticaleParent.transform);
        gameObject.SetActive(true);
        await Task.Delay(time);
        gameObject.SetActive(false);
    }
    // 결과 팝업 창 표시
    private void ShowResultPopup(Item item , bool _acitve , HunteritemData hunterdata , string resourctpye, Utill_Enum.Resource_Type resourcetpye, int resourceamount, bool _ishightopiotn  , int leveldiffrent = -1)
    {
        // 여기서 결과 팝업 창을 띄우는 로직을 구현합니다.
        // 팝업 UI를 활성화하고, 아이템 정보를 표시할 수 있습니다.
        if (_ishightopiotn) //상위옵션이거나 동일 옵션일때
        {
            newitmepopup.ShowBtn(leveldiffrent);
        }
        else
        {
            newitmepopup.HideBtn();
        }

        if(_acitve)
        {
            newitmepopup.Show();
        }
        else
        {
            newitmepopup.Hide();
        }

        newitmepopup.resultMessage(resourctpye, resourcetpye , resourceamount);
        newitmepopup.SettingNewItem(hunterdata);
    }

    // 아이템 슬롯의 타입에따라 해당팝업 켜주기
    public void OpenITemDrawer(int typenumber , HunteritemData CurrentHunterItem)
    {
        switch (typenumber)
        {
            case 0:
                //헌터비활성화
                HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(false);
                
                itemDrawerview.gameObject.SetActive(true); //타입 1 추출기만 셋팅
                itemDrawerview.hunteritem = CurrentHunterItem;
                itemDrawerview.init_drawerCount();//뽑기 갯수도 1번으로
                itemDrawerview.View_Darwer();// ui셋팅
                break;
                case 1:
                //헌터비활성화
                HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(false);

                itemDrawerview.hunteritem = CurrentHunterItem;
                UIManager.Instance.gachaPopUp.Setting(CurrentHunterItem.Part);
                UIManager.Instance.gachaPopUp.Show();
                break;
                case 2:
                break;
            default:
                break;
        }
    }

    public void CloseITemDrawer()
    {
        itemDrawerview.gameObject.SetActive(false); //타입 1 추출기만 셋팅
        HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);
        

    }


    public List<string> ChangeStringList(List<Utill_Enum.Option> list)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < list.Count; i++)
        {
            result.Add(list[i].ToString());
        }

        return result;
    }

    public List<double> ChangedoubleList(List<ObscuredDouble> list)
    {
        List<double> result = new List<double>();

        for (int i = 0; i < list.Count; i++)
        {
            result.Add(list[i]);
        }

        return result;
    }


}
