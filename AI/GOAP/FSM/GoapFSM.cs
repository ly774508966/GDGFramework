using System;
using System.Collections.Generic;
using GDG.Utils;

namespace GDG.AI
{
    public class GoapFSM : IGoapFSM
    {
        public IGoapFSMState CurrentState { get; private set;}
        public void ChangeState(IGoapFSMState newState)
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }
        public void OnUpdate()
        {
            CurrentState?.OnUpdate();
        }
    }
}