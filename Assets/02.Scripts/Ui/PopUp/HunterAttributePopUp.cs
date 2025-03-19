using Game.Debbug;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 캐릭터 속성
/// /// </summary>
/// 
public class HunterAttributePopUp : MonoBehaviour, IPopUp
{

   public HunterAttributeView hunterattributeview;

    [SerializeField] private int ClickStack = 0; //가장 바깥쪽 카테고리의 버튼 클릭스텍
    [SerializeField] private int InfoClickIndex = 0; //카테고리의 세부 버튼의 인덱스

    //Image
    public Image ButtonImg;

    //button
    public Button[] Upgradeinbtnarray; //직업별 속성 업그레이드버튼
    public Button[] Categorybtnarray; //직업별 속성전환
    public Button Init_AttributeBtn;

    //text
    public TextMeshProUGUI[] Upgradeintextarray; //카테고리 버튼 이름
    public int CurrentSubCategoryindex = -1;




    private void Awake()
    {
        // 세부 카테고리 버튼 이벤트 할당
        for (int i = 0; i < Upgradeinbtnarray.Length; i++)
        {
            int index = i; // 반복문 변수의 값을 캡처하기 위해 변수를 생성합니다.
            Categorybtnarray[i].onClick.AddListener(() => ActiveSubCategory(index));
            Upgradeinbtnarray[i].onClick.AddListener(() => UpgradeBtnAttribute(index));
        }
        Init_AttributeBtn.onClick.AddListener(delegate { OnClickAttribute(CurrentSubCategoryindex); });
        GameEventSystem.GameLevel_SendGameEventHandler += HunterLevelUp; //헌터레벨업 이벤트
        ActiveHunterPortraits();
    }

    private void ActiveHunterPortraits() //Contrain에 값에따라서 탭 매뉴 활성화
    {
        int value = HunterPortraitSystem.Instance.GetHunterDeployment();
        for (int i = 0; i < Categorybtnarray.Length; i++)
        {
            if (i < value)
            {
                Categorybtnarray[i].gameObject.SetActive(true);
            }
            else
            {
                Categorybtnarray[i].gameObject.SetActive(false);
            }
        }
    }

    private void HunterLevelUp(float _level)
    {
        Utill_Enum.SubClass subclass = (Utill_Enum.SubClass)CurrentSubCategoryindex;
        hunterattributeview.UpdateUi_Attribute(GameDataTable.Instance.User.HunterAttribute[CurrentSubCategoryindex] , subclass);
    }

    //속성초기화함수
    private void OnClickAttribute(int _CurrentSubCategoryindex)
    {
        HunterAttributeSystem.Instance.Init_AttributeBtn(_CurrentSubCategoryindex);

    }

    private void UpgradeBtnAttribute(int index )
    {
        //사운드재생
        SoundManager.Instance.PlayAudio("HunterProperty");

        bool isx10toggle = hunterattributeview.AttributeToogle[index].isOn; //10배 토글이 눌러져있는지 

        switch (index)
        {
            case 0:
                HunterAttributeSystem.Instance.PluseAttribute_01(CurrentSubCategoryindex , isx10toggle);
                break;
            case 1:
                HunterAttributeSystem.Instance.PluseAttribute_02(CurrentSubCategoryindex , isx10toggle);
                break;
            case 2:
                HunterAttributeSystem.Instance.PluseAttribute_03(CurrentSubCategoryindex , isx10toggle);
                break;

        }
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
            CurrentSubCategoryindex = 0;

            ButtonImg.color = new Color32(0x2A, 0xD0, 0xB8, 0xFF);
            ActiveButton(CurrentSubCategoryindex);//
           hunterattributeview.Init_Toogle();
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

        //해당 챔피언을 구매했는지 안했는지 알고 안했으면 알림 
        if (GameDataTable.Instance.User.HunterPurchase[index] == false) 
        {
            //구매가 필요합니다.
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_BuyHunterFirst"), SystemNoticeType.Default);
            return;
        }


        for (int i = 0; i < Upgradeinbtnarray.Length; i++)
        {
            if (i == index)
            {
                //글씨색바꾸기
                Upgradeintextarray[i].color = Color.white;
                //하위팝업은 켜기
                hunterattributeview.Charactorindex = index;
                Utill_Enum.SubClass subclass = (Utill_Enum.SubClass)index;
                hunterattributeview.UpdateUi_Attribute(GameDataTable.Instance.User.HunterAttribute[index] , subclass);
                //나머지 밑의 오브젝트들도 끄기 
                Categorybtnarray[i].transform.GetChild(0).gameObject.SetActive(true);
                
            }
            else
            {
                //글씨색 바꾸기
                Upgradeintextarray[i].color = Color.gray;
                //나머지 밑의 오브젝트들도 끄기 
                Categorybtnarray[i].transform.GetChild(0).gameObject.SetActive(false);
            
            }
        }
    }
    public void ActiveSubCategory(int i)
    { 
        //클릭한 인덱스 열기
        //해당 챔피언을 구매했는지 안했는지 알고 안했으면 알림 
        if (GameDataTable.Instance.User.HunterPurchase[i] == false)
        {
            //구매가 필요합니다.
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
