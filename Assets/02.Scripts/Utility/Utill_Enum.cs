using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 전반적으로사용하는 이넘값
/// </summary>
public static class Utill_Enum
{
    public enum Enum_GameSequence //게임시퀀스
    {
        DataLoad,
        CreateAndInit,
        Start,
        Tutorial,
        InGame,
    }

    public enum Enum_GameStat //게임상태
    {
        Puase,
        Play,
        GameOver,
        GameClose,
    }

    public enum EnemyClass //몹타입
    {
        None = 0,
        Normal,
        Boss,
        EBoss
    }

    public enum Enum_AttackType
    {
        None,
        Melee,
        Range,
        Spell
    }
    public enum NoticeType
    {
        NomalDamage,
        MagicDamage,
        CriDamage,
        PoisonDamage,
        Dodge,
        Gold,
        Exp,
        LevelUp,
        Reflection,
        CoupDamage, //일격피해
        Drain, //체력흡수, 마나흡수
        ExpFever,
        GoldFever,
    }

    public enum HunterStateType
    {
        Move,
        JoystickMove,  //조이스틱 컨트롤 시 움직임 제어
        Die,
        Attack,
        Stop,
        Whirlwind,
        Skill,
        MoveToRest, //피로도가 쌓였을 시 쉬러 이동하는 상태
        Resting, //피로도가 쌓였을 시 휴식중인 상태
        CheatJoystickMove, //치트 사용시 상태 (안 움직이고 조이스틱만 사용됨)
    }

    public enum EnemyStateType
    {
        Idle,
        Move,
        Die,
        Attack,
        Debuff,
        Stop,
    }

    public enum Option
    {
        None, AttackSpeed, AttackSpeedPercent, MoveSpeed, MoveSpeedPercent, GroupMoveSpeed, AttackRange, PhysicalPower, PhysicalPowerPercent, MagicPower, MagicPowerPercent, 
        PhysicalTrueDamage, PhysicalTrueDamagePercent, MagicTrueDamage, MagicTrueDamagePercent, CriChance, CriDamage, PhysicalPowerDefense, PhysicalPowerDefensePercent, 
        MagicPowerDefense, MagicPowerDefensePercent, PhysicalDamageReduction, MagicDamageReduction, CCResist, DodgeChance, HP, HPPercent, MP, MPPercent, ExpBuff, GoldBuff, 
        ItemBuff, AddDamageToNormalMob, AddDamageToBossMob, ArrowCount, ArrowRange, InstantKillBossOnBasicAttack, ReflectRange, AddDamageArrowRain, ElectricshockNumber, 
        AddDamageChainLightning, AddDamageHammerSummon, AddDamageMagneticWaves, AddDamageStrongShot, AddDamageRapidShot, AddDamageDemonicBlow, AddDamageSpikeTrap, 
        AddDamageVenomousSting, AddDamageElectric, AddDamageWhirlwindRush, AddDamageDokkaebiFire, AddDamageMeteor, AddDamageElectricshock, 
        BeastTransformationStackCountReduce, ContinuousHitStackCountReduce, DokkaebiFireRotationSpeed, ElectricCastingChance, SplitPiercingNumber, 
        SpikeTrapNumber, SpikeTrapDamageRadius, VenomousStingNumber, VenomousStingDamageRadius, WhirlwindRushSpeedPercent, ChainLightningNumber, 
        LastLeafAblityNumber, MagneticWavesRangeNumber,CoupChance,CoupDamage,HPDrain,MPDrain, GroupMoveSpeedPercent,
    }

    public enum Grade { None, Normal , Superior, Rare, Unique , Epic, Hero,
        Ancient,
        Abyssal,
        Legendary,
        Mythic,
        Celestial,
        Primordial,
        Absolute,
    } // 아이템 등급 종류     

    public enum EquipmentType  // 장착아이템 종류       
    {
        None,
        Pet, Wing , Hat, Mask , Cloak , Back ,
        Necklace , Earrings , Weapon , Clothes,
        LeftBracelet, RightBracelet,
        LeftRing, RightRing,
        Gloves, Shoes,
        Hair, Body, //커마용
    }
    public enum AchievementType
    {
        Purchase,
        Gacha,
    }

    public enum Optiontype { Fixed, Random }

    public enum HeroClass //직업클래스
    {
        None,
        Hunter,
    }

    public enum SubClass
    {
        None = -1,
        Archer = 0,
        Guardian = 1,
        Mage = 2,
        //추후 추가 예정
    }

    public enum AttackDamageType
    {
        Physical,
        Magic,
        PhysicalTrueDamage,
        MagicTrueDamage,
    }

    public enum BossSqeunce
    {
        None,
        Cinemtic,
        DataSet,
        Spwan,
        Die,
    }

    public enum IdleModeRestCycleSequence
    {
        None,
        Cinematic, //연출
        Start, //피로도 회복 시작(모든 헌터 캠프 도착 후)
        End,
    }


    public enum ButtonType
    {
        Active,
        DeActive,
    }

    public enum EventNotificationDotType //조건제어 레드 닷 타입
    {
        None, //무조건 활성화
        CanEnterBoss, //보스전 입장 가능
        Mail, //메일함에 메일 있는지
    }
    public enum CheckableNotificationDotType //확인유무 제어 레드 닷 타입
    {
        None,
        RateCheck, //평점강화 가능
        Guide, //가이드
    }
    public enum CsvType
    {
        Hunter_Exp,
    }

    /// <summary>
    /// 랭킹의 종류
    /// 랭킹 데이터를 불러올 때 어떤 랭킹을 불러올지 기준이 됨
    /// </summary>
    public enum RankType
    {
        None, BPArcher1, BPGuardian1, BPMage1, ClearStageLevel1,
    }
    
    public enum SystemNoticeType
    {
        Default,
        NoBackground,
    }

    public enum ChatBroadcastType //아이템 드랍 등의 알림의 종류
    {
        None = -1,
        Boss_FirstClear,
        Item_Drop,
        Gacha,
        END //마지막 enum값으로, 모든 enum은 해당 enum보다 위에 존재해야함
    }
    public enum ChatCellType //채팅 셀의 타입
    {
        Chat,           //일반 채팅
        Alarm,          //유저 고급 아이템 획득 등의 알림
        Notification,   //채팅 최대수 제한 등의 안내
        Console,        //운영자가 콘솔에서 보낸 채팅
    }

    #region 재화
    public enum Resource_Type
    {
        None,
        Gold,
        Dia,
        Free,
        Premium,
        Exp,
    }
    #endregion
    #region SKillType
    public enum Skill_Type
    {
        Active,  //액티브
        Passive, //패시브
        RangeDamage, //1대1 대응 대미지 타입인지?
        Buff, // 광역 대미지 타입인지?
        SingleDamage,//단일대미지
        Archer ,
        Guardian ,
        Mage2,
        Daily,
    }

    public enum DailySkillType
    {
        None,
        Archer,
        Guardian,
        Mage,
    }

    public enum SkillCastingPositionType
    {
        RelativeToCamera,
        SkillDiameterToHunter,
        SkillRangeToHunter,
        None,
        FindSKillUnitList
    }
    #endregion

    #region 
    public enum Skill
    {
        Empty,
        Fire,
        Heal,
        Arrow
    }
    #endregion


    public enum Debuff_Type
    {
        Posion,
        Stun,
        Freezing,
        Slow,
        electricEffect,
        ChainLightning,
        BloodSurge,
    }

    public enum Backend_Result_Type
    {
        False,
        ReTry,
        True,
    }

    public enum SkillAnimationType
    {
        None = 0,
        Whirlwind = 1,
        StrongShot = 2, 
        HammerSummon = 3,
        ArrowRain = 4,
    }

    public enum DailyMissionType
    {
        DailyAttendance,
        NomalMobKill,
        EBossMobKill,
    }


    #region 컬러
    public enum Text_Color
    {
        Red, Yellow, Gray, Green, White, Default , OptionColor, OptionValueColor, Gray2
    }
    #endregion

    #region 강화
    public enum Upgrade_Type
    {
        PhysicalPower, PhysicalPowerPercent, MagicPower, PhysicalPowerDefense, PhysicalPowerDefensePercent, HP, MP, CriChance, CriDamage,
        AttackSpeedPercent, MoveSpeedPercent, GoldBuff, ExpBuff, ItemBuff , ArrowCount, DodgeChance , MagicPowerPercent , PhysicalTrueDamage,
        PhysicalTrueDamagePercent , MagicTrueDamage , MagicTrueDamagePercent , MagicPowerDefense, MagicPowerDefensePercen,
        AddDamageArrowRain, ElectricshockNumber,CoupChance,
    }

    #endregion

    #region 속성
    public enum Hunter_Attribute
    {
        Archer_Attribute01,
        Archer_Attribute02,
        Archer_Attribute03,
        Guardian_Attribute01,
        Guardian_Attribute02,
        Guardian_Attribute03,
        Mage_Attribute01,
        Mage_Attribute02,
        Mage_Attribute03
    }

    #endregion

    #region 상점
    public enum ShopType
    {
        Resource
    }
    #endregion

    #region 슬라이더컬러
    public enum Slider_Color
    {
        SliderBG01, SliderBar01
    }
    #endregion

    #region 헌터슬롯 아이템
    public enum DrawerGrade
    {
        None,
        Normal,
        Superior,
        Rare,
        Unique,
        Epic,
        Hero,
        Ancient,
        Abyssal,
        Legendary,
       Mythic,
       Celestial,
       Primordial,
       Absolute,

    }
    #endregion


    #region 스킬 슬롯 상태
    public enum SkillSlotState
    {
        Waiting,
        ManaShortage,
        Usable,
        NoSkill
    }
    #endregion
}
