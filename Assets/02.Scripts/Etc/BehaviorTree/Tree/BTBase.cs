using UnityEngine;

public abstract class BTBase : MonoBehaviour
{
    protected BTNode rootNode; //처음 시작할 root 노드

    /// <summary>
    /// Tree속에다 각종 Node를 넣어줌. 넣어준 순서에 따라 순차적으로 실행됨
    /// </summary>
    public abstract void SetupTree();

    /// <summary>
    /// 트리 실행
    /// </summary>
    public void Operate()
    {
        if (rootNode == null)
            return;

        rootNode.Evaluate();
    }

    //사용 여부 결정 후 없앨 것
    //public BTBase (BTNode node)
    //{
    //    rootNode = node;
    //}

}
