using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 궁수 치트 스크립트 
/// </summary>
public class TestArcherStat : TestHunterStat
{
    [Space(20)]
    //궁수 특수스탯
    public int ArrowCount; //궁수 화살개수
    public float AddDamageArrowRain; //화살비 추가대미지
    public float AddDamageRapidShot; //래피드샷 추가대미지
    public float AddDamageStrongShot; //정조준일격 추가대미지
    public float AddDamageDemonicBlow; //악귀의일격 추가데미지
    public float AddDamageSpikeTrap; //가시덫 추가데미지
    public float AddDamageVenomousSting; //맹독침 추가데미지
    public float AddDamageElectric; //벼락치기 추가데미지

    public int ElectricCastingChance; //벼락치기 발동 확률 증가 n
    public int SplitPiercingNumber; //분열관통 분열 수 증가 n
    public int SpikeTrapNumber; //가시덫 투사체 발사 개수 증가 n
    public float SpikeTrapDamageRadius; //가시덫 장판 범위 증가 n
    public int VenomousStingNumber; //맹독침 투사체 발사 개수 증가 n
    public float VenomousStingDamageRadius; //맹독침 폭발 범위 증가 n

    public override HunterStat MakeUserStat()
    {
        HunterStat userstat = base.MakeUserStat();
        userstat.ArrowCount = ArrowCount;
        userstat.AddDamageArrowRain = AddDamageArrowRain;
        userstat.AddDamageRapidShot = AddDamageRapidShot;
        userstat.AddDamageStrongShot = AddDamageStrongShot;
        userstat.AddDamageDemonicBlow = AddDamageDemonicBlow;
        userstat.AddDamageSpikeTrap = AddDamageSpikeTrap;
        userstat.AddDamageVenomousSting = AddDamageVenomousSting;
        userstat.AddDamageElectric = AddDamageElectric;

        userstat.ElectricCastingChance = ElectricCastingChance;
        userstat.SplitPiercingNumber = SplitPiercingNumber;
        userstat.SpikeTrapNumber = SpikeTrapNumber;
        userstat.SpikeTrapDamageRadius = SpikeTrapDamageRadius;
        userstat.VenomousStingNumber = VenomousStingNumber;
        userstat.VenomousStingDamageRadius = VenomousStingDamageRadius;

        //userstat.CurrentHp = userstat.HP;

        return userstat;
    }
}

