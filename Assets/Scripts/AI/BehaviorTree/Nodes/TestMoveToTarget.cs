using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class TestMoveToTarget : BTAction
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            // 실제 로직은 MonoBehaviour에서 구현해야 합니다.
            // 여기서는 단순히 상태를 반환하고 종료함
            state = NodeState.Running;
            return state;
        }
    }
}