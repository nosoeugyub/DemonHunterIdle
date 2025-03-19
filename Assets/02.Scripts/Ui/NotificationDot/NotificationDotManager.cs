using NSY;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-30
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 레드닷 시스템 관리                                                
/// </summary>
public class NotificationDotManager : MonoSingleton<NotificationDotManager>
{
    Dictionary<EventNotificationDotType, List<EventNotificationDot>> eventNotificationDic = new();
    Dictionary<CheckableNotificationDotType, List<CheckableNotificationDot>> checkableNotificationDic = new();

    private List<bool> isCheckList = new();
    //CheckableDot 확인 여부 세팅 위한 딕셔너리
    public Dictionary<CheckableNotificationDotType, List<CheckableNotificationDot>> ischeckDotDic = new();

    /// <summary>
    /// 확인 여부 저장한 리스트 (Enum 순)
    /// </summary>
    public List<bool> IscheckList
    {
        get { return isCheckList; }
        set { isCheckList = value; }
    }
    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
    }
    private bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.Start:

                foreach (var checkDot in ischeckDotDic)
                {
                    for (int i = 0; i < checkDot.Value.Count; i++)
                        ConformIsChecked(checkDot.Key, checkDot.Value[i]);
                }
                ischeckDotDic.Clear();
                return true;
        }
        return false;
    }

    #region Event Notification Dot
    /// <summary>
    /// Init event dot 
    /// </summary>
    public void AddDot(List<EventNotificationDotType> types, EventNotificationDot dot)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (eventNotificationDic.ContainsKey(types[i]))
            {
                eventNotificationDic[types[i]].Add(dot);
            }
            else
            {
                List<EventNotificationDot> tmpList = new() { dot };
                eventNotificationDic.Add(types[i], tmpList);
            }
        }
    }
    /// <summary>
    /// 해당 이벤트 상황의 레드닷 표시
    /// </summary>
    public void ShowNotificationDot(EventNotificationDotType type)
    {
        if (!eventNotificationDic.ContainsKey(type))
        {
            //Debug.LogError("해당 이벤트가 딕셔너리에 존재하지 않습니다.");
            return;
        }

        for (int i = 0; i < eventNotificationDic[type].Count; i++)
        {
            eventNotificationDic[type][i].ShowDot(type);
        }
    }

    /// <summary>
    /// 해당 이벤트 상황의 레드닷 숨기기
    /// </summary>
    public void HideNotificationDot(EventNotificationDotType type)
    {
        if (!eventNotificationDic.ContainsKey(type))
        {
            return;
        }

        for (int i = 0; i < eventNotificationDic[type].Count; i++)
        {
            eventNotificationDic[type][i].HideDot(type);
        }
    }
    #endregion

    #region Checkable Notification Dot
    /// <summary>
    /// Init checkable dot 
    /// </summary>
    public void AddDot(List<CheckableNotificationDotType> types, CheckableNotificationDot dot)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (checkableNotificationDic.ContainsKey(types[i]))
            {
                checkableNotificationDic[types[i]].Add(dot);
            }
            else
            {
                List<CheckableNotificationDot> tmpList = new() { dot };
                checkableNotificationDic.Add(types[i], tmpList);
            }
        }
    }

    /// <summary>
     /// 해당 이벤트 상황의 레드닷 표시
     /// </summary>
    public void ShowNotificationDot(CheckableNotificationDotType type)
    {
        if (!checkableNotificationDic.ContainsKey(type))
        {
            return;
        }

        for (int i = 0; i < checkableNotificationDic[type].Count; i++)
        {
            checkableNotificationDic[type][i].ShowDot(type);
        }
    }

    /// <summary>
    /// 해당 이벤트 상황의 레드닷 숨기기
    /// </summary>
    public void HideNotificationDot(CheckableNotificationDotType type)
    {
        if (!checkableNotificationDic.ContainsKey(type))
        {
            return;
        }

        for (int i = 0; i < checkableNotificationDic[type].Count; i++)
        {
            checkableNotificationDic[type][i].HideDot(type);
        }
    }

    /// <summary>
    /// Checkable Dot이 켜져있는지 여부를 반환
    /// </summary>
    public bool GetIsChecked(CheckableNotificationDotType type)
    {
        int index = ((int)type);
        if (index < 0 || isCheckList.Count <= index)
            return false;
        return isCheckList[index];
    }

    /// <summary>
    /// Checkable Dot의 켜짐/꺼짐 여부를 수정 (저장 데이터도 수정됨)
    /// </summary>
    public void SetIsChecked(CheckableNotificationDotType type, bool isChecked)
    {
        int index = ((int)type);
        //Debug.Log($"[setischecked] isCheckList {isCheckList.Count}/ (ischeckDotDic.ContainsKey(type){(ischeckDotDic.ContainsKey(type))}/ type {type}/ tmp {tmp}/ (int)type {(int)type}/ index {index}/ dic count {ischeckDotDic.Count}");
        if (index < 0 || isCheckList.Count <= index)
            return;
        isCheckList[index] = isChecked;
        //DataManager.Instance.User.notidotCheckArr[index] = isChecked;
        if (isChecked)
            HideNotificationDot(type);
        else
            ShowNotificationDot(type);
    }
    /// <summary>
    /// IsCheckList 관련 레드닷 초기화
    /// </summary>
    public void ConformIsChecked(CheckableNotificationDotType type, CheckableNotificationDot dot)
    {
        if (isCheckList.Count == 0) //리스트 초기화 전이라면
        {
            //나중에 초기화 시키도록 예약
            if (ischeckDotDic.ContainsKey(type))
            {
                ischeckDotDic[type].Add(dot);
            }
            else
            {
                List<CheckableNotificationDot> tmpList = new() { dot };
                ischeckDotDic.Add(type, tmpList);
            }
            return;
        }
        if (isCheckList[(int)type])
        {
            dot.HideDot(type);
        }
        else
        {
            dot.ShowDot(type);
        }
    }
    #endregion

}
