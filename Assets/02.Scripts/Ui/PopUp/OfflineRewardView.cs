using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-09-24
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 오프라인 View만 처리
/// </summary>
public class OfflineRewardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI OfflineTitle;
    [SerializeField] private TextMeshProUGUI Rewardgold;
    [SerializeField] private TextMeshProUGUI RewardExp;
    [SerializeField] private TextMeshProUGUI RewardDia;
    [SerializeField] private TextMeshProUGUI Etc;
    [SerializeField] private TextMeshProUGUI btn_Reawardtxt;
  
    public Button Reward_Btn;


    public TimeSpan timespan;


    private void Awake()
    {
        Reward_Btn.onClick.AddListener(delegate { OnClickReciveOfflineReward(); });
    }

    private void OnClickReciveOfflineReward()
    {
        SoundManager.Instance.PlayAudio("RewardReceived");
        //우편함알림
        SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_SendToMail"));
        GameEventSystem.Send_OfflineReward();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Close()
    {

    }
    
    public void Update_OfflineVIew(int minit, int gold, int exp, int dia, int offlineMinRewardTime, int offlineMaxRewardTime)
    {
        StringBuilder stb = new StringBuilder();
        string timetext = Utility.TimeFormatDefault(timespan);

        Title.text = LocalizationTable.Localization("Title_NonConnectionCompensation");
        stb.Clear();
        stb.Append(string.Format(LocalizationTable.Localization("NonConnection"), timetext));
        stb.AppendLine();
        stb.Append(string.Format(LocalizationTable.Localization("MaxiamNonConnection"), offlineMaxRewardTime / 60));

        OfflineTitle.text = stb.ToString();

        Rewardgold.text = Utill_Math.FormatCurrency(gold); 
        RewardExp.text = Utill_Math.FormatCurrency(exp); //exp.ToString();
        RewardDia.text = Utill_Math.FormatCurrency(dia); //dia.ToString();


        Etc.text =string.Format( LocalizationTable.Localization("NonConnectionApplicationTime") , offlineMinRewardTime);
        btn_Reawardtxt.text = LocalizationTable.Localization("Button_Recive");


    }
}
