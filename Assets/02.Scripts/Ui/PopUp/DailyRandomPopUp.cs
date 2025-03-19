using NSY;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;
using UnityEngine.UI;
using TMPro;

/// </summary>
/// 작성일자   : 2024-07-22
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 요일스킬 팝업창
/// </summary>
public class DailyRandomPopUp : MonoBehaviour, IPopUp
{
    public List<BattleSkillUi> BattleSkillUiList;
    public List<SkillUiElement> Dailybtnlist;
    public int DailyCount = 0;
    public List<string> Dailyskillstring; //현재 유저가 클릭한 요일스킬 저장용 리스트

    public TextMeshProUGUI DailySelectDesc;

    private void OnEnable()
    {
        //착용스킬
        Init_Dailyskill_UIData();

        //이 팝업이 켜질땐 현재스킬 비활성화 + 중지
        StopDailySkill(); 

        //헌터랜덤스킬
        Init_DailySkillSlotSet();

        DailySelectDesc.text =string.Format( LocalizationTable.Localization("DailySelectSkill") , GameManager.Instance.DailySkillCount );
    }

    public void StopDailySkill()
    {
        //스킬 비활성화
        foreach (BattleSkillUi battleskill in BattleSkillUiList)
        {
            battleskill.DeActiveSetUi(Utill_Enum.SubClass.None);
            //스킬중지
            battleskill.StopCooldown();
        }
    }

    public void Init_Dailyskill_UIData()
    {
        //현재만약 저장된 요일스킬있다면 비활성화 및 사용안되게
        int count = GameDataTable.Instance.User.DailySkillSlotIndex.Length;

        if (count == 4) //비어있는 새 데이터일 경우
        {
            DailyCount = 0;
            GameDataTable.Instance.User.DailySkillSlotIndex = new string[2];

            int i = 0;
            // battleUI 초기화 (치트 재시작 고려)
            foreach (BattleSkillUi item in BattleSkillUiList)
            {
                string name = GameDataTable.Instance.User.DailySkillSlotIndex[i];
                if (name == null || name == "") //초기화 진행
                {
                    item.InitSlot(); 
                    item.Skill = null;
                    item.CooldownOverlay.gameObject.SetActive(false);
                    item.TextColor.gameObject.SetActive(false);
                    continue;
                }
                i++;
            }
        }
        else if (count == 2)
        {
            int i = 0;
            //바인딩
            for (int b = 0; b < GameDataTable.Instance.User.DailySkillSlotIndex.Length; b++)
            {
                string skillname = GameDataTable.Instance.User.DailySkillSlotIndex[b];
                ISkill skill = SkillManager.Instance.Get_DailySkillForName(skillname);
                if (skill != null)
                {
                    AddDailyData(skill, b);
                }
            }

            //battleui 에서도 보이게...
            foreach (BattleSkillUi item in BattleSkillUiList)
            {

                string name = GameDataTable.Instance.User.DailySkillSlotIndex[i];
                if (name == null || name == "") //데이터가없으면 넘기기
                {
                    item.InitSlot(); //null이면 데이터 빈슷롯만들기
                    continue;
                }
                ISkill skill = SkillManager.Instance.Get_DailySkillForName(name);
                item.Skill = skill;

                if (name == string.Empty || name == null)
                {
                    item.initslot();
                }
                else
                {
                    item.ChanngeSprite(name);
                }

                item.CooldownOverlay.gameObject.SetActive(false);
                i++;
            }

            //비활성화 및 사용안되게
        }
        else //아무것도없다면... 데이터 초가화 + view도초기화
        {

        }
    }

    public void Init_DailySkillSlotSet()
    {
        DailyCount = 0;
        //스킬 랜덤 셋팅 현재는 기존스킬로함
        List<BaseSkill> skillist = SkillManager.Instance.GetUniqueRandomSkills(SkillManager.Instance.DailySkills, 4);
        for (int i = 0; i < Dailybtnlist.Count; i++)
        {
            Dailybtnlist[i].SkillName = skillist[i].SkillName;
            Dailybtnlist[i].isEquip = false;
            Dailybtnlist[i].Clickindex = 0;
            float value = 0f;
            //스킬에 등급 부여
            Dailybtnlist[i].Grade = DailySkillData.GetRandom_Gradeskill(GameDataTable.Instance.DailySkillDataDic);
            //등급에 따른 값 부여
            value = DailySkillData.Get_GradeSkillValue(Dailybtnlist[i].SkillName, GameDataTable.Instance.DailySkillDataDic, Dailybtnlist[i].Grade);
            //값 셋팅
            skillist[i].Value = value;
            skillist[i]._Hunters = DailySkillData.Get_BindingHunter(SkillManager.Instance.DailyArcherValue, SkillManager.Instance.DailyGurdianValue, SkillManager.Instance.DailyMageValue, SkillManager.Instance.DailyAllValue);

            //부여된 등급으로 ui변환
            Dailybtnlist[i].setgradesprite();
            //Text도 변환
            if (skillist[i].SkillName == Tag.ElectricshockNumber)
            {
                Hunter hunter = DataManager.Instance.Hunters[1];
                Dailybtnlist[i].Desc_GradeSkill(skillist[i]._Hunters, LocalizationTable.Localization(skillist[i].SkillName), StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, skillist[i].SkillName, value), hunter);
            }
            else if (skillist[i].SkillName == Tag.AddDamageArrowRain)
            {
                Hunter hunter = DataManager.Instance.Hunters[0];
                Dailybtnlist[i].Desc_GradeSkill(skillist[i]._Hunters, LocalizationTable.Localization(skillist[i].SkillName), StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, skillist[i].SkillName, value), hunter);
            }
            else
            {
                Dailybtnlist[i].Desc_GradeSkill(skillist[i]._Hunters, LocalizationTable.Localization(skillist[i].SkillName), StatTableData.Get_DisplayString(GameDataTable.Instance.StatTableDataDic, skillist[i].SkillName, value));
            }
        }


        //버튼 초기화
        foreach (SkillUiElement item in Dailybtnlist)
        {
            ISkill skill = SkillManager.Instance.Get_DailySkillForName(item.SkillName);
            item.m_Skills = skill;
            item.SpriteName = skill.SkillName;
            //스킬 스프라이트 셋팅
            item.UpdateSkillUi();


            item.ClickSkillBtn.onClick.RemoveAllListeners();//이벤트 해제
            item.ClickSkillBtn.onClick.AddListener(delegate { OnSkillSelectionConfirmed(item); });//이벤트 할당
        }
    }

    //요일스킬눌렀을떄.
    public void OnSkillSelectionConfirmed(SkillUiElement skill)
    {
        //2개선택함으로 2이상일때는... 클릭못하게...
        SoundManager.Instance.PlayAudio("Equipment");
        if (DailyCount >= 2)
        {
            return;
        }

        //장착표시 클릭인덱스
        if (skill.Clickindex >= 1)
        {
            skill.isEquip = false;
            skill.Clickindex = 0;

            //battlueUI도 할당이후 데이터..바인딩 + 한칸한칸 이미지보여주기
            DailyCount -= 1;
            BattleSkillUiList[DailyCount].initslot();
            RemoveDailyData(skill.m_Skills.SkillName, DailyCount);

        }
        else
        {
            skill.Clickindex++;
            skill.isEquip = true;
            //데이터 추가 //1개일떈 저장안함
            AddDailyData(skill.m_Skills, DailyCount);
            //게임 이미지 넣기 저장 X
            skill.UpdateSkillUi();
            //battlueUI도 할당이후 데이터..바인딩 + 한칸한칸 이미지보여주기
            BattleSkillUiList[DailyCount].ChanngeSprite(Dailyskillstring[DailyCount]);
            BattleSkillUiList[DailyCount].CooldownOverlay.gameObject.SetActive(false);
            DailyCount += 1;

            //2개선택함으로 2이상일때는... 클릭못하게...
            if (DailyCount >= 2)
            {
                Hide();//종료..
                //요일스킬 받음처리
                GameDataTable.Instance.User.isDailySkill = true;

                //2개스킬활성화
                foreach (BattleSkillUi item in BattleSkillUiList)
                {
                    item.ActiveSetUi(Utill_Enum.SubClass.None);

                    //쿨도돔
                    item.StartCooldown();
                }

                //두개스킬 저장 
                User.User_DailyinitSkill(GameDataTable.Instance.User, Dailyskillstring);

                //장착 ui 활성화
                skill.UpdateSkillUi();
                return;
            }
        }
        //장착 ui 활성화
        skill.UpdateSkillUi();

    }

    public void Close()
    {
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void AddDailyData(ISkill skill, int index)
    {
        // weeklyskillstring 리스트가 비어 있을 경우 초기화
        if (Dailyskillstring.Count == 0)
        {
            Dailyskillstring = new List<string>(new string[2]);
        }

        // index가 유효한지 확인
        if (index >= 0 && index < Dailyskillstring.Count)
        {
            Dailyskillstring[index] = skill.SkillName;
            BattleSkillUiList[index].Skill = skill;

        }
        else
        {
            Debug.LogError("Index out of range: " + index);
        }
    }

    public void RemoveDailyData(string skillname, int index)
    {
        // weeklyskillstring 리스트가 null이거나 비어 있을 경우 처리
        if (Dailyskillstring == null || Dailyskillstring.Count == 0)
        {
            return;
        }

        // index가 유효한지 확인
        if (index >= 0 && index < Dailyskillstring.Count)
        {
            if (Dailyskillstring[index] == skillname)
            {
                Dailyskillstring[index] = null; // 또는 빈 문자열로 설정: weeklyskillstring[index] = "";
            }
            else
            {
                Debug.LogError("Skill name does not match the skill at the given index.");
            }
        }
        else
        {
            Debug.LogError("Index out of range: " + index);
        }
    }


}
