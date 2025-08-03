using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public abstract class BTDecorator : BTNode
    {
        [SerializeField] public BTNode child;

        public override void OnValidateNode()
        {
            if (child != null)
                child.parent = this;
        }
    }
}
