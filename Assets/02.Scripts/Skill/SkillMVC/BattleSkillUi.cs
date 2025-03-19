using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬ui보여주는곳
/// </summary>
public class BattleSkillUi : MonoBehaviour
{
    [Range(0, 1)]
    public float coolDownAlpha;

    public Image Background;
    public Image SkillICon;
    public Image CooldownOverlay;
    public Image SkillIconMask;

    public Sprite DefalutSpace;
    public TextMeshProUGUI Text;
    public TextMeshProUGUI StackCountText;
    public TextMeshProUGUI ActiveSecondText;

    [SerializeField]
    private Image ActiveEffectImage = null;

    public Image ManaShortageImage = null;
    
    public TextColorSet TextColor;

    private int count;
    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    [SerializeField] bool _isUnuse = false;
    public bool isUnuse
    {
        get { return _isUnuse; }
        set { _isUnuse = value; }
    }

    private ISkill skill;
    public ISkill Skill
    {
        get
        {
            return skill;
        }
        set
        {
            if (skill != null && skill.IsStackable)
            {
                skill.OnStackChanged -= UpdateStackCount;
            }
            if(skill != null)
            {
                skill.OnActiveSecondChanged -= UpdateActiveSecond;
            }

            skill = value;

            if (skill != null)
            {
                skill.OnActiveSecondChanged += UpdateActiveSecond;
            }
            if (skill != null && skill.IsStackable)
            {
                skill.OnStackChanged += UpdateStackCount;
            }
        }
    }

    [SerializeField] private Utill_Enum.SubClass m_subClass;
    public Utill_Enum.SubClass m_SubClass
    {
        get { return m_subClass; }
        set { m_subClass = value; }
    }

    /// <summary>
    /// 스택 UI 표기
    /// </summary>
    public void UpdateStackCount(int stackCount)
    {
        StackCountText.text = stackCount.ToString();
    }

    Coroutine activeSecondReduceCoroutine = null;
    /// <summary>
    /// 지속시간 UI 연출 + 표기
    /// </summary>
    public void UpdateActiveSecond(float activeSecond,bool isOn)
    {
        if(!isOn)
        {
            if (activeSecondReduceCoroutine != null)
                StopCoroutine(activeSecondReduceCoroutine);
            ActiveSecondText.gameObject.SetActive(false);
            ActiveEffectImage.gameObject.SetActive(false);
            return;
        }
        ActiveSecondText.gameObject.SetActive(true);
        ActiveEffectImage.gameObject.SetActive(true);
        
        if(activeSecondReduceCoroutine != null)
            StopCoroutine(activeSecondReduceCoroutine);

        activeSecondReduceCoroutine = StartCoroutine(ActiveSecondReduce(activeSecond));
    }



    private IEnumerator ActiveSecondReduce(float activeSecond)
    {
        float reduceTime = activeSecond;
        ActiveEffectImage.fillAmount = 1;
        var delay = new WaitForSeconds(0.1f);

        while (reduceTime > 0)
        {
            reduceTime -= Time.deltaTime;  // 정확한 시간 감소
            if (reduceTime < 0)
                break;

            // 남은 시간을 텍스트로 표시, 소수점 첫째 자리까지
            ActiveSecondText.text = reduceTime.ToString("F1");

            // fillAmount를 자연스럽게 1에서 0으로 감소
            ActiveEffectImage.fillAmount = Mathf.Clamp01(reduceTime / activeSecond);

            yield return null;  // 한 프레임 대기
        }

        // 최종적으로 fillAmount가 0이 되도록 설정
        ActiveEffectImage.fillAmount = 0;

        ActiveSecondText.gameObject.SetActive(false);
        ActiveEffectImage.gameObject.SetActive(false);
    }

    private void Awake()
    {
        coolDownAlpha *= 255;
        GameEventSystem.GameHunterPortraitClickBtn_GameEventHandler_Event += EquipmentHunter; //캐릭터 변경에대한 스킬초기화 이벤트
        GameEventSystem.GameHunterReviveSkillInit_GameEventHandler_Event += RevivieSkillinit;//헌터 부활&죽음 스킬초기화 이벤트
   
    }
    private SkillSlotState currentState;

    public void ChangeState(SkillSlotState newState)
    {
        currentState?.Exit(); // 이전 상태 종료
        currentState = newState; // 새로운 상태 설정
        currentState.Enter(); // 새로운 상태 진입
    }




    private Coroutine cooldownCoroutine;
    public Coroutine CooldownCoroutine
    {
        get { return cooldownCoroutine; }
        set { cooldownCoroutine = value; }
    }

    public void InitSlot(int cooltime, ISkill _skill, Utill_Enum.SubClass subclass, bool isEquip)
    {
        m_SubClass = subclass;
        Count = cooltime;
        Text.text = Count.ToString();
        CooldownOverlay.fillAmount = 1.0f;
        isUnuse = false;
        Skill = _skill;

        if (Skill == null)
        {
            ChangeState(new EmptyState(this));
            return;
        }


        ManaShortageImage.gameObject.SetActive(false);

        UpdateActiveSecond(0, false);
        if (Skill.IsStackable)
        {
            StackCountText.gameObject.SetActive(true);
            StackCountText.SetText($"{0}");

            if (isEquip == false)
            {
                StackCountText.gameObject.SetActive(false);
            }
        }
    }

    public void InitSlot()
    {
        if (Skill == null)
        {
            SetImageAlpha(SkillICon, 2);
            TextColor.gameObject.SetActive(false);
            ChangeState(new EmptyState(this));
            return;
        }
        Count = Skill.SkillCoolDown;
        if (Count <= 0)
        {
            Text.gameObject.SetActive(false);
        }
        else
        {
            Text.gameObject.SetActive(true);
        }

        ManaShortageImage.gameObject.SetActive(false);
        UpdateActiveSecond(0, false);
        StackCountText.SetText($"{0}");

        SetImageAlpha(SkillICon, 255);
        SetImageAlpha(CooldownOverlay, coolDownAlpha);
        Text.text = Count.ToString();
        StopCooldown();
    }


    // 카운트다운을 시작하는 메서드
    public void StartCooldown()
    {
        isUnuse = false;
        if (CooldownCoroutine != null)
        {
            StopCoroutine(CooldownCoroutine);
        }
        CooldownCoroutine = StartCoroutine(UseCool());

    }

    public void StopCooldown()
    {
        isUnuse = true;
        if (CooldownCoroutine != null && skill != null)
        {
            StopCoroutine(CooldownCoroutine);
            skill.isActiveSkill = false; //스킬 사용 변수 해제 버프 중첩을 막기위한 변수임
            skill.Init_Skill();
          
        }
    }
    private bool canRunningSkill = true; //스킬 쿨이 돌아갈 수 있는지 여부

    // Count초만큼 쿨타임을 진행하는 코루틴
    private IEnumerator UseCool()
    {
        if (Skill == null)
        {
            yield break;
        }

        
        Text.text = count.ToString();
        CooldownOverlay.fillAmount = 1f;
        ManaShortageImage.gameObject.SetActive(false);
        bool isused = false;
        float elapsed = 0f;

        // 쿨타임이 0인 경우 무한루프
        if (Count <= 0)
        {
            Text.gameObject.SetActive(true);
            while (!isUnuse)
            {
                if (Skill == null)
                {
                    StopCooldown();
                    yield break;
                }
                Text.gameObject.SetActive(false);
                skill.Execute(m_SubClass);
                if (isUnuse == true)
                {
                    isUnuse = false;
                    Skill.Init_Skill();
                    yield break;
                }

                yield return Utill_Standard.WaitTimeOne; // 무한 루프 방지
                //스킬쿨이 돌아갈 수 있을때까지 대기함
                do
                {
                    if (canRunningSkill)
                    {
                        elapsed += Time.deltaTime;
                    }
                    yield return null;
                } while (!canRunningSkill);
            }
        }
        else
        {
            Text.gameObject.SetActive(false);
            CooldownOverlay.fillAmount = 1f;
            SetImageAlpha(CooldownOverlay, coolDownAlpha);
            while (true)
            {
                if (Skill == null)
                {
                    StopCooldown();
                    yield break;
                }
                elapsed += Time.deltaTime;
                float remainingTime = Count - elapsed;

                // 1초 미만의 경우 소수점 첫째 자리까지 표시
                if (remainingTime < 1f)
                {
                    Text.text = remainingTime.ToString("F1");
                }
                else
                {
                    // 1초 이상은 기본적인 포맷 적용
                    Text.text = Utility.TimeFormatSkill(remainingTime);
                }
              

                // fillAmount가 1에서 0으로 자연스럽게 감소하도록 설정
                CooldownOverlay.fillAmount = Mathf.Clamp01(remainingTime / Count);

                if (remainingTime <= 0)
                {
                    elapsed = 0f;
                    CooldownOverlay.fillAmount = 0f;
                    skill.Execute(m_SubClass);
                }


                //스킬쿨이 돌아갈 수 있을때까지 대기함
                do
                {
                    if (canRunningSkill)
                    {
                        elapsed += Time.deltaTime;
                    }
                    yield return null;
                } while (!canRunningSkill);

                yield return null;

         
            }
        }
    }

    // 외부에서 쿨타임 설정
    public void SetCanRunningSkill(bool isOn)
    {
        canRunningSkill = isOn;
    }

    public IEnumerator OnlyCoolSlotRotation(float _Time, float elapsedTime = 0f)
    {
        // 쿨타임이 0을 초과하는 경우
        float elapsed = elapsedTime;
        CooldownOverlay.fillAmount = 1f; // 초기값을 1로 설정
        while (elapsed < _Time)
        {
            elapsed += Time.deltaTime;
            float remainingTime = _Time - elapsed;

            // 1초 미만의 경우 소수점 첫째 자리까지 표시
            if (remainingTime < 1f)
            {
                Text.text = remainingTime.ToString("F1");
            }
            else
            {
                // 1초 이상은 기본적인 포맷 적용
                Text.text = Utility.TimeFormatSkill(remainingTime);
            }

            // fillAmount가 1에서 0으로 자연스럽게 감소하도록 설정
            CooldownOverlay.fillAmount = Mathf.Clamp01(remainingTime / _Time);

            if (remainingTime <= 0)
            {
                // 유저의 쿨다운 스탯 시점이 여기서 적용되어야 함
                Skill.Execute(m_SubClass);
                // 스킬 연속 시전을 위해서...
                elapsed = 0f;
                CooldownOverlay.fillAmount = 0f;
                yield break;
            }

            if (isUnuse == true)
            {
                isUnuse = false;
                yield break;
            }

            yield return null;
        }
    }
    public void ChanngeSprite(string name)
    {
        if ((name) == Tag.Empty)
        {
            Text.gameObject.SetActive(false);
            // 알파값을 0으로 설정
            initslot();
            return;
        }

        SetImageAlpha(SkillICon, 255);
        SetImageAlpha(CooldownOverlay, coolDownAlpha);
        SkillICon.sprite = Utill_Standard.GetUiSprite(name);
    }

    public void SetImageAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = Mathf.Clamp(alpha, 0, 255) / 255f; // 알파값을 0~255 범위에서 0.0f~1.0f로 변환
            image.color = color;
        }
    }



    public void EquipmentHunter(Utill_Enum.SubClass type, bool isEquip)
    {

        int i = GameDataTable.Instance.User.CurrentEquipHunter.Count;


        if (i == 1) //전장배치가 1명이라면 리턴
        {
            DeActiveSetUi(type);
            return;
        }

        if (isEquip) //캐릭터가 장착되었을때 
        {
            ActiveSetUi(type);
        }
        else //캐릭터가 장착이 안되어있을때
        {
            DeActiveSetUi(type);
        }
    }
    /// <summary>
    /// 헌터가 죽었을때 또는 살아났을때
    /// </summary>
    /// <param name="type"></param>
    private void RevivieSkillinit(SubClass type, bool isRevive)
    {
        if (isRevive)//살아났을때 스킬 쿨돌리기
        {
            ActiveSetUi(type);
        }
        else //죽었으니 스킬 비활성화
        {
            DeActiveSetUi(type);
        }
    }
    //장착버튼눌러서 헌터가없어졌을시
    public void DeActiveSetUi(Utill_Enum.SubClass type, bool isResetCool = true)
    {
        if (m_SubClass == type)
        {
            //쿨초기화
            if (isResetCool)
                InitSlot();
            Text.gameObject.SetActive(false);

            if (isResetCool)
                CooldownOverlay.fillAmount = 0;
            //빈 스킬 슬롯은 기본슬롯으로..
            if (Skill == null)
            {
                SetImageAlpha(SkillICon, 2);
                //SetImageAlpha(CooldownOverlay, 0);
            }
            else
            {
                // Black 머테리얼을 Resources에서 로드하여 할당
                Utill_Standard.SetImageMaterial(SkillICon, "Black");
                Utill_Standard.SetImageMaterial(CooldownOverlay, "Black");
            }

        }
        else
        {
            return;
        }
    }

    public void ActiveSetUi(Utill_Enum.SubClass type, bool isResetCool = true)
    {
        if (Skill == null)
        {
            Text.gameObject.SetActive(false);
            return;
        }

        if (m_SubClass == type)
        {
            if (isResetCool)
                InitSlot();
         

            if (isResetCool)
                StartCooldown();
            // 머테리얼을 None으로 설정
            SkillICon.material = null;
            CooldownOverlay.material = null;
            SetImageAlpha(SkillICon, 255);
        }
        else
        {
            return;
        }
    }
    //슬롯을 빈슬롯만들댸
    public void initslot()
    {
        StackCountText.gameObject.SetActive(false);
        Text.gameObject.SetActive(false);
        SetImageAlpha(SkillICon, 2);
        //SetImageAlpha(CooldownOverlay, 0f);
    }



    public void Empty_Slot() // 겜시작시 아예 투명으로 만들어버리는 로직
    {
        SkillICon.sprite = null;
        SkillICon.material = null;
        SetImageAlpha(SkillICon, 2f);
        StackCountText.gameObject.SetActive(false);
        Text.gameObject.SetActive(false);
        SetImageAlpha(CooldownOverlay, 0f);
    }

    public void SlotEmpty()//contstain으로 슬롯조절할떄쓰는함수
    {
        Background.enabled = false;
        SkillIconMask.gameObject.SetActive(false);
        CooldownOverlay.gameObject.SetActive(false);
        Text.gameObject.SetActive(false);
    }

    public void UnEmpty_Slot()// 겜시작시 보이게끔 만들어버리는 로직
    {
        Background.enabled = true;
        SkillIconMask.gameObject.SetActive(true);
        CooldownOverlay.gameObject.SetActive(true);
        Text.gameObject.SetActive(true);
    }
}

