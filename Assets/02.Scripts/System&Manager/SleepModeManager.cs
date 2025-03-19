using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-23
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 절전모드의 활성화/비활성화 상태와 동작을 제어함
/// </summary>
public class SleepModeManager : MonoSingleton<SleepModeManager>
{
    [SerializeField]
    private Camera sleepModeCamera;

    private List<Camera> camList = new(); //슬립모드 아닐 시 사용하는 카메라 리스트
    private int mainCamIndex = 0;

    private float sleepTime = 0.0f;
    private bool isSleepMode = false;
    public bool IsSleepMode => isSleepMode; //절전모드 여부


    public DateTime sleepModeEnterTime; //절전모드 활성화 된 시각
    public Dictionary<Resource_Type, int> earnResource = new(); //추후 다양한 아이템 파밍될 경우 고려해 딕셔너리로 제작함
    private int earnExp = 0; //절전모드 된 이후로 얻은 경험치
    public int EarnExp { get => earnExp; set { earnExp = value; } }


    private int lastRequestedFrame = 0;

    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSleepModeStop_SendEventHandler += ExitSleepMode;

        //0은 none이라 1부터
        for (int i = 1; i < (int)Resource_Type.Dia; i++)
            earnResource.Add((Resource_Type)i, 0);
        camList.AddRange(FindObjectsOfType<Camera>());
    }

    private void Update()
    {
        if (!isSleepMode)
        {
            EnterSleepModeCheck(); //절전모드 진입 감지
        }
    }

    private void EnterSleepModeCheck()
    {
        // 입력 체크
        if (Input.touchCount > 0 || Input.anyKeyDown)
        {
            sleepTime = 0.0f;
        }
        else
        {
            sleepTime += Time.deltaTime;

            if (sleepTime >= GameManager.Instance.EnterSleepModeTime && GameEventSystem.GameSleepModeCheck_GameEventHandler_Event())
            {
                // 절전모드 진입
                EnterSleepMode();
            }
        }
    }

    /// <summary>
    /// 절전모드 시작
    /// </summary>
    private void EnterSleepMode()
    {
        if (isSleepMode) return;
        isSleepMode = true;
        sleepModeEnterTime = DateTime.Now;

        FrameRateManager.Instance.StartLimitFrame();

        for (int i = 0; i < camList.Count; i++) //사용중이던 카메라 비활성화
        {
            camList[i].enabled = false;
            camList[i].gameObject.SetActive(false);
            if (camList[i].tag == Tag.MainCamera)
            {
                mainCamIndex = i;
                camList[i].tag = Tag.Untagged;
            }
        }
        //절전모드 카메라 활성화
        sleepModeCamera.tag = Tag.MainCamera;
        sleepModeCamera.enabled = true;
        sleepModeCamera.gameObject.SetActive(true);
        UIManager.Instance.settingPopUp.MuteBGM();
        UIManager.Instance.settingPopUp.MuteFx();
        UIManager.Instance.sleepModePopUp.Show();
    }

    /// <summary>
    /// 절전모드 종료
    /// </summary>
    private void ExitSleepMode()
    {
        if (!isSleepMode) return;
        isSleepMode = false;
        sleepTime = 0;

        for (int i = 0; i < earnResource.Count; i++)
            earnResource[(Resource_Type)i] = 0;
        earnExp = 0;

        FrameRateManager.Instance.EndLimitFrame();

        for (int i = 0; i < camList.Count; i++) //사용중이던 카메라 활성화
        {
            camList[i].enabled = true;
            camList[i].gameObject.SetActive(true);
            if(mainCamIndex == i)
            {
                camList[i].tag = Tag.MainCamera;
            }
        }

        //절전모드 카메라 비활성화
        sleepModeCamera.tag = Tag.Untagged;
        sleepModeCamera.enabled = false;
        sleepModeCamera.gameObject.SetActive(false);
        UIManager.Instance.settingPopUp.UnMuteBGM();
        UIManager.Instance.settingPopUp.UnMuteFx();
        UIManager.Instance.sleepModePopUp.Hide();
    }
}
