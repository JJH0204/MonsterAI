using System.Collections;
using System;
using UnityEngine;

namespace Monster.AI.Command
{
    public class WanderCommand : AICommand
    {
        private static readonly int Run = Animator.StringToHash("Run");

        // private static bool _animationRunning;
        private static bool CheckBlackboard(Blackboard.Blackboard blackboard)
        {
            if (blackboard is null)
            {
                Debug.LogError("Blackboard is null. Cannot execute Wander Command.");
                return false;
            }
            
            // Wander 정보가 유효한지 확인
            if (blackboard.WanderInfo is null)
            {
                Debug.LogWarning("WanderInfo is not valid for WanderCommand.");
                return false;
            }
                
            if (blackboard.WanderInfo.wanderAreaRadius <= 0f)
            {
                Debug.LogWarning("Wander area radius is not set or invalid.");
                return false;
            }
                
            // NavMeshAgent가 유효한지 확인
            if (blackboard.NavMeshAgent is null)
            {
                Debug.LogError("NavMeshAgent is null. Cannot execute Wander Command.");
                return false;
            }

            return true;
        }
        
        public override void OnEnter(Blackboard.Blackboard blackboard, Action onComplete = null)
        {
            base.OnEnter(blackboard);
            Debug.Log("WanderCommand OnEnter");
            
            if (!CheckBlackboard(blackboard))
            {
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            
            blackboard.WanderInfo.StartWanderTime = Time.time;
            blackboard.WanderInfo.CurrentWanderTime = blackboard.WanderInfo.GetRandomWanderTime();
            blackboard.WanderInfo.CurrentWanderPoint = blackboard.WanderInfo.GetRandomWanderPoint();
            blackboard.NavMeshAgent.destination = blackboard.WanderInfo.CurrentWanderPoint;
            blackboard.NavMeshAgent.isStopped = false; // 이동을 시작
                
            // NavMesh Speed 설정
            blackboard.NavMeshAgent.speed = blackboard.CharData.walkSpeed;
            // Wander 애니메이션 재생
            if (CheckAnimator(blackboard, "Run"))
            {
                // _animationRunning = true;
                blackboard.Animator.SetTrigger(Run);
            }
        }
        
        public override void Execute(Blackboard.Blackboard blackboard, Action onComplete)
        {
            if (!CheckBlackboard(blackboard)) return;
            
            // Wander 진행
            // if (blackboard.State is MonsterState.Wander)
            // if (blackboard.Action.HasState(EState.Wander))
            // 현재 Wander 시간이 초과되었는지 확인
            if (Time.time - blackboard.WanderInfo.StartWanderTime >= blackboard.WanderInfo.CurrentWanderTime)
            {
                // blackboard.WanderInfo.IsWandering = false;
                // Debug.Log("Wander time exceeded. Stopping wandering.");
                // blackboard.State = MonsterState.Idle;
                // blackboard.Action.RemoveState(EState.Wander);
                // blackboard.Action.RemoveAction(EAction.Wandering);
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            // 현재 Wander 지점으로 이동 중
            if (!(blackboard.NavMeshAgent.remainingDistance <= blackboard.NavMeshAgent.stoppingDistance)) return;
            // Debug.Log("Reached current wander point. Continuing to wander.");
            // blackboard.WanderInfo.IsWandering = false; // 현재 Wander를 종료하고 새로운 Wander를 시작
            blackboard.WanderInfo.CurrentWanderPoint = blackboard.WanderInfo.GetRandomWanderPoint();
            blackboard.NavMeshAgent.destination = blackboard.WanderInfo.CurrentWanderPoint;
            // blackboard.NavMeshAgent.isStopped = false; // 이동을 계속

            // // 명령어 완료 콜백 호출
            // onComplete?.Invoke();
        }
    }
}