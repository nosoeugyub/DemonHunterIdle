using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 일직선으로 이동하는 투사체 (타겟 유무 결정 가능)
/// </summary>
public class LinearProjectile : Projectile
{
    [SerializeField]
    private BoxCollider castCollider;
    public override Vector3 moveVector => dir;
    [SerializeField]
    private Vector3 dir = Vector3.up; //이동 방향 백터(투사체 생김새에 따라 달라짐)
    [Header("타격 후 삭제할것인지")]
    [SerializeField]
    private bool isDeleteAfterDamage = true;

    private Vector3 halfExtents = Vector3.zero;
    private Vector3 center = Vector3.zero;

    private List<IDamageAble> damagedList = new List<IDamageAble>(); //이미 데미지를 준 타겟의 리스트

    IDamageAble firstTarget = null;

    protected override void FixedUpdateLoop()
    {
        FindTarget(); //타겟 감지
        if (!isHit)  //충돌하지 않았다면 
            Move();  //이동
        else         //충돌했다면
        {
            damagedList.AddRange(targetList); //타격한 적들 저장
            bool flag = DamageToTarget(targetList); //데미지 주고
            isHit = false;

            if (isDeleteAfterDamage && flag)
                gameObject.SetActive(false);//삭제
            else
            {
                targetList.Clear(); //타겟 리스트도 삭제
                Move(); //이번 프레임에 움직여야하는 양만큼 움직여줌
            }
        }
    }
    protected override void Move()
    {
        transform.Translate(moveVector * moveSpeed * Time.fixedDeltaTime);
    }

    protected override void FindTarget()
    {
        center = transform.TransformPoint(castCollider.center);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation, searchLayer);

        if (hits.Length <= 0) return;

        // 유효한 타겟을 필터링하여 추가
        foreach (var hit in hits)
        {
            var damageable = hit.transform.GetComponent<IDamageAble>();
            if (damageable != null && damageable.CanGetDamage && !damagedList.Contains(damageable))
            {
                targetList.Add(damageable);
            }
        }

        // 추가적으로 반경 내의 모든 객체 감지(다중 타겟 로직)
        var additionalTargets = Utill_Standard.FindAllObjectsInRadius<IDamageAble>(
            transform.position + (transform.forward * applicationDistance),
            damageRadius, searchLayer, maxTarget);

        foreach (var target in additionalTargets)
        {
            if (target != null && target.CanGetDamage && !targetList.Contains(target) && !damagedList.Contains(target))
            {
                targetList.Add(target);
            }
        }
        if (firstTarget != null)
            targetList.Add(firstTarget); //첫 타겟은 무조건 hit

        // 중복을 제거
        targetList = targetList.Distinct().ToList();

        // 최대 타겟 수를 초과하면 삭제
        if (maxTarget >= 0 && targetList.Count > maxTarget)
        {
            targetList = targetList.Take(maxTarget).ToList();
        }

        // 타겟이 존재하는지 확인
        isHit = targetList.Count > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(castCollider.center), 0.1f);
        // 월드 공간에서의 변환 행렬을 설정 (위치, 회전, 스케일 적용)
        Gizmos.matrix = transform.localToWorldMatrix;

        // 콜라이더의 로컬 크기와 중심을 반영하여 WireCube 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(castCollider.center, castCollider.size);

        // Gizmos 행렬 초기화
        // 다른 그리기 요소에 영향을 미치지 않도록
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * applicationDistance), damageRadius);
    }

    protected override void Setting()
    {
        if (targetList.Count > 0)
            firstTarget = targetList?[0];
        targetList.Clear();
        damagedList.Clear();

        //자신의 크기와 콜라이더 크기를 계산하여 실제 탐색해야할 범위를 미리 정의
        //스킬의 경우 생성 후 투사체의 크기를 변경해주는 경우가 있기 때문에 Setting시마다 정의함
        halfExtents = new Vector3(
        transform.localScale.x * castCollider.size.x * 0.5f,
        transform.localScale.y * castCollider.size.y * 0.5f,
        transform.localScale.z * castCollider.size.z * 0.5f);

        if (targetPos == Vector3.zero)
        {
            if (dir == Vector3.forward)
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
            else if (dir == Vector3.up)
                transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
            return;
        }

        // 타겟 방향 벡터 계산
        Vector3 directionToTarget = (targetPos - transform.position).normalized;

        // LookRotation을 사용해 타겟을 바라보는 회전값 생성
        Quaternion rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        //rotation = Quaternion.Euler(0, rotation.eulerAngles.y, rotation.eulerAngles.z);

        // 계산한 회전을 적용
        // 두 회전 값 사이의 각도 차이를 계산
        float angleDifference = Quaternion.Angle(Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z), rotation);

        // 각도 차이가 충분히 큰 경우에만 회전 추가
        if (angleDifference > 0.01f) // 너무 작은 각도는 무시
        {
            Vector3 currentEulerAngles = transform.eulerAngles;
            Quaternion finalRotation = rotation * Quaternion.Euler(0, currentEulerAngles.y, currentEulerAngles.z);

            // 계산한 최종 회전을 적용
            transform.rotation = finalRotation;
        }

        // 움직일 방향에 따라 투사체 회전을 결정
        if (dir == Vector3.forward)
        {
        }
        else if (dir == Vector3.up)
        {
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public void SetDir(Vector3 dir)
    {
        this.dir = dir;
    }
}
