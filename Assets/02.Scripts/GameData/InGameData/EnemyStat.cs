using Game.Debbug;
using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 몬스터 스텟 제어 스크립트
/// </summary>

public class EnemyStat 
{   

    public string ProjectilePrefab;
    public float ProjectileSpeed;
    public string MobName;
    public bool SkillUseType;
    public int SkillIndex;
    public Utill_Enum.EnemyClass Class;
    public Utill_Enum.Enum_AttackType AttackType;
    public Utill_Enum.AttackDamageType AttackDamageType;
    public float AttackSpeed; //초당 공격횟수수 
    public float AttackSpeedPercent; // 초당공격횟수 %증가량

    public float MoveSpeed; //이동속도
    public float MoveSpeedPercent; //이동속도 %증가량

    public float AttackRange;// 공격시 사정거리

    private float physicalPower;
    public float PhysicalPower  //물리공격
    {
        get { return physicalPower; }
        set { physicalPower = value; }
    }
    public float PhysicalPowerPercent; //물리공격증가

    public float MagicPower;//마법공격
    public float MagicPowerPercent;//마법공격 증가

    public float PhysicalTrueDamage;//물리 관통피해
    public float PhysicalTrueDamagePercent;//물리 관통피해 증가

    public float MagicTrueDamage;//마법 관통피해
    public float MagicTrueDamagePercent;//마법 관통피해 증가

    public float CriChance;//치명타확률
    public float CriDamage;//치명타 피해


    private float physicalPowerDefense;//물리공격 방어력
    public float PhysicalPowerDefense
    {
        get { return physicalPowerDefense; }
        set { physicalPowerDefense = value; }
    }
    public float PhysicalPowerDefensePercent; //물리공격 방어력 증가

    public float MagicPowerDefense;//마법공격 방어력
    public float MagicPowerDefensePercent;//마법공격 방어력 증가

    public float PhysicalDamageReduction; //물리공격 피해감소
    public float MagicDamageReduction; //마법공격 피해 감소

    //public float PhysicalDamageIncrease; // 물리공격 피해증가
    //public float MagicDamageIncrease; //마법공격 피해증가


    public float CCResist;//상태이상회복률
    public float DodgeChance; //회피율

    private float hP;//체력
    public float HP
    {
        get { return hP; } set { hP = value; }
    }
    public float HPPercent;

    public float MP; //마나
    public float MPPercent;

    public float ExpBuff; //경험치획득 버프
    public float Exp; //경험치

    public float Level; // 레벨
    public float DamageRadius; //피해범위

    public float IntervalRate; //간격
    public float DropChance; //아이템획득확률




    /// <summary>
    /// 로컬에서만쓰는 데이터
    /// </summary>
    /// 


 


    private float currentHp;
    public float CurrentHp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    private List<DeBuff> enemyDebuffList;
    public List<DeBuff> EnemyDebuffList
    {
        get { return enemyDebuffList; }
        set { enemyDebuffList = value;}
    }



    public EnemyStat(EnemyStat other)
    {
        this.ProjectilePrefab = other.ProjectilePrefab;
        this.ProjectileSpeed = other.ProjectileSpeed;
        this.MobName = other.MobName;
        this.SkillUseType = other.SkillUseType;
        this.SkillIndex = other.SkillIndex;
        this.Class = other.Class;
        this.AttackType = other.AttackType;
        this.AttackDamageType = other.AttackDamageType;
        this.AttackSpeed = other.AttackSpeed;
        this.AttackSpeedPercent = other.AttackSpeedPercent;
        this.MoveSpeed = other.MoveSpeed;
        this.MoveSpeedPercent = other.MoveSpeedPercent;
        this.AttackRange = other.AttackRange;
        this.PhysicalPower = other.PhysicalPower;
        this.PhysicalPowerPercent = other.PhysicalPowerPercent;
        this.MagicPower = other.MagicPower;
        this.MagicPowerPercent = other.MagicPowerPercent;
        this.PhysicalTrueDamage = other.PhysicalTrueDamage;
        this.PhysicalTrueDamagePercent = other.PhysicalTrueDamagePercent;
        this.MagicTrueDamage = other.MagicTrueDamage;
        this.MagicTrueDamagePercent = other.MagicTrueDamagePercent;
        this.CriChance = other.CriChance;
        this.CriDamage = other.CriDamage;
        this.PhysicalPowerDefense = other.PhysicalPowerDefense;
        this.PhysicalPowerDefensePercent = other.PhysicalPowerDefensePercent;
        this.MagicPowerDefense = other.MagicPowerDefense;
        this.MagicPowerDefensePercent = other.MagicPowerDefensePercent;
        this.PhysicalDamageReduction = other.PhysicalDamageReduction;
        this.MagicDamageReduction = other.MagicDamageReduction;
        //this.PhysicalDamageIncrease = other.PhysicalDamageIncrease;
        //this.MagicDamageIncrease = other.MagicDamageIncrease;
        this.CCResist = other.CCResist;
        this.DodgeChance = other.DodgeChance;
        this.HP = other.HP;
        this.HPPercent = other.HPPercent;
        this.MP = other.MP;
        this.MPPercent = other.MPPercent;
        this.ExpBuff = other.ExpBuff;
        this.Exp = other.Exp;
        this.Level = other.Level;
        this.DamageRadius = other.DamageRadius;
        this.IntervalRate = other.IntervalRate;
        this.DropChance = other.DropChance;


        //로컬..
        EnemyDebuffList = new List<DeBuff>();
    }

    public EnemyStat()
    {
        HP = 0;
        PhysicalPowerDefense = 0;
        PhysicalPower = 0;
    }

    //데이터 테이블 생성자
    public EnemyStat(string mobName, bool skillUseType, int skillIndex, Utill_Enum.EnemyClass enemyClass, Utill_Enum.Enum_AttackType attackType, Utill_Enum.AttackDamageType attackDamageType, float hp, float physicalPower, float attackSpeed, float attackRange, float moveSpeed, float physicalPowerDefense, float dodgeChance, float criChance, float criDamage, float ccResist, string projectilePrefab , float projectileSpeed)
    {
        MobName = mobName;
        SkillUseType = skillUseType;
        SkillIndex = skillIndex;
        Class = enemyClass;
        AttackType = attackType;
        AttackDamageType = attackDamageType;
        HP = hp;
        PhysicalPower = physicalPower;
        AttackSpeed = attackSpeed;
        AttackRange = attackRange;
        MoveSpeed = moveSpeed;
        PhysicalPowerDefense = physicalPowerDefense;
        DodgeChance = dodgeChance;
        CriChance = criChance;
        CriDamage = criDamage;
        CCResist = ccResist;
        ProjectilePrefab = projectilePrefab;
        ProjectileSpeed = projectileSpeed;

        
    }

    //테스트매니저 생성자
    public EnemyStat(string projectilePrefab , float projectileSpeed, EnemyClass @class, Enum_AttackType attackType, AttackDamageType attackDamageType, float attackSpeed, float attackSpeedPercent, float moveSpeed, float moveSpeedPercent, float attackRange, float physicalPower, float physicalPowerPercent, float magicPower, float magicPowerPercent, float physicalTrueDamage, float physicalTrueDamagePercent, float magicTrueDamage, float magicTrueDamagePercent, float criChance, float criDamage, float physicalPowerDefense, float physicalPowerDefensePercent, float magicPowerDefense, float magicPowerDefensePercent, float physicalDamageReduction, float magicDamageReduction, /*float physicalDamageIncrease, float magicDamageIncrease,*/ float cCResist, float dodgeChance, float hP, float hPPercent, float mP, float mPPercent, float expBuff)
    {
        ProjectilePrefab = projectilePrefab;
        ProjectileSpeed = projectileSpeed;
        Class = @class;
        AttackType = attackType;
        AttackDamageType = attackDamageType;
        AttackSpeed = attackSpeed;
        AttackSpeedPercent = attackSpeedPercent;
        MoveSpeed = moveSpeed;
        MoveSpeedPercent = moveSpeedPercent;
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
        PhysicalPowerDefense = physicalPowerDefense;
        PhysicalPowerDefensePercent = physicalPowerDefensePercent;
        MagicPowerDefense = magicPowerDefense;
        MagicPowerDefensePercent = magicPowerDefensePercent;
        PhysicalDamageReduction = physicalDamageReduction;
        MagicDamageReduction = magicDamageReduction;
        //PhysicalDamageIncrease = physicalDamageIncrease;
        //MagicDamageIncrease = magicDamageIncrease;
        CCResist = cCResist;
        DodgeChance = dodgeChance;
        HP = hP;
        HPPercent = hPPercent;
        MP = mP;
        MPPercent = mPPercent;
        ExpBuff = expBuff;

        EnemyDebuffList = new List<DeBuff>();
    }




    #region EnemyStat Value
    public static float MoveSpeedResult(EnemyStat _enemystat)//이동속도 구하는함수
    {
        float temp = 0;
        float attackspeedper = _enemystat.MoveSpeedPercent * 0.01f;
        float attackspeed = _enemystat.MoveSpeed;
        temp = attackspeed + (attackspeed * attackspeedper);
        return temp;
    }
    public static float EnemyAttackSpeedResult(EnemyStat _enemystat)
    {
        float temp = 0;
        float attackspeedper = _enemystat.AttackSpeedPercent * 0.01f;
        float attackspeed = _enemystat.AttackSpeed;
        temp = attackspeed + (attackspeed * attackspeedper);
        return temp;
    }

    public static float EnemyPhysicalPowerResult(EnemyStat _enemystat)//물리공격 구하는함수
    {
        float temp = 0;
        float attackspeedper = _enemystat.PhysicalPowerPercent * 0.01f;
        float attackspeed = _enemystat.PhysicalPower;
        temp = attackspeed + (attackspeed * attackspeedper);
        return temp;
    }

    public static float EnemyMagicPowerResult(EnemyStat _enemystat)//마법물리공격 구하는함수
    {
        float temp = 0;
        float attackspeedper = _enemystat.MagicPowerPercent * 0.01f;
        float attackspeed = _enemystat.MagicPower;
        temp = attackspeed + (attackspeed * attackspeedper);
        return temp;
    }

    public static float EnemyPhysicalTrueDamageResult(EnemyStat _enemystat)//관통피해 구하는함수
    {
        float temp = 0;
        float attackspeedper = _enemystat.PhysicalTrueDamagePercent * 0.01f;
        float attackspeed = _enemystat.PhysicalTrueDamage;
        temp = attackspeed + (attackspeed * attackspeedper);
        return temp;
    }
    public static float EnemyMagicTrueDamageResult(EnemyStat _enemystat)//마법 관통피해 구하는함수
    {
        float temp = 0;
        float attackspeedper = _enemystat.MagicTrueDamagePercent * 0.01f;
        float attackspeed = _enemystat.MagicTrueDamage;
        temp = attackspeed + (attackspeed * attackspeedper);
        return temp;
    }


    #endregion





    #region 적이 헌터에게 주는 데미지 처리

    /// <param name="enemystat"></param>
    /// <param name="_userstat"></param>
    /// <param name="HeroClass"></param>  


    /// <summary>
    /// 물리데미지 타입시 처리
    /// </summary>
    public static (float damage, float additionalDamage, bool IsCritical, bool isDotge, StunStatusEffect isStun, FrozenStatusEffect isFrozen, PosionStatusEffect isPosion, SlowStatusEffect isSlow) DamageToPhysical(EnemyStat enemystat , HunterStat _userstat , Utill_Enum.HeroClass HeroClass = Utill_Enum.HeroClass.None)
    {
        //피격시 스턴 및 CC  를 먼저 검사

        //1.스턴이 검사
        //피격시 스턴 및 CC  를 먼저 검사
        StunStatusEffect stuneffect = null;
        FrozenStatusEffect frozenstatuseffect = null;
        PosionStatusEffect posionstatuseffect = null;
        SlowStatusEffect slowstatuseffect = null;

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
        //float SlowTime = HunterStat.Get_SlowTime(_userstat);
        isSlow = Utill_Math.Attempt(Slowvalue);


        if (isStun)
        {
            stuneffect = new StunStatusEffect(Utill_Enum.Debuff_Type.Stun, Stunduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isFreezon)
        {
            frozenstatuseffect = new FrozenStatusEffect(Utill_Enum.Debuff_Type.Stun, Stunduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isPosion)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            posionstatuseffect = new PosionStatusEffect(Utill_Enum.Debuff_Type.Posion, Posionduartion, Posiondamage, hunter);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isSlow)
        {
           // slowstatuseffect = new SlowStatusEffect(Utill_Enum.Debuff_Type.Slow, SlowDuration ,SlowTime);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }






        //대미지 검사
        float totalDamage = 0;
        float additionalDamage = 0;
        float totalnocridamage = 0;
        bool _isCri = false;
        //치명율
        float critChance = 0;
        bool isCrit = false;
        //회피율
        float UserDotge = 0;
        bool isdotge = false;

        // 1차 연산: 유저 데미지와 적의 방어력을 이용한 기본 대미지 계산
        float enemyDamage = EnemyPhysicalPowerResult(enemystat);
        float userDefense = _userstat.PhysicalPowerDefense;
        float tempDamage = enemyDamage - userDefense;

        // 2차 연산: 물리 피해 감소 및 무시를 고려한 추가 대미지 계산
        float physicalDamageReduction = _userstat.PhysicalDamageReduction * 0.01f; //상대방의 물리피해감소
        float enemyIgnore = 0.00f; // 나의 물리피해감소 무시
        float reductionAmount = tempDamage * (physicalDamageReduction - enemyIgnore);
        if (reductionAmount < 0)
        {
            reductionAmount = 0;
        }
        tempDamage -= reductionAmount;

        // 3차 연산: 물리 관통 대미지 추가
        tempDamage += EnemyPhysicalTrueDamageResult(enemystat);
        totalnocridamage = tempDamage;

        // 4차 연산: 물리 피해 증가 및 타입별 추가 데미지
        float hunterAddDamage = 0f;
        //보스피해 추가연산
        switch (HeroClass)
        {
            case Utill_Enum.HeroClass.None:
                break;
            case Utill_Enum.HeroClass.Hunter:
                hunterAddDamage = 0f;
                break;

            default:
                break;
        }
        float additionalDmg = /*enemystat.PhysicalDamageIncrease +*/ hunterAddDamage; // 피해 증가는 다 여기로..
        float additionalDmgPercent = (additionalDmg * 0.01f);
        float totalAdditionalDmg = tempDamage * additionalDmgPercent; //추가피해량 대미지 

        additionalDamage = totalAdditionalDmg;

        // 5차 연산: 치명타 발생 여부 및 치명타 대미지 계산 ** 회피율 등 추후연산 처리

        //상대방 회피율 타입에따라 계싼 
        switch (enemystat.AttackType)
        {
            case Utill_Enum.Enum_AttackType.None:
                //치명율
                critChance = enemystat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                UserDotge = _userstat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(UserDotge);
                break;
            case Utill_Enum.Enum_AttackType.Melee:
                //치명율
                critChance = enemystat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                UserDotge = _userstat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(UserDotge);
                break;
            case Utill_Enum.Enum_AttackType.Range:
                //치명율
                critChance = enemystat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                UserDotge = _userstat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(UserDotge);
                break;
            case Utill_Enum.Enum_AttackType.Spell:
                break;
        }
        //회피율 먼저 계싼
        if (isdotge) //상대방이 먼저 회피를 성공했다면?
        {
            isdotge = true;
            return (-1, -1, _isCri, isdotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect);
        }

        if (isCrit)
        {
            _isCri = true;
            //일반 데미지 크리티컬 계산
            float critDmg = tempDamage * (enemystat.CriDamage * 0.01f);
            tempDamage += critDmg;
            //추가 데미지 크리티컬 계산
            float additionalCritDmg = additionalDamage * (enemystat.CriDamage * 0.01f);
            additionalDamage += additionalCritDmg;

            //최후 대미지 할당 
            totalDamage = tempDamage;
            if (totalDamage <= 0)
            {
                totalDamage = 0;
            }

            return (totalDamage, additionalDamage, _isCri, isdotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect);

        }
        else
        {
            //최후 대미지 할당 
            _isCri = false;
            if (totalnocridamage <= 0)
            {
                totalnocridamage = 0;
            }
            return (totalnocridamage, additionalDamage, _isCri, isdotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect);
        }
    }


    /// <summary>
    /// 마법데미지 타입시 처리
    /// </summary>
    public static (float damage, float additionalDamage, bool iscri, bool isDotge, StunStatusEffect isStun, FrozenStatusEffect isFrozen, PosionStatusEffect isPosion, SlowStatusEffect isSlow) DamageToMagic(EnemyStat enemystat, HunterStat _userstat, Utill_Enum.HeroClass HeroClass = Utill_Enum.HeroClass.None)
    {
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
       // float SlowTime = HunterStat.Get_SlowTime(_userstat);
        isSlow = Utill_Math.Attempt(Slowvalue);


        if (isStun)
        {
            stuneffect = new StunStatusEffect(Utill_Enum.Debuff_Type.Stun, Stunduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isFreezon)
        {
            frozenstatuseffect = new FrozenStatusEffect(Utill_Enum.Debuff_Type.Stun, Stunduartion);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isPosion)
        {
            Hunter hunter = null;
            hunter = DataManager.Instance.GetHunterUsingSubClass(_userstat.SubClass);
            posionstatuseffect = new PosionStatusEffect(Utill_Enum.Debuff_Type.Posion, Posionduartion, Posiondamage, hunter);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }
        if (isSlow)
        {
           // slowstatuseffect = new SlowStatusEffect(Utill_Enum.Debuff_Type.Slow, SlowDuration , SlowTime);

            //DeBuff Debuff = new DeBuff(Utill_Enum.Debuff_Type.Stun, 3);
            //DeBuff.Add_Debuff(enemystat, Debuff);
        }




        // 1차 연산: 유저 데미지와 적의 방어력을 이용한 기본 대미지 계산
        float enemyDamage = EnemyMagicPowerResult(enemystat);
        float userDefense = _userstat.MagicPowerDefense;
        float tempDamage = enemyDamage - userDefense;

        // 2차 연산: 물리 피해 감소 및 무시를 고려한 추가 대미지 계산
        float physicalDamageReduction = _userstat.MagicDamageReduction * 0.01f; //상대물리피해감소율
        float enemyIgnore = 0.00f; // 나의 물리피해감소무시
        float reductionAmount = tempDamage * (physicalDamageReduction - enemyIgnore);
        if (reductionAmount < 0)
        {
            reductionAmount = 0;
        }
        tempDamage -= reductionAmount;

        // 3차 연산: 물리 관통 대미지 추가
        tempDamage += EnemyMagicTrueDamageResult(enemystat);
        totalnocridamage = tempDamage;

        // 4차 연산: 물리 피해 증가 및 타입별 추가 데미지
        float hunterAddDamage = 0f;
        //보스피해 추가연산
        switch (HeroClass)
        {
            case Utill_Enum.HeroClass.None:
                break;
            case Utill_Enum.HeroClass.Hunter:
                hunterAddDamage = 0f;
                break;

            default:
                break;
        }
        float additionalDmg = /*enemystat.PhysicalDamageIncrease +*/ hunterAddDamage; //피해 증가는 다 여기로..
        float additionalDmgPercent = additionalDmg * 0.01f;
        float totalAdditionalDmg = tempDamage * additionalDmgPercent; //추가피해량 대미지 

        additionalDamage = additionalDmgPercent;

        // 5차 연산: 치명타 발생 여부 및 치명타 대미지 계산 ** 회피율 등 추후연산 처리
        //치명율
        float critChance = 0;
        bool isCrit = false;
        //회피율
        float UserDotge = 0;
        bool isdotge = false;
        //상대방 회피율 타입에따라 계산
        switch (enemystat.AttackType)
        {
            case Utill_Enum.Enum_AttackType.Melee:
                //치명율
                critChance = enemystat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                UserDotge = _userstat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(UserDotge);
                break;
            case Utill_Enum.Enum_AttackType.Range:
                //치명율
                critChance = enemystat.CriChance * 0.01f;
                isCrit = Utill_Math.CalculateProbability(critChance);
                //회피율
                UserDotge = _userstat.DodgeChance * 0.01f;
                isdotge = Utill_Math.CalculateProbability(UserDotge);
                break;
            case Utill_Enum.Enum_AttackType.Spell:
                break;
        }

        //회피율 먼저 계싼
        if (isdotge) //상대방이 먼저 회피를 성공했다면?
        {
            isDotge = true;
            Debbuger.Debug("=================Dotge================= ");
            return (-1, -1, isCri, isDotge, stuneffect , frozenstatuseffect , posionstatuseffect , slowstatuseffect);
        }



        //최후 대미지 할당 
        return (totalnocridamage, totalAdditionalDmg, isCri, isDotge, stuneffect, frozenstatuseffect, posionstatuseffect, slowstatuseffect);

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




    /// <summary>
    /// Enemy HP..............
    /// </summary>
    /// <param name="_userstat"></param>
    /// 
    // SaveMoveSpeed에서 speedpersent 퍼센트만큼 이동 속도를 감소시킵니다.
    public static float SetDeBuffMinusMoveSpeed(float SaveMoveSpeed, float speedpersent)
    {
        float reductionAmount = SaveMoveSpeed * (speedpersent / 100f);
        float newMoveSpeed = SaveMoveSpeed - reductionAmount;
        return newMoveSpeed;
    }

    // SaveMoveSpeed에서 speedpersent 퍼센트만큼 이동 속도를 증가시킵니다.
    public static float SetDeBuffPluseMoveSpeed(float SaveMoveSpeed, float speedpersent)
    {
        float increaseAmount = SaveMoveSpeed * (speedpersent / 100f);
        float newMoveSpeed = SaveMoveSpeed + increaseAmount;
        return newMoveSpeed;
    }

    public static void MiuseEnemyMoveSpeed(EnemyStat enemystat, float value)
    {
        enemystat.MoveSpeed -= value;
    }
    public static void MiuseEnemyMoveAttackSpeed(EnemyStat enemystat, float value)
    {
        enemystat.AttackSpeed -= value;
    }

    public static void PluseEnemyMoveSpeed(EnemyStat enemystat, float value)
    {
        enemystat.MoveSpeed -= value;
    }
    public static void PluseEnemyMoveAttackSpeed(EnemyStat enemystat, float value)
    {
        enemystat.AttackSpeed -= value;
    }

    public static void SetMaxHp(EnemyStat _userstat)
    {
        _userstat.CurrentHp = _userstat.HP;
    }

    public static void SetHp(EnemyStat _userstat, float value)
    {
        _userstat.HP = value;
        _userstat.CurrentHp = _userstat.HP;
    }

    public static void SetminusgeHp(EnemyStat _userstat, float value)
    {
        _userstat.CurrentHp -= value;
    }

    public static void SetPlusHp(EnemyStat _userstat, float value)
    {
        _userstat.CurrentHp += value;
    }

    public static void SetZeroHp(EnemyStat _userstat)
    {
        _userstat.CurrentHp = 0f;
    }

    public static float GetCurrentHp(EnemyStat _userstat)
    {
        float temp_hp = 0;

        temp_hp = _userstat.CurrentHp;

        return temp_hp;
    }

    public static float GetTotaiHp(EnemyStat _userstat)
    {
        float temp_hp = 0;

        temp_hp = _userstat.HP;

        return temp_hp;
    }

    #region 공격력 & 방어력 셋팅
    public static void SetPhysicalPower(EnemyStat _userstat, float value)
    {
        _userstat.PhysicalPower = value;
    }
    public static void SetPhysicalDefence(EnemyStat _userstat, float value)
    {
        _userstat.PhysicalPowerDefense = value;
    }
    #endregion

    #region 스킬로 인한 스탯 변동
    public static void UseSkillAddEnemyStat(EnemyStat enemyStat, int value, Utill_Enum.Upgrade_Type type)
    {
        switch (type)
        {
            case Upgrade_Type.PhysicalPower:
                enemyStat.PhysicalPower += value;
                break;
            case Upgrade_Type.MagicPower:
                enemyStat.MagicPower += value;
                break;
            case Upgrade_Type.PhysicalPowerDefense:
                enemyStat.PhysicalPowerDefense += value;
                break;
            case Upgrade_Type.HP:
                enemyStat.HP += value;
                break;
            case Upgrade_Type.MP:
                enemyStat.MP += value;
                break;
            case Upgrade_Type.CriChance:
                enemyStat.CriChance += value;
                break;
            case Upgrade_Type.CriDamage:
                enemyStat.CriDamage += value;
                break;
            case Upgrade_Type.AttackSpeedPercent:
                enemyStat.AttackSpeedPercent += value;
                break;
            case Upgrade_Type.MoveSpeedPercent:
                enemyStat.MoveSpeedPercent += value;
                break;
        }
    }

    public static void UseSkillMinusEnemyStat(EnemyStat enemyStat, int value, Utill_Enum.Upgrade_Type type)
    {
        switch (type)
        {
            case Upgrade_Type.PhysicalPower:
                enemyStat.PhysicalPower -= value;
                break;
            case Upgrade_Type.MagicPower:
                enemyStat.MagicPower -= value;
                break;
            case Upgrade_Type.PhysicalPowerDefense:
                enemyStat.PhysicalPowerDefense -= value;
                break;
            case Upgrade_Type.HP:
                enemyStat.HP -= value;
                break;
            case Upgrade_Type.MP:
                enemyStat.MP -= value;
                break;
            case Upgrade_Type.CriChance:
                enemyStat.CriChance -= value;
                break;
            case Upgrade_Type.CriDamage:
                enemyStat.CriDamage -= value;
                break;
            case Upgrade_Type.AttackSpeedPercent:
                enemyStat.AttackSpeedPercent -= value;
                break;
            case Upgrade_Type.MoveSpeedPercent:
                enemyStat.MoveSpeedPercent -= value;
                break;
        }
    }
    #endregion
}
