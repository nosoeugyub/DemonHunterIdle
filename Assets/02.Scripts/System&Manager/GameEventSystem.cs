using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEventSystem;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-04
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :게임 이벤트 델리게이트 시스템
/// </summary>
public class GameEventSystem 
{
    public delegate void GameEventHandler(); //예제 델리게이트
    public static event GameEventHandler SendGameEventHandler;

    public static void GameEventHandler_Event()
    {
        SendGameEventHandler.Invoke();
    }

    //게임시퀀스 델리게이트
    public delegate bool GameSequence_GameEventHandler(Utill_Enum.Enum_GameSequence GameSequence);
    public static event GameSequence_GameEventHandler GameSequence_SendGameEventHandler;

    public static bool GameSequence_GameEventHandler_Event(Utill_Enum.Enum_GameSequence GameSequence)
    {
        if (GameSequence_SendGameEventHandler != null)
        {
            // 이벤트 핸들러가 모두 호출되었는지 확인하기 위한 변수
            bool result = true;
            // 이벤트 핸들러 호출
            GameSequence_SendGameEventHandler.Invoke(GameSequence);
            // 모든 이벤트 핸들러의 반환값이 true인 경우 true를 반환하고, 그렇지 않으면 false를 반환
            return result;
        }
        else
        {
            // 등록된 이벤트 핸들러가 없는 경우 기본값으로 false를 반환
            return false;
        }
    }

    //게임 치트 활성화/비활성화
    public delegate void GameToggleCheat_GameEventHandler(bool isOn);
    public static event GameToggleCheat_GameEventHandler GameToggleCheat_SendGameEventHandler;

    public static void GameToggleCheat_GameEventHandler_Event(bool isOn)
    {
        GameToggleCheat_SendGameEventHandler.Invoke(isOn);
    }
    //헌터 스텟 치트 활성화/비활성화
    public delegate void GameToggleHunterStatCheat_GameEventHandler(bool isOn);
    public static event GameToggleHunterStatCheat_GameEventHandler GameToggleHunterStatCheat_SendGameEventHandler;

    public static void GameToggleHunterStatCheat_GameEventHandler_Event(bool isOn)
    {
        GameToggleHunterStatCheat_SendGameEventHandler.Invoke(isOn);
    }
    //적 스텟 치트 활성화/비활성화
    public delegate void GameToggleEnemyStatCheat_GameEventHandler(bool isOn);
    public static event GameToggleEnemyStatCheat_GameEventHandler GameToggleEnemyStatCheat_SendGameEventHandler;

    public static void GameToggleEnemyStatCheat_GameEventHandler_Event(bool isOn)
    {
        GameToggleEnemyStatCheat_SendGameEventHandler.Invoke(isOn);
    }

    //게임 레벨업 델리게이트
    public delegate void GameLevelUp_GameEventHandler(float _level);
    public static event GameLevelUp_GameEventHandler GameLevel_SendGameEventHandler;

    public static void GameLevel_GameEventHandler_Event(float _level)
    {
        if (GameLevel_SendGameEventHandler == null) return;
        GameLevel_SendGameEventHandler.Invoke(_level);
    }

    //게임 경험치업업 델리게이트
    public delegate void GameAddExp_GameEventHandler(float _exp);
    public static event GameLevelUp_GameEventHandler GameAddExp_SendGameEventHandler;

    public static void GameAddExp_GameEventHandler_Event(float _exp)
    {
        GameAddExp_SendGameEventHandler.Invoke(_exp);
    }

    //전투 시작/종료 델리게이트
    public delegate void GameBattleSequence_GameEventHandler(bool isBattle);
    public static event GameBattleSequence_GameEventHandler GameBattleSequence_SendEventHandler;
    public static void GameBattleSequence_GameEventHandler_Event(bool isBattle)
    {
        GameBattleSequence_SendEventHandler?.Invoke(isBattle);
    }

    //절전모드 조건 체크 델리게이트
    //true : 절전모드 돌입 가능
    public delegate bool GameSleepModeCheck_GameEventHandler();
    public static event GameSleepModeCheck_GameEventHandler GameSleepModeCheck_SendEventHandler;
    public static bool GameSleepModeCheck_GameEventHandler_Event()
    {
        if(GameSleepModeCheck_SendEventHandler == null) 
            return true;
        return GameSleepModeCheck_SendEventHandler.Invoke();
    }

    //절전모드 종료 델리게이트
    public delegate void GameSleepModeStop_GameEventHandler();
    public static event GameSleepModeStop_GameEventHandler GameSleepModeStop_SendEventHandler;
    public static void GameSleepModeStop_GameEventHandler_Event()
    {
        GameSleepModeStop_SendEventHandler.Invoke();
    }

    //방치모드 피로도 증가 델리게이트
    public delegate void GameAddRestCycle_GameEventHandler(int amount);
    public static event GameAddRestCycle_GameEventHandler GameAddRestCycle_SendGameEventHandler;

    public static void GameAddRestCycle_GameEventHandler_Event(int amount)
    {
        if (GameAddRestCycle_SendGameEventHandler == null) return;
        GameAddRestCycle_SendGameEventHandler.Invoke(amount);
    }

    //방치모드 피로도 감소 델리게이트
    public delegate void GameSubRestCycle_GameEventHandler(int amount);
    public static event GameSubRestCycle_GameEventHandler GameSubRestCycle_SendGameEventHandler;

    public static void GameSubRestCycle_GameEventHandler_Event(int amount)
    {
        if (GameSubRestCycle_SendGameEventHandler == null) return;
        GameSubRestCycle_SendGameEventHandler.Invoke(amount);
    }

    //방치모드 휴식 시작 델리게이트
    public delegate void GameRestModeSequence_GameEventHandler(Utill_Enum.IdleModeRestCycleSequence sequence);
    public static event GameRestModeSequence_GameEventHandler GameRestModeSequence_SendGameEventHandler;

    public static void GameRestModeSequence_GameEventHandler_Event(Utill_Enum.IdleModeRestCycleSequence sequence)
    {
        if (GameRestModeSequence_SendGameEventHandler == null) return;
        GameRestModeSequence_SendGameEventHandler.Invoke(sequence);
    }

    //적 킬 델리게이트
    public delegate void GameKillCount_GameEventHandler(EnemyStat enemyStat);
    public static event GameKillCount_GameEventHandler GameKillCount_SendGameEventHandler;
    public static void GameKillClunt_GameEventHandler_Event(EnemyStat enemyStat)
    {
        GameKillCount_SendGameEventHandler.Invoke(enemyStat);
    }

    //보스 등장 델리게이트
    public delegate void GameBossSqeunce_GameEventHandler(Utill_Enum.BossSqeunce bossSqenunce);
    public static event GameBossSqeunce_GameEventHandler GameBossSqeuce_SendGameEventHandler;
    public static BossSqeunce CurBossSqenunce = BossSqeunce.None;
    public static void GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce bossSqenunce)
    {
        CurBossSqenunce = bossSqenunce;
        GameBossSqeuce_SendGameEventHandler.Invoke(bossSqenunce);
    }

    //헌터 사망 델리게이트
    public delegate void GameHunterDie_GameEventHandler();
    public static event GameHunterDie_GameEventHandler GameHunterDie_SendGameEventHandler;
    public static void GameHunterDie_SendGameEventHandler_Event()
    {
        GameHunterDie_SendGameEventHandler.Invoke();
    }

    //몬스터 극단적 선택 함수
    public delegate void GameEnemyDie_GameEventHandler();
    public static event GameEnemyDie_GameEventHandler GameEnemyDie_SendGameEventHandler;
    public static void GameEnemyDie_SendGameEventHandler_Event()
    {
        GameEnemyDie_SendGameEventHandler.Invoke();
    }

    //디버프를 추가하는 이벤트를 보냄
    public delegate void GameDebuff_GameEventHandler(EnemyStat target ,  DeBuff Debuff_type);
    public static event GameDebuff_GameEventHandler GameDebuff_GameEventHandler_Event;

    public static void Send_GameDebuff_GameEventHandler(EnemyStat target, DeBuff Debuff_type)
    {
        GameDebuff_GameEventHandler_Event.Invoke(target , Debuff_type);
    }


    //디버프를 감소하는 이벤트를 보냄
    public delegate void GameRemoveDebuff_GameEventHandler(EnemyStat target , DeBuff Debuff_type);
    public static event GameRemoveDebuff_GameEventHandler GameRemoveDebuff_GameEventHandler_Event;

    public static void Send_GameRemoveDebuff_GameEventHandler(EnemyStat target , DeBuff Debuff_type)
    {
        GameRemoveDebuff_GameEventHandler_Event.Invoke(target , Debuff_type);
    }

    //스킬 마나부족 이벤트
    public delegate void GameSkillManaShortage_GameEventHandler(BattleSkillUi battleSkillUi);
    public static event GameSkillManaShortage_GameEventHandler GameSkillManaShortage_SendEventHandler;
    public static void GameSkillManaShortage_GameEventHandler_Event(BattleSkillUi battleSkillUi)
    {
        GameSkillManaShortage_SendEventHandler?.Invoke(battleSkillUi);
    }

    #region 재화류
    public delegate void GamePlusGold_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value);
    public static event GamePlusGold_GameEventHandler GamePluseGold_GameEventHandler_Event;

    public static void Send_GamePlusGold_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye , int Value)
    {
        GamePluseGold_GameEventHandler_Event.Invoke(Resourcetpye ,Value);
    }

    public delegate void GameMinusGold_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value);
    public static event GameMinusGold_GameEventHandler GameMinusGold_GameEventHandler_Event;

    public static void Send_GameMinusGold_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        GameMinusGold_GameEventHandler_Event.Invoke(Resourcetpye, Value);
    }

    public delegate void GamePlusDia_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value);
    public static event GamePlusDia_GameEventHandler GamePluseDia_GameEventHandler_Event;

    public static void Send_GamePlusDia_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        GamePluseGold_GameEventHandler_Event.Invoke(Resourcetpye, Value);
    }


    public delegate void GameMinusDia_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value);
    public static event GameMinusDia_GameEventHandler GameMinusDia_GameEventHandler_Event;

    public static void Send_GameMinusDia_GameEventHandler(Utill_Enum.Resource_Type Resourcetpye, int Value)
    {
        GameMinusDia_GameEventHandler_Event.Invoke(Resourcetpye, Value);
    }
    #endregion

    #region 현재 재화를 얻으면 강화가 가능한지
    //현재 재화들로 강화가 될수 있는지 없는 확인해야함 
    public delegate void GameCheckResource_GameEventHandler(int level , int Gold , int Dia);
    public static event GameCheckResource_GameEventHandler GameCheckResource_GameEventHandler_Event;

    public static void Send_GameCheckResource_GameEventHandler(int level, int Gold, int Dia)
    {
        if (GameCheckResource_GameEventHandler_Event != null)
        {
             GameCheckResource_GameEventHandler_Event.Invoke(level, Gold , Dia);
        }
      
    }
    #endregion


    #region 헌터들의 출전버튼을 눌렀을때 스킬초기화 && 헌터가 죽거나 부활했을때 초기화
    public delegate void GameHunterPortraitClickBtn_GameEventHandler(Utill_Enum.SubClass type, bool isEquip);
    public static event GameHunterPortraitClickBtn_GameEventHandler GameHunterPortraitClickBtn_GameEventHandler_Event;

    public static void Send_GameHunterPortraitClickBtn_GameEventHandler(Utill_Enum.SubClass type, bool isEquip)
    {
        if (GameHunterPortraitClickBtn_GameEventHandler_Event != null)
        {
            GameHunterPortraitClickBtn_GameEventHandler_Event.Invoke(type, isEquip);
        }

    }

    public delegate void GameHunterReviveSkillInit_GameEventHandler(Utill_Enum.SubClass type, bool isRevive);
    public static event GameHunterReviveSkillInit_GameEventHandler GameHunterReviveSkillInit_GameEventHandler_Event;

    public static void Send_GameHunterReviveSkillInit_GameEventHandler_Event(Utill_Enum.SubClass type, bool isRevive)
    {
        if (GameHunterReviveSkillInit_GameEventHandler_Event != null)
        {
            GameHunterReviveSkillInit_GameEventHandler_Event.Invoke(type, isRevive);
        }

    }
    #endregion


    #region 몬스터 사망시 이벤트
    public delegate void EnemyDie_GameEventHandler();
    public static event EnemyDie_GameEventHandler EnemyDie_GameEventHandler_Event;

    public static void Send_EnemyDie_GameEventHandler()
    {
        EnemyDie_GameEventHandler_Event.Invoke();
    }
    #endregion


    #region 모루 추출 이벤트 
    public delegate int Click_ItemDrawer(int Count , HunteritemData hunterdata);
    public static event Click_ItemDrawer Click_ItemDrawer_Event;

    public static int Send_Click_ItemDrawer(int Count , HunteritemData hunterdata)
    {
        return Click_ItemDrawer_Event.Invoke(Count, hunterdata); 
    }

    #endregion


    #region 모루 추출시 앙이템 선택 이벤트
    public delegate void Click_ItemSelectDrawer();
    public static event Click_ItemSelectDrawer Click_ItemSelectDrawer_Event;

    public static void Send_Click_ItemSelectDrawer_Event()
    {
        Click_ItemSelectDrawer_Event.Invoke(); 
    }
    #endregion


    #region 모루 추출시 앙이템 판매 이벤트
    public delegate void Click_ItemSellDrawer();
    public static event Click_ItemSellDrawer Click_ItemSellDrawer_Event;

    public static void Send_Click_ItemSellDrawer_Event()
    {
        Click_ItemSellDrawer_Event.Invoke();
    }
    #endregion

    #region 모루 강화 이벤트
    public delegate void Click_UpgradeBtnDrawer(HunteritemData data);
    public static event Click_UpgradeBtnDrawer Click_UpgradeBtnDrawer_Event;

    public static void Send_Click_UpgradeBtnDrawer_Event(HunteritemData data)
    {
        Click_UpgradeBtnDrawer_Event.Invoke(data);
    }
    #endregion


    #region 레벨에 따라 ui표기 
    public delegate void StageLevel_UiDraw(int StageLevel);
    public static event StageLevel_UiDraw StageLevel_UiDraw_Event;

    public static void Send_StageLevel_UiDraw(int StageLevel)
    {
        StageLevel_UiDraw_Event.Invoke(StageLevel);
    }
    #endregion
    
    #region 실시간 시간과 갱신 표기 
    public delegate void DailyInit_Event(TimeSpan timespan, bool isToday);
    public static event DailyInit_Event DailyInit_Event_Event;

    public static void Send_DailyInit_Event(TimeSpan timespan, bool isToday)
    {
        DailyInit_Event_Event.Invoke(timespan , isToday);

    }
    #endregion

    #region 일일 임무 달성도가 변경되었을 시
    public delegate void DailyMission_UiDraw(DailyMissionType type);
    public static event DailyMission_UiDraw DailyMission_UiDraw_Event;

    public static void Send_DailyMission_UiDraw(DailyMissionType type)
    {
        if (DailyMission_UiDraw_Event == null) return;

        DailyMission_UiDraw_Event.Invoke(type);
    }
    #endregion


    #region 현재 장비 데이터 슬롯을 장착
    public delegate void Equipment_EuipSlot(HunteritemData type);
    public static event Equipment_EuipSlot Equipment_EuipSlot_Event;

    public static void Send_Equipment_EuipSlot(HunteritemData type)
    {
        Equipment_EuipSlot_Event.Invoke(type);
    }
    #endregion

    #region 현재 장비 데이터 슬롯 해제
    public delegate void UnEquipment_EuipSlot(HunteritemData type);
    public static event UnEquipment_EuipSlot UnEquipment_EuipSlot_Event;

    public static void Send_UnEquipment_EuipSlot(HunteritemData type)
    {
        UnEquipment_EuipSlot_Event.Invoke(type);
    }
    #endregion

    #region 오프라인보상 받기 이벤트
    public delegate void OfflineReward();
    public static event OfflineReward OfflineReward_Event;

    public static void Send_OfflineReward()
    {
        OfflineReward_Event.Invoke();
    }
    #endregion

    #region 승급 버튼 이벤트
    public delegate void Promotion();
    public static event Promotion Promotion_Event;

    public static void Send_Promotion()
    {
        Promotion_Event.Invoke();
    }
    #endregion

    #region 스킬 정보보기 버튼 이벤트
    public delegate void GameShowSkillInfo_GameEventHandler(ISkill skill);
    public static event GameShowSkillInfo_GameEventHandler GameShowSkillInfo_GameEventHandler_Event;

    public static void Send_GameShowSkillInfo_GameEventHandler_Event(ISkill skill)
    {
        GameShowSkillInfo_GameEventHandler_Event.Invoke(skill);
    }
    #endregion

    #region 스킬 정보보기 버튼 이벤트
    public delegate void GameUpdateSkillUIElement_GameEventHandler();
    public static event GameUpdateSkillUIElement_GameEventHandler GameUpdateSkillUIElement_GameEventHandler_Event;

    public static void Send_GameUpdateSkillUIElement_GameEventHandler_Event()
    {
        GameUpdateSkillUIElement_GameEventHandler_Event.Invoke();
    }
    #endregion

    #region 모루 추출 이벤트시 일어나면안되는 상황들 이벤트
    public delegate void Drawer_PlayAnimation_DeActiveFunc();
    public static event Drawer_PlayAnimation_DeActiveFunc Drawer_PlayAnimation_DeActiveFunc_Event;

    public static void Send_Drawer_PlayAnimation_DeActiveFunc()
    {
        Drawer_PlayAnimation_DeActiveFunc_Event.Invoke();
    }
    #endregion


    #region 모루 추출 끝났으면 다시 기능  원상복구도돌리는 이벤트
    public delegate void Drawer_PlayAnimation_ActiveFunc();
    public static event Drawer_PlayAnimation_ActiveFunc Drawer_PlayAnimation_ActiveFunc_Event;

    public static void Send_Drawer_PlayAnimation_ActiveFunc()
    {
        Drawer_PlayAnimation_ActiveFunc_Event.Invoke();
    }
    #endregion

    #region 10 뽑기로 장비 흭득 애니메이션 중일때 제어 이벤트
    public delegate void Gacha_PlayAnimation_ActiveFunc();
    public static event Gacha_PlayAnimation_ActiveFunc Gacha_PlayAnimation_ActiveFunc_Event;

    public static void Send_Gacha_PlayAnimation_ActiveFunc()
    {
        Gacha_PlayAnimation_ActiveFunc_Event.Invoke();
    }
    #endregion

    #region  10 뽑기로 장비 흭득 애니메이션 중일때 제어 히제 이벤트
    public delegate void Gacha_PlayAnimation_DeActiveFunc();
    public static event Gacha_PlayAnimation_DeActiveFunc Gacha_PlayAnimation_DeActiveFunc_Event;

    public static void Send_Gacha_PlayAnimation_DeActiveFunc()
    {
        Gacha_PlayAnimation_DeActiveFunc_Event.Invoke();
    }
    #endregion


    #region  재화 일괄 업데이트
    public delegate void Resource_Update();
    public static event Resource_Update Resource_Update_Event;

    public static void Send_Resource_Update()
    {
        Resource_Update_Event.Invoke();
    }
    #endregion

    #region 게임 상태 슬롯 이벤트
    public delegate void Use_Skill_Delegate(BattleSkillUi battleskillui);
    public static event Use_Skill_Delegate Use_Skill_Delegate_Event;

    public static void Send_Use_Skill_Delegate(BattleSkillUi battleskillui)
    {
        Use_Skill_Delegate_Event.Invoke(battleskillui);
    }
    #endregion


    #region 게임 저장 이벤트 
    public delegate void GameSave_Delegate();
    public static event GameSave_Delegate GameSave_Delegate_Event;

    public static void Send_GameSave_Delegate()
    {
        GameSave_Delegate_Event.Invoke();
    }
    #endregion
}
