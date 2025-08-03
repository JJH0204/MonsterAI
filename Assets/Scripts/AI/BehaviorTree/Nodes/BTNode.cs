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
        [HideInInspector] public NodeState state;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;

        public BTNode parent;
        public abstract NodeState Evaluate(Blackboard blackboard);

        public virtual void OnValidateNode()
        {

        }
    }
}
