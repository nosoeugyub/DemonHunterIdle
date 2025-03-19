using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 채팅 셀의 길이 조절/ 채팅 셀 초기화를 담당하는 컨트롤러 스크립트
/// </summary>
public class ChatScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField]
    private EnhancedScroller scroller;
    [SerializeField]
    private List<ChatCellData> cellData;
    [SerializeField]
    private ChatCellView chatCellPrefab;

    private float _totalCellSize = 0; //셀길이+ 스크롤러 상하 패딩
    private float _oldScrollPosition = 0; //새로운 셀로 점프하기 전의 위치 저장

    /// <summary>
    /// The estimated width of each character. Note that this is just an estimate
    /// since most fonts are not mono-spaced.
    /// </summary>
    public int characterWidth = 8;

    /// <summary>
    /// The height of each character.
    /// </summary>
    public int characterHeight = 26;

    private bool isInit = false;

    void Awake()
    {
        scroller.Delegate = this;
        cellData = new List<ChatCellData>();
        if(!isInit)
        {
            SetData(cellData);
        }
    }

    /// <summary>
    /// Populates the data with some random Lorum Ipsum text
    /// </summary>
    public void SetData(List<ChatCellData> data)
    {
        if (!isInit) cellData.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            AddNewRow(data[i].cellType, data[i].ranking, data[i].nickName, data[i].message, data[i].alertType, ResizeScroll:false);
        }
        isInit = true;
        if (cellData.Count <= 0) return;
        ResizeScroller();
        scroller.JumpToDataIndex(cellData.Count - 1, 1f, 1f, tweenType: EnhancedScroller.TweenType.easeInOutSine, tweenTime: 0.5f);
    }

    public void ResetData()
    {
        cellData = new List<ChatCellData>();
        ResizeScroller(keepPosition: false);
    }

    public void SetDataWithOutValidate(List<ChatCellData> data)
    {
        cellData = data;
    }

    /// <summary>
    /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
    /// </summary>
    public void AddNewRow(ChatCellType cellType, int ranking, string nickName , string message, ChatBroadcastType alertChatType = ChatBroadcastType.None, bool ResizeScroll = true, bool keepPosition = false)
    {
        // first, clear out the cells in the scroller so the new text transforms will be reset
        scroller.ClearAll();

        _oldScrollPosition = scroller.ScrollPosition;
        // reset the scroller's position so that it is not outside of the new bounds
        scroller.ScrollPosition = 0;


        // calculate the space needed for the text in the cell
        
        //메시지에서 색 표현식을 삭제(표현식은 메시지 길이에 영향을 안 주기 때문)
        string tmpMessage = Utill_Standard.RemoveColorTags(message);

        // get the estimated total width of the text (estimated because the font is assumed to be mono-spaced)
        float totalTextWidth = (float)tmpMessage.Length * (float)characterWidth;

        // get the number of rows the text will take up by dividing the total width by the widht of the cell
        int numRows = Mathf.CeilToInt(totalTextWidth / (scroller.GetComponent<RectTransform>().sizeDelta.x + 1));

        // get the cell size by multiplying the rows times the character height
        var cellSize = numRows * (float)characterHeight;

        // now we can add the data row
        cellData.Add(new ChatCellData()
        {
            cellType = cellType,
            ranking = ranking,
            nickName = nickName,
            message = message,

            cellSize = cellSize,
            alertType = alertChatType,
        });
        if(ResizeScroll)
        {
            ResizeScroller(keepPosition: keepPosition);
            // optional: jump to the end of the scroller to see the new content
            if(!keepPosition)
                scroller.JumpToDataIndex(cellData.Count - 1, 1f, 1f);
        }
    }

    /// <summary>
    /// This function will exand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
    /// reset the scroller's size back, then reload the data once more to display the cells.
    /// </summary>
    public void ResizeScroller(float scrollPositionFactor = 0, bool keepPosition = false)
    { 
        // capture the scroll rect size.
        // this will be used at the end of this method to determine the final scroll position
        var scrollRectSize = scroller.ScrollRectSize;

        // capture the scroller's position so we can smoothly scroll from it to the new cell
        var offset = _oldScrollPosition - scroller.ScrollSize;

        // capture the scroller dimensions so that we can reset them when we are done
        var rectTransform = scroller.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;

        // set the dimensions to the largest size possible to acommodate all the cells
        rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

        _totalCellSize = scroller.padding.top + scroller.padding.bottom;
        for (var i = 1; i < cellData.Count; i++)
        {
            _totalCellSize += cellData[i].cellSize + (i < cellData.Count - 1 ? scroller.spacing : 0);
        }
        // reset the scroller size back to what it was originally
        rectTransform.sizeDelta = size;
        scroller.ReloadData(scrollPositionFactor: scrollPositionFactor, keepPosition: keepPosition);
        if (cellData.Count <= 0) return;
        // set the scroll position to the previous cell (plus the offset of where the scroller currently is) so that we can jump to the new cell.
        if (keepPosition)
            scroller.ScrollPosition = _oldScrollPosition;
        else
            scroller.ScrollPosition = scroller.ScrollSize - 0.7f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ChatCellView cellView;

        cellView = scroller.GetCellView(chatCellPrefab) as ChatCellView;

        // set the cell's game object name. Not necessary, but nice for debugging.
        cellView.name = "ChatCell_" + dataIndex.ToString();

        // initialize the cell's data so that it can configure its view.
        cellView.SetData(cellData[dataIndex]);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellData[dataIndex].cellSize;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return cellData?.Count ?? 0;
    }
}
