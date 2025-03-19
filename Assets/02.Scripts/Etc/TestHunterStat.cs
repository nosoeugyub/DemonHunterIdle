using Game.Debbug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 헌터 치트 스크립트
/// </summary>
public class TestHunterStat : MonoBehaviour
{
    [SerializeField] private bool _isTestStat;
    public bool isTestStat
    {
        get { return _isTestStat; }
        set
        {
            _isTestStat = value;
        }
    }


    public Utill_Enum.HeroClass Class;
    public Utill_Enum.SubClass SubClass; //혹시몰라 준비한 스텟 이름;;
    public Utill_Enum.Enum_AttackType AttacckType;
    public Utill_Enum.AttackDamageType AttacckDamageType;


    [Header("일반스텟")]
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
    public float AddDamageToBossMob; //보스에게 주는 평타피해 추가

    public float CCResist;//상태이상회복률
    public float DodgeChance; //회피율

    public float HP;//체력
    public float HPPercent;

    public float MP; //마나
    public float MPPercent;

    public float HPDrain; //체력 흡수
    public float MPDrain; //마나 흡수

    public float ExpBuff; //경험치획득 버프
    public float GoldBuff; //골드획득 버프




    [Header("특수스탯")]
    //공통 특수스탯
    public float PhysicalTrueDamage;//물리 관통피해
    public float PhysicalTrueDamagePercent;//물리 관통피해 증가
    public float MagicTrueDamage;//마법 관통피해
    public float MagicTrueDamagePercent;//마법 관통피해 증가
    public float InstantKillBossOnBasicAttack; //보스즉사
    public float ReflectRange; //원거리 공격 반사




    public virtual HunterStat MakeUserStat()
    {
        HunterStat userstat = new HunterStat(
      SubClass, Class, AttacckType, AttacckDamageType,
      AttackSpeed, AttackSpeedPercent, MoveSpeed, MoveSpeedPercent, GroupMoveSpeed, GroupMoveSpeedPercent,
      AttackRange, PhysicalPower, PhysicalPowerPercent, MagicPower,
      MagicPowerPercent, PhysicalTrueDamage, PhysicalTrueDamagePercent,
      MagicTrueDamage, MagicTrueDamagePercent, CriChance, CriDamage, CoupChance, CoupDamage,
      PhysicalPowerDefense, PhysicalPowerDefensePercent, MagicPowerDefense,
      MagicPowerDefensePercent, PhysicalDamageReduction, MagicDamageReduction,
      /*PhysicalDamageIncrease, MagicDamageIncrease,*/AddDamageToNormalMob, AddDamageToBossMob, CCResist, DodgeChance,
      HP, HPPercent, MP, MPPercent,HPDrain,MPDrain, ExpBuff, GoldBuff  , InstantKillBossOnBasicAttack , ReflectRange
      );


        //userstat.CurrentHp = userstat.HP;

        return userstat;
    }
}
