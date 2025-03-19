using AppleAuth;
using AppleAuth.Interfaces;
using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-31
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 설정 UI 팝업 관리 및 설정에 관련된 기능들
/// </summary>
public class SettingPopUp : MonoBehaviour, IPopUp
{
    public GameObject[] downObjs;
    public int ClickCount = 0;

    private static string VersionFormat = "VER. {0}";

    public AudioMixer mixer = null;
    public AudioMixer bgmMixer = null;
    [SerializeField] private GameObject sliderObject = null;
    [SerializeField] private Button sliderDeactiveBtn = null;

    private bool isInit = false;

    [Header("Button")]
    [SerializeField] private Button couponSubmitBtn = null;

    [SerializeField] private Button languageBtn = null;
    [SerializeField] private Button optimizationBtn = null;

    [SerializeField] private Button bgmBtn = null;
    [SerializeField] private Button fxBtn = null;

    [SerializeField] private Button rateBtn = null;
    [SerializeField] private Button sharingBtn = null;

    [SerializeField] private Button appleFedBtn = null;
    [SerializeField] private Button googleFedBtn = null;

    [SerializeField] private Button logoutBtn = null;
    [SerializeField] private Button withdrawalBtn = null;

    [SerializeField] private Button communityBtn = null;
    [SerializeField] private Button reportBtn = null;
    [SerializeField] private Button creditBtn = null;

    [SerializeField] private Button closeBtn = null;


    [Header("Text")]
    [SerializeField] private TextMeshProUGUI titleText = null;
    [SerializeField] private TMP_InputField coupon = null;
    [SerializeField] private TextMeshProUGUI versionText = null;

    [SerializeField] private string aosSetupUrl = null;
    [SerializeField] private string iosSetupUrl = null;

    [SerializeField] private Slider Slider = null;

    [Header("문의하기 링크")]
    [SerializeField] private string reportURL = null;
    [Header("평점강화 링크")]
    [SerializeField] private string rateURL = null;
    [Header("공유하기 링크")]
    [SerializeField] private string sharingURL = null;

    private bool isMuteFx = false;
    private bool isMuteBgm = false;

    private float fxValueBeforeMute = 0f;
    private float bgmValueBeforeMute = 0f;

    private void Start()
    {
        sliderObject.SetActive(false);

        if (!isInit)
            Initialize();
    }
    public void SoundSetting()
    {
        float bgm = PlayerPrefs.GetFloat("BGMVolume");
        float fx = PlayerPrefs.GetFloat("FXVolume");
        BGMSliderAction(bgm);
        FXSliderAction(fx);
    }
    public void Initialize()
    {

        if (PlayerPrefs.GetString("LoginType") == "Guest")
        {
            appleFedBtn.gameObject.SetActive(true);
            googleFedBtn.gameObject.SetActive(true);
        }
        else
        {
            appleFedBtn.gameObject.SetActive(false);
            googleFedBtn.gameObject.SetActive(false);
        }

        LanguageSetting();

        OnClickEventSetting();

        versionText.text = string.Format(VersionFormat, Application.version);

        isInit = true;
    }
    public void Initialize(string json)
    {
        //if (!isInit)
        //    Initialize();

        //var dict = TableUtility.JsonToDictionary(json, str => str, str => str);
        //Utility.RemoveBrackets(dict, new[] { "FXVolume", "BGMVolume" });
        //mixer.SetFloat("BGMVolume", float.Parse(dict["BGMVolume"]));
        //mixer.SetFloat("FXVolume", float.Parse(dict["FXVolume"]));

    }
    private void OnClickEventSetting()
    {
        sliderDeactiveBtn.onClick.AddListener(() =>
        {
            SliderDeactive();

        });
        languageBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.laguagePopUp.Show();
        });
        optimizationBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.optimizationPopUp.Show();
        });
        bgmBtn.onClick.AddListener(() =>
        {
            BGM();
        });
        fxBtn.onClick.AddListener(() =>
        {
            FX();
        });
        communityBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.communityPopUp.Show();
        });
        reportBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            ReportButtonAction();
        });
        creditBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.creditPopUp.Show();
        });
        sharingBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            SharingButtonAction();
        });
        rateBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            RateButtonAction();
            NotificationDotManager.Instance.SetIsChecked(Utill_Enum.CheckableNotificationDotType.RateCheck, true);
        });
        withdrawalBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            UIManager.Instance.withdrawalPopUp.Show();
        });
        googleFedBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            GoogleFed();
        });
        appleFedBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            StartCoroutine(AppleFed());
        });

        logoutBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio("UIClick");
            Logout();
        });
    }
    public void BGM()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        sliderObject.SetActive(true);
        //mixer.GetFloat("BGMVolume", out var value);
        //value = Mathf.Pow(10, value / 30);
        float value = PlayerPrefs.GetFloat("BGMVolume");

        Slider.onValueChanged.RemoveAllListeners();
        Slider.value = value;
        Slider.onValueChanged.AddListener(BGMSliderAction);
        closeBtn.gameObject.SetActive(false);
        versionText.gameObject.SetActive(false);
    }
    public void FX()
    {
        SoundManager.Instance.PlayAudio("UIClick");
        sliderObject.SetActive(true);
        //mixer.GetFloat("FXVolume", out var value);
        //value = Mathf.Pow(10, value / 30);
        float value = PlayerPrefs.GetFloat("FXVolume");
        Slider.onValueChanged.RemoveAllListeners();
        Slider.value = value;
        Slider.onValueChanged.AddListener(FXSliderAction);
        closeBtn.gameObject.SetActive(false);
        versionText.gameObject.SetActive(false);

        //PlayerPrefs.SetFloat("FXVolume", value);
        //Debug.Log(value);   
        //PlayerPrefs.Save();
    }
    public void GoogleFed()
    {
#if UNITY_IOS
         TheBackend.ToolKit.GoogleLogin.iOS.GoogleLogin(GoogleLoginCallback);
#endif
    }

    string ios_token = null;

    public IEnumerator AppleFed()
    {
        if (ios_token == null)
        {
            if (LoginManager.Instance.authManager == null)
            {
                LoginManager.Instance.authManager = new AppleAuthManager(new AppleAuth.Native.PayloadDeserializer());
            }

            LoginManager.Instance.authManager.LoginWithAppleId(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName,
                success =>
                {
                    var appleIdCredential = success as IAppleIDCredential;
                    ios_token = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    Debug.Log($"Apple Login Success {ios_token}");
                },
                error =>
                {
                    Debug.Log($"Apple LoginError {error}");
                });
        }
        yield return new WaitUntil(() => ios_token != null);
        BackendReturnObject bro = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.ChangeCustomToFederation(ios_token, FederationType.Apple), true);
        if (bro.IsSuccess())
        {
            //노티스 계정이 성공적으로 변환되었습니다라는 문구 출력

            appleFedBtn.gameObject.SetActive(false);
            googleFedBtn.gameObject.SetActive(false);
            PlayerPrefs.SetString("LoginToken", ios_token);
            PlayerPrefs.SetString("LoginType", "IOSApple");
            PlayerPrefs.Save();
        }
        else
        {
            if (bro.GetStatusCode() == "409")
            {
                //이미 계정이 있어서 전환할수 없습니다라는 팝업출력
                BackendErrorManager.Instance.SettingPopUp(bro.GetStatusCode(), bro.GetErrorCode(), LocalizationTable.Localization("Message_DuplicatedParameterException"));
            }
            else
                BackendErrorManager.Instance.SettingPopUp(bro);
        }
    }
    public void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
    {
        if (isSuccess == false)
        {
            Debug.LogError(errorMessage);
            return;
        }
        else
        {
            BackendReturnObject bro = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.ChangeCustomToFederation(token, FederationType.Google), true);

            if (bro.IsSuccess())
            {
                //노티스 계정이 성공적으로 변환되었습니다라는 문구 출력

                appleFedBtn.gameObject.SetActive(false);
                googleFedBtn.gameObject.SetActive(false);

                PlayerPrefs.SetString("LoginToken", token);
                PlayerPrefs.SetString("LoginType", "IOSGoogle");
                PlayerPrefs.Save();
            }
            else
            {
                if (bro.GetStatusCode() == "409")
                {
                    //이미 계정이 있어서 전환할수 없습니다라는 팝업출력
                    BackendErrorManager.Instance.SettingPopUp(bro.GetStatusCode(), bro.GetErrorCode(), LocalizationTable.Localization("Message_DuplicatedParameterException"));
                }
                else
                    BackendErrorManager.Instance.SettingPopUp(bro);
            }
        }
    }
    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Setting");

        (coupon.placeholder as TextMeshProUGUI).text = LocalizationTable.Localization("Setting_EnterCoupon");

        couponSubmitBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Check");

        languageBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_LanguageSetting");
        optimizationBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Optimization");

        bgmBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Setting_BGM");
        fxBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Setting_FX");

        rateBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Grade");
        sharingBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Share");

        appleFedBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_AppleFed");
        googleFedBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_GoogleFed");

        logoutBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Logout");
        withdrawalBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Withdrawal");

        communityBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Community");
        reportBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Report");
        creditBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Credit");

        LocalizationTable.languageSettings += LanguageSetting;
    }
    public void SliderDeactive()
    {
        closeBtn.gameObject.SetActive(true);
        versionText.gameObject.SetActive(true);
        sliderObject.SetActive(false);
    }
    public void Logout()
    {
        if (BackendManager.Instance.IsLocal)
            return;

        StreamingReader.SaveAllData();

        string loginType = PlayerPrefs.GetString("LoginType");
        if (loginType != "Guest")
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }


        BackEnd.Backend.BMember.Logout();

        LoginManager.Instance.SignOutGoogleLogin();
        LoginManager.Instance.SingOutIOSGoogleLogin();


#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    public void Withdraw()
    {

    }
    #region 공유하기 
    private void SharingButtonAction()
    {
        Application.OpenURL(sharingURL);
    }
    #endregion
    #region 평점강화 
    private void RateButtonAction()
    {
        Application.OpenURL(rateURL);
    }
    #endregion
    #region 문의하기 카톡
    private void ReportButtonAction()
    {
        Application.OpenURL(reportURL);
    }
    #endregion
    private void BGMSliderAction(float value)
    {
        if (value == 0)
            value = 0.00001f;
        if (value == 1)
            value = 0.99999f;
        bgmMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 30);
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    private void FXSliderAction(float value)
    {
        if (value == 0)
            value = 0.00001f;
        if (value == 1)
            value = 0.99999f;
        mixer.SetFloat("FXVolume", Mathf.Log10(value) * 30);
        PlayerPrefs.SetFloat("FXVolume", value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 효과음 일시 무음처리
    /// (게임 끌 시 저장 안 됨)
    /// </summary>
    public void MuteFx()
    {
        isMuteFx = true;

        mixer.GetFloat("FXVolume", out var fxValue);
        fxValueBeforeMute = fxValue;
        mixer.SetFloat("FXVolume", -80);
    }

    /// <summary>
    /// 배경음 일시 무음처리
    /// (게임 끌 시 저장 안 됨)
    /// </summary>
    public void MuteBGM()
    {
        isMuteBgm = true;
        bgmMixer.GetFloat("BGMVolume", out var bgmValue);
        bgmValueBeforeMute = bgmValue;
        bgmMixer.SetFloat("BGMVolume", -80);
    }

    /// <summary>
    /// 효과음 일시 무음처리 해제
    /// </summary>
    public void UnMuteFx()
    {
        isMuteFx = false;
        mixer.SetFloat("FXVolume", fxValueBeforeMute);
        fxValueBeforeMute = 0;
        PlayerPrefs.SetFloat("FXVolume", fxValueBeforeMute);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 배경음 일시 무음처리 해제
    /// </summary>
    public void UnMuteBGM()
    {
        isMuteBgm = false;
        bgmMixer.SetFloat("BGMVolume", bgmValueBeforeMute);
        bgmValueBeforeMute = 0;
        PlayerPrefs.SetFloat("BGMVolume", bgmValueBeforeMute);
        PlayerPrefs.Save();
    }

    public override string ToString()
    {
        float fxValue = 0f;
        float bgmValue = 0f;

        if (!isMuteFx)
            mixer.GetFloat("FXVolume", out fxValue);
        else
            fxValue = fxValueBeforeMute; //무음이었을 경우엔 이젠 값 저장
        if (!isMuteBgm)
            bgmMixer.GetFloat("BGMVolume", out bgmValue);
        else
            bgmValue = bgmValueBeforeMute; //무음이었을 경우엔 이젠 값 저장

        var returnValue = $"{{FXVolume:{{{fxValue}}},BGMVolume:{{{bgmValue}}}}}";

        return returnValue;
    }

    public void Close()
    {
    }

    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove();
        for (int i = 0; i < downObjs.Length; i++)
        {
            downObjs[i].SetActive(true);
        }

        if (UIManager.Instance.laguagePopUp.gameObject.activeSelf)
        {
            UIManager.Instance.optimizationPopUp.Hide();
        }
        if (UIManager.Instance.optimizationPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.optimizationPopUp.Hide();
        }
        if (UIManager.Instance.communityPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.communityPopUp.Hide();
        }
        if (UIManager.Instance.creditPopUp.gameObject.activeSelf)
        {
            UIManager.Instance.creditPopUp.Hide();
        }

        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            for (int i = 0; i < downObjs.Length; i++)
            {
                downObjs[i].SetActive(false);
            }
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }

    }


}
