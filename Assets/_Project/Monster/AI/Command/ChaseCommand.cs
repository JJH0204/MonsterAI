using System;
using System.Collections;
using UnityEngine;

namespace Monster.AI.Command
{
    public class ChaseCommand : AICommand
    {
        private static readonly int Run = Animator.StringToHash("Run");

        private static bool CheckBlackboard(Blackboard.Blackboard blackboard)
        {
            if (blackboard?.NavMeshAgent is null)
            {
                Debug.LogError("NavMeshAgent is null. Cannot execute ChaseCommand.");
                return false;
            }
            
            // 타겟의 위치가 유효한지 확인
            if (blackboard.Target is null || !blackboard.Target.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Target is not valid for ChaseCommand.");
                return false;
            }
            return true;
        }

        public override void OnEnter(Blackboard.Blackboard blackboard, Action onComplete = null)
        {
            base.OnEnter(blackboard);
            Debug.Log("ChaseCommand OnEnter");

            if (!CheckBlackboard(blackboard))
            {
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }

            if (blackboard.Target is null || blackboard.NavMeshAgent is null)
            {
                Debug.LogError("NavMeshAgent is null. Cannot execute ChaseCommand.");
                OnExit(blackboard);
                return;
            }
            
            blackboard.NavMeshAgent.stoppingDistance = blackboard.CharData.minDetectiveRange;
            blackboard.Agent.transform.LookAt(blackboard.Target.transform);
            blackboard.NavMeshAgent.speed = blackboard.CharData.runSpeed;
            blackboard.NavMeshAgent.isStopped = false;
            
            // TODO: 이동 애니메이션 재생
            
            Debug.Log("Chase Strat");
        }

        public override void Execute(Blackboard.Blackboard blackboard, Action onComplete)
        {
            if (!CheckBlackboard(blackboard)) 
            {
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            
            // 1. 목표물이 유효한지 매 프레임 확인
            if (blackboard.Target is null || blackboard.NavMeshAgent is null)
            {
                Debug.Log("Target is null or NavMeshAgent is null. Exiting ChaseCommand.");
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            
            // 2. 목표물의 위치가 변경 될 수 있음으로, 목적지 갱신
            blackboard.NavMeshAgent.SetDestination(blackboard.Target.transform.position);
            
            // 3. 목표물과 충분히 가까운지 확인
            // float distance = Vector3.Distance(blackboard.Agent.transform.position, blackboard.Target.transform.position);
            // float stopDistance = blackboard.CharData.minDetectiveRange;
            // if (distance <= stopDistance)
            if (!blackboard.NavMeshAgent.pathPending && blackboard.NavMeshAgent.remainingDistance <= blackboard.NavMeshAgent.stoppingDistance)
            {
                // 4. 충분히 가까운 경우, Chase 상태 종료
                Debug.Log("Reached target or close enough. Exiting ChaseCommand.");
                
                OnExit(blackboard);
                onComplete?.Invoke();
            }
        }
        
        public override void OnExit(Blackboard.Blackboard blackboard)
        {
            // Chase 상태 종료 처리
            // blackboard.State = MonsterState.Idle;
            // blackboard.Action.RemoveAction(EAction.Chaseing);
            if (blackboard.NavMeshAgent is not null && blackboard.NavMeshAgent.isOnNavMesh)
            {
                blackboard.NavMeshAgent.isStopped = true; // 이동을 멈춤
                blackboard.NavMeshAgent.ResetPath(); // 경로를 초기화
            }

            if (blackboard.Animator.gameObject.activeInHierarchy)
            {
                blackboard.Animator.SetBool(Run, false);
            }
            
            base.OnExit(blackboard);
            Debug.Log("ChaseCommand OnExit");
        }
    }
}