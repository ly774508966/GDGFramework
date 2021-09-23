using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface IEntityEnable
    {
        void EnableEntity(Entity entity);
    }
}