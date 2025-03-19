using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-10-10
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 상점 셀 그리트 뷰
///  </summary>
public class ShopGrildSell : EnhancedScrollerCellView
{
    public ShopCellView[] shopCellViews;

    /// <summary>
    /// 이 함수는 데이터를 받아서 셀에 표시합니다.
    /// </summary>
    /// <param name="data">데이터 사전</param>
    /// <param name="startingIndex">시작 인덱스</param>
    public void SetData(ref Dictionary<int, ShopGoldCellData> data, int startingIndex)
    {
        // 셀의 데이터를 설정하거나, 데이터 범위를 벗어나면 null로 비활성화
        for (var i = 0; i < shopCellViews.Length; i++)
        {
            // 데이터 범위를 벗어난 경우 null을 전달
            shopCellViews[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i + 1] : null);
        }
    }
}
