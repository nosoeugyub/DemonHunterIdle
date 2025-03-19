using BackEnd;
using BackEnd.Util;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.Collections;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System.Threading.Tasks;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 게임자체를 관리하는 중계자
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
    [Header("일일 요일 스킬 갯수")]
    public int DailySkillCount;

    [Header("게임 저장 시간 주기")]
    public int SaveForTime;

    [Header("초기화 보정 시간 (h)")]
    public  int dailyResetDateTime = 9;

    [Header("일일 초기화 치트 시간 (s)")]
    public int CheatdailyResetDateTime = 5;

    [Header("절전모드 돌입 시간 (s)")]
    public float EnterSleepModeTime = 30f;
 	[Header("오프라인 보상 최소 적용 시간 (m)")]
    public int OfflineMinRewardTime = 30;

    [Header("오프라인 보상 최대 적용 시간 (m)")]
    public int OfflineMaxRewardTime = 60;

    [Header("오프라인 보상 골드계수")]
    public int OfflineGoldCoefficient = 1;

    [Header("오프라인 보상 경험치 계수")]
    public int OfflineExpCoefficient = 1;

    [Header("오프라인 보상 다이아 계수")]
    public int OfflineDiaCoefficient = 1;

    [Header("방치모드 피로도 시간 (s)")]
    public int IdleModeRestCycle = 36000;

    [Header("방치모드 휴식 시간 (s)")]
    public int IdleModeRestTime = 600;

    [Header("사망시 피로도 패널티 시간 (s)")]
    public int IdleModeHunterDeathPenalty = 60;
    [Header("장비 소환시 보여질 수 있는 최대 등급")]
    public int GachaMergeMax = 8;

    [Header("에디터 유저인지")]
    public bool IsEditorUser = false;

    [Header("페이드 인/아웃 시 그래프 타입")]
    [SerializeField]
    Ease fadeInOutEaseType;
    public float fadeDuration = 1f;
    public Image fadeImage;


    [Header("헌터 사이의 간격")]
    public float hunterDistance = 1f;


    [Header("헌터 사망시 전체 쿨다운 시간 (s)")]
    public float hunterDieCoolDownDuration = 5f;

    [Header("헌터 사망시 재생성 쿨다운 시간 (s)")]
    public float hunterReviveDuration = 3f;

    [Header("헌터 이동 범위 제한시 하단 추가 제한값")]
    public float hunterLimitMinZ = 3f;

    [Header("헌터 이동 범위 제한시 상단 추가 제한값")]
    public float hunterLimitMaxZ = 3f;

    [Header("빙결시 적용될 메테리얼")]
    [SerializeField]
    public Material freezingMaterial = null;

    [Header("헌터 전투력 체력 계수")]
    public float HunterHpcoefficient = 1f;

    [Header("헌터 전투력 마나 계수")]
    public float HunterMpcoefficient = 1f;

    [Header("헌터 전투력 마법공격력 계수")]
    public float HunterMagicPowercoefficient = 1f;

    [Header("헌터 전투력 물리방어력 계수")]
    public float HunterPhycisDefcoefficient = 1f;

    [Header("헌터 전투력 마법방어력 계수")]
    public float HunterMagicDefcoefficient = 1f;





    public ObjectPooler ObjectPooler;
    public BossController BossController;

    private int gamesquencecount = 0;
    private int maxgamesquencecount = 5;

    public Camera MainCam;
    public Camera UiCam;

   
   public Canvas ErrorCanvas;
    public RectTransform ErrorRect;
    

    private bool isForceQuit = false;

    public TextMeshProUGUI savefortimetxt;
    public TextMeshProUGUI Gold;
    public TextMeshProUGUI Exp;


    #region 세팅 최적화 관련

    private bool isHpOn = true;
    public bool IsHpOn
    {
        get { return isHpOn; }
        set
        {
            if (isHpOn != value)
            {
                isHpOn = value;
                OnHpOnChanged?.Invoke(isHpOn);
            }
        }
    }

   
    public event Action<bool> OnHpOnChanged;
    public void HpOnChangedCall(bool isHpOn) => OnHpOnChanged?.Invoke(isHpOn);

    private bool isDamageOn = true;
    public bool IsDamageOn
    {
        get { return isDamageOn; }
        set { isDamageOn = value; }
    }

    private bool isShadowOn = true;
    public bool IsShadowOn
    {
        get { return isShadowOn; }
        set { isShadowOn = value; }
    }

    private bool isGrapicOn = true;
    public bool IsGrapicOn
    {
        get { return isGrapicOn; }
        set { isGrapicOn = value; }
    }
    #endregion


    #region 상태 매시지
    public TextMeshProUGUI GameStatusMsgtxt;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        // GameStateMachine 인스턴스를 명시적으로 초기화합니다.
        StreamingReader.PathInit();
        BackendManager.Instance.OwnerIndate = BackEnd.Backend.UserInDate;
        GameEventSystem.GameBossSqeuce_SendGameEventHandler += SetScnenmatic;
        GameEventSystem.GameRestModeSequence_SendGameEventHandler += SetScnenmatic;
        GameEventSystem.GameHunterDie_SendGameEventHandler += HunterDie;

        

        

        //IAP초기화
        UnityServices.InitializeAsync();
        StartCoroutine(AutoSaveRoutine());
    }


    // This routine will save the game at intervals without stopping the game
    private IEnumerator AutoSaveRoutine()
    {
        int timeer = SaveForTime;
        while (true)
        {
            yield return new WaitForSeconds(1); // Wait for the interval duration

            savefortimetxt.text = "";
#if UNITY_EDITOR
            savefortimetxt.text = "서버 저장 시간" + SaveForTime.ToString();
#endif
            SaveForTime--;
            if (SaveForTime <= 0)
            {
                StreamingReader.SaveAllData();
                SaveForTime = timeer;
            }
        }
    }



    private async Task InitializeUnityServices()
    {
        var options = new InitializationOptions()
            .SetEnvironmentName("production");

        try
        {
            await UnityServices.InitializeAsync(options);
            Debug.Log("Unity Services Initialized");
        }
        catch (Exception e)
        {
            Debug.LogError($"Initialization Failed: {e.Message}");
        }
    }
    private void HunterDie()
    {
        //카메라 흔들리는 연출
        //CameraShake(0.2f, 0.5f,30);

        //FadeOutAndSummonBoss();
        //StopCoroutine(FadeOutAndSummonBoss());
        //StartCoroutine(FadeOutAndSummonBoss());
    }

    private void SetScnenmatic(Utill_Enum.BossSqeunce bossSqenunce)
    {
        switch (bossSqenunce)
        {
            case Utill_Enum.BossSqeunce.Cinemtic:
                //화살 끄기
                Utill_Standard.DisableObjectsWithTag("Arrow");
                //페이드인/아웃
                for(int i = 0; i<DataManager.Instance.Hunters.Count; i++)
                {
                    StopCoroutine(DataManager.Instance.Hunters[i].WaitHunter(fadeDuration + 1f));
                    StartCoroutine(DataManager.Instance.Hunters[i].WaitHunter(fadeDuration + 1f));
                }
                //페이드인/아웃
                FadeOut();
                //StopCoroutine(FadeOutAndSummonBoss());
                //StartCoroutine(FadeOutAndSummonBoss());
                break;

            case Utill_Enum.BossSqeunce.Die:
                for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                {
                    StopCoroutine(DataManager.Instance.Hunters[i].WaitHunter(fadeDuration + 1f));
                    StartCoroutine(DataManager.Instance.Hunters[i].WaitHunter(fadeDuration + 1f));
                }
                //페이드인/아웃
                FadeOut();
                //StopCoroutine(FadeOutAndSummonBoss());
                //StartCoroutine(FadeOutAndSummonBoss());
                break;
        }
    }

    private void SetScnenmatic(IdleModeRestCycleSequence sqenunce)
    {
        switch (sqenunce)
        {
            case IdleModeRestCycleSequence.End:
                FadeOut();
                break;
        }
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        if (!isForceQuit)
            StreamingReader.SaveAllData();
    }
    private void Start()
    {
        StartGameSquence();

        if (!BackendManager.Instance.IsLocal)
            StartCoroutine(RefreshToken());

        if (BackendManager.Instance.IsLocal)
        {
            UIManager.Instance.settingPopUp.SoundSetting();
        }

     
        // UIManager.Instance.mailPopUp.PostListGet(PostType.Admin, true);
    }

    private void Update()
    {
        GameStateMachine.Instance.Update();
    }

    public void ForceQuit() //unity event로 외부에서 호출
    {
        isForceQuit = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResetGameSquence()
    {
        gamesquencecount = 0;
        StartGameSquence();
    }

    public void StartGameSquence()
    {
        while (gamesquencecount < maxgamesquencecount)
        {
            if (gamesquencecount == 0)  //데이터로드.. 로컬인지 서버인지 부터 시작
            {
                GameEventSystem.GameSequence_GameEventHandler_Event(Utill_Enum.Enum_GameSequence.DataLoad);
                gamesquencecount += 1;
            }
            else if (gamesquencecount == 1) //동적으로 미리생성해놓을 오브젝트는 여기서 생성
            {
                GameEventSystem.GameSequence_GameEventHandler_Event(Utill_Enum.Enum_GameSequence.CreateAndInit);
                gamesquencecount += 1;
            }
            else if (gamesquencecount == 2) //게임시작 ui셋팅 및 서버에서 연동해야할것들 불러오기 
            {
                GameEventSystem.GameSequence_GameEventHandler_Event(Utill_Enum.Enum_GameSequence.Start);
                gamesquencecount += 1;
            }
            else if (gamesquencecount == 3) // 튜토리얼이 있으면 튜토리얼 진행 
            {
                GameEventSystem.GameSequence_GameEventHandler_Event(Utill_Enum.Enum_GameSequence.Tutorial);
                gamesquencecount += 1;
            }
            else if (gamesquencecount == 4) //인게임 시작
            {
                GameEventSystem.GameSequence_GameEventHandler_Event(Utill_Enum.Enum_GameSequence.InGame);
                GameStateMachine.Instance.ChangeState(new NonCombatState()); // 초기 상태를 비전투 상태로 설정
                SoundManager.Instance.PlayAudioLoop("BGM_Battle_01");

                //로컬시간 타이머
                ServerController.Instance.StartLocalTimer();
                //로컬에서 시작했을때도 오프라인보상이 먼저 나오게끔
                OfflineRewardPopupSystem.Instance.Setting_OfflinePopup();
                gamesquencecount += 1;
            }
        }
    }


    IEnumerator RefreshToken()
    {
        WaitForSeconds waitForRefreshTokenCycle = new WaitForSeconds(60 * 60 * 8); // 60초 x 60분 x 8시간
        WaitForSeconds waitForRetryCycle = new WaitForSeconds(15f);

        // 첫 호출시에는 리프레시 토큰하지 않도록 8시간을 기다리게 해준다.
        bool isStart = false;

        if (!isStart)
        {
            isStart = true;
            yield return waitForRefreshTokenCycle; // 8시간 기다린 후 반복문 시작
        }
        BackendReturnObject bro = null;
        bool isRefreshSuccess = false;

        // 이후부터는 반복문을 돌면서 8시간마다 최대 3번의 리프레시 토큰을 수행하게 된다.
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                bro = Backend.BMember.RefreshTheBackendToken();
                Debug.Log("리프레시 토큰 성공 여부 : " + bro);

                if (bro.IsSuccess())
                {
                    Debug.Log("토큰 재발급 완료");
                    isRefreshSuccess = true;
                    break;
                }
                else
                {
                    if (bro.GetMessage().Contains("bad refreshToken"))
                    {
                        Debug.LogError("중복 로그인 발생");
                        isRefreshSuccess = false;
                        break;
                    }
                    else
                        Debug.LogWarning("15초 뒤에 토큰 재발급 다시 시도");
                }
                yield return waitForRetryCycle; // 15초 뒤에 다시시도
            }
            // 3번 이상 재시도시 에러가 발생할 경우, 리프레시 토큰 에러 외에도 네트워크 불안정등의 이유로 이후에도 로그인에 실패할 가능성이 높습니다. 
            // 유저들이 스스로 토큰 리프레시를 할수 있도록 구현해주시거나 수동 로그인을 하도록 구현해주시기 바랍니다.
            if (bro == null)
            {
                Debug.LogError("토큰 재발급에 문제가 발생했습니다. 수동 로그인을 시도해주세요");
                //토큰 재발급 실패 다시 로그인 팝업 
            }
            if (!bro.IsSuccess())
            {
                Debug.LogError("토큰 재발급에 문제가 발생하였습니다 : " + bro);
                BackendErrorManager.Instance.SettingPopUp(bro, true);
                //토큰 재발급 문제 발생 수동 로그인 시도 팝업 
            }
            // 성공할 경우 값 초기화 후 8시간을 wait합니다.
            if (isRefreshSuccess)
            {
                Debug.Log("8시간 토큰 재 호출");
                isRefreshSuccess = false;
                yield return waitForRefreshTokenCycle;
            }
        }
    }

    /// <summary>
    /// 나중에 연출 전용 매니저를 따로 파는 것도 괜찮을 듯?
    /// </summary>
    void FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOKill();

        //카메라 흔들림
        //알파값 0으로
        Color startColor = fadeImage.color;
        startColor.a = 0;

        fadeImage.color = startColor;
        fadeImage.DOFade(1, fadeDuration).SetEase(fadeInOutEaseType).OnComplete(() =>
        {
            FadeIn(); 
        });

        //이전 코드
        //float timer = 0f;
        //Color originalColor = Color.white;

        //while (timer < fadeDuration)
        //{
        //    timer += Time.deltaTime;
        //    fadeImage.color = Color.Lerp(Color.clear, originalColor, timer / fadeDuration);
        //    yield return null;
        //}

        //// Fade in
        //StartCoroutine(FadeIn());
    }

    void FadeIn()
    {
        fadeImage.DOKill();
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(0, fadeDuration).SetEase(fadeInOutEaseType).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false); 
        });

        //이전 코드
        //float timer = 0f;
        //Color originalColor = fadeImage.color;

        //while (timer < fadeDuration)
        //{
        //    timer += Time.deltaTime;
        //    fadeImage.color = Color.Lerp(originalColor, Color.clear, timer / fadeDuration);
        //    yield return null;
        //}
        //fadeImage.gameObject.SetActive(false);
    }
    /// <summary>
    /// 카메라 흔들리는 연출
    /// </summary>
    /// <param name="duraction">흔들리는 시간</param>
    /// <param name="strength">흔들리는 강도</param>
    /// <param name="vibrato">흔들림의 정도</param>
    void CameraShake(float duraction,float strength,int vibrato)
    {
        MainCam.DOKill();
        DataManager.Instance.MainCameraMovement.CanMove = false;

        MainCam.DOShakePosition(duraction, strength: strength,vibrato: vibrato).OnComplete(() => { 
            DataManager.Instance.MainCameraMovement.CanMove = true;
        });
    }

    public void SetStatusMsg(string msg)
    {
        GameStatusMsgtxt.text = msg;
    }

    public void ActiveStatusMsg(bool isactive)
    {
        GameStatusMsgtxt.gameObject.SetActive(isactive);
    }

    public void SendRewards(ShopGoldCellData data, string itemnumber)
    {
        //DateTime inDate = UTCTimeManager.Instance.Now();
        PostCellData mailCellDatas = new PostCellData();

        PostCellData postdata = new PostCellData();
        postdata.itemName = itemnumber;
        postdata.itemCount = data.totalamount;
        if (GameDataTable.Instance.Mail == null)
        {
            GameDataTable.Instance.Mail = new Mail();
        }
        //유저의 로컬데이터로 데이터 저장
        Mail.Add_Shop1Data(GameDataTable.Instance.Mail.Shop1, postdata.itemName, postdata.itemCount);
    }
    public void SendRewards(string rewardName,int rewardCount, AchievementType achievementType)
    {
        //DateTime inDate = UTCTimeManager.Instance.Now();
        PostCellData mailCellDatas = new PostCellData();

        PostCellData postdata = new PostCellData();
        postdata.itemName = rewardName;
        postdata.itemCount = rewardCount;
        if (GameDataTable.Instance.Mail == null)
        {
            GameDataTable.Instance.Mail = new Mail();
        }
        //유저의 로컬데이터로 데이터 저장
        if(achievementType == AchievementType.Purchase)
            Mail.Add_PurchaseAchievementData(GameDataTable.Instance.Mail.PurchaseAchievement, postdata.itemName, postdata.itemCount);
        else if (achievementType == AchievementType.Gacha)
            Mail.Add_GachaAchievementData(GameDataTable.Instance.Mail.GachaAchievement, postdata.itemName, postdata.itemCount);
    }
    public void SendRewards(DailyMissionCellData cellData)
    {
        //DateTime inDate = UTCTimeManager.Instance.Now();
        PostCellData mailCellDatas = new PostCellData();

        PostCellData postdata = new PostCellData();
        postdata.itemName = $"{cellData.stageLevel}_{(int)cellData.missionType}"; //기존 csv로는 고유 값을 얻을 수 없어 최대 스테이지 + 미션 타입을 키값으로 사용함
        postdata.itemCount = cellData.rewardCount;
        if (GameDataTable.Instance.Mail == null)
        {
            GameDataTable.Instance.Mail = new Mail();
        }
        //유저의 로컬데이터로 데이터 저장
        Mail.Add_DailyMissionData(GameDataTable.Instance.Mail.DailyMission, postdata.itemName, postdata.itemCount);
    }



    public void SendRewards(OfflineRewardData cellData)
    {
        //DateTime inDate = UTCTimeManager.Instance.Now();
        PostCellData mailCellDatas = new PostCellData();

        PostCellData postdata = new PostCellData();
        postdata.itemName = cellData.ResourceType.ToString(); //기존 csv로는 고유 값을 얻을 수 없어 최대 스테이지 + 미션 타입을 키값으로 사용함
        postdata.itemCount = cellData.Count;
        if (GameDataTable.Instance.Mail == null)
        {
            GameDataTable.Instance.Mail = new Mail();
        }
        //유저의 로컬데이터로 데이터 저장
        Mail.Add_OfflineReawerdData(GameDataTable.Instance.Mail.OfflineReward, postdata.itemName, postdata.itemCount);
    }

    //일일 처리 로직
    public void DailyQuest_Init()
    {
       // CheatdailyResetDateTime = DateTime.Now;//현재 로컬시간으로 갱신
        dailyResetDateTime = CheatdailyResetDateTime + dailyResetDateTime; // 현재시간 + CheatdailyResetDateTime 변수 (+ 초)
    }


    public void SetGold(int gold)
    {

        Gold.text = "";
#if UNITY_EDITOR
        Gold.text = "미접속 평균재화 " +  gold.ToString();
#endif
    }


    public void SetExp(int exp)
    {

        Exp.text = "";
#if UNITY_EDITOR
        Exp.text = "미접속 평균재화 "  + exp.ToString();
#endif
    }
}
