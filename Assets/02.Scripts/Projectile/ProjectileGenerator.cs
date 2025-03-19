using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 투사체를 다중으로 발사할 시 위치/각도를 설정하고 생성
/// </summary>
public class ProjectileGenerator
{
    /// <summary>
    /// 지정된 풀 이름(poolName)에 따라 투사체(Projectile)를 여러 개 생성하고, 각도를 적용하여 리스트로 반환합니다.
    /// </summary>
    /// <param name="poolName">투사체를 소환할 오브젝트 풀의 이름</param>
    /// <param name="spawnPos">투사체가 생성될 위치와 회전을 나타내는 Transform</param>
    /// <param name="genNum">생성할 투사체의 개수</param>
    /// <param name="angleBetweenArrows">각 투사체 사이의 각도</param>
    /// <returns>생성된 투사체 리스트를 반환합니다. 투사체 생성에 실패할 경우 null을 반환합니다.</returns>
    public static List<Projectile> GenerateMultiProjectile(string poolName, Transform spawnPos, int genNum, float angleBetweenArrows, bool isFouce = false)
    {
        if (genNum == 0)
        {
            Game.Debbug.Debbuger.ErrorDebug("genNum cannot be zero");
            return null;
        }

        List<Projectile> projectileList = new List<Projectile>();

        if (isFouce)
        {
            poolName = "FocusArrow";
            GenerateSingleTargetProjectiles(poolName, spawnPos, genNum, projectileList);
        }
        else
        {
            GenerateSpreadProjectiles(poolName, spawnPos, genNum, angleBetweenArrows, projectileList);
        }

        return projectileList;
    }


    /// <summary>
    /// 단일 공격 시, 투사체를 나란히 캐릭터 앞에 배치하여 생성.
    /// </summary>
    private static void GenerateSingleTargetProjectiles(string poolName, Transform spawnPos, int genNum, List<Projectile> projectileList)
    {
        float spacing = 0.3f; // 투사체 간격
        Vector3 forward = spawnPos.forward;
        Vector3 initialPos = spawnPos.position; // 초기 위치 저장
        int rowLimit = 10; // 한 줄에 최대 생성할 투사체 수
        int currentRow = 0; // 현재 줄 번호
        float zOffset = 1f; // Z축 위치 증가 값

        // 투사체를 생성
        while (genNum > 0)
        {
            // 현재 줄에서 남은 투사체 수
            int currentCount = Mathf.Min(rowLimit, genNum);
            List<Projectile> tempProjectiles = new List<Projectile>();

            // 현재 줄에 투사체 생성
            for (int i = 0; i < currentCount; i++)
            {
                Vector3 spawnPosition;

                // Z축 위치 조정
                float zPositionOffset = currentRow * zOffset; // 현재 줄에 따라 Z축 위치 증가

                // 투사체 간격에 따라 x축 위치 결정
                if (currentRow == 0) // 첫 번째 줄
                {
                    spawnPosition = initialPos + forward * (1.5f) + spawnPos.right * (i - (currentCount / 2)) * spacing;
                }
                else // 두 번째 줄 이상
                {
                    int secondRowIndex = i - (currentCount / 2); // 중앙 기준으로 배치
                    spawnPosition = initialPos + forward * (1.5f + zPositionOffset) + spawnPos.right * (secondRowIndex * spacing);
                }

                // Y값 고정
                spawnPosition.y = spawnPos.position.y;
                spawnPosition.z += zPositionOffset;

                // 투사체 생성
                Projectile tempProjectile = ObjectPooler.SpawnFromPool(poolName, spawnPosition).GetComponent<Projectile>();
                tempProjectiles.Add(tempProjectile);
            }

            // 생성된 투사체를 리스트에 추가
            projectileList.AddRange(tempProjectiles);

            // 남은 투사체 수 감소
            genNum -= currentCount;
            currentRow++; // 다음 줄로 이동
        }
    }

    /// <summary>
    /// 다중 발사 시, 각 투사체 사이의 각도를 계산하여 좌우로 배치.
    /// </summary>
    private static void GenerateSpreadProjectiles(string poolName, Transform spawnPos, int genNum, float angleBetweenArrows, List<Projectile> projectileList)
    {
        // 투사체의 개수가 0이면 함수 종료
        if (genNum <= 0)
            return;

        // 중앙에 투사체 하나 생성
        Projectile centerProjectile = ObjectPooler.SpawnFromPool(poolName, spawnPos.position, spawnPos.localRotation).GetComponent<Projectile>();
        projectileList.Add(centerProjectile);

        // 투사체가 1개라면 중앙에만 생성하므로 함수 종료
        if (genNum == 1)
            return;

        int angleStep = 1;
        // 나머지 투사체들을 좌우로 교차 배치
        bool isRight = true; // 다음 투사체가 오른쪽에 배치될지 왼쪽에 배치될지 결정
               

        for (int i = 1; i < genNum; i++)
        {
     
            // 현재 각도를 계산 (angleStep을 사용하여 각도를 점진적으로 증가)
            float currentAngle = (angleStep) * angleBetweenArrows;

            // 투사체 생성
            Projectile projectile = ObjectPooler.SpawnFromPool(poolName, spawnPos.position, spawnPos.localRotation).GetComponent<Projectile>();

            // 로컬 좌표계를 기준으로 회전 적용
            if (isRight)
            {
                projectile.transform.Rotate(Vector3.forward, currentAngle, Space.Self);
            }
            else
            {
                projectile.transform.Rotate(Vector3.forward, -currentAngle, Space.Self);
            }

            // 생성된 투사체를 리스트에 추가
            projectileList.Add(projectile);

            // 다음 투사체는 반대쪽에 배치되도록 플래그 변경
            isRight = !isRight;

            // 두 개의 투사체가 한 쌍이 되므로 각도 증가
            if (i % 2 == 0)
            {
                angleStep++;
            }
        }
    }

    /// <summary>
    /// 좌우 대칭으로 첫 번째 투사체 생성.
    /// </summary>
    private static void GenerateSymmetricalProjectiles(string poolName, Transform spawnPos, float firstSpawnAngle, List<Projectile> projectileList)
    {
        float leftAngle = firstSpawnAngle * -1;
        Projectile tempLeft = ObjectPooler.SpawnFromPool(poolName, spawnPos.position, spawnPos.localRotation).GetComponent<Projectile>();
        tempLeft.transform.rotation = Quaternion.AngleAxis(leftAngle, Vector3.up) * spawnPos.localRotation;
        projectileList.Add(tempLeft);

        Projectile tempRight = ObjectPooler.SpawnFromPool(poolName, spawnPos.position, spawnPos.localRotation).GetComponent<Projectile>();
        tempRight.transform.rotation = Quaternion.AngleAxis(firstSpawnAngle, Vector3.up) * spawnPos.localRotation;
        projectileList.Add(tempRight);
    }

    /// <summary>
    /// 각도를 설정하여 좌우에 투사체를 생성.
    /// </summary>
    private static void CreateProjectilePair(string poolName, Transform spawnPos, float currentAngle, List<Projectile> projectileList)
    {
        // 좌우 투사체 각각 생성
        Projectile tempLeft = ObjectPooler.SpawnFromPool(poolName, spawnPos.position).GetComponent<Projectile>();
        tempLeft.transform.rotation = Quaternion.AngleAxis(-currentAngle, Vector3.up) * spawnPos.localRotation;
        projectileList.Add(tempLeft);

        Projectile tempRight = ObjectPooler.SpawnFromPool(poolName, spawnPos.position).GetComponent<Projectile>();
        tempRight.transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up) * spawnPos.localRotation;
        projectileList.Add(tempRight);
    }


}
