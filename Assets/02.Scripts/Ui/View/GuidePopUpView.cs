using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



/// </summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 길잡이 view 관련 스크립트
/// </summary>
public class GuidePopUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Guide_DescText;

    public void SettingGuidData()
    {
        Title.text = LocalizationTable.Localization("Title_Guide");
        Guide_DescText.text = LocalizationTable.Localization("Guideinfo_Test");
    }

}
