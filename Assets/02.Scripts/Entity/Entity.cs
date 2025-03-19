using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적과 헌터의 베이스 스크립트
/// 필요한 변수/형식만 공유되도록 (많은 책임 x)
/// </summary>
public class Entity : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected Rigidbody rigid;
    [SerializeField]
    protected CapsuleCollider col = null;

    [Header("사운드")]
    [SerializeField]
    protected AudioClip AttackFx;
    [SerializeField]
    protected AudioClip DamagedFx;
    [SerializeField]
    protected AudioClip DeathFx;
    [SerializeField]
    protected AudioClip WalkFx;

    [Header("이동 속도 계수")]
    [SerializeField]
    protected float moveSpeedNum = 2.5f;
    [Header("공격 속도 계수")]
    [SerializeField]
    protected float attackSpeedNum = 1f;
    [Header("대기, 죽음 애니메이션 계수")]
    [SerializeField]
    protected float etcAnimSpeed = 1f;
    [Header("이펙트 오프셋")]
    [SerializeField]
    protected Vector3 effectOffset;

    public Vector3 EffectOffset => effectOffset;

    //투사체 피격 횟수 체크 딕셔너리
    protected Dictionary<int,int> projectileHitDic = new Dictionary<int,int>();
    public Dictionary<int,int> ProjectileHitDic { get { return projectileHitDic; } }


    //각 캐릭터더별로 관리될 디버프 
    [SerializeField] private List<StatusEffect> statusEffectList;
    public List<StatusEffect> StatusEffectList
    {
        get { return statusEffectList; }
        set { statusEffectList = value; }
    }

    public virtual void ApplyStatusEffect(StatusEffect effect)
    {
        // 상태 효과를 적용하는 기본 메서드
        // 하위 클래스에서 재정의할 수 있음
    }

    public virtual void RemoveStatusEffect(StatusEffect type)
    {
        // 특정 타입의 상태 효과를 제거하는 기본 메서드
        // 하위 클래스에서 재정의할 수 있음
    }
}
