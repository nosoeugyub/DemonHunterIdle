using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

public class DailyMissionCellView : EnhancedScrollerCellView
{
    [SerializeField]
    private TMP_Text missionTitle;
    [SerializeField]
    private TMP_Text missionText; //현재 달성수 / 필요 달성수 표시하는 텍스트
    [SerializeField]
    private TMP_Text rewardText;

    [SerializeField]
    private Image missionBar; //현재 달성수 / 필요 달성수 표시하는 바
    [SerializeField]
    private Image rewardImage;

    [SerializeField]
    private Button receiveButton;

    private Basic_Prefab receiveButton_Color;

    private DailyMissionCellData cellData;

    private void Awake()
    {
        GameEventSystem.DailyMission_UiDraw_Event += UpdateTotalValue;
        GameEventSystem.StageLevel_UiDraw_Event += OnStageUp;
    }
    private void OnStageUp(int StageLevel)
    {
        var data = DailyMissionData.GetCurrentDailyMissionData(GameDataTable.Instance.DailyMissionList, GameDataTable.Instance.User.ClearStageLevel);
        DailyMissionCellData tmpCellData = new DailyMissionCellData()
        {
            missionType = cellData.missionType,
            title = LocalizationTable.Localization((cellData.missionType).ToString()),
            needCount = data.Missions[cellData.missionType].NeedValue,
            rewardCount = data.Missions[cellData.missionType].ResourceCount,
            rewardType = data.Missions[cellData.missionType].ResourceType,
            stageLevel = data.StageLevelMax
        };
        SetData(tmpCellData);
    }
    private void UpdateTotalValue(DailyMissionType type)
    {
        if (type == cellData.missionType && gameObject.activeInHierarchy)
        {
            int totalCount = GetTotalValue(cellData.missionType);

            missionText.text = $"{Utill_Math.FormatCurrency(totalCount)}/{Utill_Math.FormatCurrency(cellData.needCount)}";

            if (totalCount > 0)
                missionBar.fillAmount = Mathf.Clamp01((float)totalCount / (float)cellData.needCount);
            else
                missionBar.fillAmount = 0; 
            
            if (receiveButton_Color == null)
                receiveButton_Color = receiveButton.GetComponent<Basic_Prefab>();

            if (cellData.needCount <= totalCount && GameDataTable.Instance.User.DailyMissionClaimStatus[(int)cellData.missionType] <= 0)
                receiveButton_Color.SetTypeButton(ButtonType.Active);
            else
                receiveButton_Color.SetTypeButton(ButtonType.DeActive);
        }
    }

    public void SetData(DailyMissionCellData cellData)
    {
        this.cellData = cellData;
        missionTitle.text = cellData.title;

        int totalCount = GetTotalValue(cellData.missionType);

        missionText.text = $"{totalCount}/{cellData.needCount}";

        if (totalCount > 0)
            missionBar.fillAmount = Mathf.Clamp01((float)totalCount / (float)cellData.needCount);
        else
            missionBar.fillAmount = 0;

        rewardText.text = Utill_Math.FormatCurrency(cellData.rewardCount);
        rewardImage.sprite = Utill_Standard.GetItemSprite(cellData.rewardType);

        if (receiveButton_Color == null)
            receiveButton_Color = receiveButton.GetComponent<Basic_Prefab>();

        if (cellData.needCount <= totalCount && GameDataTable.Instance.User.DailyMissionClaimStatus[(int)cellData.missionType] <= 0)
            receiveButton_Color.SetTypeButton(ButtonType.Active);
        else
            receiveButton_Color.SetTypeButton(ButtonType.DeActive);
        ButtonSetting();
    }

    /// <summary>
    /// 셀의 타입에 따라 미션 달성도를 반환함
    /// </summary>
    private int GetTotalValue(DailyMissionType type)
    {
        int temp = GameDataTable.Instance.User.DailyMissionTaskProgress[(int)type];
        return temp;
    }

    private void ButtonSetting()
    {
        receiveButton.onClick.RemoveAllListeners();
        receiveButton.onClick.AddListener(() => SendReward());
    }

    private void SendReward()
    {
        if(GameDataTable.Instance.User.DailyMissionClaimStatus[(int)cellData.missionType] > 0)
        {
            SoundManager.Instance.PlayAudio("UIClick");
            return;
        }
        int totalCount = GetTotalValue(cellData.missionType);
        if (cellData.needCount > totalCount)
        {
            SoundManager.Instance.PlayAudio("UIClick");
            return;
        }
        SoundManager.Instance.PlayAudio("RewardReceived");
        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_SendToMail"));
        GameDataTable.Instance.User.DailyMissionClaimStatus[(int)cellData.missionType]++;
        
        if (cellData.needCount <= totalCount && GameDataTable.Instance.User.DailyMissionClaimStatus[(int)cellData.missionType] <= 0)
            receiveButton_Color.SetTypeButton(ButtonType.Active);
        else
            receiveButton_Color.SetTypeButton(ButtonType.DeActive);
        
        //메일 보내기
        GameManager.Instance.SendRewards(cellData);
        //저장
        StreamingReader.SaveMailData();
    }
}
