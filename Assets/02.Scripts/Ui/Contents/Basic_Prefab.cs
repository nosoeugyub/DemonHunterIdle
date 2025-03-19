using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 기본 ui프리펩
/// /// </summary>
public class Basic_Prefab : MonoBehaviour
{
    
    private Utill_Enum.ButtonType buttonType;

    [SerializeField] private Image BtnImg;
    [SerializeField] private TextMeshProUGUI BtnText;

    [SerializeField] private Image Resourcesimg;
    [SerializeField] private TextMeshProUGUI ResourceText;
    [SerializeField] private TextMeshProUGUI ResourceBtnText;
    [SerializeField] private bool isDeActive = false;



    [Header("활성화")]
    public Color ActiveImgColor;
    public float ActiveImgAlphaValue;

    public Color ActiveTextColor;
    public float ActiveTextAlphaValue;

    [Header("비활성화")]
    public Color DeActiveImgColor;
    public float DeActiveImgAlphaValue;

    public Color DeActiveTextColor;
    public float DeActiveTextAlphaValue;

    [Header("재화 활성화")]
    public Color ResourceActiveTextColor;
    public float ResourceActiveTextAlphaValue;


    [Header("재화 비활성화")]
    public Color ResourceDeActiveTextColor;
    public float ResourceDeActiveTextAlphaValue;

    public bool isAwake;
    private void Awake()
    {
        if (isAwake)
        {
            return;
        }

        if (isDeActive)
        {
            SetTypeButton(Utill_Enum.ButtonType.DeActive);
        }
        else
        {
            SetTypeButton(Utill_Enum.ButtonType.Active);
        }
    }

    public void SetTypeButton(Utill_Enum.ButtonType ButtonType)
    {
        switch (ButtonType)
        {
            case Utill_Enum.ButtonType.Active:
                SetButtonState(ActiveImgColor, ActiveImgAlphaValue, ActiveTextColor, ActiveTextAlphaValue);
                //재화 리소스도 활성화 색깔 타게 (화이트 ,화이트) 
                if (Resourcesimg != null)
                {
                    //이미지 원상복구
                    Resourcesimg.material = null;
                }
                
                if(ResourceText != null)
                {
                    Color newTextColor = ResourceActiveTextColor;
                    newTextColor.a = Mathf.Clamp01(ResourceActiveTextAlphaValue);
                    //ResourceText.color = newTextColor;
                    ResourceBtnText.color = newTextColor;
                }
                break;
            case Utill_Enum.ButtonType.DeActive:
                SetButtonState(DeActiveImgColor, DeActiveImgAlphaValue, DeActiveTextColor, DeActiveTextAlphaValue);

                //재화 리소스도 비활성화 색깔 타게 (흑백 / 그레이)
                if (Resourcesimg != null)
                {
                    //이미지 흑백
                    // Black 머테리얼을 Resources에서 로드하여 할당
                    Utill_Standard.SetImageMaterial(Resourcesimg, "Black");
                }

                if (ResourceText != null)
                {
                    Color newTextColor = ResourceDeActiveTextColor;
                    newTextColor.a = Mathf.Clamp01(ResourceDeActiveTextAlphaValue);
                    //ResourceText.color = newTextColor;
                    ResourceBtnText.color = newTextColor;
                }
                break;
        }
    }



    private void SetButtonState(Color imgColor, float imgAlpha, Color textColor, float textAlpha)
    {
        if (BtnImg != null)
        {
            Color newImgColor = imgColor;
            newImgColor.a = Mathf.Clamp01(imgAlpha);
            BtnImg.color = newImgColor;
        }
        else
        {
            Debug.LogWarning("BtnImg is not assigned.");
        }

        if (BtnText != null)
        {
            Color newTextColor = textColor;
            newTextColor.a = Mathf.Clamp01(textAlpha);
            BtnText.color = newTextColor;
        }
        else
        {
            Debug.LogWarning("BtnText is not assigned.");
        }
    }
}
