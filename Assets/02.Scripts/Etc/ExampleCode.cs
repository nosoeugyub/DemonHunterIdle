
#region --------규칙--------
//아래의 내용은 예시이며 변경이 가능 하며 지속적으로 보완해나갈 예정
/* 

클레스 내 메소드,변수 위치
- Fields (static/const)
- properties
- events/delegates (action)
- Monobehaviour Methods 
  (Awake, Start, OnEnable, OnDisable, OnDestroy,etc.)
- Abstract/Virtual/Override Methods
- Private Methods
- Protected Methods
- Public Methods

네이밍 규칙
- 전역변수(public,static,const,readonly) 대문자
- private, protected 소문자
- 파라미터(매개변수) _소문자

- 긴 단어는 조사를 사용하도록 ex. ListToArray/FindOfPlayer (with,over,for,to,of,,,,)
- 단어에 숫자가 들어가면 숫자키로 (tempVector1,tempVector2..)
- 잠깐 쓸 변수는 temp_area (지속되는 변수는 일반 소문자로)
- 전부다 대문자 쓰는 경우는 없음

- 변수는 명사 단위로
- 함수는 동사 단위로
- 리턴값이 bool일 시 Is붙이기
- 인터페이스 앞엔 I붙이기

애트리뷰트 위치
- 애트리뷰트는 엔터로 띄우기
  [SerializeField]
  private string str;

함수 형식
- 괄호는 한 칸 띄어서
  void Func() 
  {
    //something…
  }
- 함수와 함수 사이는 한 칸 띄우기

주석
- 안 쓰는 함수는 주석을 달기
- 긴 주석은 #region이용하기
- 특정 함수가 너무 길어지면 #region이용하기

*/
#endregion

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-05-28
/// 작성자     : 예은 (abcd@gmail.com)                                                       //  *최초 작성자 / 이후 유지 보수자는 비중 고려하여 추가 바람
/// 클래스용도 : 적이 움직이는 상태 구현                                                     //  *용도에 관해 최대한 자세히 기재
/// </summary>
public class ExampleCode : EnemyState
{
    NavMeshAgent navmeshAgent = null; //적의 navMeshAgent 참조 줄이기 위해 저장함            //  *변수용도

    /// <summary>
    /// 상태 초기화 함수
    /// </summary>                                                                         //  *함수기능 설명, <summary> 사용
    /// <param name="enemy">상태를 사용할 적</param>                                        //  필요시 매개변수 설명
    public ExampleCode(Enemy enemy) : base(enemy)
    {
        navmeshAgent = enemy.NavMeshAgent;
    }

    /// <summary>
    /// 이동 상태일 때 FixedUpdate에서 실행될 함수                                            //  *함수기능 설명, <summary> 사용
    /// </summary>
    public override void FixedUpdate()
    {
        if (enemy._EnemyStat == null) return;

        //개별 탐지 이동/공격 하려면 주석 해제
        List<IHunterDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(enemy.transform.position, enemy.SearchRadius, enemy.SearchLayer);
        //단체 탐지 이동/공격 하려면 주석 해제
        //List<IHunterDamageAble> damageable = Utill_Standard.FindAllObjectsInRadius<IHunterDamageAble>(transform.position, 15, searchLayer);

        for (int i = 0; i < damageable.Count; i++)
        {
            if (!damageable[i].CanGetDamage)
                damageable.Remove(damageable[i]);
        }
        //이미 사망한 IHunterDamageAble을 타겟으로 결정할 경우                                 //  *복잡한 로직의 경우 주석을 작성하여 다른 개발자들의 이해를 도움
        //부자연스럽게 움직여 데미지를 받을 수 있는 상황이지 않은 경우 삭제하도록 보완함 By YE    //  *작성자가 필요시 By OO 기재,  작성일자 필요시 기재

        List<IHunterDamageAble> attackAble = damageable.FindAll((target) => (target.ObjectTransform.position - enemy.transform.position).sqrMagnitude <= enemy._EnemyStat.AttackRange);
        if (attackAble.Count > 0)
        {
            enemy.ChangeFSMState(EnemyStateType.Attack);
            return;
        }
        if (damageable.Count <= 0)
        {
            enemy.ChangeFSMState(EnemyStateType.Idle);
            return;
        }

        if (navmeshAgent.isOnNavMesh)//네비메쉬에 매치되어있으면
        {
            //다음 목적지 설정
            navmeshAgent.SetDestination(damageable[0].ObjectTransform.position);
        }
        Rotate();
    }

    /// <summary>
    /// 상태가 종료될 시 호출될 함수
    /// </summary>
    public override void Exit()
    {
        enemy.Rigid.velocity = Utill_Standard.Vector3Zero;
        if (navmeshAgent.isOnNavMesh)
        {
            //navmeshAgent.SetDestination(transform.position);
            navmeshAgent.velocity = Utill_Standard.Vector3Zero;
        }
    }

    /// <summary>
    /// navMeshAgent를 이용해 움직인 값으로 방향을 구해 조절하는 함수                          //  *함수기능 설명, <summary> 사용
    /// </summary>
    private void Rotate()
    {
        Vector2 forward = new Vector2(enemy.transform.position.z, enemy.transform.position.x);
        Vector2 steeringTarget = new Vector2(navmeshAgent.steeringTarget.z, navmeshAgent.steeringTarget.x);

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        enemy.transform.eulerAngles = Vector3.up * angle;
    }
}