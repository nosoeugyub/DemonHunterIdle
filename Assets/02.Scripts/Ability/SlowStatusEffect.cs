using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 둔화 상태 
/// </summary>
public class SlowStatusEffect : StatusEffect
{
    private Coroutine stunCoroutine;

    //저장할 이속
    private float SaveSpeed;
    private float movespeedslowtime;
    private float attackpeedslowtime;
    //저장할 공속
    private float SaveAttackSpeed;

    private float MoveSpeedValue;
    private float AttackSpeedValue;


    GameObject PosionEffect;
    Coroutine followCoroutine;
    public SlowStatusEffect(Utill_Enum.Debuff_Type statusType , float duration , float _movespeedslowtime ,float _attackpeedslowtime)
    {
        StatusType = statusType;
        Duration = duration;
        movespeedslowtime = _movespeedslowtime;
        attackpeedslowtime = _attackpeedslowtime;
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
        stunCoroutine = target.StartCoroutine(SlowCoroutine(target));



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
    private IEnumerator SlowCoroutine(Entity target)
    {
        // 스턴 상태를 설정합니다.

        Debug.Log("Stunned");
        //각타입에맞게 알맞은로직을 구성
        if (target is Hunter)
        {
            Hunter hunter = entity as Hunter;
        }
        else if (target is Enemy)
        {
            Enemy enemy = entity as Enemy;
            float elapsed = 0f;
            float interval = 1f; // 1초마다 대미지를 줍니다.
            SaveSpeed = enemy._EnemyStat.MoveSpeed;
            SaveAttackSpeed = enemy._EnemyStat.AttackSpeed;

            //특정사운드도 ..
            SoundManager.Instance.PlayAudio("SlowOnBasicAttack");

           
            //이펙트도생깁니다.
            PosionEffect = ObjectPooler.SpawnFromPool("PoisonOnBasicAttack", enemy.transform.position);

            // 지속적으로 이펙트의 위치를 업데이트하는 코루틴
            followCoroutine = enemy.StartCoroutine(FollowTarget(enemy));

            // 대상에게 이속공속을 줄입니다
            float Beforemovespeed = EnemyStat.SetDeBuffMinusMoveSpeed(SaveSpeed, Duration);
            float BeforeAttackSpeed = EnemyStat.SetDeBuffMinusMoveSpeed(SaveAttackSpeed, Duration);

            EnemyStat.MiuseEnemyMoveSpeed(enemy._EnemyStat, Beforemovespeed);
            EnemyStat.MiuseEnemyMoveAttackSpeed(enemy._EnemyStat, BeforeAttackSpeed);

            Debug.Log("이동속도 감소값 " + Beforemovespeed + "현재 이동속도 " + enemy._EnemyStat.MoveSpeed);
            // interval 시간만큼 기다립니다.
            yield return new WaitForSeconds(Duration);

            // 대상에게 이속공속을 되돌립니다.
            float Aftermovespeed = EnemyStat.SetDeBuffPluseMoveSpeed(SaveSpeed, Duration);
            float AfterAttackSpeed = EnemyStat.SetDeBuffPluseMoveSpeed(SaveAttackSpeed, Duration);

            EnemyStat.PluseEnemyMoveSpeed(enemy._EnemyStat, Aftermovespeed);
            EnemyStat.PluseEnemyMoveAttackSpeed(enemy._EnemyStat, AfterAttackSpeed);

            //이펙트없어지
            PosionEffect.gameObject.SetActive(false);
            Debug.Log("이동속도 증가값 " + Aftermovespeed + "현재 이동속도 " + enemy._EnemyStat.MoveSpeed);

             enemy.StopCoroutine(FollowTarget(enemy));

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
