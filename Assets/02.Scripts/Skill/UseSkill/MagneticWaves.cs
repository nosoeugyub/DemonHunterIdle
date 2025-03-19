using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 자기장파 스킬
/// </summary>
public class MagneticWaves : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수 

    [Header("자기장파 데미지 적용확률")]
    [SerializeField] private float MagneticWavesHitChance;

    [Header("자기장파 피해범위")]
    [SerializeField] private float DamageRadius;

    [Header("지속데미지 간격")]
    [SerializeField] private float TickInterval;

    [Header("시전 후 데미지 적용 딜레이")]
    [SerializeField] private float ImpactDelay;

    [Header("이펙트 사이즈")]
    [SerializeField] float EffectSize;

    #endregion


    private Coroutine currentCoroutine;

    private GameObject fx; //이펙트 담는 변수
    private DamageInfo magneticWavesDamageInfo = null; //데미지 정보

    private float elapsedTime;
    private float nextTickTime;
    private bool isApplyingDamage = false;
    private Vector3 damagePosition;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(UseMagneticWaves());
    }

    private IEnumerator UseMagneticWaves()
    {
        if (hunter.CanGetDamage) // 이 변수로 플레이어 상태를 조율함
        {
            Vector3 skillPosition = hunter.transform.position;

            EffectSound(skillPosition);

            magneticWavesDamageInfo = new DamageInfo(0, skillDamageType == Utill_Enum.AttackDamageType.Physical, hunter);

            ShowDamageRadius(skillPosition); //피해 범위 표시

            yield return new WaitForSeconds(ImpactDelay); // 기다렸다 데미지 적용

            StartApplyingDamage(skillPosition); 
        }
    }
    void EffectSound(Vector3 skillPosition)
    {
        fx = ObjectPooler.SpawnFromPool(Tag.MagneticWaves, Vector3.zero);
        fx.transform.position = skillPosition;
        float effectSize = EffectSize + hunter._UserStat.MagneticWavesRangeNumber;
        fx.transform.localScale = new Vector3(effectSize, effectSize, effectSize);

        SoundManager.Instance.PlayAudio(skillName);
    }

    private void StartApplyingDamage(Vector3 impactPosition)
    {
        damagePosition = impactPosition;            // 데미지를 적용할 위치 설정
        elapsedTime = 0f;                           // 경과 시간 초기화
        nextTickTime = Time.time + TickInterval;    // 다음 데미지 틱 발생할 시간 설정
        isApplyingDamage = true;                    // 데미지 적용 상태 시작
    }
    private void ApplyDamage(Vector3 impactPosition)
    {
        // 데미지 반경 내의 모든 적 탐지
        Collider[] hitColliders = Physics.OverlapSphere(impactPosition, DamageRadius + hunter._UserStat.MagneticWavesRangeNumber, hunter.SearchLayer);
        foreach (var hitCollider in hitColliders)
        {
            IDamageAble target = hitCollider.GetComponent<IDamageAble>();
            if (target != null && target.CanGetDamage)
            {
                // 확률적으로 데미지 적용
                if (UnityEngine.Random.Range(0f, 100f) <= MagneticWavesHitChance)
                {
                    //HunterStat.CalculateSkillDamage(hunter._UserStat, (target as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, magneticWavesDamageInfo);
                    target.Damaged(magneticWavesDamageInfo); // 데미지 적용
                }
            }
        }
    }
    private void Update()
    {
        if (isApplyingDamage)
        {
            elapsedTime += Time.deltaTime;  // 경과 시간 증가

            // 스킬 지속시간이 끝나면 데미지 적용 종료
            if (elapsedTime >= SkillDuration)
            {
                isApplyingDamage = false;
                if (fx != null)
                {
                    fx.SetActive(false); // 이펙트를 비활성화
                }
                return;
            }

            // 현재 시간이 다음 데미지 틱 시점에 도달하면 데미지 적용
            if (Time.time >= nextTickTime)
            {
                ApplyDamage(damagePosition);
                nextTickTime = Time.time + TickInterval; // 다음 틱 시점 설정
            }
        }
    }
    private void ShowDamageRadius(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3((hunter._UserStat.MagneticWavesRangeNumber +DamageRadius) * 2, 0.1f, (hunter._UserStat.MagneticWavesRangeNumber + DamageRadius) * 2); // Y축을 줄여 평평하게 만듭니다.
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f); // 반투명 빨간색으로 설정

        Destroy(sphere, ImpactDelay); // 일정 시간 후 삭제
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
}
