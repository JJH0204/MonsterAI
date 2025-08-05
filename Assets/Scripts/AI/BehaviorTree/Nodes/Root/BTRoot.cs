using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [CreateAssetMenu(fileName = "New Root Node", menuName = "BehaviorTree/Root")]
    public class BTRoot : BTComposite
    {
        // [HideInInspector] public new string name = "Root";       // TODO: 노드의 이름을 설정해야 하는데 방법을 모르겠다. 
        
        public override NodeState Evaluate(Blackboard blackboard)
        {
            foreach (var child in children)
            {
                var result = child.Evaluate(blackboard);    // 자식 노드를 순회하면서 실행 > 상태를 받아 옴
                if (result != NodeState.Success)                    // 자식의 실행 결과가 Success가 아니면
                    return result;                                  // 자식의 상태를 반환하고 루프 종료 > 자식 노드 들 중 하나라도 실패하면 전체 실패 > 시퀄스 노드와 같은 기능 : 굳이 루트 노드를 따로 정의할 필요가 있을까?
            }
            return NodeState.Success;
        }
    }
}
