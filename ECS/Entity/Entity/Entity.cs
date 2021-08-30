using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GDG.ECS
{
    public class Entity : AbsEntity
    {
        
    }
    public static class EntityExtension
    {
        public static void Recycle(this AbsEntity entity)
        {
            BaseWorld.Instance.EntityManager.RecycleEntity(entity);
        }
        public static void Destory(this AbsEntity entity)
        {
            BaseWorld.Instance.EntityManager.DestroyEntity(entity);
        }
    }
}