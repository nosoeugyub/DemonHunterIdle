using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Header("최소 회전 각도")]
    public float minRotationAngle = -30f; 
    [Header("최대 회전 각도")]
    public float maxRotationAngle = 30f;


    public void OnMonsterHit(float eulerAnglesY)
    {
        // 히어로의 Y축 회전을 기준으로 랜덤 각도 생성
        float randomYAngle = Random.Range(minRotationAngle, maxRotationAngle);
        
        // 이펙트의 Y축 회전만 변경
        transform.rotation = Quaternion.Euler(0, randomYAngle + eulerAnglesY , 0);

        // 이펙트를 활성화 (이미 활성화된 상태로 스폰된 경우 이 줄은 생략 가능)
        gameObject.SetActive(true);
    }
}
