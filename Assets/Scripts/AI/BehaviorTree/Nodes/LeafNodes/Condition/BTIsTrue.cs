using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTIsTrue : BTCondition
    {
        protected override bool CheckCondition(NodeContext context)
        {
            return true;
        }
    }
}