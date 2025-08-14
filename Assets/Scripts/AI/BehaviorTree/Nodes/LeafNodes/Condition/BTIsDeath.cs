using AI.Blackboard;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class BTIsDeath : BTCondition
    {
        protected override bool CheckCondition(NodeContext context)
        {
            var blackboard = context.Blackboard;
            // 몬스터의 생명력이 0 이하인지 확인
            // return blackboard.Health <= 0f;
            return blackboard.TryGet(new BBKey<float>("Health"), out var health) && health <= 0f;
        }
    }
}