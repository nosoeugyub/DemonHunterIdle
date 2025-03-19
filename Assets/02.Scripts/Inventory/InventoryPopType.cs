using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 인벤토리 팝업 UI 관리                                           
/// </summary> 
public class InventoryPopType : MonoSingleton<InventoryPopType>, IPopUp
{
    public GameObject createBtn = null; //아이템 생성 버튼
    public GameObject deleteBtn = null;  //아이템 삭제 버튼

    public ClickItemPopType clickItemPopType = null; //자세히 보기 팝업
    public int ClickCount = 0;

    public Item item;
    public ItemCell cell;

    public void Close()
    {
    }
    /// <summary>
    /// 아이템 자세히 보기 팝업 켰을때 아이템 데이터 넘겨주기 
    /// </summary>
    public void ShowClickItemPopUp(Item _item,ItemCell itemCell)
    {
        item = _item;
        cell = itemCell;
        clickItemPopType.Init(item);
        clickItemPopType.Show();
    }

    public void Hide()
    {
        if (UIManager.Instance.clickItemPopType.gameObject.activeSelf)
        {
            UIManager.Instance.clickItemPopType.Hide();
        }

        PopUpSystem.Instance.EscPopupListRemove();

        HunterChangeSystem.Instance.ActiveUIHunter();

        ClickCount = 0;

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            InventoryManager.Instance.InitItemCell();
            InventoryManager.Instance.SettingItemCell();
            InventoryManager.Instance.UpdateItemCell();

            PopUpSystem.Instance.EscPopupListAdd(this);

            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }
    }

}
