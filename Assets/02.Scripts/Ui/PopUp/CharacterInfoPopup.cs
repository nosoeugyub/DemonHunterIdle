using BackEnd;
using NSY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 캐릭터 정보 활용한 컨트롤러 
/// /// </summary>
public class CharacterInfoPopup : MonoBehaviour, IPopUp
{
  //  public UiHunter Uihunter;
    public SubPopType SubPopType;
    public InventoryPopType InventoryPopUp;

    [SerializeField] 
    private TextMeshProUGUI HunterNickName;
    [SerializeField] 
    private TextMeshProUGUI HunterDPS;

    private List<CostumeStlyer> uiHunterCostumeStlyer = new List<CostumeStlyer>();

    public int ClickStack = 0;


    public Image ButtonImg;

    //button
    public Button Charinfobtn;

    public TextMeshProUGUI promotionText;

    public void Close()
    {
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);

        SubPopType.gameObject.SetActive(false);
    }

    public void Hide()
    {
        if (UIManager.Instance.inventoryPopType.gameObject.activeSelf)
        {
            UIManager.Instance.inventoryPopType.Hide();
        }
        if (UIManager.Instance.promotionPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.promotionPopUp.Hide();
        }
        if (UIManager.Instance.gachaAchievementPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.gachaAchievementPopUp.Hide();
        }
        if (UIManager.Instance.gachaDictionaryPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.gachaDictionaryPopUp.Hide();
        }
        if (UIManager.Instance.gachaPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.gachaPopUp.Hide();
        }
        if (UIManager.Instance.ItemDrawerview.gameObject.activeSelf)
        {
            UIManager.Instance.ItemDrawerview.gameObject.SetActive(false);
        }
        if (UIManager.Instance.newitempopup.gameObject.activeSelf)
        {
            UIManager.Instance.newitempopup.gameObject.SetActive(false);
        }
        ClickStack = 0;
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
        ButtonImg.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        //배틀유아이이동
        PopUpSystem.Instance.MoveBattleCanvs(0);
        PopUpSystem.Instance.EscPopupListRemove();

        SubPopType.gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickStack++;
        if (ClickStack == 1)
        {
            HunterChangeSystem.Instance.SetHunterOpenNumber();
            SetPromotionText();
            if (uiHunterCostumeStlyer.Count == 0)
            {
                for (int i = 0; i < HunterChangeSystem.Instance.hunterObjs.Length; i++)
                {
                    uiHunterCostumeStlyer.Add(HunterChangeSystem.Instance.hunterObjs[i].Model);
                    uiHunterCostumeStlyer[i].Init();
                }
            }
            PopUpSystem.Instance.EscPopupListAdd(this);
            // 팝업을 화면에 표시하는 로직을 여기에 구현
            gameObject.SetActive(true);
            ButtonImg.color = new Color32(0x2A, 0xD0, 0xB8, 0xFF);
            //배틀유아이이동
            PopUpSystem.Instance.MoveBattleCanvs(3000);

            HunterChangeSystem.Instance.ActiveUIHunter();
            HunterChangeSystem.Instance.ResetRotateValueY();

            //닉네임설정
            if (BackendManager.Instance.IsLocal == false)
            {
                HunterNickName.text = Backend.UserNickName.ToString();
            }
            else
            {
                string Nickname = "테스트닉네임";
                HunterNickName.text = Nickname.ToString();
            }
            for (int i = 0; i < uiHunterCostumeStlyer.Count; i++)
            {
                uiHunterCostumeStlyer[i].SkinnedmeshSetting();
                uiHunterCostumeStlyer[i].SetCostumeStyle(GameDataTable.Instance.InventoryList[i]);
            }
            EquipmentItemManager.Instance.SettingCellList();
            //EquipmentItemManager.Instance.SettingCellLock();
            //전투력도 표시
            SetDPS();
        }
        else
        {
            Hide();
        }
    }

    public  void SetDPS() // 헌터 dps 셋팅해서보여주는곳
    {
        int currenthunter = GameDataTable.Instance.User.currentHunter;
        bool isbuyhunter = GameDataTable.Instance.User.HunterPurchase[currenthunter];
        float dps = 0;

        if (isbuyhunter)
        {
             dps = HunterStat.GetCombatPoint(DataManager.Instance.Hunters[currenthunter]._UserStat, currenthunter);//일단 치트도 포함해야함
            HunterDPS.text =Utill_Math.FormatCurrency((int)dps);
        }
        else
        {
            HunterDPS.text = "-";
        }
       
    }

    public void SetPromotionText()
    {
        promotionText.SetText($"{GameDataTable.Instance.User.HunterPromotion[GameDataTable.Instance.User.currentHunter]}승급");
    }


    public void OnInventoryPopUp()
    {

        if (HunterPurchaseSystem.Instance.IsHunterPurchase())
        {
            InventoryPopUp.Show();

            HunterChangeSystem.Instance.AllDeactiveUIHunter();
        }
        else
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_BuyHunterFirst"), SystemNoticeType.Default);
        }

       

       // Uihunter.gameObject.SetActive(false);
    }
    public void OnEquipmentItemCell()
    {
        HunterChangeSystem.Instance.AllDeactiveUIHunter();

       // Uihunter.gameObject.SetActive(false);
        HunterDPS.gameObject.SetActive(false);
        HunterNickName.gameObject.SetActive(false);
    }
    public void OnPromotion()
    {
        if (HunterPurchaseSystem.Instance.IsHunterPurchase())
        {
            UIManager.Instance.promotionPopUp.Show();
        }
        else
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_BuyHunterFirst"), SystemNoticeType.Default);
        }

    }

    //헌터 인포창 열었을때
    public void OnPopUpSubInfo()
    {
        if (HunterPurchaseSystem.Instance.IsHunterPurchase())
        {

            HunterChangeSystem.Instance.AllDeactiveUIHunter();
            //Uihunter.gameObject.SetActive(false);
            SubPopType.Show();
            SubPopType.SetStatData(); //스텟정보 깔기
        }
        else
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_BuyHunterFirst"), SystemNoticeType.Default);
        }
    }

    public void OffPopUpSubInfo()
    {
        HunterChangeSystem.Instance.ActiveUIHunter();

        SubPopType.Hide();
       // SubPopType.SetStatData(); //스텟정보 깔기
    }
}
