using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 랭킹 스크롤을 관리함
/// </summary>
public class RankingScrollController : MonoBehaviour, IEnhancedScrollerDelegate, IBeginDragHandler, IEndDragHandler
{
    private List<RankingCellData> _data = new();
    public List<RankingCellData> GetData => _data;
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize = 50;
    public float pullDownThreshold;

    public Action onScrollEnd;

    [HideInInspector] public int maxStoreCount;
    private bool dragging = false;
    private bool scrollToEnd = false;

    void Start()
    {
        scroller.Delegate = this;
        scroller.scrollerScrolled = ScrollerScrolled;
    }

    /// <summary>
    /// 데이터를 기반으로 셀 생성 후 스크롤러 위치 조절
    /// </summary>
    public void SetData(List<RankingCellData> data)
    {
        _data = data;

        if (scroller.ScrollPosition == scroller.ScrollSize)
        {
            ResizeScroller(scrollPositionFactor: 1);
        }
        else
        {
            ResizeScroller(keepPosition: true);
        }
    }

    /// <summary>
    /// 스크롤러 위치 조절 없이 셀 생성
    /// </summary>
    public void SetDataWithOutValidate(List<RankingCellData> data)
    {
        _data = data;

        ResizeScroller(keepPosition: true);
    }

    /// <summary>
    /// 이 함수는 새로운 레코드를 추가하고, 스크롤러 크기를 조정하며 모든 셀의 크기를 계산
    /// </summary>
    public void AddNewRow()
    {
        // 먼저 스크롤러에 있는 셀을 모두 지워 새 텍스트 변환이 리셋되도록 합니다
        scroller.ClearAll();

        // 새 바운드 밖으로 나가지 않도록 스크롤러의 위치를 리셋합니다
        scroller.ScrollPosition = 0;

        // 이제 데이터를 추가합니다
        _data.Add(new RankingCellData());

        ResizeScroller();

        // 선택 사항: 스크롤러 끝으로 이동하여 새 콘텐츠를 확인합니다
        scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f);
    }

    /// <summary>
    /// 이 함수는 셀이 맞을 수 있도록 스크롤러를 확장하고, 셀 크기를 계산하기 위해 데이터를 다시 로드한 후
    /// 스크롤러의 크기를 원래대로 되돌리고, 셀을 표시하기 위해 다시 한 번 데이터를 로드
    /// </summary>
    public void ResizeScroller(float scrollPositionFactor = 0, bool keepPosition = false)
    {
        // 스크롤러 크기를 캡처하여 완료 시 다시 설정할 수 있도록 합니다
        var rectTransform = scroller.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;

        // 셀을 모두 수용할 수 있도록 가능한 최대 크기로 차원을 설정합니다
        rectTransform.sizeDelta = new Vector2(size.x, size.y);

        // 첫 번째 패스: 스크롤러를 다시 로드하여 셀 보기의 텍스트 UI 요소를 채웁니다.
        // 콘텐츠 크기 피터가 후속 패스에서 셀이 얼마나 커야 하는지를 결정합니다
        scroller.ReloadData(scrollPositionFactor: scrollPositionFactor, keepPosition: keepPosition);

        // 스크롤러 크기를 원래 크기로 다시 설정합니다
        rectTransform.sizeDelta = size;

        // 두 번째 패스: 새로 설정된 셀 보기 크기 및 스크롤러 콘텐츠 크기로 데이터를 다시 한 번 로드합니다
        scroller.ReloadData(scrollPositionFactor: scrollPositionFactor, keepPosition: keepPosition);
    }

    #region 드래그 관련
    private void ScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollPosition)
    {
        var scrollMoved = ((scrollPosition >= pullDownThreshold + scroller.ScrollSize) || (scrollPosition == 0 && scroller.ScrollSize == 0)) && val.y <= 0;


        var count = _data.Count;
        if (dragging && scrollMoved && count < maxStoreCount)
        {
            scrollToEnd = true;
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        var count = _data.Count;
        if (count < maxStoreCount)
            onScrollEnd?.Invoke();
        dragging = true;
    }

    public void OnEndDrag(PointerEventData data)
    {
        dragging = false;

        if (scrollToEnd)
        {
            //onScrollEnd?.Invoke();
            scrollToEnd = false;
        }
    }
    #endregion

    #region EnhancedScroller Handlers

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data?.Count ?? 0;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellSize;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        RankingCellView cellView = scroller.GetCellView(cellViewPrefab) as RankingCellView;
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }

    #endregion
}
