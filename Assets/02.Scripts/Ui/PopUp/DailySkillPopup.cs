using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 요일 스킬 파업관련 스크립트
/// /// </summary>
public class DailySkillPopup : MonoBehaviour, IPopUp
{
    //현재 유저가 장착했던 스킬
    public string[] HasSelectName;
    public int PresetCount;
    public Utill_Enum.SubClass ClassType;

    [SerializeField] private SubPopType subpoptype;
    int ClickCount = 0;


    public DailySkillLayout dailySkillLayout;

    private void OnEnable()
    {
        //해당 데이터로 교체
        SkillManager.Instance.OnMageSKillUi();
        dailySkillLayout.presetClick();
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
}
