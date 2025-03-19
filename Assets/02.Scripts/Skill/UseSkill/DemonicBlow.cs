using Game.Debbug;
using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 악귀의 일격 스킬
/// </summary>
public class DemonicBlow : BaseSkill
{ 
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수 
    [Header("투사체 개수")]
    [SerializeField] int DemonicBlowNumber = 1;
    [Header("투사체 사이의 각도")]
    [SerializeField] float shotAngle = 30;
    #endregion

    private DamageInfo demonicBlowDamageInfo = null;
    private int projectileId = 0; //중복 타격 방지용
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        //스킬 데미지 정보 세팅
        demonicBlowDamageInfo = new(0, skillDamageType == Utill_Enum.AttackDamageType.Magic, hunter);

        //타입정하기
        var skillposlist = SpawnSkillArea(SkillCastingPositionType);
        Vector3 SpawnEffectPos = skillposlist.pos;
        SpawnEffectPos.y = hunter.FireTransform.position.y;

        //스킬 투사체 소환 
        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(Tag.DemonicBlow, hunter.FireTransform, DemonicBlowNumber, shotAngle);
        for (int i = 0; i < projectileList.Count; i++)
        {
            projectileList[i].gameObject.SetActive(false);
            projectileList[i].transform.position = SpawnEffectPos;
            Vector3 targetPos = projectileList[i].transform.position + projectileList[i].transform.forward * 5f;
            if (TargetDirection == false)
            {
                targetPos = SpawnEffectPos;
                projectileList[i].transform.eulerAngles = Utill_Standard.Vector3Zero;
            }
            projectileList[i].SetProjectileWithoutTarget(hunter, demonicBlowDamageInfo, targetPos, hunter.SearchLayer, projectileId);
            projectileList[i].gameObject.SetActive(true);
        }
        projectileId = projectileId < 1010 ? projectileId + 1 : 1000; //일정 id 넘어가면 초기화
        SoundManager.Instance.PlayAudio(skillName);
    }


    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

}
