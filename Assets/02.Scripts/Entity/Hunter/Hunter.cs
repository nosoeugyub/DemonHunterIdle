using MonsterLove.StateMachine;
using NSY;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Game.Debbug;
using static Utill_Enum;
using System;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 헌터 객체를 전체적으로 관리
/// FSM 외부 에셋 사용함. git 링크
/// https://github.com/thefuntastic/Unity3d-Finite-State-Machine?tab=readme-ov-file
/// </summary>
public class Hunter : Entity, IHunterDamageAble
{
    [Header("Mpuiview")]
    public MpUiView mpuiview;
    [Header("Hpuiview")]
    public HpUiView hpuiview;
    [SerializeField] private RectTransform HPUIPOS;
    [SerializeField] private Vector3 Hunterhpuiviewpos;
    [SerializeField] private Vector2 Hunterhpuiviewsize;

    [Header("NavMesh")]
    public NavMeshSurface NavmeshSufece;
    [SerializeField]
    private NavMeshAgent navmeshAgent;
    public NavMeshAgent NavMeshAgent { get { return navmeshAgent; } }
    public Rigidbody Rigid {  get { return rigid; } }
    public Transform FireTransform {  get { return fireTransform; } }
    public CapsuleCollider Col {  get { return col; } }

    [Space(20)]

    [Header("검색할 범위")]
    [SerializeField]
    private float searchRadius = 7f; // 검색할 범위
    public float SearchRadius { get { return searchRadius; } }
    [SerializeField]
    private LayerMask searchLayer;   // 검색할 레이어
    public LayerMask SearchLayer { get { return searchLayer; } }

    [SerializeField]
    private Transform fireTransform = null;  //공격시 화살 생성하는 위치
    [Header("게임 시작 후 몇초 후에 움직일 것인지")]
    [SerializeField]
    private float moveStartDelay = 1f;
    [Header("조이스틱 이동 시 x축 움직임 제한 범위")]
    [SerializeField]
    private float limitXRange = 3.8f;
    public float LimitXRange { get { return limitXRange; } }

    [SerializeField]
    private FloatingJoystick joyStick = null;
    public FloatingJoystick JoyStick { get { return joyStick; } }

    [Header("화살 사이의 각도")]
    public float shotAngle = 30f;

    private StateMachine<HunterStateType> stateMachine;

    private float skillAnimationSpeed = 1f; //스킬 시전시 애니메이션 스피드

    private bool isMagicAtk;
    public bool IsMagicAtk
    {
        get { return isMagicAtk; }
        set { isMagicAtk = value; }
    }

    [SerializeField]
    private TestHunterStat testHunterStat; //스탯 계산이랑 상관 없는 테스트용 헌터 스텟(치트 유무 확인 말고는 접근해선 안 됨)

    public TestHunterStat TestHunterStat
    {
        get {  return testHunterStat; }
        set {  testHunterStat = value; }
    }

    internal void SetHpUiPostionY(float v)//해당 hpui의 위치y값을 조정하는 함수
    {
        // 현재 RectTransform의 위치를 가져옴
        Vector2 currentPosition = HPUIPOS.anchoredPosition;

        // y 값을 업데이트하고, x 값은 유지
        currentPosition.y = v;

        // 변경된 위치를 RectTransform에 다시 설정
        HPUIPOS.anchoredPosition = currentPosition;
    }

    private HunterStat orginstat;//기본적으로 사용할 데이터 프로퍼티 스텟연산은 여기에 해주고 UserStat에 할당해주는방삭 Why? test스텟때문에
    public HunterStat Orginstat
    {
        get { return orginstat; }
        set { orginstat = value; }
    }

                               //연산관련 로직에는 사용해선안됨 오직 Ui표시용일때나 써야함
    private HunterStat userstat; //최종적으로 사용할데이터 프로퍼티
    public HunterStat _UserStat
    {
        get { return userstat; } set { userstat = value; }
    }


    private Vector3 moveVec = Utill_Standard.Vector3Zero;
    public Vector3 MoveVec
    {
        get { return moveVec; }
        set { moveVec = value; }
    }

    private Vector3 firstPos = Utill_Standard.Vector3Zero;

    private bool isLimitMove = false;
    public bool IsLimitMove { get { return isLimitMove; } }

    private Vector2 limitGroundX = Utill_Standard.Vector2Zero;
    private Vector2 limitGroundZ = Utill_Standard.Vector2Zero;
    public Vector2 LimitGroundX { get { return limitGroundX; } }
    public Vector2 LimitGroundZ { get { return limitGroundZ; } }

    protected Dictionary<HunterStateType, HunterState> stateDict = new();
    protected HunterAttackState attackState = null;
    protected HunterAnimationController animationController = null;
    protected CostumeStlyer costumeStlyer = null; //현재 모델의 코스튬 스타일러
    /// <summary>
    /// 현재 인게임상 헌터의 CostumeStlyer.
    /// UI헌터와 인게임 헌터 둘 다 처리해야할 경우에는 CostumeStlyer 속 static 함수들을 사용해야함
    /// </summary>
    public CostumeStlyer GetCostumeStlyer => costumeStlyer;


    //IHunterDamageAble 인터페이스
    public Hunter DamageAbleHunter { get { return this; } }
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
            return stateMachine.State != HunterStateType.Die && canGetDamage;
        }
    }

    private bool canGetDamage = true; //fsm기준 제외 다른 데미지를 받을 수 없는 기준 (휴식상태 등)

    //평타로 투사체를 추가할 헌터일 시 투사체의 이름
    private string projectileName = string.Empty;
    public string ProjectileName
    {
        get { return projectileName; }
        set { projectileName = value; }
    }

    // 이전 프레임에서의 치트 모드 여부
    private bool prevCheatMode;

    float shiedHP;
    public float ShiedHP
    {
        get { return shiedHP; }
        set { shiedHP = value; }
    }

    float maxHp;
    public float MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }

    float mp;
    public float MP
    {
        get { return mp; }
        set { mp = value; }
    }

    private float power;
    public float Power
    {
        get { return power; }
        set { power = value; }
    }

    private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    private float attackSpeed;
    public float AttackSpeed
    {
        get { return attackSpeed; }
        set { attackSpeed = value; }
    }
    
    private float xAxisOffset; //헌터가 가야할 x축 좌표
    public float XAxisOffset
    {
        get { return xAxisOffset; }
        set { xAxisOffset = value; }
    }

    private bool isDie = false; //사망시 중복 실행 막기위한 bool

    private bool isForcus;
    public bool IsForcus
    {
        get { return isForcus; }
        set { isForcus = value; }
    }

    private bool canMoveJoystick = true; //조이스틱으로 헌터 이동 가능 여부
    public bool CanMoveJoystick
    {
        get { return canMoveJoystick; }
        set { canMoveJoystick = value; }
    }

    public Action<Hunter> OnDieAction = null; //사망시 호출될 Action
    public Action<Hunter> OnShieldEndAction = null; //쉴드 다 썻을시 호출될 Action
    public Action<Hunter> OnDodgeAction = null; //회피 성공시 호출될 Action
    public Action<Hunter> OnNormalAttackAction = null; //평타시 호출될 Action(임시)
    public Action<Hunter> OnCriDamageAction = null; //적을 치명타 데미지로 죽였을시 호출될 Action
    public Action<Hunter> OnCoupDamageAction = null; //적을 일격 데미지로 죽였을시 호출될 Action
    public Action<Hunter, DamageInfo> OnGuardianShieldAction = null; //가디언 수호 스킬 발동시 호출될 Action

    private Coroutine attackLoop = null; //공격 코루틴

    private void Awake()
    {
        //FSM 세팅
        stateMachine = StateMachine<HunterStateType>.Initialize(this, HunterStateType.Stop);
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
        GameEventSystem.GameRestModeSequence_SendGameEventHandler += RestModeSequence;
    }


    private void OnEnable()
    {
        //치트중인지 확인...
        prevCheatMode = TestHunterStat.isTestStat;
        //hpuiview.ActiveObject(true);
        //hpuiview.gameObject.SetActive(true);
        //추후 불필요할 시 삭제
        //GameEventSystem.GameBattleSequence_SendEventHandler += ((bool isBattle) =>
        //{
        //    if (stateMachine.State != HunterState.Move && !isBattle)
        //    {
        //        stateMachine.ChangeState(HunterState.Move, StateTransition.Overwrite);
        //    }
        //});
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnHpOnChanged += HandleHpOnChanged;
        }
    }

    public void OnDisable()
    {
        //hpuiview.ActiveObject(false);
        //hpuiview.gameObject.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnHpOnChanged -= HandleHpOnChanged;
        }
    }
    
    public void PlusShield(float value)
    {
        shiedHP += value;
    }

    public void MinusShield(float value)
    {
        shiedHP -= value;
    }
    public void InitShield()
    {
        shiedHP = 0;
    }
    /// <summary>
    /// 바 관련 UI 위치 옮겨 안 보이는 것 처럼 처리해줌
    /// hpuiview의 ActiveObject 참고함
    /// </summary>
    public void ActiveHpObject(bool isactive)
    {
        if (isactive)
        {
            HPUIPOS.localScale = Hunterhpuiviewsize;
            HPUIPOS.localPosition = Hunterhpuiviewpos;
        }
        else
        {
            HPUIPOS.transform.position = new Vector3(9999, 9999, 9999);
        }

    }
    private void HandleHpOnChanged(bool isHpOn)
    {
        ActiveHpObject(isHpOn);
        if (isHpOn)
        {
            hpuiview.UpdateHpBar(_UserStat.HP, _UserStat.CurrentHp);
            mpuiview.UpdateHpBar(_UserStat.MP, _UserStat.CurrentMp);
        }
  
    }

    private void Update()
    {
        if (TestHunterStat.isTestStat != prevCheatMode)
        {
            SetCheatStat();   
            // 현재 프레임의 치트 모드 값을 이전 프레임의 값으로 업데이트
            prevCheatMode = TestHunterStat.isTestStat;
        }
        else if(TestHunterStat.isTestStat)
        {
            float curHp = _UserStat.CurrentHp;
            float curMp = _UserStat.CurrentMp;
            //테스스텟중에서는 실시간으로 적용
            HunterStat _userstat = TestHunterStat.MakeUserStat();
            _UserStat = _userstat;

            //체력이 같지않으면 체력이 새롭게 변경되는거니깐 체력 리셋
            if (MaxHp != _UserStat.HP)
            {
                MaxHp = _UserStat.HP;
                _UserStat.CurrentHp = MaxHp;
            }
            else
            {
                //아니면 현재 체력 그대로
                _UserStat.CurrentHp = curHp;
            }
            //마나도 똑같이 리셋
            if(MP != _UserStat.MP)
            {
                MP = _UserStat.MP;
                _UserStat.CurrentMp = MP;
            }
            else
            {
                //아니면 현재 마나 그대로
                _UserStat.CurrentMp = curMp;
            }
        }

        //조이스틱 감지
        if (joyStick.isClick)
            JoyStickOn();
    }

    private void SetCheatStat()
    {
        // 치트 모드일 경우 치트 스텟을 사용
        if (TestHunterStat.isTestStat)
        {
            HunterStat _userstat = TestHunterStat.MakeUserStat();
            _UserStat = _userstat;
            //현제 체력 치트체력으로 동기화..
            HunterStat.SetHp(_UserStat, _UserStat.HP);
            HunterStat.SetMp(_UserStat, _UserStat.MP);
        }
        else
        {
            _UserStat = Orginstat;
            //체력이 같지않으면 체력이 새롭게 변경되는거니깐 체력 리셋
            if (MaxHp != _UserStat.HP)
            {
                MaxHp = _UserStat.HP;
                _UserStat.CurrentHp = MaxHp;
            }
            if (MP != _UserStat.MP)
            {
                MP = _UserStat.MP;
                _UserStat.CurrentMp = MP;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        // Set the color for the Gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        Gizmos.color = Color.red;
        if (_UserStat != null)
        {
            Gizmos.DrawWireSphere(transform.position, _UserStat.AttackRange);
        }

    }
    private void RestModeSequence(Utill_Enum.IdleModeRestCycleSequence sequence)
    {
        switch (sequence)
        {
            case IdleModeRestCycleSequence.Cinematic:
                ChangeFSMState(HunterStateType.MoveToRest);
                break;
            case IdleModeRestCycleSequence.Start:
                ChangeFSMState(HunterStateType.Resting);
                break;
            case IdleModeRestCycleSequence.End:
                break;
        }
    }

    private bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:

                if (TestHunterStat.isTestStat)//실시간 테스트 스텟을 반영하기위해 임시로 일단업데이트문에서 관리...
                {
                    HunterStat _userstat = TestHunterStat.MakeUserStat();
                    _UserStat = _userstat;
                    HunterStat.SetMaxHp(_UserStat);
                }
                else
                {
                    _UserStat = Orginstat;
                }
                hpuiview.gameObject.SetActive(true); //첫 설정시 HPview 활성화 되도록

                //헌터 타입에 따라 발사할 투사체의 이름을 설정
                ProjectileName = GetDefaultProjectileName();

                SetState();
                Setting(false,true);

                //스텟적용
                SetCheatStat();
                return true;
            case Utill_Enum.Enum_GameSequence.InGame:
                if(gameObject.activeInHierarchy)
                {
                    StartCoroutine(FSMStart());
                    SetHunterXAxis(false);
                    Vector3 startPos = new Vector3(XAxisOffset,firstPos.y,firstPos.z); //초기 위치 세팅
                    navmeshAgent.Warp(startPos);
                }
                else
                    ActiveHpObject(false);
                HunterPortraitSystem.Instance.InitEquipSetting(gameObject.activeInHierarchy,_UserStat.SubClass);
                return true;
        }
        return false;
    }

    /// <summary>
    /// 사용할 state Init
    /// </summary>
    protected virtual void SetState()
    {
        stateDict.Clear();
        stateDict.Add(HunterStateType.Move, new HunterMoveState(this));
        stateDict.Add(HunterStateType.JoystickMove, new HunterJoyStickMoveState(this));
        stateDict.Add(HunterStateType.Die, new HunterDieState(this));
        stateDict.Add(HunterStateType.Whirlwind, new HunterWhirlwindState(this));
        stateDict.Add(HunterStateType.MoveToRest, new HunterMoveToRestState(this));
        stateDict.Add(HunterStateType.Resting, new HunterRestingState(this));
        stateDict.Add(HunterStateType.CheatJoystickMove, new HunterCheatJoyStickMoveMoveState(this));
        
        switch (_UserStat.AttacckType)
        {
            case Utill_Enum.Enum_AttackType.None:
            case Utill_Enum.Enum_AttackType.Melee:
                attackState = new HunterMeleeAttackState(this);
                break;
            case Utill_Enum.Enum_AttackType.Range:
            case Utill_Enum.Enum_AttackType.Spell:
                attackState = new HunterRangeAttackState(this);
                break;
        }
        stateDict.Add(HunterStateType.Attack, attackState);
    }

    public string GetDefaultProjectileName()
    {

        switch (_UserStat.SubClass)
        {
            case SubClass.Archer:
                return Tag.Arrow;
            case SubClass.Mage:
                return Tag.SpellBall;
            case SubClass.Guardian: //수호자는 근접 공격임
                break;
        }
        return "";
    }
    public void SetHunterXAxis(bool isWarp = true)
    {
        if (GameDataTable.Instance.User.CurrentEquipHunter.IndexOf(Orginstat.SubClass) == -1) return;
        XAxisOffset = Utill_Math.GetDistancedValue(GameManager.Instance.hunterDistance, GameDataTable.Instance.User.CurrentEquipHunter.IndexOf(Orginstat.SubClass), GameDataTable.Instance.User.CurrentEquipHunter.Count);

        if (!isWarp) return;

        Vector3 startPos = new Vector3(XAxisOffset, firstPos.y, DataManager.Instance.MainCameraMovement.GetHuntersMidPosition().z); //초기 위치 세팅
        ResetPosition(startPos);
    }

    /// <summary>
    /// 시작/사망 시 수치 세팅
    /// </summary>
    public void Setting(bool startFSM = true,bool isResetHP = false)
    {
        //ui크기위치
        HPUIPOS.localScale = Hunterhpuiviewsize;
        HPUIPOS.localPosition = Hunterhpuiviewpos;

        //처음 위치 저장
        if (firstPos == Utill_Standard.Vector3Zero)
            firstPos = transform.position;

        //애니메이터 초기화
        animator.Rebind();
        if(animationController != null)
            animationController.UnBlockChangeAnimation();
        else
            animationController = new HunterAnimationController(animator);

        if(costumeStlyer == null)
        {
            //추후 인스펙터에서 넣어주는 방식으로 교체할지 고민 필요
            costumeStlyer = animator.gameObject.GetComponent<CostumeStlyer>();
        }    

        //사망시 처리한 내용 초기화
        col.enabled = true; 
        rigid.isKinematic = false;

        //navMesh 관련 초기화
        navmeshAgent.enabled = true;
        NavmeshSufece.BuildNavMesh();
        navmeshAgent.speed = speed * moveSpeedNum;
        navmeshAgent.updateRotation = false;

        skillAnimationSpeed = etcAnimSpeed;

        OnDieAction = null;
        OnShieldEndAction = null;
        OnDodgeAction = null;
        OnNormalAttackAction = null;
        OnGuardianShieldAction = null;
        OnCriDamageAction = null;
        OnCoupDamageAction = null;

        //스텟할당
        if (isResetHP) //만약 체력 초기화도 필요하다면 초기화
        {
            HunterStat.SetMaxHp(_UserStat);
            HunterStat.SetMaxMp(_UserStat);
            MaxHp = _UserStat.HP;
            _UserStat.CurrentHp = MaxHp;
            MP = _UserStat.CurrentMp;
        }
        //NSY작성 게임시작시 hp셋팅
        hpuiview.ActiveObject(true);
        hpuiview.UpdateHpBar(_UserStat.HP, _UserStat.CurrentHp);
        mpuiview.UpdateHpBar(_UserStat.MP, _UserStat.CurrentMp);
        HandleHpOnChanged(GameManager.Instance.IsHpOn);
        //hpbar 위치셋팅
        // 새로운 위치 설정
       

        //스텟할당
        Power = HunterStat.PhysicalPowerResult(orginstat);
        Speed = HunterStat.MoveSpeedResult(orginstat);
        AttackSpeed = HunterStat.AttackSpeedResult(orginstat);
        IsMagicAtk = Orginstat.AttackDamageType == Utill_Enum.AttackDamageType.Physical ? false : true;


        if (startFSM)
        {
            StopHunter();
            ChangeFSMState(HunterStateType.Move);
        }
    }

    /// <summary>
    /// 헌터가 데미지를 받을 수 있는지를 제어하는 변수
    /// </summary>
    public void SetCanGetDamage(bool canGetDamage)
    {
        this.canGetDamage = canGetDamage;
    }
    public void BlockChangeAnimation()
    {
        animationController.BlockChangeAnimation();
    }
    public void UnBlockChangeAnimation()
    {
        animationController.UnBlockChangeAnimation();
    }
    public void SetAnimationSpeed(float speed)
    {
        animationController.SetSpeed(speed);
    }
    public void SetSkillAnimationSpeed(float speed)
    {
        animationController.SetSpeed(speed);
    }
    /// <summary>
    /// 밖에서 애니메이션 제어하는 함수 (특별한 경우 아니면 사용 자제)
    /// </summary>
    public void SetAnimation(HunterStateType hunterState,bool isOn,int skillType = 0)
    {
        animationController.SetAnimationWithState(hunterState, isOn, skillType);
    }

    /// <summary>
    /// 초기 위치로 설정
    /// </summary>
    public void ResetPosition()
    {
        if(navmeshAgent.enabled)
            navmeshAgent.Warp(firstPos);
        else
            transform.position = firstPos;
    }
    /// <summary>
    /// 특정 위치로 위치로 설정
    /// </summary>
    public void ResetPosition(Vector3 resetPos)
    {
        if (navmeshAgent.enabled)
            navmeshAgent.Warp(resetPos);
        else
            transform.position = resetPos;
    }

    public void StopHunter()
    {
        Game.Debbug.Debbuger.Debug(gameObject.name + "is stopped");
        ChangeFSMState(HunterStateType.Stop, true);
    }

    /// <summary>
    /// 그라운드 기반으로 헌터 움직일 수 있는 범위 제한
    /// </summary>
    /// <param name="ground">제한하는 기반이 될 그라운드</param>
    /// <param name="extraMinXRange">추가적인 min z값 제한</param>
    /// <param name="extraMaxZRange">추가적인 max z값 제한</param>
    public void LimitHunterPos(Ground ground,float extraMinXRange = 0f, float extraMaxZRange = 0f)
    {
        isLimitMove = true; 
        Bounds bounds = ground.Plane.bounds;

        limitGroundX = new Vector2(bounds.min.x ,bounds.max.x);
        limitGroundZ = new Vector2(bounds.min.z - extraMinXRange, bounds.max.z + extraMaxZRange);
    }

    public void UnLimitHunterPos()
    {
        isLimitMove = false;
    }

    public void BlockJoystickMove()
    {
        canMoveJoystick = false;
    }
    public void UnBlockJoystickMove()
    {
        canMoveJoystick = true;
    }
    
    /// <summary>
    /// 페이드인/페이드 아웃 대기
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitHunter(float duration)
    {
        ChangeFSMState(HunterStateType.Stop, true);
        yield return new WaitForSeconds(duration);
        ChangeFSMState(HunterStateType.Move, true);
        GameEventSystem.GameBattleSequence_GameEventHandler_Event(false);
    }

    public float GetMoveSpeedByGameState()
    {
        if (GameStateMachine.Instance.CurrentState is NonCombatState)
        {
            return HunterStat.GroupMoveSpeedResult(_UserStat);
        }
        else if(GameStateMachine.Instance.CurrentState is CombatState)
        {
            var curState = GetState();
            //회전돌격을 사용중일 수 있는상태라면 추가 속도 퍼센트값까지 적용
            if (curState == HunterStateType.Whirlwind || curState == HunterStateType.JoystickMove)
                return HunterStat.MoveSpeedResult(_UserStat) + SkillManager.Instance.whirlwindRush.GetAdditionalWhirlwindMoveSpeed();
            return HunterStat.MoveSpeedResult(_UserStat);
        }
        return 0;
    }
 
    // fsm 코드 

    public void ChangeFSMState(HunterStateType state, bool isOverwrite = false)
    {
        //치트모드이고, 움직이려고 한다면
        if (CheatManager.Instance.HunterMoveOff && (state == HunterStateType.Move || state == HunterStateType.JoystickMove)) 
        {
            state = HunterStateType.CheatJoystickMove;
        }

        Game.Debbug.Debbuger.Debug(gameObject.name+" change state " + state.ToString());

        if (isOverwrite)
        {
            stateMachine.ChangeState(state,StateTransition.Overwrite);
        }
        else
            stateMachine.ChangeState(state);
    }
    public HunterStateType GetState()
    {
        return stateMachine.State;
    }
    public HunterStateType GetLastState()
    {
        if (stateMachine.LastStateExists == false)
            return HunterStateType.Stop;
        return stateMachine.LastState;
    }

    /// <summary>
    /// 게임 시작시 n초동안 멈춰있다가 움직임
    /// </summary>
    /// <returns></returns>
    private IEnumerator FSMStart()
    {
        StopHunter();
        yield return new WaitForSeconds(moveStartDelay);
        ChangeFSMState(HunterStateType.Move);
        GameEventSystem.GameBattleSequence_GameEventHandler_Event(false);
    }

    /// <summary>
    /// 여러가지 이유로 FSM 잘 동작하지 않을 때, FSM을 이전 state로 변경해주는 함수
    /// </summary>
    private void ResetFSM()
    {
        //없앤 후 세팅해줄 스테이트
        HunterStateType settingState = HunterStateType.Stop;
        if (stateMachine.LastStateExists)
        {
            settingState = stateMachine.LastState;
        }
        ChangeFSMState(settingState,true);
    }

    // StateName_MethodName (상태명_메서드명)으로 함수를 작성하면 State에 맞춰 실행됨
    #region FSM 코드
    protected void Move_Enter()
    {
        animationController.SetAnimationWithState(HunterStateType.Move, true);
        stateDict[HunterStateType.Move].Enter();
    }

    protected virtual void Move_Update()
    {
        if (stateMachine.State != HunterStateType.Move)
        {
            Game.Debbug.Debbuger.Debug("FSM is something wrong. Starting to reset...");
            ResetFSM();
        }
        //Game.Debbug.Debbuger.Debug("Move update");

        float tmpSpeed = GetMoveSpeedByGameState();
        //테스트 때문에 Update에서 처리
        animationController.SetSpeed(tmpSpeed / moveSpeedNum);
        navmeshAgent.speed = tmpSpeed;

        stateDict[HunterStateType.Move].Update();
    }

    protected void Move_Exit()
    {
        stateDict[HunterStateType.Move].Exit();
        animationController.SetAnimationWithState(HunterStateType.Move, false);
        animationController.SetSpeed(etcAnimSpeed);
    }

    protected void JoystickMove_Enter()
    {
        //Game.Debbug.Debbuger.Debug("JoyStick start");
        stateDict[HunterStateType.JoystickMove].Enter();

        animationController.SetAnimationWithState(HunterStateType.JoystickMove, true);
    }

    protected void JoystickMove_FixedUpdate()
    {
        if (stateMachine.State != HunterStateType.JoystickMove)
        {
            Game.Debbug.Debbuger.Debug("FSM is something wrong. Starting to reset...");
            ResetFSM();
        }

        //Game.Debbug.Debbuger.Debug("JoyStick update");
        animationController.SetSpeed(GetMoveSpeedByGameState() / moveSpeedNum);
        moveVec = new Vector3(joyStick.Horizontal, 0, joyStick.Vertical).normalized;

        stateDict[HunterStateType.JoystickMove].FixedUpdate();
    }

    protected void JoystickMove_LateUpdate()
    {
        stateDict[HunterStateType.JoystickMove].LateUpdate();
    }

    protected void JoystickMove_Exit()
    {
        //Game.Debbug.Debbuger.Debug(gameObject.name + ": JoyStick_Exit");
        stateDict[HunterStateType.JoystickMove].Exit();

        animationController.SetAnimationWithState(HunterStateType.JoystickMove, true);
        animationController.SetSpeed(etcAnimSpeed);
    }

    protected void CheatJoystickMove_Enter()
    {
        //Game.Debbug.Debbuger.Debug("JoyStick start");
        stateDict[HunterStateType.CheatJoystickMove].Enter();

        animationController.SetAnimationWithState(HunterStateType.CheatJoystickMove, true);
    }

    protected void CheatJoystickMove_FixedUpdate()
    {
        if (stateMachine.State != HunterStateType.CheatJoystickMove)
        {
            Game.Debbug.Debbuger.Debug("FSM is something wrong. Starting to reset...");
            ResetFSM();
        }

        //Game.Debbug.Debbuger.Debug("JoyStick update");
        animationController.SetSpeed(HunterStat.MoveSpeedResult(_UserStat) / moveSpeedNum);
        moveVec = new Vector3(joyStick.Horizontal, 0, joyStick.Vertical).normalized;

        stateDict[HunterStateType.CheatJoystickMove].FixedUpdate();
    }

    protected void CheatJoystickMove_LateUpdate()
    {
        stateDict[HunterStateType.CheatJoystickMove].LateUpdate();
    }

    protected void CheatJoystickMove_Exit()
    {
        //Game.Debbug.Debbuger.Debug(gameObject.name + ": JoyStick_Exit");
        stateDict[HunterStateType.CheatJoystickMove].Exit();

        animationController.SetAnimationWithState(HunterStateType.CheatJoystickMove, true);
        animationController.SetSpeed(etcAnimSpeed);
    }

    protected void Whirlwind_Enter()
    {
        float tmpSpeed = GetMoveSpeedByGameState();
        navmeshAgent.speed = tmpSpeed;
        stateDict[HunterStateType.Whirlwind].Enter();

        animationController.SetSpeed(SkillManager.Instance.whirlwindRush.WhirlwindAnimationSpeed);
    }
    protected void Whirlwind_Update()
    {
        stateDict[HunterStateType.Whirlwind].Update();
    }

    protected void Attack_Enter()
    {
        GameEventSystem.GameBattleSequence_GameEventHandler_Event(true);

        stateDict[HunterStateType.Attack].Enter();
        if(attackLoop != null)
        {
            StopCoroutine(attackLoop);
        }
        attackLoop = StartCoroutine(AttackLoop());
    }

    protected void Attack_Update()
    {
        if (stateMachine.State != HunterStateType.Attack)
        {
            Game.Debbug.Debbuger.Debug("FSM is something wrong. Starting to reset...");
            ResetFSM();
        }
        stateDict[HunterStateType.Attack].Update();
    }


    protected void Attack_Exit()
    {
        animationController.SetSpeed(etcAnimSpeed);
        if (attackLoop != null)
        {
            StopCoroutine(attackLoop);
        }
        stateDict[HunterStateType.Attack].Exit();
        animationController.SetAnimationWithState(HunterStateType.Attack, false);
    }

    protected void Die_Enter()
    {
        //스킬도 비활성화
        GameEventSystem.Send_GameHunterPortraitClickBtn_GameEventHandler(_UserStat.SubClass, false);
        //Game.Debbug.Debbuger.Debug("Die start");

        if (stateMachine.LastState == HunterStateType.Die) return;

        if (OnDieAction != null)
            OnDieAction.Invoke(this); //사망시

        bool isAllDie = GameDataTable.Instance.User.CurrentEquipHunter.Count == 1 && GameDataTable.Instance.User.CurrentEquipHunter.Contains(_UserStat.SubClass);

        GameDataTable.Instance.User.CurrentDieHunter.Add(_UserStat.SubClass);

        stateDict[HunterStateType.Die].Enter();
        animationController.SetAnimationWithState(HunterStateType.Die);

        if(isAllDie) //죽기 전에 카메라 움직임 멈춤
        {
            DataManager.Instance.MainCameraMovement.CanMove = false;
        }

        //헌터 장착 해제
        GameDataTable.Instance.User.CurrentEquipHunter.Remove(_UserStat.SubClass);

        //유저 죽음 델리게이트 호출
        if (isAllDie)
        {
            GameEventSystem.GameHunterDie_SendGameEventHandler_Event();
            GameEventSystem.GameAddRestCycle_GameEventHandler_Event(GameManager.Instance.IdleModeHunterDeathPenalty);
            return;
        }




        //장착 예약
        HunterPortraitSystem.Instance.hunterPortraitView.hunterPortraits[(int)_UserStat.SubClass].EquipAfterDuration(GameManager.Instance.hunterReviveDuration, _UserStat.SubClass);
        //아직 살아있는 다른 헌터 쿨다운 시작
        HunterPortraitSystem.Instance.hunterPortraitView.hunterPortraits[(int)orginstat.SubClass].CoolDownPortrait(GameManager.Instance.hunterDieCoolDownDuration);
        HunterPortrait portraits = HunterPortraitSystem.Instance.hunterPortraitView.hunterPortraits[(int)_UserStat.SubClass];
        //장착 시도 못하게끔 막음
        portraits.CoolDownPortrait(GameManager.Instance.hunterDieCoolDownDuration);
        //이후 처리는 애니메이션 이벤트로 (OnDieAnimation)
    }

    protected void Stop_Enter()
    {
        if(animationController == null)
            animationController = new HunterAnimationController(animator);
        animationController.SetAnimationWithState(HunterStateType.Stop, true);
    }

    protected void Skill_Enter()
    {
        animationController.SetSpeed(skillAnimationSpeed);
    }

    protected void Skill_Exit()
    {
        animationController.SetAnimationWithState(HunterStateType.Skill, false);
    }

    protected void MoveToRest_Enter()
    {
        animationController.SetAnimationWithState(HunterStateType.MoveToRest, true);
        stateDict[HunterStateType.MoveToRest].Enter();
    }

    protected void MoveToRest_Update()
    {
        if (stateMachine.State != HunterStateType.MoveToRest)
        {
            Game.Debbug.Debbuger.Debug("FSM is something wrong. Starting to reset...");
            ResetFSM();
        }

        float tmpSpeed = GetMoveSpeedByGameState();
        //테스트 때문에 Update에서 처리
        animationController.SetSpeed(tmpSpeed / moveSpeedNum);
        navmeshAgent.speed = tmpSpeed;

        stateDict[HunterStateType.MoveToRest].Update();
    }
    protected void MoveToRest_Exit()
    {
        stateDict[HunterStateType.MoveToRest].Exit();
        animationController.SetAnimationWithState(HunterStateType.MoveToRest, false);
        animationController.SetSpeed(etcAnimSpeed);
    }


    protected void Resting_Enter()
    {
        animationController.SetAnimationWithState(HunterStateType.Resting, true);
    }

    protected void Resting_Exit()
    {
        animationController.SetAnimationWithState(HunterStateType.Resting, false);
    }

    #endregion

    /// <summary>
    /// 일정 주기마다 공격을 반복함
    /// </summary>
    protected virtual IEnumerator AttackLoop()
    {

        while (stateMachine.State == HunterStateType.Attack)
        {
            //테스트 때문에 Update에서 처리
            float userAttackDelay = HunterStat.AttackSpeedResult(_UserStat); //유저 공격속도

            animationController.SetSpeed(userAttackDelay / attackSpeedNum);

            WaitForSeconds delay = new WaitForSeconds(1f / userAttackDelay);

            if (!attackState.CheckAttack())
            {
                ChangeFSMState(HunterStateType.Move);
                GameEventSystem.GameBattleSequence_GameEventHandler_Event(false);

                if (stateMachine.State != HunterStateType.Attack)
                    break;
            }

            animationController.SetAnimationWithState(HunterStateType.Attack);

            if (stateMachine.State != HunterStateType.Attack)
                break;
            yield return delay;
        }
    }

    /// <summary>
    /// attack animation clip에서 이벤트로 호출됨
    /// </summary>
    public void OnAttackAnimation()
    {
        if (IsForcus)
        {
            SoundManager.Instance.PlayAudio("Focus_Arrow");
        }
        else
        {
            SoundManager.Instance.PlayAudio(AttackFx);
        }
       
        attackState.OnAttackAnimation();
        if(OnNormalAttackAction != null)
            OnNormalAttackAction.Invoke(this);
    }

    /// <summary>
    /// die animation clip에서 이벤트로 호출됨
    /// </summary>
    public void OnDieAnimation()
    {
        StopHunter();
        if (GameDataTable.Instance.User.CurrentEquipHunter.Count <= 0)
        {
            gameObject.SetActive(true);
        }
        else
             gameObject.SetActive(false);
    }   

    /// <summary>
    /// 조이스틱을 클릭하였을 때 움직임
    /// </summary>
    public void JoyStickOn()
    {
        if (!canMoveJoystick || CheatManager.Instance.HunterMoveOff || stateMachine.State == HunterStateType.Die || GameStateMachine.Instance.CurrentState is NonCombatState) 
            return;

        ChangeFSMState(HunterStateType.JoystickMove, true);
    }

    public void Drain(int HpDrain,int MpDrain)
    {
        string hpDrainText = "";
        string mpDrainText = "";

        hpDrainText = $"+{HpDrain}";
        mpDrainText = $"+{MpDrain}";


        mpuiview.UpdateHpBar(Orginstat.MP, Orginstat.CurrentMp);
        hpuiview.UpdateHpBar(Orginstat.HP, Orginstat.CurrentHp);
        NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Drain, transform.position + (Vector3.right * 0.5f), hpDrainText, mpDrainText,false);
    }
    /// <summary>
    /// 데미지를 받았을 때 처리
    /// </summary>
    /// <param name="info">받은 데미지의 정보</param>
    public void Damaged(DamageInfo info)
    {
        if (stateMachine.State == HunterStateType.Die) return;

        int infodmg = (int)info.damage;
        int infoaddDmg = (int)info.addDamage;

        //데미지 텍스트 띄움
        #region 회피검사
        if (info.isdodge)
        {
           
            if (OnDodgeAction != null)
                OnDodgeAction.Invoke(this);
                return;
        }
        #endregion

        #region 반사검사
        bool isReflect =false;
        if (isReflect)
        {
            //반사연출하고 리턴
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Reflection, transform.position);
            return;
        }
        #endregion     
        if (OnGuardianShieldAction != null)
        {
            OnGuardianShieldAction.Invoke(this, info);
            return;
        }

        //데미지 공식 테스트 기능도 포함..
        if ( info.isPosion!=null )
        {
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.PoisonDamage, transform.position, infodmg, infoaddDmg);
        }
        else if(info.isCoup && info.isCri)
        {
            NoticeManager.Instance.SpawnNoticeText(transform.position, infodmg, infoaddDmg,(int)info.CoupDamage,(int)info.CoupAddDamage);
        }
        else if (info.isCoup)
        {
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.CoupDamage, transform.position, (int)info.CoupDamage, (int)info.CoupAddDamage);
        }
        else if (info.isMagic)
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

        float damage = info.damage + info.addDamage + info.CoupDamage + info.CoupAddDamage; // 총 데미지 계산
        float remainingDamage = damage; // 남은 데미지 초기화

        // 방어막 HP 감소
        if (shiedHP > 0)
        {
            if (shiedHP >= remainingDamage)
            {
                shiedHP -= remainingDamage;
                remainingDamage = 0; // 방어막으로 모든 데미지 흡수
            }
            else
            {
                remainingDamage -= shiedHP; // 방어막이 흡수한만큼 데미지 감소
                shiedHP = 0; // 방어막 파괴

                if (OnShieldEndAction != null)
                    OnShieldEndAction.Invoke(this);
            }
        }

        _UserStat.CurrentHp -= remainingDamage;
        _UserStat.CurrentHp = (_UserStat.CurrentHp < 0) ? 0 : _UserStat.CurrentHp;



        hpuiview.UpdateHpBar(_UserStat.HP, _UserStat.CurrentHp);

        //사망 처리
        if (_UserStat.CurrentHp <= 0)
        {
            //hpuiview.enabled = fals e;
            ActiveHpObject(false);
            if (gameObject.activeInHierarchy && stateMachine.State != HunterStateType.Die)
                ChangeFSMState(HunterStateType.Die, true);
        }
    }
}
