using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬및에 하위 버튼들을 관리하는 로직 
/// </summary>
public class SkillSubPopUp : MonoBehaviour , IPopUp
{
    [SerializeField] private SubPopType subpoptype;


    public Image ActiceClickImg;
    public TextMeshProUGUI Tabname;

    int ClickCount = 0;

    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        PopUpSystem.Instance.EscPopupListRemove();

        ClickCount = 0;
        ActiceClickImg.gameObject.SetActive(false);
        gameObject.SetActive(false);
        subpoptype.Hide();
        Tabname.color = Color.gray;
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(PopUpSystem.Instance.CharacterUpgradePopup);
        }

        gameObject.SetActive(true);
        ActiceClickImg.gameObject.SetActive(true);
        Tabname.color = Color.white;
    }
}
