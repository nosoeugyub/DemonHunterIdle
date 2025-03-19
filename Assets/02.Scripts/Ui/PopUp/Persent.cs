using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persent : MonoBehaviour , IPopUp
{
    public int ClickCount = 0;
    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove();

        //뒷배경도 Off.
        //UIManager.Instance.ActiveBackBardBlack(false);

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            //뒷배경도 ON.
            PopUpSystem.Instance.EscPopupListAdd(this);

           // UIManager.Instance.ActiveBackBardBlack(true);

            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
