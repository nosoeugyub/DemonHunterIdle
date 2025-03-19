using NSY;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-30
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 이벤트에 따라 조종되는 레드닷
/// </summary>
public class EventNotificationDot : MonoBehaviour, INotificationDot<EventNotificationDotType>
{
    //None은 무조건 활성화, 길이가 0이면 무조건 비활성화
    [SerializeField]
    protected List<EventNotificationDotType> runTypes = new();

    //현재 켜져있는지
    private Dictionary<EventNotificationDotType, bool> activatingEventsDic = new();

    [SerializeField]
    private List<int> limitLevels = new List<int>();
    private Dictionary<EventNotificationDotType, int> limitLevelDic = new();

    private bool isInit = false;

    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;

    }
    //UI 깊이때문에 Init되지못한 Dot Init
    private void Start()
    {
        if (!isInit)
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

    /// <summary>
    /// 자신이 가지고 있는 Event타입 매니저에 Init
    /// </summary>
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
            HideDot(EventNotificationDotType.None);
            if (runTypes[0] == EventNotificationDotType.None)
            {
                ShowDot(EventNotificationDotType.None);
                return;
            }

            NotificationDotManager.Instance.AddDot(runTypes, this);
            for (int i = 0; i < runTypes.Count; i++)
            {
                activatingEventsDic.Add(runTypes[i], false);
            }
        }
    }

    /// <summary>
    /// Dot 활성화
    /// </summary>
    public void ShowDot(EventNotificationDotType runType)
    {
        gameObject.SetActive(true); 
        if (runType == EventNotificationDotType.None)
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
    public void HideDot(EventNotificationDotType runType)
    {
        if (runType == EventNotificationDotType.None)
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
