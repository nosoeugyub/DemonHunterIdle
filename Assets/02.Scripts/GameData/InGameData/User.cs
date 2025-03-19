using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using static Utill_Enum;
using System;
using NSY;
using System.Linq;



public class User
{
    //저장되는 로직
    public string UserSaveForTime; //유저가 최근에 종료한 시간날짜...
    public Dictionary<string, bool[]> SkillPreset; // 유저가 구매한 각각의 캐릭터들의 프리셋 <캐릭터타입, 구매한 프리셋 배열> 구조를감 
    public string[,] HunterSkillSlotindex; //헌터 프리셋
    public string[,] GuardianSkillSlotindex; //가디언 프리셋
    public string[,] MageSkillSlotindex; //메이지 프리셋
    public string[] DailySkillSlotIndex; //요일 스킬 목록
    public bool isDailySkill;
    public ObscuredInt[] HunterLevel;
   // public ObscuredInt[] BattlePower;
    public ObscuredFloat[] HunterExp;
    public ObscuredInt[] Euipmentuserpreset; //현재유저의 프리셋
    public int UserResurrectionTime; // 유저의 부활 시간 

    public ObscuredInt[] AverageAcquisitionResource; // 0 골드 1 경험치 비접속 보상 

    //가챠 업적 보상 획득 여부
    public Dictionary<string, bool[]> ArcherGachaRewardReceived; 
    public Dictionary<string, bool[]> GuardianGachaRewardReceived; 
    public Dictionary<string, bool[]> MageGachaRewardReceived; 
    
    //총 가챠 횟수
    public Dictionary<string, int> ArcherTotalGacha; 
    public Dictionary<string, int> GuardianTotalGacha; 
    public Dictionary<string, int> MageTotalGacha; 

    // 유저 스탯 (스탯, 스탯 업그레이드 횟수)
    public List<Dictionary<string, int>> ArcherUpgradeList = new List<Dictionary<string, int>>();
    public List<Dictionary<string, int>> GuardianUpgradeList = new List<Dictionary<string, int>>();
    public List<Dictionary<string, int>> MageUpgradeList = new List<Dictionary<string, int>>();

    //헌터가 가지고있는 속성들 (총속성, 속성1, 속성2, 속성3)
    public List<ObscuredInt[]> HunterAttribute = new List<ObscuredInt[]>();




    public ObscuredInt currentHunter; // 현재 헌터 인덱스 0 아처 1수호자 2법사

    public ObscuredInt[] HunterPromotion; //헌터 승급 정보
    public ObscuredBool[] HunterPurchase; //헌터 구매 정보

    public ObscuredString[,] UserHasResources; //유저가 가지고있는 재화 갯수 (재화 타입 , 갯수 ) 식으로 저장될 예정        
    public ObscuredInt ClearStageLevel; //유저가 클리어한 스테이지 

    //현재 출전중인 헌터
    public List<SubClass> CurrentEquipHunter;

    public ObscuredInt TotalPurchase; //현재까지 과금으로 구매한 총 가격
    public ObscuredBool[] RewardReceived; //과금 업적 보상을 얼마나 받았는지

    public ObscuredInt[] DailyMissionTaskProgress; //일일임무 임무별 달성도 (DailyMissionType enum값 순서)
    public ObscuredInt[] DailyMissionClaimStatus; //일일임무 보상 획득 여부 (0 미획득 / 1~ 획득 횟수)

    //가챠 일일제한
    public ObscuredInt[] ArcherDailyGachaLimit; 
    public ObscuredInt[] GuardianDailyGachaLimit;
    public ObscuredInt[] MageDailyGachaLimit;

    public ObscuredInt IdleModeRestCycle; //현재 방치모드 피로도 게이지 양
    public ObscuredInt IdleModeRestTime; //현재 방치모드 휴식 게이지 양

    //저장이 되지않고 로컬에서만 관리할... 
    public List<SubClass> CurrentDieHunter = new(); //현재 사망한 헌터

    private int currentKillCount;  //현재 라운드 킬
    public int CurrentKillCount
    {
        get { return currentKillCount; }
        set { currentKillCount = value; }
    }

    //저장이 되지않고 로컬에서만 관리할...
    private int accumulatecurrentKillCount; //누적 킬
    public int AccumulateCurrentKillCount
    {
        get { return accumulatecurrentKillCount; }
        set { accumulatecurrentKillCount = value; }
    }
    public void RandomizeKey_UserData()
    {
        for (int i = 0; i < GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value; i++)
        {
            HunterExp[i].RandomizeCryptoKey();
            HunterLevel[i].RandomizeCryptoKey();
        }
        ClearStageLevel.RandomizeCryptoKey();
    }
    //로컬에서쓰는 재화 , 경험치
    public static int Gold;
    public static int Exp;


    //헌터가 들고있는  스킬 + 강화 구조
    public Dictionary<string , int> ArcherSkillDic; // Key as preset number
    public Dictionary<string, int> GurdianSkillDic; // Key as preset number
    public Dictionary<string, int> MageSkillDic; // Key as preset number

  

    //스킬 리스트 초기화
    internal static void Init_HunterSkillDic_Data(Dictionary<string, int> archerSkillDic, Dictionary<string, List<BaseSkillData>> archerSkillList)
    {
        // archerSkillDic이 비어있을 때만 실행
        if (archerSkillDic.Count <= 0)
        {
            // archerSkillList의 key 값들을 archerSkillDic에 저장 (key, 0 으로)
            foreach (var skillKey in archerSkillList.Keys)
            {
                if (!archerSkillDic.ContainsKey(skillKey))
                {
                    // 스킬 이름(key)을 archerSkillDic에 추가하고, 초기값 0 설정
                    archerSkillDic.Add(skillKey, 0);
                }
            }
        }
    }

    public void SetSkillNameForUpgrade(Dictionary<string, int> HunterSkillDic, string name , int level)//이름과 프리셋의 정보로 해당 스킬의 리벨을 셋팅하는함수
    {
        // 스킬 이름이 이미 HunterSkillDic에 존재하는지 확인
        if (HunterSkillDic.ContainsKey(name))
        {
            // 스킬이 존재하면 해당 스킬의 레벨을 업데이트
            HunterSkillDic[name] += level;
        }
        else
        {
            // 스킬이 존재하지 않으면 새로운 스킬과 레벨을 추가
            HunterSkillDic.Add(name, level);
        }
    }



    //데이터가 없을시 새로 유저 데이터를 새로생성하는 로직
    public void InitUserData(ObscuredInt _ClearStageLevel, ObscuredString[,] _userhasResource, ObscuredInt[] level, ObscuredFloat[] exp, List<List<Dictionary<string, int>>> _Hunter_Upgrade, List<ObscuredInt[]> _HunterAttribute, 
                            ObscuredInt[] _HunterPromotion,string[,] hunterSkillSlotindex, string[,] guardianSkillSlotindex, string[,] mageSkillSlotindex , Dictionary<string, bool[]> _SkillPreset , ObscuredInt[] _euipmentuserpreset,
                            ObscuredBool[] _hunterPurchase, ObscuredInt _selectedTabHunter, List<SubClass> currentEquipHunter , string[] _DailySkillSlotIndex , bool _isDailySkill ,string _UserSaveForTime,ObscuredInt _TotalPurchase, 
                            ObscuredBool[] _RewardReceived, ObscuredInt[] _DailyMissionTaskProgress, ObscuredInt[] _DailyMissionClaimStatus, ObscuredInt[] _ArcherDailyGachaLimit, ObscuredInt[] _GuardianDailyGachaLimit, ObscuredInt[] _MageDailyGachaLimit, 
                            Dictionary<string, bool[]> _ArcherGachaRewardReceived, Dictionary<string, int> _ArcherTotalGacha, Dictionary<string, bool[]> _GuardianGachaRewardReceived, Dictionary<string, int> _GuardianTotalGacha, 
                            Dictionary<string, bool[]> _MageGachaRewardReceived, Dictionary<string, int> _MageTotalGacha , ObscuredInt[] _AverageAcquisitionResource, ObscuredInt idleModeRestCycle, ObscuredInt idleModeRestTime ,
                            int _UserResurrectionTime , Dictionary<string , int> _HunterSkillDic , Dictionary<string, int> _GurdainSkillDic)
    {
        ClearStageLevel = _ClearStageLevel;
        UserHasResources = _userhasResource;

        //게임에 남긴 데이터...
        CurrentKillCount = 0;
        AccumulateCurrentKillCount = 0;

        HunterLevel = level;
        HunterExp = exp;

        int upgradecount =  _Hunter_Upgrade.Count;
        ArcherUpgradeList = _Hunter_Upgrade[0];
        if (2 <= upgradecount)
        {
            GuardianUpgradeList = _Hunter_Upgrade[1];
        }
        if (3 <= upgradecount)
        {
            MageUpgradeList = _Hunter_Upgrade[2];
        }


        
        
        HunterAttribute = _HunterAttribute;

        HunterPromotion = _HunterPromotion;
        
        HunterSkillSlotindex = hunterSkillSlotindex;
        GuardianSkillSlotindex = guardianSkillSlotindex;
        MageSkillSlotindex = mageSkillSlotindex;
        SkillPreset = _SkillPreset;

        Euipmentuserpreset = _euipmentuserpreset;
        
        HunterPurchase = _hunterPurchase;

        currentHunter = _selectedTabHunter;

        CurrentEquipHunter = currentEquipHunter;

        DailySkillSlotIndex = _DailySkillSlotIndex;

        isDailySkill = _isDailySkill;
        UserSaveForTime = _UserSaveForTime;

        TotalPurchase = _TotalPurchase;
        RewardReceived = _RewardReceived;
        

        DailyMissionTaskProgress = _DailyMissionTaskProgress;
        DailyMissionClaimStatus = _DailyMissionClaimStatus;
        
        ArcherDailyGachaLimit = _ArcherDailyGachaLimit;
        GuardianDailyGachaLimit = _GuardianDailyGachaLimit;
        MageDailyGachaLimit = _MageDailyGachaLimit;

        ArcherGachaRewardReceived = _ArcherGachaRewardReceived;
        GuardianGachaRewardReceived = _GuardianGachaRewardReceived;
        MageGachaRewardReceived = _MageGachaRewardReceived;
        ArcherTotalGacha = _ArcherTotalGacha;
        GuardianTotalGacha = _GuardianTotalGacha;
        MageTotalGacha = _MageTotalGacha;
        AverageAcquisitionResource = _AverageAcquisitionResource;

        IdleModeRestCycle = idleModeRestCycle;
        IdleModeRestTime = idleModeRestTime;
        UserResurrectionTime = _UserResurrectionTime;
        ArcherSkillDic = _HunterSkillDic;
        GurdianSkillDic = _GurdainSkillDic;
    }
    
    public void InitCurrentUserHunter()
    {
        currentHunter = 0;
    }
    public List<Dictionary<string, int>> GetUpgradeList(int index)
    {
        switch (index)
        {
            case 0:
                return ArcherUpgradeList;
            case 1:
                return GuardianUpgradeList;
            case 2:
                return MageUpgradeList;
            default:
                return null;
        }
    }

    public ObscuredInt[] GetAttribute(int index)
    {
        // HunterAttribute 배열이 null인지 확인 후, index가 유효한 범위 내에 있는지 확인
        if (HunterAttribute != null && index >= 0 && index < HunterAttribute.Count)
        {
            return HunterAttribute[index];
        }
        else
        {
            return null;
        }
    }
   public void PlusCurrentHunter()
    {
        currentHunter++;
        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_OPEN_NUMBER].Value;

        if (value > 1)
        {
            if (currentHunter > --value)
            {
                currentHunter = 0; // 최소 인덱스인 0으로 순환
            }
        }

    }
    public void MinusCurrentHunter()
    {
        currentHunter--;

        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_OPEN_NUMBER].Value;

        if (value > 1)
        {
            if (currentHunter < 0)
            {
                currentHunter = --value; // 최대 인덱스인 2로 순환
            }
        }
      
    }
    #region 승급
    public void Promotion()
    {
        HunterPromotion[currentHunter]++;
    }
    #endregion
    
    #region 구매
    public void SetHunterPurchase()
    {
        HunterPurchase[currentHunter] = true;
    }
    #endregion

    /// <summary>
    /// Stat Data 
    /// </summary>
    /// <param name="userDataDict"></param>
    /// <param name="_exp"></param>
    public static void InitUserStata_Data(User _userdata)
    {

    }
    #region 레벨 경험치관련 로직
    /// <summary>
    /// Level Data 
    /// </summary>
    /// <param name="userData"></param>
    /// <param name="_exp"></param>
    public static void initUser_Level_Data(User userData)
    {
        userData.HunterLevel = new ObscuredInt[] { 1, 1, 1 };
    }
    public static void initUser_Exp_Data(User userData)
    {
        userData.HunterExp = new ObscuredFloat[] { 0, 0, 0 };
    }

    //ex -> userdata.GetUser_Exp_Data(GameDatatTable.instance.userdata);
    public static float GetUser_Exp_Data(User userData,SubClass subClass)
    {
        float temp = 0;
        temp = userData.HunterLevel[(int)subClass];
        return temp;
    }

    public static void SetLevelUser_Data(User userData, int _Level, SubClass subClass)
    {
        userData.HunterLevel[(int)subClass] = _Level;
    }
    public static void SetExp(User userDataDict, float _exp, SubClass subClass)
    {
        userDataDict.HunterExp[(int)subClass] = _exp;
    }

    public static void AddExp(User userDataDict, float _exp, HunterStat userstat,SubClass subClass)
    {
        float expbuff = userstat.GetUserExpBuffValue(userstat);

        if (userDataDict != null)
        {
            float bouseexp = _exp * expbuff;
            userDataDict.HunterExp[(int)subClass] += _exp + bouseexp;
            bool ischeck = ConstraintsData.CheckMaxValue(GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_LEVEL], userDataDict.HunterLevel[(int)subClass]);
            Vector3 expTextPos = DataManager.Instance.Hunters[(int)subClass].transform.position + (2f * Utill_Standard.vector3Up);

            if (ischeck) //최대 레벨에 도달한 경우
            {
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Exp, expTextPos, 0, (int)bouseexp);
            }
            else
            {
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Exp, expTextPos, (int)_exp,(int)bouseexp);
            }

            CheckLevelUp(userDataDict, userDataDict.HunterExp[(int)subClass],subClass);
        }
        else
        {
            // 예외 처리 또는 적절한 대체 로직 추가
        }
    }

    public static void CheckLevelUp(User userData, float _exp, SubClass subClass)
    {
        float maxexp = 0;
        int level = userData.HunterLevel[(int)subClass];

        // 최대 레벨 체크
        ConstraintsData data = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_LEVEL];
        int ckeckMaxvalue = ConstraintsData.CheckMaxValueResult(data, level);
        string levelstr = ckeckMaxvalue.ToString();

        // 필요한 경험치 설정
        if (BackendManager.Instance.IsLocal == false)
        {
            maxexp = GameDataTable.Instance.RequireDataDic[levelstr].Exp;
        }
        else
        {
            maxexp = GameDataTable.Instance.RequireDataDic[level.ToString()].Exp;
        }

        if (userData != null)
        {
            while (_exp >= maxexp) // 남은 경험치가 최대 경험치보다 넘는다면 계속 레벨업
            {
                // 레벨업 사운드 재생
                SoundManager.Instance.PlayAudio("LevelUp");
                SoundManager.Instance.PlayAudio("LevelUp2");

                // 레벨업 알림 출력
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.LevelUp, GameManager.Instance.MainCam.transform.position + (Vector3.down * 5f));

                // 남은 경험치 계산
                _exp -= maxexp;
                userData.HunterLevel[(int)subClass]++;

                // 속성 포인트 지급
                int A_value = AttributeAllocationData.Get_Attribute_value(GameDataTable.Instance.AttributeAllocationData, userData.HunterLevel[(int)subClass]);
                HunterAttributeData.Plus_TotalStat(GameDataTable.Instance.User, A_value, (int)subClass);

                // 경험치 갱신
                userData.HunterExp[(int)subClass] = _exp;

                // 레벨 변경에 따른 이벤트 발생
                GameEventSystem.GameLevel_GameEventHandler_Event(userData.HunterLevel[(int)subClass]);

                // 최대 레벨이 있는지 다시 체크하고 그에 맞는 maxexp 갱신
                levelstr = userData.HunterLevel[(int)subClass].ToString();
                if (GameDataTable.Instance.RequireDataDic.ContainsKey(levelstr))
                {
                    maxexp = GameDataTable.Instance.RequireDataDic[levelstr].Exp;
                }
                else
                {
                    break; // 최대 레벨 도달 시 레벨업 종료
                }
            }

            // 최종 남은 경험치 저장
            userData.HunterExp[(int)subClass] = _exp;
        }
        else
        {
            // 예외 처리 또는 적절한 대체 로직 추가
        }
    }
    #endregion

    #region 스테이지 & 몹스터 처치관련
    public static int GetCurrentisClearStage(User userData) //현재 클리어한 스테이지.. 1을 못깻으면 1 ... 1을 꺳으면 2.. 2를 못깼으면 2...무조건현재스테이지를 깨야 다음으로 넘어감...
    {
        int temp = 0;
        temp = userData.ClearStageLevel;
        return temp;
    }

    public static int ClearStage(User userData, StageTableData stagetabledata) //스테이지를 클리어했을경우.
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = 0; // 기본값 설정
        // userindex가 StageTableDic 딕셔너리에 존재하는지 확인
        if (GameDataTable.Instance.StageTableDic.ContainsKey(userindex))
        {
            totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;
        }
        else
        {
            // 예외 처리: userindex가 유효하지 않을 경우
            Debug.LogError($"Invalid userindex: {userindex}. It does not exist in StageTableDic.");
        }
        int temp = 0;

        //깬스테이지 정리
        Utill_Math.MakeStageArray(userData.ClearStageLevel);
        stagetabledata.IsSClearStageArray[userRound - 1] = true;
        //스테이지 증가
        userData.ClearStageLevel += 1;

        //현재 처치중인 몬스터도 0 이되야함
        Init_CurrentillCount(userData);


        temp = userData.ClearStageLevel;
        return temp;
    }

    public static int ClearLevel(User userData, StageTableData stagetabledata) //스테이지를 클리어했을경우.
    {
        //데이터 가공
        int clearstage = GameDataTable.Instance.User.ClearStageLevel;
        var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
        //현재 유저의 스테이지와 라운드를 계산 
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;
        int temp = 0;



        //특정 스테이지 클리어시 뒤끝서버 로그 보내기
        if (clearstage == 12)
        {
            BackendManager.Instance.SendLogToServer("StageProgress", "진행스테이지", "10스테이지 클리어");         // 행동 유형,       로그내용:   "소제목": "내용"
        }




        //깬스테이지 정리
        Utill_Math.MakeStageArray(userData.ClearStageLevel);
        stagetabledata.IsSClearStageArray[userStage - 1] = true;
        //클리어스테이지 증가
        userData.ClearStageLevel += 1;
        //현재 처치중인 몬스터도 0 이되야함
        Init_CurrentillCount(userData);
        //누적킬도 초기화
        Init_AccumulateKillCount(userData);

        temp = userData.ClearStageLevel;
        return temp;
    }

    public static void ClearSettingStage(User userData, int stage) //스테이지를 클리어했을경우.
    {
        //stagedata bool값도 true
        userData.ClearStageLevel = stage;
    }

    public static void Init_ClearStage(User userData) //클리어 스테이지 초기화
    {
        userData.ClearStageLevel = 1;
    }
    public static void Set_ClearStage(User userData, int value)
    {
        userData.ClearStageLevel = value;
    }
    public static void Init_AllKillCount(User userData) //킬 초기화 & 누적 킬 초기화
    {
        Init_CurrentillCount(userData);
        Init_AccumulateKillCount(userData);
    }

    public static void Init_CurrentillCount(User userData) //킬 초기화 
    {
        userData.CurrentKillCount = 0;
    }
    public static void Init_AccumulateKillCount(User userData) //누적 킬 초기화
    {
        userData.AccumulateCurrentKillCount = 0;
    }

    public static bool CheckClearStage(User userData, StageTableData stageTableData)//순수한 다음 스테이지
    {
        bool isClear = false;
        int MaxGoalCount = stageTableData.GoalKillCount / stageTableData.ChapterCycle;
        if (userData.CurrentKillCount >= MaxGoalCount)
        {
            isClear = true;
            return isClear;
        }
        return isClear;
    }


    public static bool CheckClearNextLevelStage(User userData, StageTableData stageTableData) //보스를 깨고 다음 으로 넘어가야함
    {
        bool isClear = false;
        int MaxGoalCount = stageTableData.GoalKillCount;
        if (userData.AccumulateCurrentKillCount >= MaxGoalCount)
        {
            isClear = true;
            return isClear;
        }
        return isClear;
    }

    public static void SettingCurrentKillCount(User userData, int CurrentKill, int AccumulKill)
    {
        // userData.CurrentKillCount = CurrentKill;
        userData.AccumulateCurrentKillCount = AccumulKill;
    }

    public static void Plus_KillCount(User userData, StageTableData stageTableData, int value)//킬카운트 증가..
    {
        int tmep = 0;
        int StageGoalCount = 0;

        tmep = userData.CurrentKillCount;
        StageGoalCount = stageTableData.GoalKillCount / stageTableData.ChapterCycle; //현재 레벨의 스테이지 goalkillcount / stagecycle;

        if (tmep + 1 >= StageGoalCount)
        {
            tmep = StageGoalCount;
        }
        else
        {
            tmep += value;
        }

        userData.CurrentKillCount = tmep;

        //프로그래머스바에 업데이트 


        //누적킬수
        userData.AccumulateCurrentKillCount += value;
    }
    #endregion

    

    

    public static void Init_SkillData()
    {

    }



    #region 재화 관련
    //외부 정제데이터를 유저 데이터에 할당하는 함수
    public static void Init_UserCurreny(User _userData, ObscuredString[,] _data)
    {
        _userData.UserHasResources = _data;
    }

    //현재 유저가 가지고있는 데이터를 재화로 리스트로 바꿔주는 함수
    public static List<ResourceTableData> ChangResourceData(ObscuredString[,] _data)
    {
        List<ResourceTableData> ResourceModels = new List<ResourceTableData>();

        // 데이터 배열의 각 행을 반복
        for (int i = 0; i < _data.GetLength(0); i++)
        {
            // 행의 첫 번째 열에서 재화 유형 추출
            var resourceTypeStr = _data[i, 0];
            if (Enum.TryParse<Utill_Enum.Resource_Type>(resourceTypeStr, out var resourceType))
            {
                // 행의 두 번째 열에서 수량 추출
                var valueStr = _data[i, 1];
                if (float.TryParse(valueStr, out var value))
                {
                    // ResourceTableData 객체 생성 및 설정
                    if (GameDataTable.Instance.ResourceTableDic.TryGetValue(resourceType, out var resourceData))
                    {
                        resourceData.UserHasValue = (int)value;
                        ResourceModels.Add(resourceData);
                    }
                    else
                    {
                        Debug.LogWarning($"Resource data not found for type: {resourceType}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to parse value '{valueStr}' for resource type: {resourceType}");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to parse resource type '{resourceTypeStr}'");
            }
        }

        return ResourceModels;
    }

    #endregion

    #region 스킬 & 프리셋 관련
    public static bool isCheckBuySkillPreset(User userData, Utill_Enum.SubClass type,int presetindex)
    {
        switch (type)
        {
            case SubClass.Archer:
                string key = SubClass.Archer.ToString();
                bool[] buyarray = userData.SkillPreset[key];

                if (buyarray[presetindex])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case SubClass.Guardian:
                string key1 = SubClass.Guardian.ToString();
                bool[] buyarray1 = userData.SkillPreset[key1];

                if (buyarray1[presetindex])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case SubClass.Mage:
                string key2 = SubClass.Mage.ToString();
                bool[] buyarray2 = userData.SkillPreset[key2];

                if (buyarray2[presetindex])
                {
                    return true;
                }
                else
                {
                    return false;
                }


                
        }
        return false;
    }

    public static void User_ChangeSkill(User data , string[,] skillnames, Utill_Enum.SubClass type)
    {
        switch (type)
        {
            case Utill_Enum.SubClass.Archer:
                data.HunterSkillSlotindex = skillnames;
                break;
            case Utill_Enum.SubClass.Guardian:
                data.GuardianSkillSlotindex = skillnames;
                break;
            case Utill_Enum.SubClass.Mage:
                data.MageSkillSlotindex = skillnames;
                break;
        }
    }
    public static void User_RemoveSkill(User data, string skillnames, Utill_Enum.SubClass type , int Row ,int Presetindex)
    {
        switch (type)
        {
            case Utill_Enum.SubClass.Archer:
                data.HunterSkillSlotindex[Presetindex, Row] = "Empty"; // 빈 문자열로 교체
                break;
            case Utill_Enum.SubClass.Guardian:
                data.GuardianSkillSlotindex[Presetindex, Row] = "Empty"; // 빈 문자열로 교체
                break;
            case Utill_Enum.SubClass.Mage:
                data.MageSkillSlotindex[Presetindex, Row] = "Empty"; // 빈 문자열로 교체
                break;
        }
    }
    public static void User_AddSkill(User data, string skillnames, Utill_Enum.SubClass type, int Row, int Presetindex)
    {
        if (skillnames == "")
        {
            skillnames = Tag.Empty;
        }


        switch (type)
        {
            case Utill_Enum.SubClass.Archer:
                data.HunterSkillSlotindex[Presetindex, Row] = skillnames; // 빈 문자열로 교체
                break;
            case Utill_Enum.SubClass.Guardian:
                data.GuardianSkillSlotindex[Presetindex, Row] = skillnames; // 빈 문자열로 교체
                break;
            case Utill_Enum.SubClass.Mage:
                data.MageSkillSlotindex[Presetindex, Row] = skillnames; // 빈 문자열로 교체
                break;
        }
    }

    public static void Set_Presetinedex(User data , int value , Utill_Enum.SubClass type)
    {
        switch (type)
        {
            case SubClass.Archer:
                data.Euipmentuserpreset[0] = value;
                break;
            case SubClass.Guardian:
                data.Euipmentuserpreset[1] = value;
                break;
            case SubClass.Mage:
                data.Euipmentuserpreset[2] = value;
                break;
        }
    }

    //WeeklySkill
    public static void User_DailyRemoveSkill(User data, int skillindex)
    {
        data.DailySkillSlotIndex[skillindex] = Tag.Empty;
    }
    public static void User_DailyAddSkill(User data, int skillindex , string skillname)
    {
        data.DailySkillSlotIndex[skillindex] = skillname;
    }


    public static void User_DailyinitSkill(User data, List<string> stringlist)
    {
        data.DailySkillSlotIndex = stringlist.ToArray();
    }

    #endregion


    #region 부활 관련 함수들
    public static int Get_ResurrectionTime(User user)//현재 죽은 타임을 가져옴
    {
        return user.UserResurrectionTime;
    }

    public static void init_ResurrectionTime(User user)//현재 죽은 타임을 초기화 
    {
        ConstraintsData constraindata = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_PROMOTION_LEVEL];
        user.UserResurrectionTime = constraindata.Value;
    }

    public static void Set_ResurrectionTime(User user)//죽을때마다 시간 갱신
    {
        //부활시간 얻기위해 데이터 가공
        ConstraintsData constraindata = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_RESURRECTION_COEFFICIENT];
        ConstraintsData constraindatamax = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_RESURRECTION_MAX];

        int time = (int)Math.Round(user.UserResurrectionTime * constraindata._floatValue, 1);
        user.UserResurrectionTime = time;

        if (constraindatamax.Value <= user.UserResurrectionTime)
        {
            user.UserResurrectionTime = constraindatamax.Value;
        }
    }

    #endregion
}
