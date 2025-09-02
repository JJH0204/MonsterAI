using UnityEngine;

namespace _Project
{
    public class Player : MonoBehaviour
    {
        public void TakeDamage(float damage)
        {
            Debug.Log($"{gameObject.name} took {damage} damage.");
        }
    }
}