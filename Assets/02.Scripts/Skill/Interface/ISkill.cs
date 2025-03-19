using System;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-05-22
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬인터페이스
/// </summary>
public interface ISkill
{
    /// <summary>
    /// 스킬의 이름
    /// </summary>
    string SkillName { get; set; }

    /// <summary>
    /// 스킬의 강화 횟수
    /// </summary>
    int _upgradeAmount { get; set; }

    /// <summary>
    /// 스킬 데미지 기반
    /// </summary>
    Utill_Enum.AttackDamageType SkillDamageBase { get; set; }

    /// <summary>
    /// 스킬 데미지 계수
    /// </summary>
    float SkillDamageCoefficient { get; set; }

    /// <summary>
    /// 스킬데미지 타입
    /// </summary>
    Utill_Enum.AttackDamageType SkillDamageType { get; set; }

    /// <summary>
    /// 쿨타임
    /// </summary>
    int SkillCoolDown { set; get; }

    /// <summary>
    /// 마나감소
    /// </summary>
    float SkillUseMP { set; get; }
    /// <summary>
    /// 스킬 범위
    /// </summary>
    float SkillDiameter { get; set; }

    /// <summary>
    /// 헌터의 버프형 지속시간
    /// </summary>
    float SkillDuration { get; set; }

    /// <summary>
    /// 스택형 스킬 여부 
    /// true일시 스택형 스킬
    /// </summary>
    bool IsStackable { get; set; }


    /// <summary>
    /// 스킬 모션이 있는 스킬중 모션이 활성화 됐는가 
    /// </summary>
    bool isMotion { get; set; }

    bool isActiveSkill { get; set; }

    /// <summary>
    /// 헌터스킬 범위
    /// </summary>
    Utill_Enum.SkillCastingPositionType SkillCastingPositionType { get; set; }

    /// <summary>
    /// 스킬 팝업창에 피해범위 띄우기 위해 사용됨. 만약 해당 특정변수를 포함한 스킬이라면 여기의 값을 추가해야됨
    /// </summary>
    float DamageRadius_ { get; set; }
    float SkillRange_ { get; set; }

    /// <summary>
    /// 버프 스택 변경 이벤트
    /// </summary>
    event Action<int> OnStackChanged;

    /// <summary>
    /// 지속시간 연출 이벤트
    /// </summary>
    event Action<float,bool> OnActiveSecondChanged;

    /// <summary>
    /// 스킬의 주요 동작을 실행합니다.
    /// </summary>
    void Execute(Utill_Enum.SubClass subclasstype);

    /// <summary>
    /// 스킬을 업그레이드하여 효과를 증가시키거나 행동을 수정합니다.
    /// </summary>
    void Upgrade(Utill_Enum.SubClass subclasstype );

    /// <summary>
    /// 스킬의 쿨다운 상태를 리셋하여 다시 사용할 수 있도록 합니다.
    /// </summary>
    bool Cooldown(Utill_Enum.SubClass subclasstype );
    /// <summary>
    /// 스킬의 쿨다운 상태를 리셋하여 다시 사용할 수 있도록 합니다.
    /// </summary>
    (Vector3 pos , List<Entity> entitylist) SpawnSkillArea ( Utill_Enum.SkillCastingPositionType SkillCastingPositionType);

    bool CheackEnemyArea(float _radlus);

    void Init_Skill();


}





