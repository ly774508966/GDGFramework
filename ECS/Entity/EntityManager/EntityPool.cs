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
        public readonly Stack<AbsEntity> entityStack = new Stack<AbsEntity>();
        public void PushEntity(AbsEntity entity,Action<AbsEntity> beforeRecycleCallback=null)
        {
            if(beforeRecycleCallback!=null)
                beforeRecycleCallback(entity);
            entity.OnRecycle();
            entityStack.Push(entity);
        }
        public AbsEntity PopEntity(Action<AbsEntity> beforeEnableCallback=null)
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
            // if(beforeInitCallback!=null)
            //     beforeEnableCallback(entity);
            if(beforeEnableCallback!=null)
                beforeEnableCallback(entity);
            entity.OnInit();
            entity.OnEnable();
            return entity;
        }
        public T PopEntity<T>(Action<T> beforeEnableCallback=null)where T:AbsEntity,new()
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
            // if(beforeInitCallback!=null)
            //     beforeInitCallback(entity);
            if(beforeEnableCallback!=null)
                beforeEnableCallback(entity);            
            entity.OnInit();
            entity.OnEnable();
            return entity;
        }
        public void EnableEntity(AbsEntity entity)
        {
            
            entity.OnEnable();
        }
    }
}