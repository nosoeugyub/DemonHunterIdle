using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using System;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 보호의 축복 스킬
/// </summary>
public class ProtectionBless :BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수

    [Header("최대체력의 몇 %를 보호막으로 줄건지")]
    [SerializeField] float shieldPercent;

    [Header("배틀 전체 헌터들에게 보호막을 줄건지")] 
    [SerializeField] private bool isAllHunter;

    #endregion
   
    private Coroutine currentCoroutine;

    // 각각의 헌터에 대한 이펙트를 관리하기 위한 딕셔너리
    private Dictionary<Hunter, GameObject> hunterEffects = new Dictionary<Hunter, GameObject>();

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Shield());
    }
    IEnumerator Shield()
    {
        SoundManager.Instance.PlayAudio(skillName);

        float maxHealth = hunter.MaxHp; // 최대 체력
        float _shieldPercent = shieldPercent * 0.01f;
        float shieldValue = maxHealth * _shieldPercent; 

        Debug.Log("보호막 실행 수치: " + shieldValue);

        if (isAllHunter)
        {
            List<Hunter> battleHunters = DataManager.Instance.GetEquippedHunters();

            foreach (var battleHunter in battleHunters)
            {
                // 이펙트 생성
                GameObject fx = ObjectPooler.SpawnFromPool(Tag.ProtectionBless, Vector3.zero);
                fx.transform.SetParent(battleHunter.transform);
                fx.transform.localPosition = battleHunter.EffectOffset;

                // 딕셔너리에 저장하여 관리
                hunterEffects[battleHunter] = fx;

                battleHunter.PlusShield(shieldValue);
                battleHunter.OnShieldEndAction += DeactiveEffect;
            }

            yield return new WaitForSeconds(_SkillDuration);

            // 모든 헌터의 보호막을 초기화하고 이펙트 비활성화
            foreach (var battleHunter in battleHunters)
            {
                battleHunter.InitShield();
                DeactiveEffect(battleHunter);
            }

        } // 배틀 참여하는 모든 헌터
        else // 수호자 전용
        {
            GameObject fx = ObjectPooler.SpawnFromPool(Tag.ProtectionBless, Vector3.zero);
            fx.transform.SetParent(hunter.transform);
            fx.transform.localPosition = hunter.EffectOffset;

            hunterEffects[hunter] = fx;

            hunter.PlusShield(shieldValue);
            hunter.OnShieldEndAction += DeactiveEffect;

            yield return new WaitForSeconds(_SkillDuration);

            hunter.InitShield();
            DeactiveEffect(hunter);
        } 

        currentCoroutine = null;
    }

    public void DeactiveEffect(Hunter targetHunter)
    {
        if (hunterEffects.TryGetValue(targetHunter, out GameObject fx))
        {
            fx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
            fx.SetActive(false);
        }
    }

   
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    public (Vector3 pos, List<Entity> entitylist) SpawnSkillArea(Utill_Enum.SkillCastingPositionType SkillCastingPositionType)
    {
        Vector3 position = Vector3.zero;
        List<Entity> entitylist = new List<Entity>();
        return (position, entitylist);
    }

    public bool CheackEnemyArea(float _radlus)
    {
        return false;
    }

    public void Init_Skill()
    {
    }
}
