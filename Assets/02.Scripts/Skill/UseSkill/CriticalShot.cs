using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 궁수 각성2 스킬
/// </summary>
public class CriticalShot : BaseSkill
{
   
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("치명타 확률 증가 수치")]
    [SerializeField] int criChance = 50;
    [Header("치명타 데미지 증가 수치")]
    [SerializeField] int criDamage = 50;
    #endregion

    CriticalShotData CurrentCriticalShotData;

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;
 
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(CriticalShotBuff());
    }

    IEnumerator CriticalShotBuff()
    {
        SoundManager.Instance.PlayAudio(skillName);
        GameObject rapidFireFx = ObjectPooler.SpawnFromPool(Tag.CriticalShot, hunter.transform.position);
        rapidFireFx.transform.SetParent(hunter.transform);
        rapidFireFx.transform.localPosition = Vector3.zero;
        // x 회전값을 90도로 설정
        rapidFireFx.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        ApplyBuff();

        yield return new WaitForSeconds(CurrentCriticalShotData.CHGValue_SkillDuration);
        rapidFireFx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
        rapidFireFx.gameObject.SetActive(false);
        RemoveBuff();
        SoundManager.Instance.PlayAudio(Tag.RageShot_Off);
        currentCoroutine = null;
    }
    private void ApplyBuff()
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentCriticalShotData.CHGValue_CriChance, Utill_Enum.Upgrade_Type.CriChance);
        HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentCriticalShotData.CHGValue_CriDamage, Utill_Enum.Upgrade_Type.CriDamage);
    }

    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentCriticalShotData.CHGValue_CriChance, Utill_Enum.Upgrade_Type.CriChance);
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentCriticalShotData.CHGValue_CriDamage, Utill_Enum.Upgrade_Type.CriDamage);
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        CurrentCriticalShotData = (CriticalShotData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.CriticalShot, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, CurrentCriticalShotData.CHGValue_UseMP); //마나감소
        if (tempmana == 0)
        {
            return false; //마나 사용실패
        }
        else
        {
            //ui까지 변화 적용
            DataManager.Instance.Hunters[index].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[index]._UserStat.MP, DataManager.Instance.Hunters[index]._UserStat.CurrentMp);
            return true;
        }
    }
}
