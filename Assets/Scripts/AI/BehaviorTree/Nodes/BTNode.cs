using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
        [HideInInspector] public NodeState state;                   // 노드 상태
        
        #region GraphView Setting

        [HideInInspector] public new string name;                   // 노드 이름
        [HideInInspector] public string guid;                       // 노드 GUI
        [HideInInspector] public Vector2 position;                  // 노드 좌표
        [HideInInspector] public BTNode input;                      // 입력 (부모 노드)
        [HideInInspector] public List<BTNode> outputs;              // 출력

        #endregion

        protected BTNode(NodeState state = NodeState.Success, string name = "Root", string guid = "", Vector2? position = null, BTNode input = null, List<BTNode> outputs = null)
        {
            this.state = state;
            this.name = name;
            this.guid = guid;
            this.position = position ?? Vector2.zero;
            this.input = input;
            this.outputs = outputs ?? new List<BTNode>();
        }
        
        // 노드 활동 정의
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Reset() { }
        public abstract NodeState Evaluate(MonsterStats monsterStats);
        public virtual void OnValidateNode() { }
    }
}
