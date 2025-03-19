using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-25
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 모든 스킬의 베이스 스크립트
/// </summary>
public abstract class BaseSkill : MonoBehaviour, ISkill
{
    [Header("--------------공통변수--------------")]
    #region 공통변수

    [Header("스킬 이름")]
    [SerializeField] protected string skillName;
    public string SkillName { get { return skillName; } set { skillName = value; } }

    [Header("스킬 데미지 기반")]
    [SerializeField] protected Utill_Enum.AttackDamageType skillDamageBase;
    public Utill_Enum.AttackDamageType SkillDamageBase { get => skillDamageBase; set { skillDamageBase = value; } }

    [Header("스킬 데미지 계수")]
    [SerializeField] protected float skillDamageCoefficient;
    public float SkillDamageCoefficient { get => skillDamageCoefficient; set { skillDamageCoefficient = value; } }

    [Header("스킬 데미지 타입")]
    [SerializeField] protected Utill_Enum.AttackDamageType skillDamageType;
    public Utill_Enum.AttackDamageType SkillDamageType { get { return skillDamageType; } set { skillDamageType = value; } }

    [Header("스킬 쿨타운")]
    [SerializeField] protected int skillCoolDown;
    public int SkillCoolDown { get { return skillCoolDown; } set { skillCoolDown = value; } }

    [Header("스킬 소모 마나")]
    [SerializeField] protected float skillUseMP;
    public float SkillUseMP { get { return skillUseMP; } set { skillUseMP = value; } }

    [Header("스킬 효과 지속시간")]
    [SerializeField] protected float _SkillDuration;
    public float SkillDuration
    {
        get { return _SkillDuration; }
        set { _SkillDuration = value; }
    }


    [Header("스택형 스킬 여부")]
    [SerializeField] protected bool _isStackable;
    public bool IsStackable { get { return _isStackable; } set { _isStackable = value; } }

    public virtual event System.Action<int> OnStackChanged;
    public virtual event Action<float,bool> OnActiveSecondChanged;

    [Header("스킬시전위치 타입")]
    [SerializeField] protected Utill_Enum.SkillCastingPositionType _SkillCastingPositionType;
    public Utill_Enum.SkillCastingPositionType SkillCastingPositionType { get { return _SkillCastingPositionType; } set { _SkillCastingPositionType = value; } }
    #endregion

    #region 에디터 변수
    //몬스터체크 변수
    [HideInInspector] public float EnemyCheckRadius;
    //RelativeToCamera
    [HideInInspector] public float PosCamX;
    [HideInInspector] public float PosCamY;
    [HideInInspector] public float PosCamZ;

    [HideInInspector] public float AreaXscale;
    [HideInInspector] public float AreaYscale;
    [HideInInspector] public float AreaZscale;
    [HideInInspector] private float _value;
     public float Value
    {
        get { return _value; } set { _value = value;  }
    }
    //SkillDistanceToHunter
    [HideInInspector] public float skillDiameter;
    public float SkillDiameter { get { return skillDiameter; } set { skillDiameter = value; } }
    public bool TargetDirection;

    //SkillRangeToHunter
    [HideInInspector] public float skillRange;
    public float SkillRange { get { return skillRange; } set { skillRange = value; } }

    //4번째 타입 대상을 지정..
    [HideInInspector]
    public List<string> FindUnitName;
    #endregion




    
    protected Hunter hunter;
    public Hunter _Hunter
    {
        get { return hunter; } set { hunter = value; }
    }

    protected List<Hunter> hunters;
    public List<Hunter> _Hunters
    {
        get { return hunters; }
        set { hunters = value; }
    }
    private int upgradeAmount = 0; //스킬 강화 횟수
    public int _upgradeAmount { get => upgradeAmount; set => upgradeAmount = value; }

    public virtual float DamageRadius_ { get => 0; set => value = 0; }
    public virtual float SkillRange_ { get => 0; set => value = 0; }

    protected bool isInit = false;

    private bool isUseSkill;
    public bool IsUseSkill
    {
        get { return isUseSkill; }
        set { isUseSkill = value; }
    }

    public bool isActiveSkill { get; set; }

    private bool _ismtion;
    public bool isMotion
    {
        get
        {
            return _ismtion;
        }
        set
        {
            _ismtion = value;
        }
    }

    public abstract void Execute(Utill_Enum.SubClass subclasstype);

    public abstract void Upgrade(Utill_Enum.SubClass subclasstype);

    public virtual void Init_Skill() { }

    public virtual bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, SkillUseMP); //마나감소
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

    #region SpawnArea
    //이펙트 떨어질 좌표구하는 함수
    protected virtual (Vector3 pos, List<Entity> entitylist) SpawnSkillArea(Utill_Enum.SkillCastingPositionType SkillCastingPositionType)
    {
        Vector3 pos = Utill_Standard.Vector3Zero;
        List<Entity> list = new List<Entity>();
        switch (SkillCastingPositionType)
        {
            case Utill_Enum.SkillCastingPositionType.RelativeToCamera:
                pos = RelativeToCamera();
                break;
            case Utill_Enum.SkillCastingPositionType.SkillDiameterToHunter:
                pos = SkillDistanceToHunter();
                break;
            case Utill_Enum.SkillCastingPositionType.SkillRangeToHunter:
                var Skillposenemylist = SkillRangeToHunter();
                pos = Skillposenemylist.RangeToHunterPos;
                list = Skillposenemylist.list;
                break;
            case Utill_Enum.SkillCastingPositionType.None:
                pos = None();
                break;
            case Utill_Enum.SkillCastingPositionType.FindSKillUnitList:
                list = FindKillUnitList();
                break;

        }

        return (pos, list);
    }

    protected virtual List<Entity> FindKillUnitList()
    {
        List<Entity> list = new List<Entity>();
        if (FindUnitName.Count > 0) //String 이름 리스트
        {
            foreach (var item in FindUnitName) //stringlist를 순회하며 해당이름에 맞는 클래스를 업캐스팅으로 저장
            {
                Entity entity = new Entity();
                entity = DataManager.Instance.Hunters.Find(c => c.name == item);
                list.Add(entity);
            }
        }

        return (list);
    }

    protected virtual Vector3 RelativeToCamera() //이펙트 떨어질 좌표구하는 함수
    {
        //지정된 위치안의 넓이에서 랜덤 값에 이펙트생성
        Vector3 campos = Utill_Standard.Vector3Zero;
        //검사기준을 카메라 중앙...으로..
        campos.x = PosCamX;
        campos.y = PosCamY;
        campos.z = GameManager.Instance.MainCam.transform.position.z + PosCamZ;

        // 범위에서 램덤 값을 찾아 내야합니다.
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(campos.x - AreaXscale / 2, campos.x + AreaXscale / 2),
            0.0f,
            UnityEngine.Random.Range(campos.z - AreaZscale / 2, campos.z + AreaZscale / 2)
        );
        return randomPosition;
    }

    protected virtual Vector3 SkillDistanceToHunter()
    {
        Vector3 DistancePos = Utill_Standard.Vector3Zero;

        if (hunter == null)
        {
            return DistancePos;
        }

        if (TargetDirection)//hunter 변수가 바라보는 앞방향 + SkillDistance 변수 값에 이펙트 생성
        {
            DistancePos = hunter.transform.position + hunter.transform.forward * SkillDiameter;
        }
        else  //hunter 변수의 바라보는 방향을 신경쓰지 않는 앞방향 + SkillDistance 변수값에 이펙트생성
        {
            // 헌터의 전방을 무시하고 월드 공간의 Z축 방향으로 계산
            DistancePos = hunter.transform.position + Vector3.forward * SkillDiameter;
        }
        return DistancePos;
    }

    protected virtual (Vector3 RangeToHunterPos, List<Entity> list) SkillRangeToHunter()
    {
        Vector3 RangeToHunterPos = Utill_Standard.Vector3Zero;
        Vector3 campos = Utill_Standard.Vector3Zero;
        LayerMask layer;
        List<Entity> List;
        //플레이어 기준 반경 넓이 안에서 적찾아낸후 적의 좌표로 지정
        //반경넓이 반지름변수는 SkillRange
        // 플레이어 기준 반경 넓이 안에서 적찾아내기
        layer = Layer.GetLayerMask(Layer.Enemy);

        if (hunter == null)
        {
            campos.x = 0; campos.y = 0; campos.z = GameManager.Instance.MainCam.transform.position.z;
            List = Utill_Standard.FindAllObjectsInRadius<Entity>(campos, SkillRange, layer);
        }
        else
        {
            List = Utill_Standard.FindAllObjectsInRadius<Entity>(hunter.transform.position, SkillRange, layer);
        }

        if (List.Count > 0)
        {
            // 무작위 인덱스를 선택하여 해당 콜라이더의 위치를 할당
            int randomIndex = UnityEngine.Random.Range(0, List.Count);
            RangeToHunterPos = List[randomIndex].transform.position;
        }
        else
        {
            Debbuger.Debug("No enemies found within range.");
            return (RangeToHunterPos, List);
        }

        return (RangeToHunterPos, List);
    }

    protected virtual Vector3 None()
    {
        Vector3 vector3 = Utill_Standard.Vector3Zero;
        if (hunter == null)
        {

        }
        else
        {
            vector3 = hunter.transform.position;
        }

        return vector3;
    }
    public virtual bool CheackEnemyArea(float _radlus)
    {
        return true;
    }

    (Vector3 pos, List<Entity> entitylist) ISkill.SpawnSkillArea(Utill_Enum.SkillCastingPositionType SkillCastingPositionType)
    {
        return (Vector3.zero,null);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (GameManager.Instance == null)
            return;
        if (GameManager.Instance.MainCam == null)
            return;

        // 카메라 위치 가져오기
        Vector3 campos = new Vector3(PosCamX, PosCamY, GameManager.Instance.MainCam.transform.position.z);

        // 범위 중앙 지점 계산
        Vector3 center = new Vector3(campos.x, 0, campos.z);

        // 기즈모 색상 설정
        Gizmos.color = Color.red;

        // 사면체를 그리기 위해 네 모서리 계산
        Vector3 frontLeft = new Vector3(center.x - AreaXscale / 2, 0, center.z - AreaZscale / 2);
        Vector3 frontRight = new Vector3(center.x + AreaXscale / 2, 0, center.z - AreaZscale / 2);
        Vector3 backLeft = new Vector3(center.x - AreaXscale / 2, 0, center.z + AreaZscale / 2);
        Vector3 backRight = new Vector3(center.x + AreaXscale / 2, 0, center.z + AreaZscale / 2);

        // 사면체를 그리기 위해 네 모서리 계산
        Vector3 frontLeft1 = new Vector3(center.x - AreaXscale / 2, 2, center.z - AreaZscale / 2);
        Vector3 frontRight1 = new Vector3(center.x + AreaXscale / 2, 2, center.z - AreaZscale / 2);
        Vector3 backLeft1 = new Vector3(center.x - AreaXscale / 2, 2, center.z + AreaZscale / 2);
        Vector3 backRight1 = new Vector3(center.x + AreaXscale / 2, 2, center.z + AreaZscale / 2);

        // 사면체 그리기
        Gizmos.DrawLine(frontLeft, frontRight);
        Gizmos.DrawLine(frontRight, backRight);
        Gizmos.DrawLine(backRight, backLeft);
        Gizmos.DrawLine(backLeft, frontLeft);

        // 사면체 높이 그리기
        Gizmos.DrawLine(frontLeft, new Vector3(frontLeft.x, 2, frontLeft.z));
        Gizmos.DrawLine(frontRight, new Vector3(frontRight.x, 2, frontRight.z));
        Gizmos.DrawLine(backLeft, new Vector3(backLeft.x, 2, backLeft.z));
        Gizmos.DrawLine(backRight, new Vector3(backRight.x, 2, backRight.z));

        // 사면체 그리기
        Gizmos.DrawLine(frontLeft1, frontRight1);
        Gizmos.DrawLine(frontRight1, backRight1);
        Gizmos.DrawLine(backRight1, backLeft1);
        Gizmos.DrawLine(backLeft1, frontLeft1);
    }
    #endregion
}
