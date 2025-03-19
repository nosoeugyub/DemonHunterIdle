using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using UnityEngine;
using System.Threading.Tasks;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 데이터 관련 컨트롤러
/// </summary>
public class ItemDrawerPopup : MonoSingleton<ItemDrawerPopup>
{
    public ItemDrawerPopupView ItemDrawerPopupView;

    [Header("진화 파티클 시간")]
    float particletime = 2.5f;

    public bool condition1;
    public bool condition2;
    public bool condition3;


    public GameObject ParticaleParent;
    private void Awake()
    {
        GameEventSystem.Click_UpgradeBtnDrawer_Event += OnClickItemDrawerBtn;
    }

    IEnumerator OnClickItemDrawerBtnCorutine(HunteritemData data)
    {
        ItemDrawerTableData Drawerdata = GameDataTable.Instance.ItemDrawerGradeDic[data.DrawerGrade];
        int currenGradeLevel = (int)data.DrawerGrade;
        var maxiamgradelevel = GameDataTable.Instance.ConstranitsDataDic[Tag.MAXIAM_ITEMDRAWER_GRADE];
        if (currenGradeLevel >= maxiamgradelevel.Value)//최대 등급일때
        {
            //진화 버튼 비활성화 처리 및 진화조건 셋팅
            ItemDrawerPopupView.MaximumUiSet();
            //최대레벨입니다 표기 - 안함으로 기획변경
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_maimnumlevel"), SystemNoticeType.Default);
            yield break;
        }
        else
        {
            //최대레벨이아닌경우 일반 ui 셋팅
            ItemDrawerPopupView.NotMaxmuim();
        }

        if (CheatManager.Instance.IgnoreDrawerEVCondition == false)//치트일떄는 일단 강화 가능 
        {
            //재화 소모 판단
            bool iscurreny = ResourceManager.Instance.CheckResource(Drawerdata.EvolutionResourceType, Drawerdata.DrawerResourceCount);
            if (!iscurreny)
            {
                //재화가 없음 알림표기하렬면 여기서
                SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_NeedMoreResource"));
                yield break;
            }
            //재화가있더라고 조건이안된다면 안됨
            if (condition1 == false || condition2 == false || condition3 == false )
            {
                //조건이 안됨 알림표기하렬면 여기서
                SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_cannotevloution"), SystemNoticeType.Default);
                yield break;
            }
        }
        

        //성공 실패 결과 도출 
        bool issucces = Utill_Math.Attempt(Drawerdata.DrawerProb);


        if (issucces) //진화 성공시 데이터 변환 및 저장
        {
            //성공사운드
            SoundManager.Instance.PlayAudio("DrawerEVSucceed");
            //다음등급으로변환
            Utill_Enum.DrawerGrade nextgrade = Utill_Standard.GetNextDrawerGrade(data.DrawerGrade);
            data.DrawerGrade = nextgrade;
            //해당 itemdrawerview의 모루도 변환
            ItemDrawer.Instance.itemDrawerview.View_Darwer();

            //저장
            switch (data.Class)
            {
                case Utill_Enum.SubClass.Archer:
                    HunteritemData.Save_Slot(GameDataTable.Instance.HunterItem.Archer, data);
                    break;
                case Utill_Enum.SubClass.Guardian:
                    HunteritemData.Save_Slot(GameDataTable.Instance.HunterItem.Guardian, data);
                    break;
                case Utill_Enum.SubClass.Mage:
                    HunteritemData.Save_Slot(GameDataTable.Instance.HunterItem.Mage, data);
                    break;
            }
        }
        else // 실패했을때 로직...
        {
            SoundManager.Instance.PlayAudio("DrawerEVFail");
        }
        //뽑기 연출 작동 연출이 다끝나면 
        // 연출 애니메이션 재생 (구체적인 애니메이션 처리 필요)
        yield return StartCoroutine(PlayGachaAnimation());


        //바뀐 데이터를 기반으로 ui 갱신
        ShowVIew(data);

    }

    private void OnClickItemDrawerBtn(HunteritemData data)
    {
        StopCoroutine(OnClickItemDrawerBtnCorutine(data));
        StartCoroutine(OnClickItemDrawerBtnCorutine(data));
    }



    public void ShowVIew(HunteritemData _hunteritemdata) //현재 슬롯의 데이터 가져옴
    {
        //최대 등급일시 조건 처리
        int currenGradeLevel = (int)_hunteritemdata.DrawerGrade;
        var maxiamgradelevel = GameDataTable.Instance.ConstranitsDataDic[Tag.MAXIAM_ITEMDRAWER_GRADE];
        if (currenGradeLevel >= maxiamgradelevel.Value)//최대 등급일때
        {
            //진화 버튼 비활성화 처리 및 진화조건 셋팅
            ItemDrawerPopupView.MaximumUiSet();
            //최대레벨입니다 표기 기획상 안되는걸로 바꿈
            //SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_maimnumlevel"), SystemNoticeType.Default);
        }
        else
        {
            //최대레벨이아닌경우 일반 ui 셋팅
            ItemDrawerPopupView.NotMaxmuim();
        }

        //조건 검사 로직
        switch (_hunteritemdata.Class)
        {
            case Utill_Enum.SubClass.Archer:
               var data =  ItemDrawerTableData.CheckEvolutionDrawer(GameDataTable.Instance.HunterItem.Archer, _hunteritemdata);
                condition1 = data.isCurrentEquipmentlvel;
                condition2 = data.isTotalEquipmentLevl;
                condition3 = data.isTotalEquipmentGrade;
                break;
            case Utill_Enum.SubClass.Guardian:
                var data1 = ItemDrawerTableData.CheckEvolutionDrawer(GameDataTable.Instance.HunterItem.Archer, _hunteritemdata);
                condition1 = data1.isCurrentEquipmentlvel;
                condition2 = data1.isTotalEquipmentLevl;
                condition3 = data1.isTotalEquipmentGrade;
                break;
            case Utill_Enum.SubClass.Mage:
                var data2 = ItemDrawerTableData.CheckEvolutionDrawer(GameDataTable.Instance.HunterItem.Archer, _hunteritemdata);
                condition1 = data2.isCurrentEquipmentlvel;
                condition2 = data2.isTotalEquipmentLevl;
                condition3 = data2.isTotalEquipmentGrade;
                break;
            default:
                break;
        }


        ItemDrawerPopupView.Show();
        //데이터 할당
        ItemDrawerPopupView.SetDarer(condition1, condition2, condition3 , _hunteritemdata);
    }

    public void HideView()
    {
        ItemDrawerPopupView.Hide();
    }

    private IEnumerator PlayGachaAnimation()
    {
        //SoundManager.Instance.PlayAudio("ReforgeAttempt");
        // 파티클 실행
        var task = RunParticle();
        yield return new WaitUntil(() => task.IsCompleted); //애니메이션 끝나면?

        Debug.Log("뽑기완료");
    }

    public Task RunParticle()
    {
        Task task = null;

        task = RunReforgingParticle();
        return task;
    }

    //진화 이펙트
    async Task RunReforgingParticle()
    {
        int time = (int)(particletime * 1000f);
        GameObject gameObject = ObjectPooler.SpawnFromPool("ItemDrawerEffectObj", Utill_Standard.Vector3Zero);
        gameObject.transform.SetParent(ParticaleParent.transform);
        gameObject.SetActive(true);
        await Task.Delay(time);
        gameObject.SetActive(false);
    }
}
