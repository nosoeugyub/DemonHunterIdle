using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.Rendering.VolumeComponent;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 헌터스텟
/// </summary>
public class HunterStat
{
    public Utill_Enum.SubClass SubClass; //혹시몰라 준비한 스텟 이름;;



    public Utill_Enum.HeroClass Class;
    public Utill_Enum.Enum_AttackType AttacckType;
    public Utill_Enum.AttackDamageType AttackDamageType;


    //헌터 일반스탯
    public float AttackSpeed; //초당 공격횟수수 
    public float AttackSpeedPercent; // 초당공격횟수 %증가량

    public float MoveSpeed; //이동속도
    public float MoveSpeedPercent; //이동속도 %증가량

    public float GroupMoveSpeed; //대형 이동속도
    public float GroupMoveSpeedPercent; //대형 이동속도 %증가량

    public float AttackRange;// 공격시 사정거리

    public float PhysicalPower; //물리공격
    public float PhysicalPowerPercent; //물리공격증가

    public float MagicPower;//마법공격
    public float MagicPowerPercent;//마법공격 증가


    public float CriChance;//치명타확률
    public float CriDamage;//치명타 피해

    public float CoupChance;//일격확률
    public float CoupDamage;//일격 피해

    public float PhysicalPowerDefense;//물리공격 방어력
    public float PhysicalPowerDefensePercent; //물리공격 방어력 증가

    public float MagicPowerDefense;//마법공격 방어력
    public float MagicPowerDefensePercent;//마법공격 방어력 증가

    public float PhysicalDamageReduction; //물리공격 피해감소
    public float MagicDamageReduction; //마법공격 피해 감소

    //public float PhysicalDamageIncrease; // 물리공격 피해증가
    //public float MagicDamageIncrease; //마법공격 피해증가

    public float AddDamageToNormalMob; // 일반몹에게 주는 평타피해 추가
    public float AddDamageToBossMob; // 보스몹에게 주는 평타피해 추가

    public float CCResist;//상태이상회복률
    public float DodgeChance; //회피율

    public float HP;//체력
    public float HPPercent;

    public float MP; //마나
    public float MPPercent;

    public float HPDrain; //체력 흡수
    public float MPDrain; //마나 흡수

    public float ExpBuff; //경험치획득 버프
    public float Exp; //경험치

    public float GoldBuff; //골드획득 버프

    public float Level; // 레벨



    //헌터 특수스탯
    public float PhysicalTrueDamage;//물리 관통피해
    public float PhysicalTrueDamagePercent;//물리 관통피해 증가

    public float MagicTrueDamage;//마법 관통피해
    public float MagicTrueDamagePercent;//마법 관통피해 증가

    public float InstantKillBossOnBasicAttack;//기본공격시 일정확률로 보스 즉사
    public float ReflectRange;//원거리 공격 반사

    #region InstantKillBossOnBasicAttack 스텟 로직
    public static float Get_InstantKillBossOnBasicAttack(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.InstantKillBossOnBasicAttack, Tag.InstantKillBossOnBasicAttack);
            return tmp;
        }
        else
        {
            float tmp = user.InstantKillBossOnBasicAttack;
            return tmp;
        }
    }

    public static void Add_InstantKillBossOnBasicAttack(HunterStat user, float value)
    {
        user.InstantKillBossOnBasicAttack += value;
    }
    public static void Remove_InstantKillBossOnBasicAttack(HunterStat user, float value)
    {
        user.InstantKillBossOnBasicAttack -= value;
    }

    public static void init_InstantKillBossOnBasicAttack(HunterStat user)
    {
        user.InstantKillBossOnBasicAttack = 0;
    }

    public static void Set_InstantKillBossOnBasicAttack(HunterStat user, float value)
    {
        user.InstantKillBossOnBasicAttack = value;
    }
    #endregion
    #region ReflectRange 스텟 로직
    public static float Get_ReflectRange(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.ReflectRange, Tag.ReflectRange);
            return tmp;
        }
        else
        {
            float tmp = user.ReflectRange;
            return tmp;
        }
    }
    public static void Add_ReflectRange(HunterStat user, float value)
    {
        user.ReflectRange += value;
    }
    public static void Remove_ReflectRange(HunterStat user, float value)
    {
        user.ReflectRange -= value;
    }

    public static void init_ReflectRange(HunterStat user)
    {
        user.ReflectRange = 0;
    }

    public static void Set_ReflectRange(HunterStat user, float value)
    {
        user.ReflectRange = value;
    }
    #endregion

    //-궁수 특수스탯
    public int ArrowCount; //궁수 화살개수
    public int ElectricCastingChance; //벼락치기 발동 확률 증가 n
    public int SplitPiercingNumber; //분열관통 분열 수 증가 n
    public int SpikeTrapNumber; //가시덫 투사체 발사 개수 증가 n
    public float SpikeTrapDamageRadius; //가시덫 장판 범위 증가 n
    public int VenomousStingNumber; //맹독침 투사체 발사 개수 증가 n
    public float VenomousStingDamageRadius; //맹독침 폭발 범위 증가 n

    #region ArrowCount 스텟 로직
    public static int Get_ArrowCount(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            int tmp = (int)GetMaximumStat(user.ArrowCount, Tag.ArrowCount);
            return tmp;
        }
        else
        {
            int tmp = user.ArrowCount;
            return tmp;
        }
    }

    public static void Add_ArrowCount(HunterStat user, int value)
    {
        user.ArrowCount += value;
    }
    public static void Remove_ArrowCount(HunterStat user, int value)
    {
        user.ArrowCount -= value;
    }

    public static void init_ArrowCount(HunterStat user)
    {
        user.ArrowCount = 0;
    }

    public static void Set_ArrowCount(HunterStat user, int value)
    {
        user.ArrowCount = value;
    }
    #endregion

    #region ElectricCastingChance 스텟 로직
    public static int Get_ElectricCastingChance(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            int tmp = (int)GetMaximumStat(user.ElectricCastingChance, Tag.ElectricCastingChance);
            return tmp;
        }
        else
        {
            int tmp = user.ElectricCastingChance;
            return tmp;
        }
    }

    public static void Add_ElectricCastingChance(HunterStat user, int value)
    {
        user.ElectricCastingChance += value;
    }
    public static void Remove_ElectricCastingChance(HunterStat user, int value)
    {
        user.ElectricCastingChance -= value;
    }

    public static void init_ElectricCastingChance(HunterStat user)
    {
        user.ElectricCastingChance = 0;
    }

    public static void Set_ElectricCastingChance(HunterStat user, int value)
    {
        user.ElectricCastingChance = value;
    }
    #endregion

    #region SplitPiercingNumber 스텟 로직
    public static int Get_SplitPiercingNumber(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            int tmp = (int)GetMaximumStat(user.SplitPiercingNumber, Tag.SplitPiercingNumber);
            return tmp;
        }
        else
        {
            int tmp = user.SplitPiercingNumber;
            return tmp;
        }
    }

    public static void Add_SplitPiercingNumber(HunterStat user, int value)
    {
        user.SplitPiercingNumber += value;
    }
    public static void Remove_SplitPiercingNumber(HunterStat user, int value)
    {
        user.SplitPiercingNumber -= value;
    }

    public static void init_SplitPiercingNumber(HunterStat user)
    {
        user.SplitPiercingNumber = 0;
    }

    public static void Set_SplitPiercingNumber(HunterStat user, int value)
    {
        user.SplitPiercingNumber = value;
    }
    #endregion

    #region SpikeTrapNumber 스텟 로직
    public static int Get_SpikeTrapNumber(HunterStat user, bool useMaxLimit = true)
    {
        int tmp = (int)GetMaximumStat(user.SpikeTrapNumber, Tag.SpikeTrapNumber);
        return tmp;
    }

    public static void Add_SpikeTrapNumber(HunterStat user, int value)
    {
        user.SpikeTrapNumber += value;
    }
    public static void Remove_SpikeTrapNumber(HunterStat user, int value)
    {
        user.SpikeTrapNumber -= value;
    }

    public static void init_SpikeTrapNumber(HunterStat user)
    {
        user.SpikeTrapNumber = 0;
    }

    public static void Set_SpikeTrapNumber(HunterStat user, int value)
    {
        user.SpikeTrapNumber = value;
    }
    #endregion

    #region SpikeTrapDamageRadius 스텟 로직
    public static float Get_SpikeTrapDamageRadius(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.SpikeTrapDamageRadius, Tag.SpikeTrapDamageRadius);
            return tmp;
        }
        else
        {
            float tmp = user.SpikeTrapDamageRadius;
            return tmp;
        }
    }

    public static void Add_SpikeTrapDamageRadius(HunterStat user, float value)
    {
        user.SpikeTrapDamageRadius += value;
    }
    public static void Remove_SpikeTrapDamageRadius(HunterStat user, float value)
    {
        user.SpikeTrapDamageRadius -= value;
    }

    public static void init_SpikeTrapDamageRadius(HunterStat user)
    {
        user.SpikeTrapDamageRadius = 0;
    }

    public static void Set_SpikeTrapDamageRadius(HunterStat user, float value)
    {
        user.SpikeTrapDamageRadius = value;
    }
    #endregion

    #region VenomousStingNumber 스텟 로직
    public static int Get_VenomousStingNumber(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            int tmp = (int)GetMaximumStat(user.VenomousStingNumber, Tag.VenomousStingNumber);
            return tmp;
        }
        else
        {
            int tmp = user.VenomousStingNumber;
            return tmp;
        }
    }

    public static void Add_VenomousStingNumber(HunterStat user, int value)
    {
        user.VenomousStingNumber += value;
    }
    public static void Remove_VenomousStingNumber(HunterStat user, int value)
    {
        user.VenomousStingNumber -= value;
    }

    public static void init_VenomousStingNumber(HunterStat user)
    {
        user.VenomousStingNumber = 0;
    }

    public static void Set_VenomousStingNumber(HunterStat user, int value)
    {
        user.VenomousStingNumber = value;
    }
    #endregion

    #region VenomousStingDamageRadius 스텟 로직
    public static float Get_VenomousStingDamageRadius(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.VenomousStingDamageRadius, Tag.VenomousStingDamageRadius);
            return tmp;
        }
        else
        {
            float tmp = user.VenomousStingDamageRadius;
            return tmp;
        }
    }

    public static void Add_VenomousStingDamageRadius(HunterStat user, float value)
    {
        user.VenomousStingDamageRadius += value;
    }
    public static void Remove_VenomousStingDamageRadius(HunterStat user, float value)
    {
        user.VenomousStingDamageRadius -= value;
    }

    public static void init_VenomousStingDamageRadius(HunterStat user)
    {
        user.VenomousStingDamageRadius = 0;
    }

    public static void Set_VenomousStingDamageRadius(HunterStat user, float value)
    {
        user.VenomousStingDamageRadius = value;
    }
    #endregion


    #region AddDamageArrowRain 스텟 로직
    public static float Get_AddDamageArrowRain(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageArrowRain, Tag.AddDamageArrowRain);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageArrowRain;
            return tmp;
        }
    }

    public static void Add_AddDamageArrowRain(HunterStat user, float value)
    {
        user.AddDamageArrowRain += value;
    }
    public static void Remove_AddDamageArrowRain(HunterStat user, float value)
    {
        user.AddDamageArrowRain -= value;
    }

    public static void init_AddDamageArrowRain(HunterStat user)
    {
        user.AddDamageArrowRain = 0;
    }

    public static void Set_AddDamageArrowRain(HunterStat user, float value)
    {
        user.AddDamageArrowRain = value;
    }
    #endregion

    #region AddDamageStrongShot 스텟 로직
    public static float Get_AddDamageStrongShot(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageStrongShot, Tag.AddDamageStrongShot);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageStrongShot;
            return tmp;
        }
    }

    public static void Add_AddDamageStrongShot(HunterStat user, float value)
    {
        user.AddDamageStrongShot += value;
    }
    public static void Remove_AddDamageStrongShot(HunterStat user, float value)
    {
        user.AddDamageStrongShot -= value;
    }

    public static void init_AddDamageStrongShot(HunterStat user)
    {
        user.AddDamageStrongShot = 0;
    }

    public static void Set_AddDamageStrongShot(HunterStat user, float value)
    {
        user.AddDamageStrongShot = value;
    }
    #endregion

    #region AddDamageRapidShot 스텟 로직
    public static float Get_AddDamageRapidShot(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageRapidShot, Tag.AddDamageRapidShot);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageRapidShot;
            return tmp;
        }
    }

    public static void Add_AddDamageRapidShot(HunterStat user, float value)
    {
        user.AddDamageRapidShot += value;
    }
    public static void Remove_AddDamageRapidShot(HunterStat user, float value)
    {
        user.AddDamageRapidShot -= value;
    }

    public static void init_AddDamageRapidShot(HunterStat user)
    {
        user.AddDamageRapidShot = 0;
    }

    public static void Set_AddDamageRapidShot(HunterStat user, float value)
    {
        user.AddDamageRapidShot = value;
    }
    #endregion

    #region AddDamageElectric 스텟 로직
    public static float Get_AddDamageElectric(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageElectric, Tag.AddDamageElectric);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageElectric;
            return tmp;
        }
    }

    public static void Add_AddDamageElectric(HunterStat user, float value)
    {
        user.AddDamageElectric += value;
    }
    public static void Remove_AddDamageElectric(HunterStat user, float value)
    {
        user.AddDamageElectric -= value;
    }

    public static void init_AddDamageElectric(HunterStat user)
    {
        user.AddDamageElectric = 0;
    }

    public static void Set_AddDamageElectric(HunterStat user, float value)
    {
        user.AddDamageElectric = value;
    }
    #endregion

    #region AddDamageDemonicBlow 스텟 로직
    public static float Get_AddDamageDemonicBlow(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageDemonicBlow, Tag.AddDamageDemonicBlow);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageDemonicBlow;
            return tmp;
        }
    }

    public static void Add_AddDamageDemonicBlow(HunterStat user, float value)
    {
        user.AddDamageDemonicBlow += value;
    }
    public static void Remove_AddDamageDemonicBlow(HunterStat user, float value)
    {
        user.AddDamageDemonicBlow -= value;
    }

    public static void init_AddDamageDemonicBlow(HunterStat user)
    {
        user.AddDamageDemonicBlow = 0;
    }

    public static void Set_AddDamageDemonicBlow(HunterStat user, float value)
    {
        user.AddDamageDemonicBlow = value;
    }
    #endregion

    #region AddDamageSpikeTrap 스텟 로직
    public static float Get_AddDamageSpikeTrap(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageSpikeTrap, Tag.AddDamageSpikeTrap);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageSpikeTrap;
            return tmp;
        }
    }

    public static void Add_AddDamageSpikeTrap(HunterStat user, float value)
    {
        user.AddDamageSpikeTrap += value;
    }
    public static void Remove_AddDamageSpikeTrap(HunterStat user, float value)
    {
        user.AddDamageSpikeTrap -= value;
    }

    public static void init_AddDamageSpikeTrap(HunterStat user)
    {
        user.AddDamageSpikeTrap = 0;
    }

    public static void Set_AddDamageSpikeTrap(HunterStat user, float value)
    {
        user.AddDamageSpikeTrap = value;
    }
    #endregion

    #region AddDamageVenomousSting 스텟 로직
    public static float Get_AddDamageVenomousSting(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageVenomousSting, Tag.AddDamageVenomousSting);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageVenomousSting;
            return tmp;
        }
    }

    public static void Add_AddDamageVenomousSting(HunterStat user, float value)
    {
        user.AddDamageVenomousSting += value;
    }
    public static void Remove_AddDamageVenomousSting(HunterStat user, float value)
    {
        user.AddDamageVenomousSting -= value;
    }

    public static void init_AddDamageVenomousSting(HunterStat user)
    {
        user.AddDamageVenomousSting = 0;
    }

    public static void Set_AddDamageVenomousSting(HunterStat user, float value)
    {
        user.AddDamageVenomousSting = value;
    }
    #endregion

    //-수호자 특수스텟                 
    public float MagneticWavesRangeNumber; //자기장파 범위 n 증가
    public float LastLeafAblityNumber; // 최후의 저항 발동시 공격력 방어력 퍼센트 증가
    public float ChainLightningNumber; // 연쇄전기 연쇄되는 횟수 증가

    public float ElectricshockNumber; //감전개수 추가
    public int BeastTransformationStackCountReduce; //야수화 필요스택 n 감소
    public int ContinuousHitStackCountReduce; //연타 필요스택 n 감소
    public float DokkaebiFireRotationSpeed; //도깨비불 회전 속도 증가 n
    public float WhirlwindRushSpeedPercent; //회전돌격 공격 속도 증가 %n

    #region ChainLightningNumber 스텟 로직
    public static float Get_ChainLightningNumber(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.ChainLightningNumber, Tag.ChainLightningNumber);
            return tmp;
        }
        else
        {
            float tmp = user.ChainLightningNumber;
            return tmp;
        }
    }

    public static void Add_ChainLightningNumber(HunterStat user, int value)
    {
        user.ChainLightningNumber += value;
    }
    public static void Remove_ChainLightningNumber(HunterStat user, int value)
    {
        user.ChainLightningNumber -= value;
    }

    public static void init_ChainLightningNumber(HunterStat user)
    {
        user.ChainLightningNumber = 0;
    }

    public static void Set_ChainLightningNumber(HunterStat user, int value)
    {
        user.ChainLightningNumber = value;
    }
    #endregion

    #region LastLeafAblityNumber 스텟 로직
    public static float Get_LastLeafAblityNumber(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.LastLeafAblityNumber, Tag.LastLeafAblityNumber);
            return tmp;
        }
        else
        {
            float tmp = user.LastLeafAblityNumber;
            return tmp;
        }
    }

    public static void Add_LastLeafAblityNumber(HunterStat user, int value)
    {
        user.LastLeafAblityNumber += value;
    }
    public static void Remove_LastLeafAblityNumber(HunterStat user, int value)
    {
        user.LastLeafAblityNumber -= value;
    }

    public static void init_LastLeafAblityNumber(HunterStat user)
    {
        user.LastLeafAblityNumber = 0;
    }

    public static void Set_LastLeafAblityNumber(HunterStat user, int value)
    {
        user.LastLeafAblityNumber = value;
    }
    #endregion

    #region MagneticWavesRangeNumber 스텟 로직
    public static float Get_MagneticWavesRangeNumber(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.MagneticWavesRangeNumber, Tag.MagneticWavesRangeNumber);
            return tmp;
        }
        else
        {
            float tmp = user.MagneticWavesRangeNumber;
            return tmp;
        }
    }

    public static void Add_MagneticWavesRangeNumber(HunterStat user, int value)
    {
        user.MagneticWavesRangeNumber += value;
    }
    public static void Remove_MagneticWavesRangeNumber(HunterStat user, int value)
    {
        user.MagneticWavesRangeNumber -= value;
    }

    public static void init_MagneticWavesRangeNumber(HunterStat user)
    {
        user.MagneticWavesRangeNumber = 0;
    }

    public static void Set_MagneticWavesRangeNumber(HunterStat user, int value)
    {
        user.MagneticWavesRangeNumber = value;
    }
    #endregion

    #region ElectricshockNumber 스텟 로직
    public static float Get_ElectricshockNumber(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.ElectricshockNumber, Tag.ElectricshockNumber);
            return tmp;
        }
        else
        {
            float tmp = user.ElectricshockNumber;
            return tmp;
        }
    }

    public static void Add_ElectricshockNumber(HunterStat user, int value)
    {
        user.ElectricshockNumber += value;
    }
    public static void Remove_ElectricshockNumber(HunterStat user, int value)
    {
        user.ElectricshockNumber -= value;
    }

    public static void init_ElectricshockNumber(HunterStat user)
    {
        user.ElectricshockNumber = 0;
    }

    public static void Set_ElectricshockNumber(HunterStat user, int value)
    {
        user.ElectricshockNumber = value;
    }
    #endregion

    #region BeastTransformationStackCountReduce 스텟 로직
    public static int Get_BeastTransformationStackCountReduce(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            int tmp = (int)GetMaximumStat(user.BeastTransformationStackCountReduce, Tag.BeastTransformationStackCountReduce);
            return tmp;
        }
        else
        {
            int tmp = user.BeastTransformationStackCountReduce;
            return tmp;
        }
    }

    public static void Add_BeastTransformationStackCountReduce(HunterStat user, int value)
    {
        user.BeastTransformationStackCountReduce += value;
    }
    public static void Remove_BeastTransformationStackCountReduce(HunterStat user, int value)
    {
        user.BeastTransformationStackCountReduce -= value;
    }

    public static void init_BeastTransformationStackCountReduce(HunterStat user)
    {
        user.BeastTransformationStackCountReduce = 0;
    }

    public static void Set_BeastTransformationStackCountReduce(HunterStat user, int value)
    {
        user.BeastTransformationStackCountReduce = value;
    }
    #endregion

    #region ContinuousHitStackCountReduce 스텟 로직
    public static int Get_ContinuousHitStackCountReduce(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            int tmp = (int)GetMaximumStat(user.ContinuousHitStackCountReduce, Tag.ContinuousHitStackCountReduce);
            return tmp;
        }
        else
        {
            int tmp = user.ContinuousHitStackCountReduce;
            return tmp;
        }
    }

    public static void Add_ContinuousHitStackCountReduce(HunterStat user, int value)
    {
        user.ContinuousHitStackCountReduce += value;
    }
    public static void Remove_ContinuousHitStackCountReduce(HunterStat user, int value)
    {
        user.ContinuousHitStackCountReduce -= value;
    }

    public static void init_ContinuousHitStackCountReduce(HunterStat user)
    {
        user.ContinuousHitStackCountReduce = 0;
    }

    public static void Set_ContinuousHitStackCountReduce(HunterStat user, int value)
    {
        user.ContinuousHitStackCountReduce = value;
    }
    #endregion

    #region DokkaebiFireRotationSpeed 스텟 로직
    public static float Get_DokkaebiFireRotationSpeed(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.DokkaebiFireRotationSpeed, Tag.DokkaebiFireRotationSpeed);
            return tmp;
        }
        else
        {
            float tmp = user.DokkaebiFireRotationSpeed;
            return tmp;
        }
    }

    public static void Add_DokkaebiFireRotationSpeed(HunterStat user, float value)
    {
        user.DokkaebiFireRotationSpeed += value;
    }
    public static void Remove_DokkaebiFireRotationSpeed(HunterStat user, float value)
    {
        user.DokkaebiFireRotationSpeed -= value;
    }

    public static void init_DokkaebiFireRotationSpeed(HunterStat user)
    {
        user.DokkaebiFireRotationSpeed = 0;
    }

    public static void Set_DokkaebiFireRotationSpeed(HunterStat user, float value)
    {
        user.DokkaebiFireRotationSpeed = value;
    }
    #endregion

    #region WhirlwindRushSpeedPercent 스텟 로직
    public static float Get_WhirlwindRushSpeedResult(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float whirlwindRushSpeedper = GetMaximumStat(user.WhirlwindRushSpeedPercent, Tag.WhirlwindRushSpeedPercent) * 0.01f;
            float whirlwindRushSpeed = SkillManager.Instance.whirlwindRush.GetTickInterval;
            temp = GetMaximumStat(whirlwindRushSpeed + (whirlwindRushSpeed * whirlwindRushSpeedper), Tag.WhirlwindRushSpeedPercent);
            return temp;
        }
        else
        {
            float temp = 0;
            float whirlwindRushSpeedper = user.WhirlwindRushSpeedPercent * 0.01f;
            float whirlwindRushSpeed = SkillManager.Instance.whirlwindRush.GetTickInterval;
            temp = whirlwindRushSpeed + (whirlwindRushSpeed * whirlwindRushSpeedper);
            return temp;
        }
    }

    public static void Add_WhirlwindRushSpeedPercent(HunterStat user, float value)
    {
        user.WhirlwindRushSpeedPercent += value;
    }
    public static void Remove_WhirlwindRushSpeedPercent(HunterStat user, float value)
    {
        user.WhirlwindRushSpeedPercent -= value;
    }

    public static void init_WhirlwindRushSpeedPercent(HunterStat user)
    {
        user.WhirlwindRushSpeedPercent = 0;
    }

    public static void Set_WhirlwindRushSpeedPercent(HunterStat user, float value)
    {
        user.WhirlwindRushSpeedPercent = value;
    }
    #endregion

    #region AddDamageChainLightning 스텟 로직
    public static float Get_AddDamageChainLightning(HunterStat user)
    {
        float tmp = user.AddDamageChainLightning;
        return tmp;
    }

    public static void Add_AddDamageChainLightning(HunterStat user, float value)
    {
        user.AddDamageChainLightning += value;
    }
    public static void Remove_AddDamageChainLightning(HunterStat user, float value)
    {
        user.AddDamageChainLightning -= value;
    }

    public static void init_AddDamageChainLightning(HunterStat user)
    {
        user.AddDamageChainLightning = 0;
    }

    public static void Set_AddDamageChainLightning(HunterStat user, float value)
    {
        user.AddDamageChainLightning = value;
    }
    #endregion

    #region AddDamageHammerSummon 스텟 로직
    public static float Get_AddDamageHammerSummon(HunterStat user)
    {
        float tmp = user.AddDamageHammerSummon;
        return tmp;
    }

    public static void Add_AddDamageHammerSummon(HunterStat user, float value)
    {
        user.AddDamageHammerSummon += value;
    }
    public static void Remove_AddDamageHammerSummon(HunterStat user, float value)
    {
        user.AddDamageHammerSummon -= value;
    }

    public static void init_AddDamageHammerSummon(HunterStat user)
    {
        user.AddDamageHammerSummon = 0;
    }

    public static void Set_AddDamageHammerSummon(HunterStat user, float value)
    {
        user.AddDamageHammerSummon = value;
    }
    #endregion

    #region AddDamageMagneticWaves 스텟 로직
    public static float Get_AddDamageMagneticWaves(HunterStat user)
    {
        float tmp = user.AddDamageMagneticWaves;
        return tmp;
    }

    public static void Add_AddDamageMagneticWaves(HunterStat user, float value)
    {
        user.AddDamageMagneticWaves += value;
    }
    public static void Remove_AddDamageMagneticWaves(HunterStat user, float value)
    {
        user.AddDamageMagneticWaves -= value;
    }

    public static void init_AddDamageMagneticWaves(HunterStat user)
    {
        user.AddDamageMagneticWaves = 0;
    }

    public static void Set_AddDamageMagneticWaves(HunterStat user, float value)
    {
        user.AddDamageMagneticWaves = value;
    }
    #endregion

    #region AddDamageWhirlwindRush 스텟 로직
    public static float Get_AddDamageWhirlwindRush(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageWhirlwindRush, Tag.AddDamageWhirlwindRush);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageWhirlwindRush;
            return tmp;
        }
    }

    public static void Add_AddDamageWhirlwindRush(HunterStat user, float value)
    {
        user.AddDamageWhirlwindRush += value;
    }
    public static void Remove_AddDamageWhirlwindRush(HunterStat user, float value)
    {
        user.AddDamageWhirlwindRush -= value;
    }

    public static void init_AddDamageWhirlwindRush(HunterStat user)
    {
        user.AddDamageWhirlwindRush = 0;
    }

    public static void Set_AddDamageWhirlwindRush(HunterStat user, float value)
    {
        user.AddDamageWhirlwindRush = value;
    }
    #endregion

    #region AddDamageDokkaebiFire 스텟 로직
    public static float Get_AddDamageDokkaebiFire(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageDokkaebiFire, Tag.AddDamageDokkaebiFire);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageDokkaebiFire;
            return tmp;
        }
    }

    public static void Add_AddDamageDokkaebiFire(HunterStat user, float value)
    {
        user.AddDamageDokkaebiFire += value;
    }
    public static void Remove_AddDamageDokkaebiFire(HunterStat user, float value)
    {
        user.AddDamageDokkaebiFire -= value;
    }

    public static void init_AddDamageDokkaebiFire(HunterStat user)
    {
        user.AddDamageDokkaebiFire = 0;
    }

    public static void Set_AddDamageDokkaebiFire(HunterStat user, float value)
    {
        user.AddDamageDokkaebiFire = value;
    }
    #endregion

    #region AddDamageMeteor 스텟 로직
    public static float Get_AddDamageMeteor(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageMeteor, Tag.AddDamageMeteor);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageMeteor;
            return tmp;
        }
    }

    public static void Add_AddDamageMeteor(HunterStat user, float value)
    {
        user.AddDamageMeteor += value;
    }
    public static void Remove_AddDamageMeteor(HunterStat user, float value)
    {
        user.AddDamageMeteor -= value;
    }

    public static void init_AddDamageMeteor(HunterStat user)
    {
        user.AddDamageMeteor = 0;
    }

    public static void Set_AddDamageMeteor(HunterStat user, float value)
    {
        user.AddDamageMeteor = value;
    }
    #endregion

    #region AddDamageElectricshock 스텟 로직
    public static float Get_AddDamageElectricshock(HunterStat user, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(user.AddDamageElectricshock, Tag.AddDamageElectricshock);
            return tmp;
        }
        else
        {
            float tmp = user.AddDamageElectricshock;
            return tmp;
        }
    }

    public static void Add_AddDamageElectricshock(HunterStat user, float value)
    {
        user.AddDamageElectricshock += value;
    }
    public static void Remove_AddDamageElectricshock(HunterStat user, float value)
    {
        user.AddDamageElectricshock -= value;
    }

    public static void init_AddDamageElectricshock(HunterStat user)
    {
        user.AddDamageElectricshock = 0;
    }

    public static void Set_AddDamageElectricshock(HunterStat user, float value)
    {
        user.AddDamageElectricshock = value;
    }
    #endregion


    //-법사 특수스탯

    #region 추가데미지
    public float AddDamageArrowRain; //화살비 추가데미지
    public float AddDamageChainLightning; //연쇄전기 추가데미지
    public float AddDamageHammerSummon; //망치 추가데미지
    public float AddDamageMagneticWaves; //자기장파 추가데미지
    public float AddDamageStrongShot; //정조준일격 추가데미지
    public float AddDamageRapidShot; //래피드샷 추가데미지
    public float AddDamageElectric; //벼락치기 추가데미지
    public float AddDamageVenomousSting; //맹독침 추가데미지
    public float AddDamageSpikeTrap; //가시덫 추가데미지
    public float AddDamageDemonicBlow; //악귀의일격 추가데미지
    public float AddDamageWhirlwindRush; //회전돌격 추가데미지
    public float AddDamageDokkaebiFire; //도깨비불 추가데미지
    public float AddDamageMeteor; //메테오 추가데미지
    public float AddDamageElectricshock; //감전 추가데미지

    #endregion






    public float StunPersent;
    public float FrozenPersent;
    public float PosionPersent;
    public float SlowPersent;
    public float HpPersent;


    public float StunDuration;
    public float FrozenDuration;
    public float PosionDuration;
    public float SlowDuration;
    public float HpDuration;


    public float PosionDamage;


    public float MoveSpeedsumpersent; //이속얼마나 느려질꺼닞 %
    public float AttackSpeedsumpersent; //공속 얼마나 느려질껀지 %
    public float ElectricPersent;
    public float ChainLightningPersent;
    #region 스턴
    public static void init_StunPersent(HunterStat stat)
    {
        stat.StunPersent = 0;
        stat.StunDuration = 0;
    }

    public static void Plus_StunPersent(HunterStat stat, float value, float duration)
    {
        stat.StunPersent += value;
        stat.StunDuration += duration;
    }
    public static void Miuse_StunPersent(HunterStat stat, float value, float duration)
    {
        stat.StunPersent -= value;
        stat.StunDuration -= duration;
    }

    public static float Get_StunPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.StunPersent, Tag.StunPersent);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.StunPersent;
            return temp_present;
        }
    }

    public static float Get_StunDuration(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.StunDuration, Tag.StunDuration);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.StunDuration;
            return temp_present;
        }
    }



    public static void Set_StunPersent(HunterStat stat, float value, float duration)
    {
        stat.StunPersent = value;
        stat.StunPersent = duration;
    }
    #endregion
    #region 빙결
    public static void init_FrozenPersent(HunterStat stat)
    {
        stat.FrozenPersent = 0;
        stat.FrozenDuration = 0;
    }

    public static void Plus_FrozenPersent(HunterStat stat, float value, float duration)
    {
        stat.FrozenPersent += value;
        stat.FrozenDuration += duration;
    }
    public static void Miuse_FrozenPersent(HunterStat stat, float value, float duration)
    {
        stat.FrozenPersent -= value;
        stat.FrozenDuration -= duration;
    }

    public static float Get_FrozenPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.FrozenPersent, Tag.FrozenPersent);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.FrozenPersent;
            return temp_present;
        }
    }

    public static float Get_FrozenDuration(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.FrozenDuration, Tag.FrozenDuration);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.FrozenDuration;
            return temp_present;
        }
    }

    public static void Set_FrozenPersent(HunterStat stat, float value, float duration)
    {
        stat.FrozenPersent = value;
        stat.FrozenDuration = duration;
    }
    #endregion
    #region 맹독
    public static void init_PosionPersent(HunterStat stat)
    {
        stat.PosionPersent = 0;
        stat.PosionDuration = 0;
        stat.PosionDamage = 0;
    }

    public static void Plus_PosionPersent(HunterStat stat, float value, float duration, float posionDamage)
    {
        stat.PosionPersent += value;
        stat.PosionDuration += duration;
        stat.PosionDamage += posionDamage;
    }
    public static void Miuse_PosionPersent(HunterStat stat, float value, float duration, float posionDamage)
    {
        stat.PosionPersent -= value;
        stat.PosionDuration -= duration;
        stat.PosionDamage -= posionDamage;
    }

    public static float Get_PosionPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.PosionPersent, Tag.PosionPersent);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.PosionPersent;
            return temp_present;
        }
    }

    public static float Get_PosionDuration(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.PosionDuration, Tag.PosionDuration);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.PosionDuration;
            return temp_present;
        }
    }

    public static float Get_PosionDamage(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.PosionDamage, Tag.PosionDamage);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.PosionDamage;
            return temp_present;
        }
    }

    public static void Set_PosionPersent(HunterStat stat, float value, float duration, float posiondamage)
    {
        stat.StunPersent = value;
        stat.PosionDuration = duration;
        stat.PosionDamage = posiondamage;
    }
    #endregion
    #region 둔화
    public static void init_SlowPersent(HunterStat stat)
    {
        stat.SlowPersent = 0;
        stat.SlowDuration = 0;
    }

    public static void Plus_SlowPersent(HunterStat stat, float value, float duration, float _MoveSpeedsumpersent, float _AttackSpeedsumpersent)
    {
        stat.SlowPersent += value;
        stat.SlowDuration += duration;
        stat.MoveSpeedPercent += _MoveSpeedsumpersent;
        stat.AttackSpeedsumpersent += _AttackSpeedsumpersent;
    }
    public static void Miuse_SlowPersent(HunterStat stat, float value, float duration, float _MoveSpeedsumpersent, float _AttackSpeedsumpersent)
    {
        stat.SlowPersent -= value;
        stat.SlowDuration -= duration;
        stat.MoveSpeedPercent -= _MoveSpeedsumpersent;
        stat.AttackSpeedsumpersent -= _AttackSpeedsumpersent;
    }

    public static float Get_SlowDuration(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(stat.SlowDuration, Tag.SlowDuration);
            return tmp;
        }
        else
        {
            float tmp = stat.SlowDuration;
            return tmp;
        }
    }

    public static float Get_SlowPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(stat.SlowPersent, Tag.SlowPersent);
            return tmp;
        }
        else
        {
            float tmp = stat.SlowPersent;
            return tmp;
        }
    }

    public static float Get_movespeedslowtime(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.MoveSpeedsumpersent, Tag.MoveSpeedsumpersent);
            return temp_present;
        }
        else
        {
            float tmp = stat.MoveSpeedsumpersent;
            return tmp;
        }
    }

    public static float Get_attackspeedslowtime(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.AttackSpeedsumpersent, Tag.AttackSpeedsumpersent);
            return temp_present;
        }
        else
        {
            float tmp = stat.AttackSpeedsumpersent;
            return tmp;
        }
    }

    public static void Set_SlowPersent(HunterStat stat, float value, float duration)
    {
        stat.SlowPersent -= value;
        stat.SlowDuration -= duration;
    }
    #endregion

    #region  벼락치기
    public static void init_ElectricPersent(HunterStat stat)
    {
        stat.ElectricPersent = 0;
    }

    public static void Plus_ElectricPersent(HunterStat stat, float value, float duration)
    {
        stat.ElectricPersent += value;
    }
    public static void Miuse_ElectricPersent(HunterStat stat, float value, float duration)
    {
        stat.ElectricPersent -= value;
    }

    public static float Get_ElectricPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp_present = 0;
            temp_present = GetMaximumStat(stat.ElectricPersent, Tag.ElectricPersent) + GetMaximumStat(stat.ElectricCastingChance, Tag.ElectricCastingChance);
            return temp_present;
        }
        else
        {
            float temp_present = 0;
            temp_present = stat.ElectricPersent + stat.ElectricCastingChance;
            return temp_present;
        }
    }

    public static void Set_ElectricPersent(HunterStat stat, float value, float duration)
    {
        stat.ElectricPersent = value;

    }
    #endregion
    #region 연쇄전기
    public static void init_ChainLightningPersent(HunterStat stat)
    {
        stat.ChainLightningPersent = 0;
    }

    public static void Plus_ChainLightningPersent(HunterStat stat, float value, float duration)
    {
        stat.ChainLightningPersent += value;
    }
    public static void Miuse_ChainLightningPersent(HunterStat stat, float value, float duration)
    {
        stat.ChainLightningPersent -= value;
    }

    public static float Get_ChainLightningPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(stat.ChainLightningPersent, Tag.ChainLightningPersent);
            return tmp;
        }
        else
        {
            float tmp = stat.ChainLightningPersent;
            return tmp;
        }
    }

    public static void Set_ChainLightningPersent(HunterStat stat, float value, float duration)
    {
        stat.ChainLightningPersent = value;

    }
    #endregion

    #region 적처치시 체력회복
    public static void init_HpPersent(HunterStat stat)
    {
        stat.HpPersent = 0;
        stat.HpDuration = 0;
    }

    public static void Plus_HpPersent(HunterStat stat, float value)
    {
        stat.HpPersent += value;
    }

    public static void Plus_HpDuration(HunterStat stat, float value)
    {
        stat.HpDuration += value;
    }
    public static void Miuse_HpPersent(HunterStat stat, float value)
    {
        stat.HpPersent -= value;
    }
    public static void Miuse_HpDuration(HunterStat stat, float value)
    {
        stat.HpDuration -= value;
    }



    public static float GetHpPersent(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(stat.HpPersent, Tag.HpPersent);
            return tmp;
        }
        else
        {
            float tmp = stat.HpPersent;
            return tmp;
        }
    }

    public static float GetHpDuration(HunterStat stat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float tmp = GetMaximumStat(stat.HpDuration, Tag.HpDuration);
            return tmp;
        }
        else
        {
            float tmp = stat.HpDuration;
            return tmp;
        }
    }

    public static void Set_HpPersent(HunterStat stat, float value, float duration)
    {
        stat.HPPercent = value;
    }

    public static void Set_HpDuration(HunterStat stat, float value, float duration)
    {
        stat.HpDuration = value;
    }
    #endregion

    /// <summary>
    /// 내부적으로 쓰는 데이터
    /// </summary>

    private float currentHp;
    public float CurrentHp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    public float currentMp;
    public float CurrentMp
    {
        get { return currentMp; }
        set { currentMp = value; }
    }
    public HunterStat()
    {

    }
    public HunterStat(SubClass name, HeroClass @class, Enum_AttackType attacckType, AttackDamageType attacckDamageType, float attackSpeed, float attackSpeedPercent, float moveSpeed, float moveSpeedPercent, float groupMoveSpeed, float groupMoveSpeedPercent, float attackRange,
                      float physicalPower, float physicalPowerPercent, float magicPower, float magicPowerPercent, float physicalTrueDamage, float physicalTrueDamagePercent, float magicTrueDamage, float magicTrueDamagePercent,
                      float criChance, float criDamage, float coupChance, float coupDamage, float physicalPowerDefense, float physicalPowerDefensePercent, float magicPowerDefense, float magicPowerDefensePercent, float physicalDamageReduction, float magicDamageReduction,
                      /*float physicalDamageIncrease, float magicDamageIncrease,*/float addDamageToNormalMob, float addDamageToBossMob, float cCResist, float dodgeChance, float hP, float hPPercent, float mP, float mPPercent, float hPDrain, float mPDrain, float expBuff,
                      float goldBuff, float instantKillBossOnBasicAttack, float reflectRange)
    {
        SubClass = name;
        Class = @class;
        AttacckType = attacckType;
        AttackDamageType = attacckDamageType;
        AttackSpeed = attackSpeed;
        AttackSpeedPercent = attackSpeedPercent;
        MoveSpeed = moveSpeed;
        MoveSpeedPercent = moveSpeedPercent;
        GroupMoveSpeed = groupMoveSpeed;
        GroupMoveSpeedPercent = groupMoveSpeedPercent;
        AttackRange = attackRange;
        PhysicalPower = physicalPower;
        PhysicalPowerPercent = physicalPowerPercent;
        MagicPower = magicPower;
        MagicPowerPercent = magicPowerPercent;
        PhysicalTrueDamage = physicalTrueDamage;
        PhysicalTrueDamagePercent = physicalTrueDamagePercent;
        MagicTrueDamage = magicTrueDamage;
        MagicTrueDamagePercent = magicTrueDamagePercent;
        CriChance = criChance;
        CriDamage = criDamage;
        CoupChance = coupChance;
        CoupDamage = coupDamage;
        PhysicalPowerDefense = physicalPowerDefense;
        PhysicalPowerDefensePercent = physicalPowerDefensePercent;
        MagicPowerDefense = magicPowerDefense;
        MagicPowerDefensePercent = magicPowerDefensePercent;
        PhysicalDamageReduction = physicalDamageReduction;
        MagicDamageReduction = magicDamageReduction;
        //PhysicalDamageIncrease = physicalDamageIncrease;
        //MagicDamageIncrease = magicDamageIncrease;
        AddDamageToNormalMob = addDamageToNormalMob;
        AddDamageToBossMob = addDamageToBossMob;
        CCResist = cCResist;
        DodgeChance = dodgeChance;
        HP = hP;
        HPPercent = hPPercent;
        MP = mP;
        MPPercent = mPPercent;
        HPDrain = hPDrain;
        MPDrain = mPDrain;
        ExpBuff = expBuff;
        GoldBuff = goldBuff;
        InstantKillBossOnBasicAttack = instantKillBossOnBasicAttack;
        ReflectRange = reflectRange;
    }

    public static string GetValueToString(HunterStat _userstat, string name)
    {
        // 타입 정보를 얻음
        Type type = typeof(HunterStat);

        // 필드 정보를 얻음
        FieldInfo field = type.GetField(name);

        // 필드가 존재하면 값을 반환
        if (field != null)
        {
            object value = field.GetValue(_userstat);
            return value.ToString();
        }

        // 필드가 존재하지 않으면 에러 메시지를 반환하거나 null을 반환
        return $"Field '{name}' does not exist.";
    }

    /// <summary>
    /// 스탯의 최대 수치가 적용된 값을 반환
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="statName"></param>
    /// <returns></returns>
    public static float GetMaximumStat(float stat, string statName)
    {
        int max = StatTableData.GetValue_MaximumData(GameDataTable.Instance.StatTableDataDic, statName);

        return ((int)stat > max) ? max : stat;
    }

    /// <summary>
    /// Info 관련
    /// </summary>
    /// <param name="_userstat"></param>
    /// <returns></returns>
    /// 
    #region 유저스텟 구하는 함수

    public static float AttackSpeedResult(HunterStat _userstat, bool useMaxLimit = true) //공격속도 구하는 함수 
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.AttackSpeedPercent, Tag.AttackSpeedPercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.AttackSpeed, Tag.AttackSpeed);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.AttackSpeed);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.AttackSpeedPercent, Tag.AttackSpeedPercent) * 0.01f;
            float attackspeed = _userstat.AttackSpeed;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }

    public static float MoveSpeedResult(HunterStat _userstat, bool useMaxLimit = true)//이동속도 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float moveSpeedper = GetMaximumStat(_userstat.MoveSpeedPercent, Tag.MoveSpeedPercent) * 0.01f;
            float moveSpeed = GetMaximumStat(_userstat.MoveSpeed, Tag.MoveSpeed);
            temp = GetMaximumStat(moveSpeed + (moveSpeed * moveSpeedper), Tag.MoveSpeed);
            return temp;
        }
        else
        {
            float temp = 0;
            float moveSpeedper = GetMaximumStat(_userstat.MoveSpeedPercent, Tag.MoveSpeedPercent) * 0.01f;
            float moveSpeed = _userstat.MoveSpeed;
            temp = moveSpeed + (moveSpeed * moveSpeedper);
            return temp;
        }
    }

    public static float GroupMoveSpeedResult(HunterStat _userstat, bool useMaxLimit = true) //대형 이동속도 구하는 함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float moveSpeedper = GetMaximumStat(_userstat.GroupMoveSpeedPercent, Tag.GroupMoveSpeedPercent) * 0.01f;
            float moveSpeed = GetMaximumStat(_userstat.GroupMoveSpeed, Tag.GroupMoveSpeed);
            temp = GetMaximumStat(moveSpeed + (moveSpeed * moveSpeedper), Tag.GroupMoveSpeed);
            return temp;
        }
        else
        {
            float temp = 0;
            float moveSpeedper = GetMaximumStat(_userstat.GroupMoveSpeedPercent, Tag.GroupMoveSpeedPercent) * 0.01f;
            float moveSpeed = _userstat.GroupMoveSpeed;
            temp = moveSpeed + (moveSpeed * moveSpeedper);
            return temp;
        }
    }

    public static float AttackRangeResult(HunterStat _userstat)//사정거리구하는함수
    {
        float temp = 0;
        float attackRange = GetMaximumStat(_userstat.AttackRange, Tag.AttackRange) * 100f;
        return attackRange;
    }

    public static float PhysicalPowerResult(HunterStat _userstat, bool useMaxLimit = true)//물리공격 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.PhysicalPowerPercent, Tag.PhysicalPowerPercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.PhysicalPower, Tag.PhysicalPower);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.PhysicalPower);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.PhysicalPowerPercent, Tag.PhysicalPowerPercent) * 0.01f;
            float attackspeed = _userstat.PhysicalPower;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }

    public static float MagicPowerResult(HunterStat _userstat, bool useMaxLimit = true)//마법공격 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.MagicPowerPercent, Tag.MagicPowerPercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.MagicPower, Tag.MagicPower);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.MagicPower);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.MagicPowerPercent, Tag.MagicPowerPercent) * 0.01f;
            float attackspeed = _userstat.MagicPower;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }


    public static float PhysicalTrueDamageResult(HunterStat _userstat, bool useMaxLimit = true)//관통피해 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.PhysicalTrueDamagePercent, Tag.PhysicalTrueDamagePercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.PhysicalTrueDamage, Tag.PhysicalTrueDamage);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.PhysicalTrueDamage);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.PhysicalTrueDamagePercent, Tag.PhysicalTrueDamagePercent) * 0.01f;
            float attackspeed = _userstat.PhysicalTrueDamage;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }

    public static float MagicTrueDamageResult(HunterStat _userstat, bool useMaxLimit = true)//마법 관통피해 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.MagicTrueDamagePercent, Tag.MagicTrueDamagePercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.MagicTrueDamage, Tag.MagicTrueDamage);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.MagicTrueDamage);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.MagicTrueDamagePercent, Tag.MagicTrueDamagePercent) * 0.01f;
            float attackspeed = _userstat.MagicTrueDamage;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }

    public static float PhysicalPowerDefenseResult(HunterStat _userstat, bool useMaxLimit = true)//물리공격방어력 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.PhysicalPowerDefensePercent, Tag.PhysicalPowerDefensePercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.PhysicalPowerDefense, Tag.PhysicalPowerDefense);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.PhysicalPowerDefense);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.PhysicalPowerDefensePercent, Tag.PhysicalPowerDefensePercent) * 0.01f;
            float attackspeed = _userstat.PhysicalPowerDefense;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }

    public static float MagicPowerDefenseResult(HunterStat _userstat, bool useMaxLimit = true)//마법공격방어력 구하는함수
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.MagicPowerDefensePercent, Tag.MagicPowerDefensePercent) * 0.01f;
            float attackspeed = GetMaximumStat(_userstat.MagicPowerDefense, Tag.MagicPowerDefense);
            temp = GetMaximumStat(attackspeed + (attackspeed * attackspeedper), Tag.MagicPowerDefense);
            return temp;
        }
        else
        {
            float temp = 0;
            float attackspeedper = GetMaximumStat(_userstat.MagicPowerDefensePercent, Tag.MagicPowerDefensePercent) * 0.01f;
            float attackspeed = _userstat.MagicPowerDefense;
            temp = attackspeed + (attackspeed * attackspeedper);
            return temp;
        }
    }

    public static float HpResult(HunterStat _userstat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float HpPercent = GetMaximumStat(_userstat.HPPercent, Tag.HPPercent) * 0.01f;
            float hp = GetMaximumStat(_userstat.HP, Tag.HP);
            temp = GetMaximumStat(hp + (hp * HpPercent), Tag.HP);
            return temp;
        }
        else
        {
            float temp = 0;
            float HpPercent = GetMaximumStat(_userstat.HPPercent, Tag.HPPercent) * 0.01f;
            float hp = _userstat.HP;
            temp = hp + (hp * HpPercent);
            return temp;
        }
    }

    public static float MpResult(HunterStat _userstat, bool useMaxLimit = true)
    {
        if (useMaxLimit)
        {
            float temp = 0;
            float MpPercent = GetMaximumStat(_userstat.MPPercent, Tag.MPPercent) * 0.01f;
            float mp = GetMaximumStat(_userstat.MP, Tag.MP);
            temp = GetMaximumStat(mp + (mp * MpPercent), Tag.MP);
            return temp;
        }
        else
        {
            float temp = 0;
            float MpPercent = GetMaximumStat(_userstat.MPPercent, Tag.MPPercent) * 0.01f;
            float mp = _userstat.MP;
            temp = mp + (mp * MpPercent);
            return temp;
        }
    }

    public static void Plus_AttackSpeed(HunterStat _userstat, float value) //공속 증가
    {
        _userstat.AttackSpeedPercent += value;
    }

    public static void Miuse_AttackSpeed(HunterStat _userstat, float value) //공속 감소
    {
        _userstat.AttackSpeedPercent -= value;
    }

    public static int ProjectileNumResult(HunterStat _userstat) //서브클래스에 따른 기본 투사체 발사 개수
    {
        switch (_userstat.SubClass)
        {
            case SubClass.Archer:
                return (int)GetMaximumStat(_userstat.ArrowCount, Tag.ArrowCount);
            case SubClass.Guardian:
                return 0;
            case SubClass.Mage:
                return 1; //아직은 관련 변수가 없어서 상수 반환
        }
        return 0;
    }
    public static float ProjectileRangeResult(HunterStat _userstat) //서브클래스에 따른 기본 투사체 거리
    {
        switch (_userstat.SubClass)
        {
            case SubClass.Archer:
                //화살 사정거리 공식
                return _userstat.AttackRange * 1.3f;
            case SubClass.Guardian:
                return 0;
            case SubClass.Mage:
                return 5; //아직은 관련 변수가 없어서 상수 반환
        }
        return 0;
    }
    #endregion

    #region 헌터가 적에게 주는 데미지 처리

    /// <summary>
    /// 스킬 데미지 처리 (함수 실행 후 damageInfo로 계산)
    /// </summary>
    public static void CalculateSkillDamage(HunterStat hunter, EnemyStat enemyStat, DamageInfo damageInfo, BaseSkillData skillData)
    {
        float PhycisDamage = 0;
        float MagicDamage = 0;
        float TrueDamage = 0;

        for (int i = 0; i < skillData.optionliast.Count; i++)
        {
            //최댓값 적용된 스탯
            float stat = HunterStat.GetMaximumStat(HunterStat.GetUserStatPerOption(hunter, skillData.optionliast[i],true), skillData.optionliast[i].ToString());
            //스탯 적용 계수 적용
            stat = stat * (skillData.DMGValue[i] * 0.01f);

            //어느 데미지 타입으로 적용되는지 확인 후 더함
            switch (skillData.attackdamagetype[i])
            {
                case AttackDamageType.Physical:
                    PhycisDamage += stat;
                    break;
                case AttackDamageType.Magic:
                    MagicDamage += stat;
                    break;
                case AttackDamageType.PhysicalTrueDamage:
                case AttackDamageType.MagicTrueDamage:
                    TrueDamage += stat;
                    break;
            }
        }

        //계산한 값 넣어주기
        damageInfo.PhycisDamageType = PhycisDamage;
        damageInfo.MagaicDamageType = MagicDamage;
        damageInfo.TrueDamageTpye = TrueDamage;
        damageInfo.skill = skillData;

        switch (skillData.attackdamagetype[0]) //데미지 타입에 따라 계산식 계산
        {
            case AttackDamageType.Physical:
            case AttackDamageType.PhysicalTrueDamage:
                damageInfo.isMagic = false;
                (damageInfo.ReflectRange, damageInfo.isInstanceKillBoss, damageInfo.damage, damageInfo.addDamage, damageInfo.CoupDamage, damageInfo.CoupAddDamage, damageInfo.isCoup, damageInfo.isCri, damageInfo.isdodge, damageInfo.isStun, damageInfo.isFreezing, damageInfo.isPosion, damageInfo.isSlow, damageInfo.isElectric, damageInfo.isChain) =
                DamageToPhysical(hunter, enemyStat, EnemyClass: enemyStat.Class, isNormalAttack:damageInfo.isNormalAttack , skill: damageInfo.skill, PycisDamage: damageInfo.PhycisDamageType  , MagicDamage: damageInfo.MagaicDamageType , TrueDamage: damageInfo.TrueDamageTpye);
                break;
            case AttackDamageType.Magic:
            case AttackDamageType.MagicTrueDamage:
                damageInfo.isMagic = true;
                (damageInfo.ReflectRange, damageInfo.isInstanceKillBoss, damageInfo.damage, damageInfo.addDamage, damageInfo.CoupDamage, damageInfo.CoupAddDamage, damageInfo.isCoup, damageInfo.isCri, damageInfo.isdodge, damageInfo.isStun, damageInfo.isFreezing, damageInfo.isPosion, damageInfo.isSlow, damageInfo.isElectric, damageInfo.isChain) =
                DamageToMagic(hunter, enemyStat, EnemyClass: enemyStat.Class, isNormalAttack: damageInfo.isNormalAttack, skill: damageInfo.skill, PycisDamage: damageInfo.PhycisDamageType, MagicDamage: damageInfo.MagaicDamageType, TrueDamage: damageInfo.TrueDamageTpye);
                break;
        }
        
    }


    /// <param name="_usersta"></param>
    /// <param name="enemystat"></param>
    
    
    /// <summary>
    /// 물리데미지 타입시 처리
    /// </summary>
    public static (bool ReflectRange, bool _isInstanceKill, float damage, float additionalDamage, float coupDamage, float coupAdditionalDamage, bool IsCoup, bool IsCritical, bool isDotge, StunStatusEffect isStun, FrozenStatusEffect isFrozen, PosionStatusEffect isPosion, SlowStatusEffect isSlow, ElectricEffect electricEffect, ChainLightningEffect changeEffects)
    DamageToPhysical(HunterStat _userstat, EnemyStat enemystat, Utill_Enum.EnemyClass EnemyClass = Utill_Enum.EnemyClass.Normal, bool isNormalAttack = false, BaseSkillData skill = null , float PycisDamage = 0 , float MagicDamage = 0 , float TrueDamage = 0)
    {
        //즉사확률체크...즉사체크해서 즉사면 그냥 바로 죽이기...
        bool isInstanceKill = false;
        float instnancekillvalue = HunterStat.Get_InstantKillBossOnBasicAttack(_userstat);
        isInstanceKill = Utill_Math.Attempt(instnancekillvalue) && enemystat.Class == EnemyClass.Boss;

        //반사 확률체크이후 반사면 댐지주기
        bool isReflectRange = false;
        float isReflectRangevalue = HunterStat.Get_ReflectRange(_userstat);
        isReflectRange = Utill_Math.Attempt(isReflectRangevalue);

        //피격시 스턴 및 CC  를 먼저 검사
        StunStatusEffect stuneffect = null;
        FrozenStatusEffect frozenstatuseffect = null;
        PosionStatusEffect posionstatuseffect = null;
        SlowStatusEffect slowstatuseffect = null;
        ElectricEffect electricEffect = null;
        ChainLightningEffect chainLightningEffect = null;

        //1.CC 검사
        bool isStun = false;
        float Stunvalue = HunterStat.Get_StunPersent(_userstat);
        float Stunduartion = HunterStat.Get_StunDuration(_userstat);
        isStun = Utill_Math.Attempt(Stunvalue);

        bool isFreezon = false;
        float Freezonvalue = HunterStat.Get_FrozenPersent(_userstat);
        float Freezonduartion = HunterStat.Get_FrozenPersent(_userstat);
        isFreezon = Utill_Math.Attempt(Freezonvalue);

        bool isPosion = false;
        float Posionvalue = HunterStat.Get_PosionDamage(_userstat);
        float Posionduartion = HunterStat.Get_PosionDuration(_userstat);
        float Posiondamage = HunterStat.Get_PosionDamage(_userstat);
        isPosion = Utill_Math.Attempt(Posionvalue);

        bool isSlow = false;
        float Slowvalue = HunterStat.Get_SlowPersent(_userstat);
        float SlowDuration = HunterStat.Get_SlowDuration(_userstat);
        float movespeedslowtime = HunterStat.Get_movespeedslowtime(_userstat);
        float attacksppedslowtime = HunterStat.Get_attackspeedslowtime(_userstat);
        isSlow = Utill_Math.Attempt(Slowvalue);

        bool isElectric = false;
        float Electricvalue = HunterStat.Get_ElectricPersent(_userstat);

        isElectric = Utill_Math.Attempt(Electricvalue);

        bool isChainLightning = false;
        float ChainLightningValue = HunterStat.Get_ChainLightningPersent(_userstat);
        isChainLightning = Utill_Math.Attempt(ChainLightningValue);




        if (isStun && isNormalAttack)
        {
            stuneffect = new StunStatusEffect(Utill_Enum.Debuff_Type.Stun, Stunduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isFreezon && isNormalAttack)
        {
            frozenstatuseffect = new FrozenStatusEffect(Utill_Enum.Debuff_Type.Freezing, Freezonduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isPosion && isNormalAttack)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            posionstatuseffect = new PosionStatusEffect(Utill_Enum.Debuff_Type.Posion, Posionduartion, Posiondamage, hunter);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isSlow && isNormalAttack)
        {
            slowstatuseffect = new SlowStatusEffect(Utill_Enum.Debuff_Type.Slow, SlowDuration, movespeedslowtime, attacksppedslowtime);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isElectric && isNormalAttack)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            ISkill tmpSkill = null;
            tmpSkill = SkillManager.Instance.electric;
            electricEffect = new ElectricEffect(Utill_Enum.Debuff_Type.electricEffect, tmpSkill, hunter);

        }
        if (isChainLightning && isNormalAttack)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            ISkill tmpSkill = null;
            tmpSkill = SkillManager.Instance.chainLightning;
            ChainLightning chainLightning = SkillManager.Instance.chainLightning;
            chainLightningEffect = new ChainLightningEffect(Utill_Enum.Debuff_Type.ChainLightning, tmpSkill, hunter, chainLightning);
        }






        //대미지 검사
        float totalDamage = 0;
        float additionalDamage = 0;
        float totalnocridamage = 0;

        //일격율
        float coupChance = 0;
        float coupDamage = 0;
        float coupAdditionalDamage = 0;
        bool _isCoup = false; //계산중 사용값
        bool IsCoup = false;  //최종 반환값

        //치명율
        float critChance = 0;
        bool isCrit = false; //계산중 사용값
        bool _isCri = false; //최종 반환값
        //회피율
        float EnemyDotge = 0;
        bool isdotge = false;

        // 1차 연산: 유저 데미지와 적의 방어력을 이용한 기본 대미지 계산
        float userDamage = PhysicalPowerResult(_userstat);
        if (skill != null && TrueDamage == 0) // 만약 스킬을 사용해 데미지가 들어온 것이고 관통 데미지 타입이 아니라면
        {
            userDamage = PycisDamage;
        }

        float enemyDefense = enemystat.PhysicalPowerDefense;
        float tempDamage = userDamage - enemyDefense;

        // 2차 연산: 물리 피해 감소 및 무시를 고려한 추가 대미지 계산
        float physicalDamageReduction = enemystat.PhysicalDamageReduction * 0.01f; //상대방의 물리피해감소
        float enemyIgnore = 0.00f; // 나의 물리피해감소 무시
        float reductionAmount = tempDamage * (physicalDamageReduction - enemyIgnore);
        if (reductionAmount < 0)
        {
            reductionAmount = 0;
        }
        tempDamage -= reductionAmount;

        // 3차 연산: 물리 관통 대미지 추가
        if (skill == null)
        {
            tempDamage += PhysicalTrueDamageResult(_userstat);
        }
        else
        {
            tempDamage += TrueDamage;
        }
         
        totalnocridamage = tempDamage;

        // 4차 연산: 물리 피해 증가 및 보스 추가 데미지

        float enemyTypeAddDamage = 0f;
        //보스피해 추가연산
        if (isNormalAttack) //평타라면
        {
            switch (EnemyClass)
            {
                case Utill_Enum.EnemyClass.None:
                    break;
                case Utill_Enum.EnemyClass.Normal:
                    enemyTypeAddDamage = _userstat.AddDamageToNormalMob;
                    break;
                case Utill_Enum.EnemyClass.Boss:
                    enemyTypeAddDamage = _userstat.AddDamageToBossMob;
                    break;
                case Utill_Enum.EnemyClass.EBoss:
                    enemyTypeAddDamage = 0f;
                    break;
                default:
                    break;
            }
        }
        float skillAddDamage = 0f;
        if (skill != null)// 만약 스킬을 사용해 데미지가 들어온 것이라면
        {
            if (skill.SkillName == Tag.ArrowRain)
            {
                skillAddDamage += _userstat.AddDamageArrowRain;
            }
            else if (skill.SkillName == Tag.ChainLightning)
            {
                skillAddDamage += _userstat.AddDamageChainLightning;
            }
            else if (skill.SkillName == Tag.HammerSummon)
            {
                skillAddDamage += _userstat.AddDamageHammerSummon;
            }
            else if (skill.SkillName == Tag.MagneticWaves)
            {
                skillAddDamage += _userstat.AddDamageMagneticWaves;
            }
            else if (skill.SkillName == Tag.StrongShot)
            {
                skillAddDamage += _userstat.AddDamageStrongShot;
            }
            else if (skill.SkillName == Tag.RapidShot)
            {
                skillAddDamage += _userstat.AddDamageRapidShot;
            }
            else if (skill.SkillName == Tag.DemonicBlow)
            {
                skillAddDamage += _userstat.AddDamageDemonicBlow;
            }
            else if (skill.SkillName == Tag.SpikeTrap)
            {
                skillAddDamage += _userstat.AddDamageSpikeTrap;
            }
            else if (skill.SkillName == Tag.VenomousSting)
            {
                skillAddDamage += _userstat.AddDamageVenomousSting;
            }
            else if (skill.SkillName == Tag.Electric)
            {
                skillAddDamage += _userstat.AddDamageElectric;
            }
            else if (skill.SkillName == Tag.WhirlwindRush)
            {
                skillAddDamage += _userstat.AddDamageWhirlwindRush;
            }
            else if (skill.SkillName == Tag.DokkaebiFire)
            {
                skillAddDamage += _userstat.AddDamageDokkaebiFire;
            }
            else if (skill.SkillName == Tag.Meteor)
            {
                skillAddDamage += _userstat.AddDamageMeteor;
            }
            else if (skill.SkillName == Tag.Electricshock)
            {
                skillAddDamage += _userstat.AddDamageElectricshock;
            }
        }
        //float additionalDmg = _userstat.PhysicalDamageIncrease + enemyTypeAddDamage; // + 보스 피해 증가  .. 피해 증가는 다 여기로..
        float additionalDmg = enemyTypeAddDamage + skillAddDamage; // + 보스 피해 증가  .. 피해 증가는 다 여기로..
        float additionalDmgPercent = (additionalDmg * 0.01f);
        float totalAdditionalDmg = tempDamage * additionalDmgPercent; //추가피해량 대미지 

        additionalDamage = totalAdditionalDmg;

        // 5차 연산: 치명타 발생 여부 및 치명타 대미지 계산 ** 회피율 등 추후연산 처리

        //상대방 회피율 타입에따라 계싼 
        switch (_userstat.AttacckType)
        {
            case Utill_Enum.Enum_AttackType.None:
                //치명율
                critChance = _userstat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                EnemyDotge = enemystat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(EnemyDotge);
                //일격율
                coupChance = _userstat.CoupChance * 0.01f;
                _isCoup = Utill_Math.CalculateProbability(coupChance);
                break;
            case Utill_Enum.Enum_AttackType.Melee:
                //치명율
                critChance = _userstat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                EnemyDotge = enemystat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(EnemyDotge);
                //일격율
                coupChance = _userstat.CoupChance * 0.01f;
                _isCoup = Utill_Math.CalculateProbability(coupChance);
                break;
            case Utill_Enum.Enum_AttackType.Range:
                //치명율
                critChance = _userstat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                EnemyDotge = enemystat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(EnemyDotge);
                //일격율
                coupChance = _userstat.CoupChance * 0.01f;
                _isCoup = Utill_Math.CalculateProbability(coupChance);
                break;
            case Utill_Enum.Enum_AttackType.Spell:
                break;
        }
        //회피율 먼저 계싼
        if (isdotge) //상대방이 먼저 회피를 성공했다면?
        {
            isdotge = true;
            return (isReflectRange, isInstanceKill, -1, -1, -1, -1, IsCoup, _isCri, isdotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect, electricEffect, chainLightningEffect);
        }

        //체력흡수, 마나 흡수 계산
        CheckDrain(_userstat, isNormalAttack);

        if (isNormalAttack && (isCrit || _isCoup))
        {
            if (_isCoup)
            {
                IsCoup = true;
                float curTempDamage = tempDamage;
                //일반 데미지 계산
                float coupDmg = curTempDamage * (_userstat.CoupDamage * 0.01f);
                curTempDamage += coupDmg;
                //추가 데미지 계산
                float additionalCoupDmg = additionalDamage * (_userstat.CriDamage * 0.01f);
                coupAdditionalDamage = additionalDamage + additionalCoupDmg;

                //최후 대미지 할당 
                coupDamage = curTempDamage;
                if (coupDamage <= 0)
                {
                    coupDamage = 0;
                }
            }
            if (isCrit)
            {
                _isCri = true;
                float curTempDamage = tempDamage;
                float curAdditionalDamage = additionalDamage;
                //일반 데미지 크리티컬 계산
                float critDmg = curTempDamage * (_userstat.CriDamage * 0.01f);
                curTempDamage += critDmg;
                //추가 데미지 크리티컬 계산
                float additionalCritDmg = curAdditionalDamage * (_userstat.CriDamage * 0.01f);
                additionalDamage += additionalCritDmg;

                //최후 대미지 할당 
                totalDamage = curTempDamage;
                if (totalDamage <= 0)
                {
                    totalDamage = 0;
                }
            }

            return (isReflectRange, isInstanceKill, totalDamage, additionalDamage, coupDamage, coupAdditionalDamage, IsCoup, _isCri, isdotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect, electricEffect, chainLightningEffect);

        }
        else
        {
            //최후 대미지 할당 
            _isCri = false;
            if (totalnocridamage <= 0)
            {
                totalnocridamage = 0;
            }

            //if (isReflectRange && enemystat.AttackType ==  Enum_AttackType.Melee)
            //{
            //    //적에게 대미지 주기
            //    enemystat.CurrentHp -= totalnocridamage;

            //    //유저에게 들어가는대미지는 0 
            //    totalnocridamage = 0;
            //}

            return (isReflectRange, isInstanceKill, totalnocridamage, additionalDamage, -1, -1, IsCoup, _isCri, isdotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect, electricEffect, chainLightningEffect);
        }
    }


    /// <summary>
    /// 마법데미지 타입시 처리
    /// </summary>
    public static (bool isReflect, bool _isInstanceKill, float damage, float additionalDamage, float coupDamage, float coupAdditionalDamage, bool IsCoup, bool iscri, bool isDotge, StunStatusEffect isStun, FrozenStatusEffect isFrozen, PosionStatusEffect isPosion, SlowStatusEffect isSlow, ElectricEffect electricEffect, ChainLightningEffect chainLightning)
        DamageToMagic(HunterStat _userstat, EnemyStat enemystat, Utill_Enum.EnemyClass EnemyClass = Utill_Enum.EnemyClass.Normal, bool isNormalAttack = false, BaseSkillData skill = null , float PycisDamage = 0, float MagicDamage = 0, float TrueDamage = 0)
    {

        //즉사확률체크...즉사체크해서 즉사면 그냥 바로 죽이기...
        bool isInstanceKill = false;
        float instnancekillvalue = HunterStat.Get_InstantKillBossOnBasicAttack(_userstat);
        isInstanceKill = Utill_Math.Attempt(instnancekillvalue);

        //반사확률체크...즉사체크해서 즉사면 그냥 바로 죽이기...
        bool isReflectRange = false;
        float iReflectRangevalue = HunterStat.Get_ReflectRange(_userstat);
        isReflectRange = Utill_Math.Attempt(iReflectRangevalue);

        float totalDamage = 0;
        float additionalDamage = 0;
        float totalnocridamage = 0;
        bool isCri = false;
        bool isDotge = false;

        //피격시 스턴 및 CC  를 먼저 검사
        StunStatusEffect stuneffect = null;
        FrozenStatusEffect frozenstatuseffect = null;
        PosionStatusEffect posionstatuseffect = null;
        SlowStatusEffect slowstatuseffect = null;
        ElectricEffect electricEffect = null;
        ChainLightningEffect chainLightningEffect = null;

        //1.CC 검사
        bool isStun = false;
        float Stunvalue = HunterStat.Get_StunPersent(_userstat);
        float Stunduartion = HunterStat.Get_StunDuration(_userstat);
        isStun = Utill_Math.Attempt(Stunvalue);

        bool isFreezon = false;
        float Freezonvalue = HunterStat.Get_FrozenPersent(_userstat);
        float Freezonduartion = HunterStat.Get_StunDuration(_userstat);
        isFreezon = Utill_Math.Attempt(Freezonvalue);

        bool isPosion = false;
        float Posionvalue = HunterStat.Get_PosionDamage(_userstat);
        float Posionduartion = HunterStat.Get_PosionDuration(_userstat);
        float Posiondamage = HunterStat.Get_PosionDamage(_userstat);
        isPosion = Utill_Math.Attempt(Posionvalue);

        bool isSlow = false;
        float Slowvalue = HunterStat.Get_SlowPersent(_userstat);
        float SlowDuration = HunterStat.Get_SlowDuration(_userstat);
        float movespeedslowtime = HunterStat.Get_movespeedslowtime(_userstat);
        float attacksppedslowtime = HunterStat.Get_attackspeedslowtime(_userstat);
        isSlow = Utill_Math.Attempt(Slowvalue);

        bool isElectric = false;
        float Electricvalue = HunterStat.Get_ElectricPersent(_userstat);
        isElectric = Utill_Math.Attempt(Electricvalue);


        bool isChainLightning = false;
        float ChainLightningValue = HunterStat.Get_ChainLightningPersent(_userstat);
        isChainLightning = Utill_Math.Attempt(ChainLightningValue);

        if (isStun && isNormalAttack)
        {
            stuneffect = new StunStatusEffect(Utill_Enum.Debuff_Type.Stun, Stunduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isFreezon && isNormalAttack)
        {
            frozenstatuseffect = new FrozenStatusEffect(Utill_Enum.Debuff_Type.Freezing, Freezonduartion);
        }
        if (isPosion && isNormalAttack)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            posionstatuseffect = new PosionStatusEffect(Utill_Enum.Debuff_Type.Posion, Posionduartion, Posiondamage, hunter);
        }
        if (isSlow && isNormalAttack)
        {
            slowstatuseffect = new SlowStatusEffect(Utill_Enum.Debuff_Type.Slow, SlowDuration, movespeedslowtime, attacksppedslowtime);

        }
        if (isElectric && isNormalAttack)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            ISkill tmpSkill = null;
            tmpSkill = SkillManager.Instance.electric;
            electricEffect = new ElectricEffect(Utill_Enum.Debuff_Type.electricEffect, tmpSkill, hunter);
        }
        if (isChainLightning && isNormalAttack)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            ISkill tmpSkill = null;
            tmpSkill = SkillManager.Instance.chainLightning;
            ChainLightning chainLightning = SkillManager.Instance.chainLightning;
            chainLightningEffect = new ChainLightningEffect(Utill_Enum.Debuff_Type.ChainLightning, tmpSkill, hunter, chainLightning);
        }

















        // 1차 연산: 유저 데미지와 적의 방어력을 이용한 기본 대미지 계산
        float userDamage = MagicPowerResult(_userstat);

        if (skill != null && TrueDamage == 0) // 만약 스킬을 사용해 데미지가 들어온 것이고 관통 데미지 타입이 아니라면
        {
            userDamage = MagicDamage;
        }

        float enemyDefense = enemystat.MagicPowerDefense;
        float tempDamage = userDamage - enemyDefense;

        // 2차 연산: 물리 피해 감소 및 무시를 고려한 추가 대미지 계산
        float physicalDamageReduction = enemystat.MagicDamageReduction * 0.01f; //상대물리피해감소율
        float enemyIgnore = 0.00f; // 나의 물리피해감소무시
        float reductionAmount = tempDamage * (physicalDamageReduction - enemyIgnore);
        if (reductionAmount < 0)
        {
            reductionAmount = 0;
        }
        tempDamage -= reductionAmount;

        // 3차 연산: 물리 관통 대미지 추가
        // 3차 연산: 물리 관통 대미지 추가
        if (skill == null)
        {
            tempDamage += MagicTrueDamageResult(_userstat);
        }
        else
        {
            tempDamage += TrueDamage;
        }
        totalnocridamage = tempDamage;

        // 4차 연산: 물리 피해 증가 및 보스 추가 데미지

        float enemyTypeAddDamage = 0f;
        if (isNormalAttack)
        {
            //보스피해 추가연산
            switch (EnemyClass)
            {
                case Utill_Enum.EnemyClass.None:
                    break;
                case Utill_Enum.EnemyClass.Normal:
                    enemyTypeAddDamage = _userstat.AddDamageToNormalMob;
                    break;
                case Utill_Enum.EnemyClass.Boss:
                    enemyTypeAddDamage = _userstat.AddDamageToBossMob;
                    break;
                case Utill_Enum.EnemyClass.EBoss:
                    enemyTypeAddDamage = 0f;
                    break;
                default:
                    break;
            }
        }
        float skillAddDamage = 0f;
        if (skill != null)// 만약 스킬을 사용해 데미지가 들어온 것이라면
        {
            if (skill.SkillName == Tag.ArrowRain)
            {
                skillAddDamage += _userstat.AddDamageArrowRain;
            }
            else if (skill.SkillName == Tag.ChainLightning)
            {
                skillAddDamage += _userstat.AddDamageChainLightning;
            }
            else if (skill.SkillName == Tag.HammerSummon)
            {
                skillAddDamage += _userstat.AddDamageHammerSummon;
            }
            else if (skill.SkillName == Tag.MagneticWaves)
            {
                skillAddDamage += _userstat.AddDamageMagneticWaves;
            }
            else if (skill.SkillName == Tag.StrongShot)
            {
                skillAddDamage += _userstat.AddDamageStrongShot;
            }
            else if (skill.SkillName == Tag.RapidShot)
            {
                skillAddDamage += _userstat.AddDamageRapidShot;
            }
            else if (skill.SkillName == Tag.DemonicBlow)
            {
                skillAddDamage += _userstat.AddDamageDemonicBlow;
            }
            else if (skill.SkillName == Tag.SpikeTrap)
            {
                skillAddDamage += _userstat.AddDamageSpikeTrap;
            }
            else if (skill.SkillName == Tag.VenomousSting)
            {
                skillAddDamage += _userstat.AddDamageVenomousSting;
            }
            else if (skill.SkillName == Tag.Electric)
            {
                skillAddDamage += _userstat.AddDamageElectric;
            }
            else if (skill.SkillName == Tag.WhirlwindRush)
            {
                skillAddDamage += _userstat.AddDamageWhirlwindRush;
            }
            else if (skill.SkillName == Tag.DokkaebiFire)
            {
                skillAddDamage += _userstat.AddDamageDokkaebiFire;
            }
            else if (skill.SkillName == Tag.Meteor)
            {
                skillAddDamage += _userstat.AddDamageMeteor;
            }
            else if (skill.SkillName == Tag.Electricshock)
            {
                skillAddDamage += _userstat.AddDamageElectricshock;
            }
        }
        //float additionalDmg = _userstat.PhysicalDamageIncrease + enemyTypeAddDamage; // + 보스 피해 증가  .. 피해 증가는 다 여기로..
        float additionalDmg = enemyTypeAddDamage + skillAddDamage; // + 보스 피해 증가 + 스킬 피해 증가  .. 피해 증가는 다 여기로..
        float additionalDmgPercent = additionalDmg * 0.01f;
        float totalAdditionalDmg = tempDamage * additionalDmgPercent; //추가피해량 대미지 

        additionalDamage = additionalDmgPercent;

        // 5차 연산: 치명타 발생 여부 및 치명타 대미지 계산 ** 회피율 등 추후연산 처리

        //일격율
        float coupChance = 0;
        bool _isCoup = false; //계산중 사용값
        //float coupDamage = 0;
        //float coupAdditionalDamage = 0;
        bool IsCoup = false;  //최종 반환값

        //치명율
        float critChance = 0;
        bool isCrit = false;
        //회피율
        float EnemyDotge = 0;
        bool isdotge = false;
        //상대방 회피율 타입에따라 계산
        switch (_userstat.AttacckType)
        {
            case Utill_Enum.Enum_AttackType.Melee:
                //치명율
                critChance = _userstat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                EnemyDotge = enemystat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(EnemyDotge);
                //일격율
                coupChance = _userstat.CoupChance * 0.01f;
                _isCoup = Utill_Math.CalculateProbability(coupChance);
                break;
            case Utill_Enum.Enum_AttackType.Range:
                //치명율
                critChance = _userstat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                EnemyDotge = enemystat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(EnemyDotge);
                //일격율
                coupChance = _userstat.CoupChance * 0.01f;
                _isCoup = Utill_Math.CalculateProbability(coupChance);
                break;
            case Utill_Enum.Enum_AttackType.Spell:
                //치명율
                critChance = _userstat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                EnemyDotge = enemystat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(EnemyDotge);
                //일격율
                coupChance = _userstat.CoupChance * 0.01f;
                _isCoup = Utill_Math.CalculateProbability(coupChance);
                break;
        }

        //회피율 먼저 계싼
        if (isdotge) //상대방이 먼저 회피를 성공했다면?
        {
            isDotge = true;
            Debbuger.Debug("=================Dotge================= ");
            return (isReflectRange, isInstanceKill, -1, -1, -1, -1, IsCoup, isCri, isDotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect, electricEffect, chainLightningEffect);
        }
        //체력흡수, 마나 흡수 계산
        CheckDrain(_userstat, isNormalAttack);

        if (isReflectRange && enemystat.AttackType == Enum_AttackType.Melee)
        {
            //적에게 대미지 주기
            enemystat.CurrentHp -= totalnocridamage;

            //유저에게 들어가는대미지는 0 
            totalnocridamage = 0;
        }

        //최후 대미지 할당 
        return (isReflectRange, isInstanceKill, totalnocridamage, totalAdditionalDmg, -1, -1, IsCoup, isCri, isDotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect, electricEffect, chainLightningEffect);

        //if (isCrit) 
        //{
        //    float critDmg = tempDamage * (_userstat.CriDamage * 0.01f);
        //    tempDamage += critDmg;

        //    //최후 대미지 할당 
        //    additionalDamage = 0;
        //    totalDamage = tempDamage;
        //    Debbuger.Debug("크리 대미지 " + totalDamage);
        //    return (totalDamage, additionalDamage);

        //}
        //else
        //{

        //}
    }

    #endregion

    #region 체력/마나 흡수
    /// <summary>
    /// 현재 스탯의 체력/마나 흡수를 확인 후 관련 로직을 실행
    /// </summary>
    private static void CheckDrain(HunterStat _userstat, bool isNormalAttack)
    {
        //체력흡수, 마나 흡수 계산
        float hpDrainChance = StatManager.Instance.HpDrainChance * 0.01f;
        float mpDrainChance = StatManager.Instance.MpDrainChance * 0.01f;

        bool isHpDrain = GetMaximumStat(_userstat.HPDrain, Tag.HPDrain) != 0 && isNormalAttack && Utill_Math.CalculateProbability(hpDrainChance);
        bool isMpDrain = GetMaximumStat(_userstat.MPDrain, Tag.MPDrain) != 0 && isNormalAttack && Utill_Math.CalculateProbability(mpDrainChance);

        if (isHpDrain || isMpDrain)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);

            int hpDrain = (int)Mathf.Min(GetMaximumStat(_userstat.HPDrain, Tag.HPDrain), StatManager.Instance.HpDrainMaximum);
            int mpDrain = (int)Mathf.Min(GetMaximumStat(_userstat.MPDrain, Tag.MPDrain), StatManager.Instance.MpDrainMaximum);

            HunterStat.SetPlusHp(hunter.Orginstat, hpDrain);
            HunterStat.SetPlusMp(hunter.Orginstat, mpDrain);
            hunter.Drain(hpDrain, mpDrain);
        }
    }
    #endregion


    /// <summary>
    /// HP..............
    /// </summary>
    /// <param name="_userstat"></param>
    #region HP 관련 함수
    public static void SetMaxHp(HunterStat _userstat)
    {
        _userstat.CurrentHp = _userstat.HP;
    }



    public static void SetHp(HunterStat _userstat, float value)
    {
        if ( value > _userstat.HP)
            _userstat.CurrentHp = _userstat.HP;
        else
            _userstat.CurrentHp = value;
    }

    public static void SetminusgeHp(HunterStat _userstat, float value)
    {
        _userstat.CurrentHp -= value;
    }

    public static void SetPlusHp(HunterStat _userstat, float value)
    {
        if (_userstat.CurrentHp + value > _userstat.HP)
            _userstat.CurrentHp = _userstat.HP;
        else
            _userstat.CurrentHp += value;
    }

    public static void SetZeroHp(HunterStat _userstat)
    {
        _userstat.CurrentHp = 0f;
    }

    public static float GetCurrentHp(HunterStat _userstat)
    {
        float temp_hp = 0;

        temp_hp = _userstat.CurrentHp;

        return temp_hp;
    }
    #endregion

    /// <summary>
    /// Mp....................관련...
    /// </summary>
    /// <param name="_userstat"></param>
    #region MP관련함수
    public static void SetMaxMp(HunterStat _userstat)
    {
        _userstat.CurrentMp = _userstat.MP;
    }

    public static void SetMp(HunterStat _userstat, float value)
    {
        if (value > _userstat.MP)
            _userstat.CurrentMp = _userstat.MP;
        else
            _userstat.CurrentMp = value;
    }

    public static float MinusMp(HunterStat _userstat, float value)
    {
        float temp = 0;
        temp = _userstat.CurrentMp - value;
        
        if (temp < 0)
        {
            return 0;
        }

        _userstat.CurrentMp -= value;
        if (_userstat.CurrentMp < 0)
            _userstat.CurrentMp = 0;
        return temp;
    }

    public static void SetPlusMp(HunterStat _userstat, float value)
    {
        if (_userstat.CurrentMp + value > _userstat.MP)
            _userstat.CurrentMp = _userstat.MP;
        else
            _userstat.CurrentMp += value;
    }
    public static void SetZeroMp(HunterStat _userstat)
    {
        _userstat.CurrentMp = 0f;
    }

    public static float GetCurrentMp(HunterStat _userstat)
    {
        float temp_mp = 0;

        temp_mp = _userstat.CurrentMp;

        return temp_mp;
    }
    #endregion


    #region 크리티컬 확률
    public static float GetCriChance(HunterStat stat)
    {
        float temp_result = 0;
        temp_result = stat.CriChance;
        return temp_result;
    }

    public static void Plus_CriChance(HunterStat stat , float value)
    {
        stat.CriChance += value;
    }

    public static void Minuse_CriChance(HunterStat stat, float value)
    {
        stat.CriChance -= value;
    }
    #endregion

    #region 회피력
    public static float GetDotgeChance(HunterStat stat)
    {
        float temp_result = 0;
        temp_result = stat.DodgeChance;
        return temp_result;
    }

    public static void Plus_DotgeChance(HunterStat stat, float value)
    {
        stat.DodgeChance += value;
    }

    public static void Minuse_DotgeChance(HunterStat stat, float value)
    {
        stat.DodgeChance -= value;
    }
    #endregion

    public  float GetUserExpBuffValue(HunterStat _userstat)
    {
        float temp_value = 0;
        temp_value = _userstat.ExpBuff * 0.01f;
        return temp_value;
    }

    public static float GetUserGoldBuffValue(HunterStat _userstat)
    {
        float temp_value = 0;
        temp_value = _userstat.GoldBuff * 0.01f;
        return temp_value;
    }


    //////
    /// STAT
    ////
    ///
    #region 유저 장비 장착 관련(HuntertImeData)
    public static void AddChangOptionToStat(HunterStat userstat, HunteritemData item)
    {
        for (int i = 0; i < item.FixedOptionTypes.Count; i++)
        {
            int index = i;
            Utill_Enum.Option tpye = CSVReader.ParseEnum<Utill_Enum.Option>(item.FixedOptionTypes[i]);
            switch (tpye)
            {
                case Utill_Enum.Option.None:
                    break;
                case Utill_Enum.Option.PhysicalPowerPercent:
                    float valuePhysicalPowerPercent = (float)item.FixedOptionValues[index];
                    userstat.PhysicalPowerPercent += valuePhysicalPowerPercent;
                    break;
                case Utill_Enum.Option.MagicPowerPercent:
                    float valueMagicPowerPercent = (float)item.FixedOptionValues[index];
                    userstat.MagicPowerPercent += valueMagicPowerPercent;
                    break;
                case Utill_Enum.Option.PhysicalPower:
                    float value0 = (float)item.FixedOptionValues[index];
                    userstat.PhysicalPower += value0;
                    break;
                case Utill_Enum.Option.AttackRange:
                    float value1 = (float)item.FixedOptionValues[index];
                    userstat.AttackRange += value1;
                    break;
                case Utill_Enum.Option.AttackSpeed:
                    float value2 = (float)item.FixedOptionValues[index];
                    userstat.AttackSpeed += value2;
                    break;
                case Utill_Enum.Option.MagicPower:
                    float value3 = (float)item.FixedOptionValues[index];
                    userstat.MagicPower += value3;
                    break;
                case Utill_Enum.Option.CriDamage:
                    float value4 = (float)item.FixedOptionValues[index];
                    userstat.CriDamage += value4;
                    break;
                case Utill_Enum.Option.HP:
                    float value5 = (float)item.FixedOptionValues[index];
                    userstat.HP += value5;
                    break;
                case Utill_Enum.Option.MP:
                    float value6 = (float)item.FixedOptionValues[index];
                    userstat.MP += value6;
                    break;
                case Utill_Enum.Option.CriChance:
                    float value7 = (float)item.FixedOptionValues[index];
                    userstat.CriChance += value7;
                    break;
                case Utill_Enum.Option.ArrowCount:
                    int value8 = (int)item.FixedOptionValues[index];
                    userstat.ArrowCount += value8;
                    break;
                case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                    float value10 = (float)item.FixedOptionValues[index];
                    userstat.InstantKillBossOnBasicAttack += value10;
                    break;
                case Utill_Enum.Option.ReflectRange:
                    float value11 = (float)item.FixedOptionValues[index];
                    userstat.ReflectRange += value11;
                    break;
                case Utill_Enum.Option.AddDamageArrowRain:
                    float value12 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageArrowRain += value12;
                    break;
                case Utill_Enum.Option.ElectricshockNumber:
                    float value13 = (float)item.FixedOptionValues[index];
                    userstat.ElectricshockNumber += value13;
                    break;
                case Utill_Enum.Option.AddDamageChainLightning:
                    float value14 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageChainLightning += value14;
                    break;
                case Utill_Enum.Option.AddDamageHammerSummon:
                    float value15 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageHammerSummon += value15;
                    break;
                case Utill_Enum.Option.AddDamageMagneticWaves:
                    float value16 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageMagneticWaves += value16;
                    break;
                case Utill_Enum.Option.AddDamageStrongShot:
                    float value17 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageStrongShot += value17;
                    break;
                case Utill_Enum.Option.AddDamageRapidShot:
                    float value18 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageRapidShot += value18;
                    break;
                case Utill_Enum.Option.AddDamageDemonicBlow:
                    float value19 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageDemonicBlow += value19;
                    break;
                case Utill_Enum.Option.AddDamageSpikeTrap:
                    float value20 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageSpikeTrap += value20;
                    break;
                case Utill_Enum.Option.AddDamageVenomousSting:
                    float value21 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageVenomousSting += value21;
                    break;
                case Utill_Enum.Option.AddDamageElectric:
                    float value22 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageElectric += value22;
                    break;
                case Utill_Enum.Option.AddDamageWhirlwindRush:
                    float value23 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageWhirlwindRush += value23;
                    break;
                case Utill_Enum.Option.AddDamageDokkaebiFire:
                    float value24 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageDokkaebiFire += value24;
                    break;

                case Utill_Enum.Option.AddDamageMeteor:
                    float value25 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageMeteor += value25;
                    break;
                case Utill_Enum.Option.AddDamageElectricshock:
                    float value26 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageElectricshock += value26;
                    break;
                case Utill_Enum.Option.BeastTransformationStackCountReduce:
                    int value27 = (int)item.FixedOptionValues[index];
                    userstat.BeastTransformationStackCountReduce += value27;
                    break;
                case Utill_Enum.Option.ContinuousHitStackCountReduce:
                    int value28 = (int)item.FixedOptionValues[index];
                    userstat.ContinuousHitStackCountReduce += value28;
                    break;
                case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                    float value29 = (float)item.FixedOptionValues[index];
                    userstat.DokkaebiFireRotationSpeed += value29;
                    break;
                case Option.ElectricCastingChance:
                    int value30 = (int)item.FixedOptionValues[index];
                    userstat.ElectricCastingChance += value30;
                    break;
                case Option.SplitPiercingNumber:
                    int value31 = (int)item.FixedOptionValues[index];
                    userstat.SplitPiercingNumber += value31;
                    break;
                case Option.SpikeTrapNumber:
                    int value32 = (int)item.FixedOptionValues[index];
                    userstat.SpikeTrapNumber += value32;
                    break;
                case Option.SpikeTrapDamageRadius:
                    float value33 = (float)item.FixedOptionValues[index];
                    userstat.SpikeTrapDamageRadius += value33;
                    break;
                case Option.VenomousStingNumber:
                    int value34 = (int)item.FixedOptionValues[index];
                    userstat.VenomousStingNumber += value34;
                    break;
                case Option.VenomousStingDamageRadius:
                    float value35 = (float)item.FixedOptionValues[index];
                    userstat.VenomousStingDamageRadius += value35;
                    break;
                case Option.MagneticWavesRangeNumber:
                    float value36 = (float)item.FixedOptionValues[index];
                    userstat.MagneticWavesRangeNumber += value36;
                    break;
                case Option.LastLeafAblityNumber:
                    float value37 = (float)item.FixedOptionValues[index];
                    userstat.LastLeafAblityNumber += value37;
                    break;
                case Option.ChainLightningNumber:
                    float value38 = (float)item.FixedOptionValues[index];
                    userstat.VenomousStingDamageRadius += value38;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// (가챠)현재 장착한 장비 타입의 모든 보유효과를 더해줌
    /// </summary>
    public static void AddAllHoldOptionToStat(HunterStat userstat, HunteritemData item)
    {
        if (item.EquipCountList != null) //보유효과가 존재한다면
        {
            for (int i = 0; i < item.EquipCountList.Count; i++)
            {
                if (item.EquipCountList[i]>0) //현재 소유 갯수가 0 이상이면
                {
                    //아이템 정보를 찾기 위해 이름을 구함
                    string curName = userstat.SubClass.ToString() + ((Grade)i+1).ToString() + item.Part.ToString();
                    Item curGradeItem = GameDataTable.Instance.EquipmentList[userstat.SubClass][curName];
                    for(int j = 0; j < curGradeItem.HoldOption.Length; j++)
                    {
                        int index = j;
                        Utill_Enum.Option tpye = curGradeItem.HoldOption[j];

                        switch (tpye)
                        {
                            case Utill_Enum.Option.None:
                                break;
                            case Utill_Enum.Option.PhysicalPowerPercent:
                                float valuePhysicalPowerPercent = (float)curGradeItem.HoldOptionValue[index];
                                userstat.PhysicalPowerPercent += valuePhysicalPowerPercent;
                                break;
                            case Utill_Enum.Option.MagicPowerPercent:
                                float valueMagicPowerPercent = (float)curGradeItem.HoldOptionValue[index];
                                userstat.MagicPowerPercent += valueMagicPowerPercent;
                                break;
                            case Utill_Enum.Option.PhysicalPower:
                                float value0 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.PhysicalPower += value0;
                                break;
                            case Utill_Enum.Option.AttackRange:
                                float value1 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AttackRange += value1;
                                break;
                            case Utill_Enum.Option.AttackSpeed:
                                float value2 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AttackSpeed += value2;
                                break;
                            case Utill_Enum.Option.MagicPower:
                                float value3 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.MagicPower += value3;
                                break;
                            case Utill_Enum.Option.CriDamage:
                                float value4 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.CriDamage += value4;
                                break;
                            case Utill_Enum.Option.HP:
                                float value5 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.HP += value5;
                                break;
                            case Utill_Enum.Option.MP:
                                float value6 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.MP += value6;
                                break;
                            case Utill_Enum.Option.CriChance:
                                float value7 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.CriChance += value7;
                                break;
                            case Utill_Enum.Option.ArrowCount:
                                int value8 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.ArrowCount += value8;
                                break;
                            case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                                float value10 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.InstantKillBossOnBasicAttack += value10;
                                break;
                            case Utill_Enum.Option.ReflectRange:
                                float value11 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.ReflectRange += value11;
                                break;
                            case Utill_Enum.Option.AddDamageArrowRain:
                                float value12 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageArrowRain += value12;
                                break;
                            case Utill_Enum.Option.ElectricshockNumber:
                                float value13 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.ElectricshockNumber += value13;
                                break;
                            case Utill_Enum.Option.AddDamageChainLightning:
                                float value14 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageChainLightning += value14;
                                break;
                            case Utill_Enum.Option.AddDamageHammerSummon:
                                float value15 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageHammerSummon += value15;
                                break;
                            case Utill_Enum.Option.AddDamageMagneticWaves:
                                float value16 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageMagneticWaves += value16;
                                break;
                            case Utill_Enum.Option.AddDamageStrongShot:
                                float value17 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageStrongShot += value17;
                                break;
                            case Utill_Enum.Option.AddDamageRapidShot:
                                float value18 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageRapidShot += value18;
                                break;
                            case Utill_Enum.Option.AddDamageDemonicBlow:
                                float value19 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageDemonicBlow += value19;
                                break;
                            case Utill_Enum.Option.AddDamageSpikeTrap:
                                float value20 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageSpikeTrap += value20;
                                break;
                            case Utill_Enum.Option.AddDamageVenomousSting:
                                float value21 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageVenomousSting += value21;
                                break;
                            case Utill_Enum.Option.AddDamageElectric:
                                float value22 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageElectric += value22;
                                break;
                            case Utill_Enum.Option.AddDamageWhirlwindRush:
                                float value23 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageWhirlwindRush += value23;
                                break;
                            case Utill_Enum.Option.AddDamageDokkaebiFire:
                                float value24 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageDokkaebiFire += value24;
                                break;

                            case Utill_Enum.Option.AddDamageMeteor:
                                float value25 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageMeteor += value25;
                                break;
                            case Utill_Enum.Option.AddDamageElectricshock:
                                float value26 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.AddDamageElectricshock += value26;
                                break;
                            case Utill_Enum.Option.BeastTransformationStackCountReduce:
                                int value27 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.BeastTransformationStackCountReduce += value27;
                                break;
                            case Utill_Enum.Option.ContinuousHitStackCountReduce:
                                int value28 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.ContinuousHitStackCountReduce += value28;
                                break;
                            case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                                float value29 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.DokkaebiFireRotationSpeed += value29;
                                break;
                            case Option.ElectricCastingChance:
                                int value30 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.ElectricCastingChance += value30;
                                break;
                            case Option.SplitPiercingNumber:
                                int value31 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.SplitPiercingNumber += value31;
                                break;
                            case Option.SpikeTrapNumber:
                                int value32 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.SpikeTrapNumber += value32;
                                break;
                            case Option.SpikeTrapDamageRadius:
                                float value33 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.SpikeTrapDamageRadius += value33;
                                break;
                            case Option.VenomousStingNumber:
                                int value34 = (int)curGradeItem.HoldOptionValue[index];
                                userstat.VenomousStingNumber += value34;
                                break;
                            case Option.VenomousStingDamageRadius:
                                float value35 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.VenomousStingDamageRadius += value35;
                                break;
                            case Option.MagneticWavesRangeNumber:
                                float value36 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.MagneticWavesRangeNumber += value36;
                                break;
                            case Option.LastLeafAblityNumber:
                                float value37 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.LastLeafAblityNumber += value37;
                                break;
                            case Option.ChainLightningNumber:
                                float value38 = (float)curGradeItem.HoldOptionValue[index];
                                userstat.VenomousStingDamageRadius += value38;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// (가챠)현재 장착한 장비 타입의 특정 등급의 보유효과를 더함
    /// </summary>
    public static void AddHoldOptionToStat(HunterStat userstat, HunteritemData item, Grade grade)
    {
        if (item.EquipCountList != null)
        {
            string curName = userstat.SubClass.ToString() + grade.ToString() + item.Part.ToString();
            Item curGradeItem = GameDataTable.Instance.EquipmentList[userstat.SubClass][curName];
            for (int i = 0; i < curGradeItem.HoldOption.Length; i++)
            {
                int index = i;
                Utill_Enum.Option tpye = curGradeItem.HoldOption[i];

                switch (tpye)
                {
                    case Utill_Enum.Option.None:
                        break;
                    case Utill_Enum.Option.PhysicalPowerPercent:
                        float valuePhysicalPowerPercent = (float)curGradeItem.HoldOptionValue[index];
                        userstat.PhysicalPowerPercent += valuePhysicalPowerPercent;
                        break;
                    case Utill_Enum.Option.MagicPowerPercent:
                        float valueMagicPowerPercent = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MagicPowerPercent += valueMagicPowerPercent;
                        break;
                    case Utill_Enum.Option.PhysicalPower:
                        float value0 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.PhysicalPower += value0;
                        break;
                    case Utill_Enum.Option.AttackRange:
                        float value1 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AttackRange += value1;
                        break;
                    case Utill_Enum.Option.AttackSpeed:
                        float value2 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AttackSpeed += value2;
                        break;
                    case Utill_Enum.Option.MagicPower:
                        float value3 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MagicPower += value3;
                        break;
                    case Utill_Enum.Option.CriDamage:
                        float value4 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.CriDamage += value4;
                        break;
                    case Utill_Enum.Option.HP:
                        float value5 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.HP += value5;
                        break;
                    case Utill_Enum.Option.MP:
                        float value6 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MP += value6;
                        break;
                    case Utill_Enum.Option.CriChance:
                        float value7 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.CriChance += value7;
                        break;
                    case Utill_Enum.Option.ArrowCount:
                        int value8 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.ArrowCount += value8;
                        break;
                    case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                        float value10 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.InstantKillBossOnBasicAttack += value10;
                        break;
                    case Utill_Enum.Option.ReflectRange:
                        float value11 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.ReflectRange += value11;
                        break;
                    case Utill_Enum.Option.AddDamageArrowRain:
                        float value12 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageArrowRain += value12;
                        break;
                    case Utill_Enum.Option.ElectricshockNumber:
                        float value13 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.ElectricshockNumber += value13;
                        break;
                    case Utill_Enum.Option.AddDamageChainLightning:
                        float value14 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageChainLightning += value14;
                        break;
                    case Utill_Enum.Option.AddDamageHammerSummon:
                        float value15 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageHammerSummon += value15;
                        break;
                    case Utill_Enum.Option.AddDamageMagneticWaves:
                        float value16 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageMagneticWaves += value16;
                        break;
                    case Utill_Enum.Option.AddDamageStrongShot:
                        float value17 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageStrongShot += value17;
                        break;
                    case Utill_Enum.Option.AddDamageRapidShot:
                        float value18 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageRapidShot += value18;
                        break;
                    case Utill_Enum.Option.AddDamageDemonicBlow:
                        float value19 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageDemonicBlow += value19;
                        break;
                    case Utill_Enum.Option.AddDamageSpikeTrap:
                        float value20 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageSpikeTrap += value20;
                        break;
                    case Utill_Enum.Option.AddDamageVenomousSting:
                        float value21 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageVenomousSting += value21;
                        break;
                    case Utill_Enum.Option.AddDamageElectric:
                        float value22 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageElectric += value22;
                        break;
                    case Utill_Enum.Option.AddDamageWhirlwindRush:
                        float value23 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageWhirlwindRush += value23;
                        break;
                    case Utill_Enum.Option.AddDamageDokkaebiFire:
                        float value24 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageDokkaebiFire += value24;
                        break;

                    case Utill_Enum.Option.AddDamageMeteor:
                        float value25 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageMeteor += value25;
                        break;
                    case Utill_Enum.Option.AddDamageElectricshock:
                        float value26 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageElectricshock += value26;
                        break;
                    case Utill_Enum.Option.BeastTransformationStackCountReduce:
                        int value27 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.BeastTransformationStackCountReduce += value27;
                        break;
                    case Utill_Enum.Option.ContinuousHitStackCountReduce:
                        int value28 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.ContinuousHitStackCountReduce += value28;
                        break;
                    case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                        float value29 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.DokkaebiFireRotationSpeed += value29;
                        break;
                    case Option.ElectricCastingChance:
                        int value30 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.ElectricCastingChance += value30;
                        break;
                    case Option.SplitPiercingNumber:
                        int value31 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.SplitPiercingNumber += value31;
                        break;
                    case Option.SpikeTrapNumber:
                        int value32 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.SpikeTrapNumber += value32;
                        break;
                    case Option.SpikeTrapDamageRadius:
                        float value33 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.SpikeTrapDamageRadius += value33;
                        break;
                    case Option.VenomousStingNumber:
                        int value34 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.VenomousStingNumber += value34;
                        break;
                    case Option.VenomousStingDamageRadius:
                        float value35 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.VenomousStingDamageRadius += value35;
                        break;
                    case Option.MagneticWavesRangeNumber:
                        float value36 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MagneticWavesRangeNumber += value36;
                        break;
                    case Option.LastLeafAblityNumber:
                        float value37 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.LastLeafAblityNumber += value37;
                        break;
                    case Option.ChainLightningNumber:
                        float value38 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.VenomousStingDamageRadius += value38;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    #endregion
    #region 유저 장비 장착 관련 (민영님 제작)
    [Obsolete("구버전 함수 / HuntertImeData기반으로 하는 함수로 변경 필요함")]
    public static void AddChangOptionToStat(HunterStat userstat, Item item)
    {
        foreach (var option in item.GetOptions.Select((value, index) => (value, index)))
        {

            var value = option.value;
            var index = option.index;
            switch (value)
            {
                case Utill_Enum.Option.None:
                    break;
                case Utill_Enum.Option.PhysicalPower:
                    float value0 = (float)item.FixedValues[index];
                    userstat.PhysicalPower += value0;
                    break;
                case Utill_Enum.Option.AttackRange:
                    float value1 = (float)item.FixedValues[index];
                    userstat.AttackRange += value1;
                    break;
                case Utill_Enum.Option.AttackSpeed:
                    float value2 = (float)item.FixedValues[index];
                    userstat.AttackSpeed += value2;
                    break;
                case Utill_Enum.Option.MagicPower:
                    float value3 = (float)item.FixedValues[index];
                    userstat.MagicPower += value3;
                    break;
                case Utill_Enum.Option.CriDamage:
                    float value4 = (float)item.FixedValues[index];
                    userstat.CriDamage += value4;
                    break;
                case Utill_Enum.Option.HP:
                    float value5 = (float)item.FixedValues[index];
                    userstat.HP += value5;
                    break;
                case Utill_Enum.Option.MP:
                    float value6 = (float)item.FixedValues[index];
                    userstat.MP += value6;
                    break;
                case Utill_Enum.Option.CriChance:
                    float value7 = (float)item.FixedValues[index];
                    userstat.CriChance += value7;
                    break;
                case Utill_Enum.Option.ArrowCount:
                    int value8 = (int)item.FixedValues[index];
                    userstat.ArrowCount += value8;
                    break;
                case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                    float value10 = (float)item.FixedValues[index];
                    userstat.InstantKillBossOnBasicAttack += value10;
                    break;
                case Utill_Enum.Option.ReflectRange:
                    float value11 = (float)item.FixedValues[index];
                    userstat.ReflectRange += value11;
                    break;
                case Utill_Enum.Option.AddDamageArrowRain:
                    float value12 = (float)item.FixedValues[index];
                    userstat.AddDamageArrowRain += value12;
                    break;
                case Utill_Enum.Option.ElectricshockNumber:
                    float value13 = (float)item.FixedValues[index];
                    userstat.ElectricshockNumber += value13;
                    break;
                case Utill_Enum.Option.AddDamageChainLightning:
                    float value14 = (float)item.FixedValues[index];
                    userstat.AddDamageChainLightning += value14;
                    break;
                case Utill_Enum.Option.AddDamageHammerSummon:
                    float value15 = (float)item.FixedValues[index];
                    userstat.AddDamageHammerSummon += value15;
                    break;
                case Utill_Enum.Option.AddDamageMagneticWaves:
                    float value16 = (float)item.FixedValues[index];
                    userstat.AddDamageMagneticWaves += value16;
                    break;
                case Utill_Enum.Option.AddDamageStrongShot:
                    float value17 = (float)item.FixedValues[index];
                    userstat.AddDamageStrongShot += value17;
                    break;
                case Utill_Enum.Option.AddDamageRapidShot:
                    float value18 = (float)item.FixedValues[index];
                    userstat.AddDamageRapidShot += value18;
                    break;
                case Utill_Enum.Option.AddDamageDemonicBlow:
                    float value19 = (float)item.FixedValues[index];
                    userstat.AddDamageDemonicBlow += value19;
                    break;
                case Utill_Enum.Option.AddDamageSpikeTrap:
                    float value20 = (float)item.FixedValues[index];
                    userstat.AddDamageSpikeTrap += value20;
                    break;
                case Utill_Enum.Option.AddDamageVenomousSting:
                    float value21 = (float)item.FixedValues[index];
                    userstat.AddDamageVenomousSting += value21;
                    break;
                case Utill_Enum.Option.AddDamageElectric:
                    float value22 = (float)item.FixedValues[index];
                    userstat.AddDamageElectric += value22;
                    break;
                case Utill_Enum.Option.AddDamageWhirlwindRush:
                    float value23 = (float)item.FixedValues[index];
                    userstat.AddDamageWhirlwindRush += value23;
                    break;
                case Utill_Enum.Option.AddDamageDokkaebiFire:
                    float value24 = (float)item.FixedValues[index];
                    userstat.AddDamageDokkaebiFire += value24;
                    break;

                case Utill_Enum.Option.AddDamageMeteor:
                    float value25 = (float)item.FixedValues[index];
                    userstat.AddDamageMeteor += value25;
                    break;
                case Utill_Enum.Option.AddDamageElectricshock:
                    float value26 = (float)item.FixedValues[index];
                    userstat.AddDamageElectricshock += value26;
                    break;
                case Utill_Enum.Option.BeastTransformationStackCountReduce:
                    int value27 = (int)item.FixedValues[index];
                    userstat.BeastTransformationStackCountReduce += value27;
                    break;
                case Utill_Enum.Option.ContinuousHitStackCountReduce:
                    int value28 = (int)item.FixedValues[index];
                    userstat.ContinuousHitStackCountReduce += value28;
                    break;
                case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                    float value29 = (float)item.FixedValues[index];
                    userstat.DokkaebiFireRotationSpeed += value29;
                    break;
                case Option.ElectricCastingChance:
                    int value30 = (int)item.FixedValues[index];
                    userstat.ElectricCastingChance += value30;
                    break;
                case Option.SplitPiercingNumber:
                    int value31 = (int)item.FixedValues[index];
                    userstat.SplitPiercingNumber += value31;
                    break;
                case Option.SpikeTrapNumber:
                    int value32 = (int)item.FixedValues[index];
                    userstat.SpikeTrapNumber += value32;
                    break;
                case Option.SpikeTrapDamageRadius:
                    float value33 = (float)item.FixedValues[index];
                    userstat.SpikeTrapDamageRadius += value33;
                    break;
                case Option.VenomousStingNumber:
                    int value34 = (int)item.FixedValues[index];
                    userstat.VenomousStingNumber += value34;
                    break;
                case Option.VenomousStingDamageRadius:
                    float value35 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius += value35;
                    break;
                case Option.MagneticWavesRangeNumber:
                    float value36 = (float)item.FixedValues[index];
                    userstat.MagneticWavesRangeNumber += value36;
                    break;
                case Option.LastLeafAblityNumber:
                    float value37 = (float)item.FixedValues[index];
                    userstat.LastLeafAblityNumber += value37;
                    break;
                case Option.ChainLightningNumber:
                    float value38 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius += value38;
                    break;

            }
        }

        //램덤 옵 추가 
        foreach (var option in item.SelectRandOptions.Select((value, index) => (value, index)))
        {

            var value = option.value;
            var index = option.index;
            switch (value)
            {
                case Utill_Enum.Option.None:
                    break;
                case Utill_Enum.Option.PhysicalPower:
                    float value0 = (float)item.RandomValues[index];
                    userstat.PhysicalPower += value0;
                    break;
                case Utill_Enum.Option.AttackRange:
                    float value1 = (float)item.RandomValues[index];
                    userstat.AttackRange += value1;
                    break;
                case Utill_Enum.Option.AttackSpeed:
                    float value2 = (float)item.RandomValues[index];
                    userstat.AttackSpeed += value2;
                    break;
                case Utill_Enum.Option.MagicPower:
                    float value3 = (float)item.RandomValues[index];
                    userstat.MagicPower += value3;
                    break;
                case Utill_Enum.Option.CriDamage:
                    float value4 = (float)item.RandomValues[index];
                    userstat.CriDamage += value4;
                    break;
                case Utill_Enum.Option.HP:
                    float value5 = (float)item.RandomValues[index];
                    userstat.HP += value5;
                    break;
                case Utill_Enum.Option.MP:
                    float value6 = (float)item.RandomValues[index];
                    userstat.MP += value6;
                    break;
                case Utill_Enum.Option.CriChance:
                    float value7 = (float)item.RandomValues[index];
                    userstat.CriChance += value7;
                    break;
                case Utill_Enum.Option.ArrowCount:
                    int value8 = (int)item.RandomValues[index];
                    userstat.ArrowCount += value8;
                    break;
                case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                    float value10 = (float)item.FixedValues[index];
                    userstat.InstantKillBossOnBasicAttack += value10;
                    break;
                case Utill_Enum.Option.ReflectRange:
                    float value11 = (float)item.FixedValues[index];
                    userstat.ReflectRange += value11;
                    break;
                case Utill_Enum.Option.AddDamageArrowRain:
                    float value12 = (float)item.FixedValues[index];
                    userstat.AddDamageArrowRain += value12;
                    break;
                case Utill_Enum.Option.ElectricshockNumber:
                    float value13 = (float)item.FixedValues[index];
                    userstat.ElectricshockNumber += value13;
                    break;
                case Utill_Enum.Option.AddDamageChainLightning:
                    float value14 = (float)item.FixedValues[index];
                    userstat.AddDamageChainLightning += value14;
                    break;
                case Utill_Enum.Option.AddDamageHammerSummon:
                    float value15 = (float)item.FixedValues[index];
                    userstat.AddDamageHammerSummon += value15;
                    break;
                case Utill_Enum.Option.AddDamageMagneticWaves:
                    float value16 = (float)item.FixedValues[index];
                    userstat.AddDamageMagneticWaves += value16;
                    break;
                case Utill_Enum.Option.AddDamageStrongShot:
                    float value17 = (float)item.FixedValues[index];
                    userstat.AddDamageStrongShot += value17;
                    break;
                case Utill_Enum.Option.AddDamageRapidShot:
                    float value18 = (float)item.FixedValues[index];
                    userstat.AddDamageRapidShot += value18;
                    break;
                case Utill_Enum.Option.AddDamageDemonicBlow:
                    float value19 = (float)item.FixedValues[index];
                    userstat.AddDamageDemonicBlow += value19;
                    break;
                case Utill_Enum.Option.AddDamageSpikeTrap:
                    float value20 = (float)item.FixedValues[index];
                    userstat.AddDamageSpikeTrap += value20;
                    break;
                case Utill_Enum.Option.AddDamageVenomousSting:
                    float value21 = (float)item.FixedValues[index];
                    userstat.AddDamageVenomousSting += value21;
                    break;
                case Utill_Enum.Option.AddDamageElectric:
                    float value22 = (float)item.FixedValues[index];
                    userstat.AddDamageElectric += value22;
                    break;
                case Utill_Enum.Option.AddDamageWhirlwindRush:
                    float value23 = (float)item.FixedValues[index];
                    userstat.AddDamageWhirlwindRush += value23;
                    break;
                case Utill_Enum.Option.AddDamageDokkaebiFire:
                    float value24 = (float)item.FixedValues[index];
                    userstat.AddDamageDokkaebiFire += value24;
                    break;
                case Utill_Enum.Option.AddDamageMeteor:
                    float value25 = (float)item.FixedValues[index];
                    userstat.AddDamageMeteor += value25;
                    break;
                case Utill_Enum.Option.AddDamageElectricshock:
                    float value26 = (float)item.FixedValues[index];
                    userstat.AddDamageElectricshock += value26;
                    break;
                case Utill_Enum.Option.BeastTransformationStackCountReduce:
                    int value27 = (int)item.FixedValues[index];
                    userstat.BeastTransformationStackCountReduce += value27;
                    break;
                case Utill_Enum.Option.ContinuousHitStackCountReduce:
                    int value28 = (int)item.FixedValues[index];
                    userstat.ContinuousHitStackCountReduce += value28;
                    break;
                case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                    float value29 = (float)item.FixedValues[index];
                    userstat.DokkaebiFireRotationSpeed += value29;
                    break;
                case Option.ElectricCastingChance:
                    int value30 = (int)item.FixedValues[index];
                    userstat.ElectricCastingChance += value30;
                    break;
                case Option.SplitPiercingNumber:
                    int value31 = (int)item.FixedValues[index];
                    userstat.SplitPiercingNumber += value31;
                    break;
                case Option.SpikeTrapNumber:
                    int value32 = (int)item.FixedValues[index];
                    userstat.SpikeTrapNumber += value32;
                    break;
                case Option.SpikeTrapDamageRadius:
                    float value33 = (float)item.FixedValues[index];
                    userstat.SpikeTrapDamageRadius += value33;
                    break;
                case Option.VenomousStingNumber:
                    int value34 = (int)item.FixedValues[index];
                    userstat.VenomousStingNumber += value34;
                    break;
                case Option.VenomousStingDamageRadius:
                    float value35 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius += value35;
                    break;
                case Option.MagneticWavesRangeNumber:
                    float value36 = (float)item.FixedValues[index];
                    userstat.MagneticWavesRangeNumber += value36;
                    break;
                case Option.LastLeafAblityNumber:
                    float value37 = (float)item.FixedValues[index];
                    userstat.LastLeafAblityNumber += value37;
                    break;
                case Option.ChainLightningNumber:
                    float value38 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius += value38;
                    break;
            }
        }
    }
    //장비 옵 빼기
    public static void MinusChangOptionToStat(HunterStat userstat, Item item)
    {

        foreach (var option in item.GetOptions.Select((value, index) => (value, index)))
        {

            var value = option.value;
            var index = option.index;
            switch (value)
            {
                case Utill_Enum.Option.None:
                    break;
                case Utill_Enum.Option.PhysicalPower:
                    float value0 = (float)item.FixedValues[index];
                    userstat.PhysicalPower -= value0;
                    break;
                case Utill_Enum.Option.AttackRange:
                    float value1 = (float)item.FixedValues[index];
                    userstat.AttackRange -= value1;
                    break;
                case Utill_Enum.Option.AttackSpeed:
                    float value2 = (float)item.FixedValues[index];
                    userstat.AttackSpeed -= value2;
                    break;
                case Utill_Enum.Option.MagicPower:
                    float value3 = (float)item.FixedValues[index];
                    userstat.MagicPower -= value3;
                    break;
                case Utill_Enum.Option.CriDamage:
                    float value4 = (float)item.FixedValues[index];
                    userstat.CriDamage -= value4;
                    break;
                case Utill_Enum.Option.HP:
                    float value5 = (float)item.FixedValues[index];
                    userstat.HP -= value5;
                    break;
                case Utill_Enum.Option.MP:
                    float value6 = (float)item.FixedValues[index];
                    userstat.MP -= value6;
                    break;
                case Utill_Enum.Option.CriChance:
                    float value7 = (float)item.FixedValues[index];
                    userstat.CriChance -= value7;
                    break;
                case Utill_Enum.Option.ArrowCount:
                    int value8 = (int)item.FixedValues[index];
                    userstat.ArrowCount -= value8;
                    break;
                case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                    float value10 = (float)item.FixedValues[index];
                    userstat.InstantKillBossOnBasicAttack -= value10;
                    break;
                case Utill_Enum.Option.ReflectRange:
                    float value11 = (float)item.FixedValues[index];
                    userstat.ReflectRange -= value11;
                    break;
                case Utill_Enum.Option.AddDamageArrowRain:
                    float value12 = (float)item.FixedValues[index];
                    userstat.AddDamageArrowRain -= value12;
                    break;
                case Utill_Enum.Option.ElectricshockNumber:
                    float value13 = (float)item.FixedValues[index];
                    userstat.ElectricshockNumber -= value13;
                    break;
                case Utill_Enum.Option.AddDamageChainLightning:
                    float value14 = (float)item.FixedValues[index];
                    userstat.AddDamageChainLightning -= value14;
                    break;
                case Utill_Enum.Option.AddDamageHammerSummon:
                    float value15 = (float)item.FixedValues[index];
                    userstat.AddDamageHammerSummon -= value15;
                    break;
                case Utill_Enum.Option.AddDamageMagneticWaves:
                    float value16 = (float)item.FixedValues[index];
                    userstat.AddDamageMagneticWaves -= value16;
                    break;
                case Utill_Enum.Option.AddDamageStrongShot:
                    float value17 = (float)item.FixedValues[index];
                    userstat.AddDamageStrongShot -= value17;
                    break;
                case Utill_Enum.Option.AddDamageRapidShot:
                    float value18 = (float)item.FixedValues[index];
                    userstat.AddDamageRapidShot -= value18;
                    break;
                case Utill_Enum.Option.AddDamageDemonicBlow:
                    float value19 = (float)item.FixedValues[index];
                    userstat.AddDamageDemonicBlow -= value19;
                    break;
                case Utill_Enum.Option.AddDamageSpikeTrap:
                    float value20 = (float)item.FixedValues[index];
                    userstat.AddDamageSpikeTrap -= value20;
                    break;
                case Utill_Enum.Option.AddDamageVenomousSting:
                    float value21 = (float)item.FixedValues[index];
                    userstat.AddDamageVenomousSting -= value21;
                    break;
                case Utill_Enum.Option.AddDamageElectric:
                    float value22 = (float)item.FixedValues[index];
                    userstat.AddDamageElectric -= value22;
                    break;
                case Utill_Enum.Option.AddDamageWhirlwindRush:
                    float value23 = (float)item.FixedValues[index];
                    userstat.AddDamageWhirlwindRush -= value23;
                    break;
                case Utill_Enum.Option.AddDamageDokkaebiFire:
                    float value24 = (float)item.FixedValues[index];
                    userstat.AddDamageDokkaebiFire -= value24;
                    break;
                case Utill_Enum.Option.AddDamageMeteor:
                    float value25 = (float)item.FixedValues[index];
                    userstat.AddDamageMeteor -= value25;
                    break;
                case Utill_Enum.Option.AddDamageElectricshock:
                    float value26 = (float)item.FixedValues[index];
                    userstat.AddDamageElectricshock -= value26;
                    break;
                case Utill_Enum.Option.BeastTransformationStackCountReduce:
                    int value27 = (int)item.FixedValues[index];
                    userstat.BeastTransformationStackCountReduce -= value27;
                    break;
                case Utill_Enum.Option.ContinuousHitStackCountReduce:
                    int value28 = (int)item.FixedValues[index];
                    userstat.ContinuousHitStackCountReduce -= value28;
                    break;
                case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                    float value29 = (float)item.FixedValues[index];
                    userstat.DokkaebiFireRotationSpeed -= value29;
                    break;
                case Option.ElectricCastingChance:
                    int value30 = (int)item.FixedValues[index];
                    userstat.ElectricCastingChance -= value30;
                    break;
                case Option.SplitPiercingNumber:
                    int value31 = (int)item.FixedValues[index];
                    userstat.SplitPiercingNumber -= value31;
                    break;
                case Option.SpikeTrapNumber:
                    int value32 = (int)item.FixedValues[index];
                    userstat.SpikeTrapNumber -= value32;
                    break;
                case Option.SpikeTrapDamageRadius:
                    float value33 = (float)item.FixedValues[index];
                    userstat.SpikeTrapDamageRadius -= value33;
                    break;
                case Option.VenomousStingNumber:
                    int value34 = (int)item.FixedValues[index];
                    userstat.VenomousStingNumber -= value34;
                    break;
                case Option.VenomousStingDamageRadius:
                    float value35 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius -= value35;
                    break;
                case Option.MagneticWavesRangeNumber:
                    float value36 = (float)item.FixedValues[index];
                    userstat.MagneticWavesRangeNumber -= value36;
                    break;
                case Option.LastLeafAblityNumber:
                    float value37 = (float)item.FixedValues[index];
                    userstat.LastLeafAblityNumber -= value37;
                    break;
                case Option.ChainLightningNumber:
                    float value38 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius -= value38;
                    break;
            }
        }





        //램덤 옵 추가 


        foreach (var option in item.SelectRandOptions.Select((value, index) => (value, index)))
        {

            var value = option.value;
            var index = option.index;
            switch (value)
            {
                case Utill_Enum.Option.None:
                    break;
                case Utill_Enum.Option.PhysicalPower:
                    float value0 = (float)item.RandomValues[index];
                    userstat.PhysicalPower -= value0;
                    break;
                case Utill_Enum.Option.AttackRange:
                    float value1 = (float)item.RandomValues[index];
                    userstat.AttackRange -= value1;
                    break;
                case Utill_Enum.Option.AttackSpeed:
                    float value2 = (float)item.RandomValues[index];
                    userstat.AttackSpeed -= value2;
                    break;
                case Utill_Enum.Option.MagicPower:
                    float value3 = (float)item.RandomValues[index];
                    userstat.MagicPower -= value3;
                    break;
                case Utill_Enum.Option.CriDamage:
                    float value4 = (float)item.RandomValues[index];
                    userstat.CriDamage -= value4;
                    break;
                case Utill_Enum.Option.HP:
                    float value5 = (float)item.RandomValues[index];
                    userstat.HP -= value5;
                    break;
                case Utill_Enum.Option.MP:
                    float value6 = (float)item.RandomValues[index];
                    userstat.MP -= value6;
                    break;
                case Utill_Enum.Option.CriChance:
                    float value7 = (float)item.RandomValues[index];
                    userstat.CriChance -= value7;
                    break;
                case Utill_Enum.Option.ArrowCount:
                    int value8 = (int)item.RandomValues[index];
                    userstat.ArrowCount -= value8;
                    break;
                case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                    float value10 = (float)item.FixedValues[index];
                    userstat.InstantKillBossOnBasicAttack -= value10;
                    break;
                case Utill_Enum.Option.ReflectRange:
                    float value11 = (float)item.FixedValues[index];
                    userstat.ReflectRange -= value11;
                    break;
                case Utill_Enum.Option.AddDamageArrowRain:
                    float value12 = (float)item.FixedValues[index];
                    userstat.AddDamageArrowRain -= value12;
                    break;
                case Utill_Enum.Option.ElectricshockNumber:
                    float value13 = (float)item.FixedValues[index];
                    userstat.ElectricshockNumber -= value13;
                    break;
                case Utill_Enum.Option.AddDamageChainLightning:
                    float value14 = (float)item.FixedValues[index];
                    userstat.AddDamageChainLightning -= value14;
                    break;
                case Utill_Enum.Option.AddDamageHammerSummon:
                    float value15 = (float)item.FixedValues[index];
                    userstat.AddDamageHammerSummon -= value15;
                    break;
                case Utill_Enum.Option.AddDamageMagneticWaves:
                    float value16 = (float)item.FixedValues[index];
                    userstat.AddDamageMagneticWaves -= value16;
                    break;
                case Utill_Enum.Option.AddDamageStrongShot:
                    float value17 = (float)item.FixedValues[index];
                    userstat.AddDamageStrongShot -= value17;
                    break;
                case Utill_Enum.Option.AddDamageRapidShot:
                    float value18 = (float)item.FixedValues[index];
                    userstat.AddDamageRapidShot -= value18;
                    break;
                case Utill_Enum.Option.AddDamageDemonicBlow:
                    float value19 = (float)item.FixedValues[index];
                    userstat.AddDamageDemonicBlow -= value19;
                    break;
                case Utill_Enum.Option.AddDamageSpikeTrap:
                    float value20 = (float)item.FixedValues[index];
                    userstat.AddDamageSpikeTrap -= value20;
                    break;
                case Utill_Enum.Option.AddDamageVenomousSting:
                    float value21 = (float)item.FixedValues[index];
                    userstat.AddDamageVenomousSting -= value21;
                    break;
                case Utill_Enum.Option.AddDamageElectric:
                    float value22 = (float)item.FixedValues[index];
                    userstat.AddDamageElectric -= value22;
                    break;
                case Utill_Enum.Option.AddDamageWhirlwindRush:
                    float value23 = (float)item.FixedValues[index];
                    userstat.AddDamageWhirlwindRush -= value23;
                    break;
                case Utill_Enum.Option.AddDamageDokkaebiFire:
                    float value24 = (float)item.FixedValues[index];
                    userstat.AddDamageDokkaebiFire -= value24;
                    break;
                case Utill_Enum.Option.AddDamageMeteor:
                    float value25 = (float)item.FixedValues[index];
                    userstat.AddDamageMeteor -= value25;
                    break;
                case Utill_Enum.Option.AddDamageElectricshock:
                    float value26 = (float)item.FixedValues[index];
                    userstat.AddDamageElectricshock -= value26;
                    break;
                case Utill_Enum.Option.BeastTransformationStackCountReduce:
                    int value27 = (int)item.FixedValues[index];
                    userstat.BeastTransformationStackCountReduce -= value27;
                    break;
                case Utill_Enum.Option.ContinuousHitStackCountReduce:
                    int value28 = (int)item.FixedValues[index];
                    userstat.ContinuousHitStackCountReduce -= value28;
                    break;
                case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                    float value29 = (float)item.FixedValues[index];
                    userstat.DokkaebiFireRotationSpeed -= value29;
                    break;
                case Option.ElectricCastingChance:
                    int value30 = (int)item.FixedValues[index];
                    userstat.ElectricCastingChance -= value30;
                    break;
                case Option.SplitPiercingNumber:
                    int value31 = (int)item.FixedValues[index];
                    userstat.SplitPiercingNumber -= value31;
                    break;
                case Option.SpikeTrapNumber:
                    int value32 = (int)item.FixedValues[index];
                    userstat.SpikeTrapNumber -= value32;
                    break;
                case Option.SpikeTrapDamageRadius:
                    float value33 = (float)item.FixedValues[index];
                    userstat.SpikeTrapDamageRadius -= value33;
                    break;
                case Option.VenomousStingNumber:
                    int value34 = (int)item.FixedValues[index];
                    userstat.VenomousStingNumber -= value34;
                    break;
                case Option.VenomousStingDamageRadius:
                    float value35 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius -= value35;
                    break;
                case Option.MagneticWavesRangeNumber:
                    float value36 = (float)item.FixedValues[index];
                    userstat.MagneticWavesRangeNumber -= value36;
                    break;
                case Option.LastLeafAblityNumber:
                    float value37 = (float)item.FixedValues[index];
                    userstat.LastLeafAblityNumber -= value37;
                    break;
                case Option.ChainLightningNumber:
                    float value38 = (float)item.FixedValues[index];
                    userstat.VenomousStingDamageRadius -= value38;
                    break;
            }
        }

    }



    #endregion
    #region 유저 장비 해제 관련 (HunterItemData)
    public static void RemoveChangOptionToStat(HunterStat userstat, HunteritemData item)
    {
        for (int i = 0; i < item.FixedOptionTypes.Count; i++)
        {
            int index = i;
            Utill_Enum.Option tpye = CSVReader.ParseEnum<Utill_Enum.Option>(item.FixedOptionTypes[i]);
            switch (tpye)
            {
                case Utill_Enum.Option.None:
                    break;
                case Utill_Enum.Option.PhysicalPowerPercent:
                    float valuePhysicalPowerPercent = (float)item.FixedOptionValues[index];
                    userstat.PhysicalPowerPercent -= valuePhysicalPowerPercent;
                    break;
                case Utill_Enum.Option.MagicPowerPercent:
                    float valueMagicPowerPercent = (float)item.FixedOptionValues[index];
                    userstat.MagicPowerPercent -= valueMagicPowerPercent;
                    break;
                case Utill_Enum.Option.PhysicalPower:
                    float value0 = (float)item.FixedOptionValues[index];
                    userstat.PhysicalPower -= value0;
                    break;
                case Utill_Enum.Option.AttackRange:
                    float value1 = (float)item.FixedOptionValues[index];
                    userstat.AttackRange -= value1;
                    break;
                case Utill_Enum.Option.AttackSpeed:
                    float value2 = (float)item.FixedOptionValues[index];
                    userstat.AttackSpeed -= value2;
                    break;
                case Utill_Enum.Option.MagicPower:
                    float value3 = (float)item.FixedOptionValues[index];
                    userstat.MagicPower -= value3;
                    break;
                case Utill_Enum.Option.CriDamage:
                    float value4 = (float)item.FixedOptionValues[index];
                    userstat.CriDamage -= value4;
                    break;
                case Utill_Enum.Option.HP:
                    float value5 = (float)item.FixedOptionValues[index];
                    userstat.HP -= value5;
                    break;
                case Utill_Enum.Option.MP:
                    float value6 = (float)item.FixedOptionValues[index];
                    userstat.MP -= value6;
                    break;
                case Utill_Enum.Option.CriChance:
                    float value7 = (float)item.FixedOptionValues[index];
                    userstat.CriChance -= value7;
                    break;
                case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                    float value10 = (float)item.FixedOptionValues[index];
                    userstat.InstantKillBossOnBasicAttack -= value10;
                    break;
                case Utill_Enum.Option.ReflectRange:
                    float value11 = (float)item.FixedOptionValues[index];
                    userstat.ReflectRange -= value11;
                    break;
                case Utill_Enum.Option.AddDamageArrowRain:
                    float value12 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageArrowRain -= value12;
                    break;
                case Utill_Enum.Option.ElectricshockNumber:
                    float value13 = (float)item.FixedOptionValues[index];
                    userstat.ElectricshockNumber -= value13;
                    break;
                case Utill_Enum.Option.AddDamageChainLightning:
                    float value14 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageChainLightning -= value14;
                    break;
                case Utill_Enum.Option.AddDamageHammerSummon:
                    float value15 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageHammerSummon -= value15;
                    break;
                case Utill_Enum.Option.AddDamageMagneticWaves:
                    float value16 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageMagneticWaves -= value16;
                    break;
                case Utill_Enum.Option.AddDamageStrongShot:
                    float value17 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageStrongShot -= value17;
                    break;
                case Utill_Enum.Option.AddDamageRapidShot:
                    float value18 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageRapidShot -= value18;
                    break;
                case Utill_Enum.Option.AddDamageDemonicBlow:
                    float value19 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageDemonicBlow -= value19;
                    break;
                case Utill_Enum.Option.AddDamageSpikeTrap:
                    float value20 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageSpikeTrap -= value20;
                    break;
                case Utill_Enum.Option.AddDamageVenomousSting:
                    float value21 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageVenomousSting -= value21;
                    break;
                case Utill_Enum.Option.AddDamageElectric:
                    float value22 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageElectric -= value22;
                    break;
                case Utill_Enum.Option.AddDamageWhirlwindRush:
                    float value23 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageWhirlwindRush -= value23;
                    break;
                case Utill_Enum.Option.AddDamageDokkaebiFire:
                    float value24 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageDokkaebiFire -= value24;
                    break;

                case Utill_Enum.Option.AddDamageMeteor:
                    float value25 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageMeteor -= value25;
                    break;
                case Utill_Enum.Option.AddDamageElectricshock:
                    float value26 = (float)item.FixedOptionValues[index];
                    userstat.AddDamageElectricshock -= value26;
                    break;
                case Utill_Enum.Option.BeastTransformationStackCountReduce:
                    int value27 = (int)item.FixedOptionValues[index];
                    userstat.BeastTransformationStackCountReduce -= value27;
                    break;
                case Utill_Enum.Option.ContinuousHitStackCountReduce:
                    int value28 = (int)item.FixedOptionValues[index];
                    userstat.ContinuousHitStackCountReduce -= value28;
                    break;
                case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                    float value29 = (float)item.FixedOptionValues[index];
                    userstat.DokkaebiFireRotationSpeed -= value29;
                    break;
                case Option.ElectricCastingChance:
                    int value30 = (int)item.FixedOptionValues[index];
                    userstat.ElectricCastingChance -= value30;
                    break;
                case Option.SplitPiercingNumber:
                    int value31 = (int)item.FixedOptionValues[index];
                    userstat.SplitPiercingNumber -= value31;
                    break;
                case Option.SpikeTrapNumber:
                    int value32 = (int)item.FixedOptionValues[index];
                    userstat.SpikeTrapNumber -= value32;
                    break;
                case Option.SpikeTrapDamageRadius:
                    float value33 = (float)item.FixedOptionValues[index];
                    userstat.SpikeTrapDamageRadius -= value33;
                    break;
                case Option.VenomousStingNumber:
                    int value34 = (int)item.FixedOptionValues[index];
                    userstat.VenomousStingNumber -= value34;
                    break;
                case Option.VenomousStingDamageRadius:
                    float value35 = (float)item.FixedOptionValues[index];
                    userstat.VenomousStingDamageRadius -= value35;
                    break;
                case Option.MagneticWavesRangeNumber:
                    float value36 = (float)item.FixedOptionValues[index];
                    userstat.MagneticWavesRangeNumber -= value36;
                    break;
                case Option.LastLeafAblityNumber:
                    float value37 = (float)item.FixedOptionValues[index];
                    userstat.LastLeafAblityNumber -= value37;
                    break;
                case Option.ChainLightningNumber:
                    float value38 = (float)item.FixedOptionValues[index];
                    userstat.VenomousStingDamageRadius -= value38;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// (가챠)현재 장착한 장비 타입의 특정 등급의 보유효과를 제거함
    /// </summary>
    public static void RemoveHoldItemOptionToStat(HunterStat userstat, HunteritemData item, Grade grade)
    {
        if (item.EquipCountList != null)
        {
            string curName = userstat.SubClass.ToString() + grade.ToString() + item.Part.ToString();
            Item curGradeItem = GameDataTable.Instance.EquipmentList[userstat.SubClass][curName];
            for (int i = 0; i < curGradeItem.HoldOption.Length; i++)
            {
                int index = i;
                Utill_Enum.Option tpye = curGradeItem.HoldOption[i];
                switch (tpye)
                {
                    case Utill_Enum.Option.None:
                        break;
                    case Utill_Enum.Option.PhysicalPowerPercent:
                        float valuePhysicalPowerPercent = (float)curGradeItem.HoldOptionValue[index];
                        userstat.PhysicalPowerPercent -= valuePhysicalPowerPercent;
                        break;
                    case Utill_Enum.Option.MagicPowerPercent:
                        float valueMagicPowerPercent = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MagicPowerPercent -= valueMagicPowerPercent;
                        break;
                    case Utill_Enum.Option.PhysicalPower:
                        float value0 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.PhysicalPower -= value0;
                        break;
                    case Utill_Enum.Option.AttackRange:
                        float value1 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AttackRange -= value1;
                        break;
                    case Utill_Enum.Option.AttackSpeed:
                        float value2 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AttackSpeed -= value2;
                        break;
                    case Utill_Enum.Option.MagicPower:
                        float value3 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MagicPower -= value3;
                        break;
                    case Utill_Enum.Option.CriDamage:
                        float value4 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.CriDamage -= value4;
                        break;
                    case Utill_Enum.Option.HP:
                        float value5 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.HP -= value5;
                        break;
                    case Utill_Enum.Option.MP:
                        float value6 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MP -= value6;
                        break;
                    case Utill_Enum.Option.CriChance:
                        float value7 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.CriChance -= value7;
                        break;
                    case Utill_Enum.Option.ArrowCount:
                        int value8 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.ArrowCount -= value8;
                        break;
                    case Utill_Enum.Option.InstantKillBossOnBasicAttack:
                        float value10 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.InstantKillBossOnBasicAttack -= value10;
                        break;
                    case Utill_Enum.Option.ReflectRange:
                        float value11 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.ReflectRange -= value11;
                        break;
                    case Utill_Enum.Option.AddDamageArrowRain:
                        float value12 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageArrowRain -= value12;
                        break;
                    case Utill_Enum.Option.ElectricshockNumber:
                        float value13 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.ElectricshockNumber -= value13;
                        break;
                    case Utill_Enum.Option.AddDamageChainLightning:
                        float value14 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageChainLightning -= value14;
                        break;
                    case Utill_Enum.Option.AddDamageHammerSummon:
                        float value15 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageHammerSummon -= value15;
                        break;
                    case Utill_Enum.Option.AddDamageMagneticWaves:
                        float value16 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageMagneticWaves -= value16;
                        break;
                    case Utill_Enum.Option.AddDamageStrongShot:
                        float value17 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageStrongShot -= value17;
                        break;
                    case Utill_Enum.Option.AddDamageRapidShot:
                        float value18 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageRapidShot -= value18;
                        break;
                    case Utill_Enum.Option.AddDamageDemonicBlow:
                        float value19 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageDemonicBlow -= value19;
                        break;
                    case Utill_Enum.Option.AddDamageSpikeTrap:
                        float value20 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageSpikeTrap -= value20;
                        break;
                    case Utill_Enum.Option.AddDamageVenomousSting:
                        float value21 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageVenomousSting -= value21;
                        break;
                    case Utill_Enum.Option.AddDamageElectric:
                        float value22 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageElectric -= value22;
                        break;
                    case Utill_Enum.Option.AddDamageWhirlwindRush:
                        float value23 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageWhirlwindRush -= value23;
                        break;
                    case Utill_Enum.Option.AddDamageDokkaebiFire:
                        float value24 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageDokkaebiFire -= value24;
                        break;

                    case Utill_Enum.Option.AddDamageMeteor:
                        float value25 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageMeteor -= value25;
                        break;
                    case Utill_Enum.Option.AddDamageElectricshock:
                        float value26 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.AddDamageElectricshock -= value26;
                        break;
                    case Utill_Enum.Option.BeastTransformationStackCountReduce:
                        int value27 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.BeastTransformationStackCountReduce -= value27;
                        break;
                    case Utill_Enum.Option.ContinuousHitStackCountReduce:
                        int value28 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.ContinuousHitStackCountReduce -= value28;
                        break;
                    case Utill_Enum.Option.DokkaebiFireRotationSpeed:
                        float value29 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.DokkaebiFireRotationSpeed -= value29;
                        break;
                    case Option.ElectricCastingChance:
                        int value30 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.ElectricCastingChance -= value30;
                        break;
                    case Option.SplitPiercingNumber:
                        int value31 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.SplitPiercingNumber -= value31;
                        break;
                    case Option.SpikeTrapNumber:
                        int value32 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.SpikeTrapNumber -= value32;
                        break;
                    case Option.SpikeTrapDamageRadius:
                        float value33 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.SpikeTrapDamageRadius -= value33;
                        break;
                    case Option.VenomousStingNumber:
                        int value34 = (int)curGradeItem.HoldOptionValue[index];
                        userstat.VenomousStingNumber -= value34;
                        break;
                    case Option.VenomousStingDamageRadius:
                        float value35 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.VenomousStingDamageRadius -= value35;
                        break;
                    case Option.MagneticWavesRangeNumber:
                        float value36 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.MagneticWavesRangeNumber -= value36;
                        break;
                    case Option.LastLeafAblityNumber:
                        float value37 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.LastLeafAblityNumber -= value37;
                        break;
                    case Option.ChainLightningNumber:
                        float value38 = (float)curGradeItem.HoldOptionValue[index];
                        userstat.ChainLightningNumber -= value38;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    #endregion

           

    #region Option Enum값으로 자신의 실제 스탯을 반환
    public static float GetUserStatPerOption(HunterStat hunterStat,Option option,bool isCalculateValue)
    {
        switch (option)
        {
            case Option.None:
                return -1;
            case Option.AttackSpeed:
                if (isCalculateValue)
                    return HunterStat.AttackSpeedResult(hunterStat);
                else
                    return hunterStat.AttackSpeed;
            case Option.AttackSpeedPercent:
                return hunterStat.AttackSpeed;
            case Option.MoveSpeed:
                if (isCalculateValue)
                    return HunterStat.MoveSpeedResult(hunterStat);
                else
                    return hunterStat.MoveSpeed;
            case Option.MoveSpeedPercent:
                return hunterStat.MoveSpeedPercent;
            case Option.GroupMoveSpeed:
                if (isCalculateValue)
                    return HunterStat.MoveSpeedResult(hunterStat);
                else
                    return hunterStat.GroupMoveSpeed;
            case Option.AttackRange:
                if (isCalculateValue)
                    return HunterStat.AttackRangeResult(hunterStat);
                else
                    return hunterStat.AttackRange;
            case Option.PhysicalPower:
                if (isCalculateValue)
                    return HunterStat.PhysicalPowerResult(hunterStat);
                else
                    return hunterStat.PhysicalPower;
            case Option.PhysicalPowerPercent:
                return hunterStat.PhysicalPowerPercent;
            case Option.MagicPower:
                if (isCalculateValue)
                    return HunterStat.MagicPowerResult(hunterStat);
                else
                    return hunterStat.MagicPower;
            case Option.MagicPowerPercent:
                return hunterStat.MagicPowerPercent;
            case Option.PhysicalTrueDamage:
                if (isCalculateValue)
                    return HunterStat.PhysicalTrueDamageResult(hunterStat);
                else
                    return hunterStat.PhysicalTrueDamage;
            case Option.PhysicalTrueDamagePercent:
                return hunterStat.PhysicalTrueDamagePercent;
            case Option.MagicTrueDamage:
                if (isCalculateValue)
                    return HunterStat.MagicTrueDamageResult(hunterStat);
                else
                    return hunterStat.MagicTrueDamage;
            case Option.MagicTrueDamagePercent:
                return hunterStat.MagicTrueDamagePercent;
            case Option.CriChance:
                    return hunterStat.CriChance;
            case Option.CriDamage:
                return hunterStat.CriDamage;
            case Option.PhysicalPowerDefense:
                if (isCalculateValue)
                    return HunterStat.PhysicalPowerDefenseResult(hunterStat);
                else
                    return hunterStat.PhysicalPowerDefense;
            case Option.PhysicalPowerDefensePercent:
                return hunterStat.PhysicalPowerDefensePercent;
            case Option.MagicPowerDefense:
                if (isCalculateValue)
                    return HunterStat.MagicPowerDefenseResult(hunterStat);
                else
                    return hunterStat.MagicPowerDefense;
            case Option.MagicPowerDefensePercent:
                return hunterStat.MagicPowerDefensePercent;
            case Option.PhysicalDamageReduction:
                return hunterStat.PhysicalDamageReduction;
            case Option.MagicDamageReduction:
                return hunterStat.MagicDamageReduction;
            case Option.CCResist:
                return hunterStat.CCResist;
            case Option.DodgeChance:
                return hunterStat.DodgeChance;
            case Option.HP:
                return hunterStat.HP;
            case Option.HPPercent:
                return hunterStat.HPPercent;
            case Option.MP:
                return hunterStat.MP;
            case Option.MPPercent:
                return hunterStat.MPPercent;
            case Option.ExpBuff:
                return hunterStat.ExpBuff;
            case Option.GoldBuff:
                return hunterStat.GoldBuff;
            case Option.ItemBuff:
                //return hunterStat.ItemBuff;
                return -1;
            case Option.AddDamageToNormalMob:
                return hunterStat.AddDamageToNormalMob;
            case Option.AddDamageToBossMob:
                return hunterStat.AddDamageToBossMob;
            case Option.ArrowCount:
                return hunterStat.ArrowCount;
            case Option.InstantKillBossOnBasicAttack:
                return hunterStat.InstantKillBossOnBasicAttack;
            case Option.ReflectRange:
                return hunterStat.ReflectRange;
            case Option.AddDamageArrowRain:
                return hunterStat.AddDamageArrowRain;
            case Option.ElectricshockNumber:
                return hunterStat.ElectricshockNumber;
            case Option.AddDamageChainLightning:
                return hunterStat.AddDamageChainLightning;
            case Option.AddDamageHammerSummon:
                return hunterStat.AddDamageHammerSummon;
            case Option.AddDamageMagneticWaves:
                return hunterStat.AddDamageMagneticWaves;
            case Option.AddDamageStrongShot:
                return hunterStat.AddDamageStrongShot;
            case Option.AddDamageRapidShot:
                return hunterStat.AddDamageRapidShot;
            case Option.AddDamageDemonicBlow:
                return hunterStat.AddDamageDemonicBlow;
            case Option.AddDamageSpikeTrap:
                return hunterStat.AddDamageSpikeTrap;
            case Option.AddDamageVenomousSting:
                return hunterStat.AddDamageVenomousSting;
            case Option.AddDamageElectric:
                return hunterStat.AddDamageElectric;
            case Option.AddDamageWhirlwindRush:
                return hunterStat.AddDamageWhirlwindRush;
            case Option.AddDamageDokkaebiFire:
                return hunterStat.AddDamageDokkaebiFire;
            case Option.AddDamageMeteor:
                return hunterStat.AddDamageMeteor;
            case Option.AddDamageElectricshock:
                return hunterStat.AddDamageElectricshock;
            case Option.BeastTransformationStackCountReduce:
                return hunterStat.BeastTransformationStackCountReduce;
            case Option.ContinuousHitStackCountReduce:
                return hunterStat.ContinuousHitStackCountReduce;
            case Option.DokkaebiFireRotationSpeed:
                return hunterStat.DokkaebiFireRotationSpeed;
            case Option.ElectricCastingChance:
                return hunterStat.ElectricCastingChance;
            case Option.SplitPiercingNumber:
                return hunterStat.SplitPiercingNumber;
            case Option.SpikeTrapNumber:
                return hunterStat.SpikeTrapNumber;
            case Option.SpikeTrapDamageRadius:
                return hunterStat.SpikeTrapDamageRadius;
            case Option.VenomousStingNumber:
                return hunterStat.VenomousStingNumber;
            case Option.VenomousStingDamageRadius:
                return hunterStat.VenomousStingDamageRadius;
            case Option.WhirlwindRushSpeedPercent:
                return hunterStat.WhirlwindRushSpeedPercent;
            case Option.ChainLightningNumber:
                return hunterStat.ChainLightningNumber;
            case Option.LastLeafAblityNumber:
                return hunterStat.LastLeafAblityNumber;
            case Option.MagneticWavesRangeNumber:
                return hunterStat.MagneticWavesRangeNumber;
            case Option.CoupChance:
                return hunterStat.CoupChance;
            case Option.CoupDamage:
                return hunterStat.CoupDamage;
            case Option.HPDrain:
                return hunterStat.HPDrain;
            case Option.MPDrain:
                return hunterStat.MPDrain;
            case Option.GroupMoveSpeedPercent:
                return hunterStat.GroupMoveSpeedPercent;
        }
        Game.Debbug.Debbuger.ErrorDebug($"{option} is not found!");
        return -1;
    }
    #endregion

    #region 유저 전체강화 강화
    //유저의 스텟을 적용 시키는
    public static void Upgrade_UserStat(HunterStat userStat, List<Dictionary<string, int>> userhasupgradedata)
    {
        foreach (var upgradeData in userhasupgradedata)
        {
            foreach (var kvp in upgradeData)
            {
                switch (kvp.Key)
                {
                    case "PhysicalPower":
                        userStat.PhysicalPower += kvp.Value;
                        break;
                    case "MagicPower":
                        userStat.MagicPower += kvp.Value;
                        break;
                    case "PhysicalPowerDefense":
                        userStat.PhysicalPowerDefense += kvp.Value;
                        break;
                    case "HP":
                        userStat.HP += kvp.Value;
                        break;
                    case "MP":
                        userStat.MP += kvp.Value;
                        break;
                    case "CriChance":
                        userStat.CriChance += kvp.Value;
                        break;
                    case "CriDamage":
                        userStat.CriDamage += kvp.Value;
                        break;
                    case "AttackSpeedPercent":
                        userStat.AttackSpeedPercent += kvp.Value;
                        break;
                    case "MoveSpeedPercent":
                        userStat.MoveSpeedPercent += kvp.Value;
                        break;
                    case "GoldBuff":
                        userStat.GoldBuff += kvp.Value;
                        break;
                    case "ExpBuff":
                        userStat.ExpBuff += kvp.Value;
                        break;
                    case "ItemBuff":
                        //userStat.ItemBuff += kvp.Value; 아이템버프도 추가되면 넣기
                        break;
                    default:
                        Debug.LogWarning($"Unknown stat key: {kvp.Key}");
                        break;
                }
            }
        }
    }

    //유저가 슬롯하나강화 강화했을때 해당강화 레벨에맞는 스텟 적용해주기
    public static void UpgradeOneStat_UserStat(HunterStat userStat, int Level, Utill_Enum.Upgrade_Type type)
    {
        //리벨맞는데이터 구하가
        Hunter_UpgradeData data = GameDataTable.Instance.HunterUpgradeDataDic[Level];

        switch (type)
        {
            case Upgrade_Type.PhysicalPower:
                userStat.PhysicalPower += data.PhysicalPower;
                break;
            case Upgrade_Type.MagicPower:
                userStat.MagicPower += data.MagicPower;
                break;
            case Upgrade_Type.PhysicalPowerDefense:
                userStat.PhysicalPowerDefense += data.PhysicalPowerDefense;
                break;
            case Upgrade_Type.HP:
                userStat.HP += data.HP;
                break;
            case Upgrade_Type.MP:
                userStat.MP += data.MP;
                break;
            case Upgrade_Type.CriChance:
                userStat.CriChance += data.CriChance;
                break;
            case Upgrade_Type.CriDamage:
                userStat.CriDamage += data.CriDamage;
                break;
            case Upgrade_Type.AttackSpeedPercent:
                userStat.AttackSpeedPercent += data.AttackSpeedPercent;
                break;
            case Upgrade_Type.MoveSpeedPercent:
                userStat.MoveSpeedPercent += data.MoveSpeedPercent;
                break;
            case Upgrade_Type.GoldBuff:
                userStat.GoldBuff = data.GoldBuff;
                break;
            case Upgrade_Type.ExpBuff:
                userStat.ExpBuff += data.ExpBuff;
                break;
            case Upgrade_Type.ItemBuff:
                //userStat.PhysicalPower = data.PhysicalPolwer;
                break;
        }
    }


    #endregion

    public static void UseSkillAddUserStat(HunterStat userStat, float value, Utill_Enum.Upgrade_Type type)
    {
        switch (type)
        {
            case Upgrade_Type.PhysicalPower:
                userStat.PhysicalPower += value;
                break;
            case Upgrade_Type.MagicPower:
                userStat.MagicPower += value;
                break;
            case Upgrade_Type.PhysicalPowerDefense:
                userStat.PhysicalPowerDefense += value;
                break;
            case Upgrade_Type.HP:
                userStat.HP += value;
                break;
            case Upgrade_Type.MP:
                userStat.MP += value;
                break;
            case Upgrade_Type.CriChance:
                userStat.CriChance += value;
                break;
            case Upgrade_Type.CriDamage:
                userStat.CriDamage += value;
                break;
            case Upgrade_Type.AttackSpeedPercent:
                userStat.AttackSpeedPercent += value;
                break;
            case Upgrade_Type.MoveSpeedPercent:
                userStat.MoveSpeedPercent += value;
                break;
            case Upgrade_Type.GoldBuff:
                userStat.GoldBuff += value;
                break;
            case Upgrade_Type.ExpBuff:
                userStat.ExpBuff += value;
                break;
            case Upgrade_Type.ItemBuff:
                //userStat.PhysicalPower = data.PhysicalPolwer;
                break;
            case Upgrade_Type.ArrowCount:
                userStat.ArrowCount += (int)value;
                break;
            case Upgrade_Type.PhysicalPowerPercent:
                userStat.PhysicalPowerPercent += value;
                break;
            case Upgrade_Type.DodgeChance:
                userStat.DodgeChance += value;
                break;
            case Upgrade_Type.PhysicalPowerDefensePercent:
                userStat.PhysicalPowerDefensePercent += value;
                break;
            case Upgrade_Type.MagicPowerPercent:
                userStat.MagicPowerPercent += value;
                break;
            case Upgrade_Type.PhysicalTrueDamage:
                userStat.PhysicalTrueDamage += value;
                break;
            case Upgrade_Type.PhysicalTrueDamagePercent:
                userStat.PhysicalTrueDamagePercent += value;
                break;
            case Upgrade_Type.MagicTrueDamage:
                userStat.MagicTrueDamage += value;
                break;
            case Upgrade_Type.MagicTrueDamagePercent:
                userStat.MagicTrueDamagePercent += value;
                break;
            case Upgrade_Type.MagicPowerDefense:
                userStat.MagicPowerDefense += value;
                break;
            case Upgrade_Type.MagicPowerDefensePercen:
                //userStat.MagicPowerDefensePercen += value;
                break;
            case Upgrade_Type.AddDamageArrowRain:
                userStat.AddDamageArrowRain += value;
                break;
            case Upgrade_Type.ElectricshockNumber:
                userStat.ElectricshockNumber += value;
                break;
            case Upgrade_Type.CoupChance:
                userStat.CoupChance += value;
                break;
        }
    }

    public static void UseSkillMinusUserStat(HunterStat userStat, float value, Utill_Enum.Upgrade_Type type)
    {
        switch (type)
        {
            case Upgrade_Type.PhysicalPower:
                userStat.PhysicalPower -= value;
                break;
            case Upgrade_Type.MagicPower:
                userStat.MagicPower -= value;
                break;
            case Upgrade_Type.PhysicalPowerDefense:
                userStat.PhysicalPowerDefense -= value;
                break;
            case Upgrade_Type.HP:
                userStat.HP -= value;
                break;
            case Upgrade_Type.MP:
                userStat.MP -= value;
                break;
            case Upgrade_Type.CriChance:
                userStat.CriChance -= value;
                break;
            case Upgrade_Type.CriDamage:
                userStat.CriDamage -= value;
                break;
            case Upgrade_Type.AttackSpeedPercent:
                userStat.AttackSpeedPercent -= value;
                break;
            case Upgrade_Type.MoveSpeedPercent:
                userStat.MoveSpeedPercent -= value;
                break;
            case Upgrade_Type.GoldBuff:
                userStat.GoldBuff -= value;
                break;
            case Upgrade_Type.ExpBuff:
                userStat.ExpBuff -= value;
                break;
            case Upgrade_Type.ItemBuff:
                //userStat.PhysicalPower = data.PhysicalPolwer;
                break;
            case Upgrade_Type.ArrowCount:
                userStat.ArrowCount -= (int)value;
                break;
            case Upgrade_Type.PhysicalPowerPercent:
                userStat.PhysicalPowerPercent -= value;
                break;
            case Upgrade_Type.DodgeChance:
                userStat.DodgeChance -= value;
                break;
            case Upgrade_Type.PhysicalPowerDefensePercent:
                userStat.PhysicalPowerDefensePercent -= value;
                break;
            case Upgrade_Type.MagicPowerPercent:
                userStat.MagicPowerPercent -= value;
                break;
            case Upgrade_Type.PhysicalTrueDamage:
                userStat.PhysicalTrueDamage -= value;
                break;
            case Upgrade_Type.PhysicalTrueDamagePercent:
                userStat.PhysicalTrueDamagePercent -= value;
                break;
            case Upgrade_Type.MagicTrueDamage:
                userStat.MagicTrueDamage -= value;
                break;
            case Upgrade_Type.MagicTrueDamagePercent:
                userStat.MagicTrueDamagePercent -= value;
                break;
            case Upgrade_Type.MagicPowerDefense:
                userStat.MagicPowerDefense -= value;
                break;
            case Upgrade_Type.MagicPowerDefensePercen:
                // userStat.MagicPowerDefensePercen -= value;
                break;
            case Upgrade_Type.AddDamageArrowRain:
                userStat.AddDamageArrowRain -= value;
                break;
            case Upgrade_Type.ElectricshockNumber:
                userStat.ElectricshockNumber -= value;
                break;
            case Upgrade_Type.CoupChance:
                userStat.CoupChance -= value;
                break;
        }
    }

    #region 헌터 직업별 DPS 반환
    public static int GetHunterDPS(HunterStat hunterstat, int hunterindex = -1)
    {
        // 1. 필요한 스탯 값 가져오기
        float attackSpeed = hunterstat.AttackSpeed;        // 초당 공격 속도
        float physicalAttack = hunterstat.PhysicalPower;     // 총 물리 공격력
        float critChance = hunterstat.CriChance * 0.01f;           // 치명타 확률
        float critDamage = hunterstat.CriDamage * 0.01f;           // 치명타 피해
        float coupChance = hunterstat.CoupChance * 0.01f;           // 일격 확률
        float coupDamage = hunterstat.CoupDamage * 0.01f;          // 일격 피해

        // 3. 총 DPS 계산
        float totalDPS = attackSpeed * (physicalAttack * (1 - critChance - coupChance) + physicalAttack * critChance * (1 + critDamage) +
                         physicalAttack * coupChance * (1 + coupDamage));

        // 4. DPS 결과 반환 (정수로 변환)
        return Mathf.FloorToInt(totalDPS);
    }
    #endregion

    #region 전투력 구하는 함수
    public static int GetCombatPoint(HunterStat hunterstat, int hunterindex = -1)
    {
        // 1. DPS 값 계산
        int temp_dps = GetHunterDPS(hunterstat, hunterindex);

        // 2. 각 스탯 값 가져오기
        float TotalHp = hunterstat.HP;
        float TotalAttack = hunterstat.PhysicalPower; // 공격력
        float TotalMagicAttack = hunterstat.MagicPower; //마법 공격력
        float TotalAttackSpeed = hunterstat.AttackSpeed; // 공격 속도
        float TotalMp = hunterstat.MP;
        float TotalCriChance = hunterstat.CriChance * 0.01f; // 치명타 확률
        float TotalCriDamage = hunterstat.CriDamage; // 치명타 피해
        float TotalCoupChance = hunterstat.CoupChance * 0.01f; // 일격 확률
        float TotalCoupDamage = hunterstat.CoupDamage; // 일격 피해
        float TotalPhysicalDefense = hunterstat.PhysicalPowerDefense; // 물리 방어력
        float TotalMagicDefense = hunterstat.MagicPowerDefense; // 마법 방어력

        // 3. 레벨에 따른 가중치 n 설정 (임의로 설정)
        float hp = GameManager.Instance.HunterHpcoefficient;
        float mp = GameManager.Instance.HunterMpcoefficient;
        float magicpower = GameManager.Instance.HunterMagicPowercoefficient;
        float magicdef = GameManager.Instance.HunterPhycisDefcoefficient;
        float physicdef = GameManager.Instance.HunterMagicDefcoefficient;

        int HunterLevel = GameDataTable.Instance.User.HunterLevel[hunterindex];
        // 4. 전투력 계산 공식 적용
        float tempPoint = temp_dps // DPS 기여도
                        + (TotalHp * hp)   // 총 체력 x n x 레벨
                        + (TotalMp * mp)   // 총 마나 x n x 레벨
                        + (TotalMagicAttack * magicpower)  // 총 공격력(마법 또는 물리) x n x 레벨
                        + (TotalPhysicalDefense * physicdef)  // 총 물리 방어력 x n x 레벨
                        + (TotalMagicDefense * magicdef); // 총 마법 방어력 x n x 레벨

        // 5. 전투력 포인트를 정수로 반환
        return Mathf.FloorToInt(tempPoint);
    }
    #endregion


}