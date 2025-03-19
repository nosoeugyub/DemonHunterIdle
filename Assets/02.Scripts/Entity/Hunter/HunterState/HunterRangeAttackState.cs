using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터가 원거리 공격하는 상태 구현
/// </summary>
public class HunterRangeAttackState : HunterAttackState
{
    public HunterRangeAttackState(Hunter hunter) : base(hunter)
    {
    }
    
    /// <summary>
    /// 공격 가능 여부
    /// </summary>
    public override bool CheckAttack()
    {
        nearEnemyList = Utill_Standard.FindAllObjectsInRadius<IEnemyDamageAble>(hunter.transform.position, hunter._UserStat.AttackRange, hunter.SearchLayer);
        nearEnemyList.RemoveAll(x => !x.CanGetDamage);

        if (nearEnemyList.Count <= 0)
        {
            return false;
        }

        nearEnemyList = nearEnemyList
        .OrderBy(obj => Vector3.Distance(hunter.transform.position, obj.ObjectTransform.position)) // 거리순으로 정렬
        .ToList();

        return true;
    }
    /// <summary>
    /// 실제 공격 로직
    /// </summary>
    public override void OnAttackAnimation()
    {
        nearEnemyList.RemoveAll(x => !x.CanGetDamage);
        //주변에 적이 있는지 체크
        if (nearEnemyList.Count <= 0)
        {
            hunter.ChangeFSMState(HunterStateType.Move);
            GameEventSystem.GameBattleSequence_GameEventHandler_Event(false);
            return;
        }

        SpawnArrow();
    }

    /// <summary>
    /// 화살 소환
    /// </summary>
    private void SpawnArrow()
    {
        hunter.IsMagicAtk = hunter._UserStat.AttackDamageType == AttackDamageType.Physical ? false : true;
        basicDamageInfo = new(hunter.Power, hunter.IsMagicAtk, hunter);
        basicDamageInfo.isNormalAttack = true;

        List<Projectile> projectileList = ProjectileGenerator.GenerateMultiProjectile(hunter.ProjectileName, hunter.FireTransform, HunterStat.ProjectileNumResult(hunter._UserStat), hunter.shotAngle, hunter.IsForcus);

        int totalProjectiles = projectileList.Count;
        float zOffset = 0f; // Z축 위치 증가 값 초기화
        float zoffsetparm = 0.0f;

        if (hunter.IsForcus) // 집중 공격일 때
        {
            // 모든 화살을 발사하기 위한 반복문
            for (int i = 0; i < totalProjectiles; i += 10)
            {
                // 현재 배치할 화살 수 결정
                zOffset = 0f;
                int batchCount = Mathf.Min(10, totalProjectiles - i); // 10발 또는 남은 발수
                List<Projectile> batchProjectiles = projectileList.GetRange(i, batchCount); // 10발 가져오기
                List<Projectile> orderedProjectiles = new List<Projectile>(); // 발사할 순서의 화살 리스트 생성

                int mid = batchCount / 2;
                int left = mid - 1;
                int right = mid;

                // 홀수일 때 중앙 화살 추가
                if (batchCount % 2 != 0)
                {
                    orderedProjectiles.Add(batchProjectiles[mid]);
                    right++;
                }

                // 좌우 화살 순서 추가
                while (left >= 0 || right < batchCount)
                {
                    if (left >= 0)
                    {
                        orderedProjectiles.Add(batchProjectiles[left]);
                        left--;
                    }

                    if (right < batchCount)
                    {
                        orderedProjectiles.Add(batchProjectiles[right]);
                        right++;
                    }
                }

                // 순서대로 발사 및 Z축 위치 설정
                foreach (var projectile in orderedProjectiles)
                {
                    Vector3 position = projectile.transform.position;
                    position.z += zOffset; // Z축 위치 증가 적용
                    projectile.transform.position = position;

                    projectile.SetProjectileWithTarget(hunter, basicDamageInfo, nearEnemyList[0], hunter.SearchLayer, projectileId);
                    projectileId = projectileId < 100 ? projectileId + 1 : 0;

                    zOffset -= zoffsetparm; // 다음 화살의 Z축 증가
                }

                // Z축 위치 초기화
                zOffset = 0f; // Z축 위치 초기화
            }
        }
        else
        {
            for (int i = 0; i < projectileList.Count; i++)
                projectileList[i].SetProjectileWithTarget(hunter, basicDamageInfo, nearEnemyList[0], hunter.SearchLayer, projectileId);
        }

       
        projectileId = projectileId < 100 ? projectileId + 1 : 0; //일정 id 넘어가면 다시 0으로 초기화
    }


}
