using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-09-12
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 가챠 업적 보상의 UI를 제어(큰 내용은 부모에서 제어하고 판단하는 부분만 따로 추가구현함)
/// </summary>
public class GachaAchievementSystem : AchievementSystem
{
    #region 슬라이더 별 반환 함수
    public override string GetTitleName(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
            return string.Format("{0}:{1}", LocalizationTable.Localization("Title_GachaAchievement"), GetTotalAchiveValue(sliderIndex));
        return string.Empty;
    }
    /// <summary>
    /// 각 슬라이더 별로 참조해야할 값을 반환
    /// </summary>
    public override int GetTotalAchiveValue(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
        {
            switch (GameDataTable.Instance.User.currentHunter)
            {
                case 0: //궁수
                    return GameDataTable.Instance.User.ArcherTotalGacha[UIManager.Instance.gachaPopUp.CurPart.ToString()];
                case 1: //수호자
                    return GameDataTable.Instance.User.GuardianTotalGacha[UIManager.Instance.gachaPopUp.CurPart.ToString()];
                case 2: //법사
                    return GameDataTable.Instance.User.MageTotalGacha[UIManager.Instance.gachaPopUp.CurPart.ToString()];
            }
        }
        return 0;
    }
    public override int GetCellLength(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
        {
            return GameDataTable.Instance.GachaAchievementDataDic.Count;
        }
        return 0;
    }
    /// <summary>
    ///  각 슬라이더 별로 셀을 제작할 때 필요한 정보들을 반환
    /// </summary>
    public override (string rewardName, int rewardCount, int needCount) GetCurrentAchiveValue(int sliderIndex, int cellIndex)
    {
        if (sliderIndex == 0) //과금 업적
        {
            return (GameDataTable.Instance.GachaAchievementDataDic[cellIndex + 1].resourceType, GameDataTable.Instance.GachaAchievementDataDic[cellIndex + 1].resourceCount, GameDataTable.Instance.GachaAchievementDataDic[cellIndex + 1].totalPurchase);
        }
        return (string.Empty, 0, 0);
    }
    /// <summary>
    /// 각 슬라이더 별로 데이터상의 최댓값을 반환
    /// </summary>
    public override float GetMaximumAchiveValue(int sliderIndex)
    {
        if (sliderIndex == 0) //과금 업적
            return GameDataTable.Instance.GachaAchievementDataDic[GameDataTable.Instance.GachaAchievementDataDic.Count].totalPurchase;
        return 0;
    }

    public override bool IsReceived(int checkIndex)
    {
        switch (GameDataTable.Instance.User.currentHunter)
        {
            case 0: //궁수
                if (GameDataTable.Instance.User.ArcherGachaRewardReceived.Count <= checkIndex)
                    return false;
                return GameDataTable.Instance.User.ArcherGachaRewardReceived[UIManager.Instance.gachaPopUp.CurPart.ToString()][checkIndex];
            case 1: //수호자
                if (GameDataTable.Instance.User.GuardianGachaRewardReceived.Count <= checkIndex)
                    return false; 
                return GameDataTable.Instance.User.GuardianGachaRewardReceived[UIManager.Instance.gachaPopUp.CurPart.ToString()][checkIndex];
            case 2: //법사
                if (GameDataTable.Instance.User.MageGachaRewardReceived.Count <= checkIndex)
                    return false;
                return GameDataTable.Instance.User.MageGachaRewardReceived[UIManager.Instance.gachaPopUp.CurPart.ToString()][checkIndex];
        }
        return false;
    }

    public override void SetReceived(int checkIndex, bool value)
    {
        switch (GameDataTable.Instance.User.currentHunter)
        {
            case 0: //궁수
                if (GameDataTable.Instance.User.ArcherGachaRewardReceived.Count <= checkIndex)
                    return;
                GameDataTable.Instance.User.ArcherGachaRewardReceived[UIManager.Instance.gachaPopUp.CurPart.ToString()][checkIndex] = value;
                break;
            case 1: //수호자
                if (GameDataTable.Instance.User.GuardianGachaRewardReceived.Count <= checkIndex)
                    return;
                GameDataTable.Instance.User.GuardianGachaRewardReceived[UIManager.Instance.gachaPopUp.CurPart.ToString()][checkIndex] = value;
                break;
            case 2: //법사
                if (GameDataTable.Instance.User.MageGachaRewardReceived.Count <= checkIndex)
                    return;
                GameDataTable.Instance.User.MageGachaRewardReceived[UIManager.Instance.gachaPopUp.CurPart.ToString()][checkIndex] = value;
                break;
        }
    }
    #endregion
}
