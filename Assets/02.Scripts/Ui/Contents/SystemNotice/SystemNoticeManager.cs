using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-30
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 시스템 알림 객체 관리                                                  
/// </summary>
public class SystemNoticeManager : MonoSingleton<SystemNoticeManager>
{
    [SerializeField]
    private GameObject defaultSystemNoticePrefab;
    [SerializeField]
    private GameObject noBackgroundSystemNoticePrefab;

    //최대 알림 수 조절
    private int maxNoticeNum = 0;
    private int currentNoticeNum = 0;

    //알림들 담고있는 리스트
    private List<SystemNotice> defaultSystemNoticeList = new List<SystemNotice>();
    private List<SystemNotice> noBackgroundSystemNoticeList = new List<SystemNotice>();

    private int defaultNoticeIdx = 0;
    private int noBackgroundNoticeIdx = 0;

    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
    }
    #region 기능테스트
    //private void Update() //기능 테스트
    //{
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        //Alert("fdlkdfs\nlsdfk");
    //        Alert("Alert_Test", AlertType.NoBackground);
    //    }

    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        Alert("Alert_Test");
    //        //Alert("testest", AlertType.NoBackground);
    //    }
    //}
    #endregion

    private bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:
                Setting();
                return true;
        }
        return false;
    }

    private void Setting()
    {
        Vector3 tempPos = new Vector3(9999, 9999, 9999); //초기화시 저 멀리 소환했다 보여질때만 가운데로 위치 옮김
        maxNoticeNum = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_SYSTEM_NOTICE].Value;

        for (int i = 0; i < maxNoticeNum; i++)
        {
            SystemNotice defaultAlert = Instantiate(defaultSystemNoticePrefab, tempPos, Quaternion.identity).GetComponent<SystemNotice>();
            //크기 세팅
            defaultAlert.transform.SetParent(transform);
            defaultAlert.transform.localScale = Vector3.one;
            defaultSystemNoticeList.Add(defaultAlert);
        }
        for (int i = 0; i < maxNoticeNum; i++)
        {
            SystemNotice noBackgroundAlert = Instantiate(noBackgroundSystemNoticePrefab, tempPos, Quaternion.identity).GetComponent<SystemNotice>();
            //크기 세팅
            noBackgroundAlert.transform.SetParent(transform);
            noBackgroundAlert.transform.localScale = Vector3.one;
            noBackgroundSystemNoticeList.Add(noBackgroundAlert);
        }
    }

    /// <summary>
    /// 리스트에서 다음 인덱스의 버튼을 순서대로 반환
    /// (보여지는 중이라도 반환함)
    /// </summary>
    private SystemNotice GetNextSystemNotice(SystemNoticeType type)
    {
        SystemNotice tmpAlert = null;
        switch (type)
        {
            case SystemNoticeType.Default:
                tmpAlert = defaultSystemNoticeList[defaultNoticeIdx++];
                if (defaultNoticeIdx == maxNoticeNum) //최대 리스트 갯수를 넘어가면
                    defaultNoticeIdx = 0; //인덱스 초기화
                break;
            case SystemNoticeType.NoBackground:
                tmpAlert = noBackgroundSystemNoticeList[noBackgroundNoticeIdx++];
                if (noBackgroundNoticeIdx == maxNoticeNum) //최대 리스트 갯수를 넘어가면
                    noBackgroundNoticeIdx = 0; //인덱스 초기화
                break;
        }
        return tmpAlert;
    }

    /// <summary>
    /// 최대 알림팝업 수 조절하기 위해 호출
    /// </summary>
    public void ReduceNoticeNum()
    {
        currentNoticeNum--;
    }

    /// <summary>
    /// 알림 팝업 띄움
    /// </summary>
    /// <param name="systemNoticeString">알림의 내용</param>
    public void SystemNotice(string systemNoticeString, SystemNoticeType systemNoticeType = SystemNoticeType.Default)
    {
        //현재 알림이 일정 갯수 이상 떠 있으면 return
        if (currentNoticeNum >= maxNoticeNum)
            return;

        SystemNotice temp = GetNextSystemNotice(systemNoticeType);
        temp.ActiveObject(true);
        temp.SetSystemNotice(systemNoticeString);

        currentNoticeNum++;
    }

    /// <summary>
    /// 알림 팝업 띄움 (에디터용)
    /// </summary>
    /// <param name="systemNoticeString">알림의 내용</param>
    public void SystemNoticeInEditor(string systemNoticeString, SystemNoticeType systemNoticeType = SystemNoticeType.Default)
    {
#if UNITY_EDITOR
        //현재 알림이 일정 갯수 이상 떠 있으면 return
        if (currentNoticeNum >= maxNoticeNum)
            return;
        editorNoticeStr.Add(systemNoticeString);
        editorNoticeType.Add(systemNoticeType);
#endif
    }
    private List<string> editorNoticeStr = new();
    private List<SystemNoticeType> editorNoticeType = new();

    public void Update()
    {
#if UNITY_EDITOR
        if (editorNoticeStr.Count > 0)
        {
            for (int i = 0; i < editorNoticeStr.Count; i++)
            {

                //현재 알림이 일정 갯수 이상 떠 있으면 return
                if (currentNoticeNum >= maxNoticeNum)
                    return;

                SystemNotice temp = GetNextSystemNotice(editorNoticeType[i]);
                temp.ActiveObject(true);
                temp.SetSystemNotice(editorNoticeStr[i]);

                currentNoticeNum++;
            }
            editorNoticeType.Clear();
            editorNoticeStr.Clear();
        }
#endif
    }
}