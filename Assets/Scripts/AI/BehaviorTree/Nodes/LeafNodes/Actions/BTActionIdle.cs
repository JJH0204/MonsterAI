using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTActionIdle : BTAction
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            // Idle 행동은 단순히 대기 상태를 유지하는 행동이다.
            // 이 행동은 일반적으로 AI가 아무것도 하지 않고 대기할 때 사용
            
            // 대기 애니메이션을 액션 스크립트에서 재생해야 할까?
            
            // Idle 행동은 항상 성공 상태를 반환한다.
            state = NodeState.Success;
            return state;
        }
    }
}