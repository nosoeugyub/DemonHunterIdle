using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 회피 스킬
/// </summary>
public class DodgeBoost : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region
    [Header("회피 확률 증가 수치")]
    [SerializeField] int dodgeChance = 50;
    #endregion

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DodgeChanceBuff());
    }

    IEnumerator DodgeChanceBuff()
    {
        SoundManager.Instance.PlayAudio(skillName);
        GameObject rapidFireFx = ObjectPooler.SpawnFromPool(Tag.DodgeBoostFx, hunter.transform.position);
        rapidFireFx.transform.SetParent(hunter.transform);
        rapidFireFx.transform.localPosition = Vector3.zero;
        ApplyBuff();

        yield return new WaitForSeconds(SkillDuration);
        rapidFireFx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
        rapidFireFx.gameObject.SetActive(false);
        RemoveBuff();
        currentCoroutine = null;
    }
    private void ApplyBuff()
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, dodgeChance, Utill_Enum.Upgrade_Type.DodgeChance);
    }

    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, dodgeChance, Utill_Enum.Upgrade_Type.DodgeChance);
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }
}
