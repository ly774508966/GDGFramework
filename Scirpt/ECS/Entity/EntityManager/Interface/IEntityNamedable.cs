using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;

namespace GDG.ECS
{
    public interface IEntityNamedable
    {
        void SetName(Entity entity);
    }
}