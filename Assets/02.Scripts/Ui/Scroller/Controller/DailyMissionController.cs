using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DailyMissionController : MonoBehaviour, IEnhancedScrollerDelegate
{
    // Enhanced Scroller 및 셀 뷰 프리팹과 셀 크기 변수
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize;
    
    private List<DailyMissionCellData> cellDataList;

    void Start()
    {
        scroller.Delegate = this;
    }

    // 데이터를 설정하는 함수
    public void SetData(List<DailyMissionCellData> data)
    {
        cellDataList = data;

        // 스크롤러 크기를 조정
        ResizeScroller();
    }

    // 유효성 검증 없이 데이터를 설정하는 함수
    public void SetDataWithOutValidate(List<DailyMissionCellData> data)
    {
        cellDataList = data;

        // 스크롤러 크기를 조정 (포지션 유지)
        ResizeScroller(true);
    }

    // 새로운 행을 추가하는 함수
    public void AddNewRow()
    {
        // 먼저, 스크롤러의 모든 셀을 지움
        scroller.ClearAll();

        // 스크롤러의 위치를 초기화
        scroller.ScrollPosition = 0;

        // 데이터를 추가 (예: 키값을 자동으로 증가시키는 방법으로 새로운 행 추가)
        cellDataList.Add(new DailyMissionCellData());

        // 스크롤러 크기를 조정
        ResizeScroller();

        // 선택 사항: 스크롤러를 맨 끝으로 이동하여 새로운 콘텐츠를 볼 수 있게 함
        scroller.JumpToDataIndex(cellDataList.Count - 1, 1f, 1f);
    }
    // 스크롤러를 조정하는 함수
    private void ResizeScroller(bool keepPosition = false)
    {
        // 스크롤러의 RectTransform을 캡처하여 나중에 복원
        var rectTransform = scroller.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;

        // 모든 셀을 수용할 수 있도록 최대 크기로 설정
        rectTransform.sizeDelta = new Vector2(size.x, size.y);

        // 첫 번째 패스: 스크롤러를 리로드하여 셀 뷰의 텍스트 UI 요소를 채움
        scroller.ReloadData(keepPosition: keepPosition);

        // 스크롤러 크기를 원래 크기로 복원
        rectTransform.sizeDelta = size;

        // 두 번째 패스: 셀 뷰 크기와 스크롤러 콘텐츠 크기가 설정된 상태로 데이터를 다시 로드
        scroller.ReloadData(keepPosition: keepPosition);
    }

    #region EnhancedScroller Handlers
    // 스크롤러의 셀 수를 반환하는 함수
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return cellDataList?.Count ?? 0;
    }

    // 특정 인덱스의 셀 크기를 반환하는 함수
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellSize;
    }

    // 셀 뷰를 반환하는 함수
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = scroller.GetCellView(cellViewPrefab) as DailyMissionCellView;
        cellView.SetData(cellDataList[cellIndex]);
        return cellView;
    }
    #endregion
}
