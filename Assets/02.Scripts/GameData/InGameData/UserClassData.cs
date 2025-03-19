using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-06-24
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 클래스별 별도적으로 다뤄야할 정보
/// </summary>
public class UserClassData 
{
    public ObscuredInt Level;
    public ObscuredFloat Exp;

    public List<Dictionary<string, int>> HunterUpgradeList = new List<Dictionary<string, int>>(); // 유저가 장착한 (스킬, 스킬 업그레이드 횟수)
    public int[] ArcherAttribute; //헌터가 가지고있는 속성들 (총속성, 속성1, 속성2, 속성3)
    public List<List<int>> _UserHasSkilldata; // 유저가 장착한 (스킬, 스킬 업그레이드 횟수)
    public int[] HunterPromotion; //헌터 승급 정보

    public void InitUserClassData(int level,float exp, List<Dictionary<string, int>> _Hunter_Upgrade, int[] _ArcherAttribute, int[] _HunterPromotion)
    {
        Level = level;
        Exp = exp;
        HunterUpgradeList = _Hunter_Upgrade;
        ArcherAttribute = _ArcherAttribute;
        HunterPromotion = _HunterPromotion;
    }
}
