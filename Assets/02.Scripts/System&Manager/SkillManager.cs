using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬관리하는 매니져
/// </summary>
public class SkillManager : MonoSingleton<SkillManager>
{
    public float DailyArcherValue;
    public float DailyGurdianValue;
    public float DailyMageValue;

    public float DailyAllValue;

    public GameObject[] skillInfoUI;

    public BattleSkillUi[] battleskillui;

    public MageSkillPopup mageskillpopup;
    public GuardianSkillPopup guardianskillpopup;
    public ArcherSkillPopup hunterskillpopup;

    public ArcherSkillLayout hunterskilllayout;
    public GuardianSKillLayout guardianskilllayout;
    public MageSkillLayout mageskilllayout;
    public DailySkillLayout Dailyskilllayout;
    


    [Header("게임에 사용되는 스킬 데이터")]
    public Electricshock electricshock;
    public Meteor meteor;
    public CoinFilp CoinFilp;
    public LastLeaf lastLeaf;
    public DemonicBlow demonicBlow;
    public VenomousSting venomousSting;
    public ShroudRay shroudRay;
    public SpikeTrap spikeTrap;
    public Focus focus;
    public ProtectionBless protectionBless;
    public WeaknessDetection weaknessDetection;
    public RageShot rageShot;
    public CriticalShot criticalShot;
    public MaximumDose maximumDose;
    public BeastTransformation beastTransformation;
    public ArrowRain arrowRain;
    public SplitPiercing splitPiercing;
    public StunOnBasicAttack stunOnBasicAttack;
    public StunOnBasicAttack1 stunOnBasicAttack1;
    public RageAttack rageAttack;
    public DodgeBoost dodgeBoost;
    public Electric electric;
    public MagneticWaves magneticWaves;
    public HammerSummon hammerSummon;
    public RapidShot rapidShot;
    public FreezingOnbasicAttack freezingonbasicattack;
    public PoisonOnBasicAttack poisonOnBasicAttack;
    public SlowOnBasicAttack slowOnBasicAttack;
    public HPRegenOnEnemyKill hpregenonenemykill;
    public WhirlwindRush whirlwindRush;
    public ForceField forceField;
    public ContinuousHit continuousHit;
    public DokkaebiFire dokkaebiFire;
    public StatusUpgardeSkill statusUpgardeSkill;
    public GuardianShield guardianShield;
    public StrongShot strongShot;
    public ChainLightning chainLightning;
    public KillDash killDash;
    public BloodSurge bloodSurge;
    public HunterStance Hunterstance;

    //현재 마나가 부족해 시전하지 못한 스킬
    public List<BattleSkillUi> ManaShortageSkillList = new();
    private Coroutine manaShortageCheckCoroutine = null;

    public List<ISkill> currentequippedSkills; // 현재 유저가 장착한 스킬 목록
    public List<BaseSkill> DailySkills; // 현재 유저가 장착한 스킬 목록

    private ISkill currentSkill; // 스킬을 눌렀을 때 해당 데이터에 기반한 스킬을 가져오기 위한 변수

    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += skillSet;
        GameEventSystem.GameSkillManaShortage_SendEventHandler += AddManaShortageCheckList;
    }
    public void SkillUISet()
    {
        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_OPEN_NUMBER].Value;

        for (int i = 0; i < skillInfoUI.Length; i++)
        {
            int[] slots;

            // 각 i에 대한 슬롯 배열 정의
            switch (i)
            {
                case 0:
                    slots = new int[] { 11, 10, 9, 8 };
                    break;
                case 1:
                    slots = new int[] { 7, 6, 5, 4 };
                    break;
                case 2:
                    slots = new int[] { 3, 2, 1, 0 };
                    break;
                default:
                    continue;
            }

            // i가 value보다 작으면 UnEmpty_Slot(), 크면 Empty_Slot()
            foreach (int slot in slots)
            {
                if (i < value)
                    battleskillui[slot].UnEmpty_Slot();
                else
                    battleskillui[slot].SlotEmpty();
            }
        }
    }
    private bool skillSet(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence) 
        {
            case Utill_Enum.Enum_GameSequence.InGame:
                // 초기화된 스킬 목록 준비
                currentequippedSkills = new List<ISkill> {electricshock  , meteor  , CoinFilp,
                                                          maximumDose,demonicBlow,venomousSting,lastLeaf, shroudRay, spikeTrap, focus, protectionBless, weaknessDetection, rageShot,
                                                          beastTransformation,criticalShot, arrowRain,splitPiercing ,stunOnBasicAttack, stunOnBasicAttack1,rageAttack,dodgeBoost,electric, hammerSummon, rapidShot,
                                                          magneticWaves ,freezingonbasicattack , poisonOnBasicAttack ,slowOnBasicAttack ,hpregenonenemykill,whirlwindRush, forceField, continuousHit,
                                                          dokkaebiFire ,statusUpgardeSkill, strongShot, chainLightning, guardianShield, killDash,bloodSurge ,Hunterstance};

                GameEventSystem.Use_Skill_Delegate_Event += EnqueueSkill;

                //각각 직업마다 스킬 만들어 셋팅해줌
                hunterskilllayout.Init_Slots(currentequippedSkills);
                guardianskilllayout.Init_Slots(currentequippedSkills);
                mageskilllayout.Init_Slots(currentequippedSkills);


                //weeklyskilllayout.Init_Slots(currentequippedSkills);

                //요일스킬...데이터파싱..
                PopUpSystem.Instance._DeilyRandomPopUp.Init_Dailyskill_UIData();

                ManaShortageSkillList = new();
                manaShortageCheckCoroutine = null;
                return true;
        }
        return false;
    }

    //유저의 현재 가지고 있는 스킬들 가져오기
    public void SetMageSkill( string[] mage)
    {
        mageskillpopup.PresetCount =int.Parse(mage[0]);
        mageskillpopup.HasSelectName = mage;
        //uisettting

    }
    //유저의 현재 가지고 있는 스킬들 가져오기
    public void SetHunterSkill(string[] Hunter)
    {
        hunterskillpopup.PresetCount = int.Parse(Hunter[0]);
        hunterskillpopup.HasSelectName = Hunter;
        //uisettting
    }
    //유저의 현재 가지고 있는 스킬들 가져오기
    public void SetGuardianSkill(string[,] guardian)
    {
        //guardianskillpopup.PresetCount = int.Parse(guardian[0]);
        //guardianskillpopup.HasSelectName = guardian;
        //uisettting
    }



    //데이터기반 ui셋팅
    public void OnguardinaSKillUi()
    {
        //skillView.UpdateView(guardianskillpopup.HasSelectName);
    }
    //데이터기반 ui셋팅
    public void OnMageSKillUi()
    {
        //skillView.UpdateView(mageskillpopup.HasSelectName);
    }

    public void AddManaShortageCheckList(BattleSkillUi battleSkillUi)
    {

    }

    private void EnqueueSkill(BattleSkillUi battleskillui)
    {
        if (battleskillui.Skill == null)//예외
        {
            battleskillui.ChangeState(new EmptyState(battleskillui));
            return;
        }

        // 스킬 대기 상태 확인: 비전투 상태이거나 다른 스킬이 재생 중인 경우
        bool isskillmotion = isCheckMotionSkill(battleskillui.Skill);
        //마나소진 검사
        bool ismanause = battleskillui.Skill.Cooldown(battleskillui.m_SubClass);

        if (GameStateMachine.Instance.CurrentState is NoneChatStrategy && isskillmotion) //1.비전투시 , 착용중인스킬중 모션 스킬을 사용중인지 && 스킬모션사용중인지     
        {
            hunterskilllayout.waitingSkillsQueue.Enqueue(battleskillui.Skill); // 스킬 대기 상태 큐에 추가
            battleskillui.ChangeState(new WaitingState(battleskillui));
        }
        // 마나 부족 상태 확인: 스킬 사용에 필요한 마나가 부족한 경우
        else if (ismanause == false)
        {
            hunterskilllayout.manaShortageSkillsQueue.Enqueue(battleskillui.Skill); // 마나 대기 상태 큐에 추가
            battleskillui.ChangeState(new ManaShortageState(battleskillui));
        }
        else
        {
            battleskillui.ChangeState(new UsableState(battleskillui));
        }
    }
    private bool isCheckMotionSkill(ISkill skill)//해당 헌터스킬중 어느 스킬이 모션중인가 체크
    {
        bool ismotion = false;
        foreach (var skillui in hunterskilllayout.BattleSkillUis)
        {
            if (skillui.Skill == null)
            {
                continue;
            }
            if (skill.SkillName != skillui.Skill.SkillName && skillui.Skill.isMotion)//자신의 스킬을 제외한 스킬들중
            {
                ismotion = true;
            }
        }
        return ismotion;
    }

    public void PauseAllHunterSkill(SubClass subClass)
    {
        int skillCount = 0;
        BattleSkillUi coolTimeSkillUi = null;
        switch (subClass)
        {
            case SubClass.None:
                //weekly
                break;
            case SubClass.Archer:
                for (int i = 0; i < hunterskilllayout.BattleSkillUis.Length; i++)
                {
                    if (hunterskilllayout.BattleSkillUis[i].Skill != null)
                    {
                        coolTimeSkillUi = hunterskilllayout.BattleSkillUis[i];
                        coolTimeSkillUi.DeActiveSetUi(subClass, false);
                        coolTimeSkillUi.SetCanRunningSkill(false);
                    }
                }
                
                break;
            case SubClass.Guardian:
                for (int i = 0; i < guardianskilllayout.BattleSkillUis.Length; i++)
                {
                    if (guardianskilllayout.BattleSkillUis[i].Skill != null)
                    {
                        coolTimeSkillUi = guardianskilllayout.BattleSkillUis[i];
                        coolTimeSkillUi.DeActiveSetUi(subClass, false);
                        coolTimeSkillUi.SetCanRunningSkill(false);
                    }
                }
                break;
            case SubClass.Mage:
                for (int i = 0; i < mageskilllayout.BattleSkillUis.Length; i++)
                {
                    if (mageskilllayout.BattleSkillUis[i].Skill != null)
                    {
                        coolTimeSkillUi = mageskilllayout.BattleSkillUis[i];
                        coolTimeSkillUi.DeActiveSetUi(subClass, false);
                        coolTimeSkillUi.SetCanRunningSkill(false);
                    }
                }
                break;
        }
 
    }
    public void ReStartAllHunterSkill(SubClass subClass)
    {
        int skillCount = 0;
        switch (subClass)
        {
            case SubClass.None:
                //weekly
                break;
            case SubClass.Archer:
                skillCount = hunterskilllayout.BattleSkillUis.Length;
                break;
            case SubClass.Guardian:
                skillCount = guardianskilllayout.BattleSkillUis.Length;
                break;
            case SubClass.Mage:
                skillCount = mageskilllayout.BattleSkillUis.Length;
                break;
        }
        for (int i = 0; i < skillCount; i++)
        {
            BattleSkillUi coolTimeSkillUi = null;
            switch (subClass)
            {
                case SubClass.None:
                    //weekly
                    break;
                case SubClass.Archer:
                    coolTimeSkillUi = hunterskilllayout.BattleSkillUis[i];
                    break;
                case SubClass.Guardian:
                    coolTimeSkillUi = guardianskilllayout.BattleSkillUis[i];
                    break;
                case SubClass.Mage:
                    coolTimeSkillUi = mageskilllayout.BattleSkillUis[i];
                    break;
            }
            if (coolTimeSkillUi != null)
            {
                coolTimeSkillUi.SetCanRunningSkill(true);
                //쿨타임 있는 스킬만
                if(coolTimeSkillUi.Count>0)
                {
                    float count = coolTimeSkillUi.CooldownOverlay.fillAmount * coolTimeSkillUi.Count;
                    //coolTimeSkillUi.SetExternalCooldown(coolTimeSkillUi.Count,count);
                    coolTimeSkillUi.ActiveSetUi(subClass,false);
                }
                else
                    coolTimeSkillUi.ActiveSetUi(subClass, false);
            }
        }
    }
    /// <summary>
    /// 스킬을 멈추게 하는 함수
    /// </summary>
    /// <param name="subClass">멈출 스킬의 타입</param>
    /// <param name="skill">멈출 스킬</param>
    public void PauseSkill(SubClass subClass,ISkill skill)
    {
        BattleSkillUi coolTimeSkillUi = null;
        switch (subClass)
        {
            case SubClass.None:
                //weekly
                break;
            case SubClass.Archer:
                coolTimeSkillUi = hunterskilllayout.BattleSkillUis.FirstOrDefault((x) => (x.Skill != null) ? x.Skill.SkillName == skill.SkillName : false);
                break;
            case SubClass.Guardian:
                coolTimeSkillUi = guardianskilllayout.BattleSkillUis.FirstOrDefault((x) => (x.Skill != null) ? x.Skill.SkillName == skill.SkillName : false);
                break;
            case SubClass.Mage:
                coolTimeSkillUi = mageskilllayout.BattleSkillUis.FirstOrDefault((x) => (x.Skill != null) ? x.Skill.SkillName == skill.SkillName : false);
                break;
        }
        if (coolTimeSkillUi != null)
        {
            coolTimeSkillUi.DeActiveSetUi(subClass, false);
            coolTimeSkillUi.StopCooldown();
        }
    }

    /// <summary>
    /// 멈춘 스킬을 다시 동작하게 하는 함수
    /// </summary>
    /// <param name="subClass">동작할 스킬의 타입</param>
    /// <param name="skill">동작할 스킬</param>
    public void ReStartSkill(SubClass subClass,ISkill skill)
    {
        BattleSkillUi coolTimeSkillUi = null;
        switch (subClass)
        {
            case SubClass.None:
                //weekly
                break;
            case SubClass.Archer:
                coolTimeSkillUi = hunterskilllayout.BattleSkillUis.FirstOrDefault((x) => (x.Skill != null) ? x.Skill.SkillName == skill.SkillName : false);
                break;
            case SubClass.Guardian:
                coolTimeSkillUi = guardianskilllayout.BattleSkillUis.FirstOrDefault((x) => (x.Skill != null) ? x.Skill.SkillName == skill.SkillName : false);
                break;
            case SubClass.Mage:
                coolTimeSkillUi = mageskilllayout.BattleSkillUis.FirstOrDefault((x) => (x.Skill != null) ? x.Skill.SkillName == skill.SkillName : false);
                break;
        }
        if (coolTimeSkillUi != null)
        {
            //쿨타임 있는 스킬만
            if (coolTimeSkillUi.Count > 0)
            {
                float count = coolTimeSkillUi.CooldownOverlay.fillAmount * coolTimeSkillUi.Count;
                //coolTimeSkillUi.SetExternalCooldown(coolTimeSkillUi.Count, count);

                coolTimeSkillUi.SetCanRunningSkill(true);
                coolTimeSkillUi.ActiveSetUi(subClass, false);
            }
            else
                coolTimeSkillUi.ActiveSetUi(subClass, false);
        }
    }

    //스킬이름으로 스킬가져오기
    public ISkill Get_SkillForName(string name)
    {
        ISkill skill = null;
        if (name == Tag.Empty || name == "" || name == null)
        {
            return null;
        }
        skill = currentequippedSkills.Find(c=> c.SkillName == name);

        return skill;

    }


    public ISkill Get_DailySkillForName(string name)
    {
        ISkill skill = null;
        if (name == Tag.Empty || name == "" || name == null)
        {
            return null;
        }
        skill = DailySkills.Find(c => c.SkillName == name);

        return skill;

    }
    /// <summary>
    /// 중복되지 않는 스킬 4개를 선택하여 반환합니다.
    /// </summary>
    /// <returns>중복되지 않는 스킬 4개의 리스트</returns>
    public List<BaseSkill> GetUniqueRandomSkills(List<BaseSkill> skills, int numberOfSkills)
    {
        // 스킬 리스트의 총 개수
        int skillCount = skills.Count;

        // 필요한 스킬 개수보다 스킬이 적으면 빈 리스트를 반환합니다.
        if (skillCount < numberOfSkills || numberOfSkills <= 0)
        {
            return new List<BaseSkill>();
        }

        // 중복되지 않는 랜덤 인덱스를 저장할 HashSet
        HashSet<int> uniqueIndices = new HashSet<int>();

        // 인덱스 개수를 만족할 때까지 랜덤 인덱스를 뽑습니다.
        System.Random random = new System.Random();
        while (uniqueIndices.Count < numberOfSkills)
        {
            int randomIndex = random.Next(skillCount);
            uniqueIndices.Add(randomIndex); // HashSet에 추가하면 자동으로 중복이 제거됩니다.
        }

        // 중복되지 않는 랜덤 인덱스를 기반으로 스킬 리스트를 생성합니다.
        List<BaseSkill> randomSkills = new List<BaseSkill>();
        foreach (int index in uniqueIndices)
        {
            randomSkills.Add(skills[index]);
        }

        return randomSkills;
    }


    public void SaveUserSkillData()
    {

    }

}
