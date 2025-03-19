public class BTSequenceNode : BTNode
{
    /// <summary>
    /// 자식 노드들이 전부 Success를 반환할 때 까지 실행함.
    /// </summary>
    public override BTNodeState Evaluate()
    {
        if (children == null || children.Count <= 0)
            return BTNodeState.Failed;

        for (int i = 0; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case BTNodeState.Success: //성공했다면 다음 자식으로
                    continue;
                case BTNodeState.Failed: //실패했다면 노드 실행 중단
                    return BTNodeState.Failed;
                case BTNodeState.Running://실행 중이라면 끝날 때 까지 기다림
                    return BTNodeState.Running;
            }
        }

        return BTNodeState.Success; //다 성공했다면 성공 반환
    }
}
