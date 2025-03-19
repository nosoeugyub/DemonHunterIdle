using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Debbug;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 적처치지 hp회복스킬
/// </summary>
public class HPRegenOnEnemyKill : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수 
    [SerializeField] private float Hppersnet;
    [SerializeField] private float Hpduration;
    [SerializeField] private Utill_Enum.Debuff_Type tpye;

    #endregion

    private DamageInfo damage;
    private bool isStunBuffApplied = false;

    //스킬사용
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(UseHeal());
        StartCoroutine(UseHeal());
    }

    private IEnumerator UseHeal()
    {
        if (!isStunBuffApplied && hunter.CanGetDamage) //이변수로 플레이어 상태를 조율함 
        {
            isStunBuffApplied = true;
            //헌터의 hp회복 확률 증가
            HunterStat.Plus_HpPersent(hunter.Orginstat, Hppersnet);
            HunterStat.Plus_HpDuration(hunter.Orginstat, Hpduration);
            yield return null;
        }
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public override void Init_Skill()
    {
        isStunBuffApplied = false;
        HunterStat.Miuse_HpPersent(hunter.Orginstat, Hppersnet);
        HunterStat.Miuse_HpDuration(hunter.Orginstat, Hpduration);
    }

}
