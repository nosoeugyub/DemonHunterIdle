using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-02
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 업적 스크롤의 한 홀더 (AchievementSystem 하고만 소통함)
/// </summary>
public class AchievementHolder : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextColorSet titleText;
    [SerializeField]
    private TextColorSet amountText; // (현재 보유량 / 필요 보유량) 표시하는 텍스트
    [SerializeField]
    private Button submitButton; // 하단 지급 버튼

    private Basic_Prefab submitButtonBasicPrefab = null; //버튼 색 지정용

    private bool isReached = false; //필요 보유량보다 현재 보유량이 더 높은지

    private int rewardAmount = 0; //셀 달성시 지급하는 보상의 양
    private int needAmount = 0; //이 셀을 달성하기위해 필요한 값의 양

    private int cellIndex = 0; //현재 보여주고 있는 셀의 인덱스
    private int sliderIndex = 0; //자신이 속해있는 슬라이더의 인덱스
    private AchievementSystem achievementSystem = null;
    private AchievementType achievementType;

    private bool isColorSetting = false;

    /// <summary>
    /// Basic_Prefab Awake문에서 색상 세팅을 해줘서 그 후에 실행될 수 있도록 해야함
    /// </summary>
    private void Start()
    {
        if(isColorSetting == false)
        {
            SetButtonColor();
            isColorSetting = true;
        }
    }
    private void SendReward()
    {
        //잘못된 인덱스거나 이미 받은 보상이면 return
        if (achievementSystem.IsReceived(cellIndex))
        {
            SoundManager.Instance.PlayAudio("UIClick");
            return;
        }
        //필요 수치가 현재 가지고 있는 양보다 많다면 (실시간으로 다시 체크해 검사함)
        if (achievementSystem.GetTotalAchiveValue(sliderIndex) < needAmount) 
        {
            SoundManager.Instance.PlayAudio("UIClick");
            return;
        }
        SoundManager.Instance.PlayAudio("RewardReceived");
        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_SendToMail"));

        GameManager.Instance.SendRewards(needAmount.ToString(), rewardAmount, achievementType);

        achievementSystem.SetReceived(cellIndex,true);
        SetButtonColor();
        //저장
        StreamingReader.SaveMailData();
    }
    private void SetButtonColor()
    {
        if(submitButtonBasicPrefab == null)
            submitButtonBasicPrefab = submitButton.GetComponent<Basic_Prefab>();

        if (achievementSystem.IsReceived(cellIndex) || !isReached)
        {
            submitButtonBasicPrefab.SetTypeButton(Utill_Enum.ButtonType.DeActive);
        Debug.Log("Set button color deactive" +gameObject.name);
        }
        else
        {
            submitButtonBasicPrefab.SetTypeButton(Utill_Enum.ButtonType.Active);
        Debug.Log("Set button color active" +gameObject.name);
        }
    }
    public void SubmitButtonSetting(string rewardName)
    {
        if(rewardName == null || rewardName.Length == 0)
        {
            submitButton.gameObject.SetActive(false);
        }
        submitButton.gameObject.SetActive(true);
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => { SendReward(); });
    }
    public void Setting(AchievementSystem system,int sliderIndex, int cellIndex, string iconName, string title, int rewardAmount , int currentAmount, int needAmount,bool isShowButton)
    {
        titleText.textMeshProUGUI.text = title;
        
        amountText.textMeshProUGUI.text = Utill_Math.FormatCurrency(currentAmount) + "/" + Utill_Math.FormatCurrency(needAmount);
        isReached = currentAmount >= needAmount;
        this.needAmount = needAmount;
        this.rewardAmount = rewardAmount;
        this.cellIndex = cellIndex;
        this.sliderIndex = sliderIndex;
        achievementSystem = system;

        Sprite sprite = Utill_Standard.GetItemSprite(iconName);
        iconImage.sprite = sprite;

        if(isReached)
        {
            iconImage.material = null;
            titleText.TextColor(Utill_Enum.Text_Color.Default);
            amountText.TextColor(Utill_Enum.Text_Color.Default); 
        }
        else //미도달 상태라면
        {
            Utill_Standard.SetImageMaterial(iconImage, "Black");
            titleText.TextColor(Utill_Enum.Text_Color.Gray);
            amountText.TextColor(Utill_Enum.Text_Color.Gray); 
        }
        scrollRect = achievementSystem.scrollRect;
        submitButton.gameObject.SetActive(isShowButton);
        if(isColorSetting)
        {
            SetButtonColor();
        }
    }

    #region 버튼/이미지가 눌려도 하위의 스크롤이 제대로 작동하도록 만드는 로직
    private ScrollRect scrollRect;

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }
    #endregion
}