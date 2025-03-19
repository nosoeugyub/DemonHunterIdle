using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 시간관련 저장 / 로드 
/// </summary>
public class StandardTime : MonoSingleton<StandardTime>
{
    public Button DailySKillreset;
     public Button DailySkillSlotReset;


    private void Awake()
    {
        DailySKillreset.onClick.AddListener(resetclick);
        DailySkillSlotReset.onClick.AddListener(resetSlot);
    }

    private void resetclick()
    {
        //스텟 false로.
        GameDataTable.Instance.User.isDailySkill = false;
        //popupup 창 켜기
        PopUpSystem.Instance._DeilyRandomPopUp.Show();
        //랜덤스킬 셋
        PopUpSystem.Instance._DeilyRandomPopUp.Init_DailySkillSlotSet();
    }
    private void resetSlot()
    {
        //랜덤스킬 셋
        PopUpSystem.Instance._DeilyRandomPopUp.Init_DailySkillSlotSet();
        
    }
}
