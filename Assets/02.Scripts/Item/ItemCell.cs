using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : ItemCell 프리팹 관리 UI 처리                                           
/// </summary>
public class ItemCell : MonoBehaviour
{
    public int cnt = 0;
    public Item item;

    public Image backgroundImg = null;
    public Image itemIconImg = null;
    public TMP_Text equipText = null;

    public bool isOccupied = false; //아이템이 할당되어있는지 확인

    protected Button btn = null;

    private void Start()
    {
        SetAddListener();
    }

    protected virtual void SetAddListener()
    {
        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                if (isOccupied == true)
                {
                    if (UIManager.Instance.clickItemPopType.gameObject.activeSelf == false)
                    {
                        //아이템 자세히 보기 팝업 키기 
                        InventoryPopType.Instance.ShowClickItemPopUp(item, this);

                        InventoryPopType.Instance.createBtn.SetActive(false);
                        InventoryPopType.Instance.deleteBtn.SetActive(false);
                    }

                }
            });
        }
    }

    #region 토글 텍스트 처리
    public void ToggleEquipText(bool toggle)
    {
        equipText.gameObject.SetActive(toggle);
    }
    public void ToggleEquipText()
    {
        equipText.gameObject.SetActive(item.equipmentItem.isEquip);
    }
    #endregion

    // 아이템이 할당되어 있으면 true, 아니면 false 반환 
    public bool IsOccupied()
    {
        return isOccupied != false;
    }
    /// <summary>
    /// 아이템 정보 할당 및 UI 처리
    /// </summary>
    /// <param name="_item"></param>
    public virtual void SetItemInfo(Item _item)
    {
        this.item = _item;

        isOccupied = true;

        #region Image
        backgroundImg.gameObject.SetActive(true);
        itemIconImg.gameObject.SetActive(true);

        Sprite backGroundSprite = Utill_Standard.GetItemSprite(item.GetBackgroundName);
        Sprite itemSprite = Utill_Standard.GetItemSprite(item.GetName);

        // 로드한 이미지를 이미지 컴포넌트에 할당합니다.
        if (backGroundSprite != null)
        {
            backgroundImg.sprite = backGroundSprite;
        }
        else
        {
            Game.Debbug.Debbuger.Debug("Image not found for backGround: " + item.GetBackgroundName);
        }

        if (itemSprite != null)
        {
            itemIconImg.sprite = itemSprite;
        }
        else
        {
            Game.Debbug.Debbuger.Debug("Image not found for item: " + item.GetName);
        }
        #endregion
    }
    /// <summary>
    /// 아이템 삭제 처리
    /// </summary>
    public virtual void DeleteItemInfo()
    {
        Item item = new Item();
        this.item = item;

        isOccupied = false;
        ToggleEquipText(false);

        backgroundImg.sprite = null;
        itemIconImg.sprite = null;

        backgroundImg.gameObject.SetActive(false);
        itemIconImg.gameObject.SetActive(false);
    }
}
