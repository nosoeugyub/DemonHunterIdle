using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 궁수 각성1 스킬
/// </summary>
public class RageShot : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("공격 속도 퍼센트 증가수치")]
    [SerializeField] int attackSpeedPercent = 50;
    #endregion

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;


    RageShotData CurrentRangeShotData;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        //해당 데이터 파싱
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(RageShotBuff());
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    IEnumerator RageShotBuff()
    {
        SoundManager.Instance.PlayAudio(skillName);
        GameObject rapidFireFx = ObjectPooler.SpawnFromPool(Tag.RageShot, hunter.transform.position);
        rapidFireFx.transform.SetParent(hunter.transform);
        rapidFireFx.transform.localPosition = Vector3.zero;
        // x 회전값을 90도로 설정
        rapidFireFx.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        ApplyBuff();

        yield return new WaitForSeconds(CurrentRangeShotData.CHGValue_SkillDuration);
        rapidFireFx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
        rapidFireFx.gameObject.SetActive(false);
        RemoveBuff();
        SoundManager.Instance.PlayAudio(Tag.RageShot_Off);
        currentCoroutine = null;
    }
    private void ApplyBuff()
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_AttackSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }

    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_AttackSpeedPercent, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        CurrentRangeShotData = (RageShotData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.RageShot, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, CurrentRangeShotData.CHGValue_UseMP); //마나감소
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
