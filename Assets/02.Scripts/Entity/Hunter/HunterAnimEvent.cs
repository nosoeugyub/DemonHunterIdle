using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터의 애니메이션 이벤트를 제어
/// </summary>
public class HunterAnimEvent : MonoBehaviour
{
    [SerializeField]
    Hunter hunter = null;
    
    /// <summary>
    /// attack animation clip에서 이벤트로 호출
    /// </summary>
    public void AttackAnimationEvent()
    {
        hunter.OnAttackAnimation();
    }
    /// <summary>
    /// die animation clip에서 이벤트로 호출
    /// </summary>
    public void DieAnimationEvent()
    {
        hunter.OnDieAnimation();
    }
    /// <summary>
    /// StrongShot animation clip에서 이벤트로 호출
    /// </summary>
    public void StrongShotkAnimationEvent()
    {
        StartCoroutine(SkillManager.Instance.strongShot.SpawnStrongShotProjectile());
    }

    /// <summary>
    /// StrongShot animation clip에서 이벤트로 호출
    /// </summary>
    public void ArrowAnimationEvent()
    {
        SkillManager.Instance.arrowRain.AnimEnd();
    }

    /// <summary>
    /// Hammer animation clip에서 이벤트로 호출
    /// </summary>
    public void HammerSummonSpawnAnimationEvent() //망치교체
    {
        SkillManager.Instance.hammerSummon.EquipWeapon();
    }/// 

    public void HammerSummonAnimationEvent() //망치 대미지 + 이펙트
    {
        StartCoroutine(SkillManager.Instance.hammerSummon.UseHammerSummon());
    }

    public void UnHammerSummonAnimationEvent()
    {
        StopCoroutine(SkillManager.Instance.hammerSummon.UseHammerSummon());
        SkillManager.Instance.hammerSummon.UnEquipWeapon();
    }
}
