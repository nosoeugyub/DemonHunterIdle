using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using System;

/// < summary >
/// 작성일자   : 2024-07-18
/// 작성자     : 민영(gksalsdud1234@gmail.com)
/// 클래스용도 : 최후의 저항 스킬
/// </summary>
public class LastLeaf : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    [Header("물리 방어력 퍼센트 증가 수치")]
    public int physicalPowerDefensePercent = 0;
    #endregion

    private GameObject fx; //이펙트 담는 용도
    public int skillusecount = 0;
    LastLeafData lastLeafdata;
    private Coroutine currentCoroutine;
    float currentHpPercent = 0;


    public Color lastcolor1;
    public Color lastcolor2;
    public Color lastcolor3;
    public Color Defaultcolor;
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(OnLaskLeaf());

    }

    public override void Init_Skill()
    {
        fx = null;
        currentHpPercent = 0;
        // 모든 버프를 비활성화로 설정
        foreach (var key in activeBuffs.Keys)
        {
            if (activeBuffs[key] == true)//버프가 하나라도 있고
            {
                if (currentHpPercent >= 90)
                {
                    //90%이상이면
                    SoundManager.Instance.PlayAudio(Tag.LastLeaf_Off);
                    //버프제거
                    RemoveBuff(temppercent);
                }
            }
        }
    }
    float temppercent = -1;
    // 활성화된 버프를 추적하기 위한 Dictionary
    private Dictionary<string, bool> activeBuffs = new Dictionary<string, bool>
{
    { "61-90", false },
    { "31-60", false },
    { "0-30", false }
};

    IEnumerator OnLaskLeaf()
    {
        float Hunterhp = hunter.Orginstat.CurrentHp; // 현재 HP
        float HunterMaxhp = hunter.Orginstat.HP; // 최대 HP 
        ParticleSystem particle = new ParticleSystem();
        float currentHpPercent = (Hunterhp / HunterMaxhp) * 100f; // 현재 HP가 최대 HP의 몇 퍼센트인지 계산
        var mainModule = particle.main;
        if (fx == null)
        {
            // 파티클 효과가 없다면 생성합니다.
            fx = ObjectPooler.SpawnFromPool(Tag.LastLeaf, hunter.transform.position);
            particle = fx.GetComponent<ParticleSystem>();
            // x 회전값을 90도로 설정
            fx.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            mainModule = particle.main;
            // 이펙트를 초기화하고 사운드 재생
            Color startColor = mainModule.startColor.color;
            startColor = Defaultcolor; // 알파값을 200으로 설정 (0~1 범위로 변환)
            mainModule.startColor = startColor;

        }
        else
        {
            fx.transform.SetParent(hunter.transform);
            if (particle == null)
            {
                particle = fx.GetComponent<ParticleSystem>();
            }
        }

        fx.transform.SetParent(hunter.transform);
        // 파티클의 MainModule 가져오기
    
        if (temppercent != currentHpPercent)
        {
            // 모든 버프를 비활성화로 설정
            foreach (var key in activeBuffs.Keys)
            {
                if (activeBuffs[key] == true)//버프가 하나라도 있고
                {
                    if (currentHpPercent >= 90)
                    {
                        //90%이상이면
                        SoundManager.Instance.PlayAudio(Tag.LastLeaf_Off);
                    }
                }
            }
            temppercent = currentHpPercent;
        }


        mainModule = particle.main;
        // 현재 HP 퍼센트에 따라 버프 적용 및 파티클 색상 변경
        if (currentHpPercent >= 60f && currentHpPercent <= 90f && !activeBuffs["61-90"])
        {
            RemoveBuff(temppercent);

            // Dictionary에서 해당 범위의 버프를 활성화합니다.
             SetBuffActive("61-90");
            // 버프1 적용 (90 ~ 61%)
            Color startColor = mainModule.startColor.color;
            startColor = lastcolor1; // 알파값을 200으로 설정 (0~1 범위로 변환)
            mainModule.startColor = startColor;

            ApplyBuff(lastLeafdata.CHGValue_CriChance, lastLeafdata.CHGValue_CriDamage, lastLeafdata.CHGValue_AttackSpeedPercent1);
            skillusecount++;
        }
        else if (currentHpPercent >= 30f && currentHpPercent < 60f && !activeBuffs["31-60"])
        {
            RemoveBuff(temppercent);

            // Dictionary에서 해당 범위의 버프를 활성화합니다.
            SetBuffActive("31-60");

            // 버프2 적용 (60 ~ 31%)
            Color startColor = mainModule.startColor.color;
            startColor = lastcolor2; // 알파값을 200으로 설정 (0~1 범위로 변환)
            mainModule.startColor = startColor;

            ApplyBuff(lastLeafdata.CHGValue_CriChance2, lastLeafdata.CHGValue_CriDamage2, lastLeafdata.CHGValue_AttackSpeedPercent2);
            skillusecount++;
        }
        else if (currentHpPercent >= 0f && currentHpPercent < 30f && !activeBuffs["0-30"])
        {
            RemoveBuff(temppercent);

            // Dictionary에서 해당 범위의 버프를 활성화합니다.
            SetBuffActive("0-30");

            // 버프3 적용 (30 ~ 0%)
            Color startColor = mainModule.startColor.color;
            startColor = lastcolor3; // 알파값을 200으로 설정 (0~1 범위로 변환)
            mainModule.startColor = startColor;

            ApplyBuff(lastLeafdata.CHGValue_CriChance3, lastLeafdata.CHGValue_CriDamage3, lastLeafdata.CHGValue_AttackSpeedPercent3);
            skillusecount++;
        }
        else
        {
            if (currentHpPercent > 90)
            {
                // 이펙트를 초기화하고 사운드 재생
                Color startColor = mainModule.startColor.color;
                startColor = Defaultcolor; // 알파값을 200으로 설정 (0~1 범위로 변환)
                mainModule.startColor = startColor;
            }
        }

        yield return null;
    }

    // 버프를 활성화하고 중복 적용을 방지하는 메서드
    private void SetBuffActive(string range)
    {
        // Dictionary에서 수정할 키와 값을 따로 저장
        List<string> keysToDisable = new List<string>(activeBuffs.Keys);

        // 모든 버프를 비활성화로 설정
        foreach (var key in keysToDisable)
        {
            activeBuffs[key] = false;
        }

        // 해당 범위의 버프를 활성화로 설정
        if (activeBuffs.ContainsKey(range))
        {
            activeBuffs[range] = true;
            SoundManager.Instance.PlayAudio(skillName);
        }
    }

    // 버프 제거
    private void RemoveBuff(float currentHpPercent)
    {
        // 현재 HP 범위에 따라 활성화된 버프를 제거
        if (activeBuffs["61-90"])
        {
            // Dictionary에서 해당 버프를 비활성화
            activeBuffs["61-90"] = false;

            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_CriChance, Utill_Enum.Upgrade_Type.CriChance);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_CriDamage, Utill_Enum.Upgrade_Type.CriDamage);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_AttackSpeedPercent1, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
            skillusecount--;
        }
        else if ( activeBuffs["31-60"])
        {
            // Dictionary에서 해당 버프를 비활성화
            activeBuffs["31-60"] = false;

            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_CriChance2, Utill_Enum.Upgrade_Type.CriChance);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_CriDamage2, Utill_Enum.Upgrade_Type.CriDamage);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_AttackSpeedPercent2, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
            skillusecount--;
        }
        else if ( activeBuffs["0-30"])
        {
            // Dictionary에서 해당 버프를 비활성화
            activeBuffs["0-30"] = false;

            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_CriChance3, Utill_Enum.Upgrade_Type.CriChance);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_CriDamage3, Utill_Enum.Upgrade_Type.CriDamage);
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, lastLeafdata.CHGValue_AttackSpeedPercent3, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
            skillusecount--;
        }
    }


    //버프 적용
    private void ApplyBuff(float crichance, float cridamage, float atkspeed)
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, crichance, Utill_Enum.Upgrade_Type.CriChance);
        HunterStat.UseSkillAddUserStat(hunter._UserStat, cridamage, Utill_Enum.Upgrade_Type.CriDamage);
        HunterStat.UseSkillAddUserStat(hunter._UserStat, atkspeed, Utill_Enum.Upgrade_Type.AttackSpeedPercent);
    }




    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        lastLeafdata = (LastLeafData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.LastLeaf, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, lastLeafdata.CHGValue_UseMP); //마나감소
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
