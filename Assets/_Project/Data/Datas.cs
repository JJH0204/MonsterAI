using System;

[Serializable]
public class CharData
{
    public int id;
    public string name;
    public float health;
    public float maxHealth;
    public float damage;
    public float walkSpeed;
    public float runSpeed;
    public float defence;
    public float maxDetectiveRange;
    public float minDetectiveRange;
}

[Serializable]
public class SkillData
{
    public int id;
    public string name;
    public enum SkillType
    {
        Melee,
        Ranged,
        Magic
    }
    public SkillType skillType;
    public float range;
    public float damage;
    public float cooldownTime;
    public float animSpeed;
    public string effectPrefab;
}

[Serializable]
public class Monster2SkillData
{
    public int charId;
    public int[] skillIds; // 스페이스바로 구분된 스킬 ID 문자열을 배열로 변환
}