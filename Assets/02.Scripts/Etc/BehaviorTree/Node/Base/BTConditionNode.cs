using System;
public class BTConditionNode : BTNode
{
    private Func<bool> condition; //검사할 조건

    /// <summary>
    /// 검사 조건에 따른 성공 여부 반환
    /// </summary>
    public override BTNodeState Evaluate()
    {
        return condition.Invoke() ? BTNodeState.Success : BTNodeState.Failed;
    }
    
    public BTConditionNode(Func<bool> conditionFunc)
    {
        condition = conditionFunc;
    }
}
