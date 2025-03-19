using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-09-11
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 업적 보상의 UI를 제어 (판단하는 부분만 자식에서 구현해야함)
/// </summary>
public abstract class AchievementSystem : MonoBehaviour
{
    [SerializeField]
    List<Slider> sliders = new List<Slider>();
    [SerializeField]
    List<TMP_Text> titles = new List<TMP_Text>();

    [SerializeField]
    protected GameObject CellPrefab = null;

    [SerializeField]
    protected float Content_Length = 500f;

    public ScrollRect scrollRect; //하위 셀이 눌려도 스크롤 될 수 있도록 하기 위해 생성됨
    protected bool isInit = false;
    protected Dictionary<int, List<AchievementHolder>> holderListDictionary = new(); //추후 다중 슬라이더 고려하여 딕셔너리로 제작함. 슬라이더별 가지고있는 홀더 딕셔너리

    /// <summary>
    /// 데이터를 기반으로 셀을 생성하고 UI를 설정
    /// </summary>
    public void InitSliders()
    {
        if (scrollRect.horizontalNormalizedPosition >= 0.15)
        {
            scrollRect.horizontalNormalizedPosition = 0.13f;
        }
       
        holderListDictionary.Clear();
        for (int i = 0; i < sliders.Count; i++)
        {
            //소제목 텍스트 설정
            titles[i].text = GetTitleName(i);
            SetupSlider(i);
            int idx = GetCellLength(i);
            for (int j = 0; j < idx; j++)
            {
                AddCell(i, j);
            }
        }
    }
    /// <summary>
    /// 존재하는 리스트를 기반으로 UI를 갱신
    /// </summary>
    public void RefrashSliders()
    {
        scrollRect.horizontalNormalizedPosition = 0f;
        for (int i = 0; i < sliders.Count; i++)
        {
            //소제목 텍스트 설정
            titles[i].text = GetTitleName(i);
            SetupSlider(i);

            int idx = GetCellLength(i);
            for (int j = 0; j < idx; j++)
            {
                RefrashCell(i, j);
            }
        }
    }

    #region 슬라이더 별 반환 함수
    public abstract string GetTitleName(int sliderIndex);

    /// <summary>
    /// 각 슬라이더 별로 참조해야할 값을 반환
    /// </summary>
    public abstract int GetTotalAchiveValue(int sliderIndex);

    public abstract int GetCellLength(int sliderIndex);

    /// <summary>
    ///  각 슬라이더 별로 셀을 제작할 때 필요한 정보들을 반환
    /// </summary>
    public abstract (string rewardName, int rewardCount, int needCount) GetCurrentAchiveValue(int sliderIndex, int cellIndex);

    /// <summary>
    /// 각 슬라이더 별로 데이터상의 최댓값을 반환
    /// </summary>
    public abstract float GetMaximumAchiveValue(int sliderIndex);

    /// <summary>
    /// 특정 인덱스를 기반으로 그 인덱스의 아이템을 받았는지 반환
    /// </summary>
    public abstract bool IsReceived(int checkIndex);

    /// <summary>
    /// 특정 인덱스의 아이템을 받기 여부 세팅
    /// </summary>
    public abstract void SetReceived(int checkIndex, bool value);
    #endregion

    /// <summary>
    /// 슬라이더 초기화 및 설정
    /// </summary>
    protected void SetupSlider(int sliderIndex)
    {
        // 슬라이더의 부모격인 콘텐트의 사이즈를 Content_Length사이즈로 변경
        RectTransform content_Rect = sliders[sliderIndex].transform.parent.GetComponent<RectTransform>();
        content_Rect.sizeDelta = new Vector2(Content_Length, content_Rect.sizeDelta.y);

        // 변경된 콘텐트의 사이즈의 길이를 슬라이더에도 대응 ( new Vector2(Content_Length - 100.0f) <- 해당 부분에 100.0f를 다른 값으로 바꾸시면 배경과, 슬라이더의 스페이싱이 넓어 집니다.
        sliders[sliderIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(Content_Length - 200.0f, sliders[sliderIndex].GetComponent<RectTransform>().sizeDelta.y);
        // 현재 슬라이더의 값 (플레이어의 값을 데이터의 최고 수치의 값으로 나눔 )
        sliders[sliderIndex].value = GetTotalAchiveValue(sliderIndex) / GetMaximumAchiveValue(sliderIndex);

        // 슬라이더의 Fill 오브젝트가 0일때는 비활성화
        if (sliders[sliderIndex].value <= 0.0f)
        {
            sliders[sliderIndex].transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            sliders[sliderIndex].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 현재 슬라이더와 셀의 인덱스 정보로 알맞은 셀을 생성하고 위치를 조절함
    /// </summary>
    protected void AddCell(int sliderIndex, int cellIndex)
    {
        AchievementHolder cell = Instantiate(CellPrefab, transform.position, Quaternion.identity, sliders[sliderIndex].transform).GetComponent<AchievementHolder>();
        var curData = GetCurrentAchiveValue(sliderIndex, cellIndex);

        //천단위 콤마
        string amountStr = string.Format("{0:#,###0}", curData.rewardCount);
        //이름이랑 합치기
        string titleStr = $"{LocalizationTable.Localization($"Common_{curData.rewardName}")}\n{amountStr}";

        cell.SubmitButtonSetting(curData.rewardName);
        cell.Setting(this, sliderIndex, cellIndex, curData.rewardName, titleStr, curData.rewardCount, GetTotalAchiveValue(sliderIndex), curData.needCount, true);
        // Content_Length 로 측정된 길이의 값을 가져옵니다.
        // 이미지의 x좌표를 맞추기 위하여 해당 길이가 필요합니다.
        float slider_delta_Size = sliders[sliderIndex].GetComponent<RectTransform>().sizeDelta.x;

        // 데이터상으로 가장 마지막 열에 해당하는 값을 최고치로 측정
        float Maximum_Count = GetMaximumAchiveValue(sliderIndex);
        // 현재 열에 값
        float Get_value = curData.needCount;
        // 두 값을 나누어서 슬라이더의 길이 조절
        float slider_Get_value = (Get_value / Maximum_Count);

        float posX = (slider_delta_Size * slider_Get_value) - (slider_delta_Size / 2);
        // 포지션 반영
        cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);

        if (holderListDictionary.ContainsKey(sliderIndex))
            holderListDictionary[sliderIndex].Add(cell);
        else
        {
            holderListDictionary.Add(sliderIndex, new List<AchievementHolder>() { cell });
        }
    }

    /// <summary>
    /// 슬라이더와 셀의 인덱스 정보로 찾은 셀의 UI를 갱신하고 위치를 조절함
    /// </summary>
    protected void RefrashCell(int sliderIndex, int cellIndex)
    {
        if (!holderListDictionary.ContainsKey(sliderIndex)) return;
        if (holderListDictionary[sliderIndex].Count <= cellIndex) return;

        AchievementHolder cell = holderListDictionary[sliderIndex][cellIndex];
        var curData = GetCurrentAchiveValue(sliderIndex, cellIndex);

        //천단위 콤마
        string amountStr = string.Format("{0:#,###0}", curData.rewardCount);
        //이름이랑 합치기
        string titleStr = $"{LocalizationTable.Localization($"Common_{curData.rewardName}")}\n{amountStr}";
        cell.Setting(this, sliderIndex, cellIndex, curData.rewardName, titleStr, curData.rewardCount, GetTotalAchiveValue(sliderIndex), curData.needCount, true);
        // Content_Length 로 측정된 길이의 값을 가져옵니다.
        // 이미지의 x좌표를 맞추기 위하여 해당 길이가 필요합니다.
        float slider_delta_Size = sliders[sliderIndex].GetComponent<RectTransform>().sizeDelta.x;

        // 데이터상으로 가장 마지막 열에 해당하는 값을 최고치로 측정
        float Maximum_Count = GetMaximumAchiveValue(sliderIndex);
        // 현재 열에 값
        float Get_value = curData.needCount;
        // 두 값을 나누어서 슬라이더의 길이 조절
        float slider_Get_value = (Get_value / Maximum_Count);

        float posX = (slider_delta_Size * slider_Get_value) - (slider_delta_Size / 2);
        // 포지션 반영
        cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
}
