using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-09-12
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 가챠 업적 팝업
/// </summary>
public class GachaDictionaryPopUp : MonoBehaviour,IPopUp
{
    [SerializeField]
    private GachaDictonaryCell dictonaryCellPrefab = null;
    [SerializeField]
    private TextMeshProUGUI titleText = null;
    [SerializeField]
    private Transform dictionaryCellParent;

    private bool isInit = false;
    private int ClickCount = 0;
    private List<GachaDictonaryCell> gachaDictionaryCellList = new();
    private void Initialize()
    {
        if (isInit) return;
        titleText.text = LocalizationTable.Localization("Title_Dictionary");
        for (int i = 0; i < GameDataTable.Instance.ItemGachaDataDic.Count;i++)
        {
            var cell = Instantiate(dictonaryCellPrefab, dictionaryCellParent);
            cell.Setting(GameDataTable.Instance.ItemGachaDataDic[i + 1]);
            gachaDictionaryCellList.Add(cell);
        }
        isInit = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        ClickCount = 0;

        PopUpSystem.Instance.EscPopupListRemove();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
            Initialize();
        }
        else
        {
            Hide();
        }
    }
}
