using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public abstract class BTCondition : BTNode
    {
        public override NodeState Evaluate(MonsterStats monsterStats)
        {
            return CheckCondition(monsterStats) ? state = NodeState.Success : NodeState.Failure;
        }

        protected abstract bool CheckCondition(MonsterStats monsterStats);
    }
}