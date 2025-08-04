using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public enum NodeState
    {
        Success,
        Failure,
        Running
    }

    // BT Node Base Class
    // [CreateAssetMenu(fileName = "New BT Node", menuName = "Behavior Tree/Node")]
    public abstract class BTNode : ScriptableObject
    {
        // GraphView에서 사용하기 위한 속성들
        [HideInInspector] public NodeState state;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public new string name;
        [HideInInspector] public List<BTNode> input = new List<BTNode>();
        [HideInInspector] public List<BTNode> outputs = new List<BTNode>();

        public BTNode parent;
        public abstract NodeState Evaluate(Blackboard blackboard);

        public virtual void OnValidateNode()
        {

        }
    }
}
