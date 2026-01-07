using System;
using System.Collections;
using UnityEngine;

namespace Monster.AI.Command
{
    public class IdleCommand : AICommand
    {
        public override void OnEnter(Blackboard.Blackboard blackboard, Action onComplete = null)
        {
            base.OnEnter(blackboard);
            Debug.Log("IdleCommand OnEnter");
            if (blackboard is null)
            {
                Debug.LogError("Blackboard is null. Cannot execute IdleCommand.");
                OnExit(blackboard);
                onComplete?.Invoke();
                return;
            }
            // NavMeshAgent를 정지시키고, 이동을 중지
            if (blackboard.NavMeshAgent is not null)
            {
                blackboard.NavMeshAgent.isStopped = true;
                blackboard.NavMeshAgent.ResetPath();
                Debug.Log("AI has stopped moving.");
            }
        }

        public override void Execute(Blackboard.Blackboard blackboard, Action onComplete)
        {
            // Idle 상태 처리
            
            OnExit(blackboard);
            // 명령어 완료 콜백 호출
            onComplete?.Invoke();
        }

        public override void OnExit(Blackboard.Blackboard blackboard)
        {
            base.OnExit(blackboard);
            Debug.Log("IdleCommand OnExit");
        }
    }
}