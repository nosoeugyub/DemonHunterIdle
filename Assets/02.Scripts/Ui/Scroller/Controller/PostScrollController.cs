using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private List<PostCellData> _data;

    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize = 180;

    private RectTransform content;
    void Start()
    {
        scroller.Delegate = this;
        content = scroller.GetComponent<ScrollRect>().content;
        scroller.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(0, 0);
    }

    /// <summary>
    /// Populates the data with some random Lorum Ipsum text
    /// </summary>
    public void SetData(List<PostCellData> data)
    {
        _data = data;

        ResizeScroller();
    }

    public void SetDataWithOutValidate(List<PostCellData> data)
    {
        _data = data;

        ResizeScroller(true);
    }


    /// <summary>
    /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
    /// </summary>
    public void AddNewRow()
    {
        // first, clear out the cells in the scroller so the new text transforms will be reset
        scroller.ClearAll();

        // reset the scroller's position so that it is not outside of the new bounds
        scroller.ScrollPosition = 0;

        // now we can add the data row
        _data.Add(new PostCellData());

        ResizeScroller();

        // optional: jump to the end of the scroller to see the new content
        scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f);
    }
    /// <summary>
    /// This function will exand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
    /// reset the scroller's size back, then reload the data once more to display the cells.
    /// </summary>
    private void ResizeScroller(bool keepPosition = false)
    {
        // capture the scroller dimensions so that we can reset them when we are done
        var rectTransform = scroller.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;

        // set the dimensions to the largest size possible to acommodate all the cells
        rectTransform.sizeDelta = new Vector2(size.x, size.y);

        // First Pass: reload the scroller so that it can populate the text UI elements in the cell view.
        // The content size fitter will determine how big the cells need to be on subsequent passes
        scroller.ReloadData(keepPosition: keepPosition);

        // reset the scroller size back to what it was originally
        rectTransform.sizeDelta = size;

        // Second Pass: reload the data once more with the newly set cell view sizes and scroller content size
        scroller.ReloadData(keepPosition: keepPosition);
    }

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
        PostCellView cellView = scroller.GetCellView(cellViewPrefab) as PostCellView;
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }

    #endregion
}