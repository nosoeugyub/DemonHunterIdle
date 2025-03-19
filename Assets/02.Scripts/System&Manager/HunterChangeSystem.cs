using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 작성일자   : 2024-07-10
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 캐릭터인포팝업 열었을때 초기화 및 좌 우 화살표 헌터 바꾸는거 관리 
/// </summary>

public class HunterChangeSystem : MonoSingleton<HunterChangeSystem>
{
    public UiHunter[] hunterObjs;
    public Button leftBtn;
    public Button rightBtn;
    private void Start()
    {
        leftBtn.onClick.AddListener(LeftCharacter);
        rightBtn.onClick.AddListener(RightCharacter);
    }

    #region 캐릭터 인포팝업 처음 들어갈때 헌터 회전 초기화
    public void ResetRotateValueY()
    {
        hunterObjs[GameDataTable.Instance.User.currentHunter].transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    #endregion

    #region UserData에 currentHunter 인덱스에 해당하는 헌터 SetActive 활성화
    public void ActiveUIHunter()
    {
        for (int i = 0; i < hunterObjs.Length; i++)
        {
            hunterObjs[i].gameObject.SetActive(false);
        }

        if (GameDataTable.Instance.User != null)
        {
            hunterObjs[GameDataTable.Instance.User.currentHunter].gameObject.SetActive(true);
        }
    }
    #endregion

    #region 모든 헌터 SetActive 비활성화
    public void AllDeactiveUIHunter()
    {
        for (int i = 0; i < hunterObjs.Length; i++)
        {
            hunterObjs[i].gameObject.SetActive(false);
        }
    }
    #endregion


    /// <summary>
    /// 왼쪽 화살표 눌렀을때 실행
    /// </summary>
    public void LeftCharacter()
    {
        //UserData currentHunter 마이너스
        GameDataTable.Instance.User.MinusCurrentHunter();
        

        //승급 UI 업데이트
        UIManager.Instance.characterInfoPopup.SetPromotionText();
        
        //구매 버튼 UI 업데이트
        HunterPurchaseSystem.Instance.hunterPurchaseView.SetUIHunterBuy();

        //아이템 UI 업데이트
        EquipmentItemManager.Instance.InitAllCell(hunterObjs[GameDataTable.Instance.User.currentHunter].Model.subClass);
        EquipmentItemManager.Instance.AllSetCell(hunterObjs[GameDataTable.Instance.User.currentHunter].Model.subClass);
        //EquipmentItemManager.Instance.SettingCellLock();

        //장비 데이터도 업데이트 
        EquipmentItemManager.Instance.SettingCellList();

        ActiveUIHunter();

        
        PopUpSystem.Instance.CharacterInfoPopup.SetDPS(); //dps 셋팅
    }

    /// <summary>
    /// 오른쪽 화살표 눌렀을때 실행
    /// </summary>
    public void RightCharacter()
    {
        //UserData currentHunter 플러스
        GameDataTable.Instance.User.PlusCurrentHunter();

        
        //승급 UI 업데이트
        UIManager.Instance.characterInfoPopup.SetPromotionText();

        //구매 버튼 UI 업데이트
        HunterPurchaseSystem.Instance.hunterPurchaseView.SetUIHunterBuy();

        //아이템 UI 업데이트
        EquipmentItemManager.Instance.InitAllCell(hunterObjs[GameDataTable.Instance.User.currentHunter].Model.subClass);
        EquipmentItemManager.Instance.AllSetCell(hunterObjs[GameDataTable.Instance.User.currentHunter].Model.subClass);
        //EquipmentItemManager.Instance.SettingCellLock();


        //장비 데이터도 업데이트 
        EquipmentItemManager.Instance.SettingCellList();


        ActiveUIHunter();

        PopUpSystem.Instance.CharacterInfoPopup.SetDPS();
    }


    /// <summary>
    /// ConstranitsData 헌터 오픈 넘버에 맞춰 currentHunter 조절
    /// </summary>
    public void SetHunterOpenNumber()
    {
        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_OPEN_NUMBER].Value;

        if (value == 1)
        {
            leftBtn.gameObject.SetActive(false);
            rightBtn.gameObject.SetActive(false);

            GameDataTable.Instance.User.InitCurrentUserHunter();
        }


        if (value > 1)
        {
            if (GameDataTable.Instance.User.currentHunter == value)
            {
                GameDataTable.Instance.User.InitCurrentUserHunter();
            }
        }
    }
}
