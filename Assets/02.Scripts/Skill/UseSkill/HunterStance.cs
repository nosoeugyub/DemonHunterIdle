using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
public class HunterStance : BaseSkill
{
    public bool CurrentStance = true; //true = 공격 false : 방어
    public float DetectionRange; //감지범위
    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;
    private HunterStacneData hunterstancedata;

    GameObject particalobj;

    bool isactiveabiltyattackordef;
    //초기화
    public override void Init_Skill()
    {
        if (particalobj != null)
        {
            particalobj.SetActive(false);
            particalobj = null; //이펙트 제거
        }
        CurrentStance = true;
        if (isactiveabiltyattackordef) // 방어 태세 였다면
        {
            //회피 감소
            HunterStat.Minuse_DotgeChance(hunter.Orginstat, hunterstancedata.CHGValue_DodgeChance);
        }
        else //공격 태세였다면
        {
            //치명 감소
            HunterStat.Minuse_DotgeChance(hunter.Orginstat, hunterstancedata.CHGValue_CriChance);
        }
    }

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        //해당 데이터 파싱
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(CheckRangeEmeny());
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator CheckRangeEmeny()
    {
       int enemycount =  Utill_Standard.FindEnemiesInCone(hunter.transform.position, hunter.transform.forward ,  DetectionRange , 360).Count;
        if (enemycount > 0) //수비태세 
        {
            if (CurrentStance == true)
            {
                isactiveabiltyattackordef = true;
                CurrentStance = false;
                //치명 감소
                HunterStat.Minuse_DotgeChance(hunter.Orginstat, hunterstancedata.CHGValue_CriChance);
                //회피력 n%증가
                HunterStat.Plus_DotgeChance(hunter.Orginstat, hunterstancedata.CHGValue_DodgeChance);
                //이펙트 소환
                particalobj = ObjectPooler.SpawnFromPool(Tag.RageShot, hunter.transform.position);
            }
           
        }
        else
        {
            //공격태세
            if (CurrentStance == true)
            {
                isactiveabiltyattackordef = false;
                CurrentStance = false;
                //회피 감소
                HunterStat.Minuse_DotgeChance(hunter.Orginstat, hunterstancedata.CHGValue_DodgeChance);
                //치명확률 n%증가
                HunterStat.Plus_CriChance(hunter.Orginstat, hunterstancedata.CHGValue_CriChance);
                //이펙트 소환
                particalobj = ObjectPooler.SpawnFromPool(Tag.CriticalShot, hunter.transform.position);
            }
        }
        particalobj.transform.position = hunter.transform.position;
        // x 회전값을 90도로 설정
        particalobj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        particalobj.transform.SetParent(hunter.transform);

        yield return null;
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        hunterstancedata = (HunterStacneData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.HunterStance, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, hunterstancedata.CHGValue_UseMP); //마나감소
        if (tempmana == 0)
        {
            IsUseSkill = false;
            return false; //마나 사용실패
        }
        else
        {
            //ui까지 변화 적용
            IsUseSkill = true;
            DataManager.Instance.Hunters[index].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[index]._UserStat.MP , NSY.DataManager.Instance.Hunters[index]._UserStat.CurrentMp);
            return true;
        }
    }
}
