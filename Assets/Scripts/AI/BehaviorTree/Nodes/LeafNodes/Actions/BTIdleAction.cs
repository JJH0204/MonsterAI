using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTIdleAction : BTAction
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            state = NodeState.Failure;
            return state;
        }
    }
}