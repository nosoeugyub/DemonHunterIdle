using BackEnd;
using EnhancedUI.EnhancedScroller;
using Game.Debbug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.UI;
using UnityEngine.Analytics;

/// </summary>
/// 작성일자   : 2024-06-8
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 상점팝업
/// </summary>
public class ShopPopup : MonoBehaviour, IPopUp, IStoreListener
{
    [SerializeField] private int ClickStack = 0; //가장 바깥쪽 카테고리의 버튼 클릭스텍
    //Image
    public Image ButtonImg;
    public Image ActiveOverlay;
    //button
    public Button[] Upgradeinbtnarray;
    public Button[] Categorybtnarray;
    public Button achievementBtn;


    //text
    public TextMeshProUGUI[] Upgradeintextarray;
    public TextMeshProUGUI TBC;
    public TextMeshProUGUI Shop_title;
    //Rect
    public RectTransform[] CategorySubArray;
    public int CurrentSubCategoryindex = -1;



    //상점구매관련
    public IStoreController storeController; //구매과정을 제어하는 함수 제공자
    private ShopGoldCellData cellData = null;
    public IExtensionProvider extensions; //여러 플랫폼을 위한 확장 처리 제공자

    public ShopPopUpController_2 shopPopupcontroller_2;
    public ShopPopUpController shoppopupcontroller;
    private void Awake()
    {
        //상점 등록 및 초기화
        if (storeController == null)
        {
            InitStore();
        }
    }

    private void OnEnable()
    {
        shopPopupcontroller_2.ResizeScroller(0 , false);
        shoppopupcontroller.ResizeScroller(false);
        // 세부 카테고리 버튼 이벤트 할당
        for (int i = 0; i < Upgradeinbtnarray.Length; i++)
        {
            int index = i; // 반복문 변수의 값을 캡처하기 위해 변수를 생성합니다.
            Upgradeinbtnarray[i].onClick.AddListener(() => ActiveSubCategory(index));
        }

        //과금 팝업 이벤트 할당
        achievementBtn.onClick.RemoveAllListeners();
        achievementBtn.onClick.AddListener(() =>{ 
            UIManager.Instance.purchaseAchievementPopUp.Show();
            SoundManager.Instance.PlayAudio("UIClick");
        });

        Shop_title.text = LocalizationTable.Localization("Title_Shop");
    }

    public void init_CharaterUpgradePopup()
    {
        CurrentSubCategoryindex = -1;
    }

    public void Close()
    {
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        PopUpSystem.Instance.EscPopupListRemove();

        ClickStack = 0;
        // 팝업을 화면에 표시하는 로직을 여기에 구현
        gameObject.SetActive(false);
        ButtonImg.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        //배틀유아이 이동
        PopUpSystem.Instance.MoveBattleCanvs(0);

    }

    public void Show()
    {
        ClickStack++;
        if (ClickStack == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);

            //배틀유아이 이동
            PopUpSystem.Instance.MoveBattleCanvs(3000);
            // 팝업을 화면에 표시하는 로직을 여기에 구현
            gameObject.SetActive(true);
            ButtonImg.color = new Color32(0x2A, 0xD0, 0xB8, 0xFF);
            ActiveButton(0);//CurrentSubCategoryindex
        }
        else
        {
            Hide();
        }
    }

    public void ActiveButton(int index)
    {
        if (index == -1)
        {
            index = 0;
        }


        for (int i = 0; i < Upgradeinbtnarray.Length; i++)
        {
            if (i == index)
            {
                //글씨색바꾸기
                Upgradeintextarray[i].color = Color.white;
                //하위팝업은 켜기
                CategorySubArray[i].gameObject.SetActive(true);

                //나머지 밑의 오브젝트들도 끄기 
                Upgradeinbtnarray[i].transform.GetChild(0).gameObject.SetActive(true);
                //나머지 밑의 오브젝트들도 끄기 
                //Upgradeinbtnarray[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {

                CategorySubArray[i].gameObject.SetActive(false);
                //글씨색 바꾸기
                Upgradeintextarray[i].color = Color.gray;

                //나머지 밑의 오브젝트들도 끄기 
                Upgradeinbtnarray[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    public void ActiveSubCategory(int i)
    { //클릭한 인덱스 열기
        switch (i)
        {
            case 0:
                CurrentSubCategoryindex = 0;
              
                break;
            case 1:
                CurrentSubCategoryindex = 1;
               
                break;
            case 2:
                CurrentSubCategoryindex = 2;
                
                break;
        }
        //인덱스 해당하는 팝업켜지게..
        ActiveButton(CurrentSubCategoryindex);
    }


    //상점..구매관련
    public void OnInitialized(IStoreController _controller, IExtensionProvider _extensions)
    {
        storeController = _controller;
        extensions = _extensions;
    }

    void InitStore()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //골드 상품부터 삼품 등록
        foreach (var item in GameDataTable.Instance.ShopResoucrceGolddataDic) //딕셔너리 데이터에서 상품을 하나식 등록
        {
            string productId = "";
#if UNITY_ANDROID 
             productId = item.Value.Aos; // 각 상품의 Android용 Product ID (AOS)
#elif UNITY_IOS
            productId = item.Value.IOS; // 각 상품의 Android용 Product ID (AOS)
#endif
            // Consumable 타입으로 상품을 등록합니다.
            builder.AddProduct(productId, ProductType.Consumable, new IDs
            {
                { productId, GooglePlay.Name },
                { item.Value.IOS, AppleAppStore.Name } // iOS용 Product ID도 추가

            });
        }
        UnityPurchasing.Initialize(this, builder);
    }

    /* 구매하는 함수 */
    public void Purchase(ShopGoldCellData productdata)
    {
        Debug.Log(productdata + " 데이터가 있나 없나");
        //aos , ios 전환
        string productId = "";
#if UNITY_ANDROID 
        productId = productdata.Aos;
#elif UNITY_IOS
        productId = productdata.IOS; 
#endif

        cellData = productdata;//데이터할당
        Debug.Log("상품아이디는 " + productId);
        Product product = storeController.products.WithID(productId); //상품 정의
        if (product == null)
        {
            UIManager.Instance.loadingPopUp.Close(true);
            Debug.Log("상품이 없거나 현재 구매가 불가능합니다");
            return;
        }
        Debug.Log(product + "상품가져오기");
        if (product != null && product.availableToPurchase) //상품이 존재하면서 구매 가능하면
        {
            storeController.InitiatePurchase(product); //구매가 가능하면 진행
        }
        else //상품이 존재하지 않거나 구매 불가능하면
        {
            Debug.Log("상품이 없거나 현재 구매가 불가능합니다");
        }
    }

    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
       //결재기능 초기화 완료
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
      //초기화 실패
    }

    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
    {
        try
        {
            bool isSuccess = true;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR

        CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

        try
        {
            IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);

            foreach (var productReceipt in result)
            {
                Analytics.Transaction(productReceipt.productID, e.purchasedProduct.metadata.localizedPrice,
                    e.purchasedProduct.metadata.isoCurrencyCode, productReceipt.transactionID, null);

#if UNITY_ANDROID
                GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                if (google != null)
                {
                    if (google.purchaseState != GooglePurchaseState.Purchased)
                    {
                        isSuccess = false;
                        Debug.LogError("구매 상태가 'Purchased'가 아님. GooglePurchaseState: " + google.purchaseState);
                    }
                }
#endif
            }
        }
        catch (IAPSecurityException ex)
        {
            isSuccess = false;
            Debug.LogError("영수증 검증 실패: " + ex.Message);
        }
#endif
            if (isSuccess)
            {
                // 구매 성공 처리
                Debug.Log("구매 성공! 영수증 검증 성공!");
                SoundManager.Instance.PlayAudio("RewardReceived");
                string productId = e.purchasedProduct.definition.id;
                string receipt = null;
                string signature = null;

#if UNITY_ANDROID
                try
                {
                    GetReceiptSignature(e.purchasedProduct.receipt, out receipt, out signature);
                    Debug.Log($"Android Receipt: {receipt}, Signature: {signature}");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Android 영수증 추출 실패: " + ex.Message);
                    return PurchaseProcessingResult.Pending;
                }
#elif UNITY_IOS
            try
            {
                receipt = e.purchasedProduct.receipt;
                Debug.Log($"iOS Receipt: {receipt}");
            }
            catch (Exception ex)
            {
                Debug.LogError("iOS 영수증 추출 실패: " + ex.Message);
                return PurchaseProcessingResult.Pending;
            }
#endif
                string currency = e.purchasedProduct.metadata.isoCurrencyCode;
                double price = decimal.ToDouble(e.purchasedProduct.metadata.localizedPrice);

                // 유저 과금 업데이트
                try
                {
                    GameDataTable.Instance.User.TotalPurchase += cellData.DiscountedPrice;
                    Debug.Log($"ProcessPurchase id: {productId}, receipt: {receipt}, signature: {signature}, currency: {currency}, price: {price}");
                }
                catch (Exception ex)
                {
                    Debug.LogError("유저 과금 업데이트 실패: " + ex.Message);
                }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
#if UNITY_ANDROID
            BackEnd.Backend.TBC.ChargeTBC(e.purchasedProduct.receipt,
                $"ProcessPurchase id: {productId}, receipt: {receipt}, signature: {signature}, currency: {currency}, price: {price}", callback =>
            {
                if (callback.IsSuccess())
                {
                    // TBC 처리 성공
                    Invoke("GetTBC", 0.1f);
                    OnPurchaseSuccess(productId);
                }
                else
                {
                    Debug.LogError($"ChargeTBC 실패 - ErrorCode: {callback.GetErrorCode()}, Message: {callback.GetMessage()}");
                    if (callback.GetStatusCode() == "404")
                        OnPurchaseSuccess(productId); // 오류 시에도 성공 처리
                }
            });
#elif UNITY_IOS
            BackEnd.Backend.TBC.ChargeTBC(productId, e.purchasedProduct.receipt, 
                $"ProcessPurchase id: {productId}, receipt: {receipt}, signature: {signature}, currency: {currency}, price: {price}", callback =>
            {
                if (callback.IsSuccess())
                {
                    // TBC 처리 성공
                    Invoke("GetTBC", 0.1f);
                    OnPurchaseSuccess(productId);
                }
                else
                {
                    Debug.LogError($"ChargeTBC 실패 - ErrorCode: {callback.GetErrorCode()}, Message: {callback.GetMessage()}");
                    if (callback.GetStatusCode() == "404")
                        OnPurchaseSuccess(productId);
                }
            });
#endif
#else
                if (!BackendManager.Instance.IsLocal)
                {
                    // 서버로 마일리지 처리 요청
                    try
                    {
                        Backend.TBC.ChargeTBC(e.purchasedProduct.receipt,
                            $"ProcessPurchase id: {productId}, receipt: {receipt}, signature: {signature}, currency: {currency}, price: {price}", callback =>
                            {
                                if (callback.IsSuccess())
                                {
                                    Invoke("GetTBC", 0.1f);
                                }
                                else
                                {
                                    Debug.LogError($"TBC 처리 실패 - ErrorCode: {callback.GetErrorCode()}, Message: {callback.GetMessage()}");
                                }
                            });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("TBC 서버 처리 실패: " + ex.Message);
                    }
                }

                OnPurchaseSuccess(productId); // 로컬 구매 성공 처리
#endif
            }
        }
        catch (Exception error)
        {
            Debug.LogError("구매 처리 중 오류 발생: " + error.Message);
            return PurchaseProcessingResult.Pending;
        }
        return PurchaseProcessingResult.Complete;
    }

    void GetReceiptSignature(string strReceipt, out string receipt, out string signature)
    {
        UnityReceipt unityReceipt = JsonUtility.FromJson<UnityReceipt>(strReceipt);
        if (unityReceipt != null)
        {
            if (unityReceipt.Payload == "ThisIsFakeReceiptData")
            {
                Debug.Log("ThisisFakeReceiptData");
                receipt = null;
                signature = null;
                return;
            }
            else
            {
                GooglePlayPayload googlePlayPayload = JsonUtility.FromJson<GooglePlayPayload>(unityReceipt.Payload);
                if (googlePlayPayload != null)
                {
                    receipt = googlePlayPayload.json;
                    signature = googlePlayPayload.signature;
                    return;
                }
            }
        }

        receipt = "";
        signature = "";
    }

    private void OnPurchaseSuccess(string productId)
    {
        string cellid = "";
        //해당 상품아이디로 상품 불러오기
#if UNITY_EDITOR && UNITY_ANDROID
        cellid = cellData.Aos;
#elif UNITY_IOS
    cellid = cellData.IOS;
#endif
        if (cellid == productId)
        {
            switch (cellData.Type)
            {
                case Utill_Enum.ShopType.Resource: // 금화
                    AddCoin(cellData       , productId);
                    break;
            }

        }


        UIManager.Instance.loadingPopUp.Close(false);//버퍼링끄기
    }


    void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
    {
        if (!error.Equals(PurchaseFailureReason.UserCancelled))
        {
           //구매실패
            
        }
    }

    void AddCoin(ShopGoldCellData data , string itemnumber)
    {
        //메일로 금화 보내주기
        GameManager.Instance.SendRewards(data , itemnumber);

        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_SendToMail"));
        //저장
        StreamingReader.SaveMailData();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }

    [System.Serializable]
    class UnityReceipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;
        public UnityReceipt()
        {
            this.Store = null;
            this.TransactionID = null;
            this.Payload = null;
        }
    }
    [System.Serializable]
    class GooglePlayPayload
    {
        public string json;
        public string signature;
        public GooglePlayPayload()
        {
            this.json = null;
            this.signature = null;
        }
    }
    private void GetTBC()
    {
        //yield return new WaitForSeconds(2.0f);
        BackEnd.Backend.TBC.GetTBC(callback =>
        {
            if (callback.IsSuccess())
            {
                var tbc = int.Parse(callback.GetReturnValuetoJSON()["amountTBC"].ToString());
                TBC.text = tbc.ToString("N0");
            }
        });
        //yield return null;
    }
}
