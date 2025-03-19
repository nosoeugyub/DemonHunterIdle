using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 회전 돌격 스킬
/// </summary>
public class WhirlwindRush : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("피해 범위")]
    [SerializeField] float DamageRadius = 1f;
    [Header("지속데미지 간격")]
    [SerializeField] float TickInterval = 0.3f;
    [Header("데미지 적용 확률")]
    [SerializeField] float WhirlwindRushHitChance = 80f;
    [Header("애니메이션 재생 속도")]
    [SerializeField] float whirlwindAnimationSpeed = 2f;
    [Header("동시 최대 타겟")]
    [SerializeField] int MaxTarget = 5;
    [Header("적 탐지 범위")]
    [SerializeField]
    private float enemySearchRange = 8f;
    [Header("발동시 추가 속도 퍼센트값")]
    [SerializeField]
    private float moveSpeedPercent = 10;
    [Header("분열되는 투사체 기본공격판정여부")]
    [SerializeField] bool BasicAttack = false;
    #endregion

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentDamageCoroutine;
    private Coroutine currentRemoveSkillCoroutine;

    private WhirlwindRushData whirlwindRushData;

    public float GetAdditionalWhirlwindMoveSpeed()
    {
        if (canUseSkill)
            return HunterStat.MoveSpeedResult(hunter._UserStat) * (0.01f * moveSpeedPercent);

        return 0;
    }
    public float WhirlwindAnimationSpeed => whirlwindAnimationSpeed;
    public float GetTickInterval => TickInterval;

    private bool canUseSkill = false; //스킬 사용 가능한지
    private DamageInfo whirlwindDamageInfo = null;
    private List<IDamageAble> targetList = null;

    private GameObject WhirlwindFx = null;
    private AudioSource whirlwindAudioSource = null;

    public float EnemySearchRange => enemySearchRange;

    UnityEngine.AI.ObstacleAvoidanceType hunterObstacleAvoidType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
    public override event Action<float, bool> OnActiveSecondChanged;

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        whirlwindDamageInfo = new DamageInfo(0, whirlwindRushData.attackdamagetype[0] == Utill_Enum.AttackDamageType.Magic, hunter);
        whirlwindDamageInfo.isNormalAttack = BasicAttack;
        StartWhirlwind();
        //애니메이션, 데미지, 이펙트 제어는 스킬
        //수호자 현재 state 추적해서 만약 조이스틱 움직임/사망/회전돌격/상태 변환중 상태가 아니라면 상태를 바꾸도록
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }
    private void StartWhirlwind()
    {
        isMotion = true;
        OnActiveSecondChanged?.Invoke(whirlwindRushData.CHGValue_SkillDuration, true);

        if(whirlwindAudioSource == null)
            whirlwindAudioSource = SoundManager.Instance.PlayAudioLoop(SkillName);
        
        hunterObstacleAvoidType = hunter.NavMeshAgent.obstacleAvoidanceType;
        hunter.NavMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance; //충돌 안 되도록
        //hunter.NavMeshAgent.velocity = Utill_Standard.Vector3Zero;
        hunter.NavMeshAgent.enabled = true;
        //hunter.Rigid.isKinematic = true;
        hunter.Col.isTrigger = true;
        canUseSkill = true;

        //targetList = Utill_Standard.FindAllObjectsInRadius<IDamageAble>(hunter.transform.position, enemySearchRange, hunter.SearchLayer);

        if (WhirlwindFx == null)
        {
            WhirlwindFx = ObjectPooler.SpawnFromPool(Tag.WhirlwindRushFx, SpawnSkillArea(SkillCastingPositionType).pos, Quaternion.Euler(new Vector3(-90, 0, 0)));
            WhirlwindFx.transform.SetParent(hunter.transform);
            WhirlwindFx.transform.localPosition = Vector3.zero;
            WhirlwindFx.transform.localScale = (Vector3.one) * DamageRadius;
        }

        hunter.ChangeFSMState(Utill_Enum.HunterStateType.Whirlwind);
        hunter.SetAnimation(HunterStateType.Whirlwind, true,(int)SkillAnimationType.Whirlwind);
        hunter.BlockChangeAnimation();

        if (currentDamageCoroutine != null)
        {
            StopCoroutine(currentDamageCoroutine);
        }
        currentDamageCoroutine = StartCoroutine(WhirlwindDamage());

        if (currentRemoveSkillCoroutine != null)
        {
            StopCoroutine(currentRemoveSkillCoroutine);
        }
        currentRemoveSkillCoroutine = StartCoroutine(StopWhirlwind());
    }

    private void EndWhirlwind()
    {
        isMotion = false;
        OnActiveSecondChanged?.Invoke(0, false);
        canUseSkill = false;
        if(hunter != null)
        {
            hunter.NavMeshAgent.obstacleAvoidanceType = hunterObstacleAvoidType;
            //hunter.NavMeshAgent.velocity = Utill_Standard.Vector3Zero;
            //hunter.NavMeshAgent.enabled = true;
            hunter.Rigid.isKinematic = false;
            hunter.Col.enabled = true;
            hunter.Col.isTrigger = false;
            hunter.UnBlockChangeAnimation();

            hunter.SetAnimation(Utill_Enum.HunterStateType.Whirlwind, false);
            hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
        }
        if (WhirlwindFx != null)
        {
            WhirlwindFx.transform.SetParent(GameManager.Instance.ObjectPooler.transform);
            WhirlwindFx.gameObject.SetActive(false);
            WhirlwindFx.transform.localScale = Vector3.one;
            WhirlwindFx = null;
        }
        if(whirlwindAudioSource != null)
        {
            SoundManager.Instance.StopAudioLoop(whirlwindAudioSource);
            whirlwindAudioSource = null;
        }
        if (currentDamageCoroutine != null)
        {
            StopCoroutine(currentDamageCoroutine);
            currentDamageCoroutine = null;
        }
        if (currentRemoveSkillCoroutine != null)
        {
            StopCoroutine(currentRemoveSkillCoroutine);
            currentRemoveSkillCoroutine = null;
        }
    }


    private IEnumerator StopWhirlwind()
    {
        yield return new WaitForSeconds(whirlwindRushData.CHGValue_SkillDuration);

        canUseSkill = false;
    }
    private IEnumerator WhirlwindDamage()
    {
        while (canUseSkill)
        {
            if (!hunter.CanGetDamage)
                canUseSkill = false;

            //헌터 상태 체크
            Utill_Enum.HunterStateType curState = hunter.GetState();

            if(GameStateMachine.Instance.CurrentState is NonCombatState || IdleModeRestCycleSystem.Instance.IsRestMode())
            {
                canUseSkill = false;
            }

            if (curState != Utill_Enum.HunterStateType.JoystickMove && curState != Utill_Enum.HunterStateType.Whirlwind && curState != Utill_Enum.HunterStateType.Die)
            {
                hunter.ChangeFSMState(Utill_Enum.HunterStateType.Whirlwind, true);
            }

            //for (int i = targetList.Count - 1; i >= 0; i--)
            //{
            //    if (targetList[i] == null || !targetList[i].CanGetDamage) //데미지를 못받거나 null이면
            //    {
            //        targetList.RemoveAt(i);
            //    }
            //}
            //if (targetList.Count == 0)
            //{
            //    yield return new WaitForSeconds(HunterStat.Get_WhirlwindRushSpeedResult(hunter._UserStat));
            //    continue;
            //}

            List<IDamageAble> hitEnemyList = Utill_Standard.FindAllObjectsInRadius<IDamageAble>(SpawnSkillArea(SkillCastingPositionType).pos, DamageRadius, hunter.SearchLayer, MaxTarget);
            if (hitEnemyList.Count > 0)
            {
                for (int i = 0; i < hitEnemyList.Count; i++)
                {
                    if (hitEnemyList[i] == null || !hitEnemyList[i].CanGetDamage) //데미지를 못받거나 null이면
                    {
                        hitEnemyList.Remove(hitEnemyList[i]);
                    }
                }
                // 중복을 제거
                hitEnemyList = hitEnemyList.Distinct().ToList();
                if (hitEnemyList.Count == 1)
                {
                    HunterStat.CalculateSkillDamage(hunter._UserStat, (hitEnemyList[0] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, whirlwindDamageInfo,whirlwindRushData);
                    hitEnemyList[0].Damaged(whirlwindDamageInfo);
                }
                else
                {
                    for (int i = 0; i < hitEnemyList.Count; i++)
                    {
                        bool isHit = Utill_Math.Attempt(WhirlwindRushHitChance);
                        if (isHit)
                        {
                            HunterStat.CalculateSkillDamage(hunter._UserStat, (hitEnemyList[i] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, whirlwindDamageInfo, whirlwindRushData);
                            hitEnemyList[i].Damaged(whirlwindDamageInfo);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(HunterStat.Get_WhirlwindRushSpeedResult(hunter._UserStat));

            //int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(hunter._UserStat.SubClass);//해당 타입의 인덱스가져오기
            //float tempmana = 0;

            //tempmana = HunterStat.MinusMp(hunter.Orginstat, SkillUseMP); //마나감소
            //if (tempmana == 0)
            //{
            //    canUseSkill = false;
            //    //여기에 스킬 스스로 꺼주는것 제작
            //    EndWhirlwind();
            //    yield break; //마나 사용실패
            //}
            //else
            //{
            //    //ui까지 변화 적용
            //    hunter.mpuiview.UpdateHpBar(hunter.Orginstat.MP, hunter.Orginstat.CurrentMp);
            //}

        }

        //각종 조건을 충족시키지 못해 while 밖으로 나온 경우
        EndWhirlwind();
        yield break;
    }

    public override void Init_Skill()
    {
        isMotion = false;
        EndWhirlwind();
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        whirlwindRushData = (WhirlwindRushData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.WhirlwindRush, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }

        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, whirlwindRushData.CHGValue_UseMP); //마나감소
        if (tempmana == 0)
        {
            return false; //마나 사용실패
        }
        else
        {
            //ui까지 변화 적용
            DataManager.Instance.Hunters[index].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[index]._UserStat.MP, DataManager.Instance.Hunters[index]._UserStat.CurrentMp);
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        if(canUseSkill && hunter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hunter.transform.position,enemySearchRange);
        }
    }
}
