using BackEnd;
using DuloGames.UI;
using LitJson;
using NSY;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 뒤끝 함수 호출 최소화를 위한 캐시 데이터
/// </summary>
[Serializable]
public class CacheData
{
    public List<RankingCellData> rankingData;
}

/// <summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 랭킹 팝업 관리
/// </summary>
public class RankingPopUp : MonoBehaviour, IPopUp
{
    [SerializeField] private RankingScrollController scroller = null;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI titleText = null;
    [SerializeField] private TextMeshProUGUI myRank = null;

    [SerializeField] private TextMeshProUGUI rankText = null;
    [SerializeField] private TextMeshProUGUI percentText = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private TextMeshProUGUI idText = null;
    [SerializeField] private TextMeshProUGUI rankingUpdateRateInfoText = null;

    public Button[] Categorybtnarray; //카테고리 버튼
    public Button[] SubCategorybtnarray; //서브 카테고리 버튼
    public TextMeshProUGUI[] Categorytextarray; //카테고리 버튼 이름
    public TextMeshProUGUI[] SubCategorytextarray; //서브 카테고리 버튼 이름

    [Space(20)]
    [SerializeField] private TextMeshProUGUI debugCacheTimeText = null;
    public TextMeshProUGUI DebugCacheTimeText => debugCacheTimeText;

    string myRankFormat = "";
    string rankUuid;

    private Dictionary<Utill_Enum.RankType, List<RankingCellData>> caches = new Dictionary<Utill_Enum.RankType, List<RankingCellData>>();
    private Dictionary<Utill_Enum.RankType, DateTime> lastUpdateTimes = new Dictionary<Utill_Enum.RankType, DateTime>();
    Utill_Enum.RankType rankType = Utill_Enum.RankType.BPArcher1;

    public int CurrentCategoryindex = -1;
    public int CurrentSubCategoryindex = -1;

    private int ClickCount = 0;
    private bool isInit = false;

    private void Start()
    {
        Initialize();
        scroller.onScrollEnd += () =>
        {
            InitializeScroller(rankType, -1, false);
        };
        scroller.maxStoreCount = RankManager.Instance.MaxStoreCount;
        ActiveHunterPortraits();
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            // caches 딕셔너리 초기화
            PlayerPrefs.DeleteAll();
            caches = new Dictionary<Utill_Enum.RankType, List<RankingCellData>>();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var item in caches)
            {
                Debug.Log("호출 222  " + item.Key);

            }
        }
#endif
    }
    public void Show()
    {

        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.SetActive(true);
            //  랭킹 UI가 팝업될 때 랭킹 정보 갱신
            if (!BackendManager.Instance.IsLocal)
                RankManager.Instance.UpdateRankData();
            caches.Clear();
            InitializeScroller(Utill_Enum.RankType.BPArcher1);
        }
        else
            Hide();
    }
    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove();
        gameObject.SetActive(false);
    }
    public void Close()
    {
    }


    public void Initialize()
    {
        LanguageSetting();
        OnClickEventSetting();
    }

    public void LanguageSetting()
    {
        titleText.text = LocalizationTable.Localization("Title_Ranking");
        rankText.text = LocalizationTable.Localization("Ranking");
        percentText.text = LocalizationTable.Localization("Rate");
        levelText.text = LocalizationTable.Localization("Level");
        idText.text = LocalizationTable.Localization("NickName");
        rankingUpdateRateInfoText.text = LocalizationTable.Localization("RankingUpdateRateInfo");

        Categorytextarray[0].text = LocalizationTable.Localization("BattlePower");
        Categorytextarray[1].text = LocalizationTable.Localization("Stage");

        SubCategorytextarray[0].text = LocalizationTable.Localization("Archer");
        SubCategorytextarray[1].text = LocalizationTable.Localization("Guardian");
        SubCategorytextarray[2].text = LocalizationTable.Localization("Mage");
    }

    public void SetDebugCacheTime()
    {
        if (BackendManager.Instance.IsLocal) return;

        DateTime now = DateTime.UtcNow;

        // 캐시된 데이터의 업데이트 시간 확인
        DateTime lastUpdateTime = GetLastUpdateTime(rankType);
        TimeSpan timeSpan = (now - lastUpdateTime);
        DebugCacheTimeText.text = Utility.TimeFormatDefault(timeSpan);
    }

    private void OnClickEventSetting()
    {
        // 세부 카테고리 버튼 이벤트 할당
        for (int i = 0; i < Categorybtnarray.Length; i++)
        {
            int index = i; // 반복문 변수의 값을 캡처하기 위해 변수를 생성합니다.
            Categorybtnarray[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayAudio("UIClick");
                ActiveCategory(index);
            });
        }
        for (int i = 0; i < SubCategorybtnarray.Length; i++)
        {
            int index = i; // 반복문 변수의 값을 캡처하기 위해 변수를 생성합니다.
            SubCategorybtnarray[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayAudio("UIClick");
                ActiveSubCategory(index);
            });
        }
        ActiveCategory(0);
        ActiveSubCategory(0);
    }

    public void ActiveCategory(int i)
    {
        //클릭한 인덱스 열기
        switch (i)
        {
            case 0:
                CurrentCategoryindex = 0;
                SubCategorybtnarray[0].transform.parent.gameObject.SetActive(true);
                ActiveSubCategory(0);
                break;
            case 1:
                CurrentCategoryindex = 1;
                SubCategorybtnarray[0].transform.parent.gameObject.SetActive(false);
                InitializeScroller(Utill_Enum.RankType.ClearStageLevel1);
                break;
        }
        //인덱스 해당하는 팝업켜지게..
        ActiveButton(CurrentCategoryindex, Categorybtnarray, Categorytextarray);
    }
    public void ActiveSubCategory(int i)
    { //클릭한 인덱스 열기
        switch (i)
        {
            case 0:
                CurrentSubCategoryindex = 0;
                InitializeScroller(Utill_Enum.RankType.BPArcher1);
                break;
            case 1:
                CurrentSubCategoryindex = 1;
                InitializeScroller(Utill_Enum.RankType.BPGuardian1);
                break;
            case 2:
                CurrentSubCategoryindex = 2;
                InitializeScroller(Utill_Enum.RankType.BPMage1);
                break;
        }
        //인덱스 해당하는 팝업켜지게..
        ActiveButton(CurrentSubCategoryindex, SubCategorybtnarray, SubCategorytextarray);
    }

    public void ActiveButton(int index, Button[] categorybtnarray, TextMeshProUGUI[] categorytextarray)
    {
        if (index == -1)
        {
            index = 0;
        }

        for (int i = 0; i < categorybtnarray.Length; i++)
        {
            if (i == index)
            {
                //글씨색바꾸기
                categorytextarray[i].color = Color.white;
                //나머지 밑의 오브젝트들도 끄기 
                categorybtnarray[i].transform.GetChild(0).gameObject.SetActive(true);

            }
            else
            {
                //글씨색 바꾸기
                categorytextarray[i].color = Color.gray;
                //나머지 밑의 오브젝트들도 끄기 
                categorybtnarray[i].transform.GetChild(0).gameObject.SetActive(false);

            }
        }
    }

    private void InitializeScroller(Utill_Enum.RankType _rankType, int _loadRank = -1, bool _gotoTop = true)
    {
        int count = 0;
        LoadCaches();
        if (_loadRank == -1)
            _loadRank = 100;
        if (BackendManager.Instance.IsLocal)
        {
            Debug.LogError("뒤끝 서버와 연결되어 있지 않습니다.");
            return;
        }

        rankUuid = RankManager.RankUuids[_rankType];

        switch (_rankType) //랭크 타입에 따라 스코어 텍스트 변경
        {
            case Utill_Enum.RankType.ClearStageLevel1:
                scoreText.text = LocalizationTable.Localization("Stage");
                break;
            case RankType.BPArcher1:
            case RankType.BPGuardian1:
            case RankType.BPMage1:
                scoreText.text = LocalizationTable.Localization("BattlePower");
                break;
        }

        // UTCTimeManager를 사용하여 현재 시간 가져오기
        DateTime now = DateTime.UtcNow;

        // 캐시된 데이터의 업데이트 시간 확인
        DateTime lastUpdateTime = GetLastUpdateTime(_rankType);
        bool isCacheExpired = (now - lastUpdateTime).TotalMinutes > 30; // 10분마다 새로고침
        //Debug.Log("호출:  " + isCacheExpired + "  전 시간:  " + lastUpdateTime + "  현재 시간:  " + now);
        if (!caches.ContainsKey(_rankType) || isCacheExpired)
        {
            Debug.Log("호출할게 없어서 다시 로드");

            scroller.scroller.ScrollPosition = 0;
            var list = new List<RankingCellData>();

            Backend.URank.User.GetRankList(rankUuid, 50, callback =>
            {
                if (callback.IsSuccess())
                {
                    var rows = callback.FlattenRows();
                    float totalCount = float.Parse(callback.GetFlattenJSON()["totalCount"].ToString());
                    for (var i = 0; i < rows.Count; ++i)
                    {
                        int _rank = int.Parse(rows[i]["rank"].ToString());
                        long value = 0;
                        if (rows[i]["score"].ToString().Contains('.'))
                        {
                            double floatValue = double.Parse(rows[i]["score"].ToString());
                            value = (long)floatValue;
                        }
                        else
                        {
                            value = long.Parse(rows[i]["score"].ToString());
                        }
                        int extraValue = 0;
                        switch (_rankType)
                        {
                            case RankType.BPArcher1:
                                extraValue = int.Parse(rows[i]["ArcherLevel"].ToString());
                                break;
                            case RankType.BPGuardian1:
                                extraValue = int.Parse(rows[i]["GuardianLevel"].ToString());
                                break;
                            case RankType.BPMage1:
                                extraValue = int.Parse(rows[i]["MageLevel"].ToString());
                                break;
                        }
                        list.Add(new RankingCellData()
                        {
                            rankType = _rankType,
                            rank = _rank,
                            percent = _rank / totalCount,
                            rankValue = value,
                            extraValue = extraValue,
                            id = rows[i]["nickname"].ToString(),
                            country = string.Empty
                        });
                    }
                    if (list.Count < 10) //랭크에 상위 10명의 데이터가 없을 경우
                    {
                        for (int i = list.Count; i < 10; i++)
                            list.Add(new RankingCellData() { rank = i + 1 });
                    }
                    list.Sort();

                }

                Backend.URank.User.GetRankList(rankUuid, 50, 50, callback =>
                {
                    if (callback.IsSuccess())
                    {
                        var rows = callback.FlattenRows();
                        float totalCount = float.Parse(callback.GetFlattenJSON()["totalCount"].ToString());
                        for (var i = 0; i < rows.Count; ++i)
                        {
                            int _rank = int.Parse(rows[i]["rank"].ToString());
                            long value = 0;
                            if (rows[i]["score"].ToString().Contains('.'))
                            {
                                double floatValue = double.Parse(rows[i]["score"].ToString());
                                value = (long)floatValue;
                            }
                            else
                            {
                                value = long.Parse(rows[i]["score"].ToString());
                            }
                            int extraValue = 0;
                            switch (_rankType)
                            {
                                case RankType.BPArcher1:
                                    extraValue = int.Parse(rows[i]["ArcherLevel"].ToString());
                                    break;
                                case RankType.BPGuardian1:
                                    extraValue = int.Parse(rows[i]["GuardianLevel"].ToString());
                                    break;
                                case RankType.BPMage1:
                                    extraValue = int.Parse(rows[i]["MageLevel"].ToString());
                                    break;
                            }
                            list.Add(new RankingCellData()
                            {
                                rankType = _rankType,
                                rank = _rank,
                                percent = _rank / totalCount,
                                rankValue = value,
                                extraValue = extraValue,
                                id = rows[i]["nickname"].ToString(),
                                country = string.Empty
                            });
                        }
                        if (list.Count < 10) //랭크에 상위 10명의 데이터가 없을 경우
                        {
                            for (int i = list.Count; i < 10; i++)
                                list.Add(new RankingCellData() { rank = i + 1 });
                        }
                        list.Sort();

                        caches[_rankType] = list;
                        rankType = _rankType;

                        if (caches.Count > 0)
                        {
                            scroller.SetData(caches[rankType]);
                            SetMyRank(_rankType);
                        }
                        scroller.scroller.ScrollPosition = 0;
                        // 캐시 데이터를 저장합니다.
                        SaveCaches();
                    }
                });
            });


            // 캐시 업데이트 시간을 현재 시간으로 설정
            SetLastUpdateTime(_rankType, now);
        }

        else
        {
            Debug.Log("호출 캐시된 데이터 사용");
            if (caches.ContainsKey(_rankType) || caches[_rankType] == null)
            {
                var list = caches[_rankType];

                Debug.Log("호출 캐시된 데이터 개수: " + list.Count); // 캐시된 데이터 개수를 확인합니다.
                scroller.SetData(list);
                SetMyRank(_rankType);
            }
            else
            {
                Debug.Log($"호출 caches에 {_rankType}에 해당하는 데이터가 없거나 null입니다.");
            }

            if (_gotoTop)
                scroller.scroller.ScrollPosition = 0;
            else
                scroller.scroller.ScrollPosition = scroller.scroller.ScrollPosition;
        }

    }
    private void SetLastUpdateTime(Utill_Enum.RankType rankType, DateTime updateTime)
    {
        // 캐시 업데이트 시간을 저장합니다.
        string key = $"{rankType}_UpdateTime";
        PlayerPrefs.SetString(key, updateTime.ToString());
        PlayerPrefs.Save();
    }

    private DateTime GetLastUpdateTime(Utill_Enum.RankType rankType)
    {
        // 저장된 캐시 업데이트 시간을 가져옵니다.
        string key = $"{rankType}_UpdateTime";
        if (PlayerPrefs.HasKey(key))
        {
            string updateTimeString = PlayerPrefs.GetString(key);
            return DateTime.Parse(updateTimeString);
        }
        else
        {
            // 기본값은 현재 시간으로 설정합니다.
            return DateTime.Now;
        }
    }
    private void SaveCaches()
    {
        foreach (var kvp in caches)
        {
            string key = $"{kvp.Key}";
            CacheData cacheData = new CacheData { rankingData = kvp.Value };
            string json = JsonUtility.ToJson(cacheData);
            PlayerPrefs.SetString(key, json);
        }
        PlayerPrefs.Save();
    }

    private void LoadCaches()
    {
        foreach (Utill_Enum.RankType rankType in Enum.GetValues(typeof(Utill_Enum.RankType)))
        {
            string key = $"{rankType}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                CacheData cacheData = JsonUtility.FromJson<CacheData>(json);
                caches[rankType] = cacheData.rankingData;
            }
        }
    }

    void SetMyRank(Utill_Enum.RankType _rankType)
    {
        rankType = _rankType;
        RankManager.Instance.FindMyRankInfo(_rankType);
        var data = RankManager.Instance.MyRankData[_rankType];
        if (myRankFormat == "")
            myRankFormat = LocalizationTable.Localization("MyRankFormat");

        if (data == null)
            myRank.text = string.Format(myRankFormat, Backend.UserNickName, "-", "-", "-");
        else
        {
            int rank = int.Parse(data["rows"][0]["rank"].ToString());

            long score = 0;
            if (data["rows"][0]["score"].ToString().Contains('.'))
            {
                float floatValue = float.Parse(data["rows"][0]["score"].ToString());

                score = (long)floatValue;
            }
            else
            {
                score = long.Parse(data["rows"][0]["score"].ToString());
            }
            float percent = rank / float.Parse(data["totalCount"].ToString());

            if (score > 100000)
                myRank.text = string.Format(myRankFormat, Backend.UserNickName, Utill_Math.FormatCurrency(rank) , percent, Utill_Math.AbbreviateNumber(long.Parse(Utill_Math.FormatCurrency((int)score))));
            else
                myRank.text = string.Format(myRankFormat, Backend.UserNickName, Utill_Math.FormatCurrency(rank) , percent, Utill_Math.FormatCurrency((int)score));
        }
    }

    private void ActiveHunterPortraits() //Contrain에 값에따라서 탭 매뉴 활성화
    {
        int value = HunterPortraitSystem.Instance.GetHunterDeployment();
        for (int i = 0; i < SubCategorybtnarray.Length; i++)
        {
            if (i < value)
            {
                SubCategorybtnarray[i].gameObject.SetActive(true);
            }
            else
            {
                SubCategorybtnarray[i].gameObject.SetActive(false);
            }
        }
    }
}
