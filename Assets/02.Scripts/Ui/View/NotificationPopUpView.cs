using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 공지사항 팝업시스템(ui처리만 하는곳)
/// </summary>
public class NotificationPopUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Noti_Text;


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Setting_Noti(string title, string desc)
    {
        Title.text = LocalizationTable.Localization("Title_Notification");

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(title);
        stringBuilder.AppendLine();
        stringBuilder.Append(desc);
        Noti_Text.text = stringBuilder.ToString();
    }
}
