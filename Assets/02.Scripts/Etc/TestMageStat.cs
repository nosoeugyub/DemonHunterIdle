using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 법사 치트 스크립트
/// </summary>
public class TestMageStat : TestHunterStat
{
    //법사 특수스탯

    public override HunterStat MakeUserStat()
    {
        HunterStat userstat = base.MakeUserStat();
        //userstat.CurrentHp = userstat.HP;

        return userstat;
    }
}