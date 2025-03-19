using Game.Debbug;
using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 도깨비불 스킬
/// </summary>
public class DokkaebiFire : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("이펙트의 높이값")]
    [SerializeField] float effectHeight = 1f;
    [Header("이펙트가 따라오는 속도")]
    [SerializeField] float followingSpeed = 5f;
    [Header("이펙트 회전속도")]
    [SerializeField] float rotationSpeed = 4f; // 회전 속도
    [Header("이펙트가 헌터와 떨어지는 거리")]
    [SerializeField] float diameter = 2f;
    [Header("이펙트가 헌터에 떨어지는 시간 간격 범위")]
    [SerializeField] float dokkaebiFireMinIntervalTime = 1;
    [SerializeField] float dokkaebiFireMaxIntervalTime = 1;
    [Header("이펙트의 개수")]
    [SerializeField] int dokkaebiFireNumber = 3;
    #endregion

    //독립적인 코루틴으로 돌게금 변수선언
    private Coroutine currentCoroutine;

    private float[] xScales;
    private float[] yScales;
    private float[] zScales;
    private float angle = 0f; // 초기 각도
    private List<DokkaebiFireEffect> Demoneffectlist = new List<DokkaebiFireEffect>();
    private DokkaebiFireData dokkaebiFireData = null;
    private AudioSource dokkaebiFireAudioSource = null;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (!isInit) //스킬 초기화
        {
            isInit = true;

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(DemonEffectCoroutine());
        }
    }
    public override void Init_Skill()
    {
        isInit = false;
        if (dokkaebiFireAudioSource != null)
        {
            SoundManager.Instance.StopAudioLoop(dokkaebiFireAudioSource);
            dokkaebiFireAudioSource = null;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        for(int i = 0; i < Demoneffectlist.Count;i++)
        {
            Demoneffectlist[i].gameObject.SetActive(false);
        }
        Demoneffectlist.Clear();
    }

    IEnumerator DemonEffectCoroutine()
    {
        float distance = diameter; // 초기 반지름 설정
        GenerateRandomScales(dokkaebiFireNumber);
        if(dokkaebiFireAudioSource == null)
            dokkaebiFireAudioSource = SoundManager.Instance.PlayAudioLoop(skillName);
        // 도깨비불 이펙트 생성
        for (int i = 0; i < dokkaebiFireNumber; i++)
        {
            DokkaebiFireEffect demonEffect = ObjectPooler.SpawnFromPool(Tag.DokkaebiFireFx, SpawnSkillArea(SkillCastingPositionType).pos).GetComponent<DokkaebiFireEffect>();
            demonEffect.Init(hunter,this, dokkaebiFireData);
            Demoneffectlist.Add(demonEffect);
        }
        // 랜덤 초마다 반지름이 변경되도록 로직 수정
        float waitTime = Random.Range(dokkaebiFireMinIntervalTime, dokkaebiFireMaxIntervalTime);

        float time = 0f;
        while (isInit)
        {
            // 헌터 주변을 회전하며 파티클 이동
            angle += (hunter._UserStat.DokkaebiFireRotationSpeed + rotationSpeed) * Time.deltaTime;
            time += Time.deltaTime;

            int count = Demoneffectlist.Count;
            for (int i = 0; i < count; i++)
            {
                float xScale = xScales[i];
                float yScale = yScales[i];
                float zScale = zScales[i];
                float theta = angle + i * Mathf.PI * 2f / Demoneffectlist.Count;

                // 각 축에 대한 이동 거리 계산
                float offsetX = Mathf.Cos(theta) * distance * xScale;
                float offsetZ = Mathf.Sin(theta) * distance * zScale;

                Vector3 targetPosition = SpawnSkillArea(SkillCastingPositionType).pos + new Vector3(offsetX * distance, effectHeight, offsetZ);
                Vector3 currentPosition = Demoneffectlist[i].transform.position;
                float step = followingSpeed * Time.deltaTime;
                Demoneffectlist[i].transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);


            }
            yield return null;
        }

    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    // 각 이펙트의 랜덤한 스케일 값을 생성하는 함수
    private void GenerateRandomScales(int count)
    {
        xScales = new float[count];
        yScales = new float[count];
        zScales = new float[count];

        for (int i = 0; i < count; i++)
        {
            // 각 축에 대한 랜덤한 스케일 값을 생성합니다.
            xScales[i] = Random.Range(0.5f, 1.5f);
            yScales[i] = Random.Range(0.5f, 1.5f);
            zScales[i] = Random.Range(0.5f, 1.5f);
        }
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        dokkaebiFireData = (DokkaebiFireData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.DokkaebiFire, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }

        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, dokkaebiFireData.CHGValue_UseMP); //마나감소
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
