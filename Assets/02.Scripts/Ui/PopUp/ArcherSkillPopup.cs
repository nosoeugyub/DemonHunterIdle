using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 아쳐스킬팝업 스크립트
/// </summary>
public class ArcherSkillPopup : MonoBehaviour, IPopUp
{

    //현재 유저가 장착했던 스킬
    public string[] HasSelectName;
    public int PresetCount;
    public Utill_Enum.SubClass ClassType;


    [SerializeField] private SubPopType subpoptype;
    int ClickCount = 0;

    public ArcherSkillLayout hunterskilllayout;

    private void OnEnable()
    {
        //해당 데이터로 교체
        hunterskilllayout.presetClick(GameDataTable.Instance.User.Euipmentuserpreset[0]);
        GameEventSystem.GameShowSkillInfo_GameEventHandler_Event += ShowSkillInfo;
    }

    private void OnDisable()
    {
        GameEventSystem.GameShowSkillInfo_GameEventHandler_Event -= ShowSkillInfo;
    }

    public void Close()
    {
        throw new System.NotImplementedException();

    }

    public void Hide()
    {
        ClickCount = 0;
        gameObject.SetActive(false);
        subpoptype.Hide();
        GameEventSystem.GameShowSkillInfo_GameEventHandler_Event -= ShowSkillInfo;
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(PopUpSystem.Instance.ShopPopup);
        }
        gameObject.SetActive(true);
       
    }

    private void ShowSkillInfo(ISkill skill)
    {
        UIManager.Instance.skillInfoPopUp.SetSkillInfo(skill, ClassType, PresetCount);
    }
}
