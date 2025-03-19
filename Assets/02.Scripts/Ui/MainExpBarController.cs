using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-08
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 메인 경험치바 별도 로직 제어 (경험치 올라가는 로직은 LevelUpUiView에서 처리)
/// </summary>
public class MainExpBarController : MonoSingleton<MainExpBarController>
{
    [SerializeField]
    private LevelUpUiView levelUpUiView;

    [SerializeField]
    private Button changeTypeButton = null;

    protected override void Awake()
    {
        base.Awake();
        changeTypeButton.onClick.AddListener(ChangeType);
    }
    public void InitType(SubClass subClass)
    {
        levelUpUiView.AdditionalLevelText = $"{LocalizationTable.Localization((subClass).ToString())} ";
        levelUpUiView.characterType = subClass;
        levelUpUiView.Init_NickName();
    }
    public SubClass GetMainExpBarType()
    {
        return levelUpUiView.characterType;
    }

    private void ChangeType()
    {
        SoundManager.Instance.PlayAudio("UIClick");

        int curType = (int)levelUpUiView.characterType;
        for(int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            if(curType>= DataManager.Instance.Hunters.Count - 1) //다음 헌터 찾기
            {
                curType = 0;
            }
            else
            {
                curType += 1;
            }

            if (GameDataTable.Instance.User.HunterPurchase[curType])
            {
                levelUpUiView.AdditionalLevelText = $"{LocalizationTable.Localization(((SubClass)curType).ToString())} ";
                LevelUpSystem.Instance.OnChangeCharacterType(levelUpUiView, (SubClass)curType);
                levelUpUiView.characterType = (SubClass)curType;
                LevelUpSystem.Instance.UpdateView();
                break;
            }
        }
    }
}
