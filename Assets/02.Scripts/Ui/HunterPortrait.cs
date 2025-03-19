using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Utill_Enum;
using NSY;

/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : HunterPortrait 자체 ui 처리                                
/// </summary> 
public class HunterPortrait : MonoBehaviour
{
    public Image lockImage;
    public Image equipImage;
    public Image activeImage;
    public Image coolDownOverlay;
    public TextMeshProUGUI levelText;
    public GameObject exp;
    public bool isEquip = false;
    public Button equipBtn;
    public Utill_Enum.SubClass characterType;
    private bool isCoolDown = false;
    public float equipCooldownDuration = 0f; // Cooldown time in seconds
    public float equipReleaseCooldownDuration = 0f; // Cooldown time in seconds
    private float cooldownTimer;

    Coroutine reEquipCoroutine = null; //특정 시간 후 재장착 해주는 코루틴(사망후 재장착)

    public void Start()
    {
        Invoke("TimeSetting", 1f); 
        equipBtn.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            //컨트롤 누른채로 클릭하였다면 장착이 아니라 다른 기능을 수행함(디버그 기능)
            if (InputManager.Instance.IsHoldKey(KeyCode.LeftControl) && isEquip && !isCoolDown) 
            {
                //현재 캐릭터 사망
                DamageInfo damageInfo = new(int.MaxValue, false, null);
                DataManager.Instance.Hunters[(int)characterType].Damaged(damageInfo);
                return;
            }
#endif
            Equip(); 
                //사용중이던 스킬도 on해야함 NSY 코드추가.
            });
    }

    /// <summary>
    /// 쿨다운 타임 세팅
    /// </summary>
    void TimeSetting()
    {
        equipCooldownDuration = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_EQUIP_COOLDOWN].Value;
        equipReleaseCooldownDuration = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_UNEQUIP_COOLDOWN].Value;
    }
 
    /// <summary>
    /// 쿨다운 코루틴
    /// </summary>
    private IEnumerator CooldownRoutine(float cooldownDuration)
    {
        coolDownOverlay.fillAmount = 0;
        coolDownOverlay.gameObject.SetActive(true);

        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            isCoolDown = true;
            coolDownOverlay.fillAmount = (cooldownDuration - cooldownTimer) / cooldownDuration;
            yield return null;
        }
        isCoolDown = false;
        coolDownOverlay.fillAmount = 0;
    }

    public void Equip()
    {
        if (isCoolDown)
            return;
        if (IdleModeRestCycleSystem.Instance.IsRestMode()) //휴식중이라면 현재 프리셋을 수정 불가능
        {
            string strformat = string.Format(LocalizationTable.Localization("SystemNotice_NotClickPresetButton") , "휴식");
            SystemNoticeManager.Instance.SystemNotice(strformat, SystemNoticeType.Default);
            return;
        }


        SubClass currentsubclass = SubClass.None;
        if (GameDataTable.Instance.User.CurrentDieHunter.Count > 0)
        {
             currentsubclass = GameDataTable.Instance.User.CurrentDieHunter.Find(c => c.ToString() == characterType.ToString());
            if (currentsubclass == SubClass.None)
            {
                currentsubclass = SubClass.None;
            }
        }
        else
        {
            currentsubclass = SubClass.None;
        }


        if (GroundCreatSystem.Instance.isBossStage && currentsubclass != SubClass.None)
        {
            //보스전에는 부활이 불가능 알림뜨기
            string strformat = string.Format(LocalizationTable.Localization("SystemNotice_NotClickPresetButton"), "보스");
            SystemNoticeManager.Instance.SystemNotice(strformat, SystemNoticeType.Default);
            return;
        }


        int CurrentEquipHunterCnt = GameDataTable.Instance.User.CurrentEquipHunter.Count;

        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.FIGHTABLE_NUMBER].Value;

        if (value == 1)
        {
            for (int i = 0; i < HunterPortraitSystem.Instance.hunterPortraitView.hunterPortraits.Length; i++)
            {
                HunterPortraitSystem.Instance.hunterPortraitView.hunterPortraits[i].isEquip = false;
            }
        }
        else if (value == 2 && isEquip == false && CurrentEquipHunterCnt >= 2)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_CannotEquipMoreHunter"), SystemNoticeType.Default);
            return;
        }
       
        isEquip = !isEquip;

        // Update equip status
        if (isEquip)
        {
            SoundManager.Instance.PlayAudio("UseBuffBook");
            
            cooldownTimer = equipCooldownDuration;
            StartCoroutine(CooldownRoutine(equipCooldownDuration));
        }
        else
        {
            if (CurrentEquipHunterCnt <= 1)
            {
                isEquip = true;
                SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_DismountableHunter"), SystemNoticeType.Default);
                SoundManager.Instance.PlayAudio("SystemNotice");
                return;
            }

            TimeSetting();
            cooldownTimer = equipReleaseCooldownDuration;
            StartCoroutine(CooldownRoutine(equipReleaseCooldownDuration));
        }
        HunterPortraitSystem.Instance.hunterPortraitView.Equip();
        HunterPortraitSystem.Instance.SettingBattle();
        //skillui해제 ...nsy
        GameEventSystem.Send_GameHunterReviveSkillInit_GameEventHandler_Event(characterType, isEquip);
    }

    /// <summary>
    /// duration 만큼 쿨타임 시작
    /// </summary>
    public void CoolDownPortrait(float duration)
    {
        equipReleaseCooldownDuration = duration;
        cooldownTimer = equipReleaseCooldownDuration;
        StopCoroutine(CooldownRoutine(equipReleaseCooldownDuration));
        StartCoroutine(CooldownRoutine(equipReleaseCooldownDuration));
    }

    /// <summary>
    /// duration후 장착
    /// </summary>
    public void EquipAfterDuration(float duration,SubClass subClass)
    {
        //만약 보스전일때면 리턴
        if (GroundCreatSystem.Instance.isBossStage)
        {
            return;
        }

        // 이미 진행 중인 Coroutine이 있으면 중지
        StopReEquipCoroutine(); 
        reEquipCoroutine = StartCoroutine(EquipAfterDurationCoroutine(duration, subClass));
    }

    private IEnumerator EquipAfterDurationCoroutine(float duration,SubClass subClass)
    {
        yield return new WaitForSeconds(duration);
        if (GameDataTable.Instance.User.CurrentDieHunter.Contains(subClass) == false || GameDataTable.Instance.User.CurrentEquipHunter.Contains(subClass)) 
            yield break;
        isEquip = true;
        GameDataTable.Instance.User.CurrentDieHunter.Remove(subClass);
        if(!GameDataTable.Instance.User.CurrentEquipHunter.Contains(subClass))
            GameDataTable.Instance.User.CurrentEquipHunter.Add(subClass);
        HunterPortraitSystem.Instance.SettingBattle(true);
        //스킬 활성화도 시켜줌
        GameEventSystem.Send_GameHunterReviveSkillInit_GameEventHandler_Event(subClass, true);
        StopReEquipCoroutine();
    }
    public void StopReEquipCoroutine()
    {
        if (reEquipCoroutine != null)
        {
            StopCoroutine(reEquipCoroutine);
            reEquipCoroutine = null;
        }
    }

    public void OnHunterDie()
    {
        StopAllCoroutines();
    }
  
    public void InitBuyCell(bool isBuy)
    {
        if (isBuy)
        {
            coolDownOverlay.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(false);
            equipImage.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
            exp.gameObject.SetActive(true);
        }
        else
        {
            coolDownOverlay.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            equipImage.gameObject.SetActive(true);
            levelText.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(false);
        }

        if (isEquip)
        {
            coolDownOverlay.gameObject.SetActive(false);
            equipImage.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(true);
        }
    }
    //경험치바
    public void InitCell(bool isEquip)
    {
        if (isEquip)
        {
            coolDownOverlay.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(false);
            equipImage.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
        }
        else
        {
            coolDownOverlay.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            equipImage.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
            exp.SetActive(false);
        }
    }
}
