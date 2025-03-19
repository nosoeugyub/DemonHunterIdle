using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 헌터 업그레이트 탭 컨틀롤러
/// /// </summary>
public class HunterUpGradeController : MonoBehaviour, IEnhancedScrollerDelegate
{

    public Utill_Enum.SubClass character;
    // 데이터를 담는 리스트
    public List<Dictionary<string, int>> _data = new();

    // Enhanced Scroller 및 셀 뷰 프리팹과 셀 크기 변수
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize;

    // 초기화 함수, 스크롤러의 Delegate를 설정
    void Start()
    {
        scroller.Delegate = this;
    }


 

    // 초기화 함수, 특정 조건에 따라 데이터를 초기화
    public void Init_Zero(bool isZero)
    {
        if (isZero)
        {
            Hunter_UpgradeData.Upgrade_init(GameDataTable.Instance.User.GetUpgradeList(0));
        }
    }

    // 데이터를 설정하는 함수
    public void SetData(List<Dictionary<string, int>> data)
    {
        _data = data;

        // 스크롤러 크기를 조정
        ResizeScroller();
    }

    // 유효성 검증 없이 데이터를 설정하는 함수
    public void SetDataWithOutValidate(List<Dictionary<string, int>> data)
    {
        _data = data;

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

        // 데이터를 추가
        _data.Add(new Dictionary<string, int>());

        // 스크롤러 크기를 조정
        ResizeScroller();

        // 선택 사항: 스크롤러를 맨 끝으로 이동하여 새로운 콘텐츠를 볼 수 있게 함
        scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f);
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
        return _data?.Count ?? 0;
    }

    // 특정 인덱스의 셀 크기를 반환하는 함수
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellSize;
    }

    // 셀 뷰를 반환하는 함수
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = scroller.GetCellView(cellViewPrefab) as HunterUpGradeView;
        cellView.UpdateCell(_data[dataIndex] ,GameDataTable.Instance.HunterUpgradeDataDic , dataIndex);
        cellView.character = character;
        cellView.SetButtonFunc();
        return cellView;
    }


    #endregion
}
