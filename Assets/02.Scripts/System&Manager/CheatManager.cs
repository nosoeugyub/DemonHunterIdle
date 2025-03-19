using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 치트 관련 컨트롤러
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(CheatManager))]
public class CheatManagerEditor : Editor
{
    const string INFO = 
        "<b>현재 단축키 목록 (추가시 목록 갱신 바람)</b>\n" +
        "ctrl T : Cheat Mode 체크\n" +
        "ctrl L : 게임 리셋\n" +
        "ctrl W : 강화 전부 0 만들기\n" +
        "ctrl Q : 현 스테이지레벨 보스전 입장\n" +
        "ctrl E : Scene 에서 작업 뷰 (스위치)\n" +
        "ctrl G : 장착중인 모든 헌터 사망\n" +
        "esc    : 창끄기 & 종료\n" +
        "<,>    : 게임 배속\n" +
        " F     : 재화 최대치\n" +
        "ctrl alt G : 스테이지 보스 사망\n" +
        "ctrl 초상화 : 원하는 캐릭터 사망\n"; 

       

    public override void OnInspectorGUI()
    {
        GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
        myStyle.fontSize = 13;
        myStyle.richText = true;

        EditorGUILayout.TextArea(INFO, myStyle);
        //GUIContent info = new GUIContent(INFO);
        //EditorGUILayout.HelpBox(INFO, MessageType.Info,);
        base.OnInspectorGUI();
    }
}
#endif
public class CheatManager : MonoSingleton<CheatManager>
{
    [Header("치트 UI 부모 오브젝트")]
    [SerializeField]
    private GameObject cheatUI = null;
    [Header("치트 활성화 여부 알려주는 텍스트")]
    [SerializeField]
    private TMP_Text cheatModeText = null;




    [Header("디버그UI 표시")]   //저장과 관련된 로직이라 꼭 필요
    [SerializeField] private bool debugUIDisplay;
    public bool DebugUIDisplay
    {
        get
        {
            return debugUIDisplay;
        }
        set
        {
            debugUIDisplay = value;
        }
    }

    [Header("시간관련 표기 on/off ")]
    [SerializeField] private bool isCheatTimeMode;
    public bool IsCheatTimeMode
    {
        get
        {
            return isCheatTimeMode;
        }
        set
        {
            isCheatTimeMode = value;
        }
    }

    [Header("몬스터 스펙 표기 on/off ")]
    [SerializeField] private bool isMonsterSpecMode;
    public bool IsMonsterSpecMode
    {
        get
        {
            return isMonsterSpecMode;
        }
        set
        {
            isMonsterSpecMode = value;
        }
    }

    [Header("강화 0 으로 전부 ")]
    [SerializeField] private bool isUpgradeMode;
    public bool IsUpgradeMode
    {
        get
        {
            return isUpgradeMode;
        }
        set
        {
            isUpgradeMode = value;
        }
    }

    [Header("헌터 이동 로직 해제")]
    [SerializeField] private bool hunterMoveOff;
    public bool HunterMoveOff
    {
        get
        {
            return hunterMoveOff;
        }
        set
        {
            hunterMoveOff = value;
        }
    }

    [Header("부활창 안 뜨게")]
    [SerializeField] private bool disableResurrection;
    public bool DisableResurrection
    {
        get
        {
            return disableResurrection;
        }
        set
        {
            disableResurrection = value;
        }
    }

    [Header("추출기 진화 조건 무시")]
    [SerializeField] private bool ignoreDrawerEVCondition;
    public bool IgnoreDrawerEVCondition
    {
        get
        {
            return ignoreDrawerEVCondition;
        }
        set
        {
            ignoreDrawerEVCondition = value;
        }
    }

    [Header("일일 초기화 기준 변경")]
    [SerializeField] private bool dailyResetTimeChange;
    public bool DailyResetTimeChange
    {
        get
        {
            return dailyResetTimeChange;
        }
        set
        {
            dailyResetTimeChange = value;
        }
    }
    [Space(10)]
    [Header("---------------------------------------------------------------")]
    [Header("게임 수치 변경")]
    [SerializeField] private bool valueEditing;
    public bool ValueEditing
    {
        get
        {
            return valueEditing;
        }
        set
        {
            valueEditing = value;
        }
    }


    [Header("재화")]
    public int Gold;
    public int Dia;

    [Header("클리어스테이지")]
    public int ClearStageLevel;

    [Header("궁수")]
    public int ArcherLevel;
    public int ArcherExp;
    public int ArcherHP;
    public int ArcherMP;

    [Header("수호자")]
    public int GuardianLevel;
    public int GuardianExp;
    public int GuardianHP;
    public int GuardianMP;

    [Header("법사")]
    public int MageLevel;
    public int MageExp;
    public int MageHP;
    public int MageMP;


    private void Start()
    {
#if UNITY_EDITOR
        cheatModeText.gameObject.SetActive(true);
        cheatModeText.transform.SetAsLastSibling();
        UpdateCheatModeText();

        GameEventSystem.GameToggleCheat_SendGameEventHandler += (isOn)=>UpdateCheatModeText();

#else
        cheatModeText.gameObject.SetActive(false);
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (ValueEditing)
        {
            //레벨 치트 적용
            LevelUpSystem.Instance.LockInputLevel(ValueEditing, ArcherLevel, Utill_Enum.SubClass.Archer);
            LevelUpSystem.Instance.LockInputLevel(ValueEditing, GuardianLevel, Utill_Enum.SubClass.Guardian);
            LevelUpSystem.Instance.LockInputLevel(ValueEditing, MageLevel, Utill_Enum.SubClass.Mage);

            //경험치 치트 적용
            LevelUpSystem.Instance.LockInputEXP(ValueEditing, ArcherExp, Utill_Enum.SubClass.Archer);
            LevelUpSystem.Instance.LockInputEXP(ValueEditing, GuardianExp, Utill_Enum.SubClass.Guardian);
            LevelUpSystem.Instance.LockInputEXP(ValueEditing, MageExp, Utill_Enum.SubClass.Mage);

            //hp,mp 치트 적용
            HunterStat.SetHp(DataManager.Instance.Hunters[0]._UserStat,ArcherHP);
            HunterStat.SetMp(DataManager.Instance.Hunters[0]._UserStat, ArcherMP);
            HunterStat.SetHp(DataManager.Instance.Hunters[1]._UserStat, GuardianHP);
            HunterStat.SetMp(DataManager.Instance.Hunters[1]._UserStat, GuardianMP);
            HunterStat.SetHp(DataManager.Instance.Hunters[2]._UserStat, MageHP);
            HunterStat.SetMp(DataManager.Instance.Hunters[2]._UserStat, MageMP);
            //치트 적용한 값으로 ui 갱신
            for(int i = 0; i < DataManager.Instance.Hunters.Count; i ++)
            {
                DataManager.Instance.Hunters[i].hpuiview.UpdateHpBar(DataManager.Instance.Hunters[i]._UserStat.HP, DataManager.Instance.Hunters[i]._UserStat.CurrentHp);
                DataManager.Instance.Hunters[i].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[i]._UserStat.MP, DataManager.Instance.Hunters[i]._UserStat.CurrentMp);
            }

            //골드 적용
            ResourceManager.Instance.SetGold(Utill_Enum.Resource_Type.Gold, Gold);

            //다이아 적용
            ResourceManager.Instance.SetDia(Utill_Enum.Resource_Type.Dia, Dia);

            //강화전부 0 
            // DataManager.Instance._HunterUpGradeController.Init_Zero(IsUpgradeMode);
            ValueEditing = false; //1회만 적용
        }
        //몬스터 스펙 팝업
        GroundCreatSystem.Instance.StageClearView.ActiveMonsterSpec(IsMonsterSpecMode);
        //서버 팝업...
        ServerController.Instance.ActivePopUp(IsCheatTimeMode);

        //전투비전투 안보이기
        GameManager.Instance.ActiveStatusMsg(DebugUIDisplay);

        //치트 모드 텍스트 설정..
        UpdateCheatModeText();

        //스킬 초기화 버튼도..
        StandardTime.Instance.DailySKillreset.gameObject.SetActive(DebugUIDisplay);
        StandardTime.Instance.DailySkillSlotReset.gameObject.SetActive(DebugUIDisplay);

        PopUpSystem.Instance.RankingPopUp.DebugCacheTimeText.gameObject.SetActive(DebugUIDisplay);
        PopUpSystem.Instance.RankingPopUp.SetDebugCacheTime();
    }
#endif

    public void UpdateCheatModeText()
    {
        StringBuilder sb = new StringBuilder();
        if (debugUIDisplay)
            sb.AppendLine("CheatMode On");

        List<Hunter> equipHunter = DataManager.Instance.GetEquippedHunters();
        for (int i = 0; i < equipHunter.Count;i++)
        {
            if (equipHunter[i].TestHunterStat.isTestStat)
            {
                sb.AppendLine("HunterStat Cheat On");
                break;
            }
        }

        if (TestEnemyStat.Instance.isTest)
            sb.AppendLine("EnemyStat Cheat On");
        cheatModeText.text = sb.ToString();
    }
}
