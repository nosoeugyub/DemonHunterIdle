using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Debbug;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 메테오 스킬
/// </summary>
public class Meteor : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수 
    [Header("메테오 발동 횟수")]
    [SerializeField] float[] MeteorNumber;


    [Header("메테오 피해범위")]
    [SerializeField] float DamageRadius;

    [Header("( (0일 경우 스턴 없음  /   스턴관련 로직은 차후 적용)")]
    [SerializeField] float stunDuration;

    [Header("시전 후 데미지 적용 딜레이")]
    [SerializeField] private float ImpactDelay;

    [Header("이펙트사이즈")]
    [SerializeField] private float EffectSize;

    #endregion
    
    private DamageInfo damage;

    //스킬사용
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(UseMetreor());
        StartCoroutine(UseMetreor());
    }

    private IEnumerator UseMetreor()
    {
        if (hunter.CanGetDamage) //이변수로 플레이어 상태를 조율함 
        {
            //이펙트 설정 후 시작
            int count = (int)MeteorNumber[0];
            for (int i = 0; i < count; i++)
            {
                //타입정하기
                var skillposlist = SpawnSkillArea(SkillCastingPositionType);
                Vector3 SpawnEffectPos = skillposlist.pos;

                //이펙트 소환해서 떨어트리기
                MeteorEffect particle = ObjectPooler.SpawnFromPool(skillName, SpawnEffectPos).GetComponent<MeteorEffect>();
                //메테오 크기
                particle.transform.localScale = Utill_Standard.vector3One * CalculateMeteorSize(DamageRadius);
                //메테오 범위를

                damage = new(0, skillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);


                particle.MeteorParticle.Play();
                particle.OnHit(hunter, particle.transform.position, SkillDamageType, damage, DamageRadius, ImpactDelay);

                yield return new WaitForSeconds(ImpactDelay);

                //첫번째 사운드 들어가고
                SoundManager.Instance.PlayAudio("FirstMeteor");
                //초후에
                yield return new WaitForSeconds(MeteorNumber[1]);

                //이팩트 꺼주고
                particle.gameObject.SetActive(false);
            }
        }
    }
    

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public float CalculateMeteorSize(float range)
    {
        float size = EffectSize * range;
        return size;
    }

}
