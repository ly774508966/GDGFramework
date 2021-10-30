using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.AI
{
	public interface IGoapAction : IGoapFSMState
	{
        int Cost{ get; }
		int Priority{ get; }
		IGoapState Effect{ get; }
		IGoapState Precondition{ get; }
        IGoapState InitEffect();
        IGoapState InitPrecondition();		
	}
	public interface IGoapAction<TAction> : IGoapAction where TAction:Enum
	{
		TAction ActionType { get; }
    }
}