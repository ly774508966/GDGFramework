using System;
using System.Collections.Generic;
using GDG.Utils;

namespace GDG.AI
{
    public abstract class GoapAction<TAction> : IGoapAction<TAction> where TAction : Enum
    {
    	public abstract TAction ActionType { get; }
        public abstract int Cost{ get; }
        public abstract int Priority{ get; }
		public bool IsDone{get;set;}
        public IGoapState Effect { get; private set; }
        public IGoapState Precondition { get;private set; }
        protected IGoapAgent agent{ get; set; }
        public GoapAction(IGoapAgent agent)
		{
            this.agent = agent;
            Effect = InitEffect();
            Precondition = InitPrecondition();
        }
		/// <summary>
		/// 初始化先决条件
		/// </summary>
        public abstract IGoapState InitPrecondition();
		/// <summary>
		/// 初始化效果
		/// </summary>
        public abstract IGoapState InitEffect();

		public abstract void OnUpdate();
        public virtual void OnEnter(){}
        public virtual void OnExit(){}
		public bool VerifyPrecondition()
		{
            return agent.AgentState.ContainsState(Precondition);
        }

    }
}