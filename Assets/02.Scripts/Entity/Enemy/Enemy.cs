using MonsterLove.StateMachine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using static Utill_Enum;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 적 객체를 전체적으로 관리
/// FSM 외부 에셋 사용함. git 링크
/// https://github.com/thefuntastic/Unity3d-Finite-State-Machine?tab=readme-ov-file
/// </summary>
public class Enemy : Entity, IEnemyDamageAble
{
    [Header("Hpbar")]
    public HpUiView enemyhpuiview;
    public RectTransform enemyuipos;
    public Vector3 Pos;
    public Vector2 size;

    [Header("NavMesh")]
    [SerializeField]
    private NavMeshAgent navmeshAgent;
    [SerializeField]
    private NavMeshObstacle obstacle;
    
    private MaterialSetter materialSetter;

    
    public NavMeshSurface EnemyNav;

    public NavMeshAgent NavMeshAgent { get { return navmeshAgent; } }
    public NavMeshObstacle Obstacle { get { return obstacle; } }
    public Rigidbody Rigid { get { return rigid; } }
    public Transform FireTransform { get { return fireTransform; } }
    public CapsuleCollider Col { get { return col; } }

    [SerializeField]
    private Transform fireTransform = null;  //공격시 화살 생성하는 위치

    //Effect
    public Dictionary<Debuff_Type,float> debuff_Types = new(); //나중에 스탯쪽으로 이전 및 관련 코드 진행

    private IDamageAble target = null;
    public IDamageAble Target { get {  return target; } set { target = value; } }

    private IDamageAble offender = null; //누구에게 죽었는지 저장
    public IDamageAble Offender { get { return offender; } set { offender = value; } }

    [Header("검색할 범위")]
    [SerializeField]
    private float searchRadius = 5f; // 검색할 범위

    [Space(20)]
    [SerializeField]
    private LayerMask searchLayer;   // 검색할 레이어

    public float SearchRadius { get { return searchRadius; } }
    public LayerMask SearchLayer { get { return searchLayer; } }
    private StateMachine<EnemyStateType> stateMachine;

    [Header("몬스터 스텟 관련")]
    [SerializeField] private Utill_Enum.EnemyClass EnemyClass;
    private EnemyStat orginEnemyStat; //기본적으로 사용할 스텟
    public EnemyStat OrginEnemyStat
    {
        get { return orginEnemyStat; }
        set { orginEnemyStat = value; }
    }

    private EnemyStat _enemystat;
    public EnemyStat _EnemyStat //최종으로...
    {
        get { return _enemystat; }
        set { _enemystat = value; }
    }

    protected Dictionary<EnemyStateType, EnemyState> stateDict = new();
    protected EnemyAttackState attackState = null;
    protected EnemyAnimationController animationController = null;

    //IEnemyDamageAble 인터페이스
    public Enemy DamageAbleEnemy { get { return this; } }
    public Transform ObjectTransform
    {
        get
        {
            return gameObject.transform;
        }
    }
    public bool CanGetDamage
    {
        get
        {
            return stateMachine.State != EnemyStateType.Die;
        }
    }

    public Action<Enemy> OnDieAction = null; //사망시 호출될 Action

    private float hp;
    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }
    private float power = 10;
    public float Power
    {
        get { return power; }
        set { power = value; }
    }


    private bool _isHunterDie = false;
    public bool isHunterDie
    {
        get { return _isHunterDie; }
        set { _isHunterDie = value; }
    }

    private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    private float curhp;
    public float CurHp
    {
        get { return curhp; }
        set { curhp = value; }
    }

    [SerializeField] private Ground _Ground;
    public Ground Ground
    {
        get { return _Ground; }
        set { _Ground = value; }
    }
    /// <summary>
    /// stage에 따라 메테리얼 다르게 설정시 사용
    /// MaterialSetter 세팅 및 호출함
    /// </summary>
    public void SetMaterial(string materialName)
    {
        if(materialSetter == null)
            materialSetter = GetComponent<MaterialSetter>();

        materialSetter.SetMaterial(materialName);
    }
    /// <summary>
    /// 현재 Enemy의 모든 메테리얼 반환
    /// MaterialSetter 세팅 및 호출함
    /// </summary>
    public List<Material> GetAllMaterials()
    {
        if (materialSetter == null)
            materialSetter = GetComponent<MaterialSetter>();

        return materialSetter.GetAllMaterialsInModel();
    }

    public void SetNaviSurface(NavMeshSurface nav)
    {
        EnemyNav = nav;
    }

    private void Awake()
    {
        //stateMachine = new(this);
        if (stateMachine == null)
        {
            stateMachine = StateMachine<EnemyStateType>.Initialize(this);
        }
    }

    public void EnemyDie()
    {
        //바로 상태 죽음
        //stateMachine.ChangeState(EnemyState.Die, StateTransition.Overwrite);
        this.gameObject.SetActive(false);
        if(Ground != null)
            Ground.GroundEnemyList.Remove(this);
        //스텟 적용
        if (_EnemyStat != null)
        {
            enemyhpuiview.UpdateHpBar(_EnemyStat.HP, _EnemyStat.CurrentHp);
        }
    }



    private void Start()
    {
        GameEventSystem.GameBattleSequence_SendEventHandler += ((bool isBattle) =>
        {
            if (isBattle)
            {
                rigid.isKinematic = false;
            }
            //단체 탐지 이동/공격 하려면 주석 해제
            //if (gameObject.activeSelf && isBattle && CanGetDamage)
            //{
            //    stateMachine.ChangeState(EnemyState.Move, StateTransition.Overwrite);
            //}
        });
        GameEventSystem.GameHunterDie_SendGameEventHandler += (() =>
        {
            isHunterDie = true;
            if(gameObject.activeInHierarchy)
            stateMachine.ChangeState(EnemyStateType.Idle, StateTransition.Overwrite);
        });
    }
    public void OnEnable()
    {
        

        GameEventSystem.GameEnemyDie_SendGameEventHandler += EnemyDie;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnHpOnChanged += HandleHpOnChanged;
        }

    }



    public void OnDisable()
    {
        if (StatusEffectList != null)
        {
            foreach (StatusEffect item in StatusEffectList)
            {
                item.RemoveEffect(this);
            }
        }
        

        //반전 시킨 값 초기화
        Vector3 currentScale = transform.localScale;
        currentScale = Utill_Math.AbsVector(currentScale);
        transform.localScale = currentScale;
        enemyuipos.localScale = Utill_Math.AbsVector(enemyuipos.localScale);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnHpOnChanged -= HandleHpOnChanged;
        }
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color for the Gizmo
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        if (_EnemyStat == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _EnemyStat.AttackRange);
    }

    private void Update()
    {
        EnemyCheat();
    }


    private void EnemyCheat()
    {
#if UNITY_EDITOR
        if (TestEnemyStat.Instance.isTest)
        {
            EnemyStat enemystat = TestEnemyStat.Instance.MakeEnemyStat();
            _EnemyStat = enemystat;

            if (HP != _EnemyStat.HP)
            {
                HP = _EnemyStat.HP;
                CurHp = HP;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //체력 치트 동기화
                EnemyStat.SetHp(_EnemyStat, _EnemyStat.HP);
                CurHp = _EnemyStat.HP;

            }
        }
        else if (!TestEnemyStat.Instance.isTest)
        {
            _EnemyStat = OrginEnemyStat;
        }
#endif
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<IHunterDamageAble>() != null)
            rigid.isKinematic = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.GetComponent<IHunterDamageAble>() != null)
            rigid.isKinematic = false;
    }

    public void ChangeFSMState(EnemyStateType state)
    {
        if (gameObject.activeInHierarchy)
            stateMachine.ChangeState(state);
    }
    public EnemyStateType GetPrevState()
    {
        return stateMachine.LastState;
    }

    /// <summary>
    /// 바 관련 UI 위치 옮겨 안 보이는 것 처럼 처리해줌
    /// hpuiview의 ActiveObject 참고함
    /// </summary>
    private void ActiveHpObject(bool isactive)
    {
        if (isactive)
        {
            enemyuipos.transform.localPosition = Pos;
            //enemyuipos.localScale = size;
        }
        else
        {
            enemyuipos.transform.localPosition = new Vector3(9999, 9999, 9999);
        }

    }
    private void HandleHpOnChanged(bool isHpOn)
    {
        ActiveHpObject(isHpOn);
        if (isHpOn)
        {
            enemyhpuiview.UpdateHpBar(OrginEnemyStat.HP, _EnemyStat.CurrentHp);
        }
    }
    /// <summary>
    /// 적 재설정
    /// </summary>
    public void Setting(EnemyStat _enemystat)
    {

        //ui셋팅도 같이해줌
        enemyuipos.anchoredPosition = Pos;
        enemyuipos.localScale = size;

        StatusEffectList = new List<StatusEffect>();

        animator.enabled = true;
        col.enabled = true;
        
        animator.Rebind(); 
        animationController = new EnemyAnimationController(animator);

        OrginEnemyStat = _enemystat;
        isHunterDie = false;
        target = null;
        ClearProjectileHit();

        OnDieAction = null;

        if (TestEnemyStat.Instance.isTest)
        {
            _EnemyStat = TestEnemyStat.Instance.MakeEnemyStat();
        }
        else
        {
            _EnemyStat = OrginEnemyStat;
        }

        EnemyStat.SetMaxHp(_EnemyStat);
        _EnemyStat.CurrentHp = _EnemyStat.HP;
        HP = _EnemyStat.HP;
        CurHp = _EnemyStat.CurrentHp;
        Speed = EnemyStat.MoveSpeedResult(_EnemyStat);
        EnemyClass = _EnemyStat.Class;

        ActiveHpObject(true);
        enemyhpuiview.UpdateHpBar(_EnemyStat.HP, _EnemyStat.CurrentHp);
        //hpbar 위치셋팅
       

        HandleHpOnChanged(GameManager.Instance.IsHpOn);

        navmeshAgent.enabled = true;
   

        //50%확률로 X값이 -1됨
        Vector3 currentScale = transform.localScale;
        if (Utill_Math.Attempt(50))
        {
            currentScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
            transform.localScale = currentScale;
            //체력바도 바꿔줘야함
            enemyhpuiview.HpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            if(enemyhpuiview.BossTimeBar != null)
                enemyhpuiview.BossTimeBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            currentScale = new Vector3(currentScale.x, currentScale.y, currentScale.z);
            transform.localScale = currentScale;
            enemyhpuiview.HpBar.fillOrigin = (int)Image.OriginHorizontal.Right;
            if (enemyhpuiview.BossTimeBar != null)
                enemyhpuiview.BossTimeBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }

        //네비
        navmeshAgent.speed = etcAnimSpeed;
        navmeshAgent.updateRotation = false;

        //상태 초기화 후 fsm  시작
        SetState();
        if (gameObject.activeInHierarchy)
            stateMachine.ChangeState(EnemyStateType.Idle, StateTransition.Overwrite);

       
    }

  

    /// <summary>
    /// 사용할 state Init
    /// </summary>
    protected virtual void SetState()
    {
        stateDict.Clear();
        stateDict.Add(EnemyStateType.Move, new EnemyMoveState(this));
        stateDict.Add(EnemyStateType.Idle, new EnemyIdleState(this));
        stateDict.Add(EnemyStateType.Die, new EnemyDieState(this));
        stateDict.Add(EnemyStateType.Debuff, new EnemyDebuffState(this));

        switch (_EnemyStat.AttackType)
        {
            case Utill_Enum.Enum_AttackType.None:
                attackState = new EnemyAttackState(this);
                break;
            case Utill_Enum.Enum_AttackType.Melee:
                attackState = new EnemyMeleeAttackState(this);
                break;
            case Utill_Enum.Enum_AttackType.Range:
                attackState = new EnemyRangeAttackState(this);
                break;
            case Utill_Enum.Enum_AttackType.Spell:
                break;
        }
        stateDict.Add(EnemyStateType.Attack, attackState);
    }

    /// <summary>
    /// 몬스터 움직임 멈춤
    /// </summary>
    public void StopEnemy()
    {
        stateMachine.ChangeState(EnemyStateType.Stop, StateTransition.Overwrite);
    }
    /// <summary>
    /// 몬스터 움직임 재시작
    /// </summary>
    public void UnStopEnemy()
    {
        //자신이 이미 죽었거나 죽는 중이면 return
        if (!gameObject.activeInHierarchy || stateMachine.State == EnemyStateType.Die) 
            return;
        stateMachine.ChangeState(EnemyStateType.Idle, StateTransition.Overwrite);
    }

    // fsm 코드 
    // StateName_MethodName (상태명_메서드명)으로 함수를 작성하면 State에 맞춰 실행됨
    #region FSM 코드
    protected void Move_Enter()
    {
        animationController.SetAnimationWithState(EnemyStateType.Move, true);
        stateDict[EnemyStateType.Move].Enter();
    }

    protected void Move_FixedUpdate()
    {
        navmeshAgent.speed = EnemyStat.MoveSpeedResult(_EnemyStat);
        animationController.SetSpeed(EnemyStat.MoveSpeedResult(_EnemyStat) / moveSpeedNum);
        stateDict[EnemyStateType.Move].FixedUpdate();
    }
    protected void Move_Exit()
    {
        animationController.SetSpeed(etcAnimSpeed);
        animationController.SetAnimationWithState(EnemyStateType.Move, false);
        stateDict[EnemyStateType.Move].Exit();
    }
    protected void Idle_Enter()
    {
        stateDict[EnemyStateType.Idle].Enter();
        animationController.SetAnimationWithState(EnemyStateType.Idle, true);
    }
    protected void Idle_Update()
    {
        if (isHunterDie) return;
        stateDict[EnemyStateType.Idle].Update();
    }
    protected void Idle_Exit()
    {
        navmeshAgent.enabled = true;
        if (_EnemyStat.Class == EnemyClass.Boss)
        {
            enemyhpuiview.UpdateBossTimeBar();
        }
        stateDict[EnemyStateType.Idle].Exit();

        
    }

    protected IEnumerator Die_Enter()
    {
       
        if (stateMachine.LastState == EnemyStateType.Die) yield break;

        stateDict[EnemyStateType.Die].Enter();

        animationController.SetAnimationWithState(EnemyStateType.Die);
        
        if(OnDieAction != null)
            OnDieAction.Invoke(this); //사망시 호출될 이벤트 호출

        yield return Utill_Standard.WaitTimeOne;
        gameObject.SetActive(false);
        animator.enabled = false;
    }
    protected void Attack_Enter()
    {
        stateDict[EnemyStateType.Attack].Enter();

        StopCoroutine(AttackLoop());
        StartCoroutine(AttackLoop());
    }

    protected void Attack_Update()
    {
        stateDict[EnemyStateType.Attack].Update();
    }

    protected void Attack_Exit()
    {
        stateDict[EnemyStateType.Attack].Exit();

        StopCoroutine(AttackLoop());
        animationController.SetSpeed(etcAnimSpeed);
    }

    protected void Debuff_Enter()
    {
        stateDict[EnemyStateType.Debuff].Enter();

        if (debuff_Types.ContainsKey(Debuff_Type.Freezing))
            animationController.SetAnimationWithState(EnemyStateType.Debuff, true);
        else if (debuff_Types.ContainsKey(Debuff_Type.Stun))
            animationController.SetAnimationWithState(EnemyStateType.Idle, true);
    }

    public void FreezAnimation(bool isStart)
    {
        animationController.SetAnimationWithState(EnemyStateType.Debuff, isStart);
    }

    public void StunAnimation(bool isStart)
    {
        animationController.SetAnimationWithState(EnemyStateType.Idle, isStart);
    }


    protected void Debuff_Update()
    {
        if (isHunterDie) return;
        stateDict[EnemyStateType.Debuff].Update();
    }
    protected void Debuff_Exit()
    {
        stateDict[EnemyStateType.Debuff].Exit();

        if (debuff_Types.ContainsKey(Debuff_Type.Freezing))
            animationController.SetAnimationWithState(EnemyStateType.Debuff, false);
        else if (debuff_Types.ContainsKey(Debuff_Type.Stun))
            animationController.SetAnimationWithState(EnemyStateType.Idle, false);
    }

    protected void Stop_Enter()
    {
        //아무것도 안 해줌
    }
    #endregion

    /// <summary>
    /// 일정 주기마다 공격을 반복함
    /// </summary>
    protected virtual IEnumerator AttackLoop()
    {
        float enemyAttackDelay = EnemyStat.EnemyAttackSpeedResult(_EnemyStat); //몬스터 공격속도
        animationController.SetSpeed(EnemyStat.EnemyAttackSpeedResult(_EnemyStat) / attackSpeedNum);

        WaitForSeconds delay = new WaitForSeconds(1f / enemyAttackDelay);

        while (stateMachine.State == EnemyStateType.Attack) //공격타입에따라 공격 변경
        {
            switch (_EnemyStat.AttackType)
            {
                case Utill_Enum.Enum_AttackType.None:
                    Game.Debbug.Debbuger.Debug($"Attack type is None");
                    yield break;
                case Utill_Enum.Enum_AttackType.Melee:
                case Utill_Enum.Enum_AttackType.Range:
                    //공격 불가하다면
                    if(!attackState.CheckAttack())
                    {
                        ChangeFSMState(EnemyStateType.Move);
                        yield break;
                    }
                    animationController.SetAnimationWithState(EnemyStateType.Attack);

                    SoundManager.Instance.PlayAudio(AttackFx);
                    yield return delay;
                    break;
                case Utill_Enum.Enum_AttackType.Spell:
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// attack animation clip에서 이벤트로 호출됨
    /// </summary>
    public void OnAttackAnimation()
    {
        if (isHunterDie) return;
        if (_EnemyStat.AttackType == Utill_Enum.Enum_AttackType.None || _EnemyStat.AttackType == Utill_Enum.Enum_AttackType.Spell)
        {
            Game.Debbug.Debbuger.Debug($"attackState doesn't has OnAttackAnimation.");
            return;
        }

        attackState.OnAttackAnimation();
    }

    /// <summary>
    /// 데미지를 받았을 때 처리
    /// </summary>
    /// <param name="info">받은 데미지의 정보</param>
    public void Damaged(DamageInfo info)
    {
        if (isHunterDie) return;
        Offender = info.sender; //해당 몹을 누가 잡았는지 저장
        Hunter huter = ((IHunterDamageAble)Offender).DamageAbleHunter;
        //헌터의 방향을 구해서 현터 정방향에서 제한 각도로 이펙트 터지게
        HitEffect hiteffect = ObjectPooler.SpawnFromPool(Tag.EnemyHitEffect, transform.position).GetComponent<HitEffect>();
        hiteffect.OnMonsterHit(huter.transform.rotation.eulerAngles.y);
        hiteffect.gameObject.SetActive(true);
        if (stateMachine.State == EnemyStateType.Die)
        {
            return;
        }
        //즉사
        IntanceKill(info.isInstanceKillBoss);

        //cc기 먼저 계산 이부분을 상태 패턴 전환으로 나중에 바꿔야함
        CheckDebuffs(info.isStun,info.isFreezing, info.isPosion, info.isSlow ,info.isElectric, info.isChain);

        //데미지 텍스트 띄움
        //회피
        if (info.isdodge && GameManager.Instance.IsDamageOn)
        {
            Vector3 pos = Utill_Standard.Vector3Zero;
            pos.y = 1;

            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Dodge, transform.position + pos);
            return;
        }

        int infodmg = (int)info.damage;
        int infoaddDmg = (int)info.addDamage;

        //데미지 공식 테스트 기능도 포함..
        if (info.isPosion != null)
        {
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.PoisonDamage, transform.position, infodmg, infoaddDmg);
        }
        else if (info.isCoup && info.isCri)
        {
            NoticeManager.Instance.SpawnNoticeText(transform.position, infodmg, infoaddDmg, (int)info.CoupDamage, (int)info.CoupAddDamage);
        }
        else if (info.isCoup)
        {
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.CoupDamage, transform.position, (int)info.CoupDamage, (int)info.CoupAddDamage);
        }
        else if (GameManager.Instance.IsDamageOn)
        {
            if (info.isMagic)
            {
                //크리티컬확인
                if (info.isCri)
                {
                    NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.MagicDamage, transform.position, infodmg, infoaddDmg);
                }
                else
                {
                    NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.MagicDamage, transform.position, infodmg, infoaddDmg);
                }

            }
            else
            {
                //크리티컬확인
                if (info.isCri)
                {
                    NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.CriDamage, transform.position, infodmg, infoaddDmg);
                }
                else
                {
                    NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.NomalDamage, transform.position, infodmg, infoaddDmg);
                }
            }
        }


        // EnemyStat.SetminusgeHp(_EnemyStat, info.damage + info.addDamage);
        SoundManager.Instance.PlayAudio("Demon@Damaged");




        //HP 감소
        CurHp -= info.damage + info.addDamage + info.CoupDamage + info.CoupAddDamage;
        enemyhpuiview.UpdateHpBar(_EnemyStat.HP, CurHp);

        _EnemyStat.CurrentHp = (CurHp < 0) ? 0 : _EnemyStat.CurrentHp;

        //사망 처리
        if (CurHp <= 0)
        {
            //적처지시 hp확률....
            ActiveHpObject(false);

            //헌터의 적처지시 확률 계산해서 헌터의 체력 갱싱
            bool ishpheal;
            float Hppersent = HunterStat.GetHpPersent(huter._UserStat);
            ishpheal  = Utill_Math.Attempt(Hppersent);
            if (ishpheal) //hp힐가능
            {
                HunterStat.SetPlusHp(huter._UserStat, huter._UserStat.HpDuration);
                //특정사운드도 ..
                SoundManager.Instance.PlayAudio("HPRegenOnEnemyKill");
            }

            //일격으로 사망시 액션 호출
            if (Offender is IHunterDamageAble && info.CoupDamage > 0)
            {
                Hunter hunter = (Offender as IHunterDamageAble).DamageAbleHunter;
                if(hunter.OnCoupDamageAction != null)
                hunter.OnCoupDamageAction.Invoke(hunter);
            }
            //치명타로 사망시 액션 호출
            if (Offender is IHunterDamageAble && info.isCri == true && info.damage > 0)
            {
                Hunter hunter = (Offender as IHunterDamageAble).DamageAbleHunter;
                if(hunter.OnCriDamageAction != null)
                hunter.OnCriDamageAction.Invoke(hunter);
            }

            if (gameObject.activeInHierarchy)
                stateMachine.ChangeState(EnemyStateType.Die, StateTransition.Overwrite);


        }
    }


    public void CheckDebuffs(StunStatusEffect stun, FrozenStatusEffect Frozen, PosionStatusEffect Postion, SlowStatusEffect Slow , ElectricEffect electric, ChainLightningEffect chainLightning)
    {
        if (stun != null)
        {
            StatusEffectList.Add(stun);
            ApplyStatusEffect(stun);
          
        }
        if (Frozen != null)
        {
            StatusEffectList.Add(Frozen);
            ApplyStatusEffect(Frozen);
           
        }
        if (Postion != null)
        {
            StatusEffectList.Add(Postion);
            ApplyStatusEffect(Postion);
           
        }
        if (Slow != null)
        {
            StatusEffectList.Add(Slow);
            ApplyStatusEffect(Slow);
      
        }
        if (electric != null)
        {
            StatusEffectList.Add(electric);
            ApplyStatusEffect(electric);
        }
        if (chainLightning != null)
        {
            StatusEffectList.Add(chainLightning);
            ApplyStatusEffect(chainLightning);
        }
    }


    /// <summary>
    ///  투사체를 맞을 수 있는지 체크
    /// </summary>
    /// <param name="maxHit">최대 피격 가능 수(-1이면 무조건 true 반환)</param>
    /// <returns>투사체 피격가능 여부 체크</returns>
    public bool CheckProjectileHit(int projectileId,int maxHit)
    {
        if (maxHit == -1)
        {
            return true;
        }


        if (!projectileHitDic.ContainsKey(projectileId))
        {
            projectileHitDic.Add(projectileId, 0);
        }
        if (projectileHitDic[projectileId] >= maxHit)
        {
            return false;
        }

        projectileHitDic[projectileId]++;

        //Game.Debbug.Debbuger.Debug($"projectileId{projectileId}/{projectileHitDic[projectileId]}/ (max hit) : {maxHit}");

        if(projectileHitDic.Count >= 100) //이 상수 고치면 HunterAttackState SpawnArrow 부분도 수정 필요
        {
            int lastShhotNum = projectileHitDic[projectileId];
            ClearProjectileHit();
            projectileHitDic.Add(projectileId, lastShhotNum); //마지막 정보는 저장 (아직 맞는 중일수도 있기 때문)
        }

        return true;
    }

    /// <summary>
    /// 최대치 체크 없이 딕셔너리에 값을 더해줌
    /// </summary>
    public void AddProjectileHit(int projectileId, int addValue)
    {
        if (!projectileHitDic.ContainsKey(projectileId))
        {
            projectileHitDic.Add(projectileId, addValue);
        }
        else
            projectileHitDic[projectileId]+= addValue;
    }

    public void ClearProjectileHit()
    {
        //Game.Debbug.Debbuger.Debug($"clear");
        projectileHitDic.Clear();
    }

    public override void ApplyStatusEffect(StatusEffect effect)
    {
        base.ApplyStatusEffect(effect);

        // 상태 효과의 종류에 따라 특별한 처리를 할 수 있음
        if (effect.StatusType == Utill_Enum.Debuff_Type.Stun)
        {
            // 빙결 상태 효과를 적용하는 로직 추가
            // 스턴 상태 효과를 적용하는 로직 추가
            ChangeFSMState(Utill_Enum.EnemyStateType.Debuff); // 예시로 상태 변경 메서드 호출
            effect.ApplyEffect(this);

        }
        if (effect.StatusType == Utill_Enum.Debuff_Type.Freezing)
        {
            // 빙결 상태 효과를 적용하는 로직 추가
            // 스턴 상태 효과를 적용하는 로직 추가
            ChangeFSMState(Utill_Enum.EnemyStateType.Debuff); // 예시로 상태 변경 메서드 호출
            effect.ApplyEffect(this);

        }
        if (effect.StatusType == Utill_Enum.Debuff_Type.Posion)
        {
            // 빙결 상태 효과를 적용하는 로직 추가
            effect.ApplyEffect(this);

        }
        if (effect.StatusType == Utill_Enum.Debuff_Type.Slow)
        {
            // 빙결 상태 효과를 적용하는 로직 추가
            effect.ApplyEffect(this);

        }
        if (effect.StatusType == Utill_Enum.Debuff_Type.electricEffect)
        {
            // 빙결 상태 효과를 적용하는 로직 추가
            effect.ApplyEffect(this);
        }
        if (effect.StatusType == Utill_Enum.Debuff_Type.ChainLightning)
        {
            // 빙결 상태 효과를 적용하는 로직 추가
            effect.ApplyEffect(this);

        }



        // 추가적인 상태 효과 처리 가능
    }

    // Enemy 특화 상태 효과를 제거하는 메서드
    public override void RemoveStatusEffect(StatusEffect type)
    {
        base.RemoveStatusEffect(type);
        if (type != null)
        {
            StatusEffectList.Remove(type); // 상태 효과 리스트에서 제거
        }
    }

    //즉사함수

    public void IntanceKill(bool isInstanceKill)
    {
        //즉사
        if (isInstanceKill)
        {
            //특수한 소리 재생
            SoundManager.Instance.PlayAudio("InstantKillBossOnBasicAttack");
            //상탭 죽음
            ChangeFSMState(EnemyStateType.Die);
        }
    }
}
