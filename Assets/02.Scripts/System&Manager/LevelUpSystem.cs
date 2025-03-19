using Game.Debbug;
using NSY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 레벨 경험치관련시스템
/// </summary>
public class LevelUpSystem : MonoSingleton<LevelUpSystem>
{
    [SerializeField]
    private List<LevelUpUiView> LevelupUiView;

    private Dictionary<Utill_Enum.SubClass, List<LevelUpUiView>> levelupUiViewDic = new();

    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameAddExp_SendGameEventHandler += AddExp;//경험치 얻었을때 흭득하는 로직
        GameEventSystem.GameSequence_SendGameEventHandler += UpdateLevelUpUi; //게임시작하면 초기화시켜주는 로직
    }
    //Cheat Method
    public void LockInputLevel(bool cheat, int _Level, Utill_Enum.SubClass subClass)
    {
        if ((int)subClass >= GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value) return;
        if (_Level > 0)
        {
            int _level = _Level; 
            User.SetLevelUser_Data(GameDataTable.Instance.User , _level, subClass);

            for (int i = 0; i < levelupUiViewDic[subClass].Count; i++)
                levelupUiViewDic[subClass][i].UpdateLevel(_level);
        }
        else if (_Level == 0)
        {
            User.initUser_Level_Data(GameDataTable.Instance.User);

            for (int i = 0; i < levelupUiViewDic[subClass].Count; i++)
                levelupUiViewDic[subClass][i].Init_Level();
        }
    }
    public void LockInputEXP(bool cheat, int _EXP, Utill_Enum.SubClass subClass)
    {
        if ((int)subClass >= GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value) return;
        if (_EXP > 0)
        {
            float exp = _EXP;
            User.SetExp(GameDataTable.Instance.User, exp , subClass);
            
            for (int i = 0; i < levelupUiViewDic[subClass].Count; i++)
                levelupUiViewDic[subClass][i].UpdateExp(GameDataTable.Instance.User.HunterExp[(int)subClass]);
        }
        else if (_EXP == 0)
        {
            User.initUser_Exp_Data(GameDataTable.Instance.User);

            for (int i = 0; i < levelupUiViewDic[subClass].Count; i++)
                levelupUiViewDic[subClass][i].UpdateExp(GameDataTable.Instance.User.HunterExp[(int)subClass]);
        }
    }

    private bool UpdateLevelUpUi(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence) 
        {
            case Utill_Enum.Enum_GameSequence.CreateAndInit:
                InitDictionary();
                UpdateView();
                return true;
        }
        return false;
    }

    public void ChitLevlInit()
    {
        Init_Level();
        Init_Exp();
    }

    public void Init_Level()
    {
        User.initUser_Level_Data(GameDataTable.Instance.User);

        foreach(var levelupView in levelupUiViewDic.Values)
        {
            for(int i = 0; i < levelupView.Count; i++)
                levelupView[i].Init_Level();
        }
    }

    public void Init_Exp()
    {
        User.initUser_Exp_Data(GameDataTable.Instance.User);
        foreach (var levelupView in levelupUiViewDic.Values)
        {
            for (int i = 0; i < levelupView.Count; i++)
                levelupView[i].Init_Exp();
        }
    }

    public void GetLevel()
    {

    }


    public void GetNextExp()
    {

    }

    private void SetLevelUp()
    {

    }

    private void AddExp(float exp)
    {
        for(int i = 0; i < GameDataTable.Instance.User.CurrentEquipHunter.Count;i++)
        {
            Utill_Enum.SubClass curSubClass = GameDataTable.Instance.User.CurrentEquipHunter[i];
            User.AddExp(GameDataTable.Instance.User , Mathf.FloorToInt(exp / GameDataTable.Instance.User.CurrentEquipHunter.Count), NSY.DataManager.Instance.Hunters[(int)curSubClass]._UserStat, curSubClass);
        }
        UpdateView();
    }

    private void ChangExpToPercent()
    {

    }

    public void UpdateView()
    {
        foreach (var levelupView in levelupUiViewDic)
        {
            for (int i = 0; i < levelupView.Value.Count; i++)
            {
                if ((int)levelupView.Key >= GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_HUNTER_COUNT].Value) return;
                levelupView.Value[i].UpdateLevel(GameDataTable.Instance.User.HunterLevel[(int)levelupView.Key]);
                levelupView.Value[i].UpdateExp(GameDataTable.Instance.User.HunterExp[(int)levelupView.Key]);
            }
        }
    }

    public void OnChangeCharacterType(LevelUpUiView levelUpUiView, Utill_Enum.SubClass subClass)
    {
        levelupUiViewDic[levelUpUiView.characterType].Remove(levelUpUiView);
        levelupUiViewDic[subClass].Add(levelUpUiView);
    }

    private void InitDictionary()
    {
        levelupUiViewDic.Clear();

        for (int i = 0; i< LevelupUiView.Count; i++)
        {
            Utill_Enum.SubClass characterType = LevelupUiView[i].characterType;

            // 이미 딕셔너리에 있는지 확인
            if (!levelupUiViewDic.ContainsKey(characterType))
            {
                levelupUiViewDic[characterType] = new List<LevelUpUiView>() { LevelupUiView[i] };
            }
            else
                levelupUiViewDic[characterType].Add(LevelupUiView[i]);
        }
    }
}
