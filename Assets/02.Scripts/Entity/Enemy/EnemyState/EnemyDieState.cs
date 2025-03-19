using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적이 사망한 상태 구현
/// </summary>
public class EnemyDieState : EnemyState
{
    public EnemyDieState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        SoundManager.Instance.PlayAudio("Demon@Death");
        enemy.NavMeshAgent.enabled = false;
        enemy.Obstacle.enabled = false;
        enemy.Col.enabled = false;

        

        //골드가 나오는 확률 레벨에 비례해서 
        float StageLevelChance = 0;
        //유저의 현재 레벨을 알아야함
        var UserStage = Utill_Math.CalculateStageAndRound(GameDataTable.Instance.User.ClearStageLevel);
        StageTableData stagedata = GameDataTable.Instance.StageTableDic[UserStage.stageindex];
        StageLevelChance = stagedata.GoldDropChance1;
        //유저의 현재 레벨 데이터 파싱
        
        int ckeckGoldMaxvalue = ResourceManager.Instance.Get_MaximumResource(Utill_Enum.Resource_Type.Gold);
        bool isGoldcheck = ConstraintsData.CheckMaxValue(ckeckGoldMaxvalue, stagedata.Gold1);


        if(enemy._EnemyStat.Class == Utill_Enum.EnemyClass.Normal) //노말몹일 경우에만 경헙치/골드 보상 지급
        {
            if (Utill_Math.Attempt(StageLevelChance)) //확률로 골드가 떨어짐
            {
                if (ResourceManager.Instance.GetGold()+stagedata.Gold1 >= ckeckGoldMaxvalue)//골드의 최대값이랑 같거나 넘었으면?
                {
                    NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Gold, enemy.transform.position, 0);
                }
                else
                {
                    NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Gold, enemy.transform.position, stagedata.Gold1);
                    float huntergoldbuff = HunterStat.GetUserGoldBuffValue(NSY.DataManager.Instance.Hunters[0].Orginstat);
                    int reslutgold = (int)(stagedata.Gold1 + (stagedata.Gold1 * huntergoldbuff / 100));
                    GameEventSystem.Send_GamePlusGold_GameEventHandler(Utill_Enum.Resource_Type.Gold, reslutgold); //골드지급 이벤트
                    
                }
                User.Gold += stagedata.Gold1;
                User.Exp += stagedata.Exp1;
            }
            GameEventSystem.GameAddExp_GameEventHandler_Event(stagedata.Exp1); //경험치주는 이벤트

            //죽을때마다 경험치와 골드가 지급되니 여기서 상점 구매를 계속 체크해야한다..
            int level = GameDataTable.Instance.User.HunterLevel[0];
            int gold = ResourceManager.Instance.GetGold();
            int Dia = ResourceManager.Instance.GetDia();
            GameEventSystem.Send_GameCheckResource_GameEventHandler(level,gold, Dia);
        }

        GameEventSystem.GameKillClunt_GameEventHandler_Event(enemy._EnemyStat); //킬올라가는 이벤트

        //몬스터 죽었을때 해당 그라운드에  몇마리아 남아있는지 체크 + 제거
        enemy.Ground.GroundEnemyList.Remove(enemy);
        bool isNoneEnemy = enemy.Ground.CheckZeroEnemyGround();
        if (isNoneEnemy) //만약 다죽었으면
        {
            //보스몹일때 다시 정상스테이지로 돌아가야하는이벤트 호출
            if (enemy._EnemyStat.Class == Utill_Enum.EnemyClass.Boss || enemy._EnemyStat.Class == Utill_Enum.EnemyClass.EBoss)
            {
                GameEventSystem.GameBossSequence_GameEventHandler_Event(Utill_Enum.BossSqeunce.Die);
            }
            else
                GameStateMachine.Instance.ChangeState(new NonCombatState());
        }
    }
}
    