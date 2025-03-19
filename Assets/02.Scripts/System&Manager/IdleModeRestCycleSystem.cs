using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-25
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 방치모드 피로도 시스템을 관리
/// </summary>
public class IdleModeRestCycleSystem : MonoSingleton<IdleModeRestCycleSystem>
{
    [SerializeField]
    private IdleModeRestCycleUIView idleModeRestCycleUIView;
    [Header("미장착 헌터 소환 시점")]
    [SerializeField]
    private float unEquipHunterSpawnDelay = 1f;

    private bool[] hunterArrivalToCamp = new bool[3];
    private bool isWaitForRest = false;

    private IdleModeRestCycleSequence curSequence = IdleModeRestCycleSequence.None;

    public bool IsWaitForRest => isWaitForRest;
    public IdleModeRestCycleSequence CurSequence => curSequence;
    public float UnEquipHunterSpawnDelay => unEquipHunterSpawnDelay;

    protected override void Awake()
    {
        base.Awake();
        GameEventSystem.GameSequence_SendGameEventHandler += GetGameSequence;
        GameEventSystem.GameRestModeSequence_SendGameEventHandler += RestModeSequence;
        GameEventSystem.GameAddRestCycle_SendGameEventHandler += AddRestCycle;
        GameEventSystem.GameAddRestCycle_SendGameEventHandler += SubRestCycle;
    }
    private void RestModeSequence(Utill_Enum.IdleModeRestCycleSequence sequence)
    {
        curSequence = sequence;
        switch (sequence)
        {
            case IdleModeRestCycleSequence.Cinematic:
                isWaitForRest = false;
                //전체사망시 재배치 로직 돌지 않도록
                //HunterPortraitSystem.Instance.StopSettingBattleCoroutine();

                //헌터 움직임 제한 해제 & 죽어있다면 오브젝트 꺼지도록
                for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                {
                    DataManager.Instance.Hunters[i].UnLimitHunterPos();
                    //휴식 상태 돌입하면 죽거나 하지 않도록
                    DataManager.Instance.Hunters[i].SetCanGetDamage(false); 
                    if (DataManager.Instance.Hunters[i].GetState() == HunterStateType.Die)
                        DataManager.Instance.Hunters[i].gameObject.SetActive(false);
                }

                StartCoroutine(SpawnUnEquipHunter());
                break;
            case IdleModeRestCycleSequence.Start:
                idleModeRestCycleUIView.SetRestCycleUI(true);
                StartCoroutine(IdleModeRestTimeCoroutine());
                //수호자 hp를 조금 조정
                NSY.DataManager.Instance.Hunters[1].SetHpUiPostionY(-1.78f);
                break;
            case IdleModeRestCycleSequence.End:
                NSY.DataManager.Instance.Hunters[1].SetHpUiPostionY(-0.8f);
                for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                {
                    StopCoroutine(DataManager.Instance.Hunters[i].WaitHunter(GameManager.Instance.fadeDuration + 1f));
                    StartCoroutine(DataManager.Instance.Hunters[i].WaitHunter(GameManager.Instance.fadeDuration + 1f));
                }

                for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
                {
                    DataManager.Instance.Hunters[i].SetCanGetDamage(true); //다시 데미지 받을 수 있도록
                    if (DataManager.Instance.isEquipHunter((SubClass)i) == false) //전에 장착했던 헌터가 아니라면
                    {
                        DataManager.Instance.Hunters[i].gameObject.SetActive(false);
                        DataManager.Instance.Hunters[i].ActiveHpObject(false);
                        //DataManager.Instance.MainCameraMovement.RemoveCalculateHunter(DataManager.Instance.Hunters[i]);
                    }
                    else
                    {
                        DataManager.Instance.Hunters[i].ResetPosition();
                    }
                }
                break;
        }
    }

    private bool GetGameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.InGame:
                StopAllCoroutines();
                idleModeRestCycleUIView.InitSetRestCycleUI();
                idleModeRestCycleUIView.SetRestCycleUI(false);
                if(CanEnterRestMode())
                {
                    isWaitForRest = true;
                    //StartCoroutine(IdleModeRestTimeCoroutine());
                }
                else
                {
                    StartCoroutine(IdleModeRestCycleCoroutine());
                }

#if UNITY_EDITOR
                StartCoroutine(SetDebugText());
#endif
                return true;
        }
        return true;
    }

    /// <summary>
    /// 피로도 가득 찼는지 검사, UI를 업데이트하고 피로도가 가득 찾다면 관련 변수 설정함
    /// </summary>
    /// <returns>피로도가 가득 찼는지 여부</returns>
    private bool CheckIdleModeRestCycle()
    {
        //피로도가 꽉 찼다면
        if (idleModeRestCycleUIView.UpdateIdleModeRestCycleBar())
        {
            //휴식모드 돌입대기
            isWaitForRest = true;
            GameDataTable.Instance.User.IdleModeRestTime = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// amount만큼 IdleModeRestCycle변수에 값을 더하고 
    /// 최대치를 확인하여 최대치 이상 데이터가 넘어가지 않도록 조절
    /// </summary>
    private void AddIdleModeRestCycle(int amount)
    {
        GameDataTable.Instance.User.IdleModeRestCycle += amount;

        //최대치 이상 올라갔다면 다시 최대치로
        if (GameDataTable.Instance.User.IdleModeRestCycle > GameManager.Instance.IdleModeRestCycle)
            GameDataTable.Instance.User.IdleModeRestCycle = GameManager.Instance.IdleModeRestCycle;
    }

    /// <summary>
    /// 현재 휴식모드로 진입 가능한지 여부
    /// </summary>
    private bool CanEnterRestMode()
    {
        return GameDataTable.Instance.User.IdleModeRestCycle >= GameManager.Instance.IdleModeRestCycle;
    }

    /// <summary>
    /// 이벤트로 외부에서 피로도를 증가시켜주는 로직
    /// </summary>
    private void AddRestCycle(int amount)
    {
        AddIdleModeRestCycle(amount);
        CheckIdleModeRestCycle();
    }

    /// <summary>
    /// 이벤트로 외부에서 피로도를 감소시켜주는 로직
    /// </summary>
    private void SubRestCycle(int amount) 
    {
        //추후 구현...
    }

    /// <summary>
    /// 방치모드 피로도 차는 로직
    /// </summary>
    private IEnumerator IdleModeRestCycleCoroutine()
    {
        curSequence = IdleModeRestCycleSequence.None;
        while (!CanEnterRestMode())
        {
            yield return Utill_Standard.WaitTimeOne;  // 1초 대기

            AddIdleModeRestCycle(1);

            if (CheckIdleModeRestCycle())
                yield break;
        }
    }

    /// <summary>
    /// 방치모드 휴식 게이지 줄어드는 로직
    /// </summary>
    private IEnumerator IdleModeRestTimeCoroutine()
    {
        while (CanEnterRestMode())
        {
            yield return Utill_Standard.WaitTimeOne;  // 1초 대기

            GameDataTable.Instance.User.IdleModeRestTime++;
            
            //휴식 게이지가 비었다면
            if (idleModeRestCycleUIView.UpdateIdleModeRestTimeBar())
            {
                //휴식모드 종료
                GameEventSystem.GameRestModeSequence_GameEventHandler_Event(IdleModeRestCycleSequence.End);
                GameDataTable.Instance.User.IdleModeRestCycle = 0;
                GameDataTable.Instance.User.IdleModeRestTime = 0;
                idleModeRestCycleUIView.SetRestCycleUI(false);
                StartCoroutine(IdleModeRestCycleCoroutine());
                yield break;
            }
        }
    }

    private IEnumerator SetDebugText()
    {
        while (true)
        {
            yield return Utill_Standard.WaitTimeOne;  // 1초 대기
            idleModeRestCycleUIView.SetTimeText(CanEnterRestMode());
        }
    }

    private IEnumerator SpawnUnEquipHunter()
    {
        yield return new WaitForSeconds(UnEquipHunterSpawnDelay);
        //구매한 캐릭터라면 미장착시에도 소환함
        for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            if (DataManager.Instance.Hunters[i].gameObject.activeInHierarchy == false &&
                GameDataTable.Instance.User.HunterPurchase[i])
            {
                DataManager.Instance.Hunters[i].gameObject.SetActive(true);
                DataManager.Instance.Hunters[i].ActiveHpObject(GameManager.Instance.IsHpOn);

                DataManager.Instance.Hunters[i].Setting(true, false); //기타 세팅 (함수 public으로 만들어줘야됨)
                Vector3 tmpRandomPos = DataManager.Instance.MainCameraMovement.GetSpawnPosition();
                Vector3 curHunterPos = new Vector3(tmpRandomPos.x, DataManager.Instance.Hunters[i].transform.position.y, tmpRandomPos.z);
                DataManager.Instance.Hunters[i].ResetPosition(curHunterPos);
                DataManager.Instance.Hunters[i].ChangeFSMState(HunterStateType.MoveToRest);

                //카메라 갱신
                //DataManager.Instance.MainCameraMovement.AddCalculateHunter(DataManager.Instance.Hunters[i]);
            }
        }
    }

    public void HunterArraivalToCamp(SubClass subClass)
    {
        hunterArrivalToCamp[(int)subClass] = true;
        for(int i = 0; i< hunterArrivalToCamp.Length; i++)
        {
            //구매하였으나 도달하지 못했으면
            if (!hunterArrivalToCamp[i] && GameDataTable.Instance.User.HunterPurchase[i])
                return;
        }
        //다 도착 하였으면 다음 시퀀스 시작
        GameEventSystem.GameRestModeSequence_GameEventHandler_Event(IdleModeRestCycleSequence.Start);
        hunterArrivalToCamp = new bool[3];
    }

    /// <summary>
    /// 현재 시퀀스를 기반으로 헌터의 fsm을 재설정함
    /// </summary>
    public void SetHunterStateUsingSequence(Hunter hunter)
    {
        switch (CurSequence)
        {
            case IdleModeRestCycleSequence.None:
            case IdleModeRestCycleSequence.End:
                break;
            case IdleModeRestCycleSequence.Cinematic:
                hunter.ChangeFSMState(HunterStateType.MoveToRest);
                break;
            case IdleModeRestCycleSequence.Start:
                hunter.ChangeFSMState(HunterStateType.Resting);
                break;
        }
    }

    /// <summary>
    /// 현재 휴식모드인지 여부
    /// </summary>
    public bool IsRestMode()
    {
        return CurSequence != IdleModeRestCycleSequence.None;
    }
    /// <summary>
    /// 현재 휴식모드에서 캠프에 앉은 상태인지 여부
    /// </summary>
    public bool IsSitCamp()
    {
        return CurSequence == IdleModeRestCycleSequence.Start;
    }
}
