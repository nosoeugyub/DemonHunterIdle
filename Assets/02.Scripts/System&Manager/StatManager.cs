using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NormalAttackStats
{
    public Utill_Enum.SubClass subClass;
    public int MaxTarget;
    public float DamageRadius;
    public float ApplicationDistance; //평타시 범위 판정 적용 거리
}

/// <summary>
/// 작성일자   : 2024-07-01
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : csv안 정보 외의 추가적인 스탯 관련 변수들을 조절할 수 있는 매니저
/// </summary>
public class StatManager : MonoSingleton<StatManager>
{
    [Header("임시 값 (추후 대체될 예정)\n헌터 평타시 범위공격 스탯")]
    [SerializeField]
    private List<NormalAttackStats> hunterStats;
    [Space(20)]
    [Header("기본공격시 체력흡수 확률")]
    [SerializeField]
    private float hpDrainChance = 10f;
    [Header("기본공격시 마나흡수 확률")]
    [SerializeField]
    private float mpDrainChance = 10f;
    [Header("체력 흡수량 최대치")]
    [SerializeField]
    private float hpDrainMaximum = 500f;
    [Header("마나 흡수량 최대치")]
    [SerializeField]
    private float mpDrainMaximum = 250f;

    public float HpDrainChance => hpDrainChance;
    public float HpDrainMaximum => hpDrainMaximum;

    public float MpDrainChance => mpDrainChance;
    public float MpDrainMaximum => mpDrainMaximum;

    public NormalAttackStats GetHunterNormalAttackStat(Utill_Enum.SubClass subClass)
    {
        return hunterStats.Find(x => x.subClass == subClass);
    }
}