using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster.Skill
{
    [CreateAssetMenu(menuName = "Monster/SkillDatabase")]
    public class MonsterSkills : ScriptableObject
    {
        public SkillData[] Datas = Array.Empty<SkillData>();
    }
}
