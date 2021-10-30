using System;
using System.Collections.Generic;
using GDG.Utils;

namespace GDG.AI
{
    public abstract class GoapGoal<TGoal> : IGoapGoal<TGoal> where TGoal : Enum
    {
        public abstract TGoal GoalType { get; }
        public IGoapState Effect{ get; }
        public bool IsDone{ get; set; }
        public GoapGoal()
        {
            Effect = InitEffect();
        }
        public abstract IGoapState InitEffect();
    }
}