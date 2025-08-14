using System;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    [CreateAssetMenu(menuName = "Monster/New Monster Stats")]
    public sealed class MonsterStatsDefault : ScriptableObject
    {
        [Header("Character Base Stats")]
        [SerializeField] private float maxHealth = 100f;        // 최대 체력
        [SerializeField] private float minSpeed = 3.5f;         // 이동 속도
        [SerializeField] private float maxSpeed = 5f;           // 추적 속도
        [SerializeField] private float damage = 10f;            // 공격력
        [SerializeField] private float attackSpeed = 1f;        // 공격 속도
        [SerializeField] private float defence = 5f;            // 방어력

        [Header("Monster Skills")]
        // [SerializeField] private MonsterSkill[] skills;          // 몬스터가 사용할 수 있는 스킬들
        // TODO: 스킬 설정을 모아둔 배열 
        
        [Header("Monster AI Config")]
        [SerializeField] private float detectionRange = 10f; // 타겟 인식 범위
        [SerializeField] private float rotationSpeed = 5f;   // 회전 속도
    
        public float MaxHealth => maxHealth;
        public float MinSpeed => minSpeed;
        public float MaxSpeed => maxSpeed;
        public float Damage => damage;
        public float AttackSpeed => attackSpeed;
        public float Defence => defence;
        
        public float DetectionRange => detectionRange;
        public float RotationSpeed => rotationSpeed;
        
        // TODO: 패트롤은 따로 구현할 예정
        // public Vector3[] PatrolPath => patrolPath;        // 방황 경로
        // public float PatrolTimer => patrolTimer;
        // public float PatrolColTimer => patrolColTimer;
    }
}
