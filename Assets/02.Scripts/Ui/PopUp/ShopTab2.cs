using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-06-8
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 상점 텝 
/// </summary>
public class ShopTab2 : MonoBehaviour ,IPopUp
{
    public ShopPopUpController_2 shoppopupcontroller_2;
    [SerializeField] private SubPopType subpoptype;
    int ClickCount = 0;

    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        ClickCount = 0;
        gameObject.SetActive(false);
        subpoptype.Hide();
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(PopUpSystem.Instance.ShopPopup);
        }
        gameObject.SetActive(true);
        //shoppopupcontroller_2.SetData();
    }
}
