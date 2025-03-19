using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-09-23
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 절전모드 팝업을 제어
/// </summary>
public class SleepModePopUp : MonoBehaviour, IPopUp
{
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private TextMeshProUGUI stageText;
    [SerializeField]
    private TextMeshProUGUI infoText;
    [SerializeField]
    private TextMeshProUGUI resourceText;
    [SerializeField]
    private TextMeshProUGUI expText;
    [SerializeField]
    private Image resourceIcon;
    [SerializeField]
    private Image expIcon;
    [SerializeField]
    private Button exitBtn;

    public float doubleClickTime = 0.3f; // 더블 클릭으로 인식할 시간 간격
    private float lastClickTime = 0f;

    private int ClickCount = 0; //팝업 클릭 카운트
    private bool isInit = false;

    //exp, 재화 표기 포멧 스트링
    //세미콜론 기준으로 양수/음수/0일 때의 표기를 지정함
    private const string resourceFomatStr = "{0:#,###;#,###;0}";

    private void Initialize()
    {
        if (isInit) return;
        isInit = true;
        exitBtn.onClick.AddListener(CheckDoubleClick);
        infoText.text = LocalizationTable.Localization("SleepModeInfo");
    }

    private void CheckDoubleClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        FrameRateManager.Instance.RequestFullFrameRate();
        if (timeSinceLastClick <= doubleClickTime)
        {
            OnDoubleClick();
        }

        lastClickTime = Time.time;
    }

    private void OnDoubleClick()
    {
        GameEventSystem.GameSleepModeStop_GameEventHandler_Event();
    }

    public void Close()
    {
    }

    public void Hide()
    {
        ClickCount = 0;

        GameEventSystem.StageLevel_UiDraw_Event -= UpdateStageCount;
        GameEventSystem.GamePluseGold_GameEventHandler_Event -= UpdateResourceText;
        GameEventSystem.GameAddExp_SendGameEventHandler -= UpdateExpText;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            gameObject.SetActive(true);
            Initialize();
            StartCoroutine(UpdateSleepTime());

            GameEventSystem.StageLevel_UiDraw_Event += UpdateStageCount;
            GameEventSystem.GamePluseGold_GameEventHandler_Event += UpdateResourceText;
            GameEventSystem.GameAddExp_SendGameEventHandler += UpdateExpText;

            UpdateStageCount(GameDataTable.Instance.User.ClearStageLevel);
            UpdateResourceText(Utill_Enum.Resource_Type.Gold, 0);
            UpdateExpText(0);

            resourceIcon.sprite = Utill_Standard.GetItemSprite("Gold");
            expIcon.sprite = Utill_Standard.GetItemSprite("Exp");
        }
        else
        {
            Hide();
        }

    }

    /// <summary>
    /// 재화 텍스트 업데이트
    /// </summary>
    private void UpdateResourceText(Utill_Enum.Resource_Type Resourcetype, int Value = 0)
    {
        if (SleepModeManager.Instance.earnResource.ContainsKey(Resourcetype))
            SleepModeManager.Instance.earnResource[Resourcetype] += Value;
        else
            SleepModeManager.Instance.earnResource.Add(Resourcetype, Value);

        resourceText.text = string.Format(resourceFomatStr, Utill_Math.FormatCurrency(SleepModeManager.Instance.earnResource[Resourcetype]));

    }

    /// <summary>
    /// 재화 텍스트 업데이트
    /// </summary>
    private void UpdateExpText(float Value = 0)
    {
        SleepModeManager.Instance.EarnExp += (int)Value;

        expText.text = string.Format(resourceFomatStr, Utill_Math.FormatCurrency(SleepModeManager.Instance.EarnExp));

    }


    /// <summary>
    /// 절전모드 시간 텍스트 업데이트
    /// </summary>
    private IEnumerator UpdateSleepTime()
    {
        while (SleepModeManager.Instance.IsSleepMode)
        {
            timeText.text = Utility.TimeFormatDefault(DateTime.Now - SleepModeManager.Instance.sleepModeEnterTime);
            yield return Utill_Standard.WaitTimeOne;
        }
    }

    public void UpdateStageCount(int stage)
    {
        var UserStage2 = Utill_Math.CalculateStageAndRound(stage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex2 = UserStage2.stageindex;
        int userStage2 = UserStage2.CurrentStage;
        int userRound2 = UserStage2.CurrentRound;
        int chapter2 = GameDataTable.Instance.StageTableDic[userindex2].ChapterZone;

        string ConvertString = chapter2 + "-" + userStage2 + "-" + userRound2;
        stageText.text = LocalizationTable.Localization("Stage") + ConvertString;
    }
}
