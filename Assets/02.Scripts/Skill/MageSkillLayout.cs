using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-07-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 마법사스킬레이아웃
/// /// </summary>
public class MageSkillLayout : MonoBehaviour
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

    public Basic_Prefab[] PresetButton;
    public Button[] PrestButtonClick;
    [Space(10)]
    private SkillUiElement[,] Skillelementdoublearray;
    private int RowIndex;
    private int ClickIndex;
    [Space(10)]
    public string[] preset1;
    public string[] preset2;
    public string[] preset3;

    public int CurrentPreset;
    public void SplitHunterSkillSlotIndex(string[,] source)
    {
        // 2차원 배열의 행과 열 길이 계산
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);

        // 각 행의 데이터를 담을 1차원 배열 초기화
        preset1 = new string[cols];
        preset2 = new string[cols];
        preset3 = new string[cols];

        // 배열이 3행 2열을 갖는다고 가정하고, 각 행의 데이터를 분리
        for (int j = 0; j < cols; j++)
        {
            preset1[j] = source[0, j];
            preset2[j] = source[1, j];
            preset3[j] = source[2, j];
        }
    }


    //게임시작할때 한번만 불러오는함수
    public void Init_Slots(List<ISkill> currentequippedSkills)
    {
        Skillelementdoublearray = new SkillUiElement[4, 4];
        //데이터 분배
        SplitHunterSkillSlotIndex(GameDataTable.Instance.User.MageSkillSlotindex);
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
        //버튼이벤트

        PrestButtonClick[0].onClick.RemoveAllListeners(); //재시작 고려하여 우선 모든 리스너를 삭제시켜줌
        PrestButtonClick[1].onClick.RemoveAllListeners();
        PrestButtonClick[2].onClick.RemoveAllListeners();

        PrestButtonClick[0].onClick.AddListener(delegate { presetClick(0); });
        PrestButtonClick[1].onClick.AddListener(delegate { presetClick(1); });
        PrestButtonClick[2].onClick.AddListener(delegate { presetClick(2); });



        // 데이터 변환 및 RowIndex 설정
        for (int i = 0; i < HunterSKillarray.Length; i++)
        {
            int row = i / 4;
            int col = i % 4;
            if (row < 4 && col < 4) // 배열 인덱스 범위 체크
            {
                Skillelementdoublearray[row, col] = HunterSKillarray[i];
                HunterSKillarray[i].Rowindex = row + 1; // RowIndex 설정
            }
            else
            {
                Debug.LogError($"Index out of bounds: row={row}, col={col}");
            }
        }

        //프리셋 0번으로 일단 설정
        CurrentPreset = GameDataTable.Instance.User.Euipmentuserpreset[2];
        string[] preset = GetPreset(CurrentPreset);

        //battleUi에도 이미지 업데이트를 해줘야함 
        Update_BattleUiView(Utill_Enum.SubClass.Mage, preset);
        //장착된 스킬이있다면 스킬사용
        int j = 1;
        for (int i = 0; i < 4; i++)
        {
            if (preset[j] != null)
            {
                ISkill skill = skillmanager.Get_SkillForName(preset[i + 1]);
                if (skill == null)
                {
                    continue;
                }
                UseSKill(i, skill);

                if (GameStateMachine.Instance.CurrentState is NonCombatState) //현재 비전투 상태라면 (일반적인 상태라면 이곳으로 들어감)
                {
                    SkillManager.Instance.PauseSkill(CurrenyClass, skill);
                }
            }
            else
            {
                continue;
            }
            j++;
        }
    }

    public void presetClick(int i)
    {
        //사운드부터
        SoundManager.Instance.PlayAudio("SkillEquipment");
        //프리셋 데이터가져오기
        string[] temp = null;
        bool isbuy = false;

        switch (i)
        {
            case 0: //1번프리셋
                temp = GetPreset(i);
                isbuy = User.isCheckBuySkillPreset(GameDataTable.Instance.User, Utill_Enum.SubClass.Mage, i);
                break;
            case 1: //2번프리셋
                temp = GetPreset(i);
                isbuy = User.isCheckBuySkillPreset(GameDataTable.Instance.User, Utill_Enum.SubClass.Mage, i);
                break;
            case 2: //3번프리셋
                temp = GetPreset(i);
                isbuy = User.isCheckBuySkillPreset(GameDataTable.Instance.User, Utill_Enum.SubClass.Mage, i);
                break;
        }
        //유저가 프리셋을 가지고있는지 확인
        if (isbuy) //구매가되있으면
        {
            //프리셋 ui도 다시..
            for (int zi = 0; zi < PresetButton.Length; zi++)
            {
                if (i == zi)
                {
                    PresetButton[zi].SetTypeButton(Utill_Enum.ButtonType.DeActive);
                }
                else
                {
                    PresetButton[zi].SetTypeButton(Utill_Enum.ButtonType.Active);
                }
            }
           
           
            //이미 같은 프리셋이면 리턴
            if (CurrentPreset == i)
            {
                //프리셋데이터로 ui바꿔줘야함
                UpdateView(temp, i);
                return;
            }
            else
            {
                //프리셋 변경전에 기존스킬 쿨타임 초기회
                for (int j = 0; j < BattleSkillUis.Length; j++)
                {
                    BattleSkillUis[j].StopCooldown();
                }
            }
            //프리셋도 변경
            CurrentPreset = i;
            User.Set_Presetinedex(GameDataTable.Instance.User, CurrentPreset, Utill_Enum.SubClass.Mage);
            //battleUi에도 이미지 업데이트를 해줘야함 
            Update_BattleUiView(Utill_Enum.SubClass.Mage, GetPreset(CurrentPreset));
            //프리셋데이터로 ui바꿔줘야함
            UpdateView(temp, i);
        }
        else
        {

            return;
        }
    }




    //장착버튼을 눌렀을때
    private void ClickSlot(ISkill skill, SkillUiElement slot)
    {
        SoundManager.Instance.PlayAudio("SkillEquipment");

        // 해당 스킬 열에 있는 스킬 중 장착한 스킬이 있는지 검사
        var reslut = CheckEquip(slot);

        //슬롯에 있는 클릭 카운트 먼저 검사
        if (slot.Clickindex == 0)
        {
            slot.Clickindex++;
        }
        else
        {
            slot.isEquip = false;
            slot.Clickindex = 0;
            //해당 스킬이 몇번째 줄인지 알아야함

            //데이터에서도 제거
            User.User_RemoveSkill(GameDataTable.Instance.User, reslut.equipmentslot.SkillName, CurrenyClass, slot.Rowindex, CurrentPreset);
            // ui업데이트
            slot.UpdateSkillUi();

            //battleui도 변경
            Update_BattleUiView(CurrenyClass, GetPreset(CurrentPreset));
            //스킬취소
            UnUseSkill(slot.Rowindex - 1, slot.m_Skills);

            return;
        }

        if (reslut.isEquip)
        {
            // 이미 장착된 스킬이 있는 경우 해당슬롯은 장착해제하고 ui적용
            reslut.equipmentslot.isEquip = false;
            reslut.equipmentslot.Clickindex = 0; //스킬 클릭카운트도 초기화
            reslut.equipmentslot.UpdateSkillUi();
            //데이터에서도 제거
            User.User_RemoveSkill(GameDataTable.Instance.User, reslut.equipmentslot.SkillName, CurrenyClass, slot.Rowindex, CurrentPreset);
            //스킬취소
            UnUseSkill(slot.Rowindex - 1, reslut.equipmentslot.m_Skills);

            // 유저가 누른 슬롯을 장착
            slot.isEquip = true;
            //데이터에서도 추가
            User.User_AddSkill(GameDataTable.Instance.User, slot.SkillName, CurrenyClass, slot.Rowindex, CurrentPreset);
            // skillmanager.EquipSkill(skill.SkillID, slot.RowIndex);
            slot.UpdateSkillUi();
            //battleui도 변경
            Update_BattleUiView(CurrenyClass, GetPreset(CurrentPreset));
            //스킬 사용
            //스킬 사용
            UseSKill(slot.Rowindex - 1, slot.m_Skills);
        }
        else
        {
            // 장착 가능한 경우
            slot.isEquip = true;
            //데이터에서도 추가
            User.User_AddSkill(GameDataTable.Instance.User, slot.SkillName, CurrenyClass, slot.Rowindex, CurrentPreset);
            // skillmanager.EquipSkill(skill.SkillID, slot.RowIndex);
            slot.UpdateSkillUi();
            //battleui도 변경
            Update_BattleUiView(CurrenyClass, GetPreset(CurrentPreset));
            //스킬 사용
            UseSKill(slot.Rowindex - 1, slot.m_Skills);
        }

        if (GameStateMachine.Instance.CurrentState is NonCombatState) //현재 비전투 상태라면
        {
            BattleSkillUis[slot.Rowindex - 1].CooldownOverlay.fillAmount = 0;
            SkillManager.Instance.PauseSkill(CurrenyClass, skill);
        }

        //슬롯에서 장착된 슬롯들 이름 string으로변환 
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

        switch (subclass)
        {
            case Utill_Enum.SubClass.Mage:
                for (int i = 0; i < BattleSkillUis.Length; i++)
                {
                    BattleSkillUis[i].ChanngeSprite(preset[i + 1]);

                    //스킬이름으로 스킬인터페이스 생성
                    ISkill skill = skillmanager.Get_SkillForName(preset[i + 1]);
                    BattleSkillUis[i].Skill = skill;
                    BattleSkillUis[i].isUnuse = false;
                }
                break;

        }
    }
    public void UpdateView(string[] userSKill, int Row)
    {
        // 모든 스킬의 isEquip을 false로 초기화
        int index = 0;
        foreach (var element in HunterSKillarray)
        {
            element.isEquip = false;
            element.Clickindex = 0;
            element.UpdateSkillUi();

        }


        // 데이터 변환 및 RowIndex 설정
        for (int i = 0; i < HunterSKillarray.Length; i++)
        {
            int row = i / 4;
            int col = i % 4;
            if (row < 4 && col < 4) // 배열 인덱스 범위 체크
            {
                Skillelementdoublearray[row, col] = HunterSKillarray[i];
            }
            else
            {
                Debug.LogError($"Index out of bounds: row={row}, col={col}");
            }
        }

        // userSkill 배열의 각 스킬을 찾아서 isEquip을 true로 설정
        for (int i = 1; i < userSKill.Length; i++)
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
                    UseSKill(i - 1, HunterSKillarray[j].m_Skills);

                    break; // 스킬을 찾으면 더 이상 검색할 필요 없음
                }
            }
        }

    }

    //유저의 현재 프리셋을 할당함
    public string[] GetPreset(int i)
    {
        SplitHunterSkillSlotIndex(GameDataTable.Instance.User.MageSkillSlotindex);

        switch (i)
        {
            case 0:
                return preset1;
            case 1: return preset2;
            case 2: return preset3;
        }
        return null;
    }

    public void UseSKill(int index, ISkill skill, bool isEquip = true)
    {
        if (!DataManager.Instance.Hunters[2].gameObject.activeSelf) //헌터가없으면 스킬발동안함
        {     //발동안할때는 비활성화
            for (int i = 0; i < BattleSkillUis.Length; i++)
            {
                BattleSkillUis[i].DeActiveSetUi(CurrenyClass);
            }
            return;
        }
        if (skill == null)
        {
            return;
        }
        if (GameStateMachine.Instance.CurrentState is NonCombatState) //현재 비전투 상태라면
        {
            return;
        }

        BattleSkillUis[index].isUnuse = true;
        BattleSkillUis[index].InitSlot(skill.SkillCoolDown, skill, Utill_Enum.SubClass.Mage, isEquip);
        BattleSkillUis[index].StartCooldown();
    }

    public void UnUseSkill(int index , ISkill skill, bool isEquip = false)
    {
        BattleSkillUis[index].InitSlot(10 , skill , Utill_Enum.SubClass.Mage, isEquip);
        BattleSkillUis[index].StopCooldown();
    }
}
