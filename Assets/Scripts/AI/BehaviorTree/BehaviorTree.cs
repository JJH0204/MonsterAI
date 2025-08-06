using System.Collections.Generic;
using System;
using AI.BehaviorTree.Nodes;
using UnityEditor;
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

            if (rootNode == null)
            {
                Debug.LogWarning("[BehaviorTree] Root node is null.");
                return;
            }

            // Validate and fix GUIDs for all nodes
            var allNodes = GetAllNodes();
            foreach (var node in allNodes)
            {
                if (string.IsNullOrEmpty(node.guid))
                {
                    node.guid = System.Guid.NewGuid().ToString();
                    Debug.LogWarning($"[BehaviorTree] Fixed missing GUID for node: {node.name}");
                }
            }

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
        
        // // Add Source Node
        // [HideInInspector]
        // public List<BTNode> nodes = new List<BTNode>();
        //
        // public T CreateNode<T>() where T : BTNode
        // {
        //     return (T)CreateNode(typeof(T));
        // }
        //
        // public BTNode CreateNode(Type type)
        // {
        //     var node = ScriptableObject.CreateInstance(type) as BTNode;
        //     node.name = type.Name;
        //     node.guid = System.Guid.NewGuid().ToString();
        //     node.position = Vector2.zero;
        //     nodes.Add(node);
        //     AssetDatabase.AddObjectToAsset(node, this);
        //     AssetDatabase.SaveAssets();
        //     return node;
        // }
        //
        // public void DeleteNode(BTNode node)
        // {
        //     nodes.Remove(node);
        //     AssetDatabase.RemoveObjectFromAsset(node);
        //     AssetDatabase.SaveAssets();
        // }
    }
}