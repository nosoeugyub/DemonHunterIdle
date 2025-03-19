using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-10
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 헌터 승급 시스템                                                   
/// </summary>
public class PromotionSystem : MonoSingleton<PromotionSystem>
{
    int goods = 0; // 승급시 필요한 재화 

    private void Awake()
    {
        GameEventSystem.Promotion_Event += Promotion;
    }


    /// <summary>
    /// 승급 버튼 눌렀을때 실행되는 함수
    /// </summary>
    public void Promotion()
    {
        int canPromotion = CanPromotion();
        if (canPromotion == 0) //승급 성공
        {
            int key = GameDataTable.Instance.User.HunterPromotion[GameDataTable.Instance.User.currentHunter] + 1;
            
            GameDataTable.Instance.User.Promotion();

            if (GameDataTable.Instance.PromotionAllocationData.ContainsKey(key))
            {
                var promotionData = GameDataTable.Instance.PromotionAllocationData[key];
                //재화소모타입에따라 소모
                Utill_Enum.Resource_Type type = CSVReader.ParseEnum<Utill_Enum.Resource_Type>(promotionData.ResourceType);
                ResourceManager.Instance.Minus_ResourceType(type, goods);

                UserPromotion(NSY.DataManager.Instance.Hunters[GameDataTable.Instance.User.currentHunter].Orginstat, GameDataTable.Instance.User);
            }

            //승급성공 사운드
            SoundManager.Instance.PlayAudio("UseBuffBook");

            //EquipmentItemManager.Instance.SettingCellLock();

                UIManager.Instance.promotionPopUp.RefreshUI();

            
        }
        else if (canPromotion == 1)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_MaxPromotionLevel"), SystemNoticeType.Default);
            UIManager.Instance.promotionPopUp.RefreshUI();
        }
        else if (canPromotion == 2)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_CannotPromotion"), SystemNoticeType.Default);
        }
        else if(canPromotion == 3)
        {
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_NeedMoreResource"));
        }
        else if(canPromotion == 4)//최대승급달성
        {
            UIManager.Instance.promotionPopUp.MaxPromitoinIUi();
        }
    }

    /// <summary>
    /// 승급 조건 체크 (재화, 헌터레벨, 클리어스테이지)
    /// 0 승급 가능
    /// 1 승급 맥스레벨
    /// 2 승급 조건 부족
    /// 3 재화부족
    /// </summary>
    public int CanPromotion()
    {
        int currentPromotionLevel = GameDataTable.Instance.User.HunterPromotion[GameDataTable.Instance.User.currentHunter];
        int nextPromotionLevel = currentPromotionLevel + 1;

        // 헌터가 최대 승급 레벨인지 확인
        if (!GameDataTable.Instance.PromotionAllocationData.ContainsKey(nextPromotionLevel))
        {
            UIManager.Instance.promotionPopUp.RefreshUI();
            return 2;
        }

        var promotionData = GameDataTable.Instance.PromotionAllocationData[nextPromotionLevel];

        int hunterLevel = promotionData.ReqHunterLevel;
        int clearChapter = promotionData.ReqClearChapterZone;
        int amountRequired = promotionData.ResourceCount;

        bool hasEnoughResource = false;

        switch (promotionData.ResourceType)
        {
            case "Gold":
                hasEnoughResource = ResourceManager.Instance.GetGold() >= amountRequired;
                break;
            case "Dia":
                hasEnoughResource = ResourceManager.Instance.GetDia() >= amountRequired;
                break;
        }

        if (!hasEnoughResource)
            return 3;


        var UserStage = Utill_Math.CalculateStageAndRound(GameDataTable.Instance.User.ClearStageLevel);
        //현재 유저의 스테이지와 라운드를 계산
        int userindex = UserStage.stageindex;
        int userStage = UserStage.CurrentStage;
        int userRound = UserStage.CurrentRound;
        int totalChapter = GameDataTable.Instance.StageTableDic[userindex].ChapterZone;

        int MaxPromotion = GameDataTable.Instance.ConstranitsDataDic[Tag.MAX_PROMOTION_LEVEL].Value;
        if (MaxPromotion <= currentPromotionLevel) //최대치..
        {
            //승급 최대알림을 띄워야함
            SystemNoticeManager.Instance.SystemNotice(LocalizationTable.Localization("SystemNotice_MaxPromotionLevel"), SystemNoticeType.Default);
            return 4;
        }

        goods = amountRequired;

        if (clearChapter < totalChapter &&   //챕터검사
            hunterLevel <= GameDataTable.Instance.User.HunterLevel[GameDataTable.Instance.User.currentHunter] && //헌터 레벨검사
            hasEnoughResource)
        {
            return 0;//승급성공은 0라고함
        }
        else
        { 
            return 2; 
        }
    }

    /// <summary>
    /// 승급했을때 능력치 값 반환
    /// </summary>
    public static (float AttackSpeed, float AttackRange, float MoveSpeed, float PhysicalPower, float MagicPower, float PhysicalPowerDefense, float Hp, float Mp) GetPromotionValue(Dictionary<int, PromotionAbilityData> data_dic, int key)
    {
        float _attackSpeed = 0;
        float _attackRange = 0;
        float _moveSpeed = 0;
        float _physicalPower = 0;
        float _magicPower = 0;
        float _physicalPowerDefense = 0;
        float _hp = 0;
        float _mp = 0;

        PromotionAbilityData data;
        if (data_dic.TryGetValue(key, out data))
        {
            _attackSpeed = data.AttackSpeed;
            _attackRange = data.AttackRange;
            _moveSpeed = data.MoveSpeed;
            _physicalPower = data.PhysicalPower;
            _magicPower = data.MagicPower;
            _physicalPowerDefense = data.PhysicalPowerDefense;
            _hp = data.Hp;
            _mp = data.Mp;
        }
        return (_attackSpeed, _attackRange, +_moveSpeed, _physicalPower, _magicPower, _physicalPowerDefense, _hp, _mp);
    }
  
    /// <summary>
    /// 승급했을때 승급한 헌터 능력치 증가
    /// </summary>
    public void UserPromotion(HunterStat stat, User data)
    {
        int key = GameDataTable.Instance.User.HunterPromotion[GameDataTable.Instance.User.currentHunter];
        var promotion = GetPromotionValue(GameDataTable.Instance.PromotionAbilityData, key);

        float _PhysicalPower = promotion.PhysicalPower;
        float _MagicPower = promotion.MagicPower;
        float _physicalPowerDefense = promotion.PhysicalPowerDefense;
        float _hp = promotion.Hp;
        float _mp = promotion.Mp;

        //능력치값더해주기
        stat.PhysicalPower += _PhysicalPower;
        stat.MagicPower += _MagicPower;
        stat.PhysicalPowerDefense += _physicalPowerDefense;
        stat.HP += _hp;
        stat.MP += _mp;
    }


    /// <summary>
    /// 게임 처음 시작했을때 데이터 로드 이후 승급데이터 바탕으로 능력치 증가 for문으로 모든 헌터 알아서 처리
    /// </summary>
    public void UserAllPromotion(HunterStat stat,int hunterIndex)
    {
        var promotionAbilityDataDic = GameDataTable.Instance.PromotionAbilityData;
        int promotionValue = GameDataTable.Instance.User.HunterPromotion[hunterIndex];

        for (int i = 0; i <= promotionValue; i++)
        {
            var promotion = GetPromotionValue(promotionAbilityDataDic, i);

            stat.PhysicalPower += promotion.PhysicalPower;
            stat.MagicPower += promotion.MagicPower;
            stat.PhysicalPowerDefense += promotion.PhysicalPowerDefense;
            stat.HP += promotion.Hp;
            stat.MP += promotion.Mp;
        }
    }
}
