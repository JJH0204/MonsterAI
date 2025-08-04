using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [CreateAssetMenu(menuName = "BehaviorTree/Actions/MoveToTarget")]
    public class TestMoveToTarget : BTAction
    {
        public override NodeState Evaluate(Blackboard blackboard)
        {
            // 실제 로직은 MonoBehaviour에서 구현해야 합니다.
            // 여기서는 단순히 상태를 반환하고 종료함
            Debug.Log("Moving to target: " + blackboard.target.name);
            
            // 몬스터 자신의 NavMeshAgent를 받아와서 이동 경로로 설정
            blackboard.navMeshAgent.SetDestination(blackboard.target.transform.position);
            
            state = NodeState.Running;
            return state;
        }
    }
}