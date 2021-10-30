using System;
using System.Collections.Generic;
using GDG.Utils;

namespace GDG.AI
{
    public abstract class GoapAgent<TGoal,TAction> : IGoapAgent<TGoal,TAction> where TGoal : Enum where TAction : Enum
    {
        public IGoapGoal<TGoal> CurrentGoal{ get;private set; }
        public List<IGoapAction<TAction>> Actions{ get; private set; }
        public List<IGoapGoal<TGoal>> Goals { get; private set; }
        public abstract TGoal DefaultGoal { get; }
        public IGoapState AgentState { get; private set; }
        private IGoapPlanner<TGoal, TAction> planner;
        private IGoapFSM fsm = new GoapFSM();
        private Dictionary<TAction, IGoapAction<TAction>> actionDic = new Dictionary<TAction, IGoapAction<TAction>>();
        private Dictionary<TGoal, IGoapGoal<TGoal>> goalsDic = new Dictionary<TGoal, IGoapGoal<TGoal>>();
        private Queue<IGoapAction<TAction>> actionQueue;
        public GoapAgent()
		{
            AgentState = InitAgentState();
            Actions = InitActions();
            Goals = InitGoals();
            foreach(var item in Actions)
            {
                actionDic.Add(item.ActionType, item);
            }
            foreach(var item in Goals)
            {
                goalsDic.Add(item.GoalType, item);
            }
            InitPlanner();
            StartGoal(DefaultGoal);
        }
        public abstract List<IGoapAction<TAction>> InitActions();
        public abstract List<IGoapGoal<TGoal>> InitGoals();
        private void InitPlanner()
        {
            planner = new GoapPlanner<TGoal, TAction>(this);
        }

        public void OnUpdate()
        {
            if(fsm.CurrentState==null)
                return;
            if(fsm.CurrentState.IsDone)
            {
                if(actionQueue.Count == 0)
                {
                    CurrentGoal.IsDone = true;
                    StartGoal(DefaultGoal);
                }
                else
                {
                    var action = actionQueue.Dequeue();
                    fsm.ChangeState(action);
                    AgentState.SetDifferentStates(action.Effect);
                }
            }
            fsm.OnUpdate();
        }
        public void StartGoal(TGoal goalType)
        { 
            if(CurrentGoal !=null && goalType.Equals(CurrentGoal.GoalType))
                return;    
            if(!goalsDic.TryGetValue(goalType,out IGoapGoal<TGoal> goal))
            {
                Log.Error($"SetGoal Failed !There is no goal which  is registered ! GoalType :{goalType}");
                return;
            }
            CurrentGoal = goal;
            
       
            planner = new GoapPlanner<TGoal, TAction>(this);
            var actionQueue =  planner.GeneratePlan();
            if(actionQueue==null || actionQueue.Count == 0)
            {
                return;
            }
            fsm.ChangeState(actionQueue.Dequeue());
        }

        public abstract IGoapState InitAgentState();
        public T GetAction<T>() where T: IGoapAction<TAction>
        {
            foreach(var item in Actions)
            {
                if(item is T action)
                    return action;
            }
            return default(T);
        }
    }
}