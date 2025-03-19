using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuff 
{
    public string name;
    public Utill_Enum.Debuff_Type DebuffType;
    public float DebuffTime;
    public int DebuffCount; // 혹시 모르ㄹ 디버프 중복에 대한 카운트 

    public DeBuff() { }

    public DeBuff(Utill_Enum.Debuff_Type _DebuffType ,  float _DebuffTime )
    {
        name = _DebuffType.ToString();
        DebuffType = _DebuffType;
        DebuffTime = _DebuffTime;
    }

    //디버프 추가
    public static List<DeBuff> Add_Debuff(EnemyStat target, DeBuff debuff)
    {
       if (target.EnemyDebuffList == null)
        {
            target.EnemyDebuffList = new List<DeBuff>();
        }
        int count = target.EnemyDebuffList.Count;
        if (count == 0 )
        {
            target.EnemyDebuffList.Add(debuff);
            return target.EnemyDebuffList;
        }

        for (int i = 0; i < count; i++)
        {
            if (target.EnemyDebuffList[i].name == debuff.name)
            {
                return target.EnemyDebuffList;
            }
        }

        target.EnemyDebuffList.Add(debuff);
        return target.EnemyDebuffList;
    }
    //디버프 제거
    public static List<DeBuff> Remove_BuffList(EnemyStat target, DeBuff debuff)
    {
        List<DeBuff> list = target.EnemyDebuffList;
        int count = list.Count;

        if (count == 0)
        {
            return list;
        }


        for (int i = 0; i < count; i++)
        {
            if (list[i].name == debuff.name)
            {
                list.RemoveAt(i);
                return list;
            }
        }

        return list;
    }
}
