using Monster.AI;
using UnityEngine;

namespace _Project
{
    public class DamageTest : MonoBehaviour
    {
        [SerializeField] private AIController aiController;
        
        public void Damage10()
        {
            aiController.OnHit(10);
        }

        public void Damage50()
        {
            aiController.OnHit(50);
        }
    }
}