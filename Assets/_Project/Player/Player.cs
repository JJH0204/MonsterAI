using UnityEngine;
using UnityEngine.AI;

namespace _Project
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private NavMeshAgent agent;
        private void Update()
        {
            agent.speed = speed;
            // 우클릭으로 캐릭터 제어
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                }
            }
                
        }
        
        public void TakeDamage(float damage)
        {
            Debug.Log($"{gameObject.name} took {damage} damage.");
        }
    }
}