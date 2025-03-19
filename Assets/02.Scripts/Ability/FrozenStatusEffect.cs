using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 빙결 상태 
/// </summary>
public class FrozenStatusEffect : StatusEffect
{
    private Coroutine stunCoroutine;
    StunEffect stunEffect = null;
    SkinnedMeshRenderer[] meshRenderers = null;
    List<Material> normalMaterials = new();
    public FrozenStatusEffect(Utill_Enum.Debuff_Type statusType , float duration)
    {
        StatusType = statusType;
        Duration = duration;
    }

    // 상태 효과 적용 메서드
    public override void ApplyEffect(Entity target)
    {
        entity = target;
        if (stunCoroutine != null)
        {
            // 이미 스턴 중인 경우, 기존의 스턴 코루틴을 중지합니다.
            target.StopCoroutine(stunCoroutine);
        }

        // 스턴 코루틴을 시작합니다.
        stunCoroutine = target.StartCoroutine(FrozenCoroutine(target));



    }

    // 상태 효과 제거 메서드
    public override void RemoveEffect(Entity target)
    {
        if (stunCoroutine != null)
        {
            // 스턴 코루틴을 중지합니다.
            target.StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }
    }

    // 스턴 코루틴
    private IEnumerator FrozenCoroutine(Entity target)
    {
        // 스턴 상태를 설정합니다.
        Hunter hunter = null;
        Enemy enemy = null;

        Debug.Log("stun");
        //각타입에맞게 알맞은로직을 구성  스턴은 정지....
        if (target is Hunter)
        {
            hunter = entity as Hunter;
            hunter.StopHunter();
        }
        else if (target is Enemy)
        {
            enemy = entity as Enemy;
            //스턴애니메이션 시작
            enemy.ChangeFSMState(Utill_Enum.EnemyStateType.Debuff);

            //빙결이펙트
            Material freezingMaterial = GameManager.Instance.freezingMaterial;
            meshRenderers = enemy.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                normalMaterials.Add(meshRenderers[i].material);
                meshRenderers[i].material = freezingMaterial;
            }

            //특정사운드도 ..
            SoundManager.Instance.PlayAudio("FreezingOnBasicAttack");

        }

        // 지정된 시간 동안 기다립니다.
        yield return new WaitForSeconds(Duration);

        // 스턴 상태를 해제합니다.
        Debug.Log("Stun removed");
        //스턴애니메이션 끔
        enemy.UnStopEnemy();
        enemy.RemoveStatusEffect(this);
        

        if (meshRenderers != null)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                normalMaterials.Add(meshRenderers[i].material);
                meshRenderers[i].material = normalMaterials[i];
            }
            meshRenderers = null;
        }

        // 스턴 코루틴을 null로 초기화합니다.
        stunCoroutine = null;
    }

    // 상태 효과 업데이트 메서드 (여기서는 사용하지 않음)
    public override void UpdateEffect(Entity target)
    {
        // 스턴 상태 효과는 시간이 지남에 따라 변화하는 것이 없으므로 사용하지 않습니다.
    }
}
