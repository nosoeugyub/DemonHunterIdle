using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 화살비 이펙트 (점점 커지면서 데미지처리)
/// </summary>
public class ArrowRainEffect : MonoBehaviour
{
    private Hunter hunter;
    private DamageInfo damageInfo;
    private float initialRadius = 1f;  // 초기 반지름
    private float maxRadius = 5f;      // 최대 반지름
    private float expansionSpeed = 2f;  // 반지름이 커지는 데 걸리는 시간

    private float currentRadius;
    private float elapsedTime = 0f;

    private bool isExpansion = false;

    private List<Collider> damagedCol = new();
    private ArrowRainData arrowRainData;
    [SerializeField]
    private float deleteTime = 3f;
    void Start()
    {
        // 초기 반지름 설정
        currentRadius = initialRadius;
    }

    void FixedUpdate()
    {
        if (!isExpansion) return;

        // 경과 시간 계산
        elapsedTime += Time.deltaTime;

        // 반지름을 점진적으로 증가
        float t = Mathf.Clamp01(elapsedTime / expansionSpeed);  // 0에서 1로 점차 증가
        currentRadius = Mathf.Lerp(initialRadius, maxRadius, t);  // 선형 보간을 이용한 반지름 증가

        // 반지름이 최대에 도달했을 때 동작 멈춤
        if (elapsedTime >= expansionSpeed)
        {
            currentRadius = maxRadius;
        }

        // 범위 내의 대상 체크
        ApplyDamage();
    }
    void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }
    private void ApplyDamage()
    {
        // 데미지 반경 내의 모든 적 탐지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentRadius, LayerMask.GetMask(Tag.Enemy));
        foreach (var hitCollider in hitColliders)
        {
            IDamageAble target = hitCollider.GetComponent<IDamageAble>();
            if (target != null && target.CanGetDamage && !damagedCol.Contains(hitCollider))
            {
                // 확률적으로 데미지 적용
                if (Utill_Math.Attempt(SkillManager.Instance.arrowRain.GetArrowRainHitChance))
                {
                    HunterStat.CalculateSkillDamage(hunter._UserStat, (target as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, damageInfo, arrowRainData);
                    target.Damaged(damageInfo);  // 데미지 적용
                    damagedCol.Add(hitCollider);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(isExpansion)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, currentRadius);
        }
    }

    private IEnumerator DeleteDaly(float time)
    {
        yield return new WaitForSeconds(time);
        damagedCol = new();
        isExpansion = false;
        gameObject.SetActive(false);
        currentRadius = 0;
        elapsedTime = 0;
    }

    public void SetEffect(float initRadius,float finalRadius,float speed,Hunter hunter,DamageInfo damageInfo,ArrowRainData arrowRainData)
    {
        initialRadius= initRadius;
        maxRadius = finalRadius;
        expansionSpeed = speed;
        this.hunter = hunter;
        this.damageInfo = damageInfo;
        isExpansion = true;
        currentRadius = initRadius;
        this.arrowRainData = arrowRainData;
        StartCoroutine(DeleteDaly(deleteTime));
    }

}