using DuloGames.UI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 랭크의 한 셀
/// </summary>
public class RankingCellView : EnhancedScrollerCellView
{
    [SerializeField] private Image rankImage = null;
    [SerializeField] private TextMeshProUGUI rankText = null;
    [SerializeField] private TextMeshProUGUI rateText = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI extraScoreText = null;
    [SerializeField] private TextMeshProUGUI nickNameText = null;

    private RankingCellData _data;

    public void SetData(RankingCellData data)
    {
        _data = data;

        rankText.text = Utill_Math.FormatCurrency(_data.rank);
        rateText.text = _data.percent.ToString("P2");

        if (_data.rankValue < 100000)
            scoreText.text = Utill_Math.FormatCurrency((int)_data.rankValue);
        else if (_data.rankValue >= 100000) //수가 너무 크면 축약시킴
            scoreText.text = Utill_Math.AbbreviateNumber(long.Parse(Utill_Math.FormatCurrency((int)_data.rankValue)));


        if (_data.extraValue != 0)
            extraScoreText.text = Utill_Math.FormatCurrency(_data.extraValue);
        else
            extraScoreText.text = "-";
        nickNameText.text = _data.id;

        if (_data.rank <= 10)
        {
            rankImage.sprite = Utill_Standard.GetUiSprite($"Rank{_data.rank}");
            rankImage.enabled = true;
        }
        else
            rankImage.enabled = false;
    }
}
