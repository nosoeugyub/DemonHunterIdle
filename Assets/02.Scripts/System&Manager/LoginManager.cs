using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;
using UnityEngine.Android;
using AppleAuth;
using AppleAuth.Interfaces;
using UnityEngine.Audio;
using System;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public enum LoginType 
{
    Guest,
    GoogleApple,
    GoogleAppleGuest,
}

/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 로그인할때 뒤끝 서버 연결 및 구글 애플 게스트 로그인 로직 
/// </summary>
/// 
public class LoginManager : MonoSingleton<LoginManager>
{
    public AppleAuthManager authManager;

    [SerializeField] private AudioMixer mixer = null;

    [SerializeField] private LoginType loginType;

    [SerializeField] AgreementPopUp agreementPopUp = null;
    [SerializeField] NicknamePopUp nicknamePopUp = null;
    [SerializeField] GuestWarningPopUp guestWarningPopUp = null;

    [SerializeField] private Button googleLoginButton = null; // 구글 로그인 버튼
    [SerializeField] private Button appleLoginButton = null; // 애플  로그인 버튼
    [SerializeField] private Button guestLoginButton = null; // 게스트 로그인 버튼

    [SerializeField] private Button reAutoNicknameBtn = null;

    [SerializeField] private TMP_InputField nicknameInputField = null; // 닉네임 입력 필드


    [SerializeField] private Button TouchToStartBtn = null;

    [SerializeField] private GameObject loading = null;

    [SerializeField] private TextMeshProUGUI versionTxt = null;


    private string nicknameKey = "PlayerNickname";

    public string NicknameKey => nicknameKey;

    private bool isLoading = false;

    private bool checkTryConnect;

    bool isLogin;
    private bool isMaintainance = false;

    //에러팝업띄우기위한..
    [SerializeField] private Canvas ErrorPopUpSpwanCanvas;
    [SerializeField] private RectTransform ErrorPopUpSpwanRect;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        Initilize();
    }

    private void Initilize()
    {
        LocalizationTable.localizationDict = CSVReader.LoadLanguageSheet("Language");//csv파일
        isLogin = false;

        agreementPopUp.Hide();
        nicknamePopUp.Hide();
        Time.timeScale = 1;
        LanguageSetting();
        OnClickEventSetting();
    }
    private void OnClickEventSetting()
    {
        guestLoginButton.onClick.AddListener(GuestPopUp);

        googleLoginButton.onClick.AddListener(() =>
        {
#if UNITY_ANDROID
            AOSGoogleLogin();
#endif

#if UNITY_IOS
            IOSGoogleLogin();
#endif
        });

        appleLoginButton.onClick.AddListener(() =>
        {
#if UNITY_ANDROID
            AOSAppleLogin();
#endif


#if UNITY_IOS
            StartCoroutine(IOSAppleLogin());         
#endif
        });
    }

    private void Start()
    {
        SoundManager.Instance.PlayAudioLoop("BGM_Login_01");
    
        StartCoroutine(Login());

        SoundSetting();
    }
    private void SoundSetting()
    {
        float bgm = PlayerPrefs.GetFloat("BGMVolume");
        float fx = PlayerPrefs.GetFloat("FXVolume");

        if (bgm == 0)
            bgm = 0.00001f;
        if (bgm == 1)
            bgm = 0.99999f;

        if (fx == 0)
            fx = 0.00001f;
        if (fx == 1)
            fx = 0.99999f;

        mixer.SetFloat("BGMVolume", Mathf.Log10(bgm) * 30);

        mixer.SetFloat("FXVolume", Mathf.Log10(fx) * 30);

    }

  
    private IEnumerator Login()
    {
        int isFirst = PlayerPrefs.GetInt("IsFirst");

        HandleLocalization();

        Backend.InitializeAsync(true, true, HandleBackendCallback);

        //안드로이드와 IOS에서만 버전 검사
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
          yield return StartCoroutine(CheckVersion());
#endif
        
        yield return StartCoroutine(HandleUserAuthentication());

        if (isMaintainance)
            yield break;
        
        string loginType = PlayerPrefs.GetString("LoginType");
        string token = PlayerPrefs.GetString("LoginToken");

        Debug.Log(loginType);

        if (isFirst == 1)
        {
            Backend.BMember.LoginWithTheBackendToken((callback) => //뒤끝 토큰으로 로그인
            {
                callback = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.LoginWithTheBackendToken(),true,callback);
                if (callback.IsSuccess()) //성공시 메인신
                {
                    Debug.Log("토큰을 이용한 자동 로그인에 성공했습니다");
                    RealLogin();
                }
                else //실패시 이전 로그인타입에 따라 처리
                {
                    Debug.Log("토큰을 이용한 자동 로그인에 실패하였습니다");

                    if (loginType == "AOSGoogle")
                    {
                        AOSGoogleLogin();
                    }
                    else if (loginType == "IOSGoogle")
                    {
                        IOSGoogleLogin();
                    }
                    else if (loginType == "AOSApple")
                    {
                        var bro = BackendErrorManager.Instance.RetryLogic(()=>Backend.BMember.AuthorizeFederation(token, FederationType.Apple, "siwa"),true);

                        if (bro.IsSuccess())
                        {
                            Debug.Log("애플 로그인 성공");
                            RealLogin();
                        }
                        else
                        {
                            if (bro.GetMessage().Contains("Token") || bro.GetMessage().Contains("refresh"))
                            {
                                Debug.Log("토큰 갱신 필요");
                                RefreshAOSAppleTokenAndLogin();
                            }
                            else
                            {
                                Debug.Log("애플 로그인 실패 코드  " + bro.GetErrorCode() + "  메세지   " + bro.GetMessage());

                                BackendErrorManager.Instance.SettingPopUp(bro, true);
                            }
                        }
                    }
                    else if (loginType == "IOSApple")
                    {
                        var bro = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.AuthorizeFederation(token, FederationType.Apple, "siwa"),true);

                        if (bro.IsSuccess())
                        {
                            Debug.Log("애플 로그인 성공");
                            RealLogin();
                        }
                        else
                        {
                            if (bro.GetMessage().Contains("Token") || bro.GetMessage().Contains("refresh"))
                            {
                                Debug.Log("토큰 갱신 필요");
                                StartCoroutine(RefreshIOSAppleTokenAndLogin());
                            }
                            else
                            {
                                BackendErrorManager.Instance.SettingPopUp(bro, true);
                                Debug.Log("애플 로그인 실패 코드  " + bro.GetErrorCode() + "  메세지   " + bro.GetMessage());
                            }
                        }
                    }
                    else if (loginType.Contains("Guest"))
                    {
                        if (callback.GetMessage().Contains("refresh"))
                        {
                            agreementPopUp.Show();
                        }
                    }
                    else
                    {
                        agreementPopUp.Show();
                    }

                }
            });
        }
        else
        {
            agreementPopUp.Show();
        }
    }
 
    void HandleLocalization()
    {
        int isFirst = PlayerPrefs.GetInt("IsFirst");

        if (isFirst != 1)
        {
            Debug.Log("처음 랭귀지 세팅");
            LocalizationTable.SetLanguage(Application.systemLanguage == SystemLanguage.Korean ? ELanguage.KR : ELanguage.EN);
        }
        else
        {
            string str = PlayerPrefs.GetString("Language");
            Debug.Log("저장되있는 랭귀지 세팅" + str);
            LocalizationTable.SetLanguage(str == "KR" ? ELanguage.KR : ELanguage.EN);
        }
    }
    void HandleBackendCallback(BackendReturnObject bro)
    {
        if (bro.IsSuccess())
        {
            // 서버시간 획득
            Debug.Log("서버상태:  " + Backend.Utils.GetServerStatus().GetReturnValue());

            BackendErrorManager.Instance.ErrorHandlerInit();

            if (Backend.Utils.GetServerStatus().GetReturnValue().Contains("0"))
            {
               //온라인 
            }
            if (Backend.Utils.GetServerStatus().GetReturnValue().Contains("1"))
            {
                //오프라인이면 일단 전부 저장
            }
            if (Backend.Utils.GetServerStatus().GetReturnValue().Contains("2"))
            {
                Debug.Log("서버 점검 상태");
                var popup = BackendErrorManager.Instance.SettingPopUp("401", "BadUnauthorizedException", LocalizationTable.Localization("Message_Maintenance"), true);
                if(popup != null)
                {
                    popup.Show();
                }
                isMaintainance = true;
                return;
            }

            Debug.Log("서버타임: " + Backend.Utils.GetServerTime());

            Backend.Notification.OnAuthorize = (bool result, string reason) =>
            {
                if (result)
                {
                    checkTryConnect = false;
                }
                else
                {
                    if (checkTryConnect)
                    {
                        return;
                    }
                    checkTryConnect = true;
                    Invoke(nameof(Reconnected), 5f);
                }

                Debug.Log($"실시간 서버 연결 : {result}, {reason}");
            };
            Backend.Notification.OnDisConnect = (string reason) =>
            {
                UIManager.Instance.internetErrorPopUp.Show();
                Debug.Log(reason);
            };
            Backend.Notification.OnReceivedUserPost += () =>
            {
                UIManager.Instance.mailPopUp.MailCount++;
            };
            Backend.Notification.OnNewPostCreated += (BackEnd.Socketio.PostRepeatType postRepeatType, string title, string content, string author) =>
            {
                UIManager.Instance.mailPopUp.MailCount++;
                Debug.Log("New Mail");
            
            };

            Backend.Notification.OnNewNoticeCreated = (string title, string content) => {
                Debug.Log(
                    $"[OnNewNoticeCreated(새로운 공지사항 생성)]\n" +
                    $"| title : {title}\n" +
                    $"| content : {content}\n"
                );

                // 팝업 창을 열고 공지 내용을 표시
                //_notificationpopUpView.gameObject.SetActive(true);
                //_notificationpopUpView.Setting_Noti(title, content);
            };

            //네트워크 체크
        }
        // 실패
        else
        {
            Debug.LogError("Failed to initialize the backend");
        }
    }
    private void Reconnected()
    {
        checkTryConnect = false;
        Backend.Notification.Connect();
    }
    /// <summary>
    /// 구글 계정 로그인 성공 여부 확인 후 다음 단계로 진행 ->
    /// iOS에서 특별한 처리를 하지 않고, 게스트 로그인도 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator HandleUserAuthentication()
    {
        var isSuccess = false;
        Social.localUser.Authenticate((success, errorMessage) =>
        {
            Debug.Log("구글 계정 로그인" + $"Login {(success ? "Success" : "Failed")}");
            isSuccess = true;
        });
        yield return new WaitUntil(() => isSuccess);
        yield return new WaitForSeconds(0.2f);
    }

    public void RealLogin()
    {
        BackendReturnObject bro = Backend.BMember.GetUserInfo();

        if (bro.GetReturnValuetoJSON()["row"]["nickname"] == null)
        {
            Debug.Log("닉네임생성");

            guestLoginButton.gameObject.SetActive(false);
            appleLoginButton.gameObject.SetActive(false);
            googleLoginButton.gameObject.SetActive(false);

            nicknamePopUp.Show();
        }
        else
        {
            nicknameKey = bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();

            Debug.Log("Real Login");

            if (isLoading)
            {
                return;
            }
            nicknamePopUp.Hide();
            guestLoginButton.gameObject.SetActive(false);
            appleLoginButton.gameObject.SetActive(false);
            googleLoginButton.gameObject.SetActive(false);


            TouchToStartBtn.gameObject.SetActive(false);

            versionTxt.text = Application.version;

            isLoading = true;

            BackendManager.Instance.IsLocal = false;
            
            PlayerPrefs.SetInt("IsFirst", 1);
            PlayerPrefs.Save();

            loading.gameObject.SetActive(true);

            StartCoroutine(LoadingBar.Instance.LoadScene("MainScene"));
        }

        Backend.Notification.Connect(); //  실시간 알림 서버 연결


#if UNITY_ANDROID && !UNITY_EDITOR
              Backend.Android.PutDeviceToken(Backend.Android.GetDeviceToken(), (callback) =>
                {
                    callback = BackendErrorManager.Instance.RetryLogic(() => Backend.Android.PutDeviceToken(Backend.Android.GetDeviceToken()),true,callback);
                    if (callback.IsSuccess())
                    {
                        Debug.Log($"푸시 알림 설정 성공 : {callback}");
                    }
                    else
                    {
                        Debug.Log($"푸시 알림 설정 실패 - statusCode : {callback.GetErrorCode()}, message : {callback.GetMessage()}");
                    }
                });
#endif
    }

    #region 안드로이드 기기의 API 레벨이 33 이상인 경우에만 로컬 푸시 알림을 위한 권한 요청
    public void InitializeAndroidLocalPush()
    {
        // 디바이스의 안드로이드 api level 얻기
        string androidInfo = SystemInfo.operatingSystem;
        int apiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
        // 디바이스의 api level이 33 이상이라면 퍼미션 요청
        if (apiLevel >= 33 && !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
    #endregion
    
    private void Update()
    {
        Backend.AsyncPoll();

        if (authManager != null)
        {
            authManager.Update();
        }
    }
   
    #region 안드로이드 구글 로그인
    public void AOSGoogleLogin()
    {
#if UNITY_ANDROID
        TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(GoogleLoginCallback);
#endif
    }
    #endregion

    #region IOS 구글 로그인
    public void IOSGoogleLogin()
    {
#if UNITY_IOS
        TheBackend.ToolKit.GoogleLogin.iOS.GoogleLogin(GoogleLoginCallback);
#endif
    }
    #endregion

    #region AOS IOS 구글로그인 모두 사용하는 콜백 함수
    public void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
    {
        if (isSuccess == false)
        {
            BackendErrorManager.Instance.SettingPopUp(errorMessage,true);
            Debug.LogError("로그인실패:  " + errorMessage);
            return;
        }

        Backend.BMember.AuthorizeFederation(token, FederationType.Google, callback =>
        {
            callback = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.AuthorizeFederation(token, FederationType.Google),false, callback);
            if (callback.IsSuccess())
            {
#if UNITY_ANDROID
                PlayerPrefs.SetString("LoginType", "AOSGoogle");
                PlayerPrefs.SetString("LoginToken", token);
                PlayerPrefs.Save();
#endif

#if UNITY_IOS
                PlayerPrefs.SetString("LoginType", "IOSGoogle");
                PlayerPrefs.SetString("LoginToken", token);
                PlayerPrefs.Save();
#endif

                RealLogin();
            }
            else
            {
                Debug.Log("구글 로그인 실패 코드  " + callback.GetErrorCode() + "  메세지   " + callback.GetMessage());

            }
        });
    }
    #endregion

    #region AOS 애플 토큰이상할때 재발급해주고 재발급 받은 토큰으로 로그인
    private void RefreshAOSAppleTokenAndLogin()
    {
#if UNITY_ANDROID
        TheBackend.ToolKit.AppleLogin.Android.AppleLogin("com.madsoft.RaisingDemonHunter.Applelogin", out var error, newToken =>
        {
            // 새 토큰으로 로그인 시도
            Backend.BMember.AuthorizeFederation(newToken, FederationType.Apple, callback =>
            {
                callback = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.AuthorizeFederation(newToken, FederationType.Apple), false, callback);
                if (callback.IsSuccess())
                {
                    Debug.Log("새로운 토큰 애플 로그인 성공: " + callback);

                    PlayerPrefs.SetString("LoginToken", newToken);  // 새 토큰 저장
                    PlayerPrefs.Save();
                    RealLogin();
                }
                else
                {
                    Debug.Log("애플 로그인 실패: " + callback);
                }
            });
        });
#endif
    }
    #endregion

    #region 안드로이드 애플 로그인
    public void AOSAppleLogin()
    {
#if  UNITY_ANDROID
        TheBackend.ToolKit.AppleLogin.Android.AppleLogin("com.madsoft.RaisingDemonHunter.Applelogin", out var error, token =>
        {
            Backend.BMember.AuthorizeFederation(token, FederationType.Apple, callback => {

                callback = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.AuthorizeFederation(token, FederationType.Apple),false,callback);
                if (callback.IsSuccess())
                {
                    Debug.Log("AOS Apple Login Success");

                    PlayerPrefs.SetString("LoginType", "AOSApple");
                    PlayerPrefs.SetString("LoginToken", token);
                    PlayerPrefs.Save();

                    RealLogin();
                }
                else
                {
                     Debug.Log("AOS Apple Login Fail:  " + callback.GetErrorCode() + "  메세지   " + callback.GetMessage());
                }
            });
         
        });

        if (string.IsNullOrEmpty(error) == false)
        {
            Debug.Log("에러 : " + error);
        }
#endif
    }
    #endregion

    #region IOS 애플 토큰이상할때 재발급해주고 재발급 받은 토큰으로 로그인
    private IEnumerator RefreshIOSAppleTokenAndLogin()
    {
        bool isSuccess = false;

        if (authManager == null)
            authManager = new AppleAuthManager(new AppleAuth.Native.PayloadDeserializer());

        authManager.LoginWithAppleId(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName,
            success =>
            {
                var appleIdCredential = success as IAppleIDCredential;
                string newToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                Debug.Log($"새로운 Apple 토큰 갱신 성공: {newToken}");

                // 새 토큰으로 로그인 시도
                var bro = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.AuthorizeFederation(newToken, FederationType.Apple, "siwa"));

                if (bro.IsSuccess())
                {
                    Debug.Log("애플 로그인 성공: " + bro);
                    PlayerPrefs.SetString("LoginToken", newToken);  // 새 토큰 저장
                    PlayerPrefs.Save();
                    RealLogin();
                    isSuccess = true;
                }
                else
                {
                    Debug.Log("애플 로그인 실패: " + bro);
                }
            },
            error =>
            {
                Debug.LogError($"Apple 토큰 갱신 실패: {error}");
            });

        yield return new WaitUntil(() => isSuccess);
    }
    #endregion

    #region 구글 로그아웃 
    public void SignOutGoogleLogin()
    {
#if UNITY_ANDROID
        TheBackend.ToolKit.GoogleLogin.Android.GoogleSignOut(GoogleSignOutCallback);
#endif
    }
    public void SingOutIOSGoogleLogin()
    {
#if UNITY_IOS
        TheBackend.ToolKit.GoogleLogin.iOS.GoogleSignOut(GoogleSignOutCallback);
#endif
    }
    private void GoogleSignOutCallback(bool isSuccess, string error)
    {
        if (isSuccess == false)
        {
            Debug.Log("구글 로그아웃 에러 응답 발생 : " + error);
        }
        else
        {
            Debug.Log("로그아웃 성공");
        }
    }
    #endregion

    #region IOS 애플 로그인


    public IEnumerator IOSAppleLogin()
    {
        bool isSuccess = false;
        string ios_token = null;
        if (authManager == null)
            authManager = new AppleAuthManager(new AppleAuth.Native.PayloadDeserializer());

        authManager.LoginWithAppleId(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName,
            success =>
            {
                var appleIdCredential = success as IAppleIDCredential;
                ios_token = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                Debug.Log($"아이폰 Apple Login 성공: {ios_token}");


                PlayerPrefs.SetInt("IsFirst", 1);
                PlayerPrefs.SetString("LoginToken", ios_token);
                PlayerPrefs.SetString("LoginType", "IOSApple");
                PlayerPrefs.Save();

                isSuccess = true;
            },
            error =>
            {
                Debug.LogError($"아이폰 Apple Login 실패: {error}");
                agreementPopUp.Show();
            });

        yield return new WaitUntil(() => isSuccess);

        var bro = BackendErrorManager.Instance.RetryLogic(() => Backend.BMember.AuthorizeFederation(ios_token, FederationType.Apple, "siwa"));

        if (bro.IsSuccess())
        {
            Debug.Log("페데레이션 iOS 로그인 성공: " + bro);
            RealLogin();
        }
        else
        {
            Debug.Log("애플 로그인 실패 코드  " + bro.GetErrorCode() + "  메세지   " + bro.GetMessage());
        }
    }
    #endregion

    #region 게스트 로그인
    public void GuestPopUp()
    {
        guestWarningPopUp.Show();
    }

    /// <summary>
    /// 게스트 로그인
    /// </summary>
    public void GuestLogin()
    {
        BackendReturnObject bro = Backend.BMember.GuestLogin();

        PlayerPrefs.SetString("LoginType", "Guest");
        PlayerPrefs.Save();

        if (bro.IsSuccess())
        {
            RealLogin();
        }
        else
        {
            if (bro.GetMessage().Contains("bad customId"))
            {
                DeleteGuestInfo();
                GuestLogin();
                return;
            }
            BackendErrorManager.Instance.SettingPopUp(bro, true);
            if (bro.GetMessage().Contains("blocked user"))
            {
            }
            else if (bro.GetMessage().Contains("Gone user"))
            {
                Debug.LogError("GoneUser");
            }
        }

    }

    /// <summary>
    /// 게스트 정보 삭제
    /// </summary>
    public void DeleteGuestInfo()
    {
        Debug.Log("-------------DeleteGuestInfo-------------");
        Backend.BMember.DeleteGuestInfo();
    }
#endregion

    public void UpdateNickname(string nickname)
    {
        BackendReturnObject bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            #region 데이터 생성    (신설되는 유저 테이블은 여기에 추가 해야 함) 

            var userParam = new Param();
            Backend.GameData.Insert("User", userParam);

            var inventoryParam = new Param();
            Backend.GameData.Insert("Inventory", inventoryParam);   //추 후 삭제 해야될 수도

            var MailParam = new Param();
            Backend.GameData.Insert("Mail", MailParam);

            var HunterItemParam = new Param();
            Backend.GameData.Insert("HunterItem", HunterItemParam);

            var RankParam = new Param();
            Backend.GameData.Insert("Rank", RankParam);
            #endregion

            RealLogin();
        }
        else
        {
            Game.Debbug.Debbuger.Debug("게스트 로그인 실패: " + bro.GetStatusCode() + " - " + bro.GetMessage());
            if(OnNotice(bro.GetStatusCode()) == false)
                BackendErrorManager.Instance.SettingPopUp(bro, true);
        }
    }

    /// <summary>
    /// 닉네임의 오류 예외처리
    /// </summary>
    /// <returns>게임을 정지 할지 여부</returns>
    public bool OnNotice(string str)
    {
        switch (str)
        {
            case "Filter":
                Game.Debbug.Debbuger.Debug("닉네임에 비속어가 포함되어 있습니다");
                break;
            case "400":
                Game.Debbug.Debbuger.Debug("공백존재 / 8글자 초과 입니다");
                break;
            case "409":
                Game.Debbug.Debbuger.Debug("중복된 닉네임입니다");
                break;
            default:
               Game.Debbug.Debbuger.Debug($"StatusCode  : {str}");
                return false;
        }
        return true;
    }

#region 언어 세팅
    void LanguageSetting()
    {
        //구글로그인

        //애플로그인

        //게스트 로그인
    }
#endregion

#region 로그인 타입에 맞게 버튼 설정
    public void SetOSLoginType()
    {
        agreementPopUp.gameObject.SetActive(false);

#if UNITY_EDITOR
        loginType = LoginType.Guest;
#endif
        switch (loginType)
        {
            case LoginType.Guest:
                googleLoginButton.gameObject.SetActive(false);
                appleLoginButton.gameObject.SetActive(false);
                guestLoginButton.gameObject.SetActive(true);
                break;
            case LoginType.GoogleApple:
                googleLoginButton.gameObject.SetActive(true);
                appleLoginButton.gameObject.SetActive(true);
                guestLoginButton.gameObject.SetActive(false);
                break;
            case LoginType.GoogleAppleGuest:
                googleLoginButton.gameObject.SetActive(true);
                appleLoginButton.gameObject.SetActive(true);
                guestLoginButton.gameObject.SetActive(true);
                break;
        }
    }
#endregion


#region Version Check 관련
    bool CheckUpdate(string version)
    {
        int[] checkVersion = version.Split('.').Select(str => int.Parse(str)).ToArray();
        int[] myVersion = Application.version.Split('.').Select(str => int.Parse(str)).ToArray();

        if (myVersion[0] > checkVersion[0])
            return false;
        else if (myVersion[0] < checkVersion[0])
            return true;
        else
        {
            if (myVersion[1] > checkVersion[1])
                return false;
            else if (myVersion[1] < checkVersion[1])
                return true;
            else
            {
                if (myVersion[2] >= checkVersion[2])
                    return false;
                else
                    return true;
            }
        }
    }
    IEnumerator CheckVersion()
    {
        BackendReturnObject bro = BackendErrorManager.Instance.RetryLogic(() => Backend.Utils.GetLatestVersion());

        if (bro.IsSuccess())
        {
            string version = bro.GetReturnValuetoJSON()["version"].ToString();

            if (version == Application.version)
                yield break;
            if (!CheckUpdate(version))
                yield break;

            bool forceUpdate = false;
            string updatetype = bro.GetReturnValuetoJSON()["type"].ToString();

            if (updatetype == "1")
            {
                //  선택 업데이트
                Debug.Log("업데이트를 하시겠습니까? y/n");
                //commonErrorPopUp.Setting("Message_VersionUpdate", true);
            }
            else if (updatetype == "2")
            {
                //  강제 업데이트
                forceUpdate = true;
                Debug.Log("강제업데이트1");
                BackendErrorManager.Instance.SettingPopUp("Message_UpdateVersion", true);
            }
            else if (updatetype == null)
            {
                //  
                Debug.Log("강제업데이트2");
                forceUpdate = true;
                BackendErrorManager.Instance.SettingPopUp("Message_UpdateVersion", true);
            }

            yield return new WaitUntil(() => (!forceUpdate));
        }
        else
        {
            Debug.LogError($"StatusCode : {bro.GetStatusCode()}\nErrorCode : {bro.GetErrorCode()}\nMessage : {bro.GetMessage()}");
        }
    }
#endregion

}