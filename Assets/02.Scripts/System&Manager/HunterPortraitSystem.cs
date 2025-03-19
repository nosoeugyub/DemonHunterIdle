using MonsterLove.StateMachine;
using NSY;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 헌터 초상화 관리                                
/// </summary> 
public class HunterPortraitSystem : MonoSingleton<HunterPortraitSystem>
{
    public HunterPortraitView hunterPortraitView;

    private Coroutine WaitFadeAndSettingCoroutine = null;
    private void Start()
    {
        GameEventSystem.GameHunterDie_SendGameEventHandler += SettingBattleAfterDie;
        GameEventSystem.GameSequence_SendGameEventHandler += GameSequence;
    }
    private bool GameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.DataLoad:
                StopAllCoroutines(); //초기화 고려해 현재 코루틴들 싹 멈춤
                return true;
        }
        return true;
    }
    /// <summary>
    /// Tag.HUNTER_OPEN_NUMBER 수치 반환
    /// </summary>
    /// <returns></returns>
    public int GetHunterDeployment()
    {
        int value = GameDataTable.Instance.ConstranitsDataDic[Tag.HUNTER_OPEN_NUMBER].Value;
        return value;
    }
    public void InitEquipSetting(bool isEquip, SubClass subClass)
    {
        //현재 장착중이라면
        hunterPortraitView.hunterPortraits[(int)subClass].isEquip = isEquip;
        hunterPortraitView.hunterPortraits[(int)subClass].InitCell(isEquip);
        hunterPortraitView.hunterPortraits[(int)subClass].InitBuyCell(GameDataTable.Instance.User.HunterPurchase[(int)subClass]);
    }

    /// <summary>
    /// 전체 사망시 헌터 재배치 로직
    /// </summary>
    public void SettingBattleAfterDie()
    {
        if (WaitFadeAndSettingCoroutine != null)
            StopCoroutine(WaitFadeAndSettingCoroutine);
        WaitFadeAndSettingCoroutine = StartCoroutine(WaitFadeAndSetting());
    }

    private IEnumerator WaitFadeAndSetting()
    {
        for (int i = 0; i < hunterPortraitView.hunterPortraits.Length; i++)
        {
            hunterPortraitView.hunterPortraits[i].StopReEquipCoroutine();
        }
        //  yield return new WaitForSeconds(GameManager.Instance.fadeDuration);
        while (!ResurrectionSystem.Instance.IsResurrection)
        {
            yield return null;
        }
        DataManager.Instance.MainCameraMovement.ClearCalculateHutner();
        //for (int i = 0; i < hunterPortraitView.hunterPortraits.Length; i++)
        //{
        //    hunterPortraitView.hunterPortraits[i].OnHunterDie();
        //}
        for (int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            SubClass subClass = DataManager.Instance.Hunters[i]._UserStat.SubClass;


            if (GameDataTable.Instance.User.CurrentDieHunter.Contains(subClass))
            {
                GameDataTable.Instance.User.CurrentDieHunter.Remove(subClass);
                GameDataTable.Instance.User.CurrentEquipHunter.Add(subClass);
                NSY.DataManager.Instance.Hunters[i].gameObject.SetActive(false);
                DataManager.Instance.MainCameraMovement.AddCalculateHunter(DataManager.Instance.Hunters[i]);
                //스킬 활성화도 시켜줌
                GameEventSystem.Send_GameHunterPortraitClickBtn_GameEventHandler(subClass, true);
            }
            else if (GameDataTable.Instance.User.CurrentEquipHunter.Contains(subClass))//간발의 차로 다시 태어난 헌터
            {
                NSY.DataManager.Instance.Hunters[i].StopHunter();
                NSY.DataManager.Instance.Hunters[i].gameObject.SetActive(false);
                DataManager.Instance.MainCameraMovement.AddCalculateHunter(DataManager.Instance.Hunters[i]);
                //스킬 활성화도 시켜줌
                GameEventSystem.Send_GameHunterPortraitClickBtn_GameEventHandler(subClass, true);
            }
        }
        SettingBattle(true); //활성화

        //yield return new WaitForSeconds(GameManager.Instance.fadeDuration); //페이드아웃까지 끝난 후
        DataManager.Instance.MainCameraMovement.CanMove = true;
        DataManager.Instance.MainCameraMovement.CalculateCamPos();


        ResurrectionSystem.Instance.IsResurrection = false;
    }

    /// <summary>
    /// 헌터 재배치 로직
    /// </summary>
    public void SettingBattle(bool isRevive = false)
    {
        for (int i = 0; i < NSY.DataManager.Instance.Hunters.Count; i++) //헌터 전체 순회
        {
            //현재 장착중이라면
            if (DataManager.Instance.isEquipHunter(DataManager.Instance.Hunters[i]._UserStat.SubClass))
            {
                //새로 장착할 헌터라면
                if (NSY.DataManager.Instance.Hunters[i].gameObject.activeInHierarchy == false)
                {
                    NSY.DataManager.Instance.Hunters[i].gameObject.SetActive(true);
                    NSY.DataManager.Instance.Hunters[i].ActiveHpObject(GameManager.Instance.IsHpOn);
                    NSY.DataManager.Instance.Hunters[i].Setting(isResetHP: isRevive); //기타 세팅 (함수 public으로 만들어줘야됨)

                    Vector3 tmpRandomPos = DataManager.Instance.MainCameraMovement.GetSpawnPosition();
                    Vector3 curHunterPos = new Vector3(tmpRandomPos.x, DataManager.Instance.Hunters[i].transform.position.y, tmpRandomPos.z);
                    NSY.DataManager.Instance.Hunters[i].ResetPosition(curHunterPos);
                    //카메라 갱신
                    DataManager.Instance.MainCameraMovement.AddCalculateHunter(DataManager.Instance.Hunters[i]);
                }
                else
                    NSY.DataManager.Instance.Hunters[i].SetHunterXAxis(false); //위치만

            }
            else //아니라면 삭제
            {
                NSY.DataManager.Instance.Hunters[i].StopHunter();
                NSY.DataManager.Instance.Hunters[i].gameObject.SetActive(false);
                NSY.DataManager.Instance.Hunters[i].ActiveHpObject(false);
                DataManager.Instance.MainCameraMovement.RemoveCalculateHunter(DataManager.Instance.Hunters[i]);
            }
        }
    }
}
