using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-04
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 승급시 올라가는 능력치 데이터                                                   
/// </summary>
public class PromotionAbilityData 
{
    public float AttackSpeed;
    public float AttackRange;
    public float MoveSpeed;
    public float PhysicalPower;
    public float MagicPower;
    public float PhysicalPowerDefense;
    public float Hp;
    public float Mp;
}

/// <summary>
/// 작성일자   : 2024-07-04
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 승급시 필요한 조건들                                                   
/// </summary>
public class PromotionAllocationData 
{
    public string ResourceType;
    public int ResourceCount;
    public int ReqHunterLevel;
    public int ReqClearChapterZone;
}
