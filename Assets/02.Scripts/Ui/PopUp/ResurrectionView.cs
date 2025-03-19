using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-09-09
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 부활 시스템 팝업창
/// </summary>
public class ResurrectionView : MonoBehaviour
{
    public GameObject resurrectionPopup;  // 부활 팝업창
    public Button resurrectionButton;     // 부활 버튼
    public TextMeshProUGUI countdowntileText; // 부활 타이틀 텍스트
    public TextMeshProUGUI countdowntilebtnText; // 부활 버튼 텍스트
    public TextMeshProUGUI countdownText; // 카운트다운 텍스트
    int countdownTime = 5;         // 카운트다운 시간 (초)


    private void Awake()
    {
        GameEventSystem.DailyInit_Event_Event += UserTImeInit;
    }

    private void UserTImeInit(TimeSpan timespan, bool isToday)
    {
        if (isToday == true)
        {
            User.init_ResurrectionTime(GameDataTable.Instance.User);
        }
        
    }

    public Basic_Prefab ResurrenctionBtn; //부활버튼
    private void OnEnable()
    {
        resurrectionButton.onClick.AddListener(delegate { ResurrectionSystem.Instance.ResurrectHunters(); });
        GameEventSystem.GameSleepModeCheck_SendEventHandler += PreventSleepMode; //해당 팝업창 떠 있을 시엔 절전모드 진입 방지
        GameEventSystem.GameSleepModeStop_GameEventHandler_Event(); //만약 절전모드의 경우엔 절전모드 종료되도록
    }

    private void OnDisable()
    {
        resurrectionButton.onClick.RemoveListener(delegate { ResurrectionSystem.Instance.ResurrectHunters(); });
        GameEventSystem.GameSleepModeCheck_SendEventHandler -= PreventSleepMode;
    }

    private bool PreventSleepMode() => false;

    public void HideTimeObj()
    {
        countdownText.gameObject.SetActive(false);
    }
    public void ShowTimeObj()
    {
        countdownText.gameObject.SetActive(true);
    }
    public void SetPopup()
    {
        //타이틀과 부활 버튼 이름 매핑
        countdowntileText.text = "부활";
        countdowntilebtnText.text = "부활";
    }


    public void UpdatePopup(int timeLeft)
    {
        SetPopup();

        countdownTime = timeLeft;

        countdownText.text = countdownTime.ToString();
    }
}
