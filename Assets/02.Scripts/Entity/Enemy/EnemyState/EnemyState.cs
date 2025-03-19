/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 적의 스테이트 베이스 스크립트
/// </summary>
public class EnemyState 
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public virtual void Finally() { }
}
