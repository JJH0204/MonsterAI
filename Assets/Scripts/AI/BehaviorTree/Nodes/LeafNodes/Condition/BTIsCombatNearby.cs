using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTIsCombatNearby : BTCondition
    {
        protected override bool CheckCondition(NodeContext context)
        {
            var blackboard = context.Blackboard;
            // 근처에서 싸움이 벌어졌는지 메니저를 통해 확인
            return MonsterManager.instance.IsNearMonsterBattle(blackboard.Agent, blackboard.DefaultStats.DetectionRange);
        }
    }
}