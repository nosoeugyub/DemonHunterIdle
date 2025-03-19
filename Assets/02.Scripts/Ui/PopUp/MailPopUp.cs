using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

/// <summary>
/// 작성일자   : 2024-07-16
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 우편 불러오기 및 받기 및 받았을때 아이템 추가                                
/// </summary>
public class MailPopUp : MonoBehaviour, IPopUp
{
    public GameObject[] downObjs;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button allReciveBtn;
    [SerializeField] PostScrollController scroll = null;
    private bool isInit = false;
    private List<Post> _postList = new List<Post>();
    public List<PostCellData> list = new List<PostCellData>();


    //모두 받기시 남겨질 아이템들 ....
    private List<Post> _postleftList = new List<Post>();
    public List<PostCellData> leftlist = new List<PostCellData>();


    private int mailCount;
    public int MailCount
    {
        get => mailCount;
        set => mailCount = value;
    }

    private int ClickCount = 0;
    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        ClickCount = 0;
        for (int i = 0; i < downObjs.Length; i++)
        {
            downObjs[i].SetActive(true);
        }
        PopUpSystem.Instance.EscPopupListRemove();
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

    private void Start()
    {
        if (!isInit)
            Initialize();

     
    }
    public void Initialize()
    {
        allReciveBtn.onClick.AddListener(() =>
        {
            PostReceiveAll(PostType.Admin);
        });
   
        LanguageSetting();
        isInit = true;
    }
    public void LanguageSetting()
    {
        allReciveBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_AllRecive");
        titleText.text = LocalizationTable.Localization("Title_Mail");

        LocalizationTable.languageSettings += LanguageSetting;

    }


    private void OnEnable()
    {
        PostListGet(BackEnd.PostType.Admin);
    }

    int i = 0;

    #region 우편 불러오기
    public void PostListGet(PostType postType, bool isInit = false)
    {
        //로컬 우편부터 작업
        List<KeyValuePair<string, int>> Shop;
        string dataname = string.Empty;
        if (GameDataTable.Instance.Mail.Shop1 != null)
        {
            Shop = GameDataTable.Instance.Mail.Shop1; //로컬 우편 데이터 리스트
            dataname = nameof(Shop);
            ProcessLocalMailData(Shop, dataname);
        }

        List<KeyValuePair<string, int>> PurchaseAchievement;
        if (GameDataTable.Instance.Mail.PurchaseAchievement != null)
        {
            PurchaseAchievement = GameDataTable.Instance.Mail.PurchaseAchievement; //로컬 우편 데이터 리스트
            dataname = string.Empty;
            dataname = nameof(PurchaseAchievement);
            ProcessLocalPurchaseAchievementMailData(PurchaseAchievement, dataname);
        }

        List<KeyValuePair<string, int>> DailyMission;
        if (GameDataTable.Instance.Mail.DailyMission != null)
        {
            DailyMission = GameDataTable.Instance.Mail.DailyMission; //로컬 우편 데이터 리스트
            dataname = string.Empty;

            dataname = nameof(DailyMission);
            ProcessLocalDailyMissionData(DailyMission, dataname);
        }

        List<KeyValuePair<string, int>> GachaAchievement;
        if (GameDataTable.Instance.Mail.GachaAchievement != null)
        {
            GachaAchievement = GameDataTable.Instance.Mail.GachaAchievement; //로컬 우편 데이터 리스트
            dataname = string.Empty;
            dataname = nameof(GachaAchievement);
            ProcessLocalGachaAchievementMailData(GachaAchievement, dataname);
        }

        List<KeyValuePair<string, int>> OfflineReward;
           if (GameDataTable.Instance.Mail.OfflineReward != null)
        {
            OfflineReward = GameDataTable.Instance.Mail.OfflineReward; //로컬 우편 데이터 리스트
            dataname = string.Empty;
            dataname = nameof(OfflineReward);
            ProcessLocalOfflineRewardMailData(OfflineReward, dataname);
        }


        if (!BackendManager.Instance.IsLocal) //서버라면 서버우편도 받게
        {
            var bro = BackendErrorManager.Instance.RetryLogic(() => Backend.UPost.GetPostList(postType, 20));


            string chartName = "MailItem";

            if (bro.IsSuccess() == false)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].index = i;
                }

                scroll.SetData(list);

                mailCount = _postList.Count;

                Debug.Log($"MailCount:  {mailCount}");


                for (int i = 0; i < _postList.Count; i++)
                {
                    Debug.Log($"{i}번 째 우편\n" + _postList[i].ToString());
                }
                Debug.LogError("우편 불러오기 중 에러가 발생했습니다.");
                return;
            }

            Debug.Log("우편 리스트 불러오기 요청에 성공했습니다. : " + bro);

            if (bro.GetFlattenJSON()["postList"].Count <= 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].index = i;
                }

                scroll.SetData(list);

                mailCount = _postList.Count;

                Debug.Log($"MailCount:  {mailCount}");


                for (int i = 0; i < _postList.Count; i++)
                {
                    Debug.Log($"{i}번 째 우편\n" + _postList[i].ToString());
                }
                Debug.LogWarning("받을 우편이 존재하지 않습니다.");
                return;
            }

            foreach (LitJson.JsonData postListJson in bro.GetFlattenJSON()["postList"])
            {
                if (postType == PostType.Admin)
                {

                    i = 0;
                    Post post = new Post();
                    post.title = postListJson["title"].ToString();
                    post.content = postListJson["content"].ToString();
                    post.inDate = postListJson["inDate"].ToString();

                    foreach (LitJson.JsonData itemJson in postListJson["items"])
                    {
                        if (itemJson["chartName"].ToString() == chartName)
                        {
                            if (isInit == false)
                            {

                                string author = postListJson["author"].ToString();
                                string expirationDate = postListJson["expirationDate"].ToString();
                                DateTime expirationDateTime = DateTime.Parse(expirationDate, null, System.Globalization.DateTimeStyles.RoundtripKind);


                                PostCellData postCell = new PostCellData();

                                string itemName = itemJson["item"]["ItemName"].ToString();
                                string itemBackgroundName = itemJson["item"]["Grade"].ToString();

                                int itemCount = int.Parse(itemJson["itemCount"].ToString());

                                Sprite itemSprite = Utill_Standard.GetItemSprite(itemName);
                                Sprite itemBackgroundSprite = Utill_Standard.GetItemSprite(itemBackgroundName);

                                if (i > 0)
                                {
                                    postCell.buttonDisable = true;
                                }
                                else
                                {
                                    postCell.buttonDisable = false;
                                }

                                postCell.title = post.title;
                                postCell.senderName = author;
                                postCell.inDate = post.inDate;
                                postCell.itemName = itemName;
                                postCell.itemCount = itemCount;

                                postCell.postType = postType;

                                DateTime currentDateTime = DateTime.UtcNow;
                                TimeSpan timeDifference = expirationDateTime - currentDateTime;

                                string str = Utility.TimeFormatMail(timeDifference);
                                postCell.remainTime = str;

                                postCell.itemSprite = itemSprite;
                                postCell.itemBackgroundSprite = itemBackgroundSprite;

                                if (post.postReward.ContainsKey(itemName))
                                {
                                    post.postReward[itemName] += itemCount;
                                }
                                else
                                {
                                    post.postReward.Add(itemName, itemCount);
                                }

                                i++;
                                list.Add(postCell);

                                post.isCanReceive = true;
                            }


                            _postList.Add(post);
                        }
                        else
                        {
                            Debug.LogWarning("아직 지원되지 않는 차트 정보 입니다. : " + itemJson["chartName"].ToString());
                            post.isCanReceive = false;
                        }
                    }
                }
                //유저쪽과 랭킹쪽은 관리자 쪽 먼저 작업한 후 작업
                else if (postType == PostType.User)
                {

                }
                else if (postType == PostType.Rank)
                {

                }

            }
        }

       

        for (int i = 0; i < list.Count; i++)
        {
            list[i].index = i;
        }

        scroll.SetData(list);

        mailCount = _postList.Count;

        Debug.Log($"MailCount:  {mailCount}");


        for (int i = 0; i < _postList.Count; i++)
        {
            Debug.Log($"{i}번 째 우편\n" + _postList[i].ToString());
        }

    }
    #endregion

    #region 우편 개별 받기
    public void ReceiveMail(PostType postType, int index)
    {
      
        if (_postList.Count <= 0)
        {
            Debug.LogWarning("받을 수 있는 우편이 존재하지 않습니다. 혹은 우편 리스트 불러오기를 먼저 호출해주세요.");
            return;
        }

        if (index >= _postList.Count)
        {
            Debug.LogError($"해당 우편은 존재하지 않습니다. : 요청 index{index} / 우편 최대 갯수 : {_postList.Count}"); 
            return;
        }

        Debug.Log($"{postType.ToString()}의 {_postList[index].inDate} 우편수령을 요청합니다.");

        UIManager.Instance.loadingPopUp.Show(true);

        if (_postList[index].inDate == "") //로컬이라면
        {
            //아이템이름으로 아이템 생성
            Post post = _postList[index];

            foreach (var item in post.postReward)
            {
                string itemname = item.Key;
                int value = item.Value;
                //개별받기
                // 상품 이름에 "gold"가 포함되어 있는지 확인
                if (itemname.ToLower().Contains("gold") || itemname.ToLower().Contains("Gold"))
                {
                    // 골드 바로 유저에게 지급 하지만 최대 수량이라면 여기서 넘거야됨
                    int maxcount = GameDataTable.Instance.ResourceTableDic[Utill_Enum.Resource_Type.Gold].MaxValue;
                    int usergolid = ResourceManager.Instance.GetGold();


                    if (maxcount <= usergolid && value > 0)  // 우편 재화가 0 이상만 맥스 재화 검사하게끔
                    {
                        //우편 재화를 받기시 max라면 ? 우편을 받지않고 넘김
                        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_MaxiamResources"));
                        UIManager.Instance.loadingPopUp.Close(true);
                        return; 
                    }

                    ResourceManager.Instance.PlusGold(Utill_Enum.Resource_Type.Gold, value);
                    //재화텍스트 업데이트
                    GameEventSystem.Send_Resource_Update();
                }
                else if (itemname.ToLower().Contains("dia") || itemname.ToLower().Contains("Dia"))
                {
                    // 골드 바로 유저에게 지급 하지만 최대 수량이라면 여기서 넘거야됨
                    int maxcount = GameDataTable.Instance.ResourceTableDic[Utill_Enum.Resource_Type.Dia].MaxValue;
                    int userdia = ResourceManager.Instance.GetDia();

                    if (maxcount <= userdia && value > 0)
                    {
                        //우편 재화를 받기시 max라면 ? 우편을 받지않고 넘김
                        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_MaxiamResources"));
                        UIManager.Instance.loadingPopUp.Close(true);
                        return;
                    }

                    // 다이아 바로 유저에게 지급
                    ResourceManager.Instance.PlusDia(Utill_Enum.Resource_Type.Dia, value);
                    //재화텍스트 업데이트
                    GameEventSystem.Send_Resource_Update();
                }
                else if (itemname.ToLower().Contains("exp"))
                {
                    //경험치 n빵으로 배급
                    GameEventSystem.GameAddExp_GameEventHandler_Event(value);

                }
                else
                {
                    // 다른 아이템에 대한 처리 (필요에 따라 추가)

                }

                //수령이 정상적으로 진행 되었을 시에만 사운드 재생
                SoundManager.Instance.PlayAudio("RewardReceived");
            }

            //로컬데이터는 삭제해야함
            RemoveLocalMailData(post);

            string inDate = _postList[index].inDate;
            _postList.Remove(_postList[index]);
            list.RemoveAt(index);
            mailCount = _postList.Count;

            Debug.Log($"남은 MailCount:  {mailCount}");

            for (int i = 0; i < list.Count; i++)
            {
                list[i].index = i;
            }

            scroll.SetData(list);

            

            UIManager.Instance.loadingPopUp.Close(true);
        }
        else //서버우편
        {
            var bro = Backend.UPost.ReceivePostItem(postType, _postList[index].inDate);

            if (bro.IsSuccess() == false)
            {
                Debug.LogError($"{postType.ToString()}의 {_postList[index].inDate} 우편수령 중 에러가 발생했습니다. : " + bro.GetMessage());
                return;
            }
            else 
            {
                string inDate = _postList[index].inDate;

                Debug.Log($"{postType.ToString()}의 {_postList[index].inDate} 우편수령에 성공했습니다. : " + bro);

                _postList.RemoveAll(postCell => postCell.inDate == inDate);
                list.RemoveAll(postCell => postCell.inDate == inDate);
                mailCount = _postList.Count;

                Debug.Log($"남은 MailCount:  {mailCount}");

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].index = i;
                }

                scroll.SetData(list);

                if (bro.GetFlattenJSON()["postItems"].Count > 0)
                {
                    SavePostToLocal(bro.GetFlattenJSON()["postItems"]);
                }
                else
                {
                    Debug.LogWarning("수령 가능한 우편 아이템이 존재하지 않습니다.");
                }

                //수령이 정상적으로 진행 되었을 시에만 사운드 재생
                SoundManager.Instance.PlayAudio("UIClick");
                UIManager.Instance.loadingPopUp.Close(true);

            }
        }

        StreamingReader.SaveUserData();
        StreamingReader.SaveMailData();
    }
    #endregion

    #region 우편 모두 받기
    public void PostReceiveAll(PostType postType)
    {
        if (_postList.Count <= 0)
        {
            Debug.LogWarning("받을 수 있는 우편이 존재하지 않습니다. 혹은 우편 리스트 불러오기를 먼저 호출해주세요.");
            return;
        }

        bool isSoundPlayed = false; //수령 사운드 재생 여부
        Debug.Log($"{postType.ToString()} 우편 모두 수령을 요청합니다.");
        UIManager.Instance.loadingPopUp.Show(true);

        if (!BackendManager.Instance.IsLocal) // 서버 우편 처리
        {
            var bro = Backend.UPost.ReceivePostItemAll(postType);

            if (bro.IsSuccess() == false) //우편이없을때
            {
                UIManager.Instance.loadingPopUp.Close(true);
            }
            else
            {
                //개별받기
                if(isSoundPlayed == false)
                {
                    SoundManager.Instance.PlayAudio("RewardReceived");
                    isSoundPlayed = true;
                }
                Debug.Log("우편 모두 수령에 성공했습니다. : " + bro);
                List<int> indicesToRemove = new List<int>();
                foreach (LitJson.JsonData postItemsJson in bro.GetFlattenJSON()["postItems"])
                {
                    SavePostToLocal(postItemsJson);
                    indicesToRemove.Add(i); // 인덱스 기록
                    i++;
                }

                // 서버 데이터에서 제거
                for (int index = 0; index < indicesToRemove.Count; index++)
                {
                    _postList.RemoveAt(0);
                    list.RemoveAt(0);
                }
            }
        }

        // 로컬 우편 처리
        List<int> localIndicesToRemove = new List<int>();
         i = 0;
        foreach (var post in _postList)
        {
            bool isMailSkipped = false; // 우편을 받지 않았는지 체크

            foreach (var item in post.postReward)
            {
                string itemname = item.Key;
                int value = item.Value;

                if (itemname.ToLower().Contains("gold"))
                {
                    int maxcount = GameDataTable.Instance.ResourceTableDic[Utill_Enum.Resource_Type.Gold].MaxValue;
                    int usergold = ResourceManager.Instance.GetGold();

                    if (maxcount <= usergold && value > 0)
                    {
                        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_MaxiamResources"));
                        isMailSkipped = true;
                        _postleftList.Add(post); //남겨진 건 따로 더해주기
                        leftlist.Add(list[i]);
                        break; // 이 우편의 다른 재화 처리 없이 넘어감
                    }

                    ResourceManager.Instance.PlusGold(Utill_Enum.Resource_Type.Gold, value);
                    //재화텍스트 업데이트
                    GameEventSystem.Send_Resource_Update();
                    i++;
                }
                else if (itemname.ToLower().Contains("dia"))
                {
                    int maxcount = GameDataTable.Instance.ResourceTableDic[Utill_Enum.Resource_Type.Dia].MaxValue;
                    int userdia = ResourceManager.Instance.GetDia();

                    if (maxcount <= userdia && value > 0)
                    {
                        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_MaxiamResources"));
                        isMailSkipped = true;
                        _postleftList.Add(post); //남겨진 건 따로 더해주기
                        leftlist.Add(list[i]);
                        break; // 이 우편의 다른 재화 처리 없이 넘어감
                    }

                    ResourceManager.Instance.PlusDia(Utill_Enum.Resource_Type.Dia, value);
                    //재화텍스트 업데이트
                    GameEventSystem.Send_Resource_Update();
                    i++;
                }
                else if (itemname.ToLower().Contains("exp"))
                {
                    i++;
                    GameEventSystem.GameAddExp_GameEventHandler_Event(value);
                    break; // 이 우편의 다른 재화 처리 없이 넘어감
                }
                else
                {
                    // 다른 아이템에 대한 처리
                    break; // 이 우편의 다른 재화 처리 없이 넘어감
                }
            }

            // 만약 해당 우편을 건너뛰었다면, 이후의 처리(삭제 등)를 건너뜀
            if (isMailSkipped)
            {
                i++;
                continue;
            }

            // 로컬 데이터를 삭제해야 함
            RemoveLocalMailData(post);
            localIndicesToRemove.Add(i);
            if(isSoundPlayed == false)
            {
                isSoundPlayed = true;
                SoundManager.Instance.PlayAudio("RewardReceived");
            }
        }

        // 로컬 데이터에서 제거
        for (int index = localIndicesToRemove.Count - 1; index >= 0; index--)
        {
            _postList.RemoveAt(localIndicesToRemove[index]-1);
            list.RemoveAt(localIndicesToRemove[index ]-1);
        }

        if (_postleftList.Count == 0) //모두 받았더라면
        {
            mailCount = _postList.Count;

            for (int z = 0; z < list.Count; z++)
            {
                list[z].index = z;
            }

            StreamingReader.SaveUserData();
            StreamingReader.SaveMailData();

            UIManager.Instance.loadingPopUp.Close(true);
            _postList.Clear();
            list.Clear();
            mailCount = 0;

            scroll.SetData(list);
        }
        else // 받지못한 우편이 하나라도 있다면
        {
            mailCount = _postleftList.Count;

            for (int z = 0; z < leftlist.Count; z++)
            {
                leftlist[z].index = z;
            }
            //저장
            StreamingReader.SaveUserData();
            StreamingReader.SaveMailData();


            UIManager.Instance.loadingPopUp.Close(true);
            scroll.SetData(leftlist);

            leftlist.Clear();
            _postleftList.Clear();
        }
        
    }
    #endregion 

    /// <summary>
    /// 받았을때 아이템 인벤토리에 추가
    /// </summary>
    /// <param name="item"></param>
    public void SavePostToLocal(LitJson.JsonData item)
    {
        foreach (LitJson.JsonData itemJson in item)
        {
            if (itemJson["item"].ContainsKey("ItemType"))
            {
                string itemType = itemJson["item"]["ItemType"].ToString();
                string itemName = itemJson["item"]["ItemName"].ToString();
                int itemCount = int.Parse(itemJson["itemCount"].ToString());

                Item getItem = InventoryManager.Instance.GetItem(itemName);
                InventoryManager.Instance.AddItem(getItem, false);

                Debug.Log($"아이템을 수령했습니다. : {itemName} - {itemCount}개");
            }
            else
            {
                Debug.LogError("지원하지 않는 item입니다.");
            }
        }
    }

    private void OnDisable()
    {
        _postList.Clear();
        list.Clear();
    }

    /// <summary>
    /// 로컬 메일에서 보낸이를 대조하여 어떤 로컬 데이터를 삭제할지 결정 후 삭제
    /// </summary>
    private void RemoveLocalMailData(Post post)
    {
        if (post.sender == "Shop")
            Mail.Remove_Shop1Data(GameDataTable.Instance.Mail.Shop1, post.localMailKey);
        else if (post.sender == "PurchaseAchievement")
            Mail.Remove_PurchaseAchievementData(GameDataTable.Instance.Mail.PurchaseAchievement, post.localMailKey);
        else if (post.sender == "DailyMission")
            Mail.Remove_DailyMissionData(GameDataTable.Instance.Mail.DailyMission, post.localMailKey);
        else if (post.sender == "GachaAchievement")
            Mail.Remove_GachaAchievementData(GameDataTable.Instance.Mail.GachaAchievement, post.localMailKey);
        else if (post.sender == "OfflineReward")
            Mail.Remove_OfflineReawerdData(GameDataTable.Instance.Mail.OfflineReward, post.localMailKey);
    }

    public void ProcessLocalMailData(List<KeyValuePair<string, int>> data, string Name)
    {
        string name = Name;//출처가 여기서 찍힘
        foreach (KeyValuePair<string, int> itemData in data)
        {
            Post post = new Post();
            post.title = "";
            post.content = "";
            post.inDate = "";
            post.sender = name;
            post.localMailKey = itemData.Key;

            // itemData.Key는 아이템 이름, itemData.Value는 아이템 개수
            string itemName = itemData.Key;
            int itemCount = itemData.Value;

            // PostCellData를 생성하고 데이터를 설정합니다.
            PostCellData postCell = new PostCellData();

            //상품 이름에따라 스프라이트 나오도록
            ShopGoldCellData _shopcelldata = ShopGoldCellData.FindShopData(GameDataTable.Instance.ShopResoucrceGolddataDic, itemName);


            Sprite itemSprite = Utill_Standard.GetItemSprite(_shopcelldata.SpriteName); //아이템이미지 파싱
            Sprite itemBackgroundSprite = Utill_Standard.GetItemSprite(_shopcelldata.SpriteName); // 예: 아이템의 배경 이미지

            postCell.itemName = LocalizationTable.Localization(itemName);
            postCell.itemCount = itemCount;
            postCell.itemSprite = itemSprite;
            postCell.itemBackgroundSprite = itemBackgroundSprite;

            // 기타 데이터 설정
            postCell.title = ""; // 필요에 따라 설정
            postCell.senderName = LocalizationTable.Localization(name); // 보낸이 설정
            postCell.inDate = DateTime.UtcNow.ToString("o"); // 예시로 현재 시간을 사용
            postCell.postType = PostType.Admin; // 예시로 Admin 타입 사용
            postCell.buttonDisable = false; // 필요에 따라 설정

            // 만료 시간 처리
            //// DateTime expirationDateTime = DateTime.UtcNow.AddHours(24); // 예시로 24시간 후 만료 설정
            // TimeSpan timeDifference = expirationDateTime - DateTime.UtcNow;
            postCell.remainTime = "-1";

            // isCanReceive 설정
            post.isCanReceive = true;

            // post.postReward에 아이템 정보를 추가합니다.
            if (post.postReward.ContainsKey(itemName))
            {
                post.postReward[itemName] += itemCount;
            }
            else
            {
                post.postReward.Add(itemName, itemCount);
            }

            // 리스트에 postCell을 추가합니다.
            list.Add(postCell);

            // _postList에도 post를 추가합니다.
            _postList.Add(post);
        }
    }
    public void ProcessLocalPurchaseAchievementMailData(List<KeyValuePair<string, int>> data, string Name)
    {
        string name = Name;//출처가 여기서 찍힘
        foreach (KeyValuePair<string, int> itemData in data)
        {
            Post post = new Post();
            post.title = "";
            post.content = "";
            post.inDate = "";
            post.sender = name;
            post.localMailKey = itemData.Key;

            // itemData.Key는 아이템 이름, itemData.Value는 아이템 개수
            string itemName = itemData.Key;
            int itemCount = itemData.Value;

            // PostCellData를 생성하고 데이터를 설정합니다.
            PostCellData postCell = new PostCellData();

            //상품 이름에따라 스프라이트 나오도록
            PurchaseAchievementData _achievementData = PurchaseAchievementData.FindPurchaseAchievementData(GameDataTable.Instance.PurchaseAchievementDataDic, itemName);


            Sprite itemSprite = Utill_Standard.GetItemSprite(_achievementData.resourceType);
            Sprite itemBackgroundSprite = Utill_Standard.GetItemSprite(_achievementData.resourceType); // 예: 아이템의 배경 이미지

            postCell.itemName = LocalizationTable.Localization($"Common_{_achievementData.resourceType}");
            postCell.itemCount = itemCount;
            postCell.itemSprite = itemSprite;
            postCell.itemBackgroundSprite = itemBackgroundSprite;

            // 기타 데이터 설정
            postCell.title = ""; // 필요에 따라 설정
            postCell.senderName = LocalizationTable.Localization("Title_"+name); // 보낸이 설정
            postCell.inDate = DateTime.UtcNow.ToString("o"); // 예시로 현재 시간을 사용
            postCell.postType = PostType.Admin; // 예시로 Admin 타입 사용
            postCell.buttonDisable = false; // 필요에 따라 설정

            // 만료 시간 처리
            //// DateTime expirationDateTime = DateTime.UtcNow.AddHours(24); // 예시로 24시간 후 만료 설정
            // TimeSpan timeDifference = expirationDateTime - DateTime.UtcNow;
            postCell.remainTime = "-1";

            // isCanReceive 설정
            post.isCanReceive = true;

            // 상품과 달리 키값 == 보상이 아니기 때문에 해당 데이터의 리소스 타입을 검사
            if (post.postReward.ContainsKey(_achievementData.resourceType))
            {
                post.postReward[_achievementData.resourceType] += itemCount;
            }
            else
            {
                post.postReward.Add(_achievementData.resourceType, itemCount);
            }

            // 리스트에 postCell을 추가합니다.
            list.Add(postCell);

            // _postList에도 post를 추가합니다.
            _postList.Add(post);
        }
    }
    public void ProcessLocalDailyMissionData(List<KeyValuePair<string, int>> data, string Name)
    {
        string name = Name;//출처가 여기서 찍힘
        foreach (KeyValuePair<string, int> itemData in data)
        {
            Post post = new Post();
            post.title = "";
            post.content = "";
            post.inDate = "";
            post.sender = name;
            post.localMailKey = itemData.Key;

            // itemData.Key는 아이템 이름, itemData.Value는 아이템 개수
            string itemName = itemData.Key;
            int itemCount = itemData.Value;

            // PostCellData를 생성하고 데이터를 설정합니다.
            PostCellData postCell = new PostCellData();

            //상품 이름에따라 스프라이트 나오도록
            DailyMissionData _dailyMissionData = DailyMissionData.FindDailyMissionData(GameDataTable.Instance.DailyMissionList, itemName);
            Utill_Enum.DailyMissionType dailyMissionType = (Utill_Enum.DailyMissionType)int.Parse(itemName.Split('_')[1]);

            Sprite itemSprite = Utill_Standard.GetItemSprite(_dailyMissionData.Missions[dailyMissionType].ResourceType);
            Sprite itemBackgroundSprite = Utill_Standard.GetItemSprite(_dailyMissionData.Missions[dailyMissionType].ResourceType); // 예: 아이템의 배경 이미지

            postCell.itemName = LocalizationTable.Localization($"Common_{_dailyMissionData.Missions[dailyMissionType].ResourceType}");
            postCell.itemCount = itemCount;
            postCell.itemSprite = itemSprite;
            postCell.itemBackgroundSprite = itemBackgroundSprite;

            // 기타 데이터 설정
            postCell.title = ""; // 필요에 따라 설정
            postCell.senderName = LocalizationTable.Localization(name); // 보낸이 설정
            postCell.inDate = DateTime.UtcNow.ToString("o"); // 예시로 현재 시간을 사용
            postCell.postType = PostType.Admin; // 예시로 Admin 타입 사용
            postCell.buttonDisable = false; // 필요에 따라 설정

            // 만료 시간 처리
            //// DateTime expirationDateTime = DateTime.UtcNow.AddHours(24); // 예시로 24시간 후 만료 설정
            // TimeSpan timeDifference = expirationDateTime - DateTime.UtcNow;
            postCell.remainTime = "-1";

            // isCanReceive 설정
            post.isCanReceive = true;

            // 상품과 달리 키값 == 보상이 아니기 때문에 해당 데이터의 리소스 타입을 검사
            if (post.postReward.ContainsKey(_dailyMissionData.Missions[dailyMissionType].ResourceType))
            {
                post.postReward[_dailyMissionData.Missions[dailyMissionType].ResourceType] += itemCount;
            }
            else
            {
                post.postReward.Add(_dailyMissionData.Missions[dailyMissionType].ResourceType, itemCount);
            }

            // 리스트에 postCell을 추가합니다.
            list.Add(postCell);

            // _postList에도 post를 추가합니다.
            _postList.Add(post);
        }
    }


    public void ProcessLocalGachaAchievementMailData(List<KeyValuePair<string, int>> data, string Name)
    {
        string name = Name;//출처가 여기서 찍힘
        foreach (KeyValuePair<string, int> itemData in data)
        {
            Post post = new Post();
            post.title = "";
            post.content = "";
            post.inDate = "";
            post.sender = name;
            post.localMailKey = itemData.Key;

            // itemData.Key는 아이템 이름, itemData.Value는 아이템 개수
            string itemName = itemData.Key;
            int itemCount = itemData.Value;

            // PostCellData를 생성하고 데이터를 설정합니다.
            PostCellData postCell = new PostCellData();

            //상품 이름에따라 스프라이트 나오도록
            GachaAchievementData _achievementData = GachaAchievementData.FindGachaAchievementData(GameDataTable.Instance.GachaAchievementDataDic, itemName);


            Sprite itemSprite = Utill_Standard.GetItemSprite(_achievementData.resourceType);
            Sprite itemBackgroundSprite = Utill_Standard.GetItemSprite(_achievementData.resourceType); // 예: 아이템의 배경 이미지

            postCell.itemName = LocalizationTable.Localization($"Common_{_achievementData.resourceType}");
            postCell.itemCount = itemCount;
            postCell.itemSprite = itemSprite;
            postCell.itemBackgroundSprite = itemBackgroundSprite;

            // 기타 데이터 설정
            postCell.title = ""; // 필요에 따라 설정
            postCell.senderName = LocalizationTable.Localization("Title_" + name); // 보낸이 설정
            postCell.inDate = DateTime.UtcNow.ToString("o"); // 예시로 현재 시간을 사용
            postCell.postType = PostType.Admin; // 예시로 Admin 타입 사용
            postCell.buttonDisable = false; // 필요에 따라 설정

            // 만료 시간 처리
            //// DateTime expirationDateTime = DateTime.UtcNow.AddHours(24); // 예시로 24시간 후 만료 설정
            // TimeSpan timeDifference = expirationDateTime - DateTime.UtcNow;
            postCell.remainTime = "-1";

            // isCanReceive 설정
            post.isCanReceive = true;

            // 상품과 달리 키값 == 보상이 아니기 때문에 해당 데이터의 리소스 타입을 검사
            if (post.postReward.ContainsKey(_achievementData.resourceType))
            {
                post.postReward[_achievementData.resourceType] += itemCount;
            }
            else
            {
                post.postReward.Add(_achievementData.resourceType, itemCount);
            }

            // 리스트에 postCell을 추가합니다.
            list.Add(postCell);

            // _postList에도 post를 추가합니다.
            _postList.Add(post);
        }
    }

    public void ProcessLocalOfflineRewardMailData(List<KeyValuePair<string, int>> data, string Name)
    {
        string name = Name;//출처가 여기서 찍힘
        foreach (KeyValuePair<string, int> itemData in data)
        {
            Post post = new Post();
            post.title = "";
            post.content = "";
            post.inDate = "";
            post.sender = Name;
            post.localMailKey = itemData.Key;

            // itemData.Key는 아이템 이름, itemData.Value는 아이템 개수
            string itemName = itemData.Key;
            int itemCount = itemData.Value;

            // PostCellData를 생성하고 데이터를 설정합니다.
            PostCellData postCell = new PostCellData();

            //상품 이름에따라 스프라이트 나오도록


            Sprite itemSprite = Utill_Standard.GetItemSprite(itemName);
            Sprite itemBackgroundSprite = Utill_Standard.GetItemSprite(itemName); // 예: 아이템의 배경 이미지

            postCell.itemName = LocalizationTable.Localization($"Common_{itemName}");
            postCell.itemCount = itemCount;
            postCell.itemSprite = itemSprite;
            postCell.itemBackgroundSprite = itemBackgroundSprite;

            // 기타 데이터 설정
            postCell.title = ""; // 필요에 따라 설정
            postCell.senderName = LocalizationTable.Localization(name); // 보낸이 설정
            postCell.inDate = DateTime.UtcNow.ToString("o"); // 예시로 현재 시간을 사용
            postCell.postType = PostType.Admin; // 예시로 Admin 타입 사용
            postCell.buttonDisable = false; // 필요에 따라 설정

            // 만료 시간 처리
            //// DateTime expirationDateTime = DateTime.UtcNow.AddHours(24); // 예시로 24시간 후 만료 설정
            // TimeSpan timeDifference = expirationDateTime - DateTime.UtcNow;
            postCell.remainTime = "-1";

            // isCanReceive 설정
            post.isCanReceive = true;

            // 상품과 달리 키값 == 보상이 아니기 때문에 해당 데이터의 리소스 타입을 검사
            if (post.postReward.ContainsKey(itemName))
            {
                post.postReward[itemName] += itemCount;
            }
            else
            {
                post.postReward.Add(itemName, itemCount);
            }

            // 리스트에 postCell을 추가합니다.
            list.Add(postCell);

            // _postList에도 post를 추가합니다.
            _postList.Add(post);
        }
    }
}
