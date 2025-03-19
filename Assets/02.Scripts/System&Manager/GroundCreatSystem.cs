using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :맵생성 시스템
/// </summary>
public class GroundCreatSystem : MonoSingleton<GroundCreatSystem>
{
    
    public string MobName;
    public bool isBossBtnClick = false;
    public Button BossImagebtn;
    public Image BossImage;

    public  List<Ground> Groundlist = new List<Ground>();
    public List<GameObject> GroundObjlist;
    Vector3 movevec;
    public List<Hunter> Hunter;
    private int clearstage;
    public StageClearView StageClearView;
    public StageProgress StageProgress;
    public List<Enemy> Bosslist = new List<Enemy>(); //현재 필드내 소환된 보스 리스트 보기
   

    private Coroutine bossSpawnCoroutine = null;


    public bool isBossStage; //현재 보스 스테이지 중인지 판단하는 bool함수
    protected override void Awake()
    {
        base.Awake();
        movevec = new Vector3 (0, 0, 23);
        GameEventSystem.GameSequence_SendGameEventHandler += Gamestart;
        GameEventSystem.GameRestModeSequence_SendGameEventHandler += SetRestModeSequence;
        GameEventSystem.GameKillCount_SendGameEventHandler += AddKill;
        GameEventSystem.GameHunterDie_SendGameEventHandler += (()=>StartCoroutine(HunterDie()));

        //보스 버튼
        BossImagebtn.onClick.AddListener(SpwanBoss);

        GameEventSystem.GameBossSqeuce_SendGameEventHandler += SetCinematic;
    }

    private IEnumerator HunterDie()
    {
        //비전투시로 바꿈

        ResurrectionSystem.Instance.StartResurrectionSequence();
        //부활 체크..
        while (!ResurrectionSystem.Instance.IsResurrection)
        {
            yield return null;
        }

        //유저의현재 레벨 들고오기 
        int CurrentUserLevlStage = GameDataTable.Instance.User.ClearStageLevel;
        //유저의 현재 스테이지 들고오기
        var UserStage = Utill_Math.CalculateStageAndRound(CurrentUserLevlStage);
        //현재 유저의 스테이지와 라운드를 계산 
        int stageindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[stageindex].ChapterCycle;
        //스테이지와 라운드 설정
        StageProgress.Init();
        StageProgress.UpdateStageProgress(totalRound);

        //킬 재설정
        Set_Kill(stageindex, userRound);
        //누적킬도 재설정

        //기존몹들 싸그리 없엠
        GameEventSystem.GameEnemyDie_SendGameEventHandler_Event();
        //유저위치 2번째로 재설정
        for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            DataManager.Instance.Hunters[i].ResetPosition(Groundlist[1].GetPlayerSpawnPos());
        }
        

        //카메라 움직임 제한 해제
        DataManager.Instance.MainCameraMovement.UnLimitCameraPos();

        //헌터 움직임 제한 해제
        for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
            DataManager.Instance.Hunters[i].UnLimitHunterPos();
        //몹젠도다시 하게끔
        isBossBtnClick = false;
        isBossStage = false; //보스도 비활성화
        //보스전이었어도 죽었으면 시퀀스 초기화됨
        GameEventSystem.CurBossSqenunce = BossSqeunce.None; 
        //비전투 상태로 초기화
        GameStateMachine.Instance.ChangeState(new NonCombatState());

        //StartCoroutine(DataManager.Instance.Hunter.WaitHunter(GameManager.Instance.fadeDuration));
        //3번째맵에 현재 스테이지 맞는 몬스터 소환 
        //StartCoroutine(Groundlist[2].StartSpawn(Groundlist[2].GenerateUniqueCoordinates(GameDataTable.Instance.UserStatData.isClearStage), Groundlist[2]));

        
    }

    private void RebuildHunterNavSufece()
    {
        for (int i = 0; i < Hunter.Count; i++)
        {
            Hunter[i].NavmeshSufece.RemoveData();
            Hunter[i].NavmeshSufece.BuildNavMesh();
        }
    }

    private void SetCinematic(Utill_Enum.BossSqeunce bossSqenunce)//보스 스테이지 시퀀스
    {
        int _clearStageLevel = GameDataTable.Instance.User.ClearStageLevel;

        switch (bossSqenunce)
        {
            case Utill_Enum.BossSqeunce.DataSet:
                isBossBtnClick = true;
                //그라운드 위치 초기화
                Init_MapList();
                
                //기존몹들 싸그리 없엠
                GameEventSystem.GameEnemyDie_SendGameEventHandler_Event();
                //플레이어위치 고정하고 멈춤

                for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                {
                    DataManager.Instance.Hunters[i].ResetPosition();
                    DataManager.Instance.Hunters[i].StopHunter();
                }

                //보스전일때는 버튼 비활성화가 되야함
                EnenbleBoss();

                //bossStageON
                isBossStage = true;
                break;
            case Utill_Enum.BossSqeunce.Spwan:
                //보스몹소환 2번 칸에
                if(bossSpawnCoroutine != null)
                    StopCoroutine(bossSpawnCoroutine);

                bossSpawnCoroutine = StartCoroutine(Groundlist[1].StartSpawn(Groundlist[1].GenerateCenterPosition() , Groundlist[1], _clearStage: _clearStageLevel, _isboss: true));
                //카메라 움직임 제한
                DataManager.Instance.MainCameraMovement.LimitCameraPos(Groundlist[1]);
                //헌터 움직임 제한
                for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                    DataManager.Instance.Hunters[i].LimitHunterPos(Groundlist[1]);
                //보스 버튼 비활성화
                EnenbleBoss();

               
             
                break;
            case Utill_Enum.BossSqeunce.Die:
                var tmpStageData = Utill_Math.CalculateStageAndRound(GameDataTable.Instance.User.ClearStageLevel); //미리 이전 스테이지 데이터 가지고 있기(최초 클리어 알림 때문)

                //기존 코루틴 취소 
                StopCoroutine(bossSpawnCoroutine);
                //다음 레벨로 가기
                NextLevelStage();
                //보스 클리어 체크
                if (GameDataTable.Instance.StageTableDic[tmpStageData.stageindex].IsClearBoss == false)
                {
                    //보스 최초 클리어
                    GameDataTable.Instance.StageTableDic[tmpStageData.stageindex].IsClearBoss = true;
                    // 클리어 알림도 생성
                    ChatManager.Instance.SendChatBroadcast(Utill_Enum.ChatBroadcastType.Boss_FirstClear, true, ChatBroadcastStringGenerator.GenerateBossFirstClearString(tmpStageData.CurrentStage));
                }

                //몹젠도다시 하게끔
                isBossBtnClick = false;
                //카메라 움직임 제한 해제
                DataManager.Instance.MainCameraMovement.UnLimitCameraPos();

                //헌터 움직임 제한 해제
                for(int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                    DataManager.Instance.Hunters[i].UnLimitHunterPos();

                //보스버튼 Off;
                EnenbleBoss();

                //비전투 상태로 초기화
                GameStateMachine.Instance.ChangeState(new NonCombatState());
                //보스도 초기화
                isBossStage = false;

                //리스트하에서 하나 지우기
                Bosslist.RemoveAt(0);

                //맵도 바꿔줘야할듯
                string groundname = StageTableData.Get_StringGroundName();
                Groundlist[0].changeGroundSprite(groundname);
                Groundlist[1].changeGroundSprite(groundname);
                Groundlist[2].changeGroundSprite(groundname);
                break;

        }
    }


    private GameObject curCampObj = null;
    private void SetRestModeSequence(IdleModeRestCycleSequence sequence) //방치모드 피로도 휴식 시퀀스
    {
        switch (sequence)
        {
            case IdleModeRestCycleSequence.Cinematic:
                //기존몹들 싸그리 없엠
                GameEventSystem.GameEnemyDie_SendGameEventHandler_Event();

                //캠프 보이도록
                curCampObj = NextArriveGround().IdleModeRestCamp;
                curCampObj.SetActive(true);

                //카메라 움직임 제한 해제
                DataManager.Instance.MainCameraMovement.UnLimitCameraPos();

                //몹 소환 중단
                isBossBtnClick = true;
                //보스 못들어가게
                EnenbleBoss();

                break;
            case IdleModeRestCycleSequence.Start:

                break;
            case IdleModeRestCycleSequence.End:
                //그라운드 위치 초기화
                Init_MapList();

                //캠프 비활성화
                curCampObj.SetActive(false);
                curCampObj = null;

                isBossBtnClick = false;
                break;
        }
    }

    private bool Gamestart(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence) 
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:
                StopAllCoroutines(); //초기화 로직 고려..
                Init_MapList();
                //기존몹들 싸그리 없엠
                GameEventSystem.GameEnemyDie_SendGameEventHandler_Event();

                clearstage = GameDataTable.Instance.User.ClearStageLevel;

                //게임 시작했을때 스테이지 레벨에 따라 ui도변경
                GameEventSystem.Send_StageLevel_UiDraw(clearstage);

                //게임시작하면 색깔 바뀜
                for (int i = 0; i < Groundlist.Count; i++)
                {
                    string groundname = StageTableData.Get_StringGroundName();
                    Groundlist[i].changeGroundSprite(groundname);
                }


                if (clearstage > 1)//혹시 클리어스테이지가 1이상인지
                {
                    // 0 1 그라운드 도착 활성화해야함
                    Groundlist[0].IsArriveW = true;
                    Groundlist[1].IsArriveW = true;

                   //클리어한 스테이지로 스테이지간의 정리
                   Utill_Math.MakeStageArray(clearstage);

                    //기존 데이터 로드
                    CreateMap_GameStart(clearstage);

                    //스테이지 데이터 로드 
                    Set_Stage(clearstage);

                    //킬 데이터 로드
                    Set_Kill(clearstage);
                    //몬스터 소폰코루틴 작동
                    StartCoroutine(MonsterSpawnCoroutine());
                }
                else // 뉴비면 이루트를 탐
                {
                    // 0 1 그라운드 도착 활성화해야함
                    Groundlist[0].IsArriveW = true;
                    Groundlist[1].IsArriveW = true;

                    //clearstage 값
                    Utill_Math.MakeStageArray(clearstage);
                    //처음부터 시작
                    CreateMap_GameStart();
                    //스테이지 초기화 
                    Set_Stage(clearstage);
                    //킬만 초기화
                    init_Kill();
                    //몬스터 소폰코루틴 작동
                    StartCoroutine(MonsterSpawnCoroutine());
                }
                
            return true;
        }
        return true;
    }
    //게임 시작했을때 시작
    private void CreateMap_GameStart()
    {
        //네비메쉬
        RebuildHunterNavSufece();
        //스테이지 시작.. 첫 시작 몬스터는 
        StartCoroutine(Groundlist[2].StartSpawn(Groundlist[2].GenerateUniqueCoordinates(GameDataTable.Instance.User.ClearStageLevel), Groundlist[2]));
        //킬도 초기화 
    }
    //유저가 클리어한곳까지 부터 다시 새시작
    private void CreateMap_GameStart(int _clearStageLevel)
    {
        //네비메쉬
        RebuildHunterNavSufece();
        //스테이지 시작.. 첫 시작 몬스터는 
       StartCoroutine(Groundlist[2].StartSpawn(Groundlist[2].GenerateUniqueCoordinates(GameDataTable.Instance.User.ClearStageLevel), Groundlist[2], _clearStage: _clearStageLevel));
    }

    public void Update()//프로토타입을위한 임시방편
    {
        Cheat();//치트..
    }

    private IEnumerator MonsterSpawnCoroutine()
    {
        WaitForSeconds waitFrame = Utill_Standard.WaitTimehalfOne; // 적절한 대기 시간 조정

        while (true) 
        {
            if (Groundlist[2].IsArriveW == true && !isBossBtnClick)
            {
                Groundlist[0].IsArriveW = false;
                //  Groundlist[1].IsArriveW = false;

                Groundlist[0].transform.position = Groundlist[2].transform.position + movevec;
                //  GroundObjlist[1].transform.position = GroundObjlist[2].transform.position + movevec + movevec;

                //맵도 바꿔줘야할듯
                string groundname = StageTableData.Get_StringGroundName();
                Groundlist[0].changeGroundSprite(groundname);

                //네비메쉬
                RebuildHunterNavSufece();

                Utill_Math.ChangeListSwap(Groundlist);
                // Utill_Math.ChangeListSwap(GroundObjlist);


                

                //몬스터재생성
                if (MobName == string.Empty)
                {
                    int _clearStageLevel = GameDataTable.Instance.User.ClearStageLevel;
                    yield return waitFrame; // 다음 프레임까지 대기
                    StartCoroutine(Groundlist[2].StartSpawn(Groundlist[2].GenerateUniqueCoordinates(_clearStageLevel), Groundlist[2], _clearStage: _clearStageLevel));
                }
            }
            yield return null;
        }
    }
    private void Cheat()
    {
        if (CheatManager.Instance.DebugUIDisplay)
        {
            if (CheatManager.Instance.ClearStageLevel != GameDataTable.Instance.User.ClearStageLevel)
            {
                Set_Stage(CheatManager.Instance.ClearStageLevel);
                Set_Kill(CheatManager.Instance.ClearStageLevel);
            }

        }
    }

    public Transform NextArrivePos()
    {
        Transform transform = null;

        for (int i = 0; i < Groundlist.Count; i++)
        {
            if (!Groundlist[i].IsArriveW)
            {
                transform = Groundlist[i].transform;
                
                return transform;
            }
        }
        return null;
    }

    public Ground NextArriveGround()
    {
        Ground ground = null;

        for (int i = 0; i < Groundlist.Count; i++)
        {
            if (!Groundlist[i].IsArriveW)
            {
                ground = Groundlist[i];
                
                return ground;
            }
        }
        return null;
    }

    public void Init_MapList()
    {
        Groundlist[0].transform.position = Utill_Standard.Vector3Zero;
        Groundlist[1].transform.position = Utill_Standard.Vector3Zero + movevec;
        Groundlist[2].transform.position = Utill_Standard.Vector3Zero + movevec + movevec;

        //네비메쉬
        RebuildHunterNavSufece();
    }


    //이벤트 함수
    public void  ClearStage() //다음 스테이지로..
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;



        //현재머물고있는 스테이지 테이블가져오기
        StageTableData currentstagedata = GameDataTable.Instance.StageTableDic[userindex];
        
        //현재 스테이지가 최대 스테이지인지 검사 
        if (currentstagedata.ChapterCycle <= userRound)
        {
            return;
        }

        

        //스테이지 클리어 증가
        User.ClearStage(GameDataTable.Instance.User , currentstagedata);
        //현재킬은 초기화
        User.Init_CurrentillCount(GameDataTable.Instance.User);


        

        //데이터가공 22
        int clearstage2 = GameDataTable.Instance.User.ClearStageLevel;
        //게임 시작했을때 스테이지 레벨에 따라 ui도변경
        GameEventSystem.Send_StageLevel_UiDraw(clearstage2);
        //최대스테이인지 검사
        //int maxstage2 = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        //if (maxstage2 < clearstage2)
        //{
        //    clearstage2 = maxstage2;
        //}
        var UserStage2 = Utill_Math.CalculateStageAndRound(clearstage2);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex2 = UserStage2.stageindex;
        int userStage2 = UserStage2.CurrentStage;
        int userRound2 = UserStage2.CurrentRound;
        int totalRound2 = GameDataTable.Instance.StageTableDic[userindex2].ChapterCycle;
        int chapter2 = GameDataTable.Instance.StageTableDic[userindex2].ChapterZone;



        //업데이트된 데이터로 다시 로드 < 이거는 레벨자체가 올라갔을때..
        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[userStage2];
        //ui셋팅 다시
        StageClearView.UpdateKillCount(GameDataTable.Instance.User.AccumulateCurrentKillCount, stageTableData.GoalKillCount);
        //스테이지도 맞춰서 업데이트
        StageClearView.UpdateStageCount(chapter2 , userStage2, userRound2 );

        //게임 시작했을때 스테이지 레벨에 따라 ui도변경
        GameEventSystem.Send_StageLevel_UiDraw(clearstage2);
        
    }

    public void NextLevelStage()
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        //최대스테이인지 검사 최대스테이지면 다음스테이지로 넘어가지 않고 리턴
        int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        if (maxstage < clearstage)
        {
            clearstage = maxstage;
            return;
        }
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;

        //현재머물고있는 스테이지 테이블가져오기
        StageTableData currentstagedata = GameDataTable.Instance.StageTableDic[userindex];

        //스테이지 레벨 클리어 증가
        User.ClearLevel(GameDataTable.Instance.User, currentstagedata);

        
        //데이터 가공2
        int clearstage2 = GameDataTable.Instance.User.ClearStageLevel;
        //게임 시작했을때 스테이지 레벨에 따라 ui도변경
        GameEventSystem.Send_StageLevel_UiDraw(clearstage2);

        //최대스테이인지 검사
        //int maxstage2 = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        //if (maxstage2 < clearstage2)
        //{
        //    clearstage2 = maxstage2;
        //}
        var UserStage2 = Utill_Math.CalculateStageAndRound(clearstage2);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex2 = UserStage2.stageindex;
        int userStage2 = UserStage2.CurrentStage;
        int userRound2 = UserStage2.CurrentRound;
        int totalRound2 = GameDataTable.Instance.StageTableDic[userStage2].ChapterCycle;
       

        //업데이트된 데이터로 다시 로드 < 이거는 레벨자체가 올라갔을때..
        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[userindex2];
        int chapter2 = stageTableData.ChapterZone;
        //ui셋팅 다시
        StageClearView.UpdateKillCount(GameDataTable.Instance.User.AccumulateCurrentKillCount, stageTableData.GoalKillCount);
        //스테이지도 맞춰서 업데이트
        StageClearView.UpdateStageCount(chapter2 ,userStage2, userRound2);

        //ui 재대로 업데이트 
        StageProgress.Init();
        StageProgress.UpdateStageProgress(totalRound2);

        //게임 시작했을때 스테이지 레벨에 따라 ui도변경
        GameEventSystem.Send_StageLevel_UiDraw(clearstage2);
    }

    public void AddKill(EnemyStat enemyStat)//킬했을때..
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        //최대스테이인지 검사
        int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        if (maxstage < clearstage)
        {
            clearstage = maxstage;
        }
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;
        int goalkill = GameDataTable.Instance.StageTableDic[userindex].GoalKillCount;

        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[userindex];
        User.Plus_KillCount(GameDataTable.Instance.User, stageTableData, 1);//킬업데이트
        StageClearView.UpdateKillCount(GameDataTable.Instance.User.AccumulateCurrentKillCount, stageTableData.GoalKillCount);

        if(enemyStat.Class == Utill_Enum.EnemyClass.Normal)
        {
            GameDataTable.Instance.User.DailyMissionTaskProgress[(int)Utill_Enum.DailyMissionType.NomalMobKill]+=1;
            GameEventSystem.Send_DailyMission_UiDraw(DailyMissionType.NomalMobKill);
        }
        else if(enemyStat.Class == Utill_Enum.EnemyClass.EBoss)
        {
            GameDataTable.Instance.User.DailyMissionTaskProgress[(int)Utill_Enum.DailyMissionType.EBossMobKill]+=1;
            GameEventSystem.Send_DailyMission_UiDraw(DailyMissionType.EBossMobKill);
        }

        //각각 스테이지마다 라운드가 달라짐에 따라 추가 코드..
        int lastarraylength = stageTableData.IsSClearStageArray.Length - 1;
        if (User.CheckClearStage(GameDataTable.Instance.User, stageTableData) &&
                                    StageTableData.IsRoundCheck(GameDataTable.Instance.User, goalkill, userRound, totalRound) &&
                                    !StageTableData.IsBossCheck(GameDataTable.Instance.User, goalkill)) 
        {
            //스테이지 클리어를 충족하였다면?
            ClearStage();
            EnenbleBoss();
        }
        else if (User.CheckClearNextLevelStage(GameDataTable.Instance.User, stageTableData) &&
                StageTableData.IsBossCheck(GameDataTable.Instance.User, goalkill)) 
        {
            //보스 준비가됀것으로 간주 
            SetWave();
            //보스 준비완료
            SetBoss();
        }
        else
        {
            EnenbleBoss();
        }
    }

    public void SetWave()
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        //최대스테이인지 검사
        //int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        //if (maxstage < clearstage)
        //{
        //    clearstage = maxstage;
        //}
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;

        //현재머물고있는 스테이지 테이블가져오기
        StageTableData currentstagedata = GameDataTable.Instance.StageTableDic[userindex];
        int chapter = currentstagedata.ChapterZone;
        //스테이지 클리어 만 증가
        // UserData.ClearSettingStage(GameDataTable.Instance.UserStatData, userRound);
        //스테이지도 맞춰서 업데이트
        StageClearView.UpdateStageCount(chapter , userStage, userRound);
    }

    public void SetBoss()//보스이미지 활성화
    {
        if (isBossStage)
        {
            return;
        }
        BossImage.raycastTarget = true;
        BossImage.material = null;
        NotificationDotManager.Instance.ShowNotificationDot(Utill_Enum.EventNotificationDotType.CanEnterBoss);
}

    public void EnenbleBoss()//보스이미지 흑백
    {
        BossImage.raycastTarget = false;
        Utill_Standard.SetImageMaterial(BossImage, "Black");
        NotificationDotManager.Instance.HideNotificationDot(Utill_Enum.EventNotificationDotType.CanEnterBoss);
    }

    public void SpwanBoss() //보스 스폰 할때...
    {
        int bossSpawnindex = 0;
        int Maxindex = 3;
        while (bossSpawnindex < Maxindex) 
        {
            switch (bossSpawnindex) 
            {
                case 0:
                    GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.DataSet);
                    break;
                case 1:
                    GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.Cinemtic);
                    break;
                case 2:
                    GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.Spwan);
                    break;

            }
            bossSpawnindex++;
        }
    }



    public void init_AllKill()//초기화..
    {
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[clearstage];


        User.Init_AllKillCount(GameDataTable.Instance.User); //킬초기화
        StageClearView.UpdateKillCount(GameDataTable.Instance.User.AccumulateCurrentKillCount, stageTableData.GoalKillCount);
    }

    public void init_Kill()//초기화..
    {
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;

        if (clearstage < 1)
        {
            clearstage = 1;
        }

        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[clearstage];


        User.Init_CurrentillCount(GameDataTable.Instance.User); //킬초기화
        StageClearView.UpdateKillCount(GameDataTable.Instance.User.AccumulateCurrentKillCount, stageTableData.GoalKillCount);
    }

    public void Set_Kill(int level)
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        //최대스테이지 인지 검사
        //int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        //if (maxstage < clearstage)
        //{
        //    clearstage = maxstage;
        //}

        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int Userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;

       
        int totalRound = 0;
        if (GameDataTable.Instance.StageTableDic.ContainsKey(Userindex))
        {
            totalRound = GameDataTable.Instance.StageTableDic[Userindex].ChapterCycle;
        }
        else
        {
            // userStage 값이 존재하지 않을 때 처리할 로직을 여기에 추가할 수 있습니다.
            // 예를 들어, 기본값을 설정하거나 에러 메시지를 출력할 수 있습니다.
            Debbuger.Debug("Console.WriteLine(\"Error: userStage 값이 StageTableDic에 존재하지 않습니다.\");");
        }




        //해당 레벨의 데이터가져오기
        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[userStage];
        //유저의 누적 저장수 1-1 이나 1번째 는 0 마리임 ... 그래서 - 1을 해주는 것
        int currentkillcount = (stageTableData.GoalKillCount / stageTableData.ChapterCycle) * (userRound - 1 );
        //해당 데이터의 MaxGoal 셋팅해주기
        User.SettingCurrentKillCount(GameDataTable.Instance.User , 0, currentkillcount); //누적 킬수 다시 셋팅
        StageClearView.UpdateKillCount(GameDataTable.Instance.User.AccumulateCurrentKillCount, stageTableData.GoalKillCount);
    }


    public void Set_Kill(int index , int Round)
    {

        //해당 레벨의 데이터가져오기
        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[index];

        int currentkillcount = (stageTableData.GoalKillCount / stageTableData.ChapterCycle) * (Round - 1);
        int init_AccumualteCurrentKiell = (stageTableData.GoalKillCount / stageTableData.ChapterCycle) * (Round -1 );// -1 을해야지 해당 맞는 라운드셋티잉됨
        //해당 데이터의 MaxGoal 셋팅해주기
        User.SettingCurrentKillCount(GameDataTable.Instance.User, 0, currentkillcount); //누적 킬수 다시 셋팅
        StageClearView.UpdateKillCount(init_AccumualteCurrentKiell, stageTableData.GoalKillCount);
    }
    public void Init_Stage()//스테이지도 초기화
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        

        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;


        StageTableData stageTableData = GameDataTable.Instance.StageTableDic[userindex];
        User.Init_ClearStage(GameDataTable.Instance.User); //스테이지 초기화
        StageClearView.Init_Stage();
    }

    public void Set_Stage(int stageclear)
    {
        //최대스테이인지 검사
        //int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        //if (maxstage < stageclear)
        //{
        //    stageclear = maxstage;
        //}

        //데이터 가공
        var UserStage = Utill_Math.CalculateStageAndRound(stageclear);

        int totalRound = 0;
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        StageTableData stageTable;
        int chapter = UserStage.chapter;
        if (GameDataTable.Instance.StageTableDic.TryGetValue(userindex, out stageTable))
        {
            totalRound = stageTable.ChapterCycle;
        }
        //int totalRound = GameDataTable.Instance.StageTableDic[userStage].StageCycle;
        //StageTableData stageTableData = GameDataTable.Instance.StageTableDic[userStage];
        //스테이지 초기화
        User.Set_ClearStage(GameDataTable.Instance.User , stageclear);
        //스테이지 바셋팅
        StageProgress.Init();
        StageProgress.UpdateStageProgress(totalRound);

        StageClearView.UpdateStageCount(chapter , userStage, userRound);

    }


    //유저의 현재 레벨에 잇는 보스에 바로 들어가기
    public void StartBossStageInLevel(int level)
    {

    }

    public void SpwanBoss(int index) //치트용입니다  현재 유저의 레벨의 보스에게 바로가는...
    {
        //유저가 현재 머무는 레벨 찾기
        int CurrentUserLevlStage = GameDataTable.Instance.User.ClearStageLevel;
        //유저의 현재 스테이지 들고오기
        var UserStage = Utill_Math.CalculateStageAndRound(CurrentUserLevlStage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int MaxRound = GameDataTable.Instance.StageTableDic[userStage].ChapterCycle;


        //유저가 현재 위치한 레벨까지의 TotalClreaLevel을 구해야함
        int temp = Utill_Math.Make_Max_stage(userindex);

        //유저 클리어스테이지를 temp로 변환
        User.Set_ClearStage(GameDataTable.Instance.User, temp);

        int bossSpawnindex = 0;
        int Maxindex = 3;
        while (bossSpawnindex < Maxindex)
        {
            switch (bossSpawnindex)
            {
                case 0:
                    GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.DataSet);
                    break;
                case 1:
                    GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.Cinemtic);
                    break;
                case 2:
                    GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.Spwan);
                    break;

            }
            bossSpawnindex++;
        }
    }
}
