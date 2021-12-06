using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.Base
{
    public interface IUpdatable
    {
        void OnUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
    }
}