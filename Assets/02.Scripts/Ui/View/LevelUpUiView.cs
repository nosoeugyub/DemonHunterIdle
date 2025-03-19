using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using BackEnd;
using Unity.Collections.LowLevel.Unsafe;

/// </summary>
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 레벨업 관련 보여지는 컨틀롤러 
/// /// </summary>
public class LevelUpUiView : MonoBehaviour
{
    [SerializeField] Image ExpBar;

    [SerializeField] TextMeshProUGUI Level;
    [SerializeField] TextMeshProUGUI ExpPercent;
    [SerializeField] TextMeshProUGUI NickName;

    public Image ExpSlider; // 경험치 바 추가

    public int CurrentLevel;
    public float CurrentExp;
    public string Nickname;
    public Utill_Enum.SubClass characterType;

    public string AdditionalLevelText = "";//메인 경험치바의 경우 레벨 앞에 클래스 이름을 덧붙여야함

    private void Awake()
    {
        if (ExpBar != null)
        {
            ExpBar.color = TextColorManager.Instance.SendExpColor;
        }
        if (ExpSlider != null)
            ExpSlider.material.SetFloat("_ProgressBar", 1);
    }

    public void Init_NickName()
    {
        if (NickName == null) return;
        if (BackendManager.Instance.IsLocal == false)
        {
            Nickname = Backend.UserNickName;
            NickName.text = Nickname.ToString();
        }
        else
        {
            Nickname = "테스트닉네임";
            NickName.text = Nickname.ToString();
        }

    }
    public void Init_Level()
    {
        if (Level == null) return;
        Level.text = AdditionalLevelText + "Lv.0";
    }

    public void Init_Exp()
    {
        if (ExpPercent != null)
            ExpPercent.text = "0%";
        if (ExpSlider != null)
        {
            ExpSlider.fillAmount = 0;
            ExpSlider.material.SetFloat("_ProgressBar", 1);
        }
    }
    public void UpdateLevel(int level)
    {
        //max재화 판단
        ConstraintsData data = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_LEVEL];
        int ckeckMaxvalue = ConstraintsData.CheckMaxValueResult(data, level);
        bool ischeck = ConstraintsData.CheckMaxValue(data, level);


        //글씨 색상
        Color color;
        Color WhiteColor = Color.white;
        if (ischeck)//최대값이랑 같거나 넘었으면?
        {
            //표기

            if (Level != null)
                Level.text = AdditionalLevelText + "Lv. " + ckeckMaxvalue.ToString();
            //색상 변경
            color = UIManager.Instance.MaxExpBarColor;

            if (Level != null)
                Level.color = color;

            if (ExpSlider != null)
                ExpSlider.material.SetColor("_ColorMask", color);
        }
        else
        {
            //표기
            if (Level != null)
                Level.text = AdditionalLevelText + "Lv. " + ckeckMaxvalue.ToString();

            color = UIManager.Instance.DefultExpBarColor;
            if (ExpSlider != null)
                ExpSlider.material.SetColor("_ColorMask", color);
        }
    }

    public void UpdateExp(float exp)
    {
        int level = 0;
        if(characterType>=0)
        {
            level = GameDataTable.Instance.User.HunterLevel[(int)characterType];
        }
        else
        {
            level = GameDataTable.Instance.User.HunterLevel[0];
        }

        //최대 레벨 계산
        ConstraintsData data = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_LEVEL];
        int ckeckMaxvalue = ConstraintsData.CheckMaxValueResult(data, level);

        //만렙이라면
        if (ckeckMaxvalue == data.Value)
        {
            if (ExpSlider != null)
                ExpSlider.fillAmount = 1;

            if (ExpPercent != null)
                ExpPercent.text = (100).ToString("F1") + "%";
        }
        else
        {
            string key = ckeckMaxvalue.ToString();
            float currentExp = 0;
            if (GameDataTable.Instance.RequireDataDic.ContainsKey(key))
            {
                // 딕셔너리에 해당 키가 존재하는 경우, 값을 할당합니다.
                currentExp = GameDataTable.Instance.RequireDataDic[key].Exp;
            }
            if (ExpPercent != null)
                ExpPercent.text = (exp / currentExp * 100f).ToString("F1") + "%";

            if (ExpSlider != null)
            {
                float value = (exp / currentExp);
                if (ExpSlider != null)
                    ExpSlider.fillAmount = Mathf.Clamp01(value);
            }
        }
    }



    public void CheckLevelUp(int _currentLevel, float _currentexp, float _addexp)
    {
        if (_currentexp > _addexp)
        {
            if (Level != null)
                Level.text = _currentLevel.ToString();
        }





    }
}
