using Managers;
using Monster.AI.Command;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Monster.AI.Blackboard
{
    public class Blackboard : MonoBehaviour
    {
        #region SerializeFields
        
        [Tooltip("몬스터의 현재 상태를 나타냅니다.")]
        [SerializeField] private MonsterState state;
        [SerializeField] private MonsterAction action; // 현재 행동 상태 (공격, 스킬 사용 등)
        [SerializeField] private int id;
        
        [Header("Dependency")] // 의존성 주입
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private GameObject agent;          // AI 에이전트 (몬스터)
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject target;
        [SerializeField] private GameObject deathEffect;    // 몬스터 사망 이펙트 프리팹
        // [SerializeField] private LayerMask obstacleLayer;
        
        // [Header("Battle Setting")][SerializeField] private MonsterBase baseInfo;   // 몬스터 기본 정보 스크립트
        [SerializeField] private MonsterPatrol patrolInfo; // 몬스터 순찰 스크립트
        [SerializeField] private MonsterWander wanderInfo; // 몬스터 방황 스크립트
        [SerializeField] private MonsterAttack attackInfo; // 몬스터 공격 스크립트

        #endregion
        
        #region private Fields
        
        private List<(int, float)> _cooldownList = new();    // 스킬의 쿨타입을 적용해 사용 가능 여부 관리
        private CharData _charData;
        private List<SkillData> _skills; // 몬스터가 사용할 수 있는 스킬들
        
        #endregion
        
        #region Properties
        
        public MonsterState State
        {
            get => state;
            set => state = value;
        }
        
        public MonsterAction Action
        {
            get => action;
            set => action = value;
        }
        
        public NavMeshAgent NavMeshAgent
        {
            get => navMeshAgent;
            set => navMeshAgent = value;
        }
        
        public GameObject Agent
        {
            get => agent;
            set => agent = value;
        }
        
        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }
        
        public GameObject Target
        {
            get => target;
            set => target = value;
        }
        
        public GameObject DeathEffect
        {
            get => deathEffect;
            set => deathEffect = value;
        }
        
        // public LayerMask ObstacleLayer
        // {
        //     get => obstacleLayer;
        //     set => obstacleLayer = value;
        // }
        
        public MonsterPatrol PatrolInfo
        {
            get => patrolInfo;
            set => patrolInfo = value;
        }
        
        public MonsterWander WanderInfo
        {
            get => wanderInfo;
            set => wanderInfo = value;
        }
        
        public MonsterAttack AttackInfo
        {
            get => attackInfo;
            set => attackInfo = value;
        }
        
        public float CurrentHealth
        {
            // get => TryGet(new BBKey<float>("health"), out float health) ? health : 0f;
            // set => Set(new BBKey<float>("health"), value);
            get => _charData?.health ?? 0f;
            set
            {
                if (_charData != null) _charData.health = value;
            }
        }

        public float MaxHealth
        {
            // get => TryGet(new BBKey<float>("maxHealth"), out float maxHealth) ? maxHealth : 0f;
            // set => Set(new BBKey<float>("maxHealth"), value);
            get => _charData?.maxHealth ?? 0f;
            set
            {
                if (_charData != null) _charData.maxHealth = value;
            }
        }

        public List<SkillData> Skills
        {
            get => _skills;
            set => _skills = value;
        }
        
        public CharData CharData
        {
            get => _charData;
            set => _charData = value;
        }

        #endregion
        
        #region Boolean Properties
        
        public bool IsAlive => CurrentHealth > 0f;
        public bool HasTarget => Target != null;

        #endregion

        #region Methods
        
        public void InitMonsterStatsByID()
        {
            // 몬스터의 인덱스 번호 확인
            if (id < 1000 || id > 1999) return;

            CharData defaultStats = DataManager.Instance.GetCharDataByID(id);
                
            if (defaultStats == null)
            {
                Debug.Log("Default stats not found for id: " + id);
                return;
            }
            
            // 몬스터 스탯 초기화
            {
                _charData = new CharData
                {
                    id = defaultStats.id,
                    name = defaultStats.name,
                    health = defaultStats.health,
                    maxHealth = defaultStats.maxHealth,
                    damage = defaultStats.damage,
                    walkSpeed = defaultStats.walkSpeed,
                    runSpeed = defaultStats.runSpeed,
                    defence = defaultStats.defence,
                    maxDetectiveRange = defaultStats.maxDetectiveRange,
                    minDetectiveRange = defaultStats.minDetectiveRange
                };
            }
            
            // 몬스터 스킬 초기화
            {
                _skills = new List<SkillData>();
                Monster2SkillData skillData = DataManager.Instance.GetMonster2SkillDataByCharID(id);

                foreach (var skillId in skillData.skillIds)
                {
                    SkillData skill = DataManager.Instance.GetSkillDataByID(skillId);
                    if (skill != null)
                    {
                        _skills.Add(skill);
                    }
                    else
                    {
                        Debug.LogWarning($"Skill data not found for ID: {id}");
                    }
                }
            }
        }
        

        public void Clear()
        {
            // _map.Clear();
            _charData = null;
            _skills?.Clear();
            Target = null;
            State.ClearStates();
            CleanUpCooldownList();
        }

        public SkillData GetSkillDataByID(int index)
        {
            foreach (SkillData skillData in _skills)
            {
                if (skillData.id == index)
                    return skillData;
            }
            return null;
        }
        
        #endregion

        #region Cooldown Management

        // 쿨타임 적용
        public void ApplyCooldown(int skillId, float cooldownTime)
        {
            // 이미 쿨타임이 적용된 스킬인지 확인
            for (int i = 0; i < _cooldownList.Count; i++)
            {
                if (_cooldownList[i].Item1 == skillId)
                {
                    // 기존 쿨타임 업데이트
                    _cooldownList[i] = (skillId, cooldownTime);
                    return;
                }
            }
            // 새로운 스킬 추가
            _cooldownList.Add((skillId, cooldownTime));
        }
        
        // 쿨타임이 끝났는지 확인
        public bool IsSkillReady(int skillId)
        {
            for (int i = 0; i < _cooldownList.Count; i++)
            {
                // 쿨타임이 적용된 스킬인지 확인
                if (_cooldownList[i].Item1 == skillId)
                {
                    // 쿨타임이 아직 남아있음
                    // Debug.Log($"Skill {skillId} is ready: {_cooldownList[i].Item2} seconds remaining.");
                    return false;
                }
            }
            // 쿨타임이 적용되지 않은 스킬
            return true;
        }
        
        // 쿨타임 목록에서 스킬 제거 (현재 시간 기준으로 쿨타임이 끝난 스킬 제거)
        public void UpdateCooldownList()
        {
            if (_cooldownList.Count == 0) return;
            float currentTime = Time.time;
            for (int i = _cooldownList.Count - 1; i >= 0; i--)
            {
                // 쿨타임이 끝난 스킬 제거
                if (_cooldownList[i].Item2 <= currentTime)
                {
                    _cooldownList.RemoveAt(i);
                }
            }
        }

        public void CleanUpCooldownList()
        {
            // 쿨타임 목록을 초기화
            _cooldownList.Clear();
        }

        public string ToStringCooldown()
        {
            string result = $"Blackboard for {gameObject.name}:\n";

            foreach (var cooldown in _cooldownList)
            {
                result += $"- Skill ID: {cooldown.Item1}, Remaining Time: {cooldown.Item2}\n";
            }

            return result;
        }

        #endregion
        
    }
}