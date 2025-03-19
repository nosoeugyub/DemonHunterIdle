using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 맹독상태
/// </summary>
public class PosionStatusEffect : StatusEffect
{
    private Coroutine stunCoroutine;
    private float Damage;
    DamageInfo damageinfo;
    Hunter senderHunter;
    GameObject PosionEffect;
    Coroutine followCoroutine;
    public PosionStatusEffect(Utill_Enum.Debuff_Type statusType , float duration , float damage , Hunter _hunter)
    {
        StatusType = statusType;
        Duration = duration;
        Damage = damage;
        senderHunter = _hunter;

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
        stunCoroutine = target.StartCoroutine(PosionCoroutine(target));



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

        // 기존의 스턴 상태를 해제합니다.
        PosionEffect.gameObject.SetActive(false);
        PosionEffect = null;
        target.StopCoroutine(followCoroutine);
    }

    // 스턴 코루틴
    private IEnumerator PosionCoroutine(Entity target)
    {
        // 스턴 상태를 설정합니다.

        Debug.Log("Stunned");
        //각타입에맞게 알맞은로직을 구성
        if (target is Hunter)
        {
            Hunter hunter = target as Hunter;
        }
        else if (target is Enemy)
        {
            Enemy enemy = target as Enemy;
            float elapsed = 0f;
            float interval = 1f; // 1초마다 대미지를 줍니다.
            damageinfo = new(Damage, false , senderHunter);

            //이펙트도생깁니다.
            PosionEffect = ObjectPooler.SpawnFromPool("PoisonOnBasicAttack", enemy.transform.position);
            //위치및사이즈조절
            PosionEffect.transform.position = enemy.transform.position;

            // 지속적으로 이펙트의 위치를 업데이트하는 코루틴
            followCoroutine = enemy.StartCoroutine(FollowTarget(enemy));

            while (elapsed < Duration)
            {
                //특정사운드도 ..
                SoundManager.Instance.PlayAudio("PoisonOnBasicAttack");
                // 대상에게 대미지를 줍니다.
                enemy.Damaged(damageinfo);
                // interval 시간만큼 기다립니다.
                yield return new WaitForSeconds(interval);

                elapsed += interval;
            }
            PosionEffect.gameObject.SetActive(false);
            enemy.StopCoroutine(followCoroutine);
               PosionEffect = null;
        }
        // 스턴 코루틴을 null로 초기화합니다.
        stunCoroutine = null;
    }

    // 상태 효과 업데이트 메서드 (여기서는 사용하지 않음)
    public override void UpdateEffect(Entity target)
    {
        // 스턴 상태 효과는 시간이 지남에 따라 변화하는 것이 없으므로 사용하지 않습니다.
    }

    private IEnumerator FollowTarget(Enemy enemy)
    {
        while (true)
        {
            if (PosionEffect != null)
            {
                PosionEffect.transform.position = enemy.transform.position;
            }
            yield return null; // 다음 프레임까지 기다립니다.
        }
    }
}
