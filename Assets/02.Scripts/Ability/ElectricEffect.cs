using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :  벼락치기
/// </summary>
public class ElectricEffect : StatusEffect
{
    private Coroutine stunCoroutine;
    private ISkill currentSKill;
    private Hunter hunter;
    private DamageInfo ElectricDamageInfo = null; //벼락치기 타격시 받을 데미지 정보
    ElectricData electricdata;


    //생성자
    public ElectricEffect(Utill_Enum.Debuff_Type statusType , ISkill currentskil , Hunter hunter)
    {
        StatusType = statusType;
        currentSKill = currentskil;
        this.hunter = hunter;

        electricdata = (ElectricData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.Electric, currentSKill._upgradeAmount);
    }

    // 상태 효과 적용 메서드
    public override void ApplyEffect(Entity target)
    {
        entity = target;
        if (stunCoroutine != null)
        {
            // 이미 스턴 중인 경우, 기존의 스턴 코루틴을 중지합니다.
            target.StopCoroutine(stunCoroutine);
        }

        // 스턴 코루틴을 시작합니다.
        stunCoroutine = target.StartCoroutine(StunCoroutine(target));



    }

    // 상태 효과 제거 메서드
    public override void RemoveEffect(Entity target)
    {
        if (stunCoroutine != null)
        {
            // 스턴 코루틴을 중지합니다.
            target.StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }

        // 기존의 스턴 상태를 해제합니다.

    }

    // 스턴 코루틴
    private IEnumerator StunCoroutine(Entity target)
    {
        // 스턴 상태를 설정합니다.

        //각타입에맞게 알맞은로직을 구성

        if (target is Hunter)
        {
            Hunter hunter = entity as Hunter;
        }
        else if (target is Enemy)
        {
            Enemy enemy = entity as Enemy;

            //대미지 연산 헌터의 해당 옵션값 가져오기
            float dmg = HunterStat.MagicPowerResult(DataManager.Instance.Hunters[1].Orginstat , true); //서버차트 로드);
            float resultdmg = dmg * (electricdata.CHGValue_DMG_MagicPower_Magic / 100);
            ElectricDamageInfo = new DamageInfo(resultdmg, electricdata.attackdamagetype[0] == Utill_Enum.AttackDamageType.Magic , hunter);
            HunterStat.CalculateSkillDamage(hunter._UserStat, enemy._EnemyStat, ElectricDamageInfo,electricdata);
            
            SoundManager.Instance.PlayAudio(currentSKill.SkillName);
            ObjectPooler.SpawnFromPool(Tag.ElectricEffectFx, enemy.EffectOffset + enemy.transform.position);
            enemy.Damaged(ElectricDamageInfo);
        }

        // 지정된 시간 동안 기다립니다.
        yield return new WaitForSeconds(Duration);

        // 스턴 상태를 해제합니다.

        // 스턴 코루틴을 null로 초기화합니다.
        stunCoroutine = null;
    }

    // 상태 효과 업데이트 메서드 (여기서는 사용하지 않음)
    public override void UpdateEffect(Entity target)
    {
        // 스턴 상태 효과는 시간이 지남에 따라 변화하는 것이 없으므로 사용하지 않습니다.
    }
}
