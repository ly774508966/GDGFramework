using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.AI
{
	public interface IGoapGoal<TGoal> where TGoal:Enum
	{
        TGoal GoalType{ get; }
        IGoapState Effect{ get; }
        bool IsDone{ get; set; }
        IGoapState InitEffect();
    }
}