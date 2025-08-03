using AI.BehaviorTree.Nodes;
using UnityEngine;

namespace AI.BehaviorTree
{
    public class BehaviorTree : ScriptableObject
    {
        public BTNode rootNode;
        public Blackboard blackboard;

        public void Init()
        {
            rootNode?.OnValidateNode();
        }

        public NodeState Tick()
        {
            return rootNode?.Evaluate(blackboard) ?? NodeState.Failure;
        }
    }
}