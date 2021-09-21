using System.Runtime.InteropServices;
using System.ComponentModel;
using System;
using GDG.ECS;
using System.Collections.Generic;
using System.Security;
using UnityEngine.Internal;
using UnityEngineInternal;


namespace GDG.Utils
{
    public class AgentComponent : ECS.IComponent
    {
        public string flowFieldName;
        public float speed = 5;
        public bool isAgent = false;
    }
}