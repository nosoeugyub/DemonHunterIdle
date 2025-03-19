using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 도깨비불 스킬의 돌아가는 이펙트 객체
/// </summary>
public class DokkaebiFireEffect : MonoBehaviour
{
    //dmg
    DamageInfo damageInfo = null;
    LayerMask layerMask;
    Hunter hunter = null;
    DokkaebiFireData dokkaebiFireData = null;
    BoxCollider boxCollider = null;
    public void Init (Hunter hunter, ISkill skill, DokkaebiFireData dokkaebiFireData)
    {
        if(boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        this.hunter = hunter;
        layerMask = hunter.SearchLayer;
        damageInfo = new DamageInfo(0, skill.SkillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);
        this.dokkaebiFireData = dokkaebiFireData;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 해당 오브젝트가 Enemy 스크립트를 가지고 있는지 확인
        Enemy enemy;

        if (other.TryGetComponent(out enemy) && Utill_Standard.IsInLayerMask(other.gameObject,layerMask))
        {
            HunterStat.CalculateSkillDamage(hunter._UserStat, enemy._EnemyStat, damageInfo, dokkaebiFireData);
            enemy.Damaged(damageInfo);
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.DrawWireSphere(transform.TransformPoint(boxCollider.center), 0.1f);
        // 월드 공간에서의 변환 행렬을 설정 (위치, 회전, 스케일 적용)
        Gizmos.matrix = transform.localToWorldMatrix;

        // 콜라이더의 로컬 크기와 중심을 반영하여 WireCube 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

        // Gizmos 행렬 초기화
        // 다른 그리기 요소에 영향을 미치지 않도록
        Gizmos.matrix = Matrix4x4.identity;
    }
}
