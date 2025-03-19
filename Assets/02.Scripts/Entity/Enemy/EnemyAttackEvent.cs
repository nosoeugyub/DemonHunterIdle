using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 몬스터의 애니메이션 이벤트를 제어
/// 공격 타이밍에 공격 함수 호출해줌
/// </summary>
public class EnemyAttackEvent : MonoBehaviour
{
    [SerializeField]
    Enemy enemy = null;

    /// <summary>
    /// attack animation clip에서 이벤트로 호출
    /// </summary>
    public void AttackAnimationEvent()
    {
        enemy.OnAttackAnimation();
    }
}
