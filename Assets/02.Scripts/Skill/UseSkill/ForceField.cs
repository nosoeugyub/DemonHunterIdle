using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Debbug;
using static Utill_Enum;
using System.Diagnostics;

/// 작성일자   : 2024-07-25
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 인력장 스킬
/// </summary>
public class ForceField : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수 
    [Header("사슬 크기")]
    [SerializeField] Vector3 ChainSize; // 끌어당기는 시간

    [Header("사슬 생성 간의 거리")]
    [SerializeField] float ChainDistance = 0.5f; //사슬 생성 간의 거리

    [Header("끌어당기는 당겨지는 시간 ")]
    [SerializeField] float pullDuration = 0.5f; // 끌어당기는 시간

    [Header("스턴 지속 시간 (끌어당겨진 후 적용)")]
    [SerializeField] private float stunDuration;


    [Header("범위 반지름")]
    [SerializeField] private float ForceFieldRadius;
    [Header("범위 크기")]
    [SerializeField] private float ForceFieldangle;


    #endregion


    ForceFieldData forcefileddata;

    //스킬사용
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        StopCoroutine(UseGraviton());
        StartCoroutine(UseGraviton());
    }

    private IEnumerator UseGraviton()
    {
        if (!hunter.CanGetDamage)
        {
            yield break;
        }

        // 탐지된 타겟 리스트
        var targets = Utill_Standard.FindEnemiesInCone(hunter.transform.position, hunter.transform.forward, ForceFieldRadius, ForceFieldangle);
        SoundManager.Instance.PlayAudio(skillName);

        // 각 타겟과 충돌 무시 설정
        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
                Physics.IgnoreCollision(hunter.GetComponent<Collider>(), enemy.GetComponent<Collider>(), true);
            }
        }

        // 각 타겟별로 체인 링크를 관리하기 위한 리스트
        List<(List<GameObject> chainLinks, Rigidbody targetRigidbody)> targetDataList = new List<(List<GameObject>, Rigidbody)>();

        if (forcefileddata.CHGValue_TargetNumber < targets.Count)
        {
            yield return null;
        }

        // 각 타겟에 대해 체인을 개별적으로 생성 및 관리
        foreach (var target in targets)
        {
            float distance = Vector3.Distance(hunter.transform.position, target.transform.position);
            if (distance < 1.5f) continue;

            // 타겟 방향 설정
            Vector3 direction = (target.transform.position - hunter.transform.position).normalized;
            Vector3 currentPosition = hunter.transform.position;

            // 체인 링크 개수 계산
            int chainLinkCount = Mathf.CeilToInt(distance / ChainDistance);
            List<GameObject> chainLinks = new List<GameObject>();
            GameObject lastChainLink = null;

            // 체인 링크 생성
            for (int i = 0; i < chainLinkCount; i++)
            {
                // 체인 링크 생성 및 초기화
                GameObject chainLink = null;
                if (i == chainLinkCount - 1)
                {
                    chainLink = ObjectPooler.SpawnFromPool(Tag.FirstChain, currentPosition, Quaternion.identity);
                }
                else
                {
                    chainLink = ObjectPooler.SpawnFromPool(Tag.Chain, currentPosition, Quaternion.identity);
                }
                Rigidbody ridbody = chainLink.GetComponent<Rigidbody>();
                var joint = chainLink.GetComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = true;
                ridbody.isKinematic = true;
                chainLink.transform.position = currentPosition;
                chainLink.transform.localScale = ChainSize;

                // 타겟 방향을 계산하여 Y축 회전만 변경
                Vector3 directionToTarget = target.transform.position - chainLink.transform.position;
                float targetYRotation = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
                Quaternion newRotation = Quaternion.Euler(0, targetYRotation, 0);
                chainLink.transform.rotation = newRotation;

                // 체인 링크의 교차 회전 설정 (Z축 기준)
                if (i % 2 == 0)
                {
                    chainLink.transform.localRotation = Quaternion.Euler(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 0); // 0도 회전
                }
                else
                {
                    chainLink.transform.localRotation = Quaternion.Euler(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 90); // 90도 회전
                }

                chainLinks.Add(chainLink);

                // 다음 위치로 이동
                currentPosition += (direction * ChainDistance);

                // 스프링 조인트 설정
                if (i == chainLinkCount - 1)
                {
                    lastChainLink = chainLink;
                    joint.connectedBody = target.GetComponent<Rigidbody>();
                    joint.spring = 10f;
                    joint.damper = 1f;
                    joint.maxDistance = 0.5f;
                }
                else if (lastChainLink != null)
                {
                    joint.connectedBody = lastChainLink.GetComponent<Rigidbody>();
                    joint.spring = 10f;
                    joint.damper = 1f;
                    joint.maxDistance = 0.5f;
                }
            }
            SoundManager.Instance.PlayAudio(Tag.ForceField_Hit);

            // 타겟별 체인 링크와 Rigidbody 저장
            Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
            targetDataList.Add((chainLinks, targetRigidbody));

            DealDamage(target as Enemy);
        }

        // 타겟을 끌어당기는 로직
        float elapsedTime = 0f;

        while (elapsedTime < pullDuration)
        {
            elapsedTime += Time.deltaTime;

            // 플레이어의 현재 위치를 계속 갱신
            Vector3 currentHunterPosition = hunter.transform.position;

            // 각 타겟별로 체인 이동
            foreach (var (chainLinks, targetRigidbody) in targetDataList)
            {
                Vector3 targetPosition = Vector3.Lerp(targetRigidbody.position, currentHunterPosition, elapsedTime / pullDuration);
                targetRigidbody.MovePosition(targetPosition);

                foreach (var chainLink in chainLinks)
                {
                    Enemy enemy = targetRigidbody.gameObject.GetComponent<Enemy>();
                    if (enemy.CanGetDamage == false) // 끌려오다 죽으면
                    {
                        continue;
                    }

                    // 체인 링크의 위치 갱신
                    chainLink.transform.position = Vector3.Lerp(chainLink.transform.position, currentHunterPosition, elapsedTime / pullDuration);

                    // 몬스터도 체인에 따라 위치 갱신
                    targetRigidbody.transform.position = Vector3.Lerp(chainLink.transform.position, currentHunterPosition , elapsedTime / pullDuration);
                }
            }

            yield return null; // 프레임마다 갱신
        }

        yield return Utill_Standard.WaitTimehalfOne;
        // 충돌 무시 해제
        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
                Physics.IgnoreCollision(hunter.GetComponent<Collider>(), enemy.GetComponent<Collider>(), false);
            }
        }

        // 체인 및 타겟 초기화
        foreach (var (chainLinks, targetRigidbody) in targetDataList)
        {
            foreach (var chainLink in chainLinks)
            {
                targetRigidbody.isKinematic = false;
                var joint = chainLink.GetComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;

                // 조인트의 연결된 위치 설정 (타겟의 로컬 좌표계로 변환)
                Vector3 worldAnchorPosition = new Vector3(0f, 0f, 0f); // 원하는 위치의 월드 좌표
                joint.connectedAnchor = worldAnchorPosition;
                joint.connectedBody = null;
                chainLink.transform.position = Utill_Standard.Vector3Zero;
                chainLink.SetActive(false);
            }
        }



        yield return new WaitForSecondsRealtime(0.1f);

        // 추가 로직: 대미지, 스턴 등 처리
        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
               StunEnemy(enemy);
            }
        }
    }

    // 대미지를 주는 메소드
    private void DealDamage(Enemy enemy)
    {
        DamageInfo  whirlwindDamageInfo = new DamageInfo(0, forcefileddata.attackdamagetype[0] == AttackDamageType.Magic, hunter);
        HunterStat.CalculateSkillDamage(hunter._UserStat, enemy._EnemyStat, whirlwindDamageInfo, forcefileddata);
        enemy.Damaged(whirlwindDamageInfo);
    }

    // 타겟을 끌어당기는 메소드 (예시)
    private IEnumerator PullTarget(Enemy target, Vector3 finalPosition)
    {
        float duration = 0.5f; // 끌어당기는 애니메이션 시간
        Vector3 initialPosition = target.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.transform.position = Vector3.Lerp(initialPosition, finalPosition, (elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        target.transform.position = finalPosition; // 최종 위치 설정
    }


    private void StunEnemy(Enemy enemy)
    {
        // 스턴 효과를 적용하는 로직
        enemy.ApplyStatusEffect(new StunStatusEffect(Utill_Enum.Debuff_Type.Stun , stunDuration));
    }

    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {

    }

    public override bool Cooldown(Utill_Enum.SubClass subclasstype)
    {
        forcefileddata = (ForceFieldData)BaseSkillData.FindLevelForData(GameDataTable.Instance.GurdianSkillLiat, Tag.ForceField, _upgradeAmount);
        int index = Utill_Standard.EnumToInt<Utill_Enum.SubClass>(subclasstype);//해당 타입의 인덱스가져오기
        if (subclasstype == Utill_Enum.SubClass.None)
        {
            return true;
        }
        float tempmana = 0;
        hunter = DataManager.Instance.Hunters[index];

        tempmana = HunterStat.MinusMp(DataManager.Instance.Hunters[index]._UserStat, forcefileddata.CHGValue_UseMP); //마나감소
        if (tempmana == 0)
        {
            return false; //마나 사용실패
        }
        else
        {
            //ui까지 변화 적용
            DataManager.Instance.Hunters[index].mpuiview.UpdateHpBar(DataManager.Instance.Hunters[index]._UserStat.MP, DataManager.Instance.Hunters[index]._UserStat.CurrentMp);
            return true;
        }
    }
}
