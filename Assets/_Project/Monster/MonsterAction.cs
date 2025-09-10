using System;

namespace Monster
{
    // TODO: State와 같이 Dynamic하게 관리할지 검토 필요
    [Flags]
    public enum EAction
    {
        None = 0,
        Moving = 1 << 0,
        Attacking = 1 << 1,
        UsingSkill = 1 << 2,
        Dying = 1 << 3,
        Idle = 1 << 4,
        Chaseing = 1 << 5,
        Patrolling = 1 << 6,
        Wandering = 1 << 7,
        Hit = 1 << 8,
    }
    
    [Serializable]
    public class MonsterAction
    {
        public EAction CurrentAction { get; set; } = EAction.None;
        public void AddAction(EAction action) => CurrentAction |= action;
        public void RemoveAction(EAction action) => CurrentAction &= ~action;
        public bool HasAction(EAction action) => (CurrentAction & action) == action;
        public void ClearActions() => CurrentAction = EAction.None;
        public override string ToString()
        {
            var actions = Enum.GetValues(typeof(EAction));
            string result = "Current Actions: ";
            foreach (EAction action in actions)
            {
                result += action.ToString();
                if (HasAction(action))
                    result += " [Active]";
                result += ", ";
            }
            return result.TrimEnd(',', ' ');
        }
    }
}