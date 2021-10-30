using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.AI
{
	public interface IGoapFSMState
	{
		bool IsDone{ get; set; }
        void OnEnter();
        void OnExit();
        void OnUpdate();
	}
	public interface IGoapFSM
	{
        IGoapFSMState CurrentState{ get; }
        void OnUpdate();
        void ChangeState(IGoapFSMState newState);
		
    }
}