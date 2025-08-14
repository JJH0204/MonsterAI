using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTActionAttack : BTAction
    {
        public int skillId; // 스킬 ID, 예시로 1번 스킬 사용
        public override NodeState Evaluate(NodeContext context, HashSet<BTNode> visited)
        {
            if (CheckCycle(visited))
                return NodeState.Failure;
            
            Debug.Log("Monster is Using Skill" + skillId);
            // blackboard.State = MonsterState.Attack;
            return state = NodeState.Success;
        }
    }
}