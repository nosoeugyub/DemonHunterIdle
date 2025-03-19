public class BTCycleNode : BTNode
{
    /// <summary>
    /// 자식 노드들이 전부 결과를 낼때까지 실행
    /// </summary>
    public override BTNodeState Evaluate()
    {
        if (children == null || children.Count <= 0)
            return BTNodeState.Failed;

        for (int i = 0; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case BTNodeState.Success: //실행했다면 다음 자식으로
                case BTNodeState.Failed:
                    continue;
                case BTNodeState.Running: //실행 중이라면 끝날 때 까지 기다림
                    return BTNodeState.Running;
            }
        }

        return BTNodeState.Success; //전부 실행 했다면 성공 반환
    }
}
