using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using System;
using static Utill_Enum;
using System.Text;

/// <summary>
/// Enum별 텍스트 프리펩 정보 받아오기 위한 구조체
/// </summary>
[Serializable]
public struct NoticeText
{
    public NoticeType type;
    public DamageNumber prefab;
}

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 텍스트 이펙트 제어
/// </summary>
public class NoticeManager : MonoSingleton<NoticeManager>
{
    [SerializeField]
    private List<NoticeText> noticeList = new();

    private Dictionary<NoticeType, DamageNumber> noticeDic = new Dictionary<NoticeType, DamageNumber>();
    private StringBuilder sb = new StringBuilder();
    private string addPercentColorHtmlStr = string.Empty; //일격피해와 치명피해가 동시에 뜬 경우에 사용할 addPercent 텍스트컬러..

    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
    }
    //test 때문에 임시 public
    public bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Enum_GameSequence.CreateAndInit:
                noticeDic.Clear();
                for (int i = 0; i < noticeList.Count; i++)
                {
                    noticeDic.Add(noticeList[i].type, noticeList[i].prefab);
                }
                return true;
        }

        return true;
    }

    /// <summary>
    /// 텍스트 에셋을 생성
    /// </summary>
    /// <param name="type">텍스트 에셋 타입</param>
    /// <param name="position">위치</param>
    public void SpawnNoticeText(Utill_Enum.NoticeType type, Vector3 position)
    {
        DamageNumber damageNumber = noticeDic[type].Spawn(position);
    }
    /// <summary>
    /// 값이 필요한 텍스트 에셋을 생성
    /// </summary>
    /// <param name="type">텍스트 에셋 타입</param>
    /// <param name="position">위치</param>
    /// <param name="value">표시될 값(텍스트 에셋 참고)</param>
    public void SpawnNoticeText(Utill_Enum.NoticeType type, Vector3 position, int value)
    {
        DamageNumber damageNumber = noticeDic[type].Spawn(position);
        damageNumber.number = value;
    }
    /// <summary>
    /// 값과 추가 지급 값이 필요한 텍스트 에셋을 생성
    /// 주로 데미지나 버프가 있는 컨텐츠에 사용함
    /// </summary>
    /// <param name="type">텍스트 에셋 타입</param>
    /// <param name="position">위치</param>
    /// <param name="value">표시될 값(텍스트 에셋 참고)</param>
    /// <param name="addPercent">+nn으로 표시될 추가 값</param>
    public void SpawnNoticeText(Utill_Enum.NoticeType type, Vector3 position, int value, int addPercent)
    {
        DamageNumber damageNumber = noticeDic[type].Spawn(position);
        
        if(damageNumber.enableNumber)
            damageNumber.number = value;
        if(damageNumber.enableRightText && addPercent > 0)
        {
            damageNumber.rightText = $"+{addPercent}";
        }
    }
    /// <summary>
    /// 일격피해와 치명피해가 동시에 뜬 경우의 텍스트 에셋을 생성
    /// </summary>
    public void SpawnNoticeText(Vector3 position, int criValue, int criAddPercent,int coupValue,int coupAddPercent)
    {
        DamageNumber damageNumber = noticeDic[NoticeType.CoupDamage].Spawn(position);
        //크리티컬
        sb.Clear();
        sb.Append(criValue);
        //크리티컬 추가 데미지
        if(criAddPercent > 0)
        {
            if(addPercentColorHtmlStr == null || addPercentColorHtmlStr == string.Empty)
            {
                addPercentColorHtmlStr = ColorUtility.ToHtmlStringRGBA(damageNumber.rightTextSettings.color);
            }
            sb.Append($"<color=#{addPercentColorHtmlStr}>");
            sb.Append(criAddPercent);
            sb.Append("</color>");
        }
        sb.Append(" ");
        if (damageNumber.enableLeftText)
            damageNumber.leftText = sb.ToString();

        //일격 데미지
        if (damageNumber.enableNumber)
            damageNumber.number = coupValue;

        //일격 추가 데미지
        if(damageNumber.enableRightText && coupAddPercent > 0)
        {
            damageNumber.rightText = $"+{coupAddPercent}";
        }
    }

    /// <summary>
    /// 위와 아래에 텍스트가 추가되는 경우의 텍스트 에셋을 생성
    /// </summary>
    /// <param name="type">텍스트 에셋 타입</param>
    /// <param name="position">위치</param>
    /// <param name="topText">위에 적힐 텍스트</param>
    /// <param name="bottomText">아래에 적힐 텍스트</param>
    /// <param name="isShowNumber">가운데 숫자 부분을 보이게 처리할지</param>
    /// <param name="number">가운데 숫자 부분</param>
    public void SpawnNoticeText(Utill_Enum.NoticeType type, Vector3 position, string topText,string bottomText, bool isShowNumber, int number = 0)
    {
        DamageNumber damageNumber = noticeDic[type].Spawn(position);

        if (topText != "")
        {
            damageNumber.enableTopText = true;
            damageNumber.topText = topText;
        }
        else
            damageNumber.enableTopText = false;

        if(bottomText != "")
        {
            damageNumber.enableBottomText = true;
            damageNumber.bottomText = bottomText;
        }
        else
            damageNumber.enableBottomText = false;

        if(damageNumber.enableNumber && isShowNumber)
        {
            damageNumber.number = number;
            damageNumber.numberSettings.customColor = false;
        }
        else if(damageNumber.enableNumber)
        {
            damageNumber.number = 0;
            damageNumber.numberSettings.customColor = true;
        }
    }
}
