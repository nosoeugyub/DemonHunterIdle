using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-23
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 연쇄전기 실제 구현
/// </summary>
public class ChainLightningEffect : StatusEffect
{
    private Coroutine stunCoroutine;
    private ISkill currentSkill;
    private Hunter hunter;
    private DamageInfo chainLightningDamageInfo = null; // 벼락치기 타격시 받을 데미지 정보
    private ChainLightning chainLightning;
  
    public GameObject LightningEffectPrefab;
    public float TickInterval = 0.05f;

    private System.Random random = new System.Random();

    ChainLighingData chainlighingdata;
    // 생성자
    public ChainLightningEffect(Utill_Enum.Debuff_Type statusType, ISkill currentSkill, Hunter hunter, ChainLightning chainLightning)
    {
        chainlighingdata = (ChainLighingData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.ChainLightning, currentSkill._upgradeAmount);
        StatusType = statusType;
        this.currentSkill = currentSkill;
        this.hunter = hunter;
        this.chainLightning = chainLightning;
    }

    // 상태 효과 적용
    public override void ApplyEffect(Entity target)
    {
        entity = target;
        if (stunCoroutine != null)
        {
            target.StopCoroutine(stunCoroutine);
        }

        stunCoroutine = target.StartCoroutine(StunCoroutine(target));
    }

    // 상태 효과 제거
    public override void RemoveEffect(Entity target)
    {
        if (stunCoroutine != null)
        {
            target.StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }
    }


    private IEnumerator StunCoroutine(Entity initialTarget)
    {
        List<Entity> affectedTargets = new List<Entity>(); // 이미 영향을 받은 대상을 추적하기 위한 리스트
        Entity currentTarget = initialTarget;

        // 첫 번째 대상을 대상으로 전기 효과를 적용합니다.
        if (currentTarget != null)
        {
            affectedTargets.Add(currentTarget);

            // 첫 번째 적에게 전기 효과 생성
            if (currentTarget is Enemy enemyTarget)
            {
                chainLightningDamageInfo = new DamageInfo(0, currentSkill.SkillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);
                HunterStat.CalculateSkillDamage(hunter._UserStat, (currentTarget as Enemy)._EnemyStat, chainLightningDamageInfo , chainlighingdata);

                // 효과와 사운드 적용
                SoundManager.Instance.PlayAudio(currentSkill.SkillName);

                if (enemyTarget.transform != null)
                {
                    // 첫 번째 적에 전기 효과 이펙트 생성
                    SkillObj particle = ObjectPooler.SpawnFromPool("ChainLightning", enemyTarget.transform.position).GetComponent<SkillObj>();
                    Vector3 spawnEffectPos = enemyTarget.transform.position;
                    Vector3 dir = (spawnEffectPos - enemyTarget.transform.position).normalized;
                    particle.SetLinePosition(enemyTarget.transform.position + dir * 0.1f, spawnEffectPos);
                }

                enemyTarget.Damaged(chainLightningDamageInfo);
            }

            // 지정된 시간 동안 기다립니다.
            yield return new WaitForSeconds(TickInterval);

            // 연쇄 효과 시작
            float chainLightningNumber = chainlighingdata.CHGValue_ChainLightningNumber + hunter._UserStat.ChainLightningNumber;
            for (int i = 0; i < chainLightningNumber     ; i++)
            {
                Entity nextTarget = FindNextTarget(currentTarget, affectedTargets);

                if (nextTarget == null) break; // 다음 대상이 없으면 종료

                affectedTargets.Add(nextTarget);

                // 다음 대상에게 전기 효과 생성
                if (nextTarget is Enemy enemyTargets)
                {
                    if (currentTarget is Enemy currentEnemy)
                    {
                        if (currentEnemy._EnemyStat != null)
                        {
                            chainLightningDamageInfo = new DamageInfo(0, currentSkill.SkillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);

                            //HunterStat.CalculateSkillDamage(hunter._UserStat, (currentTarget as Enemy)._EnemyStat, chainLightningDamageInfo);

                            // 효과와 사운드 적용
                            SoundManager.Instance.PlayAudio(currentSkill.SkillName);

                            // 다음 적에 전기 효과 이펙트 생성
                            SkillObj particle = ObjectPooler.SpawnFromPool("ChainLightning", enemyTargets.transform.position).GetComponent<SkillObj>();
                            if (particle != null)
                            {
                                Vector3 spawnEffectPos = enemyTargets.transform.position;
                                Vector3 dir = (enemyTargets.transform.position - currentTarget.transform.position).normalized;
                                particle.SetLinePosition(currentTarget.transform.position + dir * 0.1f, spawnEffectPos);
                            }
                        }
                    }
                    // 다음 대상에게 피해 적용
                    enemyTargets.Damaged(chainLightningDamageInfo);
                }

                // 지정된 시간 동안 기다립니다.
                yield return new WaitForSeconds(TickInterval);

                // 현재 대상을 다음 대상으로 업데이트
                currentTarget = nextTarget;
            }
        }

        stunCoroutine = null;
    }

    // 다음 대상을 찾는 헬퍼 메서드
    private Entity FindNextTarget(Entity currentTarget, List<Entity> affectedTargets)
    {
        Collider[] hitColliders = Physics.OverlapSphere(currentTarget.transform.position, chainLightning.ChainLightningRange);

        Entity nextTarget = null;
        float closestDistance = chainLightning.ChainLightningRange;

        foreach (var collider in hitColliders)
        {
            Entity potentialTarget = collider.GetComponent<Entity>();

            if (potentialTarget != null && potentialTarget != currentTarget && !affectedTargets.Contains(potentialTarget))
            {
                float distance = Vector3.Distance(currentTarget.transform.position, potentialTarget.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nextTarget = potentialTarget;
                }
            }
        }

        return nextTarget;
    }
    // 상태 효과 업데이트 메서드 (여기서는 사용하지 않음)
    public override void UpdateEffect(Entity target)
    {
        // 스턴 상태 효과는 시간이 지남에 따라 변화하는 것이 없으므로 사용하지 않습니다.
    }
}