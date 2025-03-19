using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :업그레이드 버튼프리펩
/// /// </summary>
public class Uprade_Prefab : MonoBehaviour
{
    private Utill_Enum.ButtonType buttonType;

    [SerializeField] private Image BtnImg;
    [SerializeField] private Image Resource_Img;
    [SerializeField] private TextMeshProUGUI Upgrade_Text;
    [SerializeField] private TextMeshProUGUI Currency_value_Text;
    [SerializeField] private bool isActive = false;

    [Header("강화 버튼 활성화")]
    public Color ActiveBtnImgColor;
    public Color ActiveResource_ImgColor;
    public Color ActiveResource_value_TextColor;
    public Color ActiveResource_value_TexColort;

    public float ActiveBtnImgColorAlpha;
    public float ActiveResource_ImgColorAlpha;
    public float ActiveResource_value_TextColorAlpha;
    public float ActiveResource_value_TexColortAlpha;


    [Header("강화 버튼 비활성화")]
    public Color DeActiveBtnImgColor;
    public Color DeActiveResource_ImgColor;
    public Color DeActiveResource_value_TextColor;
    public Color DeActiveResource_value_TexColort;

    public float DeActiveBtnImgColorAlpha;
    public float DeActiveResource_ImgColorAlpha;
    public float DeActiveResource_value_TextColorAlpha;
    public float DeActiveResource_value_TexColortAlpha;


   
     
    public void SetTypeButton(Utill_Enum.ButtonType ButtonType)
    {
        switch (ButtonType)
        {
            case Utill_Enum.ButtonType.Active:
                SetButtonState(ActiveBtnImgColor, ActiveResource_ImgColor, ActiveResource_value_TextColor, ActiveResource_value_TexColort,
                               ActiveBtnImgColorAlpha, ActiveResource_ImgColorAlpha, ActiveResource_value_TextColorAlpha, ActiveResource_value_TexColortAlpha);
                break;
            case Utill_Enum.ButtonType.DeActive:
                SetButtonState(DeActiveBtnImgColor, DeActiveResource_ImgColor, DeActiveResource_value_TextColor, DeActiveResource_value_TexColort,
                               DeActiveBtnImgColorAlpha, DeActiveResource_ImgColorAlpha, DeActiveResource_value_TextColorAlpha, DeActiveResource_value_TexColortAlpha);
                break;
        }
    }

    private void SetButtonState(Color _BtnImgColor, Color _resource_ImgColor, Color _currency_value_TextColor, Color _Resource_value_TexColort,
                                float _btnImgColorAlpha, float _imgColorAlpha, float _currency_value_TextColorAlpha, float _currency_value_TexColortAlpha)
    {
        // 버튼 이미지 색상 및 알파 값 설정
        BtnImg.color = new Color(_BtnImgColor.r, _BtnImgColor.g, _BtnImgColor.b, _btnImgColorAlpha);

        // 통화 이미지 색상 및 알파 값 설정
        Resource_Img.color = new Color(_resource_ImgColor.r, _resource_ImgColor.g, _resource_ImgColor.b, _imgColorAlpha);

        // 텍스트 색상 및 알파 값 설정
        Currency_value_Text.color = new Color(_currency_value_TextColor.r, _currency_value_TextColor.g, _currency_value_TextColor.b, _currency_value_TextColorAlpha);
        Upgrade_Text.color = new Color(_Resource_value_TexColort.r, _Resource_value_TexColort.g, _Resource_value_TexColort.b, _currency_value_TexColortAlpha);
    }
}
