using UnityEngine;

namespace AI.BehaviorTree
{
    public class Blackboard : ScriptableObject
    {
        public Transform target; // 타겟 오브젝트
        public float distanceToTarget; // 타겟과의 거리
        public float health; // AI의 현재 체력
        public float maxHealth; // AI의 최대 체력
        
        // AI의 상태를 나타내는 변수들
    }
}