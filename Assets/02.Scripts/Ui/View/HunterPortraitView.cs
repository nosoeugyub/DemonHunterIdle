using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 헌터 초상화 전체 ui 처리
/// </summary> 
public class HunterPortraitView : MonoBehaviour
{
    public HunterPortrait[] hunterPortraits;
    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += GameSequence;
    }

    private bool GameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.Start:
                ActiveHunterPortraits();
                return true;
        }
        return true;
    }

    /// <summary>
    /// 장착했을때 실행
    /// </summary>
    public void Equip()
    {
        GameDataTable.Instance.User.CurrentEquipHunter.Clear();

        for (int i = 0; i < hunterPortraits.Length; i++)
        {
            if (hunterPortraits[i].isEquip)
            {
                hunterPortraits[i].equipImage.gameObject.SetActive(false);
                hunterPortraits[i].activeImage.gameObject.SetActive(true);
                GameDataTable.Instance.User.CurrentEquipHunter.Add(hunterPortraits[i].characterType);
                Debug.Log(hunterPortraits[i].characterType);
            }
            else
            {
                hunterPortraits[i].equipImage.gameObject.SetActive(true);
                hunterPortraits[i].activeImage.gameObject.SetActive(false);
            }
        }
        Debug.Log(GameDataTable.Instance.User.CurrentEquipHunter.Count);
    }

    /// <summary>
    /// HUNTER_OPEN_NUMBER에 맞춰서 헌터 초상화 셋엑티브 설정
    /// </summary>
    public void ActiveHunterPortraits()
    {
        int value = HunterPortraitSystem.Instance.GetHunterDeployment();
        for (int i = 0; i < hunterPortraits.Length; i++)
        {
            if (i < value)
            {
                hunterPortraits[i].gameObject.SetActive(true);
            }
            else
            {
                hunterPortraits[i].gameObject.SetActive(false);
            }
        }
        SetHunterPortraitsCell();
    }
  

    /// <summary>
    /// 서버나 로컬에 저장된 값으로 헌터 초상화 셀 UI 설정 
    /// </summary>
    public void SetHunterPortraitsCell()
    {
        for (int i = 0; i < hunterPortraits.Length; i++)
        {
            hunterPortraits[i].InitBuyCell(GameDataTable.Instance.User.HunterPurchase[i]);
        }
    }
}
