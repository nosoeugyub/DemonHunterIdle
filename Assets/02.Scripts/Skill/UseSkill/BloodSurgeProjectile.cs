using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-10-16
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 분열되는 투사체
/// 혈파 스킬이지만 생성을 헌터 state에서 해주기 때문에 이 코드를 스킬에 넣을 수 없었음
/// </summary>
public class BloodSurgeProjectile : LinearProjectile
{
    protected override void Setting()
    {
        base.Setting();
        OnAttackAction -= SkillManager.Instance.bloodSurge.SpawnBloodSurgeEffect;
        OnAttackAction += SkillManager.Instance.bloodSurge.SpawnBloodSurgeEffect;
    }
}
