using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자 : 2024-10-11
/// 작성자 : 현서 (hyeunseo812@gmail.com)
/// 클래스 용도 : 슬라이더 컬러관리
/// </summary>
public class SliderColorSet : MonoBehaviour
{
    [SerializeField] 
    private Utill_Enum.Slider_Color selectColorBG;
    public Utill_Enum.Slider_Color SelectColorBG
    {
        get { return selectColorBG; }
        set { selectColorBG = value; }
    }

    [SerializeField] 
    private Utill_Enum.Slider_Color selectColorBar;
    public Utill_Enum.Slider_Color SelectColorBar
    {
        get { return selectColorBar; }
        set { selectColorBar = value; }
    }

    [SerializeField] 
    private Image _SliderBG;
    public Image sliderBG
    {
        get { return _SliderBG; }
        set { _SliderBG = value; }
    }

    [SerializeField] 
    private Image _sliderBar;
    public Image sliderBar
    {
        get { return _sliderBar; }
        set { _sliderBar = value; }
    }


    public bool isLevelDiffecnt;
    private void Awake()
    {
        if (!isLevelDiffecnt)
        {
            SliderColor01(SelectColorBG);
            SliderColor02(SelectColorBar);
        }
    }

    public void SliderColor01(Utill_Enum.Slider_Color color)
    {
        switch (color)
        {
            case Utill_Enum.Slider_Color.SliderBG01:
                sliderBG.color = UIManager.Instance.SliderBGColor01;
                break;
            case Utill_Enum.Slider_Color.SliderBar01:
                sliderBG.color = UIManager.Instance.SliderBarColor01;
                break;
        }
    }

    public void SliderColor02(Utill_Enum.Slider_Color color)
    {
        switch (color)
        {
            case Utill_Enum.Slider_Color.SliderBG01:
                sliderBar.color = UIManager.Instance.SliderBGColor01;
                break;
            case Utill_Enum.Slider_Color.SliderBar01:
                sliderBar.color = UIManager.Instance.SliderBarColor01;
                break;
        }
    }
}
