using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DailyMissionPopUp : MonoBehaviour, IPopUp
{
    [SerializeField]
    private TMP_Text titleText = null;
    [SerializeField]
    private TMP_Text remainTimeText = null;
    [SerializeField]
    private TMP_Text descriptionText = null;
    
    [SerializeField]
    private DailyMissionController controller;

    public int ClickCount = 0;
    public bool isInit = false;

    //위 팝업은 초기화가 필요하나 초기에 꺼져있는 상태로 세팅되어있기 떄문에 awake가 아닌 생성자에서 event를 추가해줌
    private DailyMissionPopUp()
    {
        if (isInit) return;
        GameEventSystem.DailyInit_Event_Event += OnTimeChanged;
        isInit = true;
    }

    private void OnTimeChanged(TimeSpan timeSpan, bool isToday)
    {
        if(isToday) //시간이 바뀌어 하루가 지남
        {
            //현재 보상 획득 여부, 현재 달성도 모두 0으로 초기화함
            for (int i = 0; i < GameDataTable.Instance.User.DailyMissionClaimStatus.Length; i++)
            {
                GameDataTable.Instance.User.DailyMissionClaimStatus[i] = 0;
                GameDataTable.Instance.User.DailyMissionTaskProgress[i] = 0;
                GameEventSystem.Send_DailyMission_UiDraw((Utill_Enum.DailyMissionType)i);
            }
            return;
        }
        remainTimeText.text = Utility.TimeFormatRemain(timeSpan);
    }

    public void Initialize()
    {
        titleText.text = LocalizationTable.Localization("Title_DailyMission");
        descriptionText.text = LocalizationTable.Localization("DailyMissionDescription");
    }
    public void ScrollSetting()
    {
        List<DailyMissionCellData> list = new List<DailyMissionCellData>();
        DailyMissionData data = new DailyMissionData();
        data = DailyMissionData.GetCurrentDailyMissionData(GameDataTable.Instance.DailyMissionList, GameDataTable.Instance.User.ClearStageLevel);
        for(int i = 0; i < data.Missions.Count; i++)
        {
            DailyMissionCellData tmpCellData = new DailyMissionCellData()
            {
                missionType = (Utill_Enum.DailyMissionType)i,
                title = LocalizationTable.Localization(((Utill_Enum.DailyMissionType)i).ToString()),
                needCount = data.Missions[(Utill_Enum.DailyMissionType)i].NeedValue,
                rewardCount = data.Missions[(Utill_Enum.DailyMissionType)i].ResourceCount,
                rewardType = data.Missions[(Utill_Enum.DailyMissionType)i].ResourceType,
                stageLevel = data.StageLevelMax
            };
            list.Add(tmpCellData);
        }
        controller.SetData(list);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        ClickCount = 0;

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
            Initialize();
            ScrollSetting();
        }
        else
        {
            Hide();
        }

    }
}
