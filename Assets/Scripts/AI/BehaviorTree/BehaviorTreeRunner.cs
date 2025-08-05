using UnityEngine;

namespace AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviorTree tree;
        [SerializeField] private MonsterStats monsterStats;
        
        private void Start() => tree?.Init();
        private void Update() => tree?.Tick(monsterStats);
        
        public BehaviorTree Tree => tree;
        public MonsterStats MonsterStats => monsterStats;
    }
}
