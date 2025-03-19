using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Debbug;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스턴 스킬
/// </summary>
public class StunOnBasicAttack : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수 
    [SerializeField] private float stunpersnet;
    [SerializeField] private float Stunduration;
    [SerializeField] private float Stuntime;
    [SerializeField] private Utill_Enum.Debuff_Type tpye;

    #endregion
 
    private bool isStunBuffApplied = false;


    //스킬사용
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(UseStun());
        StartCoroutine(UseStun());
    }

    private IEnumerator UseStun()
    {
        if (!isStunBuffApplied  && hunter.CanGetDamage) //이변수로 플레이어 상태를 조율함 
        {
            isStunBuffApplied = true;
            //헌터의 스턴확률 증가
            HunterStat.Plus_StunPersent(hunter.Orginstat, stunpersnet, Stunduration);
            yield return null;
            //N초후 원래대로
        }
    }


    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public override void Init_Skill()
    {
        isStunBuffApplied = false;
        HunterStat.Miuse_StunPersent(hunter.Orginstat, stunpersnet, Stunduration);
    }

}
