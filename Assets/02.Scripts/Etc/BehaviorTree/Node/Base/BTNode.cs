using System.Collections.Generic;

/// <summary>
/// BT 노드들의 점검 상태
/// </summary>
public enum BTNodeState
{
    Success, //조건을 만족함
    Failed,  //조건을 만족하지 않음
    Running  //실행 중
}
public abstract class BTNode
{
    protected List<BTNode> children = new List<BTNode>(); //노드의 자식 노드
    
    /// <summary>
    /// 현재 노드의 조건을 검사
    /// </summary>
    /// <returns>실행 상태 여부</returns>
    public abstract BTNodeState Evaluate();

    /// <summary>
    /// 기존 노드 지우고 새로 Init
    /// </summary>
    public void InitChildren(List<BTNode> nodelist)
    {
        children.Clear();
        children.AddRange(nodelist);
    }

    public void AddChild(BTNode node)
    {
        children.Add(node);
    }
}
