using UnityEngine;

/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 데미지 주는 인터페이스
/// </summary>
public interface IDamageAble
{
    void Damaged(DamageInfo info);
    Transform ObjectTransform { get; }
    bool CanGetDamage { get; }
}
/// <summary>
/// enemy 접근하기 위해 추가
/// </summary>
public interface IEnemyDamageAble : IDamageAble
{
    Enemy DamageAbleEnemy { get; }
}
/// <summary>
/// hunter 접근하기 위해 추가
/// </summary>
public interface IHunterDamageAble : IDamageAble
{
    Hunter DamageAbleHunter { get; }
}