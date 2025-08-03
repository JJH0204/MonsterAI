using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [CreateAssetMenu(menuName = "BehaviorTree/Root")]
    public class BTRoot : BTComposite
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            foreach (var child in children)
            {
                var result = child.Evaluate(blackboard);
                if (result != NodeState.Success)
                    return result;
            }
            return NodeState.Success;
        }
    }
}
