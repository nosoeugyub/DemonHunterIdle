using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 맵 컨트롤러
/// </summary>
public class Ground : MonoBehaviour
{
    //그라운드에 할당한 지형 스프라이트
    [SerializeField] private MeshRenderer groundRenderer;
    public MeshRenderer GroundRenderer
    {
        get { return groundRenderer; }
        set { groundRenderer = value; }
    }

    //방치모드 휴식시 캐릭터들의 위치
    [SerializeField] private Transform[] _idleModeRestCharacterPos;
    public Transform[] IdleModeRestCharacterPos
    {
        get { return _idleModeRestCharacterPos; }
        set { _idleModeRestCharacterPos = value; }
    }
    //방치모드 휴식시 구조물
    [SerializeField] private GameObject _idleModeRestCamp;
    public GameObject IdleModeRestCamp
    {
        get { return _idleModeRestCamp; }
        set { _idleModeRestCamp = value; }
    }

    [SerializeField] private Transform _playerPos;
    public Transform PlayerPos
    {
        get { return _playerPos; }
        set { _playerPos = value; }
    }


    [SerializeField] private Transform[] _bossPos; //보스소환지점
    public Transform[] BossPos
    {
        get { return _bossPos; }
        set { _bossPos = value; }
    }


    [SerializeField] private Transform _arrivalPos; //도착지점
    public Transform ArrivalPos
    {
        get { return _arrivalPos; }
        set { _arrivalPos = value; }
    }

    [SerializeField] private bool _isarrive;
    public bool IsArriveW
    {
        get { return _isarrive; }
        set { _isarrive = value; }
    }

    [SerializeField] private Transform _monsterArea;
    public Transform MonsterArea
    {
        get { return _monsterArea; }
        set { _monsterArea = value; }
    }

    [SerializeField] private NavMeshSurface nav;
    public NavMeshSurface Nav
    {
        get { return nav; }
        set { nav = value; }
    }

    [SerializeField] private MeshRenderer plane;
    public MeshRenderer Plane
    {
        get { return plane; }
        set { plane = value; }
    }



    private List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> enemies
    {
        get { return _enemies; }
        set { _enemies = value; }
    }

    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
        GameEventSystem.GameEnemyDie_SendGameEventHandler += ClearList;

    }

    public bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence) //재시작시 필요한 리스트 초기화
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:
                ClearList();
                break;
        }
        return true;
    }
    public void ClearList()
    {
        //GroundEnemyList.Clear();
        enemies.Clear();
    }

    public void AllRemoveEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].gameObject.SetActive(false);

            ObjectPooler.ReturnToPool(enemies[i].gameObject);
        }
        enemies.Clear();
    }



    // 주어진 중심 좌표(centerPosition) 주변의 영역 안에서 중복되지 않은 좌표를 n개 생성하여 반환합니다.
    public List<Vector3> GenerateUniqueCoordinates(int _clearStage)
    {
        //데이터 가공
        int clearstage = _clearStage;
        //최대스테이인지 검사
        int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        if (maxstage <= clearstage)
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


        StageTableData stagedabledata = GameDataTable.Instance.StageTableDic[userindex];
        float n = Random.Range(stagedabledata.Count1[0], stagedabledata.Count1[1] + 1); // 랜덤한 개수를 지정합니다.
        List<Vector3> uniqueCoordinates = new List<Vector3>();
        HashSet<Vector3> uniqueCoordinateSet = new HashSet<Vector3>();

        for (int i = 0; i < n; i++)
        {
            Vector3 randomPosition = GenerateRandomPosition(MonsterArea.transform.position, stagedabledata);

            // 이미 생성된 좌표인지 확인하고, 중복되지 않은 경우에만 리스트에 추가합니다.
            if (!uniqueCoordinateSet.Contains(randomPosition))
            {
                uniqueCoordinateSet.Add(randomPosition);
                uniqueCoordinates.Add(randomPosition);
            }
        }

        return uniqueCoordinates;
    }

    // 중심 좌표(centerPosition) 주변의 영역 안에서 랜덤한 좌표를 생성하여 반환합니다.
    private Vector3 GenerateRandomPosition(Vector3 centerPosition, StageTableData stagedabledata)
    {
        float randomX = Random.Range(centerPosition.x - stagedabledata.SpawnArea[1], centerPosition.x + stagedabledata.SpawnArea[1]);
        float randomZ = Random.Range(centerPosition.z - stagedabledata.SpawnArea[0], centerPosition.z + stagedabledata.SpawnArea[0]);

        return new Vector3(randomX, 0, randomZ);
    }

    public List<Vector3> GenerateCenterPosition()
    {
        List<Vector3> tempvec = new List<Vector3>();
        for (int i = 0; i < BossPos.Length; i++)
        {
            tempvec.Add(BossPos[i].transform.position);
        }
        return tempvec;
    }

    [SerializeField] private List<Enemy> groundEnemyList;
    public List<Enemy> GroundEnemyList
    {
        get { return groundEnemyList; }
        set { groundEnemyList = value; }
    }









    public IEnumerator StartSpawn(List<Vector3> _transformlist, Ground groound, string BossMobName = "", int _clearStage = 1, bool _isboss = false)
    {
        Quaternion spawnAngle = Quaternion.Euler(0, 180, 0);
        groound.Nav.BuildNavMesh();


        //데이터 가공
        int clearstage = _clearStage;
        //최대스테이인지 검사
        int maxstage = Utill_Math.Make_Max_stage(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL].Value);
        if (maxstage < clearstage)
        {
            clearstage = maxstage;
        }
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int stageindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[stageindex].ChapterCycle;


        StageTableData currentstagedata = GameDataTable.Instance.StageTableDic[stageindex];



        string[] MonsterName = GameDataTable.Instance.StageTableDic[stageindex].MobName1;
        string BosMonsterName = GameDataTable.Instance.StageTableDic[stageindex].BossName;

        string BossMonsterMaterialName = GameDataTable.Instance.StageTableDic[stageindex].BossMaterial;
        string MonsterMaterialName = GameDataTable.Instance.StageTableDic[stageindex].MobMaterial1;

        int MonsterCount = 0;
        if (_isboss)
        {
            MonsterCount = GameDataTable.Instance.StageTableDic[stageindex].BossCount; //csv의 bossCount 만큼 생성
        }
        else
        {
            MonsterCount = _transformlist.Count;
        }
        //원래 이 아래부턴 while(true)로 감싸져 있었으나 불필요한 로직으로 판단되어 삭제함.
        //추후 문제시 검토 필요 -YE
        for (int i = 0; i < MonsterCount; i++)
        {
            Enemy tempEnemy = null;
            EnemyStat tempStat = null;
            // tempStat을 복사하여 새로운 객체 생성

            if (_isboss)//보스를 소환
            {
                GameObject tempEnemyobj = ObjectPooler.SpawnFromPool(BosMonsterName, _transformlist[0], spawnAngle);
                tempEnemy = tempEnemyobj.GetComponent<Enemy>();
                tempStat = new EnemyStat(GameDataTable.Instance.EnemyStatDic[BosMonsterName]);
                //타임바 셋팅
                int limited = (int.Parse(currentstagedata.BossTimelimited));
                tempEnemy.enemyhpuiview.Init_Timebar(limited);

                //스텟 보정값 적용
                EnemyStat.SetHp(tempStat, StageTableData.GetBossHpWeight(currentstagedata, tempStat.HP, userRound));
                EnemyStat.SetPhysicalPower(tempStat, StageTableData.GetBossPhysicalPower(currentstagedata, tempStat.PhysicalPower, userRound));
                EnemyStat.SetPhysicalDefence(tempStat, StageTableData.GetBossPhysicalDefence(currentstagedata, tempStat.PhysicalPowerDefense, userRound));
                tempEnemy.SetMaterial(BossMonsterMaterialName);
                GroundCreatSystem.Instance.Bosslist.Add(tempEnemy);
                //적스텟 표기
                GroundCreatSystem.Instance.StageClearView.UpdateEnemyInfo(tempStat, MonsterName[0], _transformlist[0], spawnAngle);
            }
            else
            {
                GameObject tempEnemyobj = ObjectPooler.SpawnFromPool(MonsterName[0], _transformlist[i], spawnAngle);
                tempEnemy = tempEnemyobj.GetComponent<Enemy>();
                tempStat = new EnemyStat(GameDataTable.Instance.EnemyStatDic[MonsterName[0]]);

                EnemyStat.SetHp(tempStat, StageTableData.GetHpWeight(currentstagedata, tempStat.HP, userRound));
                EnemyStat.SetPhysicalPower(tempStat, StageTableData.GetPhysicalPower(currentstagedata, tempStat.PhysicalPower, userRound));
                EnemyStat.SetPhysicalDefence(tempStat, StageTableData.GetPhysicalDefence(currentstagedata, tempStat.PhysicalPowerDefense, userRound));
                tempEnemy.SetMaterial(MonsterMaterialName);
                //적스텟 표기
                GroundCreatSystem.Instance.StageClearView.UpdateEnemyInfo(tempStat, MonsterName[0], _transformlist[i], spawnAngle);
            }
            enemies.Add(tempEnemy);
                 GroundEnemyList.Add(tempEnemy);

            tempEnemy.Ground = groound; //어떤 그라운드 소속인지..
            tempEnemy.Setting(tempStat);
            tempEnemy.SetNaviSurface(groound.Nav);
        }
        yield return new WaitUntil(() => enemies.Count <= 0);

        GameEventSystem.GameBattleSequence_GameEventHandler_Event(false);
    }


    public Vector3 GetPlayerSpawnPos()
    {
        Vector3 newvec3 = PlayerPos.position;
        return newvec3;
    }

    public bool CheckZeroEnemyGround()
    {
        if (GroundEnemyList.Count <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //그라운드 땅을 바꾸는 로직
    public void changeGroundSprite(string _stringname)
    {
        //문자로 텍스쳐 가져오기 
        Texture2D texture = null;
        texture = Utill_Standard.GetBgTextureSprite(_stringname);

        GroundRenderer.material.SetTexture("_BaseMap", texture);
    }
}
