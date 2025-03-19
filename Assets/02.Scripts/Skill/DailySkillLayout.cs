using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬 슬롯 레이아웃
/// /// </summary>
public class DailySkillLayout : MonoBehaviour
{
    [SerializeField] private Utill_Enum.SubClass currenyClass;
    public Utill_Enum.SubClass CurrenyClass
    {
        get { return currenyClass; }
        set { currenyClass = value; }
    }
    public SkillUiElement[] HunterSKillarray;
    public SkillManager skillmanager;

    public BattleSkillUi[] BattleSkillUis;

    [HideInInspector] public Basic_Prefab[] PresetButton;
    [HideInInspector] public Button[] PrestButtonClick;
    [Space(10)]
    private SkillUiElement[,] Skillelementdoublearray;
    private int RowIndex;
    private int ClickIndex;
    [Space(10)]
  [HideInInspector]  public string[] preset1;
    [HideInInspector] public string[] preset2;
    [HideInInspector] public string[] preset3;

  
    public void SplitHunterSkillSlotIndex(string[] source)
    {
        // 각 행의 데이터를 담을 1차원 배열 초기화
        preset1 = new string[4];
        preset1 = source;
    }


    //게임시작할때 한번만 불러오는함수
    public void Init_Slots(List<ISkill> currentequippedSkills)
    {
        return;

        //데이터 분배
        SplitHunterSkillSlotIndex(GameDataTable.Instance.User.DailySkillSlotIndex);
        // 각 슬롯에 스킬 데이터 할당
        for (int i = 0; i < HunterSKillarray.Length; i++)
        {
            // 해당 슬롯의 데이터 찾기
            ISkill skill = currentequippedSkills.Find(c => c != null && c.SkillName == HunterSKillarray[i].SkillName);
            // 데이터 바인딩
            HunterSKillarray[i].m_Skills = skill;
            HunterSKillarray[i].Index = i;
            HunterSKillarray[i].SpriteName = skill?.SkillName ?? string.Empty;
            // UI 업데이트
            HunterSKillarray[i].UpdateSkillUi();
            SkillUiElement skillUiElement = HunterSKillarray[i];

            HunterSKillarray[i].ClickSkillBtn.onClick.RemoveAllListeners();
            HunterSKillarray[i].ClickSkillBtn.onClick.AddListener(delegate { ClickSlot(skill, skillUiElement); });
        }

        string[] preset = GameDataTable.Instance.User.DailySkillSlotIndex;

        //battleUi에도 이미지 업데이트를 해줘야함 
        //Update_BattleUiView(Utill_Enum.SubClass.Weekly, preset);
        //장착된 스킬이있다면 스킬사용
        int j = 0;
        for (int i = 0; i < 4; i++)
        {
            if (preset[j] != null)
            {
                ISkill skill = skillmanager.Get_SkillForName(preset[i]);
                if (skill == null)
                {
                    continue;
                }
                UseSKill(i, skill);
            }
            else
            {
                continue;
            }
            j++;
        }
    }

    public void presetClick()
    {
        //사운드부터
        SoundManager.Instance.PlayAudio("SkillEquipment");
        //프리셋 데이터가져오기
        string[] temp = null;
        //유저의 요일스킬 가져오기
        temp = GameDataTable.Instance.User.DailySkillSlotIndex;

        //프리셋데이터로 ui바꿔줘야함
        UpdateView(temp);
    }


    //장착버튼을 눌렀을때
    private void ClickSlot(ISkill skill, SkillUiElement slot)
    {
        SoundManager.Instance.PlayAudio("SkillEquipment");

        //요일스킬장착
        if (slot.Clickindex == 0)
        {
            slot.Clickindex++;
            // 장착 가능한 경우
            slot.isEquip = true;
            //데이터에서도 추가
            User.User_DailyAddSkill(GameDataTable.Instance.User, slot.Index , slot.SkillName);
           //ui업데이트
            slot.UpdateSkillUi();
            //battleui도 변경
            Update_BattleUiView(CurrenyClass, GameDataTable.Instance.User.DailySkillSlotIndex);
            //스킬 사용
            UseSKill(slot.Rowindex, slot.m_Skills);
        }
        else//요일스킬해제
        {
            slot.isEquip = false;
            slot.Clickindex = 0;
            //해당 스킬이 몇번째 줄인지 알아야함

            //데이터에서도 제거
            User.User_DailyRemoveSkill(GameDataTable.Instance.User, slot.Index);
            // ui업데이트    
            slot.UpdateSkillUi();
            //battleui도 변경
            Update_BattleUiView(CurrenyClass, GameDataTable.Instance.User.DailySkillSlotIndex);
            //스킬취소
            UnUseSkill(slot.Rowindex, slot.m_Skills);

            return;
        }

       
          


    }

    // 해당 스킬 열에 장착한 스킬이 있는지 검사
    private (bool isEquip, SkillUiElement equipmentslot) CheckEquip(SkillUiElement slot)
    {
        int row = slot.Rowindex;
        bool isEquip = false;
        SkillUiElement equipmentslot = new SkillUiElement();
        for (int col = 0; col < 4; col++)
        {
            if (Skillelementdoublearray[row - 1, col].isEquip)
            {
                //만약 이미 장착한 슬롯이있으면 슬롯과 bool값반환
                isEquip = true;
                equipmentslot = Skillelementdoublearray[row - 1, col];

                if (equipmentslot == null)
                {
                    equipmentslot = new SkillUiElement();
                }

                return (isEquip, equipmentslot);
            }
        }
        return (isEquip, equipmentslot);
    }

    //한번만 호출하는 베틀유아이에서 보이느 스킬 이미지;
    public void Update_BattleUiView(Utill_Enum.SubClass subclass, string[] preset)
    {
        //switch (subclass)
        //{
        //    case Utill_Enum.SubClass.Weekly:
        //        for (int i = 0; i < BattleSkillUis.Length; i++)
        //        {
        //            //BattleSkillUis[i].ChanngeSprite(preset[i]);

        //            //스킬이름으로 스킬인터페이스 생성
        //         //   ISkill skill = skillmanager.Get_SkillForName(preset[i]);
        //          //  BattleSkillUis[i].Skill = skill;
        //           // BattleSkillUis[i].isUnuse = false;
        //        }
        //        break;

        //}
    }
    public void UpdateView(string[] userSKill)
    {
        foreach (var element in HunterSKillarray)
        {
            element.isEquip = false;
            element.Clickindex = 0;
            element.UpdateSkillUi();

        }
        // userSkill 배열의 각 스킬을 찾아서 isEquip을 true로 설정
        for (int i = 0; i < userSKill.Length; i++)
        {
            string skillName = userSKill[i];
            for (int j = 0; j < HunterSKillarray.Length; j++)
            {
                if (HunterSKillarray[j].SkillName == skillName)
                {
                    HunterSKillarray[j].isEquip = true;
                    HunterSKillarray[j].Clickindex = 1;
                    HunterSKillarray[j].UpdateSkillUi();
                    //바로 스킬사용
                    UseSKill(i , HunterSKillarray[j].m_Skills);

                    break; // 스킬을 찾으면 더 이상 검색할 필요 없음
                }
            }
        }

    }



    public void UseSKill(int index, ISkill skill)
    {
        BattleSkillUis[index].isUnuse = true;
        //BattleSkillUis[index].InitSlot(skill.SkillCoolDown, skill, Utill_Enum.SubClass.Weekly);
        BattleSkillUis[index].StartCooldown();
    }

    public void UnUseSkill(int index, ISkill skill)
    {
        //BattleSkillUis[index].InitSlot(10, skill, Utill_Enum.SubClass.Weekly);
        BattleSkillUis[index].StopCooldown();
    }
}
