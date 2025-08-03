using UnityEngine;

namespace AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviorTree tree;
        private void Start() => tree?.Init();
        private void Update() => tree?.Tick();
    }
}
