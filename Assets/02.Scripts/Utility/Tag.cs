/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :테그 전용 / 자주쓰는 스트링  + Tag 
/// </summary>
public class Tag 
{
    public static readonly string Player = "Player";
    public static readonly string Enemy = "Enemy";
    public static readonly string UI = "UI";
    public static readonly string Boss = "Boss";
    public static readonly string MainCamera = "MainCamera";
    public static readonly string Untagged = "Untagged";
    public static readonly string MAX = "MAX";
    public static readonly string Gold = "Gold";
    public static readonly string Dia = "Dia";
    public static readonly string PET_UNLOCK = "PET_UNLOCK";
    public static readonly string WING_UNLOCK = "WING_UNLOCK";


    public static readonly string Empty = "Empty";

    public static readonly string Mage = "Mage";

    public static readonly string Gurdian = "Gurdian";

    public static readonly string Archer = "Archer";

    #region 화살표 문자
    public static readonly string UpArrowChar = "↑";
    public static readonly string RightArrowChar = "→";
    #endregion

    #region 색상 컬러
    public static readonly string TEXT_RED1 = "TEXT_RED1";
    #endregion

    #region Constrains
    public static readonly string MAX_LEVEL = "MAX_LEVEL";
    public static readonly string MAX_STAGELEVEL = "MAX_STAGELEVEL";
    public static readonly string MAX_GOLD = "MAX_GOLD";
    public static readonly string MAX_DIA = "MAX_DIA";
    public static readonly string MAX_SYSTEM_NOTICE = "MAX_SYSTEM_NOTICE";
    public static readonly string MAX_UPGRADELEVEL = "MAX_UPGRADELEVEL";
    public static readonly string MAX_HUNTER_COUNT = "MAX_HUNTER_COUNT";
    public static readonly string HUNTER_OPEN_NUMBER = "HUNTER_OPEN_NUMBER";
    public static readonly string FIGHTABLE_NUMBER = "FIGHTABLE_NUMBER";
    public static readonly string HUNTER_EQUIP_COOLDOWN = "HUNTER_EQUIP_COOLDOWN";
    public static readonly string HUNTER_UNEQUIP_COOLDOWN = "HUNTER_UNEQUIP_COOLDOWN";
    public static readonly string MAXIAM_ITEMDRAWER_GRADE = "MAXIAM_ITEMDRAWER_GRADE";
    public static readonly string HUNTER_RESURRECTION_TIME = "HUNTER_RESURRECTION_TIME";
    public static readonly string MAX_PROMOTION_LEVEL = "MAX_PROMOTION_LEVEL";
    public static readonly string HUNTER_RESURRECTION_COEFFICIENT = "HUNTER_RESURRECTION_COEFFICIENT";
    public static readonly string HUNTER_RESURRECTION_MAX = "HUNTER_RESURRECTION_MAX";
    #endregion

    #region 유저 스텟 
    public static readonly string AttackSpeed = "AttackSpeed";
    public static readonly string AttackSpeedPercent = "AttackSpeedPercent";
    public static readonly string MoveSpeed = "MoveSpeed";
    public static readonly string MoveSpeedPercent = "MoveSpeedPercent";
    public static readonly string GroupMoveSpeed = "GroupMoveSpeed";
    public static readonly string GroupMoveSpeedPercent = "GroupMoveSpeedPercent";
    public static readonly string AttackRange = "AttackRange";
    public static readonly string PhysicalPower = "PhysicalPower";
    public static readonly string PhysicalPowerPercent = "PhysicalPowerPercent";
    public static readonly string MagicPower = "MagicPower";
    public static readonly string MagicPowerPercent = "MagicPowerPercent";
    public static readonly string PhysicalTrueDamage = "PhysicalTrueDamage";
    public static readonly string PhysicalTrueDamagePercent = "PhysicalTrueDamagePercent";
    public static readonly string MagicTrueDamage = "MagicTrueDamage";
    public static readonly string MagicTrueDamagePercent = "MagicTrueDamagePercent";
    public static readonly string CriChance = "CriChance";
    public static readonly string CriDamage = "CriDamage";
    public static readonly string CoupChance = "CoupChance";
    public static readonly string CoupDamage = "CoupDamage";
    public static readonly string PhysicalPowerDefense = "PhysicalPowerDefense";
    public static readonly string PhysicalPowerDefensePercent = "PhysicalPowerDefensePercent";
    public static readonly string MagicPowerDefense = "MagicPowerDefense";
    public static readonly string MagicPowerDefensePercent = "MagicPowerDefensePercent";
    public static readonly string PhysicalDamageReduction = "PhysicalDamageReduction";
    public static readonly string MagicDamageReduction = "MagicDamageReduction";
    //public static readonly string PhysicalDamageIncrease = "PhysicalDamageIncrease";
    //public static readonly string MagicDamageIncrease = "MagicDamageIncrease";
    public static readonly string AddDamageToNormalMob = "AddDamageToNormalMob";
    public static readonly string AddDamageToBossMob = "AddDamageToBossMob";
    public static readonly string CCResist = "CCResist";
    public static readonly string DodgeChance = "DodgeChance";
    public static readonly string HP = "HP";
    public static readonly string HPPercent = "HPPercent";
    public static readonly string MP = "MP";
    public static readonly string MPPercent = "MPPercent";
    public static readonly string HPDrain = "HPDrain";
    public static readonly string MPDrain = "MPDrain";
    public static readonly string ExpBuff = "ExpBuff";
    public static readonly string GoldBuff = "GoldBuff";
    public static readonly string DefensePower = "DefensePower";
    public static readonly string ArrowCount = "ArrowCount";
    public static readonly string ArrowRange = "ArrowRange";
    public static readonly string InstantKillBossOnBasicAttack = "InstantKillBossOnBasicAttack";
    public static readonly string ReflectRange = "ReflectRange";
    public static readonly string AddDamageArrowRain = "AddDamageArrowRain";
    public static readonly string AddDamageStrongShot = "AddDamageStrongShot";
    public static readonly string AddDamageRapidShot = "AddDamageRapidShot";
    public static readonly string AddDamageElectric = "AddDamageElectric";
    public static readonly string AddDamageDemonicBlow = "AddDamageDemonicBlow";
    public static readonly string AddDamageSpikeTrap = "AddDamageSpikeTrap";
    public static readonly string AddDamageVenomousSting = "AddDamageVenomousSting";
    public static readonly string ChainLightningNumber = "ChainLightningNumber";
    public static readonly string LastLeafAblityNumber = "LastLeafAblityNumber";
    public static readonly string MagneticWavesRangeNumber = "MagneticWavesRangeNumber";
    public static readonly string WhirlwindRushSpeedPercent = "WhirlwindRushSpeedPercent";
    public static readonly string AddDamageWhirlwindRush = "AddDamageWhirlwindRush";
    public static readonly string AddDamageDokkaebiFire = "AddDamageDokkaebiFire";
    public static readonly string AddDamageMeteor = "AddDamageMeteor";
    public static readonly string AddDamageElectricshock = "AddDamageElectricshock";
    public static readonly string StunPersent = "StunPersent";
    public static readonly string StunDuration = "StunDuration";
    public static readonly string FrozenPersent = "FrozenPersent";
    public static readonly string FrozenDuration = "FrozenDuration";
    public static readonly string PosionPersent = "PosionPersent";
    public static readonly string PosionDuration = "PosionDuration";
    public static readonly string PosionDamage = "PosionDamage";
    public static readonly string SlowDuration = "SlowDuration";
    public static readonly string SlowPersent = "SlowPersent";
    public static readonly string MoveSpeedsumpersent = "MoveSpeedsumpersent";
    public static readonly string AttackSpeedsumpersent = "AttackSpeedsumpersent";
    public static readonly string ElectricshockNumber = "ElectricshockNumber";
    public static readonly string ElectricPersent = "ElectricPersent";
    public static readonly string BloodSurge = "BloodSurge";
    public static readonly string ChainLightningPersent = "ChainLightningPersent";
    public static readonly string HpPersent = "HpPersent";
    public static readonly string HpDuration = "HpDuration";
    public static readonly string BeastTransformationStackCountReduce = "BeastTransformationStackCountReduce";
    public static readonly string ContinuousHitStackCountReduce = "ContinuousHitStackCountReduce";
    public static readonly string DokkaebiFireRotationSpeed = "DokkaebiFireRotationSpeed";
    #endregion

    #region 스폰
    public static readonly string LastLeaf_Off = "LastLeaf_Off";
    public static readonly string EnemyTestProjectile = "EnemyTestProjectile";
    public static readonly string Arrow = "Arrow";
    public static readonly string SpellBall = "SpellBall";
    public static readonly string FocusArrow = "FocusArrow";
    public static readonly string DemonicBlow = "DemonicBlow";
    public static readonly string VenomousSting = "VenomousSting";
    public static readonly string VenomousStingDamageRadiusAttackFx = "VenomousStingDamageRadiusAttackFx";
    public static readonly string SkullFx = "SkullFx";
    public static readonly string SpikeTrap = "SpikeTrap";
    public static readonly string SpikeTrapAOE = "SpikeTrapAOE";
    public static readonly string LastLeaf = "LastLeaf";
    public static readonly string ShroudRay = "ShroudRay";
    public static readonly string ProtectionBless = "ProtectionBless";
    public static readonly string FocusFx = "FocusFx";
    public static readonly string WeaknessDetectionFx = "WeaknessDetectionFx";
    public static readonly string MaximumDose = "MaximumDose";
    public static readonly string BeastTransformationFx = "BeastTransformationFx";
    public static readonly string RageShot = "RageShot";
    public static readonly string CriticalShot = "CriticalShot";
    public static readonly string RageAttack = "RageAttack";
    public static readonly string DodgeBoostFx = "DodgeBoostFx";
    public static readonly string SplitArrow = "SplitArrow";
    public static readonly string FragileArrow = "FragileArrow";
    public static readonly string SplitPiercing = "SplitPiercing";
    public static readonly string SplitPiercing_Split = "SplitPiercing_Split";
    public static readonly string ElectricEffectFx = "ElectricEffectFx";
    public static readonly string ArrowRain = "ArrowRain";
    public static readonly string StrongArrow = "StrongArrow";
    public static readonly string RapidArrow = "RapidArrow";
    public static readonly string RapidShot_Explosion = "RapidShot_Explosion";
    public static readonly string MagneticWaves = "MagneticWaves";
    public static readonly string HammerSummon = "HammerSummon";
    public static readonly string BloodSurgeEffect = "BloodSurgeEffect";
    public static readonly string GuardianShield = "GuardianShield";
    public static readonly string GuardianShield_Damaged = "GuardianShield_Damaged";
    public static readonly string WhirlwindRushFx = "WhirlwindRushFx";
    public static readonly string DokkaebiFireFx = "DokkaebiFireFx";
    public static readonly string CoinFilp = "CoinFilp";
    public static readonly string CoinBomb = "CoinBomb";
    public static readonly string ChainLightning = "ChainLightning";
    public static readonly string Chain = "Chain";
    public static readonly string FirstChain = "FirstChain";
    public static readonly string StrongShot = "StrongShot";
    public static readonly string RapidShot = "RapidShot";
    public static readonly string Electric = "Electric";
    public static readonly string WhirlwindRush = "WhirlwindRush";
    public static readonly string DokkaebiFire = "DokkaebiFire";
    public static readonly string Meteor = "Meteor";
    public static readonly string Electricshock = "Electricshock"; 
    public static readonly string ElectricCastingChance = "ElectricCastingChance"; 
    public static readonly string SplitPiercingNumber = "SplitPiercingNumber"; 
    public static readonly string SpikeTrapNumber = "SpikeTrapNumber"; 
    public static readonly string SpikeTrapDamageRadius = "SpikeTrapDamageRadius"; 
    public static readonly string VenomousStingNumber = "VenomousStingNumber"; 
    public static readonly string VenomousStingDamageRadius = "VenomousStingDamageRadius";
    public static readonly string EnemyHitEffect = "EnemyHitEffect";
    public static readonly string KillDashEffect = "KillDashEffect";
    public static readonly string HunterStance = "HunterStance";
    public static readonly string ForceField = "ForceField";
    public static readonly string ForceField_Hit = "ForceField_Hit";
    public static readonly string ChainPrefab = "ChainPrefab";
    #endregion

    #region 포멧
    public static readonly string ChatFormatStr = "<color=#{0}>{1}</color> {2} : {3}";   //채팅 입력 기본 포맷
    #endregion

    #region 스킬
    public static readonly string Lighgterblue = "Lighgterblue";
    public static readonly string Lighgterred = "Lighgterred";
    public static readonly string Lighgteryellow = "Lighgteryellow";
    public static readonly string RageShot_Off = "RageShot_Off";
    public static readonly string CriticalShot_Off = "CriticalShot_Off";
    public static readonly string CoinFilp_Buff = "CoinFilp_Buff";
    public static readonly string RapidExplosion = "RapidExplosion";
    #endregion

    #region 추출기 등급 이미지
    public static readonly string ItemDrawer_Normal = "ItemDrawer_Normal";
    public static readonly string ItemDrawer_Superior = "ItemDrawer_Superior";
    public static readonly string ItemDrawer_Rare = "ItemDrawer_Rare";
    public static readonly string ItemDrawer_Unique = "ItemDrawer_Unique";
    public static readonly string ItemDrawer_Epic = "ItemDrawer_Epic";
    public static readonly string ItemDrawer_Hero = "ItemDrawer_Hero";
    public static readonly string ItemDrawer_Ancient = "ItemDrawer_Ancient";
    public static readonly string ItemDrawer_Abyssal = "ItemDrawer_Abyssal";
    #endregion

}
