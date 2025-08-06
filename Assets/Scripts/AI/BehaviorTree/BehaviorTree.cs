using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using UnityEngine;

namespace AI.BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/Tree")]
    public class BehaviorTree : ScriptableObject
    {
        public BTNode rootNode;
        // public MonsterStats blackboard;

        public void Init()
        {
            // blackboard.target = monsterStats.target;
            // blackboard.navMeshAgent = monsterStats.navMeshAgent;
            // blackboard.agent = monsterStats.gameObject;
            
            rootNode?.OnValidateNode();
        }

        public NodeState Tick(MonsterStats monsterStats)
        {
            var visited = new HashSet<BTNode>();
            return rootNode?.Evaluate(monsterStats, visited) ?? NodeState.Failure;
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