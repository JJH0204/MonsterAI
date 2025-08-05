using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTActionDeath : BTNode
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            
            
            return state = NodeState.Success;
        }
    }
}