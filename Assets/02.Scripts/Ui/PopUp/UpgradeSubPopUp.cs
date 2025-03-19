using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeSubPopUp : MonoBehaviour, IPopUp
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
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove();

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
