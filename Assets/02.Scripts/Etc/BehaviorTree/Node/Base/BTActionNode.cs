using System;

public class BTActionNode : BTNode
{
    private Func<BTNodeState> action = null;

    /// <summary>
    /// 본인의 실행 성공 여부를 반환함
    /// </summary>
    public override BTNodeState Evaluate()
    {
        if (action == null)
            return BTNodeState.Failed;

        return action.Invoke();
    }

    public BTActionNode(Func<BTNodeState> action)
    {
        this.action = action;
    }
}
