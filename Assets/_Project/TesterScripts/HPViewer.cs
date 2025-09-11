using Monster.AI.Blackboard;
using Monster.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project
{
    public class HPViewer : MonoBehaviour
    {
        [SerializeField] private Blackboard blackboard;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Slider hpBar;
        
        private void Update()
        {
            if (blackboard is null || text is null || hpBar is null) return;

            float currentHP = blackboard.CurrentHealth;
            float maxHP = blackboard.MaxHealth;

            text.text = $"{currentHP} / {maxHP}";
            hpBar.value = maxHP > 0 ? currentHP / maxHP : 0;
        }
    }
}