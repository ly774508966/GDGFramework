using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    /// <summary>
    /// 实体的代理，用于与Unity组件的混合开发
    /// </summary>
    public interface IEntityProxy
    {
        void Convert(Entity entity, EntityManager entityManager);
    }
}