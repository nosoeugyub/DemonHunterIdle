using NSY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-09-02
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 과금 업적 보상의 UI를 제어 (큰 내용은 부모에서 제어하고 판단하는 부분만 따로 추가구현함)
/// </summary>
public class PurchaseAchievementSystem : AchievementSystem
{

    #region 슬라이더 별 반환 함수
    public override string GetTitleName(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
            return string.Format("{0}:{1}", LocalizationTable.Localization("Title_PurchaseAchievement"), GetTotalAchiveValue(sliderIndex));
        return string.Empty;
    }
    /// <summary>
    /// 각 슬라이더 별로 참조해야할 값을 반환
    /// </summary>
    public override int GetTotalAchiveValue(int sliderIndex)
    {
        if(sliderIndex == 0) //과금 업적
            return GameDataTable.Instance.User.TotalPurchase;
        return 0;
    }
    public override int GetCellLength(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
            return GameDataTable.Instance.PurchaseAchievementDataDic.Count;
        return 0;
    }
    /// <summary>
    ///  각 슬라이더 별로 셀을 제작할 때 필요한 정보들을 반환
    /// </summary>
    public override (string rewardName,int rewardCount,int needCount) GetCurrentAchiveValue(int sliderIndex, int cellIndex)
    {
        if (sliderIndex == 0) //과금 업적
            return (GameDataTable.Instance.PurchaseAchievementDataDic[cellIndex+1].resourceType, GameDataTable.Instance.PurchaseAchievementDataDic[cellIndex + 1].resourceCount, GameDataTable.Instance.PurchaseAchievementDataDic[cellIndex + 1].totalPurchase);
        return (string.Empty,0,0);
    }
    /// <summary>
    /// 각 슬라이더 별로 데이터상의 최댓값을 반환
    /// </summary>
    public override float GetMaximumAchiveValue(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
            return GameDataTable.Instance.PurchaseAchievementDataDic[GameDataTable.Instance.PurchaseAchievementDataDic.Count].totalPurchase;
        return 0;
    }

    public override bool IsReceived(int checkIndex)
    {
        if (GameDataTable.Instance.User.RewardReceived.Length <= checkIndex)
            return false;
        return GameDataTable.Instance.User.RewardReceived[checkIndex];
    }

    public override void SetReceived(int checkIndex, bool value)
    {
        if (GameDataTable.Instance.User.RewardReceived.Length <= checkIndex)
            return;
        GameDataTable.Instance.User.RewardReceived[checkIndex] = value;
    }
    #endregion

}
