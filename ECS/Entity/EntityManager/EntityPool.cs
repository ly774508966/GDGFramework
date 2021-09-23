using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
namespace GDG.ECS
{
    public class EntityPool:IEntityEnable
    {
        public uint typeId;
        public readonly Stack<Entity> entityStack = new Stack<Entity>();
        public void PushEntity(Entity entity,Action<Entity> beforeRecycleCallback=null)
        {
            if(beforeRecycleCallback!=null)
                beforeRecycleCallback(entity);
            entity.OnRecycle();
            entityStack.Push(entity);
        }
        public Entity PopEntity(Action<Entity> beforeEnableCallback=null)
        {
            if(entityStack.Count!=0)
            {
                var tempEntity = entityStack.Pop();
                if(beforeEnableCallback!=null)
                    beforeEnableCallback(tempEntity);
                EnableEntity(tempEntity);
                return tempEntity;
            }
            var entity = new Entity();
            BaseWorld.Instance.EntityMaxIndexIncrease();
            entity.SetIndex(World.GetEntityMaxIndex());
            if(beforeEnableCallback!=null)
                beforeEnableCallback(entity);
            entity.OnInit();
            entity.OnEnable();
            return entity;
        }
        public T PopEntity<T>(Action<T> beforeEnableCallback=null)where T:Entity,new()
        {
            if(entityStack.Count!=0)
            {
                var tempEntity = entityStack.Pop() as T;
                if(beforeEnableCallback!=null)
                    beforeEnableCallback(tempEntity);
                EnableEntity(tempEntity);

                return tempEntity;
            }
            var entity = new T();
            BaseWorld.Instance.EntityMaxIndexIncrease();
            entity.SetIndex(World.GetEntityMaxIndex());
            if(beforeEnableCallback!=null)
                beforeEnableCallback(entity);            
            entity.OnInit();
            entity.OnEnable();
            return entity;
        }
        public void EnableEntity(Entity entity)
        {
            entity.OnEnable();
        }
    }
}