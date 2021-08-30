using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface IRecyclable
    {
        void OnRecycle();
    }
}