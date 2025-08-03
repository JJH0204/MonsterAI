using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTSelector : BTComposite
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            foreach (var child in children)
            {
                var childState = child.Evaluate(blackboard);
                if (childState == NodeState.Success)
                {
                    state = NodeState.Success;
                    return state;
                }
                if (childState == NodeState.Running)
                {
                    state = NodeState.Running;
                    return state;
                }
            }
            state = NodeState.Failure;
            return state;
        }
    }
}