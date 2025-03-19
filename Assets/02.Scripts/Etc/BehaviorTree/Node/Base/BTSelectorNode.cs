public class BTSelectorNode : BTNode
{
    /// <summary>
    /// 자식 노드들 중 하나가 Success를 반환할 때 까지 실행함.
    /// </summary>
    public override BTNodeState Evaluate()
    {
        if (children == null || children.Count <= 0)
            return BTNodeState.Failed;

        for(int i = 0; i < children.Count; i++)
        {
            switch(children[i].Evaluate())
            {
                case BTNodeState.Running: //검사중이면 계속 검사
                    return BTNodeState.Running;
                case BTNodeState.Success: //성공하면 실행 중단 후 성공 반환
                    return BTNodeState.Success;
            }
        }
        return BTNodeState.Failed; //모두 실패했다면 실패 반환
    }
}
