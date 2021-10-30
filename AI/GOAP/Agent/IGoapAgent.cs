using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.AI
{
	public interface IGoapAgent
    {
        IGoapState AgentState{ get; }
        void OnUpdate();
        IGoapState InitAgentState();
    }
	public interface IGoapAgent<TGoal,TAction> : IGoapAgent where TGoal:Enum where TAction:Enum
	{
        IGoapGoal<TGoal> CurrentGoal{ get; }
        void StartGoal(TGoal goal);
        List<IGoapAction<TAction>> Actions{ get; }
        List<IGoapGoal<TGoal>> Goals { get; }
        List<IGoapAction<TAction>> InitActions();
        List<IGoapGoal<TGoal>> InitGoals();
        T GetAction<T>() where T : IGoapAction<TAction>;
    }
}