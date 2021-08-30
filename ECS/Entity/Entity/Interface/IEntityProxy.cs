using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    
    public interface IEntityProxy
    {
        void Convert(GameEntity entity, EntityManager entityManager);
    }
}