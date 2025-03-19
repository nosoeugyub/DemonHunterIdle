using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 작성일자   : 2024-09-30
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 소환 팝업에서 합성현황의 셀
/// </summary>
public class GachaMergeItemCell : MonoBehaviour
{
    [SerializeField]
    private ItemCell itemCell;
    [SerializeField]
    private Image mergeItemBar;
    [SerializeField]
    private Image itemCellImage;
    [SerializeField]
    private TextMeshProUGUI equipText;
    [SerializeField]
    private Button infoButton;

    private bool isInit = false;
    private Item item = new();

    private void Initialize()
    {
        if (isInit)
            return;
        isInit = true;
        infoButton.onClick.AddListener(() =>
        {
            UIManager.Instance.gachaItemInfoPopUp.Show();
            UIManager.Instance.gachaItemInfoPopUp.SettingNewItem(UIManager.Instance.gachaPopUp.CurEquipData, item);
        });
    }

    /// <summary>
    /// bool값 따라 버튼 작동 비작동 여부 변경
    /// </summary>
    /// <param name="isOn"></param>
    public void SetButtonInteractable(bool isOn)
    {
        infoButton.interactable = isOn;
    }

    /// <summary>
    /// 아이템을 기반으로 아이템 셀을 표시해줌
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="isGet">이 아이템을 얻은적 있는지</param>
    public void SetItemInfo(Item _item,bool isGet)
    {
        Initialize();
        itemCellImage.color = Color.white;
        itemCell.SetItemInfo(_item);
        item = _item;
        itemCell.equipText.gameObject.SetActive(false);
        if (!isGet)
        {
            //아이템셀 이미지를 물음표 이미지로 변경하는 함수
            itemCellImage.sprite = Utill_Standard.GetUiSprite("QuestionMark");
            itemCellImage.color = Color.black;
        }
    }

    /// <summary>
    /// 보유갯수를 기반으로 합성현황 UI 세팅
    /// </summary>
    /// <param name="curEquipNum">현재 보유 갯수</param>
    /// <param name="needEquipNum">다음 등급으로 넘어가기위한 보유 갯수</param>
    public void SetEuipNum(int curEquipNum,int needEquipNum)
    {
        float fillNum = (float)curEquipNum / needEquipNum;

        mergeItemBar.fillAmount = fillNum;
        equipText.text = $"{curEquipNum}/{needEquipNum}";
    }
}
