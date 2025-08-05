using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public abstract class BTCondition : BTNode
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            return CheckCondition(blackboard) ? state = NodeState.Success : NodeState.Failure;
        }

        protected abstract bool CheckCondition(Blackboard blackboard);
    }
}