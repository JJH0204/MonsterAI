using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[Serializable]
public class MonsterStats
{
    public NavMeshAgent navMeshAgent;
    public Transform body;
    public Transform target;
}

namespace AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviorTree tree;
        [SerializeField] private MonsterStats monsterStats;
        
        private void Start() => tree?.Init(monsterStats);
        private void Update() => tree?.Tick();
        
        public BehaviorTree Tree => tree;
        public MonsterStats MonsterStats => monsterStats;
    }
}
