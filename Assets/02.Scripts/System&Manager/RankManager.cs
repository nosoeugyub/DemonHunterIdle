using BackEnd;
using LitJson;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-10
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 랭크의 저장/불러오기 + 랭크 관련 데이터 접근 관리
/// </summary>
public class RankManager : MonoSingleton<RankManager>
{
    [SerializeField]
    private int maxStoreCount = 100; //최대 랭킹 등수
    [SerializeField]
    private int loadCount = 100; //랭킹 로드 갯수

    public int MaxStoreCount => maxStoreCount;
    public int LoadCount => loadCount;

    public static Dictionary<RankType, string> RankUuids; //랭킹 타입에 따른 uuid를 저장한 딕셔너리

    const string tableName = "Rank"; //랭킹 테이블의 이름

    //뒤끝 콘솔에 생성한 모든 유저 랭킹의 정보
    //GetRankTableList를 사용해 초기화함
    JsonData rankTableList = new JsonData();

    public Dictionary<RankType, JsonData> MyRankData = new(); //자신의 랭크 데이터 (랭크표기에 사용됨)
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
    }
    private bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:
                Initialize();
                return true;
        }
        return false;
    }
    void Initialize()
    {
        if (BackendManager.Instance.IsLocal)
        {
            return;
        }

        RankUuids = new Dictionary<RankType, string>();
        string[] titles = System.Enum.GetNames(typeof(RankType));
        rankTableList = BackendErrorManager.Instance.RetryLogic(() => Backend.URank.User.GetRankTableList()).FlattenRows();
        

        for (int i = 0; i < rankTableList.Count; i++)
        {
            RankType type = RankType.None;
            int idx = 0;

            for (int j = 0; j < titles.Length; j++)
            {
                if (titles[j] == rankTableList[i]["title"].ToString())
                {
                    type = (idx.Equals(0)) ? (RankType)j : (RankType)idx++;
                    break;
                }
            }
            if (type != RankType.None)


                if (type != RankType.None)
                {
                    string uuid = rankTableList[i]["uuid"].ToString();

                    // 예외 처리 코드: 데이터가 이미 존재하는지 확인
                    if (!RankUuids.ContainsKey(type))
                    {
                        // 데이터가 존재하지 않으면 추가
                        // RankUuids.Add(type, uuid);
                        RankUuids.Add(type, rankTableList[i]["uuid"].ToString());
                    }
                    else
                    {
                        // 데이터가 이미 존재하는 경우에 대한 처리
                        // 여기에 필요한 코드를 추가하세요.
                        // 예: 이미 존재하는 경우 로그를 남기거나, 특정 처리를 수행할 수 있습니다.
                        Debug.LogWarning($"Data with RankType {type} already exists in RankUuids.");
                    }
                }
        }
    }

    public JsonData GetRankTable(RankType type)
    {
        for (int i = 0; i < rankTableList.Count; i++)
        {
            if (type.ToString() == rankTableList[i]["title"].ToString())
                return rankTableList[i];
        }

        return null;
    }

    //랭크 로드... 무키 코드에선 여기서 자신의 데이터를 로드시키지만 악키는  StreamingReader.LoadRankData()에서 저장함

    //public void LoadRankData()
    //{
    //    if (BackendManager.Instance.IsLocal) return;

    //    BackendReturnObject data = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.GetMyData(tableName, new Where()));

    //    if (data.IsSuccess())
    //    {
    //        bool isExist = false;
    //        JsonData rows = data.FlattenRows();
    //        JsonData rankData = new JsonData();

    //        if (rows.Count > 0)
    //        {
    //            isExist = true;
    //            rankData = rows[0];
    //            inDate = rankData["inDate"].ToString();
    //        }

    //        if (isExist) //랭킹 데이터가 존재한다면
    //        {
    //            현재 랭크관련 데이터 최신화
    //            Rank.Save_Rank_Power(GameDataTable.Instance.userRank, GameDataTable.Instance.UserData);
    //        }
    //        else //랭킹 데이터가 존재하지 않다면
    //        {
    //            새로 랭킹 데이터를 삽입
    //            Param param = new Param();
    //            삽입전 현재 랭크관련 데이터 최신화
    //            Rank.Save_Rank_Power(GameDataTable.Instance.userRank, GameDataTable.Instance.UserData);

    //            param.Add("ArcherBattlePower", GameDataTable.Instance.userRank.ArcherBattlePower);
    //            param.Add("ArcherLevel", GameDataTable.Instance.userRank.ArcherLevel);
    //            param.Add("GuardianBattlePower", GameDataTable.Instance.userRank.GuardianBattlePower);
    //            param.Add("GuardianLevel", GameDataTable.Instance.userRank.GuardianLevel);
    //            param.Add("MageBattlePower", GameDataTable.Instance.userRank.MageBattlePower);
    //            param.Add("MageLevel", GameDataTable.Instance.userRank.MageLevel);
    //            param.Add("ClearStageLevel", GameDataTable.Instance.userRank.ClearStageLevel);

    //            BackendReturnObject insertData = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert(tableName, param));
    //            if (insertData.IsSuccess())
    //                inDate = insertData.GetInDate();
    //        }

    //        무사키우기는 해당 위치에 isExist로 신규유저 여부를 검사했음..추후 필요시 검토
    //    }
    //}

    /// <summary>
    /// 랭킹 데이터에 자신의 랭크 스코어를 갱신시킴
    /// </summary>
    public void UpdateRankData()
    {
        if (BackendManager.Instance.IsEditorUser) //에디터 유저라면 랭크 갱신 안 함
            return;
        if (BackendManager.Instance.IsLocal)
            return;

        Param param = new Param();
        JsonData rows = new JsonData();

        //현재 랭크관련 데이터 최신화
        Rank.Save_Rank_Power(GameDataTable.Instance.Rank, GameDataTable.Instance.User);
        Rank rank = GameDataTable.Instance.Rank;

        foreach (var uuid in RankUuids)
        {
            double value = -1;
            param.Clear();
            switch (uuid.Key)
            {
                case RankType.BPArcher1:
                    value = rank.ArcherBattlePower;
                    //if (rows.Count > 0) //이미 값이 있을 경우
                    //{
                    //    if (value == double.Parse(rows["ArcherBattlePower"].ToString()))
                    //        value = 0;
                    //}
                    param.Add("ArcherBattlePower", value);
                    param.Add("ArcherLevel", rank.ArcherLevel);
                    break;
                case RankType.BPGuardian1:
                    value = rank.GuardianBattlePower;
                    //if (rows.Count > 0) //이미 값이 있을 경우
                    //{
                    //    if (value == double.Parse(rows["GuardianBattlePower"].ToString()))
                    //        value = 0;
                    //}
                    param.Add("GuardianBattlePower", value);
                    param.Add("GuardianLevel", rank.GuardianLevel);
                    break;
                case RankType.BPMage1:
                    value = rank.MageBattlePower;
                    //if (rows.Count > 0) //이미 값이 있을 경우
                    //{
                    //    if (value == double.Parse(rows["MageBattlePower"].ToString()))
                    //        value = 0;
                    //}
                    param.Add("MageBattlePower", value);
                    param.Add("MageLevel", rank.MageLevel);
                    break;
                case RankType.ClearStageLevel1:
                    value = rank.ClearStageLevel;
                    //if (rows.Count > 0) //이미 값이 있을 경우
                    //{
                    //    if (value == double.Parse(rows["ClearStageLevel"].ToString()))
                    //        value = 0;
                    //}
                    param.Add("ClearStageLevel", value);
                    break;
            }
            if (value > 0) //스코어가 0일경우에는 그냥 갱신이 안 되도록
            {
                var bro1 = BackendErrorManager.Instance.RetryLogic(() => Backend.URank.User.UpdateUserScore(uuid.Value, tableName, StreamingReader.inDateRankData, param), true);
                if (bro1.IsSuccess())
                    SystemNoticeManager.Instance.SystemNoticeInEditor("뒤끝 랭킹 리더보드 갱신(전체)", Utill_Enum.SystemNoticeType.NoBackground);
                else if (bro1.GetErrorCode() == "Precondition Required")
                    BackendErrorManager.Instance.SettingPopUp("428", "Precondition Required", "Precondition Required ranking is being counted");
                else
                    BackendErrorManager.Instance.SettingPopUp(bro1);
            }
        }
        //서버 랭킹 데이터 저장..?
    }

    private void AddToDic(RankType type, JsonData data)
    {
        if (!MyRankData.ContainsKey(type))
        {
            MyRankData.Add(type, data);
        }
        else
        {
            MyRankData[type] = data;
        }
    }

    public JsonData FindMyRank(RankType type)
    {
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RankUuids[type]);
        if (bro.IsSuccess())
            AddToDic(type, bro.GetFlattenJSON());
        return (bro.IsSuccess()) ? bro.FlattenRows()[0] : null;
    }
    public JsonData FindMyRankInfo(RankType type)
    {
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RankUuids[type]);
        if (bro.IsSuccess())
            AddToDic(type, bro.GetFlattenJSON());
        return (bro.IsSuccess()) ? bro.GetFlattenJSON() : null;
    }
    /// <summary>
    /// 마지막으로 조회한 데이터를 반환함.
    /// 만약 조회한 적이 없다면 조회하여 반환
    /// </summary>
    public JsonData GetLastCheckedMyData(RankType type)
    {
        if(!MyRankData.ContainsKey(type))
        {
            MyRankData.Add(type, FindMyRankInfo(type));
        }
        return MyRankData[type];
    }
}
