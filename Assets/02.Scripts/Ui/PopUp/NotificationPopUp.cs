using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheBackend;
using BackEnd;
using LitJson;


/// </summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 공지사항 팝업시스템
/// </summary>
public class NotificationPopUp : MonoSingleton<NotificationPopUp>, IPopUp
{
    [SerializeField] NotificationPopUpView _notificationpopUpView;
    [SerializeField] Button CloseBtn;
    bool notifi = false;
    string titile = string.Empty;
    string contents = string.Empty;

    private int ClickCount = 0;
    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSequence_SendGameEventHandler += Initialize;
    }

    private bool Initialize(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.InGame:
                _notificationpopUpView.gameObject.SetActive(false);
                CloseBtn.onClick.RemoveListener(Hide);
                CloseBtn.onClick.AddListener(Hide);
                return true;
        }
        return true;
    }

    private void Start()
    {
        if (BackendManager.Instance.IsLocal) return;
        Backend.Notification.Connect();
        Backend.Notification.OnNewNoticeCreated = (string _title, string _content) =>
        {
            notifi = true;
            titile = _title;
            contents = _content;
            Debug.Log("On NewNoticeCreated");
        };
    }

    private void Update()
    {
        if (notifi == true && BackendManager.Instance.IsLocal == false)
        {
            // 팝업 창을 열고 공지 내용을 표시
            _notificationpopUpView.gameObject.SetActive(true);
            _notificationpopUpView.Setting_Noti(titile, contents);
            notifi = false;
        }
    }

    public void GetNoticeAsync()
    {
        //BackendReturnObject BRO = Backend.Notice.NoticeList();

        //if (BRO.IsSuccess())
        //{

        //    JsonData noticeData = BRO.GetReturnValuetoJSON()["rows"][0];

        //    string date = noticeData["postingDate"][0].ToString();
        //    string title = noticeData["title"][0].ToString();
        //    string content = noticeData["content"][0].ToString().Substring(0, 10);

        //    _notificationpopUpView.Setting_Noti(title, content);
        //}
    }


    public void Close()
    {
    }

    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove(); //팝업 종료시 esc리스트에서 제거

        _notificationpopUpView.gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            //뒷배경도 ON.
            PopUpSystem.Instance.EscPopupListAdd(this);//팝업 보여질 시 esc리스트에 추가

            _notificationpopUpView.gameObject.SetActive(true);
            GetNoticeAsync();
        }
        else
        {
            Hide();
        }

    }

}
