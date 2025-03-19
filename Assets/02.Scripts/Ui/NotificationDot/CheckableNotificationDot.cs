using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-30
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 확인 여부 저장하는 레드닷
/// </summary>
public class CheckableNotificationDot : MonoBehaviour, INotificationDot<CheckableNotificationDotType>
{
    //None은 무조건 활성화, 길이가 0이면 무조건 비활성화
    [SerializeField]
    protected List<CheckableNotificationDotType> runTypes = new();

    //현재 켜져있는지
    private Dictionary<CheckableNotificationDotType, bool> activatingEventsDic = new();

    [SerializeField]
    private List<int> limitLevels = new List<int>();
    private Dictionary<CheckableNotificationDotType, int> limitLevelDic = new();

    bool isInit = false;
    //자신이 가지고 있는 Event타입 매니저에 Init
    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;

    }
    private void Start()
    {
        if(!isInit)
        {
            Initialize();
        }
    }
    private bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.Start:
                Initialize();
                return true;
        }
        return false;
    }
    protected virtual void Initialize()
    {
        if (isInit) return;
        isInit = true;

        for (int i = 0; i < runTypes.Count; i++)
        {
            limitLevelDic.Add(runTypes[i], limitLevels[i]);
        }
        if (runTypes.Count > 0)
        {
            HideDot();
            if (runTypes[0] == CheckableNotificationDotType.None)
            {
                ShowDot();
                return;
            }

            NotificationDotManager.Instance.AddDot(runTypes, this);
            for (int i = 0; i < runTypes.Count; i++)
            {
                activatingEventsDic.Add(runTypes[i], false);
                //IsCheckList 전용 초기화 함수 실행
                NotificationDotManager.Instance.ConformIsChecked(runTypes[i], this);
            }
        }
    }

    /// <summary>
    /// Dot 활성화
    /// </summary>
    public virtual void ShowDot(CheckableNotificationDotType runType = CheckableNotificationDotType.None)
    {
        gameObject.SetActive(true);
        if (runType == CheckableNotificationDotType.None)
        {
            return;
        }
        // 일정레벨 미만일 경우 Hide처리
        if (limitLevelDic[runType] > DataManager.Instance.Hunters[0]._UserStat.Level)
        {
            HideDot(runType);
            return;
        }
        activatingEventsDic[runType] = true;
    }
    /// <summary>
    ///  Dot 비활성화
    /// </summary>
    public void HideDot(CheckableNotificationDotType runType = CheckableNotificationDotType.None)
    {
        if (runType == CheckableNotificationDotType.None)
        {
            gameObject.SetActive(false);
            return;
        }
        activatingEventsDic[runType] = false;

        //만약 다른 event의 값이 true라면 켜진상태로 유지
        foreach (var eventType in activatingEventsDic)
        {
            if (eventType.Value)
                return;
        }
        gameObject.SetActive(false);
    }
}
