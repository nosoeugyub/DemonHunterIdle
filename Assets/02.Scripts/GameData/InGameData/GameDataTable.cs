using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 게임에사용하는 모든 데이터를 모아둔곳
/// </summary>
public class GameDataTable 
{

    //동적 CSV
   // public Dictionary<string, UserData> UserDataDic;  안쓰는 유저데이터
    public Dictionary<string, ConstraintsData> ConstranitsDataDic = new(); //ETC 최대갯수들
    public Dictionary<string, RequireData> RequireDataDic = new(); // 레벨업당 필요한 경험치 테이블 
    public Dictionary<string, EnemyStat> EnemyStatDic = new(); // 몬스터 스텟 테이블 
    public Dictionary<string, HunterStat> UserStatDic = new(); //유저 디폴트 스텟
    public Dictionary<int, StageTableData> StageTableDic = new(); //스테이지 테이블 딕셔너리
    public Dictionary<SubClass, Dictionary<string, Item>> EquipmentList = new(); // 아이템테이블
    public Dictionary<string, int> ItemRandomOptionCnt = new Dictionary<string, int>();
    public Dictionary<Utill_Enum.Resource_Type , ResourceTableData> ResourceTableDic = new();//재화 테이블
    public Dictionary<string, StatTableData> StatTableDataDic = new(); //스텟 최대치 테이블
    public Dictionary<string, ColorCodeData> ColorCodeDataDic = new(); //스텟 최대치 테이블
    public Dictionary<int , Hunter_UpgradeData> HunterUpgradeDataDic = new(); //헌터업그레이드데이터
    public Dictionary<Utill_Enum.Hunter_Attribute, HunterAttributeData> HunterAttraibuteDataDic = new(); //헌터 속성구성 데이터
    public Dictionary<int , AttributeAllocationData> AttributeAllocationData = new(); //헌터 속성지급 데이터
    public Dictionary<string , DailySkillData> DailySkillDataDic = new(); //헌터요일퀘스트
    public Dictionary<int, ShopGoldCellData> ShopResoucrceGolddata_One_Dic = new(); //골드상점1
    public Dictionary<int , ShopGoldCellData> ShopResoucrceGolddataDic = new(); //골드상점2
    public Dictionary<int , PurchaseAchievementData> PurchaseAchievementDataDic = new(); //과금업적 테이블
    public Dictionary<int , GachaAchievementData> GachaAchievementDataDic = new(); //가챠 과금업적 테이블
    public Dictionary<Utill_Enum.DrawerGrade, ItemDrawerTableData> ItemDrawerGradeDic = new(); //모루등급 테이블
    public List<DailyMissionData> DailyMissionList = new(); //일일임무 테이블
    public Dictionary<int , ItemGachaData> ItemGachaDataDic = new(); //아이템 가챠 테이블
    public Dictionary<int , ItemGachaMerge> ItemGachaMergeDataDic = new(); //아이템 가챠 합성 테이블

    public Dictionary<int , PromotionAllocationData> PromotionAllocationData = new(); //헌터 승급시 필요한 데이터
    public Dictionary<int, PromotionAbilityData> PromotionAbilityData = new(); //헌터 승급 능력치 데이터
    public Dictionary<string, List<BaseSkillData>> ArcherSkillList; //헌터 스킬리스트
    public Dictionary<string, List<BaseSkillData>> GurdianSkillLiat; //수호자 스킬리스트
    public List<BaseSkillData> MageSkillLiat; // 마법사 스킬리스트




    //StreamingAsset / Server 정적
    public User User; //유저 데이터 
    public Mail Mail; //유저 메일데이터 
    public HunterItem HunterItem; //헌터들 슬롯아이템 데이터
    public Rank Rank;// 유저의 랭킹 정보

    public Dictionary<string, ConstraintsData> InventoryInfoData = new();  //사용하지 않음

    public Dictionary<int, List<Item>> InventoryList = new Dictionary<int, List<Item>>(); // 인벤토리 리스트


    /// <summary>
    /// 테스트용 데이터 초기화 기능을 위한 리셋 기능
    /// </summary>
    public void ResetAllTable()
    {
        ConstranitsDataDic.Clear();
        RequireDataDic.Clear();
        EnemyStatDic.Clear();
        UserStatDic.Clear();
        StageTableDic.Clear();
        EquipmentList.Clear();
        ItemRandomOptionCnt.Clear();
        ResourceTableDic.Clear();
        StatTableDataDic.Clear();
        ColorCodeDataDic.Clear();
        HunterUpgradeDataDic.Clear();
        HunterAttraibuteDataDic.Clear();
        AttributeAllocationData.Clear();
        PromotionAllocationData.Clear();
        PromotionAbilityData.Clear();

        User = null;
    }

    // 싱글톤 인스턴스
    private static GameDataTable instance;

    // 외부에서 접근 가능한 인스턴스 프로퍼티
    public static GameDataTable Instance
    {
        get
        {
            // 인스턴스가 생성되지 않았다면 생성
            if (instance == null)
            {
                instance = new GameDataTable();
            }
            return instance;
        }
    }

}
