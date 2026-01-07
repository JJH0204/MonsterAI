using System;
using System.Collections;
using UnityEngine;

namespace Monster.AI.Command
{
    public class DeathCommand : AICommand
    {
        private static readonly int Death = Animator.StringToHash("Death");
        private Collider _collider;
        private Rigidbody _rigidbody;
        
        private bool CheckBlackboard(Blackboard.Blackboard blackboard)
        {
            if (blackboard is null)
            {
                Debug.LogError("Blackboard is null. Cannot execute DeathCommand.");
                return false;
            }
            // NavMeshAgent가 유효한지 확인
            if (blackboard.NavMeshAgent is null)
            {
                Debug.LogError("NavMeshAgent is null. Cannot execute DeathCommand.");
                return false;
            }
            if (blackboard.Agent is null)
            {
                Debug.LogError("Agent is null. Cannot execute DeathCommand.");
                return false;
            }

            _collider = blackboard.Agent.GetComponent<Collider>();
            if (_collider is null)
            {
                Debug.LogError("Collider is null. Cannot execute DeathCommand.");
                return false;
            }
            
            _rigidbody = blackboard.Agent.GetComponent<Rigidbody>();
            if (_rigidbody is null)
            {
                Debug.LogError("Rigidbody is null. Cannot execute DeathCommand.");
                return false;
            }
            
            return true;
        }

        public override void OnEnter(Blackboard.Blackboard blackboard, Action onComplete = null)
        {
            base.OnEnter(blackboard, () => { });
            Debug.Log("Entering DeathCommand.");
            
            // 예외처리 - 블랙보드 체크
            if (!CheckBlackboard(blackboard))
            {
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            
            // 1. 모든 상호작용 및 AI 로직 중단
            var navMeshAgent = blackboard.NavMeshAgent;
            if (navMeshAgent is not null)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }
            
            // 2. 물리적 상호작용 비활성화
            var collider = blackboard.Agent.GetComponent<Collider>();
            if (collider is not null)
                collider.enabled = false;
            
            // 3. 죽음 애니메이션 재생
            var animator = blackboard.Animator;
            if (animator is not null && CheckAnimator(blackboard, "Death"))
            {
                animator.SetTrigger(Death);
            }
        }
        
        public override void Execute(Blackboard.Blackboard blackboard, Action onComplete)
        {
            // if (!CheckBlackboard(blackboard)) return;
            //
            // // if (blackboard.State is MonsterState.Death) return;
            // if (blackboard.Action.HasAction(EAction.Dying)) return;
            //
            // // 죽음 상태 처리
            // _collider.enabled = false;
            // _rigidbody.isKinematic = true;
            // blackboard.NavMeshAgent.isStopped = true;
            // blackboard.NavMeshAgent.ResetPath();
            //
            //
            // // Animator가 유효한지 확인
            // if (CheckAnimator(blackboard, "Death"))
            // {
            //     // 재생 중인 모든 애니메이션 초기화
            //     blackboard.Animator.Rebind();
            //     blackboard.Animator.Update(0f);
            //
            //     // Death 애니메이션 재생
            //     blackboard.Animator.SetTrigger(Death);
            //     // yield return null;
            //
            //     AnimatorStateInfo animaState = blackboard.Animator.GetCurrentAnimatorStateInfo(0);
            //     float animationLength = animaState.length;
            //     // yield return new WaitForSeconds(animationLength); // 애니메이션 재생 시간 대기 (예: 2초)
            // }
            //
            // // blackboard.Agent.SetActive(false);  // 오브젝트 풀링 사용 시 별도 처리 필요
            // blackboard.DeathEffect.SetActive(true);
            // // yield return new WaitForSeconds(2.0f);
            //
            // // 명령어 완료 콜백 호출
            // onComplete?.Invoke();
            //
            // // blackboard.State = MonsterState.Death;
            // blackboard.Action.AddAction(EAction.Dying);
            if (!CheckBlackboard(blackboard))
            {
                OnExit(blackboard);
                
                // 명령어 완료 콜백 호출
                onComplete?.Invoke();
            }
            
            AnimatorStateInfo animatorStateInfo = blackboard.Animator.GetCurrentAnimatorStateInfo(0);
            
            if (animatorStateInfo.IsName("Death") && animatorStateInfo.normalizedTime >= 1.0f)
            {
                // 애니메이션이 끝났을 때 처리할 로직
                Debug.Log("Death animation completed.");
                
                // 사망 이펙트 활성화
                blackboard.DeathEffect.SetActive(true);
                
                OnExit(blackboard);
                
                // 명령어 완료 콜백 호출
                onComplete?.Invoke();
            }
        }
        
        public override void OnExit(Blackboard.Blackboard blackboard)
        {
            Debug.Log("Exiting DeathCommand.");
            
            // 사망 후 처리 로직
            // 예: 오브젝트 풀링을 사용하는 경우, 오브젝트를 비활성화하거나 재활용 큐에 넣기
            blackboard.Agent.SetActive(false);  // 오브젝트 풀링 사용 시 별도 처리 필요
            
            base.OnExit(blackboard);
            // 추가적인 정리 작업이 필요할 경우 여기에 작성
        }
    }
}