using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스테이지 진척도 관련
/// /// </summary>
public class StageClearView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CurrentStageTxt;
    [SerializeField] TextMeshProUGUI CurentKillTxt;
    [SerializeField] Image StageBar;
    Enemy tempEnemy;
    [SerializeField] Button BossButton;
    public Image StageSlider; // 경험치 바 추가
    public RectTransform PrefabPos;
    public TextMeshProUGUI[] TestNameAtkDefHp;


    [Space(10)]
    [SerializeField] private GameObject MonsterSpecUi;


    public bool isMaxStage;
    public int CurrentStage;
    public float CurrentKill;
    private void Awake()
    {
        StageBar.color = TextColorManager.Instance.SendStageColor;
    }
    public void Init_Stage()
    {
       string ConversString = Utill_Standard.ConvertNumberToPattern(1);
        CurrentStageTxt.text = "스테이지" + ConversString;
        StageSlider.fillAmount = 0;
    }

    public void Init_Kill()
    {
        CurentKillTxt.text = "0/0";
        StageSlider.fillAmount = 0;
    }
    public void UpdateStageCount(int chapter,  int stage , int round)  
    {
        //최대 스테이지인지 확인
        ConstraintsData data = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_STAGELEVEL];
        var ckeckMaxvalue = ConstraintsData.StageCheckMaxValueResult(data, stage , round);
        

        
        //글씨 색상
        string textColorCode = ColorCodeData.GetTextColor(GameDataTable.Instance.ColorCodeDataDic, Tag.TEXT_RED1);
        
        Color WhiteColor = Color.white;
        if (ckeckMaxvalue.isMaxstage)//최대값이랑 같거나 넘었으면?
        {
            //표기
            isMaxStage = true;

            string ConvertString = ckeckMaxvalue.chapter + "-" + ckeckMaxvalue.stage + "-" + ckeckMaxvalue.Round;
            CurrentStageTxt.text = LocalizationTable.Localization("Stage") + ConvertString;
            //색상 변경
            CurrentStageTxt.GetComponent<TextColorSet>().TextColor(Utill_Enum.Text_Color.Red);
            //보스 버튼도 비활성화
            GroundCreatSystem.Instance.EnenbleBoss();
        }
        else
        {
            isMaxStage = false;
            //표기
            string ConvertString = Utill_Math.FormatCurrency(chapter) + "-" + Utill_Math.FormatCurrency(stage) + "-" + Utill_Math.FormatCurrency(round);
            CurrentStageTxt.text = "스테이지" + ConvertString;

            //색상원래대로
            CurrentStageTxt.GetComponent<TextColorSet>().TextColor(Utill_Enum.Text_Color.White);
        }


    }

    public void UpdateKillCount(int kill , int maxgoal)
    {
        //색상가져오기
        string textColorCode = ColorCodeData.GetTextColor(GameDataTable.Instance.ColorCodeDataDic, Tag.TEXT_RED1);
        Color color;
        Color WhiteColor = Color.white;

        float Amount = (float)kill / maxgoal;
        if (Amount < 0f)
        {
            Amount = 0f;
        }

        if (isMaxStage)
        {
            //색상 변경
            if (ColorUtility.TryParseHtmlString(textColorCode, out color))
            {
                CurentKillTxt.color = color;
                StageSlider.color = color;
            }


            StageSlider.fillAmount = 1;
            CurentKillTxt.text = Utill_Math.FormatCurrency(maxgoal) + "/" + Utill_Math.FormatCurrency (maxgoal);
        }
        else
        {
            //표기
            StageSlider.fillAmount = Amount;
            CurentKillTxt.text = Utill_Math.FormatCurrency( kill) + "/" + Utill_Math.FormatCurrency(maxgoal);
        }
    }

    public void UpdateEnemyInfo(EnemyStat stat , string BossMobName , Vector3 _transformlist ,Quaternion spawnAngle)
    {
        TestNameAtkDefHp[0].text = stat.MobName;
        TestNameAtkDefHp[1].text = stat.HP.ToString();
        TestNameAtkDefHp[2].text = stat.PhysicalPower.ToString();
        TestNameAtkDefHp[3].text = stat.PhysicalPowerDefense.ToString();
        
    }



    public void ActiveMonsterSpec(bool _isActive)
    {
        MonsterSpecUi.SetActive(_isActive);
    }
}



