using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using System.Globalization;
using BackEnd;
using LitJson;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Events;

public class ShopPopUpController_2 : MonoBehaviour, IEnhancedScrollerDelegate
{
    // 데이터를 담는 리스트
    public Dictionary<int, ShopGoldCellData> _data = new();
    // Enhanced Scroller 및 셀 뷰 프리팹과 셀 크기 변수
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize;
    private int numberOfCellsPerRow = 2; //한줄에 몇 셀이 뜨는지?



    void Start()
    {
        scroller.Delegate = this;
        SetData(GameDataTable.Instance.ShopResoucrceGolddata_One_Dic);
    }

    /// <summary>
    /// Populates the data with some random Lorum Ipsum text
    /// </summary>
    public void SetData(Dictionary<int, ShopGoldCellData> data)
    {
        _data = data;

        ResizeScroller();
    }

    public void ResetData()
    {
        _data = new Dictionary<int, ShopGoldCellData>();
        ResizeScroller(keepPosition: false);
    }

    public void SetDataWithOutValidate(Dictionary<int, ShopGoldCellData> data)
    {
        _data = data;

        ResizeScroller();
    }

    /// <summary>
    /// 새로운 행을 추가하고 스크롤러를 리사이징하는 함수
    /// </summary>
    public void AddNewRow()
    {
        // 먼저 스크롤러에 있는 모든 셀을 초기화
        scroller.ClearAll();

        // 스크롤러의 위치를 초기화
        scroller.ScrollPosition = 0;

        // 새로운 데이터 행 추가
        _data.Add(_data.Count, new ShopGoldCellData());

        ResizeScroller();

        // 선택적으로 스크롤러를 마지막 데이터로 이동
        scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f);
    }

    /// <summary>
    /// This function will exand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
    /// reset the scroller's size back, then reload the data once more to display the cells.
    /// </summary>
    public void ResizeScroller(float scrollPositionFactor = 0, bool keepPosition = false)
    {
        // capture the scroller dimensions so that we can reset them when we are done
        var rectTransform = scroller.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;

        // set the dimensions to the largest size possible to acommodate all the cells
        rectTransform.sizeDelta = new Vector2(size.x, size.y);

        // First Pass: reload the scroller so that it can populate the text UI elements in the cell view.
        // The content size fitter will determine how big the cells need to be on subsequent passes
        scroller.ReloadData(scrollPositionFactor: scrollPositionFactor, keepPosition: keepPosition);

        // reset the scroller size back to what it was originally
        rectTransform.sizeDelta = size;

        // Second Pass: reload the data once more with the newly set cell view sizes and scroller content size
        scroller.ReloadData(scrollPositionFactor: scrollPositionFactor, keepPosition: keepPosition);
    }

    #region EnhancedScroller Handlers

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)_data.Count / (float)numberOfCellsPerRow);
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellSize;
    }

    // 셀 뷰를 반환하는 함수
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = scroller.GetCellView(cellViewPrefab) as ShopGrildSell;
        cellView.SetData(ref _data, dataIndex * numberOfCellsPerRow);

        return cellView;



    }
    #endregion
}
