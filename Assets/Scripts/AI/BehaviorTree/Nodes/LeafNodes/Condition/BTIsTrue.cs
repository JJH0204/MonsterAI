using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTIsTrue : BTCondition
    {
        protected override bool CheckCondition(MonsterStats monsterStats)
        {
            return true;
        }
    }
}