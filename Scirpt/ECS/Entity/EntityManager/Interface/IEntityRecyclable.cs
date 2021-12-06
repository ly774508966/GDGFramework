using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface IEntityRecyclable
    {
        void RecycleEntity(Entity entity);
    }
}