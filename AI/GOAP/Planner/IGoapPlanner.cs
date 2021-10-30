using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.AI
{
	
	public interface IGoapPlanner<TGoal,TAction> where TGoal:Enum where TAction:Enum
	{
         Queue<IGoapAction<TAction>> GeneratePlan();
    }
}