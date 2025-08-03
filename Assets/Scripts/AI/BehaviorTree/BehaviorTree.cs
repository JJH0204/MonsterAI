using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using UnityEngine;

namespace AI.BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/Tree")]
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
        
        public List<BTNode> GetAllNodes()
        {
            var nodes = new List<BTNode>();
            CollectNodes(rootNode, nodes);
            return nodes;
        }

        private void CollectNodes(BTNode node, List<BTNode> nodes)
        {
            if (node == null || nodes.Contains(node)) return;
            nodes.Add(node);

            if (node is BTComposite composite)
            {
                foreach (var child in composite.children)
                    CollectNodes(child, nodes);
            }
            else if (node is BTDecorator decorator && decorator.child != null)
            {
                CollectNodes(decorator.child, nodes);
            }
        }
    }
}