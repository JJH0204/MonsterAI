using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTIsAlive : BTCondition
    {
        protected override bool CheckCondition(Blackboard blackboard)
        {
            return blackboard.health > 0f;
        }
    }
}