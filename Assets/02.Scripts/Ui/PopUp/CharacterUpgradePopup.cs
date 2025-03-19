using Game.Debbug;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 캐릭터 업그레이드
/// /// </summary>
public class CharacterUpgradePopup : MonoBehaviour, IPopUp
{
    [SerializeField] private int ClickStack = 0; //가장 바깥쪽 카테고리의 버튼 클릭스텍
    [SerializeField] private int InfoClickIndex = 0; //카테고리의 세부 버튼의 인덱스

    //Image
    public Image ButtonImg;
    


    //button
    public Button[] Upgradeinbtnarray; //가장바깥의버튼
    public Button[] Categorybtnarray; // 업그레이드팝업의 서브버튼
    
    //text
    public TextMeshProUGUI[] Upgradeintextarray;

    //Rect
    public RectTransform[] CategorySubbtnarray; //서브버튼을눌렀을대의 팝업


    public int CurrentSubCategoryindex = -1;

    private void OnEnable()
    {
        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_OPEN_NUMBER].Value;

        for (int i = 0; i < value; i++)
        {
            Upgradeinbtnarray[i].gameObject.SetActive(true);
        }

        // 세부 카테고리 버튼 이벤트 할당
        for (int i = 0; i < Upgradeinbtnarray.Length; i++)
        {
            int index = i; // 반복문 변수의 값을 캡처하기 위해 변수를 생성합니다.
            Upgradeinbtnarray[i].onClick.AddListener(() => ActiveSubCategory(index));
        }

    }

        public void init_CharaterUpgradePopup()
        {
        CurrentSubCategoryindex = -1;
        }

    public void Close()
    {
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        PopUpSystem.Instance.EscPopupListRemove();

        ClickStack = 0;
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
        ButtonImg.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        //배틀유아이 이동
        PopUpSystem.Instance.MoveBattleCanvs(0);
    }

    public void Show()
    {

        ClickStack++;
        if (ClickStack == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            //배틀유아이 이동
            PopUpSystem.Instance.MoveBattleCanvs(3000);
            // 팝업을 화면에 표시하는 로직을 여기에 구현
            gameObject.SetActive(true);
            ButtonImg.color = new Color32(0x2A, 0xD0, 0xB8, 0xFF);
            ActiveButton(0);//CurrentSubCategoryindex
        }
        else
        {
            Hide();
        }
    }
    public void ActiveButton(int index)
    {

        if (index == -1)
        {
            index = 0;
        }

        for (int i = 0; i < Upgradeinbtnarray.Length; i++) 
        {
            if ( i == index)
            {
                //글씨색바꾸기
                Upgradeintextarray[i].color = Color.white;
                //안의팝업내용은 켜기
                CategorySubbtnarray[i].gameObject.SetActive(true);

                //나머지 밑의 오브젝트들도 끄기 
                 Upgradeinbtnarray[i].transform.GetChild(0).gameObject.SetActive(true);
                //나머지 밑의 오브젝트들도 끄기 
                //  Upgradeinbtnarray[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                CategorySubbtnarray[i].gameObject.SetActive(false);
                //글씨색 바꾸기
                Upgradeintextarray[i].color = Color.gray;
                //나머지 밑의 오브젝트들도 끄기 
                Upgradeinbtnarray[i].transform.GetChild(0).gameObject.SetActive(false);
                //나머지 밑의 오브젝트들도 끄기 
              //  Upgradeinbtnarray[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }

    }


    public void ActiveSubCategory(int i)
    { //클릭한 인덱스 열기
        
        if (GameDataTable.Instance.User.HunterPurchase[i] == false)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_BuyHunterFirst"), SystemNoticeType.Default);
            return;
        }

        
        switch (i) 
        {
            case 0:
                CurrentSubCategoryindex = 0;
                break;
                case 1:
                CurrentSubCategoryindex = 1;
                break;
                case 2:
                CurrentSubCategoryindex = 2;
                break;
        }
        //인덱스 해당하는 팝업켜지게..
        ActiveButton(CurrentSubCategoryindex); 
    }
}
