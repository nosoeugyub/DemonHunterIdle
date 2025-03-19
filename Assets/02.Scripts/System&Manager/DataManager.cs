using BackEnd;
using BackEnd.Util;
using Game.Debbug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Utill_Enum;
using CodeStage.AntiCheat.ObscuredTypes;
using System.ComponentModel.Composition.Primitives;
using Unity.VisualScripting;
using System.Linq;
using SRF;
using Newtonsoft.Json;
using LitJson;
using System.Text.RegularExpressions;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 데이터 관련 컨트롤러
/// </summary>
namespace NSY
{
    [Serializable]
    public struct LoadSheet
    {
        public CsvType type;
        public bool isLocalLoadSheet;
    }

    public class DataManager : MonoSingleton<DataManager>
    {
        public List<HunterUpGradeController> _HunterUpGradeController;
        public List<Hunter> Hunters;
        #region Hunter Func

        /// <summary>
        /// 현재 장착된 헌터 리스트 반환
        /// </summary>
        public List<Hunter> GetEquippedHunters()
        {
            List<Hunter> equippedHunters = new List<Hunter>();

            foreach (var subClass in userData.CurrentEquipHunter)
            {
                equippedHunters.Add(GetHunterUsingSubClass(subClass));
            }

            return equippedHunters;
        }

        /// <summary>
        /// 현재 특정 헌터가 장착중인지 확인
        /// </summary>
        public bool isEquipHunter(SubClass subClass)
        {
            return userData.CurrentEquipHunter.Contains(subClass);
        }
        /// <summary>
        /// SubClass를 기반으로 헌터를 찾아 반환 (추후 사용도 낮으면 삭제)
        /// </summary>
        public Hunter GetHunterUsingSubClass(SubClass subClass)
        {
            return Hunters[(int)subClass];
        }
        /// <summary>
        /// 헌터를 기반으로 SubClass를 찾아 반환 (추후 사용도 낮으면 삭제)
        /// </summary>
        public SubClass GetSubClassUsingHunter(Hunter hunter)
        {
            return (SubClass)Hunters.IndexOf(hunter);
        }
        #endregion

        [SerializeField]
        private MainCameraMovement mainCameraMovement;
        public MainCameraMovement MainCameraMovement => mainCameraMovement;

        [Header("테이블 로드 관리 (체크시 로컬로드)")]
        [SerializeField]
        private List<LoadSheet> isLocalLoadSheetList = new();
        public Dictionary<CsvType, bool> isLocalLoadSheetDict = new();

        private string game_Version = string.Empty;

        User userData = new User();
        Mail maildata = new Mail();
        HunterItem hunteritem = new HunterItem();
        Rank userRank = new Rank();
        //게임 데이터 로드 함수 이벤트 할당
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < isLocalLoadSheetList.Count; i++)
            {
                isLocalLoadSheetDict.Add(isLocalLoadSheetList[i].type, isLocalLoadSheetList[i].isLocalLoadSheet);
            }

            if (Backend.IsInitialized)
            {
                BackendManager.Instance.IsLocal = false;
                //닉네임에 테스터라는 이름이 포함되어 있다면 테스터 유저 취급
                //여기서 따로 처리하지 않아도 테스터라는 이름은 에디터에서만 생성 가능하여 이것으로 처리 가능
                if (BackendManager.myNickname.Contains("테스터"))
                {
                    BackendManager.Instance.IsEditorUser = true;
                }
                else
                {
                    BackendManager.Instance.IsEditorUser = false;
                }
            }
            else
            {
                BackendManager.Instance.IsEditorUser = false;
                BackendManager.Instance.IsLocal = true;
            }

            game_Version = Application.version;
            GameEventSystem.GameSequence_SendGameEventHandler += DataLoad;
        }

        private bool DataLoad(Utill_Enum.Enum_GameSequence GameSequence)
        {
            switch (GameSequence)
            {
                case Utill_Enum.Enum_GameSequence.DataLoad:
                    //Databinding 데이터가 들어갈곳엔 데이터를 넣어줘야함
                    GameDataTable.Instance.ResetAllTable();
                    GameDataTable.Instance.EnemyStatDic = CSVReader.EnemyStatRead("MobTable");//MobTable csv파일
                    GameDataTable.Instance.StageTableDic = CSVReader.StageTableRead("StageTable");//StageTable csv파일
                    GameDataTable.Instance.UserStatDic = CSVReader.UserStatRead("HunterTable");//csv파일
                    GameDataTable.Instance.ResourceTableDic = CSVReader.LoadResource("ResourceTable");// csv파일
                    LocalizationTable.localizationDict = CSVReader.LoadLanguageSheet("Language");//csv파일
                    GameDataTable.Instance.StatTableDataDic = CSVReader.LoadStatMaximumLimt("EntityStatTable");// csv파일
                    Hunters[0].Orginstat = GameDataTable.Instance.UserStatDic[SubClass.Archer.ToString()]; //csv파일
                    Hunters[1].Orginstat = GameDataTable.Instance.UserStatDic[SubClass.Guardian.ToString()]; //csv파일
                    Hunters[2].Orginstat = GameDataTable.Instance.UserStatDic[SubClass.Mage.ToString()]; //csv파일
                    GameDataTable.Instance.EquipmentList = CSVReader.EquipmentListRead("EquipmentList");// csv 파일
                    //GameDataTable.Instance.ColorCodeDataDic = CSVReader.LoadColorCode("TextColorCode");// csv파일
                    GameDataTable.Instance.ConstranitsDataDic = CSVReader.ConstraintsRead("Constraints"); //csv
                    GameDataTable.Instance.HunterUpgradeDataDic = CSVReader.LoadHunter_Upgrdae("Hunter_Upgrade");//csv
                    GameDataTable.Instance.HunterAttraibuteDataDic = CSVReader.LoadAttributeCompositione("Hunter_AttributeStat");//
                    GameDataTable.Instance.AttributeAllocationData = CSVReader.LoadAttributeAlloc("Hunter_AttributeAward");//                                                                                                            
                    GameDataTable.Instance.PromotionAllocationData = CSVReader.LoadPromotionRequirement("Hunter_PromotionReq");//                                                                                                            
                    GameDataTable.Instance.PromotionAbilityData = CSVReader.LoadPromotionReward("Hunter_PromotionReward");//
                    GameDataTable.Instance.DailySkillDataDic = CSVReader.LoadDailySkillData("DailySkill");//요일스킬
                    GameDataTable.Instance.ShopResoucrceGolddata_One_Dic = CSVReader.LoadGoldCell_Two_Data("ShopResource_1");//골드상점1
                    GameDataTable.Instance.ShopResoucrceGolddataDic = CSVReader.LoadGoldCell_Two_Data("ShopResource_2");//골드상점2
                    GameDataTable.Instance.PurchaseAchievementDataDic = CSVReader.LoadPurchaseAchievementData("PurchaseAchievement");//과금 업적
                    GameDataTable.Instance.GachaAchievementDataDic = CSVReader.LoadGachaAchievementData("GachaAchievement");//가챠 업적
                    GameDataTable.Instance.ItemDrawerGradeDic = CSVReader.LoaditemDarwerGradeData("ItemDrawerTable");//모루
                    GameDataTable.Instance.DailyMissionList = CSVReader.LoadDailyMissionData("DailyMission");//일일임무
                    GameDataTable.Instance.ItemGachaDataDic = CSVReader.LoadItemGachaDataTable("ItemGachaTable");//아이템 가챠 테이블
                    GameDataTable.Instance.ItemGachaMergeDataDic = CSVReader.LoadItemGachaMerge("ItemGachaMerge");//아이템 가챠 합성
                    GameDataTable.Instance.ArcherSkillList = CSVReader.ReadHunterSkill();//헌터 데이터 읽기
                    GameDataTable.Instance.GurdianSkillLiat = CSVReader.ReadGurdainSkill();//수호자 데이터 읽기

                    //CSV Data
                    if (BackendManager.Instance.IsLocal == false) // && !isLocalLoadSheetDict[CsvType.Hunter_Exp])
                    {
                        GameDataTable.Instance.RequireDataDic = CSVReader.ServerExpRead();
                    }
                    else
                    {
                        GameDataTable.Instance.RequireDataDic = CSVReader.ExpRead("Hunter_Exp");
                    }
                    //Local & Server Data
                    //스프라이트 ..로드도 같이
                    Utill_Standard.OnLoadAllSprite();
                    StreamingReader.LoadRankData();//로컬랭크시스템
                    StreamingReader.LoadHunterItemData();//로컬 헌터아이템
                    StreamingReader.LoadMailData(); //로컬메일
                    StreamingReader.LoadLocalData();//로컬 데이터
                    StreamingReader.LoadUserData(); //로컬유저데이터
                    //StreamingReader.LoadInventoryData(); //로컬인벤토리데이터

                    SpriteManager.OnLoadAllSprite();
                    //속성포인트 바인딩    
                    HunterAttributeSystem.Instance.binding_attribute(userData.HunterAttribute);

                    //재화같은경우는 바로 띄워야해서 여기서 데이터 변환을 해줌
                    _HunterUpGradeController[0]._data = GameDataTable.Instance.User.GetUpgradeList(0);
                    _HunterUpGradeController[1]._data = GameDataTable.Instance.User.GetUpgradeList(1);
                    _HunterUpGradeController[2]._data = GameDataTable.Instance.User.GetUpgradeList(2);
                    ResourceManager.Instance.ResourceModels = User.ChangResourceData(GameDataTable.Instance.User.UserHasResources);
                    ResourceManager.Instance.UpdateReousrce();
                    SkillManager.Instance.SkillUISet();

                    //일일임무 출석 체크
                    if (GameDataTable.Instance.User.DailyMissionTaskProgress[(int)Utill_Enum.DailyMissionType.DailyAttendance] <= 0)
                    {
                        GameDataTable.Instance.User.DailyMissionTaskProgress[(int)Utill_Enum.DailyMissionType.DailyAttendance]++;
                        GameEventSystem.Send_DailyMission_UiDraw(DailyMissionType.DailyAttendance);
                    }

                    mainCameraMovement.ClearCalculateHutner();

                    //게임시작할때 유저 스텟 정해주는곳
                    StartUserStatSetting();

                    //서버..랜덤스킬 지급 체크
                    ServerController.Instance.isDailySkillCheck();

                    //저장시 시스템 알림 띄우는 코루틴 시작
                    StopCoroutine(SystemNoticeCoroutine());
                    StartCoroutine(SystemNoticeCoroutine());

                    return true;

            }

            return false;
        }

        private void StartUserStatSetting() //유저가시작할때 스텟셋팅해주는곳
        {
            for (int i = 0; i < GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value; i++)
            {
                //유저강화가있으면 적용
                HunterStat.Upgrade_UserStat(DataManager.Instance.Hunters[i].Orginstat, GameDataTable.Instance.User.GetUpgradeList(i));
                //유저 속성이 강화되어있으면 적용
                HunterAttributeData.UserAttributeUpgrda(DataManager.Instance.Hunters[i].Orginstat, GameDataTable.Instance.User);

                PromotionSystem.Instance.UserAllPromotion(Hunters[i].Orginstat, i);

                //유저 장비를 체크하여 장비 착용시 스텟올려줌
                if (i == 0)
                {
                    HunterItem.Stat_UserDataEquipment(GameDataTable.Instance.HunterItem.Archer, Hunters[0].Orginstat);
                }
                else if (i == 1)
                {
                    HunterItem.Stat_UserDataEquipment(GameDataTable.Instance.HunterItem.Guardian, Hunters[1].Orginstat);
                }
                else if (i == 2)
                {
                    HunterItem.Stat_UserDataEquipment(GameDataTable.Instance.HunterItem.Mage, Hunters[2].Orginstat);
                }
            }

            for (int i = 0; i < Hunters.Count; i++)
            {
                if (!GameDataTable.Instance.User.CurrentEquipHunter.Contains((SubClass)i)) //현재 장착한 헌터인지 판단 후 온/오프 설정
                {
                    Hunters[i].gameObject.SetActive(false);
                    mainCameraMovement.RemoveCalculateHunter(Hunters[i]);
                }
                else
                {
                    Hunters[i].gameObject.SetActive(true);
                    mainCameraMovement.InitCalculateHunter(Hunters[i]);
                }
            }

        }

        /// <summary>
        /// 서버-로컬 로드 둘 다 가능한 csv를 모두 재로드
        /// </summary>
        public void LoadAllServerCsv()
        {
            foreach (var csv in isLocalLoadSheetDict)
            {
                if (BackendManager.Instance.IsLocal == false)
                {
                    switch (csv.Key)
                    {
                        case CsvType.Hunter_Exp:
                            GameDataTable.Instance.RequireDataDic = CSVReader.ServerExpRead();
                            break;
                    }

                }
                else
                {
                    switch (csv.Key)
                    {
                        case CsvType.Hunter_Exp:
                            GameDataTable.Instance.RequireDataDic = CSVReader.ExpRead("Hunter_Exp");
                            break;
                    }
                }
            }
        }

        //Local & server Load
        public void Load(string json, LitJson.JsonData jsonData, bool _isLocal, ESaveType eSaveType)
        {
            bool islocal = _isLocal;
            switch (islocal || eSaveType == ESaveType.LocalData)
            {
                case true: // Local
                    LocalLoad(json, eSaveType);
                    break;
                case false: //server
                    ServerLoad(jsonData, eSaveType);
                    break;
            }
        }
        /// <summary>
        /// 파일이 아예 존재하지 않을때 초기화
        /// </summary>
        public void InitLoad(ESaveType eSaveType)
        {
            switch (eSaveType)
            {
                case ESaveType.LocalData:
                    NotificationDotManager notificationDotManagerInst = NotificationDotManager.Instance;
                    int tmplength = Enum.GetValues(typeof(CheckableNotificationDotType)).Length;

                    notificationDotManagerInst.IscheckList = new();
                    for (int i = 0; i < tmplength; i++)
                    {
                        notificationDotManagerInst.IscheckList.Add(false);
                    }
                    break;
                case ESaveType.User:

                    #region 몇 스테이지까지 클리어 
                    ObscuredInt ClearStageLevel = 1;
                    #endregion

                    #region 유저 재화
                    string gold = GameDataTable.Instance.ResourceTableDic[Utill_Enum.Resource_Type.Gold].StartCount.ToString();
                    string dia = GameDataTable.Instance.ResourceTableDic[Utill_Enum.Resource_Type.Dia].StartCount.ToString();
                    ObscuredString[,] UserHasResource = new ObscuredString[,] { { "Gold", gold }, { "Dia", dia } };
                    #endregion;

                    #region 헌터들 전투력
                    // ObscuredInt[] BattlePower = new ObscuredInt[3] { 1000,1000,1000};
                    #endregion

                    #region 헌터들 전투력
                    ObscuredInt[] AverageAcquisitionResource = new ObscuredInt[2] { 0, 0 };
                    #endregion


                    #region 헌터 최대 레벨
                    int maxHunterVal = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value;
                    #endregion

                    #region 헌터들 레벨 배열
                    ObscuredInt[] levelArr = new ObscuredInt[maxHunterVal];
                    #endregion

                    #region 헌터들 경험치 배열
                    ObscuredFloat[] expArr = new ObscuredFloat[maxHunterVal];
                    #endregion

                    List<List<Dictionary<string, int>>> hunterUpgradeList = new();

                    List<ObscuredInt[]> hunterAttribute = new();

                    #region 헌터 승급
                    ObscuredInt[] HunterPromotion = new ObscuredInt[maxHunterVal];
                    #endregion 

                    for (int i = 0; i < maxHunterVal; i++)
                    {
                        levelArr[i] = 1;
                        expArr[i] = 0;
                        #region 유저 강화업그레이드
                        List<Dictionary<string, int>> Hunter_Upgrade = new List<Dictionary<string, int>>();

                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "PhysicalPower", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "MagicPower", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "PhysicalPowerDefense", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "HP", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "MP", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "CriChance", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "CriDamage", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "AttackSpeedPercent", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "MoveSpeedPercent", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "GoldBuff", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "ExpBuff", 0 } });
                        Hunter_Upgrade.Add(new Dictionary<string, int> { { "ItemBuff", 0 } });
                        hunterUpgradeList.Add(Hunter_Upgrade);
                        #endregion

                        #region 헌터 속성
                        ObscuredInt[] ArcherAttribute = new ObscuredInt[] { 0, 0, 0, 0 };
                        hunterAttribute.Add(ArcherAttribute);
                        #endregion
                    }

                    #region 아처 프리셋 
                    string[,] ArcherSkillSlotindex = new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };
                    #endregion

                    #region 가디언 프리셋 
                    string[,] GuardianSkillSlotindex = new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };
                    #endregion

                    #region 메이지 프리셋 
                    string[,] MageSkillSlotindex = new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };
                    #endregion


                    //유저가구매한 프리셋
                    Dictionary<string, bool[]> SkillPreset = new Dictionary<string, bool[]> { { "Archer", new bool[] { true, true, false } }, { "Guardian", new bool[] { true, true, false } }, { "Mage", new bool[] { true, true, false } } };

                    #region 현재 유저의 프리셋
                    ObscuredInt[] Euipmentuserpreset = new ObscuredInt[] { 0, 0, 0 };
                    #endregion

                    #region 헌터 구매 배열
                    ObscuredBool[] HunterPurchase = new ObscuredBool[] { true, false, false };
                    #endregion

                    #region 캐릭터 인포팝업에서 어디에 위치하는지 인덱스
                    ObscuredInt SelectedTabHunter = 0;
                    #endregion

                    #region 장착한 헌터 리스트
                    List<SubClass> currentEquipHunter = new List<SubClass>() { SubClass.Archer };
                    #endregion


                    #region 헌터 요일 스킬

                    string[] DailySkillSlotIndex = { "Empty", "Empty", "Empty", "Empty" };

                    #endregion

                    #region 헌터 요일 스킬 받은 유무
                    bool isDailySkill = false;
                    #endregion

                    #region 사용자가 저장할 시간
                    string UserSaveForTime = "";
                    #endregion

                    #region 총 과금 양
                    ObscuredInt totalPurchase = 0;
                    #endregion

                    #region 과금업적 보상 획득 여부 리스트
                    ObscuredBool[] rewardReceived = new ObscuredBool[GameDataTable.Instance.PurchaseAchievementDataDic.Count];
                    #endregion

                    #region 일일임무 임무별 달성도
                    ObscuredInt[] dailyMissionTaskProgress = new ObscuredInt[GameDataTable.Instance.DailyMissionList[0].Missions.Count];
                    #endregion
                    #region 일일임무 보상 획득 여부
                    ObscuredInt[] dailyMissionClaimStatus = new ObscuredInt[GameDataTable.Instance.DailyMissionList[0].Missions.Count];
                    #endregion

                    #region 궁수 가챠 제한 횟수
                    ObscuredInt[] archerDailyGachaLimit = new ObscuredInt[8];
                    #endregion
                    #region 수호자 가챠 제한 횟수
                    ObscuredInt[] guardianDailyGachaLimit = new ObscuredInt[8];
                    #endregion
                    #region 법사 가챠 제한 횟수
                    ObscuredInt[] mageDailyGachaLimit = new ObscuredInt[8];
                    #endregion


                    #region 가챠 업적 보상 획득 여부
                    Dictionary<string, bool[]> archerGachaRewardReceived = new();

                    bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                    archerGachaRewardReceived.Add("Pet", tmpList);
                    archerGachaRewardReceived.Add("Hat", tmpList);
                    archerGachaRewardReceived.Add("Cloak", tmpList);
                    archerGachaRewardReceived.Add("Necklace", tmpList);
                    archerGachaRewardReceived.Add("Wing", tmpList);
                    archerGachaRewardReceived.Add("Mask", tmpList);
                    archerGachaRewardReceived.Add("Back", tmpList);
                    archerGachaRewardReceived.Add("Earrings", tmpList);

                    Dictionary<string, bool[]> guardianGachaRewardReceived = new();

                    bool[] tmpList2 = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                    guardianGachaRewardReceived.Add("Pet", tmpList);
                    guardianGachaRewardReceived.Add("Hat", tmpList);
                    guardianGachaRewardReceived.Add("Cloak", tmpList);
                    guardianGachaRewardReceived.Add("Necklace", tmpList);
                    guardianGachaRewardReceived.Add("Wing", tmpList);
                    guardianGachaRewardReceived.Add("Mask", tmpList);
                    guardianGachaRewardReceived.Add("Back", tmpList);
                    guardianGachaRewardReceived.Add("Earrings", tmpList);

                    Dictionary<string, bool[]> mageGachaRewardReceived = new();

                    bool[] tmpList3 = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                    mageGachaRewardReceived.Add("Pet", tmpList);
                    mageGachaRewardReceived.Add("Hat", tmpList);
                    mageGachaRewardReceived.Add("Cloak", tmpList);
                    mageGachaRewardReceived.Add("Necklace", tmpList);
                    mageGachaRewardReceived.Add("Wing", tmpList);
                    mageGachaRewardReceived.Add("Mask", tmpList);
                    mageGachaRewardReceived.Add("Back", tmpList);
                    mageGachaRewardReceived.Add("Earrings", tmpList);

                    #endregion

                    #region 총 가챠 횟수
                    Dictionary<string, int> archerTotalGacha = new();
                    archerTotalGacha.Add("Pet", 0);
                    archerTotalGacha.Add("Hat", 0);
                    archerTotalGacha.Add("Cloak", 0);
                    archerTotalGacha.Add("Necklace", 0);
                    archerTotalGacha.Add("Wing", 0);
                    archerTotalGacha.Add("Mask", 0);
                    archerTotalGacha.Add("Back", 0);
                    archerTotalGacha.Add("Earrings", 0);

                    Dictionary<string, int> guardianTotalGacha = new();
                    guardianTotalGacha.Add("Pet", 0);
                    guardianTotalGacha.Add("Hat", 0);
                    guardianTotalGacha.Add("Cloak", 0);
                    guardianTotalGacha.Add("Necklace", 0);
                    guardianTotalGacha.Add("Wing", 0);
                    guardianTotalGacha.Add("Mask", 0);
                    guardianTotalGacha.Add("Back", 0);
                    guardianTotalGacha.Add("Earrings", 0);

                    Dictionary<string, int> mageTotalGacha = new();
                    mageTotalGacha.Add("Pet", 0);
                    mageTotalGacha.Add("Hat", 0);
                    mageTotalGacha.Add("Cloak", 0);
                    mageTotalGacha.Add("Necklace", 0);
                    mageTotalGacha.Add("Wing", 0);
                    mageTotalGacha.Add("Mask", 0);
                    mageTotalGacha.Add("Back", 0);
                    mageTotalGacha.Add("Earrings", 0);
                    #endregion

                    #region 현재 방치모드 피로도 게이지 양
                    ObscuredInt idleModeRestCycle = 0;
                    #endregion

                    #region 현재 방치모드 휴식 게이지 양
                    ObscuredInt idleModeRestTime = 0;
                    #endregion

                    #region 헌터 부활 시간
                    ConstraintsData constraindata = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_PROMOTION_LEVEL];
                    int UserResurrectionTime = constraindata.Value;
                    #endregion

                    Dictionary<string, int> HunterSkillDic = new Dictionary<string , int>();

                    Dictionary<string, int> GurdianSkillDic = new Dictionary<string, int>();
                    userData = new User();
                    userData.InitUserData(ClearStageLevel, UserHasResource, levelArr, expArr, hunterUpgradeList, hunterAttribute, HunterPromotion,
                                          ArcherSkillSlotindex, GuardianSkillSlotindex, MageSkillSlotindex, SkillPreset, Euipmentuserpreset, HunterPurchase,
                                          SelectedTabHunter, currentEquipHunter, DailySkillSlotIndex, isDailySkill, UserSaveForTime, totalPurchase, rewardReceived,
                                          dailyMissionTaskProgress, dailyMissionClaimStatus, archerDailyGachaLimit, guardianDailyGachaLimit, mageDailyGachaLimit,
                                          archerGachaRewardReceived, archerTotalGacha, guardianGachaRewardReceived, guardianTotalGacha, mageGachaRewardReceived, mageTotalGacha,
                                          AverageAcquisitionResource, idleModeRestCycle, idleModeRestTime , UserResurrectionTime , HunterSkillDic , GurdianSkillDic);
                    GameDataTable.Instance.User = userData;
                    break;
                case ESaveType.Mail:
                    maildata = new Mail();

                    List<KeyValuePair<string, int>> itemNumAndToTal = new List<KeyValuePair<string, int>>();
                    maildata.Init_MailData(itemNumAndToTal);

                    List<KeyValuePair<string, int>> itemNumAndToTal_PurchaseAchievement = new List<KeyValuePair<string, int>>();
                    maildata.Init_PurchaseAchievement_MailData(itemNumAndToTal_PurchaseAchievement);

                    List<KeyValuePair<string, int>> itemNumAndToTal_DailyMission = new List<KeyValuePair<string, int>>();
                    maildata.Init_DailyMission_MailData(itemNumAndToTal_DailyMission);

                    List<KeyValuePair<string, int>> itemNumAndToTal_Gachachievement = new List<KeyValuePair<string, int>>();
                    maildata.Init_GachaAchievement_MailData(itemNumAndToTal_Gachachievement);

                    List<KeyValuePair<string, int>> itemNumAndToTal_OfflineReward = new List<KeyValuePair<string, int>>();
                    maildata.Init_OfflineReawerd_MailData(itemNumAndToTal_OfflineReward);

                    GameDataTable.Instance.Mail = maildata;
                    break;
                case ESaveType.HunterItem:
                    hunteritem = new HunterItem();
                    //슬롯데이터부터
                    CreateDefaultHunterSlotItmeData(hunteritem.Archer, Utill_Enum.SubClass.Archer);
                    CreateDefaultHunterSlotItmeData(hunteritem.Guardian, Utill_Enum.SubClass.Guardian);
                    CreateDefaultHunterSlotItmeData(hunteritem.Mage, Utill_Enum.SubClass.Mage);
                    //초보무기 장착시킴
                    HunterItem.Add_PartItem(hunteritem.Archer, EquipmentType.Weapon, "ArcherNormalWeapon", "Normal" , true , "Normal");
                    HunterItem.Add_PartItem(hunteritem.Guardian, EquipmentType.Weapon, "GuardianNormalWeapon", "Normal" , false , "Normal");
                    //HunterItem.Add_PartItem(hunteritem.Mage, EquipmentType.Weapon, "MageNormalWeapon", "Normal");

                    GameDataTable.Instance.HunterItem = hunteritem;
                    break;
                case ESaveType.Rank:
                    double archerBattlePower = 0;
                    double GuardanBattlePower = 0;
                    double MageBattlePower = 0;

                    int ArcerLevl = 0;
                    int GuardianLvel = 0;
                    int MageLevel = 0;

                    double ClearStageLevel2 = 0;

                    userRank = new Rank(archerBattlePower, GuardanBattlePower, MageBattlePower, ArcerLevl, GuardianLvel, MageLevel, ClearStageLevel2);
                    GameDataTable.Instance.Rank = userRank;
                    break;
                default:
                    break;
            }
        }
        // Local Load 신설데이터 만들때 없을경우 반드시 추가
        //서버로드에도 작업해줬는지 확인
        public void LocalLoad(string json, ESaveType eSaveType)
        {
            var dict = Utill_Standard.JsonToDictionary(json, str => str, str => str);

            switch (eSaveType)
            {
                case ESaveType.LocalData:
                    {
                        Utility.RemoveBrackets(dict, new[] { "IsHpOn", "IsDamageOn", "IsShadowOn", "IsGrapicOn", "CurMainExpBarType" });

                        GameManager.Instance.IsHpOn = (dict.ContainsKey("IsHpOn") ? Utill_Standard.LoadStringToGeneric<bool>(dict["IsHpOn"].ToString(), new bool()) : true);
                        GameManager.Instance.IsDamageOn = (dict.ContainsKey("IsDamageOn") ? Utill_Standard.LoadStringToGeneric<bool>(dict["IsDamageOn"].ToString(), new bool()) : true);
                        GameManager.Instance.IsShadowOn = (dict.ContainsKey("IsShadowOn") ? Utill_Standard.LoadStringToGeneric<bool>(dict["IsShadowOn"].ToString(), new bool()) : true);

                        MainExpBarController.Instance.InitType(dict.ContainsKey("CurMainExpBarType") ? Utill_Standard.StringToEnum<SubClass>(dict["CurMainExpBarType"]) : SubClass.Archer);

                        NotificationDotManager notificationDotManagerInst = NotificationDotManager.Instance;
                        int tmplength = Enum.GetValues(typeof(CheckableNotificationDotType)).Length;

                        if (dict.ContainsKey("IscheckList"))
                        {
                            List<bool> tmpCheckList = Utill_Standard.JsonToList(dict["IscheckList"], str => bool.Parse(str));

                            notificationDotManagerInst.IscheckList = tmpCheckList;
                            if (tmplength < tmpCheckList.Count) //저장된 내용이 원래 Enum양보다 더 많을 경우
                            {
                                notificationDotManagerInst.IscheckList.RemoveRange(tmplength, tmpCheckList.Count - tmplength);
                            }
                            if (tmplength > tmpCheckList.Count) //저장된 내용보다 더 많은 Enum이 생겼을 경우
                            {
                                for (int i = tmpCheckList.Count; i < tmplength; i++)
                                {
                                    notificationDotManagerInst.IscheckList.Add(false);
                                }
                            }
                        }
                        else
                        {
                            notificationDotManagerInst.IscheckList = new();
                            for (int i = 0; i < tmplength; i++)
                            {
                                notificationDotManagerInst.IscheckList.Add(false);
                            }
                        }
                        break;
                    }
                case ESaveType.User:
                    {
                        Utility.RemoveBrackets(dict, new[] { "ClearStageLevel", "UserHasResource", "ArcherSkillSlotindex", "GuardianSkillSlotindex", "MageSkillSlotindex", "HunterPurchase", "SelectedTabHunter", "PurchasedHunters", "DailySkillSlotIndex", "isDailySkill", "TotalPurchase", "RewardReceived", "DailyMissionTaskProgress", "DailyMissionClaimStatus", "ArcherDailyGachaLimit", "GuardianDailyGachaLimit", "MageDailyGachaLimit", "IdleModeRestCycle", "IdleModeRestTime" });

                        Dictionary<SubClass, UserClassData> userClassDic = new();

                        #region 몇 스테이지까지 클리어 
                        ObscuredInt ClearStageLevel = dict.ContainsKey("ClearStageLevel") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(dict["ClearStageLevel"].ToString(), new ObscuredInt()) : 1; // 현재 클리어 스테이지
                        #endregion

                        #region 유저 재화
                        ObscuredString[,] _UserHasResource = dict.ContainsKey("UserHasResource") ? Utill_Standard.LoadStringToGeneric<ObscuredString[,]>(dict["UserHasResource"].ToString(), new ObscuredString[0, 0], 2) : new ObscuredString[2, 2] { { "Gold", "200" }, { "Dia", "0" } };
                        #endregion
                        Dictionary<SubClass, string> jsonUserClassDic = Utill_Standard.JsonToDictionary(dict["UserClassData"], str => Utill_Standard.StringToEnum<SubClass>(str), str => str);

                        #region 헌터 최대 레벨
                        int maxHunterVal = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value;
                        #endregion

                        #region 헌터들 레벨 배열
                        ObscuredInt[] levelArr = new ObscuredInt[maxHunterVal];
                        #endregion

                        #region 헌터들 전투력
                        //ObscuredInt[] BattlePower = dict.ContainsKey("BattlePower") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["BattlePower"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0, 0 };
                        #endregion


                        #region 헌터 오프라인 보상
                        ObscuredInt[] AverageAcquisitionResource = dict.ContainsKey("AverageAcquisitionResource") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["AverageAcquisitionResource"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0 };
                        #endregion

                        #region 헌터들 경험치 배열
                        ObscuredFloat[] expArr = new ObscuredFloat[maxHunterVal];
                        #endregion

                        List<List<Dictionary<string, int>>> hunterUpgradeList = new();

                        List<ObscuredInt[]> hunterAttribute = new();

                        ObscuredInt[] HunterPromotion = new ObscuredInt[maxHunterVal];

                        for (int i = 0; i < GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value; i++)
                        {
                            string classStr = jsonUserClassDic[(SubClass)i];
                            classStr = classStr.Remove(0, 1);
                            Dictionary<string, string> singleClassDict = Utill_Standard.JsonToDictionary(classStr, str => str, str => str);

                            Utility.RemoveBrackets(singleClassDict, new[] { "Level", "Exp", "HunterUpgrade", "HunterAttribute", "HunterPromotion" });

                            levelArr[i] = singleClassDict.ContainsKey("Level") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(singleClassDict["Level"].ToString(), new ObscuredInt()) : 1; //레벨
                            expArr[i] = singleClassDict.ContainsKey("Exp") ? Utill_Standard.LoadStringToGeneric<ObscuredFloat>(singleClassDict["Exp"].ToString(), new ObscuredFloat()) : 0; //경험치

                            List<Dictionary<string, int>> Hunter_Upgrade = new List<Dictionary<string, int>>();
                            if (singleClassDict.ContainsKey("HunterUpgrade"))
                            {
                                Hunter_Upgrade = Utill_Standard.MakeDictionaryListOfStringData(singleClassDict["HunterUpgrade"].ToString());
                            }
                            else
                            {
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "PhysicalPower", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "MagicPower", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "PhysicalPowerDefense", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "HP", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "MP", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "CriChance", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "CriDamage", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "AttackSpeedPercent", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "MoveSpeedPercent", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "GoldBuff", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "ExpBuff", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "ItemBuff", 0 } });
                            }
                            hunterUpgradeList.Add(Hunter_Upgrade);

                            #region 헌터 속성
                            hunterAttribute.Add(singleClassDict.ContainsKey("HunterAttribute") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(singleClassDict["HunterAttribute"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0, 0, 0 });
                            #endregion

                            #region 헌터 승급 얼마나 했는지
                            HunterPromotion[i] = singleClassDict.ContainsKey("HunterPromotion") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(singleClassDict["HunterPromotion"].ToString(), new ObscuredInt()) : 0;
                            #endregion
                        }

                        #region 아처 스킬 슬롯 인덱스
                        string[,] ArcherSkillSlotindex = dict.ContainsKey("ArcherSkillSlotindex") ? Utill_Standard.LoadStringToGeneric<string[,]>(dict["ArcherSkillSlotindex"].ToString(), new string[0, 0], 5)
                            : new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };
                        #endregion

                        #region 가디언 스킬 슬롯 인덱스
                        string[,] GuardianSkillSlotindex = dict.ContainsKey("GuardianSkillSlotindex") ? Utill_Standard.LoadStringToGeneric<string[,]>(dict["GuardianSkillSlotindex"].ToString(), new string[0, 0], 5)
                            : new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };
                        #endregion

                        #region 메이지 스킬 슬롯 인덱스
                        string[,] MageSkillSlotindex = dict.ContainsKey("MageSkillSlotindex") ? Utill_Standard.LoadStringToGeneric<string[,]>(dict["MageSkillSlotindex"].ToString(), new string[0, 0], 5)
                            : new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };
                        #endregion

                        //유저가 구매한 프리셋 
                        Dictionary<string, bool[]> PurchasedSkillPreset = new Dictionary<string, bool[]> { { "Archer", new bool[] { true, true, false } }, { "Guardian", new bool[] { true, true, false } }, { "Mage", new bool[] { true, true, false } } };

                        #region 현재 유저의 프리셋
                        ObscuredInt[] Euipmentuserpreset = dict.ContainsKey("EquippedSkillPreset") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["EquippedSkillPreset"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0, 0 };
                        #endregion

                        #region 헌터 구매 배열
                        ObscuredBool[] HunterPurchase = dict.ContainsKey("PurchasedHunters") ? Utill_Standard.LoadStringToGeneric<ObscuredBool[]>(dict["PurchasedHunters"].ToString(), new ObscuredBool[0]) : new ObscuredBool[] { true, false, false };
                        #endregion

                        #region 캐릭터 인포팝업에서 어디에 위치하는지 인덱스
                        ObscuredInt SelectedTabHunter = dict.ContainsKey("SelectedTabHunter") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(dict["SelectedTabHunter"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 장착한 헌터 리스트
                        List<SubClass> currentEquipHunter = dict.ContainsKey("SelectedBattleHunters") ? Utill_Standard.JsonToList(dict["SelectedBattleHunters"], str => Utill_Standard.StringToEnum<SubClass>(str)) : new List<SubClass>() { SubClass.Archer };
                        #endregion

                        #region 요일 스킬 리스트
                        string[] DailySkillSlotIndex = dict.ContainsKey("DailySkillSlotIndex") ? Utill_Standard.LoadStringToGeneric<string[]>(dict["DailySkillSlotIndex"], new string[0]) : new string[] { "Empty", "Empty", "Empty", "Empty" };

                        #endregion

                        #region 요일스킬 여부 
                        bool isDailySkill = dict.ContainsKey("isDailySkill") ? Utill_Standard.LoadStringToGeneric<bool>(dict["isDailySkill"], false) : false;
                        #endregion

                        #region 유저의 마지막 접속 시간 
                        string _UserSaveForTime = dict.ContainsKey("UserSaveForTime") ? dict["UserSaveForTime"] : BackendManager.Get_ServerTime();
                        #endregion

                        #region 총 과금 양
                        ObscuredInt totalPurchase = dict.ContainsKey("TotalPurchase") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(dict["TotalPurchase"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 과금업적 보상 획득 여부 리스트
                        ObscuredBool[] rewardReceived = dict.ContainsKey("RewardReceived") ? Utill_Standard.LoadStringToGeneric<ObscuredBool[]>(dict["RewardReceived"].ToString(), new ObscuredBool[0]) : new ObscuredBool[GameDataTable.Instance.PurchaseAchievementDataDic.Count];
                        #endregion


                        #region 일일임무 임무별 달성도
                        ObscuredInt[] dailyMissionTaskProgress = dict.ContainsKey("DailyMissionTaskProgress") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["DailyMissionTaskProgress"].ToString(), new ObscuredInt[0]) : new ObscuredInt[GameDataTable.Instance.DailyMissionList[0].Missions.Count];
                        #endregion
                        #region 일일임무 보상 획득 여부
                        ObscuredInt[] dailyMissionClaimStatus = dict.ContainsKey("DailyMissionClaimStatus") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["DailyMissionClaimStatus"].ToString(), new ObscuredInt[0]) : new ObscuredInt[GameDataTable.Instance.DailyMissionList[0].Missions.Count];
                        #endregion

                        #region 궁수 가챠 제한 횟수
                        ObscuredInt[] archerDailyGachaLimit = dict.ContainsKey("ArcherDailyGachaLimit") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["ArcherDailyGachaLimit"].ToString(), new ObscuredInt[8]) : new ObscuredInt[8];
                        #endregion
                        #region 수호자 가챠 제한 횟수
                        ObscuredInt[] guardianDailyGachaLimit = dict.ContainsKey("GuardianDailyGachaLimit") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["GuardianDailyGachaLimit"].ToString(), new ObscuredInt[8]) : new ObscuredInt[8];
                        #endregion
                        #region 법사 가챠 제한 횟수
                        ObscuredInt[] mageDailyGachaLimit = dict.ContainsKey("MageDailyGachaLimit") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(dict["MageDailyGachaLimit"].ToString(), new ObscuredInt[8]) : new ObscuredInt[8];
                        #endregion

                        #region 가챠 업적 보상 획득 여부
                        Dictionary<string, bool[]> archerGachaRewardReceived = new();

                        if (dict.ContainsKey("ArcherGachaRewardReceived"))
                        {
                            archerGachaRewardReceived = Utill_Standard.ConvertToDictionary(dict["ArcherGachaRewardReceived"]);
                        }
                        else
                        {
                            bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                            archerGachaRewardReceived.Add("Pet", tmpList);
                            archerGachaRewardReceived.Add("Hat", tmpList);
                            archerGachaRewardReceived.Add("Cloak", tmpList);
                            archerGachaRewardReceived.Add("Necklace", tmpList);
                            archerGachaRewardReceived.Add("Wing", tmpList);
                            archerGachaRewardReceived.Add("Mask", tmpList);
                            archerGachaRewardReceived.Add("Back", tmpList);
                            archerGachaRewardReceived.Add("Earrings", tmpList);
                        }
                        Dictionary<string, bool[]> guardianGachaRewardReceived = new();

                        if (dict.ContainsKey("GuardianGachaRewardReceived"))
                        {
                            guardianGachaRewardReceived = Utill_Standard.ConvertToDictionary(dict["GuardianGachaRewardReceived"]);
                        }
                        else
                        {
                            bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                            guardianGachaRewardReceived.Add("Pet", tmpList);
                            guardianGachaRewardReceived.Add("Hat", tmpList);
                            guardianGachaRewardReceived.Add("Cloak", tmpList);
                            guardianGachaRewardReceived.Add("Necklace", tmpList);
                            guardianGachaRewardReceived.Add("Wing", tmpList);
                            guardianGachaRewardReceived.Add("Mask", tmpList);
                            guardianGachaRewardReceived.Add("Back", tmpList);
                            guardianGachaRewardReceived.Add("Earrings", tmpList);
                        }
                        Dictionary<string, bool[]> mageGachaRewardReceived = new();

                        if (dict.ContainsKey("MageGachaRewardReceived"))
                        {
                            mageGachaRewardReceived = Utill_Standard.ConvertToDictionary(dict["MageGachaRewardReceived"]);
                        }
                        else
                        {
                            bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                            mageGachaRewardReceived.Add("Pet", tmpList);
                            mageGachaRewardReceived.Add("Hat", tmpList);
                            mageGachaRewardReceived.Add("Cloak", tmpList);
                            mageGachaRewardReceived.Add("Necklace", tmpList);
                            mageGachaRewardReceived.Add("Wing", tmpList);
                            mageGachaRewardReceived.Add("Mask", tmpList);
                            mageGachaRewardReceived.Add("Back", tmpList);
                            mageGachaRewardReceived.Add("Earrings", tmpList);
                        }

                        #endregion

                        #region 총 가챠 횟수

                        Dictionary<string, int> archerTotalGacha = new();
                        if (dict.ContainsKey("ArcherTotalGacha"))
                        {
                            Dictionary<string, string> tmpDict = Utill_Standard.JsonToDictionary<string, string>(dict["ArcherTotalGacha"], str => str, str => str);
                            foreach (var stringDict in tmpDict)
                            {
                                string totalGacha = stringDict.Value.Replace("{", "").Replace("}", "");
                                archerTotalGacha.Add(stringDict.Key, Utill_Standard.LoadStringToGeneric<int>(totalGacha, 0));
                            }
                        }
                        else
                        {
                            archerTotalGacha.Add("Pet", 0);
                            archerTotalGacha.Add("Hat", 0);
                            archerTotalGacha.Add("Cloak", 0);
                            archerTotalGacha.Add("Necklace", 0);
                            archerTotalGacha.Add("Wing", 0);
                            archerTotalGacha.Add("Mask", 0);
                            archerTotalGacha.Add("Back", 0);
                            archerTotalGacha.Add("Earrings", 0);
                        }

                        Dictionary<string, int> guardianTotalGacha = new();
                        if (dict.ContainsKey("GuardianTotalGacha"))
                        {
                            Dictionary<string, string> tmpDict = Utill_Standard.JsonToDictionary<string, string>(dict["GuardianTotalGacha"], str => str, str => str);
                            foreach (var stringDict in tmpDict)
                            {
                                string totalGacha = stringDict.Value.Replace("{", "").Replace("}", "");
                                guardianTotalGacha.Add(stringDict.Key, Utill_Standard.LoadStringToGeneric<int>(totalGacha, 0));
                            }
                        }
                        else
                        {
                            guardianTotalGacha.Add("Pet", 0);
                            guardianTotalGacha.Add("Hat", 0);
                            guardianTotalGacha.Add("Cloak", 0);
                            guardianTotalGacha.Add("Necklace", 0);
                            guardianTotalGacha.Add("Wing", 0);
                            guardianTotalGacha.Add("Mask", 0);
                            guardianTotalGacha.Add("Back", 0);
                            guardianTotalGacha.Add("Earrings", 0);
                        }

                        Dictionary<string, int> mageTotalGacha = new();
                        if (dict.ContainsKey("MageTotalGacha"))
                        {
                            Dictionary<string, string> tmpDict = Utill_Standard.JsonToDictionary<string, string>(dict["MageTotalGacha"], str => str, str => str);
                            foreach (var stringDict in tmpDict)
                            {
                                string totalGacha = stringDict.Value.Replace("{", "").Replace("}", "");
                                mageTotalGacha.Add(stringDict.Key, Utill_Standard.LoadStringToGeneric<int>(totalGacha, 0));
                            }
                        }
                        else
                        {
                            mageTotalGacha.Add("Pet", 0);
                            mageTotalGacha.Add("Hat", 0);
                            mageTotalGacha.Add("Cloak", 0);
                            mageTotalGacha.Add("Necklace", 0);
                            mageTotalGacha.Add("Wing", 0);
                            mageTotalGacha.Add("Mask", 0);
                            mageTotalGacha.Add("Back", 0);
                            mageTotalGacha.Add("Earrings", 0);
                        }

                        #endregion

                        #region 현재 방치모드 피로도 게이지 양
                        ObscuredInt idleModeRestCycle = dict.ContainsKey("IdleModeRestCycle") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(dict["IdleModeRestCycle"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 현재 방치모드 휴식 게이지 양
                        ObscuredInt idleModeRestTime = dict.ContainsKey("IdleModeRestTime") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(dict["IdleModeRestTime"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 현재 방치모드 휴식 게이지 양
                        ObscuredInt _UserResurrectionTime = dict.ContainsKey("UserResurrectionTime") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(dict["UserResurrectionTime"].ToString(), new ObscuredInt()) : 0;
                        #endregion

          
                        // ArcherSkillDic가 딕셔너리에 존재하는지 확인
                        string jsonString = dict.ContainsKey("ArcherSkillDic") ? dict["ArcherSkillDic"].ToString() : "[]";
                        string jsonString2 = dict.ContainsKey("GurdianSkillDic") ? dict["GurdianSkillDic"].ToString() : "[]";

                        // JSON 문자열을 Dictionary<string, int>로 변환
                        Dictionary<string , int> HunterSkillDic = Utill_Standard.ReadHunterSkillDic(jsonString);
                        Dictionary<string, int> GurdianSkillDic = Utill_Standard.ReadHunterSkillDic(jsonString2);

                        userData = new User();
                        userData.InitUserData(ClearStageLevel, _UserHasResource, levelArr, expArr, hunterUpgradeList, hunterAttribute, HunterPromotion, ArcherSkillSlotindex, GuardianSkillSlotindex, MageSkillSlotindex, PurchasedSkillPreset,
                                                Euipmentuserpreset, HunterPurchase, SelectedTabHunter, currentEquipHunter, DailySkillSlotIndex, isDailySkill, _UserSaveForTime, totalPurchase, rewardReceived, dailyMissionTaskProgress, dailyMissionClaimStatus, archerDailyGachaLimit, guardianDailyGachaLimit, mageDailyGachaLimit,
                                                archerGachaRewardReceived, archerTotalGacha, guardianGachaRewardReceived, guardianTotalGacha, mageGachaRewardReceived, mageTotalGacha, AverageAcquisitionResource, idleModeRestCycle, idleModeRestTime ,
                                                _UserResurrectionTime , HunterSkillDic , GurdianSkillDic);
                        GameDataTable.Instance.User = userData;
                        break;
                    }
                case ESaveType.Inventory:
                    {
                        Utility.RemoveBrackets(dict, new[] { "ArcherInventory", "GuardianInventory", "MageInventory" });

                        #region 인벤토리 관련
                        GameDataTable.Instance.InventoryList[0] = new List<Item>(); // Archer
                        GameDataTable.Instance.InventoryList[1] = new List<Item>(); // Guardian
                        GameDataTable.Instance.InventoryList[2] = new List<Item>(); // Mage

                        if (dict.ContainsKey("ArcherInventory"))
                        {
                            string archerInventoryString = dict["ArcherInventory"].ToString();
                            Debug.Log("Archer Inventory String: " + archerInventoryString);
                            AddItemsFromInventoryString(archerInventoryString, SubClass.Archer);
                        }

                        if (dict.ContainsKey("GuardianInventory"))
                        {
                            string guardianInventoryString = dict["GuardianInventory"].ToString();
                            Debug.Log("Guardian Inventory String: " + guardianInventoryString);
                            AddItemsFromInventoryString(guardianInventoryString, SubClass.Guardian);
                        }

                        if (dict.ContainsKey("MageInventory"))
                        {
                            string mageInventoryString = dict["MageInventory"].ToString();
                            Debug.Log("Mage Inventory String: " + mageInventoryString);
                            AddItemsFromInventoryString(mageInventoryString, SubClass.Mage);
                        }
                        #endregion
                        break;
                    }
                case ESaveType.Mail:
                    Utility.RemoveBrackets(dict, new[] { "itemNumAndToTal", "itemNumAndToTal_PurchaseAchievement", "itemNumAndToTal_DailyMission", "itemNumAndToTal_GachaAchievement", "OfflineReward" });
                    //아이템 삼품번호와 , 수량
                    List<KeyValuePair<string, int>> itemNumAndToTal;
                    List<KeyValuePair<string, int>> itemNumAndToTal_PurchaseAchievement;
                    List<KeyValuePair<string, int>> itemNumAndToTal_DailyMission;
                    List<KeyValuePair<string, int>> itemNumAndToTal_GachaAchievement;
                    List<KeyValuePair<string, int>> itemNumAndToTal_OfflineReward;

                    #region Shop1
                    if (dict.ContainsKey("itemNumAndToTal"))
                    {
                        if (dict["itemNumAndToTal"] == "")
                        {
                            itemNumAndToTal = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal = Utill_Standard.JsonToKeyValuePairList(dict["itemNumAndToTal"]);
                        }

                    }
                    else
                    {
                        itemNumAndToTal = new List<KeyValuePair<string, int>>();
                    }
                    #endregion

                    #region PurchaseAchievement
                    if (dict.ContainsKey("itemNumAndToTal_PurchaseAchievement"))
                    {
                        if (dict["itemNumAndToTal_PurchaseAchievement"] == "")
                        {
                            itemNumAndToTal_PurchaseAchievement = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_PurchaseAchievement = Utill_Standard.JsonToKeyValuePairList(dict["itemNumAndToTal_PurchaseAchievement"]);
                        }

                    }
                    else
                    {
                        itemNumAndToTal_PurchaseAchievement = new List<KeyValuePair<string, int>>();
                    }
                    #endregion

                    #region DailyMission
                    if (dict.ContainsKey("itemNumAndToTal_DailyMission"))
                    {
                        if (dict["itemNumAndToTal_DailyMission"] == "")
                        {
                            itemNumAndToTal_DailyMission = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_DailyMission = Utill_Standard.JsonToKeyValuePairList(dict["itemNumAndToTal_DailyMission"]);
                        }

                    }
                    else
                    {
                        itemNumAndToTal_DailyMission = new List<KeyValuePair<string, int>>();
                    }
                    #endregion

                    #region GachaAchievement
                    if (dict.ContainsKey("itemNumAndToTal_GachaAchievement"))
                    {
                        if (dict["itemNumAndToTal_GachaAchievement"] == "")
                        {
                            itemNumAndToTal_GachaAchievement = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_GachaAchievement = Utill_Standard.JsonToKeyValuePairList(dict["itemNumAndToTal_GachaAchievement"]);
                        }

                    }
                    else
                    {
                        itemNumAndToTal_GachaAchievement = new List<KeyValuePair<string, int>>();
                    }
                    #endregion

                    #region OfflineReward
                    if (dict.ContainsKey("OfflineReward"))
                    {
                        if (dict["OfflineReward"] == "")
                        {
                            itemNumAndToTal_OfflineReward = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_OfflineReward = Utill_Standard.JsonToKeyValuePairList(dict["OfflineReward"]);
                        }

                    }
                    else
                    {
                        itemNumAndToTal_OfflineReward = new List<KeyValuePair<string, int>>();
                    }
                    #endregion
                    maildata = new Mail();
                    maildata.Init_MailData(itemNumAndToTal);
                    maildata.Init_PurchaseAchievement_MailData(itemNumAndToTal_PurchaseAchievement);
                    maildata.Init_DailyMission_MailData(itemNumAndToTal_DailyMission);
                    maildata.Init_GachaAchievement_MailData(itemNumAndToTal_GachaAchievement);
                    maildata.Init_OfflineReawerd_MailData(itemNumAndToTal_OfflineReward);
                    GameDataTable.Instance.Mail = maildata;
                    break;
                case ESaveType.HunterItem:
                    // JSON 문자열을 파싱하여 Dictionary로 변환
                    var data = JsonConvert.DeserializeObject<Dictionary<string, List<HunteritemData>>>(json);
                    List<HunteritemData> ArcherItems = new List<HunteritemData>();
                    List<HunteritemData> GuardianItems = new List<HunteritemData>();
                    List<HunteritemData> MageItems = new List<HunteritemData>();

                    // 각 리스트 초기화
                    ArcherItems = data.ContainsKey("Archer") ? data["Archer"] : new List<HunteritemData>();
                    GuardianItems = data.ContainsKey("Guardian") ? data["Guardian"] : new List<HunteritemData>();
                    MageItems = data.ContainsKey("Mage") ? data["Mage"] : new List<HunteritemData>();

                    // Initialize hunter items with parsed data
                    hunteritem = new HunterItem();
                    hunteritem.init_HunterItem(ArcherItems, GuardianItems, MageItems);
                    GameDataTable.Instance.HunterItem = hunteritem;
                    break;
                case ESaveType.Rank:
                    // JSON 문자열을 파싱하여 Dictionary로 변환
                    Utility.RemoveBrackets(dict, new[] { "ArcherBattlePower", "GuardanBattlePower", "MageBattlePower", "ArcherLevel", "GuardianLevel", "MageLevel", "ClearStageLevel" });

                    double ArcherBattlePower = dict.ContainsKey("ArcherBattlePower") ? Utill_Standard.LoadStringToGeneric<double>(dict["ArcherBattlePower"], 0) : new double();
                    double GuardanBattlePower = dict.ContainsKey("GuardanBattlePower") ? Utill_Standard.LoadStringToGeneric<double>(dict["GuardanBattlePower"], 0) : new double();
                    double MageBattlePower = dict.ContainsKey("MageBattlePower") ? Utill_Standard.LoadStringToGeneric<double>(dict["MageBattlePower"], 0) : new double();

                    int ArcerLevl = dict.ContainsKey("ArcherLevel") ? Utill_Standard.LoadStringToGeneric<int>(dict["ArcherLevel"], 0) : new int();
                    int GuardianLvel = dict.ContainsKey("GuardianLevel") ? Utill_Standard.LoadStringToGeneric<int>(dict["GuardianLevel"], 0) : new int();
                    int MageLevel = dict.ContainsKey("MageLevel") ? Utill_Standard.LoadStringToGeneric<int>(dict["MageLevel"], 0) : new int();

                    double ClearStageLevel1 = dict.ContainsKey("ClearStageLevel") ? Utill_Standard.LoadStringToGeneric<double>(dict["ClearStageLevel"], 0) : new double();

                    userRank = new Rank(ArcherBattlePower, GuardanBattlePower, MageBattlePower, ArcerLevl, GuardianLvel, MageLevel, ClearStageLevel1);
                    GameDataTable.Instance.Rank = userRank;
                    break;
            }
        }

        private void AddItemsFromInventoryString(string inventoryString, SubClass character)
        {
            string itemPattern = "Item:";
            List<string> itemStrings = new List<string>();
            int startIndex = inventoryString.IndexOf(itemPattern);
            while (startIndex != -1)
            {
                int endIndex = inventoryString.IndexOf(itemPattern, startIndex + itemPattern.Length);
                if (endIndex == -1)
                {
                    endIndex = inventoryString.Length;
                }
                string itemString = inventoryString.Substring(startIndex, endIndex - startIndex);
                itemStrings.Add(itemString);
                startIndex = inventoryString.IndexOf(itemPattern, endIndex);
            }

            foreach (string itemString in itemStrings)
            {
                // 이름
                string itemName = GetItemPropertyValue(itemString, "Name");
                // 개수
                int itemNum = int.Parse(GetItemPropertyValue(itemString, "Num"));
                // 장착여부
                bool isEquip = bool.Parse(GetItemPropertyValue(itemString, "IsEquip"));

                // 고정 옵션 값
                string[] fixedOptions = GetItemPropertyValue(itemString, "FO").Split(',');
                List<Option> fixedOption = new List<Option>();
                foreach (string option in fixedOptions)
                {
                    fixedOption.Add(CSVReader.ParseEnum<Utill_Enum.Option>(option));
                }

                // 고정 옵션 퍼센트
                string[] fixedOptionsPercent = GetItemPropertyValue(itemString, "FOVP").Split(',');
                List<ObscuredDouble> fixedOptionValues = new List<ObscuredDouble>();
                foreach (string percent in fixedOptionsPercent)
                {
                    ObscuredDouble value = double.Parse(percent);
                    fixedOptionValues.Add(value);
                }

                // 랜덤 옵션 값
                string[] randomOptions = GetItemPropertyValue(itemString, "RO").Split(',');
                List<Option> randomOption = new List<Option>();
                foreach (string option in randomOptions)
                {
                    randomOption.Add(CSVReader.ParseEnum<Utill_Enum.Option>(option));
                }

                // 랜덤 옵션 퍼센트
                string[] randomOptionsPercent = GetItemPropertyValue(itemString, "ROVP").Split(',');
                List<ObscuredDouble> randomOptionValues = new List<ObscuredDouble>();
                foreach (string percent in randomOptionsPercent)
                {
                    ObscuredDouble value = double.Parse(percent);
                    randomOptionValues.Add(value);
                }

                // 받아온 정보를 바탕으로 아이템 생성
                Item item = new Item(itemName, itemNum, fixedOption, fixedOptionValues, randomOption, randomOptionValues, isEquip);

                // 아이템을 각 헌터의 인벤토리에 추가
                if (character == SubClass.Archer)
                {
                    GameDataTable.Instance.InventoryList[0].Add(item);
                }
                else if (character == SubClass.Guardian)
                {
                    GameDataTable.Instance.InventoryList[1].Add(item);
                }
                else if (character == SubClass.Mage)
                {
                    GameDataTable.Instance.InventoryList[2].Add(item);
                }

                InventoryManager.Instance.AddItem(item, true);
            }
        }

        // 아이템 문자열에서 특정 속성의 값을 가져오는 메서드
        private string GetItemPropertyValue(string itemString, string propertyName)
        {
            int propertyStartIndex = itemString.IndexOf(propertyName + ":");
            if (propertyStartIndex != -1)
            {
                int propertyValueStartIndex = propertyStartIndex + propertyName.Length + 1; // ":" 뒤의 값의 시작 인덱스
                int propertyValueEndIndex = itemString.IndexOf(",", propertyValueStartIndex); // 값의 끝 인덱스

                if (propertyName == "FO")
                {
                    propertyValueEndIndex = itemString.IndexOf("FOVP:", propertyValueStartIndex) - 1;
                }
                else if (propertyName == "FOVP")
                {
                    propertyValueEndIndex = itemString.IndexOf("RO:", propertyValueStartIndex) - 1;
                }
                else if (propertyName == "RO")
                {
                    propertyValueEndIndex = itemString.IndexOf("ROVP:", propertyValueStartIndex) - 1;
                }
                else if (propertyName == "ROVP")
                {
                    propertyValueEndIndex = itemString.IndexOf("Num:", propertyValueStartIndex) - 1;
                }

                else // 그 외의 속성의 경우
                {
                    propertyValueEndIndex = itemString.IndexOf(",", propertyValueStartIndex);
                }

                // 값의 끝 인덱스를 찾지 못했다면 마지막까지의 값을 가져와야 합니다.
                if (propertyValueEndIndex == -1)
                {
                    propertyValueEndIndex = itemString.Length;
                }

                string propertyValue = itemString.Substring(propertyValueStartIndex, propertyValueEndIndex - propertyValueStartIndex);

                propertyValue = propertyValue.Trim(' ', ',', '{', '}');

                return propertyValue;
            }
            return "";
        }

        //server Load  신설데이터 만들때 없을경우 반드시 추가
        //로컬로드에도 작업해줬는지 확인
        public void ServerLoad(LitJson.JsonData json, ESaveType eSaveType)
        {
            switch (eSaveType)
            {
                case ESaveType.User:
                    {
                        Dictionary<SubClass, UserClassData> userClassDic = new();

                        ObscuredInt ClearStageLevel = json.ContainsKey("ClearStageLevel") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(json["ClearStageLevel"]["N"].ToString(), new ObscuredInt()) : 1; // 현재 클리어 스테이지 
                        ObscuredString[,] _UserHasResource = json.ContainsKey("UserHasResource") ? Utill_Standard.LoadStringToGeneric<ObscuredString[,]>(json["UserHasResource"]["S"].ToString(), new ObscuredString[0, 0], 2) : new ObscuredString[,] { { "Gold", "0" }, { "Dia", "0" } };

                        Dictionary<SubClass, string> jsonUserClassDic;

                        if (json.ContainsKey("UserClassData"))
                        {
                            string userClassDataStr = json["UserClassData"]["S"].ToJson();
                            jsonUserClassDic = Utill_Standard.JsonToDictionary(userClassDataStr, str => Utill_Standard.StringToEnum<SubClass>(str), str => str);
                        }
                        else
                        {
                            jsonUserClassDic = new Dictionary<SubClass, string>();
                            jsonUserClassDic.Add(SubClass.Archer, "1");
                            jsonUserClassDic.Add(SubClass.Guardian, "1");
                            jsonUserClassDic.Add(SubClass.Mage, "1");
                        }

                        int maxHunterVal = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value;
                        ObscuredInt[] levelArr = new ObscuredInt[maxHunterVal];
                        ObscuredFloat[] expArr = new ObscuredFloat[maxHunterVal];
                        List<List<Dictionary<string, int>>> hunterUpgradeList = new();
                        List<ObscuredInt[]> hunterAttribute = new();
                        ObscuredInt[] HunterPromotion = new ObscuredInt[maxHunterVal];


                        ObscuredInt[] BattlePower = json.ContainsKey("BattlePower") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["BattlePower"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0, 0 };


                        ObscuredInt[] AverageAcquisitionResource = json.ContainsKey("AverageAcquisitionResource") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["AverageAcquisitionResource"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0 };


                        for (int i = 0; i < GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value; i++)
                        {
                            SubClass subclass = (SubClass)i;
                            string classStr = jsonUserClassDic[(SubClass)i];
                            classStr = classStr.Remove(0, 1);

                            Dictionary<string, string> singleClassDict = Utill_Standard.JsonToDictionary(classStr, str => str, str => str);

                            levelArr[i] = singleClassDict.ContainsKey("Level") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(singleClassDict["Level"].ToString(), new ObscuredInt()) : 1; //레벨

                            expArr[i] = singleClassDict.ContainsKey("Exp") ? Utill_Standard.LoadStringToGeneric<ObscuredFloat>(singleClassDict["Exp"].ToString(), new ObscuredFloat()) : 0; //경험치

                            List<Dictionary<string, int>> Hunter_Upgrade = new List<Dictionary<string, int>>();
                            if (singleClassDict.ContainsKey("HunterUpgrade"))
                            {
                                Hunter_Upgrade = Utill_Standard.MakeDictionaryListOfStringData(singleClassDict["HunterUpgrade"]);
                            }
                            else
                            {
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "PhysicalPower", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "MagicPower", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "PhysicalPowerDefense", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "HP", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "MP", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "CriChance", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "CriDamage", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "AttackSpeedPercent", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "MoveSpeedPercent", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "GoldBuff", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "ExpBuff", 0 } });
                                Hunter_Upgrade.Add(new Dictionary<string, int> { { "ItemBuff", 0 } });
                            }
                            hunterUpgradeList.Add(Hunter_Upgrade);
                            hunterAttribute.Add(singleClassDict.ContainsKey("HunterAttribute") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(singleClassDict["HunterAttribute"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0, 0, 0 });
                            HunterPromotion[i] = singleClassDict.ContainsKey("HunterPromotion") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(singleClassDict["HunterPromotion"], new ObscuredInt()) : 0;
                        }

                        string[,] ArcherSkillSlotindex = json.ContainsKey("ArcherSkillSlotindex") ? Utill_Standard.LoadStringToGeneric<string[,]>(json["ArcherSkillSlotindex"]["S"].ToString(), new string[0, 0], 5)
                          : new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };

                        string[,] GuardianSkillSlotindex = json.ContainsKey("GuardianSkillSlotindex") ? Utill_Standard.LoadStringToGeneric<string[,]>(json["GuardianSkillSlotindex"]["S"].ToString(), new string[0, 0], 5)
                          : new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };

                        string[,] MageSkillSlotindex = json.ContainsKey("MageSkillSlotindex") ? Utill_Standard.LoadStringToGeneric<string[,]>(json["MageSkillSlotindex"]["S"].ToString(), new string[0, 0], 5)
                          : new string[,] { { "0", "Empty", "Empty", "Empty", "Empty" }, { "1", "Empty", "Empty", "Empty", "Empty" }, { "2", "Empty", "Empty", "Empty", "Empty" } };


                        //유저가 구매한 프리셋 
                        Dictionary<string, bool[]> PurchasedSkillPreset = new Dictionary<string, bool[]>
                         {
                            { "Archer", new bool[] { true, true, false } },
                            { "Guardian", new bool[] { true, true, false } },
                            { "Mage", new bool[] { true, true, false } }
                        };

                        //유저가 가지고있는프리셋
                        ObscuredInt[] Euipmentuserpreset = json.ContainsKey("EquippedSkillPreset") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["EquippedSkillPreset"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[] { 0, 0, 0 };
                        ObscuredBool[] HunterPurchase = json.ContainsKey("PurchasedHunters") ? Utill_Standard.LoadStringToGeneric<ObscuredBool[]>(json["PurchasedHunters"]["S"].ToString(), new ObscuredBool[0]) : new ObscuredBool[] { true, false, false };
                        ObscuredInt CurrentHunter = json.ContainsKey("SelectedTabHunter") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(json["SelectedTabHunter"]["N"].ToString(), new ObscuredInt()) : 0;

                        List<SubClass> currentEquipHunter;

                        if (json.ContainsKey("SelectedBattleHunters"))
                        {
                            string currentEquipHunterStr = json["SelectedBattleHunters"]["S"].ToJson();
                            currentEquipHunter = Utill_Standard.JsonToList(currentEquipHunterStr, str => Utill_Standard.StringToEnum<SubClass>(str));
                        }
                        else
                        {
                            currentEquipHunter = new List<SubClass>() { SubClass.Archer };
                        }
                        string[] DailySkillSlotIndex = json.ContainsKey("DailySkillSlotIndex") && json["DailySkillSlotIndex"].ContainsKey("S")
                            ? Utill_Standard.LoadStringToGeneric<string[]>(json["DailySkillSlotIndex"]["S"].ToString(), new string[] { "Empty", "Empty", "Empty", "Empty" })
                            : new string[] { "Empty", "Empty", "Empty", "Empty" };

                        bool isDailySkill = json.ContainsKey("isDailySkill") ? Utill_Standard.LoadStringToGeneric<bool>(json["isDailySkill"]["S"].ToString(), new bool())
                            : false;

                        string _UserSaveForTime = json.ContainsKey("UserSaveForTime") ? json["UserSaveForTime"]["S"].ToString() : string.Empty;


                        #region 총 과금 양
                        ObscuredInt totalPurchase = json.ContainsKey("TotalPurchase") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(json["TotalPurchase"]["N"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 과금업적 보상 획득 여부 리스트
                        ObscuredBool[] rewardReceived = json.ContainsKey("RewardReceived") ? Utill_Standard.LoadStringToGeneric<ObscuredBool[]>(json["RewardReceived"]["S"].ToString(), new ObscuredBool[0]) : new ObscuredBool[GameDataTable.Instance.PurchaseAchievementDataDic.Count];
                        #endregion

                        #region 일일임무 임무별 달성도
                        ObscuredInt[] dailyMissionTaskProgress = json.ContainsKey("DailyMissionTaskProgress") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["DailyMissionTaskProgress"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[GameDataTable.Instance.DailyMissionList[0].Missions.Count];
                        #endregion
                        #region 일일임무 보상 획득 여부
                        ObscuredInt[] dailyMissionClaimStatus = json.ContainsKey("DailyMissionClaimStatus") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["DailyMissionClaimStatus"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[GameDataTable.Instance.DailyMissionList[0].Missions.Count];
                        #endregion

                        #region 궁수 가챠 제한 횟수
                        ObscuredInt[] archerDailyGachaLimit = json.ContainsKey("ArcherDailyGachaLimit") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["ArcherDailyGachaLimit"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[8];
                        #endregion
                        #region 수호자 가챠 제한 횟수
                        ObscuredInt[] guardianDailyGachaLimit = json.ContainsKey("GuardianDailyGachaLimit") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["GuardianDailyGachaLimit"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[8];
                        #endregion
                        #region 법사 가챠 제한 횟수
                        ObscuredInt[] mageDailyGachaLimit = json.ContainsKey("MageDailyGachaLimit") ? Utill_Standard.LoadStringToGeneric<ObscuredInt[]>(json["MageDailyGachaLimit"]["S"].ToString(), new ObscuredInt[0]) : new ObscuredInt[8];
                        #endregion

                        #region 가챠 업적 보상 획득 여부
                        Dictionary<string, bool[]> archerGachaRewardReceived = new();

                        if (json.ContainsKey("ArcherGachaRewardReceived"))
                        {
                            archerGachaRewardReceived = Utill_Standard.ConvertToDictionary(json["ArcherGachaRewardReceived"]["S"].ToString());
                        }
                        else
                        {
                            bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                            archerGachaRewardReceived.Add("Pet", tmpList);
                            archerGachaRewardReceived.Add("Hat", tmpList);
                            archerGachaRewardReceived.Add("Cloak", tmpList);
                            archerGachaRewardReceived.Add("Necklace", tmpList);
                            archerGachaRewardReceived.Add("Wing", tmpList);
                            archerGachaRewardReceived.Add("Mask", tmpList);
                            archerGachaRewardReceived.Add("Back", tmpList);
                            archerGachaRewardReceived.Add("Earrings", tmpList);
                        }
                        Dictionary<string, bool[]> guardianGachaRewardReceived = new();

                        if (json.ContainsKey("GuardianGachaRewardReceived"))
                        {
                            guardianGachaRewardReceived = Utill_Standard.ConvertToDictionary(json["GuardianGachaRewardReceived"]["S"].ToString());
                        }
                        else
                        {
                            bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                            guardianGachaRewardReceived.Add("Pet", tmpList);
                            guardianGachaRewardReceived.Add("Hat", tmpList);
                            guardianGachaRewardReceived.Add("Cloak", tmpList);
                            guardianGachaRewardReceived.Add("Necklace", tmpList);
                            guardianGachaRewardReceived.Add("Wing", tmpList);
                            guardianGachaRewardReceived.Add("Mask", tmpList);
                            guardianGachaRewardReceived.Add("Back", tmpList);
                            guardianGachaRewardReceived.Add("Earrings", tmpList);
                        }
                        Dictionary<string, bool[]> mageGachaRewardReceived = new();

                        if (json.ContainsKey("MageGachaRewardReceived"))
                        {
                            mageGachaRewardReceived = Utill_Standard.ConvertToDictionary(json["GuardianGachaRewardReceived"]["S"].ToString());
                        }
                        else
                        {
                            bool[] tmpList = new bool[GameDataTable.Instance.GachaAchievementDataDic.Count];
                            mageGachaRewardReceived.Add("Pet", tmpList);
                            mageGachaRewardReceived.Add("Hat", tmpList);
                            mageGachaRewardReceived.Add("Cloak", tmpList);
                            mageGachaRewardReceived.Add("Necklace", tmpList);
                            mageGachaRewardReceived.Add("Wing", tmpList);
                            mageGachaRewardReceived.Add("Mask", tmpList);
                            mageGachaRewardReceived.Add("Back", tmpList);
                            mageGachaRewardReceived.Add("Earrings", tmpList);
                        }

                        #endregion

                        #region 총 가챠 횟수

                        Dictionary<string, int> archerTotalGacha = new();
                        if (json.ContainsKey("ArcherTotalGacha"))
                        {
                            archerTotalGacha = Utill_Standard.JsonToDictionary(json["ArcherTotalGacha"]["S"].ToString(), str => str, str => int.Parse(str));
                        }
                        else
                        {
                            archerTotalGacha.Add("Pet", 0);
                            archerTotalGacha.Add("Hat", 0);
                            archerTotalGacha.Add("Cloak", 0);
                            archerTotalGacha.Add("Necklace", 0);
                            archerTotalGacha.Add("Wing", 0);
                            archerTotalGacha.Add("Mask", 0);
                            archerTotalGacha.Add("Back", 0);
                            archerTotalGacha.Add("Earrings", 0);
                        }

                        Dictionary<string, int> guardianTotalGacha = new();
                        if (json.ContainsKey("GuardianTotalGacha"))
                        {
                            guardianTotalGacha = Utill_Standard.JsonToDictionary(json["GuardianTotalGacha"]["S"].ToString(), str => str, str => int.Parse(str));
                        }
                        else
                        {
                            guardianTotalGacha.Add("Pet", 0);
                            guardianTotalGacha.Add("Hat", 0);
                            guardianTotalGacha.Add("Cloak", 0);
                            guardianTotalGacha.Add("Necklace", 0);
                            guardianTotalGacha.Add("Wing", 0);
                            guardianTotalGacha.Add("Mask", 0);
                            guardianTotalGacha.Add("Back", 0);
                            guardianTotalGacha.Add("Earrings", 0);
                        }

                        Dictionary<string, int> mageTotalGacha = new();
                        if (json.ContainsKey("MageTotalGacha"))
                        {
                            mageTotalGacha = Utill_Standard.JsonToDictionary(json["MageTotalGacha"]["S"].ToString(), str => str, str => int.Parse(str));
                        }
                        else
                        {
                            mageTotalGacha.Add("Pet", 0);
                            mageTotalGacha.Add("Hat", 0);
                            mageTotalGacha.Add("Cloak", 0);
                            mageTotalGacha.Add("Necklace", 0);
                            mageTotalGacha.Add("Wing", 0);
                            mageTotalGacha.Add("Mask", 0);
                            mageTotalGacha.Add("Back", 0);
                            mageTotalGacha.Add("Earrings", 0);
                        }

                        #endregion


                        #region 현재 방치모드 피로도 게이지 양
                        ObscuredInt idleModeRestCycle = json.ContainsKey("IdleModeRestCycle") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(json["IdleModeRestCycle"]["S"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 현재 방치모드 휴식 게이지 양
                        ObscuredInt idleModeRestTime = json.ContainsKey("IdleModeRestTime") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(json["IdleModeRestTime"]["S"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        #region 유저 부활 시간
                        ObscuredInt UserResurrectionTime = json.ContainsKey("UserResurrectionTime") ? Utill_Standard.LoadStringToGeneric<ObscuredInt>(json["UserResurrectionTime"]["S"].ToString(), new ObscuredInt()) : 0;
                        #endregion

                        // JSON 문자열에서 HunterSkillDic 로드
                        string jsonString = json.ContainsKey("HunterSkillDic") ? json.ContainsKey("HunterSkillDic").ToString() : "[]";
                        Dictionary<string, int> HunterSkillDic = Utill_Standard.ReadHunterSkillDic(jsonString);

                        string jsonStringGurdian = json.ContainsKey("HunterSkillDic") ? json.ContainsKey("HunterSkillDic").ToString() : "[]";
                        Dictionary<string, int> GurdianSkillDic = Utill_Standard.ReadHunterSkillDic(jsonStringGurdian);


                        userData = new User();
                        userData.InitUserData(ClearStageLevel, _UserHasResource, levelArr, expArr, hunterUpgradeList, hunterAttribute, HunterPromotion, ArcherSkillSlotindex, GuardianSkillSlotindex, MageSkillSlotindex, PurchasedSkillPreset,
                                               Euipmentuserpreset, HunterPurchase, CurrentHunter, currentEquipHunter, DailySkillSlotIndex, isDailySkill, _UserSaveForTime, totalPurchase, rewardReceived, dailyMissionTaskProgress, dailyMissionClaimStatus, archerDailyGachaLimit, guardianDailyGachaLimit, mageDailyGachaLimit,
                                               archerGachaRewardReceived, archerTotalGacha, guardianGachaRewardReceived, guardianTotalGacha, mageGachaRewardReceived, mageTotalGacha, AverageAcquisitionResource, idleModeRestCycle, idleModeRestTime ,
                                               UserResurrectionTime , HunterSkillDic , GurdianSkillDic);
                        GameDataTable.Instance.User = userData;
                        break;
                    }
                case ESaveType.Inventory:
                    {
                        #region 인벤토리 관련
                        GameDataTable.Instance.InventoryList[0] = new List<Item>(); // Archer
                        GameDataTable.Instance.InventoryList[1] = new List<Item>(); // Guardian
                        GameDataTable.Instance.InventoryList[2] = new List<Item>(); // Mage

                        if (json.ContainsKey("ArcherInventory"))
                        {
                            string archerInventoryString = json["ArcherInventory"]["S"].ToString();
                            AddItemsFromInventoryString(archerInventoryString, SubClass.Archer);
                        }

                        if (json.ContainsKey("GuardianInventory"))
                        {
                            string guardianInventoryString = json["GuardianInventory"]["S"].ToString();
                            AddItemsFromInventoryString(guardianInventoryString, SubClass.Guardian);
                        }

                        if (json.ContainsKey("MageInventory"))
                        {
                            string mageInventoryString = json["MageInventory"]["S"].ToString();
                            AddItemsFromInventoryString(mageInventoryString, SubClass.Mage);
                        }
                        #endregion
                        break;
                    }
                case ESaveType.Mail:
                    List<KeyValuePair<string, int>> itemNumAndToTal;
                    List<KeyValuePair<string, int>> itemNumAndToTal_PurchaseAchievement;
                    List<KeyValuePair<string, int>> itemNumAndToTal_DailyMission;
                    List<KeyValuePair<string, int>> itemNumAndToTal_GachaAchievement;
                    List<KeyValuePair<string, int>> itemNumAndToTal_OfflineReward;
                    #region Shop1
                    if (json.ContainsKey("itemNumAndToTal"))
                    {
                        if (json["itemNumAndToTal"]["S"].ToString() == "")
                        {
                            itemNumAndToTal = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal = Utill_Standard.JsonToKeyValuePairList(json["itemNumAndToTal"]["N"].ToString());
                        }

                    }
                    else
                    {
                        itemNumAndToTal = new List<KeyValuePair<string, int>>();
                    }
                    #endregion
                    #region PurchaseAchievement
                    if (json.ContainsKey("itemNumAndToTal_PurchaseAchievement"))
                    {
                        if (json["itemNumAndToTal_PurchaseAchievement"]["S"].ToString() == "")
                        {
                            itemNumAndToTal_PurchaseAchievement = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_PurchaseAchievement = Utill_Standard.JsonToKeyValuePairList(json["itemNumAndToTal_PurchaseAchievement"]["N"].ToString());
                        }

                    }
                    else
                    {
                        itemNumAndToTal_PurchaseAchievement = new List<KeyValuePair<string, int>>();
                    }
                    #endregion
                    #region DailyMission
                    if (json.ContainsKey("itemNumAndToTal_DailyMission"))
                    {
                        if (json["itemNumAndToTal_DailyMission"]["S"].ToString() == "")
                        {
                            itemNumAndToTal_DailyMission = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_DailyMission = Utill_Standard.JsonToKeyValuePairList(json["itemNumAndToTal_DailyMission"]["N"].ToString());
                        }

                    }
                    else
                    {
                        itemNumAndToTal_DailyMission = new List<KeyValuePair<string, int>>();
                    }
                    #endregion
                    #region DailyMission
                    if (json.ContainsKey("itemNumAndToTal_GachaAchievement"))
                    {
                        if (json["itemNumAndToTal_GachaAchievement"]["S"].ToString() == "")
                        {
                            itemNumAndToTal_GachaAchievement = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_GachaAchievement = Utill_Standard.JsonToKeyValuePairList(json["itemNumAndToTal_GachaAchievement"]["N"].ToString());
                        }

                    }
                    else
                    {
                        itemNumAndToTal_GachaAchievement = new List<KeyValuePair<string, int>>();
                    }
                    #endregion
                    #region OfflineReward
                    if (json.ContainsKey("OfflineReward"))
                    {
                        if (json["OfflineReward"]["S"].ToString() == "")
                        {
                            itemNumAndToTal_OfflineReward = new List<KeyValuePair<string, int>>();
                        }
                        else
                        {
                            itemNumAndToTal_OfflineReward = Utill_Standard.JsonToKeyValuePairList(json["OfflineReward"]["N"].ToString());
                        }

                    }
                    else
                    {
                        itemNumAndToTal_OfflineReward = new List<KeyValuePair<string, int>>();
                    }
                    #endregion
                    maildata = new Mail();
                    maildata.Init_MailData(itemNumAndToTal);
                    maildata.Init_PurchaseAchievement_MailData(itemNumAndToTal_PurchaseAchievement);
                    maildata.Init_DailyMission_MailData(itemNumAndToTal_DailyMission);
                    maildata.Init_GachaAchievement_MailData(itemNumAndToTal_GachaAchievement);
                    maildata.Init_OfflineReawerd_MailData(itemNumAndToTal_OfflineReward);
                    GameDataTable.Instance.Mail = maildata;
                    break;
                case ESaveType.HunterItem:
                    // JSON 문자열을 파싱하여 Dictionary로 변환
                    List<HunteritemData> ArcherItems = new List<HunteritemData>();
                    List<HunteritemData> GuardianItems = new List<HunteritemData>();
                    List<HunteritemData> MageItems = new List<HunteritemData>();
                    HunterItem hunteritem = new HunterItem();


                    if (json.ContainsKey("Archer") )
                    {
                        ArcherItems = ConvertStringToHunterItemDataList(json["Archer"]["S"].ToString());
                    }
                    else
                    {
                        //슬롯데이터부터
                        ArcherItems = CreateDefaultHunterSlotItmeData(hunteritem.Archer, Utill_Enum.SubClass.Archer);
                        HunterItem.Add_PartItem(ArcherItems, EquipmentType.Weapon, "ArcherNormalWeapon", "Normal" ,true , "Normal");
                    }

                    if (json.ContainsKey("Guardian"))
                    {
                        GuardianItems = ConvertStringToHunterItemDataList(json["Guardian"]["S"].ToString());
                    }
                    else
                    {
                        //슬롯데이터부터
                        GuardianItems = CreateDefaultHunterSlotItmeData(hunteritem.Guardian, Utill_Enum.SubClass.Guardian);
                        HunterItem.Add_PartItem(GuardianItems, EquipmentType.Weapon, "GuardianNormalWeapon", "Normal" , false , "Normal");
                    }

                    if (json.ContainsKey("Mage"))
                    {
                        MageItems = ConvertStringToHunterItemDataList(json["Mage"]["S"].ToString());
                    }
                    else
                    {
                        //슬롯데이터부터
                        MageItems = CreateDefaultHunterSlotItmeData(hunteritem.Mage, Utill_Enum.SubClass.Mage);
                        //HunterItem.Add_PartItem(MageItems, EquipmentType.Weapon, "MageNormalWeapon", "Normal");
                    }


                  
                    


                    // HunterItem 초기화
                    hunteritem.init_HunterItem(ArcherItems, GuardianItems, MageItems);
                    GameDataTable.Instance.HunterItem = hunteritem;
                    break;
                case ESaveType.Rank:
                    // JSON 문자열을 파싱하여 Dictionary로 변환
                    double ArcherBattlePower = json.ContainsKey("ArcherBattlePower") ? Utill_Standard.LoadStringToGeneric<double>(json["ArcherBattlePower"]["N"].ToString(), new double()) : new double();
                    double GuardanBattlePower = json.ContainsKey("GuardianBattlePower") ? Utill_Standard.LoadStringToGeneric<double>(json["GuardianBattlePower"]["N"].ToString(), new double()) : new double();
                    double MageBattlePower = json.ContainsKey("MageBattlePower") ? Utill_Standard.LoadStringToGeneric<double>(json["MageBattlePower"]["N"].ToString(), new double()) : new double();

                    int ArcerLevl = json.ContainsKey("ArcherLevel") ? Utill_Standard.LoadStringToGeneric<int>(json["ArcherLevel"]["N"].ToString(), new int()) : new int();
                    int GuardianLvel = json.ContainsKey("GuardianLevel") ? Utill_Standard.LoadStringToGeneric<int>(json["GuardianLevel"]["N"].ToString(), new int()) : new int();
                    int MageLevel = json.ContainsKey("MageLevel") ? Utill_Standard.LoadStringToGeneric<int>(json["MageLevel"]["N"].ToString(), new int()) : new int();

                    double ClearStageLevel1 = json.ContainsKey("ClearStageLevel") ? Utill_Standard.LoadStringToGeneric<double>(json["ClearStageLevel"]["N"].ToString(), new double()) : new double();

                    userRank = new Rank(ArcherBattlePower, GuardanBattlePower, MageBattlePower, ArcerLevl, GuardianLvel, MageLevel, ClearStageLevel1);
                    GameDataTable.Instance.Rank = userRank;
                    break;
                default:
                    break;
            }
        }


        public string Save(bool _isLocal, ESaveType eSaveType)
        {
            bool islocal = _isLocal;
            switch (islocal || eSaveType == ESaveType.LocalData)
            {
                case true: // Local
                    return LocalSave(eSaveType);
                case false: //server
                    return null; // 일단 null 반환
            }
        }
        //Local save
        /// 서버 저장 했는지 확인
        public string LocalSave(ESaveType eSaveType)
        {
            string returnValue = string.Empty;

            switch (eSaveType)
            {
                case ESaveType.LocalData:
                    {
                        returnValue = "{{IscheckList:{0}, IsHpOn:{{{1}}}, IsDamageOn:{{{2}}}, IsShadowOn:{{{3}}}, IsGrapicOn:{{{4}}}, CurMainExpBarType:{{{5}}}}}";
                        returnValue = string.Format(returnValue,
                            Utill_Standard.ListToJson(NotificationDotManager.Instance.IscheckList),
                            GameManager.Instance.IsHpOn,
                            GameManager.Instance.IsDamageOn,
                            GameManager.Instance.IsShadowOn,
                            GameManager.Instance.IsGrapicOn,
                            MainExpBarController.Instance.GetMainExpBarType().ToString()
                            );
                        break;
                    }
                case ESaveType.User:
                    {
                        returnValue = "{{ClearStageLevel:{{{0}}},UserHasResource:{{{1}}},UserClassData:{2},ArcherSkillSlotindex:{{{3}}},GuardianSkillSlotindex:{{{4}}},MageSkillSlotindex:{{{5}}}, PurchasedSkillPreset:{{{6}}}, EquippedSkillPreset:{{{7}}}, PurchasedHunters:{{{8}}}, SelectedTabHunter:{{{9}}}, SelectedBattleHunters:{10} , DailySkillSlotIndex:{{{11}}}, isDailySkill:{{{12}}} , UserSaveForTime:{{{13}}}, TotalPurchase:{{{14}}}, RewardReceived:{{{15}}}, GameVersion:{{{16}}},DailyMissionTaskProgress:{{{17}}},DailyMissionClaimStatus:{{{18}}},ArcherDailyGachaLimit:{{{19}}},GuardianDailyGachaLimit:{{{20}}},MageDailyGachaLimit:{{{21}}},ArcherGachaRewardReceived:{22},GuardianGachaRewardReceived:{23},MageGachaRewardReceived:{24},ArcherTotalGacha:{25},GuardianTotalGacha:{26},MageTotalGacha:{27}, AverageAcquisitionResource:{{{28}}},IdleModeRestCycle:{{{29}}},IdleModeRestTime:{{{30}}}, UserResurrectionTime:{{{31}}}, ArcherSkillDic:{{{32}}}, GurdianSkillDic:{{{33}}}}}";
                        Dictionary<SubClass, string> userClassDataDic = new();

                        #region 재화
                        ObscuredString[,] temp_curreny = Utill_Standard.ChangeCurrenyData(ResourceManager.Instance.ResourceModels);
                        User.Init_UserCurreny(GameDataTable.Instance.User, temp_curreny);
                        string UserHasResource_Str = Utill_Standard.SaveGenericToSring<ObscuredString[,]>(userData.UserHasResources);
                        #endregion

                        for (int i = 0; i < GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value; i++)
                        {
                            Dictionary<string, string> singleUserClassDataDic = new();
                            string HunterUpgradeList_Str = Utill_Standard.ConvertHunterUpgradeListToString(userData.GetUpgradeList(i));

                            string HunterAttribute_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.GetAttribute(i));

                            singleUserClassDataDic.Add("Level", GameDataTable.Instance.User.HunterLevel[i].ToString());
                            singleUserClassDataDic.Add("Exp", GameDataTable.Instance.User.HunterExp[i].ToString());
                            singleUserClassDataDic.Add("HunterUpgrade", HunterUpgradeList_Str);
                            singleUserClassDataDic.Add("HunterAttribute", HunterAttribute_Str);
                            singleUserClassDataDic.Add("HunterPromotion", userData.HunterPromotion[i].ToString());

                            string valueStr = Utill_Standard.DictionaryToJson(singleUserClassDataDic);
                            userClassDataDic.Add((SubClass)i, valueStr);
                        }

                        #region 헌터들 슬롯 인덱스
                        string ArcherSkillSlotindex_Str = Utill_Standard.SaveGenericToSring<string[,]>(userData.HunterSkillSlotindex);
                        string GuardianSkillSlotindex_Str = Utill_Standard.SaveGenericToSring<string[,]>(userData.GuardianSkillSlotindex);
                        string MageSkillSlotindex_Str = Utill_Standard.SaveGenericToSring<string[,]>(userData.MageSkillSlotindex);
                        #endregion

                        string SkillPreset_Str = Utill_Standard.DictionaryToJson(userData.SkillPreset);

                        string Euipmentuserpreset_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.Euipmentuserpreset);

                        //string BattlePower_str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.BattlePower);
                        string AverageAcquisitionResource_str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.AverageAcquisitionResource);
                        #region 헌터 구매 정보
                        string HunterPurchase_Str = Utill_Standard.SaveGenericToSring<ObscuredBool[]>(userData.HunterPurchase);
                        #endregion

                        List<SubClass> tempEquipHunter = new();
                        tempEquipHunter.AddRange(userData.CurrentEquipHunter);
                        tempEquipHunter.AddRange(userData.CurrentDieHunter);
                        string CurrentEquipHunter_Str = Utill_Standard.ListToJson(tempEquipHunter);


                        #region  요일스킬 정보
                        string DailySkillSlotIndex_Str = Utill_Standard.SaveGenericToSring(userData.DailySkillSlotIndex);
                        #endregion

                        string isDailySkill_Str = Utill_Standard.SaveGenericToSring<bool>(userData.isDailySkill);
                        string UserSaveForTime_Str = userData.UserSaveForTime;

                        string RewardReceived_Str = Utill_Standard.SaveGenericToSring<ObscuredBool[]>(userData.RewardReceived);

                        string DailyMissionTaskProgress_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.DailyMissionTaskProgress);
                        string DailyMissionClaimStatus_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.DailyMissionClaimStatus);

                        string ArcherDailyGachaLimit_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.ArcherDailyGachaLimit);
                        string GuardianDailyGachaLimit_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.GuardianDailyGachaLimit);
                        string MageDailyGachaLimit_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.MageDailyGachaLimit);

                        string ArcherGachaRewardReceived_Str = Utill_Standard.ConvertToString(userData.ArcherGachaRewardReceived);
                        string GuardianGachaRewardReceived_Str = Utill_Standard.ConvertToString(userData.GuardianGachaRewardReceived);
                        string MageGachaRewardReceived_Str = Utill_Standard.ConvertToString(userData.MageGachaRewardReceived);

                        string ArcherTotalGacha_Str = Utill_Standard.DictionaryToJson(userData.ArcherTotalGacha);
                        string GuardianTotalGacha_Str = Utill_Standard.DictionaryToJson(userData.GuardianTotalGacha);
                        string MageTotalGacha_Str = Utill_Standard.DictionaryToJson(userData.MageTotalGacha);

                        string IdleModeRestCycle_str = Utill_Standard.SaveGenericToSring<ObscuredInt>(userData.IdleModeRestCycle);
                        string IdleModeRestTime_str = Utill_Standard.SaveGenericToSring<ObscuredInt>(userData.IdleModeRestTime);
                        string UserResurrectionTime_str = Utill_Standard.SaveGenericToSring<ObscuredInt>(userData.UserResurrectionTime);

                        // Serialize HunterSkillDic
                        string HunterSkillDic_Str = Utill_Standard.SaveHunterSkillDic(GameDataTable.Instance.User.ArcherSkillDic);
                        string GuradianSkillDIC_Str = Utill_Standard.SaveHunterSkillDic(GameDataTable.Instance.User.GurdianSkillDic);

                        returnValue = string.Format(returnValue, userData.ClearStageLevel, UserHasResource_Str, Utill_Standard.DictionaryToJson(userClassDataDic), ArcherSkillSlotindex_Str,
                                                   GuardianSkillSlotindex_Str, MageSkillSlotindex_Str, SkillPreset_Str, Euipmentuserpreset_Str, HunterPurchase_Str, userData.currentHunter,
                                                   CurrentEquipHunter_Str, DailySkillSlotIndex_Str, isDailySkill_Str, UserSaveForTime_Str, userData.TotalPurchase, RewardReceived_Str, Application.version,
                                                   DailyMissionTaskProgress_Str, DailyMissionClaimStatus_Str, ArcherDailyGachaLimit_Str, GuardianDailyGachaLimit_Str, MageDailyGachaLimit_Str,
                                                   ArcherGachaRewardReceived_Str, GuardianGachaRewardReceived_Str, MageGachaRewardReceived_Str, ArcherTotalGacha_Str, GuardianTotalGacha_Str, MageTotalGacha_Str,
                                                   AverageAcquisitionResource_str, IdleModeRestCycle_str, IdleModeRestTime_str , UserResurrectionTime_str , HunterSkillDic_Str , GuradianSkillDIC_Str);

                        break;
                    }
                case ESaveType.Inventory:
                    {
                        returnValue = "{{ArcherInventory:{0}, GuardianInventory:{1}, MageInventory:{2}}}";
                        returnValue = string.Format(returnValue, Utill_Standard.ListToJson(GameDataTable.Instance.InventoryList[0]),
                            Utill_Standard.ListToJson(GameDataTable.Instance.InventoryList[1]),
                            Utill_Standard.ListToJson(GameDataTable.Instance.InventoryList[2]));
                        break;
                    }
                case ESaveType.Mail:
                    // maildata = GameDataTable.Instance.Usermail;
                    returnValue = "{{itemNumAndToTal:{{{0}}}, itemNumAndToTal_PurchaseAchievement:{{{1}}},itemNumAndToTal_DailyMission:{{{2}}}, OfflineReward: {{{3}}}}}";
                    string itemNumAndToTal_Str = Utill_Standard.KeyValuePairListToString(maildata.Shop1);
                    string itemNumAndToTal_PurchaseAchievement_Str = Utill_Standard.KeyValuePairListToString(maildata.PurchaseAchievement);
                    string itemNumAndToTal_DailyMission_Str = Utill_Standard.KeyValuePairListToString(maildata.DailyMission);
                    string itemNumAndToTal_OfflineReward_Str = Utill_Standard.KeyValuePairListToString(maildata.OfflineReward);


                    returnValue = string.Format(returnValue, itemNumAndToTal_Str, itemNumAndToTal_PurchaseAchievement_Str, itemNumAndToTal_DailyMission_Str, itemNumAndToTal_OfflineReward_Str);
                    break;
                case ESaveType.HunterItem:
                    hunteritem = GameDataTable.Instance.HunterItem;
                    returnValue = "{{Archer:{0}, Guardian:{1}, Mage:{2}}}";
                    string archerItemsStr = SerializeHunteritemDataList(hunteritem.Archer);
                    string guardianItemsStr = SerializeHunteritemDataList(hunteritem.Guardian);
                    string mageItemsStr = SerializeHunteritemDataList(hunteritem.Mage);

                    returnValue = string.Format(returnValue, archerItemsStr, guardianItemsStr, mageItemsStr);
                    break;
                case ESaveType.Rank:
                    userRank = GameDataTable.Instance.Rank;

                    // 포맷 문자열 설정
                    returnValue = "{{ArcherBattlePower:{{{0}}}, GuardianBattlePower:{{{1}}}, MageBattlePower:{{{2}}}, ArcherLevel:{{{3}}}, GuardianLevel:{{{4}}}, MageLevel:{{{5}}}, ClearStageLevel:{{{6}}}}}";

                    // 각 값을 문자열로 변환
                    string ArcherBattlePowerStr = Utill_Standard.SaveGenericToSring<double>(userRank.ArcherBattlePower);
                    string GuardanBattlePowerStr = Utill_Standard.SaveGenericToSring<double>(userRank.GuardianBattlePower);
                    string MageBattlePowerStr = Utill_Standard.SaveGenericToSring<double>(userRank.MageBattlePower);

                    string ArcerLevlStr = Utill_Standard.SaveGenericToSring<int>(userRank.ArcherLevel);
                    string GuardianLvelStr = Utill_Standard.SaveGenericToSring<int>(userRank.GuardianLevel);
                    string MageLevelStr = Utill_Standard.SaveGenericToSring<int>(userRank.MageLevel);

                    string ClearStageLevelStr = Utill_Standard.SaveGenericToSring<double>(userRank.ClearStageLevel);

                    // string.Format을 통해 포맷 문자열과 값을 연결
                    returnValue = string.Format(returnValue, ArcherBattlePowerStr, GuardanBattlePowerStr, MageBattlePowerStr, ArcerLevlStr, GuardianLvelStr, MageLevelStr, ClearStageLevelStr);
                    break;
            }

            return returnValue; // 저장된 데이터 반환
        }

        //서버 save
        /// 로컬 저장 했는지 확인
        public BackEnd.Param ServerSave(ESaveType eSaveType)
        {
            var returnValue = new BackEnd.Param();

            switch (eSaveType)
            {
                case ESaveType.User:
                    returnValue.Add("ClearStageLevel", userData.ClearStageLevel.GetDecrypted());

                    ObscuredString[,] temp_curreny = Utill_Standard.ChangeCurrenyData(ResourceManager.Instance.ResourceModels);
                    User.Init_UserCurreny(GameDataTable.Instance.User, temp_curreny);
                    string UserHasResource_Str = Utill_Standard.SaveGenericToSring(userData.UserHasResources);
                    returnValue.Add("UserHasResource", UserHasResource_Str);


                    string AverageAcquisitionResource_Str = Utill_Standard.SaveGenericToSring(userData.AverageAcquisitionResource);
                    returnValue.Add("AverageAcquisitionResource", AverageAcquisitionResource_Str);

                    Dictionary<SubClass, string> userClassDataDic = new();

                    for (int i = 0; i < GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value; i++)
                    {
                        Dictionary<string, string> singleUserClassDataDic = new();
                        string HunterUpgradeList_Str = Utill_Standard.ConvertHunterUpgradeListToString(userData.GetUpgradeList(i));
                        string HunterAttribute_Str = Utill_Standard.SaveGenericToSring(userData.GetAttribute(i));

                        singleUserClassDataDic.Add("Level", GameDataTable.Instance.User.HunterLevel[i].ToString());
                        singleUserClassDataDic.Add("Exp", GameDataTable.Instance.User.HunterExp[i].ToString());
                        singleUserClassDataDic.Add("HunterUpgrade", HunterUpgradeList_Str);
                        singleUserClassDataDic.Add("HunterAttribute", HunterAttribute_Str);
                        singleUserClassDataDic.Add("HunterPromotion", userData.HunterPromotion[i].ToString());

                        string valueStr = Utill_Standard.DictionaryToJson(singleUserClassDataDic);
                        userClassDataDic.Add((SubClass)i, valueStr);
                    }

                    returnValue.Add("UserClassData", Utill_Standard.DictionaryToJson(userClassDataDic));


                    string HunterSkillSlotindex_Str = Utill_Standard.SaveGenericToSring(userData.HunterSkillSlotindex);
                    returnValue.Add("ArcherSkillSlotindex", HunterSkillSlotindex_Str);

                    string GuardianSkillSlotindex_Str = Utill_Standard.SaveGenericToSring(userData.GuardianSkillSlotindex);
                    returnValue.Add("GuardianSkillSlotindex", GuardianSkillSlotindex_Str);

                    string MageSkillSlotindex_Str = Utill_Standard.SaveGenericToSring(userData.MageSkillSlotindex);
                    returnValue.Add("MageSkillSlotindex", MageSkillSlotindex_Str);

                    string DailySkillSlotIndex_Str = Utill_Standard.SaveGenericToSring(userData.DailySkillSlotIndex);
                    returnValue.Add("DailySkillSlotIndex", DailySkillSlotIndex_Str);

                    string SkillPreset_Str = Utill_Standard.DictionaryToJson(userData.SkillPreset);
                    returnValue.Add("PurchasedSkillPreset", SkillPreset_Str);

                    string Euipmentuserpreset_Str = Utill_Standard.SaveGenericToSring(userData.Euipmentuserpreset);
                    returnValue.Add("EquippedSkillPreset", Euipmentuserpreset_Str);

                    string HunterPurchase_Str = Utill_Standard.SaveGenericToSring(userData.HunterPurchase);
                    returnValue.Add("PurchasedHunters", HunterPurchase_Str);

                    string isDailySkill_Str = Utill_Standard.SaveGenericToSring(userData.isDailySkill);
                    returnValue.Add("isDailySkill", isDailySkill_Str);


                    string UserSaveForTime_Str = userData.UserSaveForTime;
                    returnValue.Add("UserSaveForTime", UserSaveForTime_Str);

                    returnValue.Add("SelectedTabHunter", userData.currentHunter.GetDecrypted());

                    List<SubClass> tempEquipHunter = new();
                    tempEquipHunter.AddRange(userData.CurrentEquipHunter);
                    tempEquipHunter.AddRange(userData.CurrentDieHunter);
                    string CurrentEquipHunter_Str = Utill_Standard.ListToJson(tempEquipHunter);
                    returnValue.Add("SelectedBattleHunters", CurrentEquipHunter_Str);

                    returnValue.Add("TotalPurchase", userData.TotalPurchase.GetDecrypted());

                    string RewardReceived_Str = Utill_Standard.SaveGenericToSring<ObscuredBool[]>(userData.RewardReceived);
                    returnValue.Add("RewardReceived", RewardReceived_Str);

                    //string HunterCombatPoint_Str = Utill_Standard.SaveGenericToSring(userData.BattlePower);
                    //returnValue.Add("HunterCombatPoint", HunterCombatPoint_Str);


                    returnValue.Add("GameVersion", game_Version); //현재 유저의 앱 버전 저장

                    string DailyMissionTaskProgress_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.DailyMissionTaskProgress);
                    returnValue.Add("DailyMissionTaskProgress", DailyMissionTaskProgress_Str);
                    string DailyMissionClaimStatus_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.DailyMissionClaimStatus);
                    returnValue.Add("DailyMissionClaimStatus", DailyMissionClaimStatus_Str);

                    string ArcherDailyGachaLimit_Str = Utill_Standard.SaveGenericToSring<ObscuredInt[]>(userData.ArcherDailyGachaLimit);
                    returnValue.Add("ArcherDailyGachaLimit", ArcherDailyGachaLimit_Str);

                    string IdleModeRestCycle_str = Utill_Standard.SaveGenericToSring<ObscuredInt>(userData.IdleModeRestCycle);
                    returnValue.Add("IdleModeRestCycle", IdleModeRestCycle_str);
                    string IdleModeRestTime_str = Utill_Standard.SaveGenericToSring<ObscuredInt>(userData.IdleModeRestTime);
                    returnValue.Add("IdleModeRestTime", IdleModeRestTime_str);

                    string _UserResurrectionTime_str = Utill_Standard.SaveGenericToSring<ObscuredInt>(userData.UserResurrectionTime);
                    returnValue.Add("UserResurrectionTime", _UserResurrectionTime_str);

                    // Add HunterSkillDic serialization
                    string HunterSkillDic_Str = Utill_Standard.DictionaryToJson(userData.ArcherSkillDic);
                    returnValue.Add("ArcherSkillDic", HunterSkillDic_Str);

                    string GurdianSkillDic_Str = Utill_Standard.DictionaryToJson(userData.GurdianSkillDic);
                    returnValue.Add("GurdianSkillDic", GurdianSkillDic_Str);
                    break;
                case ESaveType.Inventory:
                    returnValue.Add("ArcherInventory", Utill_Standard.ListToJson(GameDataTable.Instance.InventoryList[0]));
                    returnValue.Add("GuardianInventory", Utill_Standard.ListToJson(GameDataTable.Instance.InventoryList[1]));
                    returnValue.Add("MageInventory", Utill_Standard.ListToJson(GameDataTable.Instance.InventoryList[2]));
                    break;
                default:
                    break;

                case ESaveType.Mail:
                    string itemNumAndToTal_Str = Utill_Standard.KeyValuePairListToString(maildata.Shop1);
                    string itemNumAndToTal_PurchaseAchievement_Str = Utill_Standard.KeyValuePairListToString(maildata.PurchaseAchievement);
                    string itemNumAndToTal_DailyMission_Str = Utill_Standard.KeyValuePairListToString(maildata.DailyMission);

                    returnValue.Add("itemNumAndToTal", itemNumAndToTal_Str);
                    returnValue.Add("itemNumAndToTal_PurchaseAchievement", itemNumAndToTal_PurchaseAchievement_Str);
                    returnValue.Add("itemNumAndToTal_DailyMission", itemNumAndToTal_DailyMission_Str);
                    break;
                case ESaveType.HunterItem:
                    hunteritem = GameDataTable.Instance.HunterItem;
                    string Archerslottime_Str = Utill_Standard.SerializeHunteritemDataList(hunteritem.Archer);
                    string Guardianslottime_Str = Utill_Standard.SerializeHunteritemDataList(hunteritem.Guardian);
                    string Mageslottime_Str = Utill_Standard.SerializeHunteritemDataList(hunteritem.Mage);


                    returnValue.Add("Archer", Archerslottime_Str);
                    returnValue.Add("Guardian", Archerslottime_Str);
                    returnValue.Add("Mage", Mageslottime_Str);

                    break;
                case ESaveType.Rank:

                    userRank = GameDataTable.Instance.Rank;
                    Rank.Save_Rank_Power(userRank, GameDataTable.Instance.User);
                    //string ArcherBattlePowerStr = Utill_Standard.SaveGenericToSring<int>(userRank.ArcherBattlePower); 
                    //string GuardanBattlePowerStr = Utill_Standard.SaveGenericToSring<int>(userRank.GuardianBattlePower);
                    //string MageBattlePowerStr = Utill_Standard.SaveGenericToSring<int>(userRank.MageBattlePower);

                    //string ArcerLevlStr = Utill_Standard.SaveGenericToSring<int>(userRank.ArcherLevel);
                    //string GuardianLvelStr = Utill_Standard.SaveGenericToSring<int>(userRank.GuardianLevel);
                    //string MageLevelStr = Utill_Standard.SaveGenericToSring<int>(userRank.MageLevel);

                    //string ClearStageLevelStr = Utill_Standard.SaveGenericToSring<int>(userRank.ClearStageLevel);

                    //랭크 저장값들은 정수
                    returnValue.Add("ArcherBattlePower", userRank.ArcherBattlePower);
                    returnValue.Add("GuardianBattlePower", userRank.GuardianBattlePower);
                    returnValue.Add("MageBattlePower", userRank.MageBattlePower);

                    returnValue.Add("ArcherLevel", userRank.ArcherLevel);
                    returnValue.Add("GuardianLevel", userRank.GuardianLevel);
                    returnValue.Add("MageLevel", userRank.MageLevel);

                    returnValue.Add("ClearStageLevel", userRank.ClearStageLevel);
                    break;
            }

            return returnValue;
        }

        public static List<KeyValuePair<string, int>> StringToKeyValuePairList(string input)
        {
            var list = new List<KeyValuePair<string, int>>();

            if (string.IsNullOrWhiteSpace(input))
            {
                UnityEngine.Debug.Log("Input string is null or empty.");
                return list;
            }

            // Split the input string by semicolon to get key-value pairs
            var keyValuePairs = input.Split(';');

            foreach (var pair in keyValuePairs)
            {
                // Split each pair by equals sign
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim(); // Remove any leading/trailing whitespace
                    var valueString = keyValue[1].Trim(); // Remove any leading/trailing whitespace

                    // Try to parse the value as an integer
                    if (int.TryParse(valueString, out var value))
                    {
                        list.Add(new KeyValuePair<string, int>(key, value));
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Value '{valueString}' for key '{key}' is not a valid integer.");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log($"Invalid key-value pair: '{pair}'.");
                }
            }

            return list;
        }


        private List<HunteritemData> CreateDefaultHunterSlotItmeData(List<HunteritemData> list, Utill_Enum.SubClass subclass)
        {
            List<HunteritemData> templist;

            for (int i = 0; i < 16; i++)
            {
                var item = new HunteritemData
                {
                    Class = subclass,
                    ItemContainsType = 0,
                    ItemGrade = Utill_Enum.Grade.None, // 무기 값 설정
                    DrawerGrade = Utill_Enum.DrawerGrade.Normal, // 모루 값 설정
                    Part = (Utill_Enum.EquipmentType)i + 1, // 기본 값 설정
                    Name = "",
                    TotalLevel = 0,
                    isEquip = false,
                    FixedOptionTypes = new List<string> { }, // 기본 값 설정
                    FixedOptionValues = new List<double> { }, // 기본 값 설정
                    FixedOptionPersent = new List<double> { }, // 기본값
                    EquipCountList = new List<int>(), // 기본값
                    HoldOption = new List<string>(), // 기본값
                    HoldOptionValue = new List<int>(), // 기본값
                };
                item.EquipCountList = Enumerable.Repeat(0, GameDataTable.Instance.ItemGachaMergeDataDic.Count).ToList();
                list.Add(item);

            }
            templist = list;
            return templist;
        }

        private string SerializeHunteritemDataList(List<HunteritemData> list)
        {
            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        private string SerializeHunteritemDataList(int list)
        {
            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        #region 저장시 시스템 알림 띄우는 로직
        struct SaveSystemNotice
        {
            public string saveName;
            public bool isSuccess;
        }
        private StringBuilder saveSystemNoticeSb = new();
        private Queue<SaveSystemNotice> saveSystemNoticeQueue = new();

        public void AddSystemNotice(string saveName, bool isSuccess)
        {
            saveSystemNoticeQueue.Enqueue(new SaveSystemNotice { saveName = saveName, isSuccess = isSuccess });
        }
        private IEnumerator SystemNoticeCoroutine()
        {
            while (true)
            {
                // 큐가 비어있으면 계속 대기
                yield return new WaitUntil(() => saveSystemNoticeQueue.Count > 0);
                // 다른 저장이 추가로 생길 가능성을 염두해 잠시 대기함
                yield return Utill_Standard.WaitTimehalfOne;

                saveSystemNoticeSb.Clear();
                saveSystemNoticeSb.Append("'");

                while (saveSystemNoticeQueue.Count > 0)
                {
                    var tmpSystemNotice = saveSystemNoticeQueue.Peek(); // Peek으로 요소 확인 후 Dequeue

                    if (tmpSystemNotice.saveName == "") //큐에 담긴 정보가 이상할 때
                    {
                        Game.Debbug.Debbuger.Debug("this Save SystemNotice string is invalid.");
                        saveSystemNoticeQueue.Dequeue();
                        continue;
                    }

                    if (tmpSystemNotice.isSuccess == false)
                    {
                        // 실패한 항목은 큐에서 제거하고 건너뜀
                        saveSystemNoticeQueue.Dequeue();
                        continue;
                    }

                    // 성공한 항목 저장
                    saveSystemNoticeSb.Append(tmpSystemNotice.saveName);

                    // 큐에서 요소 제거
                    saveSystemNoticeQueue.Dequeue();

                    if (saveSystemNoticeQueue.Count > 0)
                        saveSystemNoticeSb.Append(", ");
                }
                saveSystemNoticeSb.Append("' 데이터 저장 성공");

                // 시스템 알림 호출
                SystemNoticeManager.Instance.SystemNoticeInEditor(saveSystemNoticeSb.ToString(), Utill_Enum.SystemNoticeType.NoBackground);
                saveSystemNoticeQueue.Clear();
            }
        }
        #endregion



        public  List<HunteritemData> ConvertStringToHunterItemDataList(string input)
        {
            var list = new List<HunteritemData>();
            var jsonString = ConvertToJson(input);

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                UnityEngine.Debug.Log("Converted JSON string is null or empty");
                return list;
            }

            // JSON 문자열을 JsonData로 변환
            JsonData jsonData = JsonMapper.ToObject(jsonString);

            // 배열인지 확인
            if (jsonData.IsArray)
            {
                foreach (JsonData item in jsonData)
                {
                    HunteritemData hunterData = new HunteritemData
                    {
                        Class = (Utill_Enum.SubClass)Enum.Parse(typeof(Utill_Enum.SubClass), item["Class"].ToString()),
                        Part = GetEnumValue<Utill_Enum.EquipmentType>(item["Part"]),
                        DrawerGrade = GetEnumValue<Utill_Enum.DrawerGrade>(item["ItemGrade"]),
                        Name = item["EquippedItemName"].ToString(),
                        isEquip = bool.Parse(item["isEquip"].ToString()),
                        FixedOptionTypes = new List<string>(),
                        FixedOptionPersent = new List<double>(),
                        EquipCountList = new List<int>(),
                        HoldOption = new List<string>(),
                        HoldOptionValue = new List<int>(),
                    };

                    // FixedOptionTypes 리스트 파싱
                    if (item.ContainsKey("FixedOptionTypes") && item["FixedOptionTypes"].IsArray)
                    {
                        foreach (var option in item["FixedOptionTypes"])
                        {
                            hunterData.FixedOptionTypes.Add(option.ToString());
                        }
                    }

                    // FixedOptionPersent 리스트 파싱
                    if (item.ContainsKey("FixedOptionValues") && item["FixedOptionValues"].IsArray)
                    {
                        foreach (var value in item["FixedOptionValues"])
                        {
                            hunterData.FixedOptionPersent.Add(double.Parse(value.ToString()));
                        }
                    }

                    if(item.ContainsKey("EquipCountList") && item["EquipCountList"].IsArray)
                    {
                        foreach (var value in item["EquipCountList"])
                        {
                            hunterData.EquipCountList.Add(int.Parse(value.ToString()));
                        }
                    }

                    if(item.ContainsKey("HoldOption") && item["HoldOption"].IsArray)
                    {
                        foreach (var value in item["HoldOption"])
                        {
                            hunterData.HoldOption.Add(value.ToString());
                        }
                    }

                    if (item.ContainsKey("HoldOptionValue") && item["HoldOptionValue"].IsArray)
                    {
                        foreach (var value in item["HoldOptionValue"])
                        {
                            hunterData.HoldOptionValue.Add(int.Parse(value.ToString()));
                        }
                    }

                    list.Add(hunterData);
                }
            }
            else
            {
                UnityEngine.Debug.Log("JSON string is not in the expected array format.");
            }

            return list;
        }
        private static T GetEnumValue<T>(JsonData jsonData)
        {
            string value = jsonData.ToString();
            if (value == "null")
            {
                return default(T); // null 또는 기본값 반환
            }
            return (T)Enum.Parse(typeof(T), value);
        }


        private  string ConvertToJson(string input)
        {
            // 여러 개의 아이템을 JSON 배열로 변환
            // 속성 이름을 올바른 JSON 형식으로 변경
            string pattern = @"{[^}]+}"; // JSON 객체 패턴
            var matches = Regex.Matches(input, pattern);

            List<string> jsonObjects = new List<string>();

            foreach (Match match in matches)
            {
                string jsonObject = match.Value
                    .Replace("=", ":") // = 을 :로 변경
                    .Replace("None", "null") // None을 null로 변경
                    .Replace("False", "false") // False를 false로 변경
                    .Replace("True", "true") // True를 true로 변경
                    .Trim(); // 양쪽 공백 제거

                // Fix for empty collections
                jsonObject = Regex.Replace(jsonObject, @"(\w+)\s*:\s*\[\]", "$1:[]"); // 빈 배열을 처리

                // Ensure keys are quoted
                jsonObject = Regex.Replace(jsonObject, @"(\w+)", "\"$1\"");

                jsonObjects.Add(jsonObject);
            }

            // JSON 배열 형식으로 결합
            return "[" + string.Join(",", jsonObjects) + "]";
        }
    }



    }



