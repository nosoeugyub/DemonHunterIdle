using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSY;
using System;

/// <summary>
/// 작성일자   : 2024-07-18
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 침묵의 광선 스킬
/// </summary>
public class ShroudRay : BaseSkill
{
    [Space(40)]
    [Header("--------------특정변수--------------")]
    #region 특정변수

    [Header("직사각형 피해범위")]
    public Vector3 cuboidDamageRadius = new Vector3(5f, 5f, 10f);

    [Header("이펙트 사이즈 가로 (이펙트 바뀌면 수정해줘야함)")]
    public float EffectSizeX;

    [Header("이펙트 사이즈 세로 (이펙트 바뀌면 수정해줘야함)")]
    public float EffectSizeY;

    #endregion

   
    private Coroutine currentCoroutine;
    private GameObject fx; //이펙트 담는 용도

    private float defenseValue = 0; //능력치가 얼마나 증가했는지 담는 변수
    
    private IEnumerator ShootRectangularRaycast()
    {
        defenseValue = 0;

        // 직사각형의 중심
        Vector3 originPoint = hunter.transform.position;
        Vector3 direction = hunter.transform.forward;

        RaycastHit[] hits = Physics.BoxCastAll(originPoint, cuboidDamageRadius / 2, direction, Quaternion.identity, cuboidDamageRadius.z, hunter.SearchLayer);

        //맞은 적들 방어력 수호자 방어력에 더하기
        foreach (var hit in hits)
        {
            defenseValue += hit.transform.GetComponent<Enemy>()._EnemyStat.PhysicalPowerDefense;
        }

        SoundManager.Instance.PlayAudio(skillName);

        Debug.Log("침묵의 광선 발사 " + defenseValue);

        fx = ObjectPooler.SpawnFromPool(Tag.ShroudRay, originPoint);
        
        // 이펙트 크기 조정
        float x = cuboidDamageRadius.x * EffectSizeX;
        float y = cuboidDamageRadius.z * EffectSizeY;
        fx.transform.localScale = new Vector3(x, y, 1);

        // 이펙트의 길이를 고려하여 시작점 설정
        Vector3 effectStartPosition = hunter.transform.position + hunter.transform.forward * (cuboidDamageRadius.z / 2);

        fx.transform.position = effectStartPosition;

        fx.transform.localRotation = Quaternion.Euler(hunter.transform.eulerAngles.x + 90, hunter.transform.eulerAngles.y, hunter.transform.eulerAngles.z);

        ApplyBuff(); // 버프 적용 

        yield return new WaitForSeconds(SkillDuration); // 버프 지속시간 기다린 후 
        
        RemoveBuff(); // 버프 삭제
    }
    public override void Execute(Utill_Enum.SubClass subclasstype)
    {
        if (currentCoroutine != null) //코루틴변수로 스킬이바껴도 버프가남아있으면다돌고 없어지게끔 수정
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ShootRectangularRaycast());
    }

    private void OnDrawGizmos()
    {
        if (hunter == null)
            return;

        Vector3 originPoint = hunter.transform.position;
        Vector3 direction = hunter.transform.forward;
        Vector3 boxSize = this.cuboidDamageRadius;
        float maxDistance = this.cuboidDamageRadius.z;

        // BoxCast의 시작점과 끝점 계산
        Vector3 boxStart = originPoint;
        Vector3 boxEnd = originPoint + direction * maxDistance;

        Gizmos.color = Color.blue;

        // BoxCast를 그리기 위한 Matrix 설정
        Matrix4x4 prevMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(boxStart, Quaternion.LookRotation(direction), Vector3.one);

        // 박스 그리기
        Gizmos.DrawWireCube(Vector3.forward * (maxDistance / 2), boxSize);

        // 최대 거리까지의 선 그리기
        Gizmos.DrawLine(originPoint, boxEnd);

        // 원래의 Gizmos.matrix로 복원
        Gizmos.matrix = prevMatrix;
    }

    //버프
    private void ApplyBuff()
    {
        HunterStat.UseSkillAddUserStat(hunter._UserStat, (int)defenseValue, Utill_Enum.Upgrade_Type.PhysicalPowerDefense);
    }

    //디버프
    private void RemoveBuff()
    {
        HunterStat.UseSkillMinusUserStat(hunter._UserStat, (int)defenseValue, Utill_Enum.Upgrade_Type.PhysicalPowerDefense);
    }

    
    public override void Upgrade(Utill_Enum.SubClass subclasstype)
    {
    }

}
