using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 약관동의 팝업 UI 관리         
/// </summary>
public class AgreementPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] TextMeshProUGUI titleText = null;
    [Serializable]
    private struct Agreement
    {
        public TextMeshProUGUI text;
        public Toggle toggle; 
        public Button button; //동의 버튼

        public Agreement(TextMeshProUGUI text, Toggle toggle, Button button)
        {
            this.text = text;
            this.toggle = toggle;
            this.button = button;

        }
    }
    [SerializeField] Agreement[] agreements = null;
    [SerializeField] Button submit = null;
    private bool isInit = false;
    private void Awake()
    {
        if (!isInit)
        {
            Initialize();
        }
    }
    public  void Initialize()
    {
        submit.GetComponent<Basic_Prefab>().SetTypeButton(Utill_Enum.ButtonType.DeActive);

        LanguageSetting();

        OnClickEventSetting();
        
        submit.interactable = false;

        isInit = true;
    }
    private void OnClickEventSetting()
    {
        agreements[0].button.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Application.OpenURL("http://storage.thebackend.io/f9405546593a9f9903d8edf372aafc9e493577cc2157c50a2068f4f7f924fb8f/terms.html");
        });
        agreements[1].button.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Application.OpenURL("http://storage.thebackend.io/f9405546593a9f9903d8edf372aafc9e493577cc2157c50a2068f4f7f924fb8f/privacy.html");
        });

        agreements[0].toggle.onValueChanged.AddListener(_ =>
        {
            OnToggleValueChanged();
        });
        agreements[1].toggle.onValueChanged.AddListener(_ =>
        {
            OnToggleValueChanged();
        });

        submit.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            LoginManager.Instance.SetOSLoginType();
            Close();
        });
    }


    /// <summary>
    /// 토글 2개 모두 동의 했을시 확인 버튼 활성화
    /// </summary>
    void OnToggleValueChanged()
    {
        SoundManager.Instance.PlayAudio("UIClick");

        bool allTogglesOn = agreements.All(agreement => agreement.toggle.isOn);
        submit.interactable = allTogglesOn;

        if (allTogglesOn)
        {
            submit.GetComponent<Basic_Prefab>().SetTypeButton(Utill_Enum.ButtonType.Active);
        }
        else
        {
            submit.GetComponent<Basic_Prefab>().SetTypeButton(Utill_Enum.ButtonType.DeActive);
        }
    }

    /// <summary>
    /// 언어 세팅
    /// </summary>
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Agreement");

        if (submit != null)
        {
            submit.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Agree");
        }


        agreements[0].text.text = LocalizationTable.Localization("Login_TermsService");
        agreements[1].text.text = LocalizationTable.Localization("Login_TermsPersonalInfo");

        for (int i = 0; i < agreements.Length; i++)
        {
            if (agreements[i].button != null)
            {
                agreements[i].button.GetComponentInChildren<TextMeshProUGUI>().text = (LocalizationTable.Localization("Button_Content"));
            }
        }

        LocalizationTable.languageSettings += LanguageSetting;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
