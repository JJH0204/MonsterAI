using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [CreateAssetMenu(menuName = "BehaviorTree/Action/Attack2Skill")]
    public class BTActionAttack2Skill : BTNode
    {
        public override NodeState Evaluate(MonsterStats monsterStats)
        {
            Debug.Log("Monster is Using Skill 2: " + monsterStats.name);
            monsterStats.State = MonsterState.Attack;
            return state = NodeState.Success;
        }
    }
}