using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private List<CharData> charDataList;
        [SerializeField] private List<SkillData> skillDataList;
        [SerializeField] private List<Monster2SkillData> monster2SkillDataList;
        public CharData GetCharDataByID(int id)
        {
            foreach (CharData charData in charDataList)
            {
                if (charData.id == id)
                    return charData;
            }

            return null;
        }
        
        public SkillData GetSkillDataByID(int id)
        {
            foreach (SkillData skillData in skillDataList)
            {
                if (skillData.id == id)
                    return skillData;
            }

            return null;
        }
        
        public Monster2SkillData GetMonster2SkillDataByCharID(int charId)
        {
            foreach (Monster2SkillData monster2SkillData in monster2SkillDataList)
            {
                if (monster2SkillData.charId == charId)
                    return monster2SkillData;
            }

            return null;
        }
    }
}