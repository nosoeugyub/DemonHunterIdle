using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;
using UnityEngine.UI;
using NSY;
/// </summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 길잡이 팝업 시스템
/// </summary>
public class GuidePopUp : MonoBehaviour, IPopUp
{
    private int ClickCount = 0;

    public GuidePopUpView guidepopupview;
    public Button GuideBtn;
    public Button GuideCloseBtn;

    private void Awake()
    {
        GuideBtn.onClick.AddListener(delegate { Show(); });
        GuideCloseBtn.onClick.AddListener(delegate { Hide(); });
    }
    public void Close()
    {
    }
    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove(); //팝업 종료시 esc리스트에서 제거
        SoundManager.Instance.PlayAudio("UIClick");
        for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);
        }
        guidepopupview.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            //뒷배경도 ON.
            PopUpSystem.Instance.EscPopupListAdd(this);//팝업 보여질 시 esc리스트에 추가
            gameObject.SetActive(true);
            SoundManager.Instance.PlayAudio("UIClick");
            NotificationDotManager.Instance.SetIsChecked(Utill_Enum.CheckableNotificationDotType.Guide, true);//레드닷없엠
            for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
            {
                HunterChangeSystem.Instance.hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(false);
            }
            guidepopupview.gameObject.SetActive(true);
            guidepopupview.SettingGuidData();
        }
        else
        {
            Hide();
        }

    }

}
