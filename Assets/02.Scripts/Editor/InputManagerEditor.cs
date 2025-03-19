using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 단축키 관련 메소드 제작 & InputManager 관련 유니티 에디터 작업
/// </summary>
[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    static bool isTestUtilOn = true;


    // MenuItem 에트리뷰트의 이름이 중복되면 작동이 안 됨!
    // [MenuItem("CustomShortcuts/[단축키이름] [단축키]")]
    // 단축키 추가 후 CheatManager 상단에 단축키 설명 기재 필요.

    /*
       단축키 특수문자
        % : Ctrl (맥에선 Cmd)
        ^ : Ctrl
        # : Shift
        & : Alt
    */

    [MenuItem("CustomShortcuts/Enter boss %q")]
    public static void BossInStage() // 보스 입장
    {
        //유저가 현재 머무는 레벨 찾기
        int CurrentUserLevlStage = GameDataTable.Instance.User.ClearStageLevel;
        //유저의 현재 스테이지 들고오기
        var Userdata = Utill_Math.CalculateStageAndRound(CurrentUserLevlStage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = Userdata.stageindex;
        int level = Userdata.CurrentStage;
        GroundCreatSystem.Instance.SpwanBoss(userindex);
    }



    [MenuItem("CustomShortcuts/Reset Upgrade %w")]
    public static void UpgradeZero() // 강화 전부 초기화
    {

    }


    [MenuItem("CustomShortcuts/Toggle Test Utility %t")]
    public static void CheckTestUtility() // 치트 UI 켜기
    {
        CheatManager.Instance.DebugUIDisplay = !CheatManager.Instance.DebugUIDisplay;
        Selection.activeGameObject = CheatManager.Instance.gameObject;
        GameEventSystem.GameToggleCheat_GameEventHandler_Event(isTestUtilOn);
    }

    [MenuItem("CustomShortcuts/Delete Data %l")]
    public static void DeleteData() // 로컬 정보 삭제 및 재시작
    {
        StreamingReader.DeleteUserData();
        GameManager.Instance.ResetGameSquence();
        InventoryManager.Instance.Init();
    }


    [MenuItem("CustomShortcuts/All Death %g")]
    public static void AllDeath() // 전투중인 모든 캐릭터 사망
    {
        DamageInfo damageInfo = new(int.MaxValue,false,null);
        List<Hunter> hunters = DataManager.Instance.GetEquippedHunters();
        for (int i = 0; i < hunters.Count; i++)
        {
            hunters[i].Damaged(damageInfo);
        }
    }

    [MenuItem("CustomShortcuts/Current Boss Death %&g")]
    public static void CurrentBossDeath() // 스테이지 보스 사망
    {
        if (!GroundCreatSystem.Instance.isBossStage) return;

        DamageInfo damageInfo = new(int.MaxValue, false, DataManager.Instance.GetEquippedHunters()[0]);
        List<Enemy> bossList = GroundCreatSystem.Instance.Bosslist;
        for (int i = 0; i < bossList.Count; i++)
        {
            bossList[i].Damaged(damageInfo);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        var data = target as InputManager;
        data.isShowHotKey = EditorGUILayout.Foldout(data.isShowHotKey, "단축키 설정");

        if (data.isShowHotKey)
        {
            data.closeKey = (KeyCode)EditorGUILayout.EnumPopup("뒤로가기 & 종료하기", data.closeKey);
            data.speedUpKey = (KeyCode)EditorGUILayout.EnumPopup("배속 Up", data.speedUpKey);
            data.speedDownKey = (KeyCode)EditorGUILayout.EnumPopup("배속 Down", data.speedDownKey);
            data.maxResourceKey = (KeyCode)EditorGUILayout.EnumPopup("재화 최대치", data.maxResourceKey);
        }
    }
}
