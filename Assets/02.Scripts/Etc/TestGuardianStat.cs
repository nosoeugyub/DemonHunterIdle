using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 수호자 치트 스크립트
/// </summary>
public class TestGuardianStat : TestHunterStat
{
    [Space(20)]
    //수호자 특수스탯
    public float ElectricshockNumber; //감전개수 추가
    public float AddDamageChainLightning; //연쇄전기 추가대미지
    public float AddDamageHammerSummon; //망치 추가대미지
    public float AddDamageMagneticWaves; //자기장파 추가대미지
    public float AddDamageWhirlwindRush; //회전돌격 추가데미지
    public float AddDamageDokkaebiFire; //도깨비불 추가데미지
    public float AddDamageMeteor; //메테오 추가데미지
    public float AddDamageElectricshock; //감전 추가데미지
    public int BeastTransformationStackCountReduce; //야수화 필요스택 n 감소
    public int ContinuousHitStackCountReduce; //연타 필요스택 n 감소
    public float DokkaebiFireRotationSpeed; //도깨비불 회전 속도 증가 n

    public float MagneticWavesRangeNumber; //자기장파 범위 n 증가
    public float LastLeafAblityNumber; // 최후의 저항 발동시 공격력 방어력 퍼센트 증가
    public float ChainLightningNumber; // 연쇄전기 연쇄되는 횟수 증가

    public override HunterStat MakeUserStat()
    {
        HunterStat userstat = base.MakeUserStat();
        userstat.ElectricshockNumber = ElectricshockNumber;
        userstat.AddDamageChainLightning = AddDamageChainLightning;
        userstat.AddDamageHammerSummon = AddDamageHammerSummon;
        userstat.AddDamageMagneticWaves = AddDamageMagneticWaves;
        userstat.AddDamageWhirlwindRush = AddDamageWhirlwindRush;
        userstat.AddDamageDokkaebiFire = AddDamageDokkaebiFire;
        userstat.AddDamageMeteor = AddDamageMeteor;
        userstat.AddDamageElectricshock = AddDamageElectricshock;
        userstat.BeastTransformationStackCountReduce = BeastTransformationStackCountReduce;
        userstat.ContinuousHitStackCountReduce = ContinuousHitStackCountReduce;
        userstat.DokkaebiFireRotationSpeed = DokkaebiFireRotationSpeed;
        userstat.MagneticWavesRangeNumber = MagneticWavesRangeNumber;
        userstat.LastLeafAblityNumber = LastLeafAblityNumber;
        userstat.ChainLightningNumber = ChainLightningNumber;

        //userstat.CurrentHp = userstat.HP;

        return userstat;
    }
}
