using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 민영(gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 럭키다이스 스킬
/// </summary>
public class CoinFilp : BaseSkill //CoinFilping
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수
    
    [Header("주사위 굴러가는 시간")]
    public float rollTime = 1f;
    #endregion

    public float bufftime = 0;
    //사용자

    private Sequence CoinSequence;
    private Coroutine currentCoroutine;
    CoinFilpData CurrentRangeShotData;
    int diceResult = 0; // 1과 3사이의 랜덤 값 생성
    public override event Action<float, bool> OnActiveSecondChanged;

    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        // 스킬이 이미 활성화 중이라면 새로운 스킬 발동을 막음
        if (isActiveSkill)
        {
            if (CurrentRangeShotData.CHGValue_SkillDuration > SkillCoolDown)
            {
                RemoveBuff(diceResult);
            }
        }

        // 스킬이 활성화 상태임을 표시
        isActiveSkill = true;

        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            CoinSequence.Kill();
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(StartCoinFilping());
    }

    IEnumerator StartCoinFilping()
    {
        // 사운드 재생
        SoundManager.Instance.PlayAudio(skillName);

        // 오브젝트 풀에서 동전 오브젝트 생성
        GameObject dice = ObjectPooler.SpawnFromPool(Tag.CoinFilp, hunter.transform.position);
        // 카메라의 뷰포트 중심을 월드 좌표로 변환
        Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, GameManager.Instance.MainCam.nearClipPlane));
        // dice의 위치를 카메라 중심에서 조금 앞으로 설정
        dice.transform.position = new Vector3(screenCenter.x, screenCenter.y, screenCenter.z + 1.0f); // 1.0f는 카메라에서의 Z 오프셋 
        // UI Canvas의 중앙 좌표를 3D 월드 좌표로 변환
        dice.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        // 동전의 초기 상태 설정 (작은 크기로 시작)
        dice.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // 아주 작은 크기
        dice.transform.rotation = Quaternion.Euler(0, 0, 0);

        // 동전 연출 시퀀스 시작
        CoinSequence.Kill();
        CoinSequence = DOTween.Sequence();

        // 현재 위치에서 위로 빠르게 이동 (y값을 2까지 빠르게 움직임)
        CoinSequence.Append(dice.transform.DOMoveY(2f, 0.3f).SetEase(Ease.OutQuad)); // 0.3초 동안 위로 이동
        // 작은 크기에서 큰 크기로 빠르게 커짐
        CoinSequence.Join(dice.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.1f).SetEase(Ease.OutBack)); // 0.3초 동안 크기 증가
        // 계속 빠른 속도로 회전
        CoinSequence.Append(dice.transform
            .DORotate(new Vector3(1080f, 0f, 0f), 1.0f, RotateMode.FastBeyond360) // 1초에 360도 회전
            .SetEase(Ease.Linear) // 일정한 속도로 회전
            .SetLoops(-1, LoopType.Restart)); // 무한 반복

        // 동전 앞면 또는 뒷면 결정 (1: 앞면, 2: 뒷면)
       
        // 동전 앞면 또는 뒷면 결정 (1: 앞면, 2: 뒷면, 3: 특별한 경우)
        float randomValue = Random.Range(0f, 100f); // 0부터 100까지의 float 값 생성

        
        if (randomValue > 45f)
        {
            diceResult = 1; // 45% 확률로 1
        }
        else if (randomValue < 90f)
        {
            diceResult = 2; // 45% 확률로 2 (45% ~ 90%)
        }
        else if (randomValue < 95)
        {
            diceResult = 3; // 45% 확률로 2 (45% ~ 90%)
        }
        else
        {
            diceResult = 4; // 10% 확률로 3 (95% ~ 100%)
        }


        yield return new WaitForSeconds(2f); // 동전 선택되고 선택된 면을 보여주는 시간 대기
        CoinSequence.Kill();
        // 사운드 재생
        SoundManager.Instance.PlayAudio(Tag.CoinFilp_Buff);
       
        switch (diceResult) // 
        {
            case 1:
                // dice의 회전값을 앞면 (0, 0, 0)으로 설정
                dice.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                // dice의 회전값을 뒷면 (180, 0, 0)으로 설정
                dice.transform.rotation = Quaternion.Euler(180, 0, 0);
                break;
            case 3:
                // dice의 회전값을 특별한 경우로 설정 (90, 0, 0)
                dice.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case 4:
                // dice의 회전값을 특별한 경우로 설정 (90도 기울임)
                dice.transform.rotation = Quaternion.Euler(90, 0, 0);
                yield return new WaitForSeconds(0.5f); // 동전 선택되고 선택된 면을 보여주는 시간 대기
                GameObject coinbomb = ObjectPooler.SpawnFromPool(Tag.CoinBomb, hunter.transform.position);
                // 코인 시퀀스 생성
                CoinSequence = DOTween.Sequence();

                dice.SetActive(false);
                coinbomb.transform.position = dice.transform.position;
                SoundManager.Instance.PlayAudio("Coin_Bomb");

                //해골 사운드 + 이펙트 등장

                // X축을 기준으로 회전시키는 연출 (Quaternion을 사용)
                //CoinSequence.Append(dice.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 1080f), 2.0f)
                //    .SetEase(Ease.Linear)
                //    .SetLoops(-1, LoopType.Incremental));

                // 옆으로 굴러가면서 씬 밖으로 나감
                //CoinSequence.Join(dice.transform.DOMoveX(10f, 2f).SetEase(Ease.OutQuad));
                break;
        }


        yield return new WaitForSeconds(0.5f);
        CoinSequence.Kill();
        // 동전 오브젝트 비활성화
        dice.SetActive(false);

       

        // 버프 적용 (동전 결과에 따라)
        ApplyBuff(diceResult);
        //지속효과 UI 시작
        OnActiveSecondChanged?.Invoke(CurrentRangeShotData.CHGValue_SkillDuration,true);
        bufftime = CurrentRangeShotData.CHGValue_SkillDuration;
        // 버프 지속시간 동안 대기
        yield return new WaitForSeconds(CurrentRangeShotData.CHGValue_SkillDuration); //CurrentRangeShotData.CHGValue_SkillDuration
        //지속효과 UI 종료
        OnActiveSecondChanged?.Invoke(0,false);
        bufftime = 0;
        // 버프 삭제
        RemoveBuff(diceResult);

        // 코루틴 종료
        currentCoroutine = null;
    }

    //버프 적용
    private void ApplyBuff(int diceResult)
    {
        switch (diceResult)
        {
            case 1:
                //골드획득 버프 증가
                //경험치
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.GoldFever, hunter.transform.position);
                HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_GoldEarned, Utill_Enum.Upgrade_Type.GoldBuff);
                break;
            case 2:
                //경험치
                //경험치
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.ExpFever, hunter.transform.position);
                HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_ExpEarned, Utill_Enum.Upgrade_Type.ExpBuff);
                break;
            case 3:
                //경험치 + 골드
                //경험치
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.GoldFever, hunter.transform.position);
                //경험치
                NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.ExpFever, hunter.transform.position);

                HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_GoldEarned, Utill_Enum.Upgrade_Type.ExpBuff);
                HunterStat.UseSkillAddUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_ExpEarned, Utill_Enum.Upgrade_Type.GoldBuff);
                break;
            case 4:
                break;
        }
    }

    //버프 삭제
    private void RemoveBuff(int diceResult)
    {
        switch (diceResult)
        {
            case 1:
                HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_GoldEarned, Utill_Enum.Upgrade_Type.GoldBuff);
                break;
            case 2:
                HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_ExpEarned, Utill_Enum.Upgrade_Type.ExpBuff);
                break;
            case 3:
                //경험치 + 골드
                HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_GoldEarned, Utill_Enum.Upgrade_Type.GoldBuff);
                HunterStat.UseSkillMinusUserStat(hunter._UserStat, CurrentRangeShotData.CHGValue_ExpEarned, Utill_Enum.Upgrade_Type.ExpBuff);
                break;
        }
    }

    public void Init_Skill()
    {
    }
    public  bool CheackEnemyArea(float _radlus)
    {
        return false;
    }
    public (Vector3 pos, List<Entity> entitylist) SpawnSkillArea(Utill_Enum.SkillCastingPositionType SkillCastingPositionType)
    {
        Vector3 position = Vector3.zero; 
        List<Entity> entitylist = new List<Entity>(); 
        return (position, entitylist);
    }
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        CurrentRangeShotData = (CoinFilpData)BaseSkillData.FindLevelForData(GameDataTable.Instance.ArcherSkillList, Tag.CoinFilp, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, CurrentRangeShotData.CHGValue_UseMP); //마나감소
        if (tempmana == 0)
        {
            IsUseSkill = false;
            return false; //마나 사용실패
        }
        else
        {
            //ui까지 변화 적용
            IsUseSkill = true;
            DataManager.Instance.Hunters[index].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[index]._UserStat.MP, DataManager.Instance.Hunters[index]._UserStat.CurrentMp);
            return true;
        }
    }
} 
