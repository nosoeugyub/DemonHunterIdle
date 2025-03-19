/// <summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터의 스테이트 베이스 스크립트
/// </summary>
public class HunterState
{
    protected Hunter hunter;

    public HunterState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Exit() { }
    public virtual void Finally() { }
}
