using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 평타시 스턴
/// </summary>
public abstract class StatusEffect  
{
    private Utill_Enum.Debuff_Type statusType;

    public Utill_Enum.Debuff_Type StatusType 
    {
        get { return statusType; }
        set { statusType = value; }
    }

    private float duration;
    public float Duration
    {
        get { return duration; }
        set { duration = value; }
    }
    protected float startTime;
    public Entity entity { get;  set; }




    public abstract void ApplyEffect(Entity target);
    public abstract void UpdateEffect(Entity target);
    public abstract void RemoveEffect(Entity target);
}
