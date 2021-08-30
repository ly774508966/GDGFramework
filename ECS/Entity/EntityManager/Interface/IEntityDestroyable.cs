using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface IEntityDestoryable
    {
        void DestroyEntity(AbsEntity entity);
    }
}