using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour , IPopUp
{
    public int ClickCount = 0;
    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        ClickCount = 0;

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }
}
