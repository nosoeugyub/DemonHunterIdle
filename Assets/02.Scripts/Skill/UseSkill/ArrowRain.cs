using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using Game.Debbug;

/// <summary>
/// 작성일자   : 2024-07-22
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 화살비 스킬
/// </summary>
public class ArrowRain : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정 변수
    [Header("화살비 발동 횟수")]
    [SerializeField] private int ArrowRainNumber;

    [Header("화살비 데미지 적용확률")]
    [SerializeField] private float ArrowRainHitChance;


    [Header("화살비 피해범위 시작값")]
    [SerializeField] private float InitialRadius;
    [Header("화살비 피해범위 최댓값")]
    [SerializeField] private float MaxRadius; 
    [Header("화살비 피해범위 최댓값까지 늘어나는 시간")]
    [SerializeField] private float ExpansionTime;


    [Header("화살비 시전 간격 랜덤 최소/최댓값")]
    [SerializeField] private float CastingIntervalMin;
    [SerializeField] private float CastingIntervalMax;

    [Header("스킬모션 이후 데미지 적용 딜레이")]
    [SerializeField] private float ImpactDelay;

    [Header("스킬모션 총 재생 시간")]
    [SerializeField] float AnimationApplyTime = 2f;

    [Header("이펙트사이즈")]
    [SerializeField] private float EffectSize;
    #endregion
    public float GetImpactDelay => ImpactDelay;
    public float GetArrowRainHitChance => ArrowRainHitChance;

    GameObject fx;
    private DamageInfo arrowRainDamageInfo = null;
    private ArrowRainData arrowRainData = null;

    private Coroutine currentCoroutine;

    public override float DamageRadius_ { get => MaxRadius; set => MaxRadius = value; } //임시

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StartShot();
    }

    public override void Init_Skill()
    {
        hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, false);
        hunter.SetAnimationSpeed(1);
        hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
        hunter.UnBlockJoystickMove();
        isMotion = false;
    }

    private void StartShot()
    {
        isMotion = true;
        if (AnimationApplyTime > 0) //애니메이션 적용시간이 0일시에는 아예 관련 로직 타지 않도록
        {
            hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, true, (int)Utill_Enum.SkillAnimationType.ArrowRain);
            hunter.ChangeFSMState(Utill_Enum.HunterStateType.Skill);

            float baseAnimationLength = 1.0f; // 기본 애니메이션 길이 
            float animationSpeed = (1 / AnimationApplyTime) / baseAnimationLength;

            // 애니메이션 속도 설정
            hunter.SetSkillAnimationSpeed(animationSpeed);
            hunter.BlockJoystickMove();
        }
        else //바로 그다음 함수 호출
            AnimEnd();
    }

    public void AnimEnd()
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(UseArrowRain());
    }

    private IEnumerator UseArrowRain()
    {
        isMotion = false;
        if (AnimationApplyTime > 0) //애니메이션이 실제 플레이 되었을 시에만 fsm 코드 호출
        {
            hunter.SetAnimation(Utill_Enum.HunterStateType.Skill, false);
            hunter.SetAnimationSpeed(1);
            hunter.ChangeFSMState(Utill_Enum.HunterStateType.Move);
            hunter.UnBlockJoystickMove();
        }
        //이펙트 설정 후 시작
        for (int i = 0; i < ArrowRainNumber; i++)
        {
            //타입정하기
            var skillposlist = SpawnSkillArea(SkillCastingPositionType);
            Vector3 SpawnEffectPos = skillposlist.pos;

            fx = ObjectPooler.SpawnFromPool(Tag.ArrowRain, Vector3.zero);
            fx.transform.localPosition = SpawnEffectPos;
            fx.transform.localScale = new Vector3(EffectSize, EffectSize, EffectSize);
            ArrowRainEffect effect = fx.GetComponent<ArrowRainEffect>();
            arrowRainDamageInfo = new(0, arrowRainData.attackdamagetype[0]==Utill_Enum.AttackDamageType.Magic, hunter);
            effect.SetEffect(InitialRadius,MaxRadius,ExpansionTime, hunter, arrowRainDamageInfo,arrowRainData);
            //ShowDamageRadius(SpawnEffectPos);

            SoundManager.Instance.PlayAudio(skillName);


            float castingInterval = UnityEngine.Random.Range(CastingIntervalMin, CastingIntervalMax);
            yield return new WaitForSeconds(castingInterval); // interval만큼 무작위 숫자로 대기
        }
    }

    private void ShowDamageRadius(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(MaxRadius * 2, 0.1f, MaxRadius * 2); // Y축을 줄여 평평하게 만듭니다.
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f); // 반투명 빨간색으로 설정
        sphere.GetComponent<SphereCollider>().enabled = false;

        Destroy(sphere, ImpactDelay); // 일정 시간 후 삭제
    }
    
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
        arrowRainData = BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, skillName, _upgradeAmount) as ArrowRainData;
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        arrowRainData = (ArrowRainData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.ArrowRain, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, arrowRainData.CHGValue_UseMP); //마나감소
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
}
