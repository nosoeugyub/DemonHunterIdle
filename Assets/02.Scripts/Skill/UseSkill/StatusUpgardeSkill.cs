using Game.Debbug;
using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스텟업그레이드 스킬
/// </summary>
public class StatusUpgardeSkill : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    public List<Utill_Enum.Upgrade_Type> Upgradetype;
    private bool hasBuffBeenApplied = true; // 버프가 적용되었는지 여부를 추적
    #endregion


    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(UseMetreor());
        StartCoroutine(UseMetreor());
    }

    private IEnumerator UseMetreor()
    {
        if (hasBuffBeenApplied)
        {
            if (_Hunters.Count >= 3)
            {
                for (int i = 0; i < _Hunters.Count; i++)
                {
                    for (int j = 0; j < Upgradetype.Count; j++)
                    {
                        HunterStat.UseSkillAddUserStat(_Hunters[i]._UserStat, Value, Upgradetype[j]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Upgradetype.Count; i++)
                {
                    if (Upgradetype[i] == Utill_Enum.Upgrade_Type.AddDamageArrowRain)//화살비는 특수처리
                    {
                        _Hunters[0] = DataManager.Instance.Hunters[0];
                        HunterStat.UseSkillAddUserStat(_Hunters[0]._UserStat, Value, Upgradetype[i]);
                    }
                    else if(Upgradetype[i] == Utill_Enum.Upgrade_Type.ElectricshockNumber)
                    {
                        _Hunters[0] = DataManager.Instance.Hunters[1];
                        HunterStat.UseSkillAddUserStat(_Hunters[0]._UserStat, Value, Upgradetype[i]);
                    }
                    else
                    {
                        HunterStat.UseSkillAddUserStat(_Hunters[0]._UserStat, Value, Upgradetype[i]);
                    }
                    
                }
            }
            hasBuffBeenApplied = false; // 버프가 적용되었음을 표시
        }


       
        
        yield return null;
    }

    public override void Init_Skill()
    {
        if (hunter == null) return;

        //타입의 갯수 or 타입 에따라 해당 수치만큼 1;1 대응로 버프 주기
        for (int i = 0; i < Upgradetype.Count; i++)
        {
            HunterStat.UseSkillMinusUserStat(hunter._UserStat, Value, Upgradetype[i]);
        }
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
      
    }
}
