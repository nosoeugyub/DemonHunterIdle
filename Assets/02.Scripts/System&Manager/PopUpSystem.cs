using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// /// </summary>
/// 작성일자   : 2024-06-04
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 팝업관리 인터페이스를 활용한 컨트롤러 
/// /// </summary>
public class PopUpSystem : MonoSingleton<PopUpSystem> 
{
    public HunterAttributePopUp HunterAttributePopUp;
    public CharacterUpgradePopup CharacterUpgradePopup;
    public CharacterInfoPopup CharacterInfoPopup;
    public ShopPopup ShopPopup;
    public SkillPopup SkillPopup;

    public DailyMissionPopUp DailyMissionPopUp;
    public SettingPopUp Setting;
    public MailPopUp MailPopUp;
    public RankingPopUp RankingPopUp;
    public UpgradeSubPopUp UpgradeSubPopUp;
//    public RellicSubPopUp RellicSubPopUp;
   // public SkillSubPopUp SkillSubPopUp;
    public ShopTab1 ShopTab1;
    public ShopTab2 ShopTab2;
    public ShopTab3 ShopTab3;
    public ChatPopUp ChatPopUp;

    public ArcherSkillPopup HunterSkillPopup;
    public GuardianSkillPopup GuardianSkillPopup;
    public MageSkillPopup MageSkillPopup;
    public DailySkillPopup _DailySkillPopup;
    public DailyRandomPopUp _DeilyRandomPopUp;
    public ShopPopUpController ShopPopUpController;

    //button
    [Space(10)]
    [Header("플레이 하단에 큰 카테고리 버튼")]
    public Button CharacterUpgradePopupBtn;
    public Button CharacterInfoPopupBtn;
    public Button ShopPopupBtn;
    public Button SkillPopupBtn;
    public Button  HunterAttributebtn;
    [Header("경험치바위에있는 설정버튼")]
    public Button DailyMissionBtn;
    public Button SettingBtn;
    public Button RankingBtn;
    public Button MailBtn;
    [Header("업그레이드 하위 버튼")]
    public Button UpgradeSubPopUpBtn;
   // public Button RellicSubPopUpBtn;
    //public Button SkillSubPopUpBtn;
    [Header("상점 하위 버튼")]
    public Button ShopTab1Btn;
    public Button ShopTab2UpBtn;
    public Button ShopTab3UpBtn;
    [Header("스킬 하위 버튼")]
    public Button HunterSkillPopupBtn;
    public Button GuardianSkillPopupBtn;
    public Button MageSkillPopupBtn;
    public Button DailySkillPopupBtn;
    [Header("채팅 옆 오른쪽 버튼")]
    public Button ChatShowBtn;

    [Space(10)]
    public RectTransform canvaseBattelUi;
    private List<IPopUp> _escPopupList = new List<IPopUp>();
    private Queue<IPopUp> _popupQueue = new Queue<IPopUp>();
    private IPopUp _currentPopup;
    private IPopUp _currentactivePopup;
    private Image mailRedDotImg = null;
    
    //ESC 단축키 사용가능 여부
    public bool CanUndoByESC = true;

    protected override void Awake()
    {
        base.Awake();
        init_PopUp();//초기화
        GameEventSystem.GameSequence_SendGameEventHandler += Gamestart;
        mailRedDotImg = MailBtn.GetComponentsInChildren<Image>(true).FirstOrDefault(img => img.gameObject != MailBtn.gameObject);
    }
    private bool Gamestart(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:
                Init_Setting();//셋팅
                return true;
        }
        return true;
    }
    public void Init_Setting()
    { 

        for(int i = 0; i < _escPopupList.Count; i++)
        {
            _escPopupList[i].Hide();
        }
        _escPopupList.Clear();
        
        //전체 비활성화
        UpgradeSubPopUp.Hide();
        ChatPopUp.Hide();

        //처음엔 모두 비활...
        _currentactivePopup = UpgradeSubPopUp;
        _popupQueue.Enqueue(_currentactivePopup);
        _currentactivePopup.Hide();
    }
    public void init_PopUp()
    {
        CharacterInfoPopupBtn.onClick.AddListener(() => ShowPopup(CharacterInfoPopup));
        ShopPopupBtn.onClick.AddListener(() => ShowPopup(ShopPopup));
        CharacterUpgradePopupBtn.onClick.AddListener(() => ShowPopup(CharacterUpgradePopup));
        HunterAttributebtn.onClick.AddListener(() => ShowPopup(HunterAttributePopUp));
        SkillPopupBtn.onClick.AddListener(() =>
        {
            ShowPopup(SkillPopup);
            _currentactivePopup = HunterSkillPopup;
        });

        DailyMissionBtn.onClick.AddListener(() => ShowPopup(DailyMissionPopUp));
        SettingBtn.onClick.AddListener(() => ShowPopup(Setting));
        RankingBtn.onClick.AddListener(() => ShowPopup(RankingPopUp));
        MailBtn.onClick.AddListener(() => ShowPopup(MailPopUp));


        UpgradeSubPopUpBtn.onClick.AddListener(() => ActivePopup(UpgradeSubPopUp));
        //RellicSubPopUpBtn.onClick.AddListener(() => ActivePopup(RellicSubPopUp));
       // SkillSubPopUpBtn.onClick.AddListener(() => ActivePopup(SkillSubPopUp));

        ShopTab1Btn.onClick.AddListener(() => ActivePopup(ShopTab2));
        ShopTab2UpBtn.onClick.AddListener(() => ActivePopup(ShopTab1));
        ShopTab3UpBtn.onClick.AddListener(() => ActivePopup(ShopTab3));

        ShopTab3UpBtn.onClick.AddListener(() => ActivePopup(ShopTab3));

        HunterSkillPopupBtn.onClick.AddListener(() => ActivePopup(HunterSkillPopup));
      // GuardianSkillPopupBtn.onClick.AddListener(() => ActivePopup(GuardianSkillPopup));
      // MageSkillPopupBtn.onClick.AddListener(() => ActivePopup(MageSkillPopup));
      //  WeeklySkillPopupBtn.onClick.AddListener(() => ActivePopup(_WeeklySkillPopup));

        ChatShowBtn.onClick.AddListener(() => ActivePopup(ChatPopUp));
    }

    public void ClosePopUp(IPopUp _popup)
    {
        if (_currentPopup == _popup)
        {
            _popup.Close();
            _popupQueue.Dequeue();
        }
       
    }

    public void ActivePopup(IPopUp popup)
    {
        SoundManager.Instance.PlayAudio("UIClick");

        if (_currentactivePopup == null)
        {
            _currentactivePopup = popup;
            _currentactivePopup.Show();
        }
        else if(_currentactivePopup == popup)
        {
            _currentactivePopup.Show();
        }
        else
        {
            _currentactivePopup.Hide();
            _currentactivePopup = popup;
            _currentactivePopup.Show();
        }
    }

    public void ShowPopup(IPopUp _ipopup)
    {
        SoundManager.Instance.PlayAudio("UIClick");
        // 현재 팝업이 없으면 새로운 팝업 열기
        if (_currentPopup == null)
        {
            _currentPopup = _ipopup;
            _popupQueue.Enqueue(_ipopup);
            _currentPopup.Show();
        }
        else if (_currentPopup == _ipopup)
        {
            _ipopup.Show();
        }
        else
        {
            HidePopUp(_ipopup);
            _popupQueue.Enqueue(_ipopup);
            _currentPopup.Show();
        }
    }

    public void HidePopUp(IPopUp _ipopup)
    {

        SoundManager.Instance.PlayAudio("UIClick");
        // 현재 열려있는 팝업이면 닫기
        if (_currentPopup == _ipopup)
        {
            _currentPopup.Hide();
            _currentPopup = null;
            // 다음 팝업 표시
        }
        else
        {
            // 큐에서 해당 팝업 제거
            _currentPopup.Hide();
            _popupQueue.Dequeue();
            //새로운팝업 등록
            _currentPopup = _ipopup;
        }
    }

    private void ShowNextPopup()
    {
        if (_popupQueue.Count > 0)
        {
            _currentPopup = _popupQueue.Dequeue();
            _currentPopup.Show();
           
        }
    }

    public void MoveBattleCanvs(float _movepos)
    {
        Vector3 newPos = canvaseBattelUi.localPosition;
        newPos.x = _movepos;
        canvaseBattelUi.localPosition = newPos;
    }

    public void EscPopupListAdd(IPopUp popUp)
    {
        if (!_escPopupList.Contains(popUp))
        {
            _escPopupList.Add(popUp);

            // isExitPopUp 플래그가 true일 때만 실행
            if (isExitPopUp)
            {
                UIManager.Instance.exitPopUp.Hide();
                isExitPopUp = false;
            }
        }
    }

    public void EscPopupListRemove()
    {
        if (_escPopupList.Count > 0)
        { 
            int lastIndex = _escPopupList.Count - 1;
            _escPopupList.RemoveAt(lastIndex);
        }

    }
    private bool isExitPopUp = false;
    public void InputCloseKey()
    {
        if (!CanUndoByESC)
            return;

        if (UIManager.Instance.exitPopUp.gameObject.activeSelf)
        {
            isExitPopUp = !isExitPopUp;
        }

        if (_escPopupList.Count > 0)
        {
            int lastIndex = _escPopupList.Count - 1;
            _escPopupList[lastIndex].Hide();
        }
        else
        {
            if (isExitPopUp)
            {
                UIManager.Instance.exitPopUp.Hide();
            }
            else
            {
                UIManager.Instance.exitPopUp.Show();
            }

            isExitPopUp = !isExitPopUp;
        }

    }
    public void Update()
    {
        if (Input.GetKeyDown(InputManager.Instance.closeKey))
        {
            InputCloseKey();
        }
        mailRedDotImg.gameObject.SetActive(UIManager.Instance.mailPopUp.MailCount > 0);
    }

}
