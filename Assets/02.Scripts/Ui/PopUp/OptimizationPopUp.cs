using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 최적화 팝업 UI 처리                                                  
/// </summary>
public class OptimizationPopUp : MonoBehaviour, IPopUp
{
    [Serializable]
    private struct OptimizationOption
    {
        public TextMeshProUGUI text;
        public Toggle toggle;
        public OptimizationOption(TextMeshProUGUI text, Toggle toggle)
        {
            this.text = text;
            this.toggle = toggle;
        }
    }

    [SerializeField] OptimizationOption[] optimizationOptions = null;

    [SerializeField] TextMeshProUGUI titleText = null;
    private bool isInit;
    public int ClickCount = 0;

    private void Awake()
    {
        if (!isInit)
        {
            Initialize();
        }
    }
    public void Initialize()
    {
        LanguageSetting();
        InitializeToggle();
        isInit = true;
    }

    /// <summary>
    /// 언어세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Optimization");
        LocalizationTable.languageSettings += LanguageSetting;

    }
    /// <summary>
    /// 토글 UI 눌렀을때 설정
    /// </summary>
    public void InitializeToggle()
    {
        optimizationOptions[0].toggle.isOn = GameManager.Instance.IsHpOn;
        optimizationOptions[0].toggle.onValueChanged.AddListener((bool isOn) => OnChangeHp());
        optimizationOptions[1].toggle.isOn = GameManager.Instance.IsDamageOn;
        optimizationOptions[1].toggle.onValueChanged.AddListener((bool isOn) => OnChangeDamage());
        optimizationOptions[2].toggle.isOn = GameManager.Instance.IsShadowOn;
        optimizationOptions[2].toggle.onValueChanged.AddListener((bool isOn) => OnChangeShadow());
        optimizationOptions[3].toggle.isOn = GameManager.Instance.IsGrapicOn;
        optimizationOptions[3].toggle.onValueChanged.AddListener((bool isOn) => OnChangeGrapic());
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        ClickCount = 0;

        PopUpSystem.Instance.EscPopupListRemove();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }
    }
    public void OnChangeHp()
    {
        GameManager.Instance.IsHpOn = !GameManager.Instance.IsHpOn;
    }
    public void OnChangeDamage()
    {
        GameManager.Instance.IsDamageOn = !GameManager.Instance.IsDamageOn;
    }
    public void OnChangeShadow()
    {
        GameManager.Instance.IsShadowOn = !GameManager.Instance.IsShadowOn;
    }
    public void OnChangeGrapic()
    {
        GameManager.Instance.IsGrapicOn = !GameManager.Instance.IsGrapicOn;
    }
}
