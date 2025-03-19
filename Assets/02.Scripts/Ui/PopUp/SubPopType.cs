    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NSY;
using System.Text;
using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine.SocialPlatforms;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 하위 팝업에 공통으로 사용하는 스크립트
/// </summary>
public class SubPopType : MonoBehaviour, IPopUp
{
    public Hunter Hunter;
    public GameObject ArcherSpecisalStatObj;
    public GameObject GurdianSpecisalStatObj;
    public GameObject MageSpecisalStatObj;

    #region 헌터 일반 스텟
    [Header("헌터 일반 스텟")]
    public TextMeshProUGUI AttackSpeedtxt; //초당 공격횟수수 
    public TextMeshProUGUI AttackSpeedPercenttxt; // 초당공격횟수 %증가량

    public TextMeshProUGUI MoveSpeedtxt; //이동속도
    public TextMeshProUGUI MoveSpeedPercenttxt; //이동속도 %증가량
    public TextMeshProUGUI GroupMoveSpeedtxt; //대형 이동속도
    public TextMeshProUGUI GroupMoveSpeedPercenttxt; //대형 이동속도 %증가량

    public TextMeshProUGUI AttackRangetxt;// 공격시 사정거리

    public TextMeshProUGUI PhysicalPowertxt; //물리공격
    public TextMeshProUGUI PhysicalPowerPercenttxt; //물리공격증가

    public TextMeshProUGUI MagicPowertxt;//마법공격
    public TextMeshProUGUI MagicPowerPercenttxt;//마법공격 증가

    

    public TextMeshProUGUI CriChancetxt;//치명타 확률
    public TextMeshProUGUI CriDamagetxt;//치명타 피해

    public TextMeshProUGUI CoupChancetxt;//일격 확률
    public TextMeshProUGUI CoupDamagetxt;//일격 피해


    public TextMeshProUGUI PhysicalPowerDefensetxt;//물리공격 방어력
    public TextMeshProUGUI PhysicalPowerDefensePercenttxt; //물리공격 방어력 증가

    public TextMeshProUGUI MagicPowerDefense;//마법공격 방어력
    public TextMeshProUGUI MagicPowerDefensePercenttxt;//마법공격 방어력 증가

    public TextMeshProUGUI PhysicalDamageReduction; //물리공격 피해감소
    public TextMeshProUGUI MagicDamageReductiontxt; //마법공격 피해 감소

    //public TextMeshProUGUI PhysicalDamageIncreasetxt; // 물리공격 피해증가
    //public TextMeshProUGUI MagicDamageIncreasetxt; //마법공격 피해증가



    public TextMeshProUGUI CCResisttxt;//상태이상회복률
    public TextMeshProUGUI DodgeChancetxt; //회피율

    public TextMeshProUGUI HPtxt;//체력
    public TextMeshProUGUI HPPercenttxt;

    public TextMeshProUGUI MPtxt; //마나
    public TextMeshProUGUI MPPercenttxt;

    public TextMeshProUGUI HPDraintxt; //체력 흡수
    public TextMeshProUGUI MPDraintxt; //마나 흡수

    public TextMeshProUGUI ExpBufftxt; //경험치획득 버프
    public TextMeshProUGUI GoldBufftxt; //골드획득 버프

    public TextMeshProUGUI Exp; //경험치

    public TextMeshProUGUI Level; // 레벨
    public TextMeshProUGUI DamageRadius; //피해범위

    public TextMeshProUGUI IntervalRate; //간격
    public TextMeshProUGUI DropChance; //아이템획득확률

    private TextColorSet textColorSet; //색깔 받아오기 위한 textColorSet
    #endregion

    #region 헌터 특수 스텟
    [Header("헌터 특수 스텟")]
    public TextMeshProUGUI AddDamageToNormalMobtxt; // 일반몹에게 주는 평타피해 추가
    public TextMeshProUGUI AddDamageToBossMobtxt; //보스에게 주는 평타피해 추가

    public TextMeshProUGUI PhysicalTrueDamagetxt;//물리 관통피해
    public TextMeshProUGUI PhysicalTrueDamagePercenttxt;//물리 관통피해 증가

    public TextMeshProUGUI MagicTrueDamagetxt;//마법 관통피해
    public TextMeshProUGUI MagicTrueDamagePercenttxt;//마법 관통피해 증가

    public TextMeshProUGUI InstantKillBossOnBasicAttacktxt;//기본공격시 보스 즉사 n.n%
    public TextMeshProUGUI ReflectRangetxt;//원거리 공격 반사
    #endregion;

    #region 궁수 특수 스텟
    [Header("궁수 특수 스텟")]
    public TextMeshProUGUI ArrowCounttxt; //궁수 화살개수
    public TextMeshProUGUI AddDamageArrowRaintxt; //화살비 추가데미지
    public TextMeshProUGUI ElectricCastingChancetxt; //벼락치기 발동 확률 증가 n
    public TextMeshProUGUI SplitPiercingNumbertxt; //분열관통 분열 수 증가 n
    public TextMeshProUGUI SpikeTrapNumbertxt; //가시덫 투사체 발사 개수 증가 n
    public TextMeshProUGUI SpikeTrapDamageRadiustxt; //가시덫 장판 범위 증가 n
    public TextMeshProUGUI VenomousStingNumbertxt; //맹독침 투사체 발사 개수 증가 n
    public TextMeshProUGUI VenomousStingDamageRadiustxt; //맹독침 폭발 범위 증가 n
    #endregion


    #region 수호자 특수 스텟
    [Header("수호자 특수 스텟")]
    public TextMeshProUGUI ElectricshockNumbertxt; //화살비 추가데미지
    public TextMeshProUGUI BeastTransformationStackCountReducetxt; //야수화 필요스택 n 감소
    public TextMeshProUGUI ContinuousHitStackCountReducetxt;//연타 필요스택 n 감소
    public TextMeshProUGUI DokkaebiFireRotationSpeedtxt; //도깨비불 회전 속도 증가 n
    #endregion

    #region 마법사 특수스텟
    [Header("마법사 특수 스텟")]
    #endregion

    public int ClickCount = 0;
    private int subTextSize = 26;

    private string defaultTextColorCode = "";

    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        //검은 바탕화면 OFF
        //UIManager.Instance.ActiveBackBardBlack(false);
        PopUpSystem.Instance.EscPopupListRemove();
         
        HunterChangeSystem.Instance.ActiveUIHunter();

        SoundManager.Instance.PlayAudio("UIClick");
        ClickCount = 0;

        gameObject.SetActive(false);
    }

    public void Show()
    {
        //검은 바탕화면 ON
       // UIManager.Instance.ActiveBackBardBlack(true);

        PopUpSystem.Instance.EscPopupListAdd(this);
        if(defaultTextColorCode == "")
            defaultTextColorCode = "#" + ColorUtility.ToHtmlStringRGB(AttackSpeedtxt.GetComponent<TextColorSet>().GetOptionValueColor);
        ClickCount++;
        if (ClickCount == 1)
        {
            gameObject.SetActive(true);
            //데이터셋...
            if(textColorSet == null)
                textColorSet = AttackSpeedtxt.GetComponent<TextColorSet>();
        }
        else
        {
            Hide();
        }
        
    }

    public void EventShow(bool _show) 
    {
        bool show = _show;
        if (show)
        {
            gameObject.SetActive(true);
            show = false;
        }
        else
        {
            gameObject.SetActive(false);
            show = true;
        }
    }

    public void SetStatData()
    {
        // StringBuilder 생성
        StringBuilder sb = new StringBuilder();
        HunterStat userStat = null;
        userStat = DataManager.Instance.Hunters[GameDataTable.Instance.User.currentHunter]._UserStat;
        //글씨 색상
        string textColorCode = "#" + ColorUtility.ToHtmlStringRGB(TextColorManager.Instance.SendStatMaxColor);

        //일반스텟 보여주는 곳
        SetNormalStat(userStat , textColorCode , sb);

        //특수 헌터스텟
        SetSpecialStat(userStat, textColorCode, sb);
        //캐릭별 특수스텟
        SetHunterSpecialStat(userStat, textColorCode, sb);

        LocalizationTable.languageSettings += SetStatData;
    }

    private void SetNormalStat(HunterStat userStat , string textColorCode, StringBuilder sb)
    {
        #region 공격속도 
        // 공격 속도
        StatTextSetting(AttackSpeedtxt, sb, HunterStat.AttackSpeedResult(userStat,false), textColorCode, Tag.AttackSpeed, LocalizationTable.Localization("AttackSpeed"));
        #endregion

        #region 공격속도 %
        // 공격 속도% 
        StatTextSetting(AttackSpeedPercenttxt, sb, userStat.AttackSpeedPercent, textColorCode, Tag.AttackSpeedPercent, LocalizationTable.Localization("AttackSpeedPercent"));
        #endregion

        #region 이동속도
        // 이동 속도
        StatTextSetting(MoveSpeedtxt, sb, HunterStat.MoveSpeedResult(userStat, false), textColorCode, Tag.MoveSpeed, LocalizationTable.Localization("MoveSpeed"));
        #endregion

        #region 이동속도 증가량 %
        // 이동 속도 증가량 (%)
        StatTextSetting(MoveSpeedPercenttxt, sb, userStat.MoveSpeedPercent, textColorCode, Tag.MoveSpeedPercent, LocalizationTable.Localization("MoveSpeedPercent"));
        #endregion

        #region 대형 이동속도
        // 대형 이동 속도
        StatTextSetting(GroupMoveSpeedtxt, sb, userStat.GroupMoveSpeed, textColorCode, Tag.GroupMoveSpeed, LocalizationTable.Localization("GroupMoveSpeed"));
        #endregion

        #region 대형 이동속도 증가량 %
        // 대형 이동 속도 증가량 %
        StatTextSetting(GroupMoveSpeedPercenttxt, sb, userStat.GroupMoveSpeedPercent, textColorCode, Tag.GroupMoveSpeedPercent, LocalizationTable.Localization("GroupMoveSpeedPercent"));
        #endregion

        #region 공격 사정거리
        // 공격 사정거리
        //float AttackRange = userStat.AttackRangeResult(userStat);
        StatTextSetting(AttackRangetxt, sb, userStat.AttackRange, textColorCode, Tag.AttackRange, LocalizationTable.Localization("AttackRange"));
        #endregion

        #region  물리 공격력
        //  물리 공격력
        StatTextSetting(PhysicalPowertxt, sb, HunterStat.PhysicalPowerResult(userStat, false), textColorCode, Tag.PhysicalPower, LocalizationTable.Localization("PhysicalPower"));
        #endregion


        #region  물리 공격력 증가량 (%)
        // 물리 공격력 증가량 (%)
        StatTextSetting(PhysicalPowerPercenttxt, sb, userStat.PhysicalPowerPercent, textColorCode, Tag.PhysicalPowerPercent, LocalizationTable.Localization("PhysicalPowerPercent"));
        #endregion

        #region  마법 공격력
        // 마법 공격력
        StatTextSetting(MagicPowertxt, sb, HunterStat.MagicPowerResult(userStat, false), textColorCode, Tag.MagicPower, LocalizationTable.Localization("MagicPower"));
        #endregion

        #region  마법 공격력 증가량 (%)
        // 마법 공격력 증가량 (%)
        StatTextSetting(MagicPowerPercenttxt, sb, userStat.MagicPowerPercent, textColorCode, Tag.MagicPowerPercent, LocalizationTable.Localization("MagicPowerPercent"));
        #endregion



        #region  치명타 확률 (%)
        // 치명타 확률 (%)
        StatTextSetting(CriChancetxt, sb, userStat.CriChance, textColorCode, Tag.CriChance, LocalizationTable.Localization("CriChance"));
        #endregion

        #region  치명타 피해
        // 치명타 피해
        StatTextSetting(CriDamagetxt, sb, userStat.CriDamage, textColorCode, Tag.CriDamage, LocalizationTable.Localization("CriDamage"));
        #endregion

        #region  일격 확률 (%)
        // 일격 확률 (%)
        StatTextSetting(CoupChancetxt, sb, userStat.CoupChance, textColorCode, Tag.CoupChance, LocalizationTable.Localization("CoupChance"));
        #endregion

        #region  일격 피해
        // 일격 피해
        StatTextSetting(CoupDamagetxt, sb, userStat.CoupDamage, textColorCode, Tag.CoupDamage, LocalizationTable.Localization("CoupDamage"));
        #endregion

        #region  물리공격 방어력
        // 물리공격 방어력
        StatTextSetting(PhysicalPowerDefensetxt, sb, HunterStat.PhysicalPowerDefenseResult(userStat, false), textColorCode, Tag.MagicPowerDefense, LocalizationTable.Localization("PhysicalPowerDefense"));
        #endregion


        #region  물리 공격 방어력 증가량 (%)
        // 물리 공격 방어력 증가량 (%)
        StatTextSetting(PhysicalPowerDefensePercenttxt, sb, userStat.MagicPowerDefensePercent, textColorCode, Tag.PhysicalPowerDefensePercent, LocalizationTable.Localization("PhysicalPowerDefensePercent"));
        #endregion

        #region  마법 공격 방어력
        // 마법 공격 방어력
        StatTextSetting(MagicPowerDefense, sb, HunterStat.MagicPowerDefenseResult(userStat, false), textColorCode, Tag.MagicPowerDefense, LocalizationTable.Localization("MagicPowerDefense"));
        #endregion

        #region  마법 공격 방어력 %
        // 마법 공격 방어력 %
        StatTextSetting(MagicPowerDefensePercenttxt, sb, userStat.MagicPowerDefensePercent, textColorCode, Tag.MagicPowerDefensePercent, LocalizationTable.Localization("MagicPowerDefensePercent"));
        #endregion

        #region  물리 공격 피해 감소
        // 물리 공격 피해 감소
        StatTextSetting(PhysicalDamageReduction, sb, userStat.PhysicalDamageReduction, textColorCode, Tag.PhysicalDamageReduction, LocalizationTable.Localization("PhysicalDamageReduction"));
        #endregion

        #region  마법 공격 피해 감소
        // 마법 공격 피해 감소
        StatTextSetting(MagicDamageReductiontxt, sb, userStat.MagicDamageReduction, textColorCode, Tag.MagicDamageReduction, LocalizationTable.Localization("MagicDamageReduction"));
        #endregion

        //#region   물리 공격 피해 증가량
        ////  물리 공격 피해 증가량
        //StatTextSetting(PhysicalDamageIncreasetxt, sb, userStat.PhysicalDamageIncrease, textColorCode, Tag.PhysicalDamageIncrease, LocalizationTable.Localization("PhysicalDamageIncrease"));
        //#endregion

        //#region   마법 공격 피해 증가량
        ////  마법 공격 피해 증가량
        //StatTextSetting(MagicDamageIncreasetxt, sb, userStat.MagicDamageIncrease, textColorCode, Tag.MagicDamageIncrease, LocalizationTable.Localization("MagicDamageIncrease"));
        //#endregion

        
        #region  상태이상 회복률 (%)
        //  상태이상 회복률 (%)
        StatTextSetting(CCResisttxt, sb, userStat.CCResist, textColorCode, Tag.CCResist, LocalizationTable.Localization("CCResist"));
        #endregion

        #region  회피율 (%)
        //  회피율 (%)
        StatTextSetting(DodgeChancetxt, sb, userStat.DodgeChance, textColorCode, Tag.DodgeChance, LocalizationTable.Localization("DodgeChance"));
        #endregion

        #region  체력
        //  체력
        StatTextSetting(HPtxt, sb, HunterStat.HpResult(userStat, false), textColorCode, Tag.HP, LocalizationTable.Localization("HP"));
        #endregion

        #region  체력 증가량 (%)
        //  체력 증가량 (%)
        StatTextSetting(HPPercenttxt, sb, userStat.HPPercent, textColorCode, Tag.HPPercent, LocalizationTable.Localization("HPPercent"));
        #endregion

        #region  마나
        //  마나
        StatTextSetting(MPtxt, sb, HunterStat.MpResult(userStat, false), textColorCode, Tag.MP, LocalizationTable.Localization("MP"));
        #endregion

        #region  마나 증가량 (%)
        //  마나 증가량 (%)
        StatTextSetting(MPPercenttxt, sb, userStat.MPPercent, textColorCode, Tag.MPPercent, LocalizationTable.Localization("MPPercent"));
        #endregion

        #region  체력흡수
        //  체력흡수
        StatTextSetting(HPDraintxt, sb, userStat.HPDrain, textColorCode, Tag.HPDrain, LocalizationTable.Localization("HPDrain"));
        AddSubText(HPDraintxt, sb, string.Format(LocalizationTable.Localization("DrainStatFormat"),StatManager.Instance.HpDrainChance, StatManager.Instance.HpDrainMaximum));
        #endregion

        #region  마나흡수
        //  마나흡수
        StatTextSetting(MPDraintxt, sb, userStat.MPDrain, textColorCode, Tag.MPDrain, LocalizationTable.Localization("MPDrain"));
        AddSubText(MPDraintxt,sb, string.Format(LocalizationTable.Localization("DrainStatFormat"),StatManager.Instance.MpDrainChance,StatManager.Instance.MpDrainMaximum));
        #endregion

        #region  경험치 획득 버프 (%)
        //  경험치 획득 버프 (%)
        StatTextSetting(ExpBufftxt, sb, userStat.ExpBuff, textColorCode, Tag.ExpBuff, LocalizationTable.Localization("ExpBuff"));
        #endregion

        #region  골드 획득 버프 (%)
        //  골드 획득 버프 (%)
        StatTextSetting(GoldBufftxt, sb, userStat.GoldBuff, textColorCode, Tag.GoldBuff, LocalizationTable.Localization("GoldBuff"));
        #endregion

     
    }

    private void SetSpecialStat(HunterStat userStat, string textColorCode, StringBuilder sb)
    {
        #region   일반몹에게 주는 평타피해 추가
        //  일반몹에게 주는 평타피해 추가
        StatTextSetting(AddDamageToNormalMobtxt, sb, userStat.AddDamageToNormalMob, textColorCode, Tag.AddDamageToNormalMob, LocalizationTable.Localization("AddDamageToNormalMob"));
        #endregion

        #region   보스몹에게 주는 평타피해 추가
        //  보스몹에게 주는 평타피해 추가
        StatTextSetting(AddDamageToBossMobtxt, sb, userStat.AddDamageToBossMob, textColorCode, Tag.AddDamageToBossMob, LocalizationTable.Localization("AddDamageToBossMob"));
        #endregion


        #region  기본공격시 보스 즉사
        // 물리 관통 피해
        StatTextSetting(InstantKillBossOnBasicAttacktxt, sb, HunterStat.Get_InstantKillBossOnBasicAttack(userStat,false), textColorCode, Tag.InstantKillBossOnBasicAttack, LocalizationTable.Localization("InstantKillBossOnBasicAttack"));
        #endregion

        #region  원거리공격반사
        // 물리 관통 피해
        StatTextSetting(ReflectRangetxt, sb, HunterStat.Get_ReflectRange(userStat,false), textColorCode, Tag.ReflectRange, LocalizationTable.Localization("ReflectRange"));
        #endregion

        #region  물리 관통 피해
        // 물리 관통 피해
        StatTextSetting(PhysicalTrueDamagetxt, sb, HunterStat.PhysicalTrueDamageResult(userStat, false), textColorCode, Tag.PhysicalTrueDamage, LocalizationTable.Localization("PhysicalTrueDamage"));
        #endregion

        #region 물리 관통 피해 증가량 (%)
        // 물리 관통 피해 증가량 (%)
        StatTextSetting(PhysicalTrueDamagePercenttxt, sb, userStat.PhysicalTrueDamagePercent, textColorCode, Tag.PhysicalTrueDamagePercent, LocalizationTable.Localization("PhysicalTrueDamagePercent"));
        #endregion

        #region  마법 관통 피해
        // 마법 관통 피해
        StatTextSetting(MagicTrueDamagetxt, sb, HunterStat.MagicTrueDamageResult(userStat, false), textColorCode, Tag.MagicTrueDamage, LocalizationTable.Localization("MagicTrueDamage"));
        #endregion

        #region  마법 관통 피해 증가량%
        // 마법 관통 피해 피해 증가량 %
        StatTextSetting(MagicTrueDamagePercenttxt, sb, userStat.MagicTrueDamagePercent, textColorCode, Tag.MagicTrueDamagePercent, LocalizationTable.Localization("MagicTrueDamagePercent"));
        #endregion
    }

    private void SetHunterSpecialStat( HunterStat userStat, string textColorCode, StringBuilder sb)
    {
        switch (userStat.SubClass)
        {
            case Utill_Enum.SubClass.Archer:
                ArcherSpecisalStatObj.gameObject.SetActive(true);

                GurdianSpecisalStatObj.gameObject.SetActive(false);
                MageSpecisalStatObj.gameObject.SetActive(false);

                #region 궁수 화살 개수
                //궁수 화살개수
                StatTextSetting(ArrowCounttxt, sb, userStat.ArrowCount, textColorCode, Tag.ArrowCount, LocalizationTable.Localization("ArrowCount"));
                #endregion

                #region  화살비 추가데미지
                // 물리 관통 피해
                StatTextSetting(AddDamageArrowRaintxt, sb, HunterStat.Get_AddDamageArrowRain(userStat, false), textColorCode, Tag.AddDamageArrowRain, LocalizationTable.Localization("AddDamageArrowRain"));
                #endregion

                #region 벼락치기 발동 확률 증가 n
                // 벼락치기 발동 확률 증가 n
                StatTextSetting(ElectricCastingChancetxt, sb, HunterStat.Get_ElectricCastingChance(userStat,false), textColorCode, Tag.ElectricCastingChance, LocalizationTable.Localization("ElectricCastingChance"));
                #endregion
                #region  분열관통 분열 수 증가 n
                // 분열관통 분열 수 증가 n
                StatTextSetting(SplitPiercingNumbertxt, sb, HunterStat.Get_SplitPiercingNumber(userStat, false), textColorCode, Tag.SplitPiercingNumber, LocalizationTable.Localization("SplitPiercingNumber"));
                #endregion
                #region  가시덫 투사체 발사 개수 증가 n
                // 가시덫 투사체 발사 개수 증가 n
                StatTextSetting(SpikeTrapNumbertxt, sb, HunterStat.Get_SpikeTrapNumber(userStat, false), textColorCode, Tag.SpikeTrapNumber, LocalizationTable.Localization("SpikeTrapNumber"));
                #endregion
                #region  가시덫 장판 범위 증가 n
                // 가시덫 장판 범위 증가 n
                StatTextSetting(SpikeTrapDamageRadiustxt, sb, HunterStat.Get_SpikeTrapDamageRadius(userStat, false), textColorCode, Tag.SpikeTrapDamageRadius, LocalizationTable.Localization("SpikeTrapDamageRadius"));
                #endregion
                #region  맹독침 투사체 발사 개수 증가 n
                //맹독침 투사체 발사 개수 증가 n
                StatTextSetting(VenomousStingNumbertxt, sb, HunterStat.Get_VenomousStingNumber(userStat, false), textColorCode, Tag.VenomousStingNumber, LocalizationTable.Localization("VenomousStingNumber"));
                #endregion
                #region 맹독침 폭발 범위 증가 n
                // 맹독침 폭발 범위 증가 n
                StatTextSetting(VenomousStingDamageRadiustxt, sb, HunterStat.Get_VenomousStingDamageRadius(userStat, false), textColorCode, Tag.VenomousStingDamageRadius, LocalizationTable.Localization("VenomousStingDamageRadius"));
                #endregion

                break;
            case Utill_Enum.SubClass.Guardian:
                GurdianSpecisalStatObj.gameObject.SetActive(true);

                ArcherSpecisalStatObj.gameObject.SetActive(false);
                MageSpecisalStatObj.gameObject.SetActive(false);

                #region 감전개수 추가     
                StatTextSetting(ElectricshockNumbertxt, sb, HunterStat.Get_ElectricshockNumber(userStat, false), textColorCode, Tag.ElectricshockNumber, LocalizationTable.Localization("ElectricshockNumber"));
                #endregion
                #region 야수화 필요스택 n 감소     
                StatTextSetting(BeastTransformationStackCountReducetxt, sb, HunterStat.Get_BeastTransformationStackCountReduce(userStat, false), textColorCode, Tag.BeastTransformationStackCountReduce, LocalizationTable.Localization("BeastTransformationStackCountReduce"));
                #endregion
                #region 연타 필요스택 n 감소     
                StatTextSetting(ContinuousHitStackCountReducetxt, sb, HunterStat.Get_ContinuousHitStackCountReduce(userStat, false), textColorCode, Tag.ContinuousHitStackCountReduce, LocalizationTable.Localization("ContinuousHitStackCountReduce"));
                #endregion
                #region 도깨비불 회전 속도 증가 n
                StatTextSetting(DokkaebiFireRotationSpeedtxt, sb, HunterStat.Get_DokkaebiFireRotationSpeed(userStat, false), textColorCode, Tag.DokkaebiFireRotationSpeed, LocalizationTable.Localization("DokkaebiFireRotationSpeed"));
                #endregion

                break;
            case Utill_Enum.SubClass.Mage:
                MageSpecisalStatObj.gameObject.SetActive(true);

                ArcherSpecisalStatObj.gameObject.SetActive(false);
                GurdianSpecisalStatObj.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 스탯 텍스트를 세팅함
    /// </summary>
    /// <param name="statText">세팅 받을 텍스트</param>
    /// <param name="sb">미리 만들어놓은 스트링빌더 넣어야함</param>
    /// <param name="value">해당 스탯의 값</param>
    /// <param name="textColorCode">스탯 최대치 초과시 표시할 색상</param>
    /// <param name="Tag">스탯의 이름(Tag클래스에서 가져올 수 있음)</param>
    /// <param name="localizationKey">스탯의 한글 이름</param>

    private void StatTextSetting(TextMeshProUGUI statText, StringBuilder sb, float value, string textColorCode, string Tag, string localizationKey)
    {
        float curValue = value;
        // 멕시멈을 넘겼냐
        int maxValue = StatTableData.GetValue_MaximumData(GameDataTable.Instance.StatTableDataDic, Tag);
        float sum_Value = curValue - (float)maxValue;

        // 로컬라이즈된 태그를 사용하여 문자열 생성
        sb.Append(localizationKey);
        if (curValue >= maxValue)
        {
            sb.Append(": ").Append("<color=").Append(textColorCode).Append(">").Append(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, Tag,maxValue));
            sb.Append("(");
            sb.Append("\u2191").Append(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, Tag, sum_Value));
            sb.Append(")").Append("</color>");
        }
        else
        {
            sb.Append(": ").Append("<color=").Append(defaultTextColorCode).Append(">").Append(StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, Tag, curValue));
            sb.Append("</color>");
        }
        statText.text = sb.ToString();
        sb.Clear();
    }

    /// <summary>
    /// 스탯 옆에 작게 써져있는 글씨를 포멧에따라 추가
    /// </summary>
    /// <param name="statText">세팅 받을 텍스트</param>
    /// <param name="sb">미리 만들어놓은 스트링빌더 넣어야함</param>
    /// <param name="subText">추가할 글</param>
    private void AddSubText(TextMeshProUGUI statText, StringBuilder sb, string subText)
    {
        sb.Append(statText.text);
        sb.Append($"<size={subTextSize}>");
        sb.Append("<color=").Append(defaultTextColorCode).Append(">");
        sb.Append(subText);
        sb.Append("</size>");
        sb.Append("</color>");
        statText.text = sb.ToString();
        sb.Clear();
    }


}
