using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 적이 디버프(스턴/빙결)을 받은 상태 구현
/// </summary>
public class EnemyDebuffState : EnemyState
{
    EnemyStateType prevState;

    Dictionary<Debuff_Type,float> curType = new();
    StunEffect stunEffect = null;
    SkinnedMeshRenderer[] meshRenderers = null;
    List<Material> normalMaterials = new();

    float stunTimer = 0f;
    float freezingTimer = 0f;

    public EnemyDebuffState(Enemy enemy) : base(enemy)
    {
        curType.Clear();
        foreach(var type in enemy.debuff_Types)
        {
            curType.Add(type.Key, type.Value);
        }
    }

    public override void Enter()
    {
        if (enemy.NavMeshAgent.isOnNavMesh)
        {
            enemy.NavMeshAgent.SetDestination(enemy.transform.position);
        }
        enemy.NavMeshAgent.enabled = false;
        enemy.Obstacle.enabled = true;
        prevState = enemy.GetPrevState();
        //if (enemy.debuff_Types.ContainsKey(Debuff_Type.Stun)) //스턴이라면?
        //{
        //    InitStun();
        //}
        //if (enemy.debuff_Types.ContainsKey(Debuff_Type.Freezing))
        //{
        //    InitFreezing();
        //}
    }

    public override void Update()
    {
        CheckAllDebuff();
        UpdateTimers();
    }

    public override void Exit()
    {
        enemy.Obstacle.enabled = false;
        enemy.NavMeshAgent.enabled = true;
        StopStun();
        StopFreezing();
    }

    private void CheckAllDebuff()
    {
        if (!enemy.debuff_Types.ContainsKey(Debuff_Type.Stun))
        {
            StopStun();
            curType.Remove(Debuff_Type.Stun);
        }
        else if(enemy.debuff_Types.ContainsKey(Debuff_Type.Stun) && !curType.ContainsKey(Debuff_Type.Stun))
        {
            InitStun();
            curType.Add(Debuff_Type.Stun, enemy.debuff_Types[Debuff_Type.Stun]);
        }

        if (!enemy.debuff_Types.ContainsKey(Debuff_Type.Freezing))
        {
            StopFreezing();
            curType.Remove(Debuff_Type.Freezing);
        }
        else if(enemy.debuff_Types.ContainsKey(Debuff_Type.Freezing) && !curType.ContainsKey(Debuff_Type.Freezing))
        {
            InitFreezing();
            curType.Add(Debuff_Type.Freezing, enemy.debuff_Types[Debuff_Type.Freezing]);
        }
    }
    private void UpdateTimers()
    {
        //if (stunTimer > 0)
        //{
        //    stunTimer -= Time.deltaTime;
        //    if (stunTimer <= 0)
        //    {
        //        StopStun();
        //        curType.Remove(Debuff_Type.Stun);
        //    }
        //}

        //if (freezingTimer > 0)
        //{
        //    freezingTimer -= Time.deltaTime;
        //    if (freezingTimer <= 0)
        //    {
        //        StopFreezing();
        //        curType.Remove(Debuff_Type.Freezing);
        //    }
        //}

        //// 만약 모든 디버프가 제거되면 이전 상태로 돌아갑니다.
        //if (curType.Count == 0)
        //{
        //    enemy.ChangeFSMState(prevState);
        //}
    }
    /// <summary>
    /// 스턴 시작시 세팅
    /// </summary>
    private void InitStun()
    {
        stunTimer = enemy.debuff_Types[Debuff_Type.Stun];
        //이펙트위치구하기
        Vector3 temp_pos = enemy.EffectOffset + enemy.transform.position;
        //스턴 이펙트 가져오기 
        //차후 몬스터 타입/헌터 타입에 따라 스턴 다른 크기의 프리펩이 소환되도록 제작
        stunEffect = ObjectPooler.SpawnFromPool("StunEffect",Vector3.zero).GetComponent<StunEffect>();
        stunEffect.Initialize(enemy.transform, enemy.EffectOffset);
    }
    /// <summary>
    /// 스턴 종료시 세팅
    /// </summary>
    private void StopStun()
    {
        if (stunEffect != null)
        {
            ObjectPooler.ReturnToPool(stunEffect.gameObject);
            stunEffect.gameObject.SetActive(false);
            stunEffect = null;
        }
    }

    /// <summary>
    /// 빙결 시작시 세팅
    /// </summary>
    private void InitFreezing()
    {
        freezingTimer = enemy.debuff_Types[Debuff_Type.Freezing];
        Material freezingMaterial = GameManager.Instance.freezingMaterial;
        meshRenderers = enemy.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            normalMaterials.Add(meshRenderers[i].material);
            meshRenderers[i].material = freezingMaterial;
        }
    }
    /// <summary>
    /// 빙결 종료시 세팅
    /// </summary>
    private void StopFreezing()
    {
        if(meshRenderers != null)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                normalMaterials.Add(meshRenderers[i].material);
                meshRenderers[i].material = normalMaterials[i];
            }
            meshRenderers = null;
        }
    }
}
