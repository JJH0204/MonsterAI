using Monster.AI.Blackboard;
using System;
using System.Collections;
using UnityEngine;

namespace Monster.AI.Command
{
    public class PatrolCommand : AICommand
    {
        private static readonly int Run = Animator.StringToHash("Run");

        // private static bool _animationRunning;
        // private static bool _isPatrol;
        private static bool CheckBlackboard(Blackboard.Blackboard blackboard)
        {
            if (blackboard is null)
            {
                Debug.LogError("Blackboard is null. Cannot execute Wander Command.");
                return false;
            }
            
            // Patrol 정보가 유효한지 확인
            if (blackboard.PatrolInfo == null)
            {
                Debug.LogWarning("PatrolInfo is not valid for PatrolCommand.");
                return false;
            }
                
            if (blackboard.PatrolInfo.wayPoints == null || blackboard.PatrolInfo.wayPoints.Length == 0)
            {
                Debug.LogWarning("Patrol waypoints are not set or invalid.");
                return false;
            }
                
            // NavMeshAgent가 유효한지 확인
            if (blackboard.NavMeshAgent is null)
            {
                Debug.LogError("NavMeshAgent is null. Cannot execute Patrol Command.");
                return false;
            }

            return true;
        }
        
        public override void OnEnter(Blackboard.Blackboard blackboard, Action onComplete = null)
        {
            base.OnEnter(blackboard, () => { });
            Debug.Log("PatrolCommand OnEnter");
            
            if (!CheckBlackboard(blackboard))
            {
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            
            // Patrol 애니메이션 재생
            if (CheckAnimator(blackboard, "Run"))
            {
                // _animationRunning = true;
                blackboard.Animator.SetTrigger(Run);
            }

            blackboard.PatrolInfo.StartPatrolTime = Time.time;
            blackboard.PatrolInfo.CurrentPatrolTime = blackboard.PatrolInfo.GetRandomPatrolTime();
            blackboard.PatrolInfo.CurrentWayPointIndex = blackboard.PatrolInfo.GetNextWayPointIndex();
            blackboard.NavMeshAgent.destination = blackboard.PatrolInfo.GetCurrentWayPoint();
            blackboard.NavMeshAgent.isStopped = false; // 이동을 시작
            blackboard.NavMeshAgent.speed = blackboard.CharData.walkSpeed;
        }
        
        public override void Execute(Blackboard.Blackboard blackboard, Action onComplete)
        {
            if (!CheckBlackboard(blackboard))
            {
                // 블랙보드가 유효하지 않으면 즉시 종료
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
    
            // 1. 전체 순찰 시간이 종료되었는지 먼저 확인합니다.
            if (Time.time - blackboard.PatrolInfo.StartPatrolTime >= blackboard.PatrolInfo.CurrentPatrolTime)
            {
                Debug.Log("Patrol time is over. Finishing PatrolCommand.");
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }

            // 2. 현재 목적지에 도달했는지 확인합니다.
            if (blackboard.PatrolInfo.IsDestinationReached(blackboard.Agent.transform.position))
            {
                Debug.Log("Reached a waypoint. Setting next waypoint.");
        
                // ★★★ 핵심 수정 부분 ★★★
                // OnExit()을 호출하는 대신, 다음 웨이포인트를 설정합니다.
                blackboard.PatrolInfo.CurrentWayPointIndex = blackboard.PatrolInfo.GetNextWayPointIndex();
                blackboard.NavMeshAgent.destination = blackboard.PatrolInfo.GetCurrentWayPoint();
            }
        }
        
        public override void OnExit(Blackboard.Blackboard blackboard)
        {
            // Patrol 상태 종료 처리
            // blackboard.State = MonsterState.Idle;
            // blackboard.Action.RemoveAction(EAction.Patrolling);
            blackboard.NavMeshAgent.isStopped = true; // 이동을 멈춤
            blackboard.NavMeshAgent.ResetPath(); // 경로를 초기화
            // Debug.Log("PatrolCommand OnExit");
            base.OnExit(blackboard);
        }
    }
}