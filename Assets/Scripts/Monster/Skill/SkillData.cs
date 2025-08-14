using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster.Skill
{
    public enum SkillType
    {

    }
    
    [Serializable]
    public class SkillData
    {
        [SerializeField] private int skillId; // Unique identifier for the skill
        [SerializeField] private string skillName; // Name of the skill
        [SerializeField] private SkillType skillType; // Type of the skill
        [SerializeField] private float cooldown; // Cooldown time in seconds before the skill can be used again
        [SerializeField] private float range; // Effective range of the skill in units
        [SerializeField] private float damage; // Damage dealt by the skill (if applicable)
        [SerializeField] private float animationSpeed = 1.0f; 
        [SerializeField] private float castTime = 0.0f; // Time taken to cast the skill (default is 0.0, meaning instant cast)

        public int SkillId
        {
            get => skillId; 
            set => skillId = value;
        }
        public string SkillName
        {
            get => skillName; 
            set => skillName = value;
        }

        public SkillType SkillType
        {
            get => skillType; 
            set => skillType = value;
        }

        public float Cooldown
        {
            get => cooldown;
            set => cooldown = value;
        }

        public float Range
        {
            get => range;
            set => range = value;
        }

        public float Damage
        {
            get => damage;
            set => damage = value;
        }

        public float AnimationSpeed
        {
            get => animationSpeed;
            set => animationSpeed = value;
        }

        public float CastTime
        {
            get => castTime;
            set => castTime = value;
        }
        
        public SkillData(int skillId, string skillName, SkillType skillType, float cooldown, float range, float damage,
            float animationSpeed, float castTime)
        {
            SkillId = skillId;
            SkillName = skillName;
            SkillType = skillType;
            Cooldown = cooldown;
            Range = range;
            Damage = damage;
            AnimationSpeed = animationSpeed;
            CastTime = castTime;
        }
    }
}
