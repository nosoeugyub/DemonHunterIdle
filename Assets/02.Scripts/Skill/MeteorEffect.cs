using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 메테오이펙트 관련
/// /// </summary>
public class MeteorEffect : MonoBehaviour
{
    #region Meteor용
    public ParticleSystem MeteorParticle;


    private void OnDrawGizmosSelected()
    {
        // 편집기에서 폭발 범위를 시각적으로 보여줌
        Gizmos.color = Color.black; // 기즈모 색상을 노란색으로 설정
        Gizmos.DrawWireSphere(transform.position, 1);
    }

    public void OnHit(Hunter hunter, Vector3 hitPos, Utill_Enum.AttackDamageType SkillDamageType, DamageInfo damage, float DamageRadius, float delay)
    {
        List<Enemy> targetEntities = GetTargetEntities(hitPos, DamageRadius);
        StopCoroutine(DelayedDamage(hunter, targetEntities, SkillDamageType, damage, delay));
        StartCoroutine(DelayedDamage(hunter, targetEntities, SkillDamageType,damage, delay));
    }

    private IEnumerator DelayedDamage(Hunter hunter, List<Enemy> targetEntities, Utill_Enum.AttackDamageType SkillDamageType, DamageInfo skillDamageInfo, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlayAudio("SecondMeteor");
        int length = targetEntities.Count;

        for (int i = 0; i < length; i++)
        {
            Enemy targetEntity = targetEntities[i];
            //HunterStat.CalculateSkillDamage(hunter._UserStat, targetEntity._EnemyStat, skillDamageInfo);
            targetEntity.Damaged(skillDamageInfo);
        }
        
    }

    //스킬사용 기준자로부터 범위크기만큼 적을 찾아내는로직
    private List<Enemy> GetTargetEntities(Vector3 startPos, float range)
    {
        LayerMask mask = LayerMask.NameToLayer("Enemy");
        int layermaskint = (1 << mask);
        Collider[] cols = Physics.OverlapSphere(startPos, range, layermaskint);
        List<Enemy> entities = new List<Enemy>();
        for (int i = 0; i < cols.Length; i++)
        {
            Enemy targetEntity = cols[i].gameObject.GetComponent<Enemy>();

            if (targetEntity != null)
                entities.Add(targetEntity);
        }

        return entities;
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }
    #endregion
}
