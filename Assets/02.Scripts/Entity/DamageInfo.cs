using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 주는 데미지에 대한 정보
/// 추후 공격 타입, 치명타 등 추가 위함.
/// </summary>
public class DamageInfo
{
    public float damage; // 최종 데미지
    public float addDamage; //추가 데미지

    //일격과 크리티컬 데미지가 동시에 뜨는 경우도 있고, 서로 분리하여 표기해야하기 때문에 일격은 데미지를 따로 저장함
    public float CoupDamage; // 일격 최종 데미지
    public float CoupAddDamage; // 일격 추가 데미지

    public float TrueDamageTpye; // 관통타입
    public float MagaicDamageType; //물리타입
    public float PhycisDamageType; //마법타입

    public bool isCri = false; //크리티컬 데미지
    public bool isCoup = false; //일격 데미지
    public bool isMagic;
    public bool isNormalAttack = false; //평타인지

    //스킬 관련
    public BaseSkillData skill = null; //데미지를 주는 스킬의 인터페이스(SKillName으로 스킬 자체에도 접근 가능)


    public bool ReflectRange = false;//반사
    public bool isInstanceKillBoss = false;// 보스즉사
    public bool isdodge = false; 
    public StunStatusEffect isStun = null;
    public FrozenStatusEffect isFreezing = null;
    public PosionStatusEffect isPosion = null;
    public SlowStatusEffect isSlow = null;

    public ElectricEffect isElectric = null;
    public ChainLightningEffect isChain = null;

    public float stunDuration = 0f;
    public float freezingDuration = 0f;

    public IDamageAble sender = null;

    public DamageInfo(float damage, bool isMagic, IDamageAble sender)
    {
        this.damage = damage;
        this.isMagic = isMagic;
        this.sender = sender;
    }
}