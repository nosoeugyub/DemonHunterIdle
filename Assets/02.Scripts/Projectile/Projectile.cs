using System;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 투사체 기본 스크립트/실질적 사용은 상속으로 사용함
/// </summary>
public abstract class Projectile : MonoBehaviour
{
    private const float DeleteRange = 0.5f; //화면 이탈시 삭제 계수
    [SerializeField]
    protected GameObject model; //투사체의 실질적인모델

    [SerializeField]
    protected float moveSpeed = 0f;
    [SerializeField]
    protected float movementThreshold = 0.03f; // 움직임 판단을 위한 임계값

    [SerializeField]
    protected float arrowRange = 1f; //투사체의 총 이동거리 제한 (초과시 삭제)
    [Header("동시 최대타격 제한치(-1이면 사용 안 함)")]
    [SerializeField]
    protected int maxHit = -1; //동시 최대타격 제한치(-1이면 사용 안 함)
    [SerializeField]
    protected string strikeEffectName; // 타격시 나타나는 이펙트의 이름
    [Header("에너미 - 헌터 데미지 계산식을 사용하는지")]
    [SerializeField]
    protected bool useStatCalculate = true; //에너미 - 헌터 데미지 계산식을 사용하는지
    [Header("사용자 투사체의 스팩에 적용 시키는지")]
    [SerializeField]
    protected bool isUseStat = true; //사용자 투사체의 스팩에 적용 시키는지

    protected int maxTarget = -1; //타격시 피격하는 총 대상 수 (-1이면 사용 안 함)
    protected float applicationDistance = 0f; //타격 성공 후 적 감지 범위의 위치 조정 값
    protected float damageRadius = 0f; //타격 성공 후 적 감지 범위

    protected float curArrowRange = 0f;
    protected int projectileId = -1; //투사체 중복 타격 제한용 투사체 id
    protected Vector3 targetPos;
    protected LayerMask searchLayer;
    protected bool isHit = false;

    protected List<IDamageAble> targetList;
    
    protected Hunter hunterOwner = null;
    protected Enemy enemyOwner = null;

    private Camera mainCamera = null; //메인 카메라 담기 위한 변수

    public DamageInfo DamageInfo;
    public BaseSkillData BaseSkillData;

    public Action<List<IDamageAble>,Projectile> OnAttackAction = null; //타격시 할 행동. 타격 받은 적들을 매개변수로 받음
    public Func<DamageInfo,DamageInfo> AdditionalDamageLogicFunc = null; //데미지 계산이 끝난 후 추가적인 계산 로직을 탈경우 사용(스킬)

    protected float MoveDistance => (moveVector * moveSpeed * Time.fixedDeltaTime).magnitude;
    public virtual Vector3 moveVector => Vector3.forward;
    protected Vector3 previousPosition; // 이전 프레임에서의 위치 저장 변수

    protected virtual void FixedUpdate()
    {
        previousPosition = transform.position; // 이전 움직임 저장
        FixedUpdateLoop();
        curArrowRange += MoveDistance;
        DeleteIfNeed();
    }

    //오브젝트 풀링
    private void OnDisable()
    {
        curArrowRange = 0;
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만
        CancelInvoke();     // Monobehaviour에 Invoke가 있다면
    }

    protected abstract void FixedUpdateLoop();
    protected abstract void Move();
    protected abstract void FindTarget();

    protected virtual bool DamageToTarget(List<IDamageAble> targetList)
    {
        bool isSuccessToReceive = false; //데미지 한 명한테라도 주는데에 성공함(반환용)
        for (int i = 0; i < targetList.Count; i++)
        {
            if(useStatCalculate)
            {
                if(hunterOwner != null) //헌터가 몬스터를 때렸을때
                {
                    if (!(targetList[i] as IEnemyDamageAble).DamageAbleEnemy.CheckProjectileHit(projectileId, maxHit))
                        continue;
                    (DamageInfo.ReflectRange, DamageInfo.isInstanceKillBoss, DamageInfo.damage, DamageInfo.addDamage, DamageInfo.CoupDamage, DamageInfo.CoupAddDamage, DamageInfo.isCoup, DamageInfo.isCri, DamageInfo.isdodge, DamageInfo.isStun, DamageInfo.isFreezing, DamageInfo.isPosion, DamageInfo.isSlow, DamageInfo.isElectric, DamageInfo.isChain) = (DamageInfo.isMagic) ? 
                        HunterStat.DamageToMagic(hunterOwner._UserStat, (targetList[i] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, EnemyClass: (targetList[i] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat.Class, isNormalAttack:DamageInfo.isNormalAttack) : 
                        HunterStat.DamageToPhysical(hunterOwner._UserStat, (targetList[i] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, EnemyClass: (targetList[i] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat.Class, isNormalAttack: DamageInfo.isNormalAttack);
                }
                else if(enemyOwner != null)
                {
                    (DamageInfo.damage , DamageInfo.addDamage , DamageInfo.isCri , DamageInfo.isdodge, DamageInfo.isStun, DamageInfo.isFreezing, DamageInfo.isPosion, DamageInfo.isSlow) = (DamageInfo.isMagic) ?
                        EnemyStat.DamageToMagic(enemyOwner._EnemyStat, (targetList[i] as IHunterDamageAble).DamageAbleHunter._UserStat) :
                        EnemyStat.DamageToPhysical(enemyOwner._EnemyStat, (targetList[i] as IHunterDamageAble).DamageAbleHunter._UserStat);
                }
            }
            else
            {
                if (hunterOwner != null) //헌터가 몬스터를 때렸을때
                {
                    if(DamageInfo.skill != null)
                        HunterStat.CalculateSkillDamage(hunterOwner._UserStat, (targetList[i] as IEnemyDamageAble).DamageAbleEnemy._EnemyStat, DamageInfo, BaseSkillData);
                }
            }

            if (targetList[i] is IEnemyDamageAble && !(targetList[i] as IEnemyDamageAble).DamageAbleEnemy.CheckProjectileHit(projectileId, maxHit)) 
                continue;

            isSuccessToReceive = true;
            if (AdditionalDamageLogicFunc != null) //추가적인 외부 로직이 필요하다면
                DamageInfo = AdditionalDamageLogicFunc(DamageInfo);
            targetList[i].Damaged(DamageInfo);
        }

        if (strikeEffectName != string.Empty && isSuccessToReceive)
        {
            //임시로 여기서 스폰
            GameObject strikeEffect = ObjectPooler.SpawnFromPool(strikeEffectName, targetList[0].ObjectTransform.position, Quaternion.Euler(new Vector3(-90,0,0)));
            strikeEffect.transform.position = new Vector3(strikeEffect.transform.position.x, 1, strikeEffect.transform.position.z);
            strikeEffect.transform.localScale = (Utill_Standard.vector3One*damageRadius);
        }

        if(OnAttackAction != null && isSuccessToReceive)
        {
            OnAttackAction.Invoke(targetList,this); //공격시 할 액션이 있다면 초기화
        }

        return isSuccessToReceive; //타격 성공 여부 반환
    }

    protected virtual void DeleteIfNeed()
    {
        // 이전 위치와 현재 위치 사이의 거리를 계산하여 움직임 여부 확인
        float distance = (previousPosition - transform.position).sqrMagnitude;

        // 거리가 임계값보다 작다면 움직임이 없다고 판단
        if (distance < movementThreshold)
        {
            // 움직임이 없음
            gameObject.SetActive(false);
            return;
        }
        //화면 밖을 벗어났으면 삭제
        if(mainCamera == null)
            mainCamera = Camera.main;
        if (!SleepModeManager.Instance.IsSleepMode)
        {
            Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
            if (viewPos.x < 0 - DeleteRange || viewPos.x > 1 + DeleteRange
                || viewPos.y < 0 - DeleteRange || viewPos.y > 1 + DeleteRange || viewPos.z <= 0 - DeleteRange)
            {
                gameObject.SetActive(false);
            }
        }
        //높이 값이 일정 이하이면
        if(transform.position.y <= -DeleteRange)
        {
            gameObject.SetActive(false);
        }
        if(curArrowRange>arrowRange) //투사체의 총 이동거리 제한 초과시
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void SetProjectileWithoutTarget(Entity entity,DamageInfo damageInfo, Vector3 targetPos, LayerMask searchLayer,int projectileId,BaseSkillData baseSkillData = null)
    {
        isHit = false;

        if (entity is Hunter)
        {
            enemyOwner = null;
            hunterOwner = (Hunter)entity;
        }
        else if (entity is Enemy)
        {
            hunterOwner = null;
            enemyOwner = (Enemy)entity;
        }

        DamageInfo = damageInfo;
        BaseSkillData = baseSkillData;
        this.targetPos = targetPos;
        this.searchLayer = searchLayer;
        this.projectileId = projectileId;

        if (targetList == null)
            targetList = new();
        else
            targetList.Clear();
        OnAttackAction = null;
        AdditionalDamageLogicFunc = null;
        Setting();
    }
    public virtual void SetProjectileWithTarget(Entity entity, DamageInfo damageInfo, IDamageAble target, LayerMask searchLayer,int projectileId)
    {
        isHit = false;

        if (entity is Hunter)
        {
            enemyOwner = null;
            hunterOwner = (Hunter)entity;

            if(isUseStat)
            {
                NormalAttackStats tmpStat = StatManager.Instance.GetHunterNormalAttackStat(hunterOwner._UserStat.SubClass);
                maxTarget = tmpStat.MaxTarget;
                damageRadius = tmpStat.DamageRadius;
                applicationDistance = tmpStat.ApplicationDistance;
    
                arrowRange = HunterStat.ProjectileRangeResult(hunterOwner._UserStat); //헌터라면 헌터 스탯을 투사체에 적용
            }
        }
        else if (entity is Enemy)
        {
            hunterOwner = null;
            enemyOwner = (Enemy)entity;
        }

        this.DamageInfo = damageInfo;
        this.searchLayer = searchLayer;
        this.projectileId = projectileId;

        targetList = new()
        {
            target
        };
        targetPos = target.ObjectTransform.position;
        OnAttackAction = null;
        AdditionalDamageLogicFunc = null;

        Setting();
    }

    protected abstract void Setting();
}
