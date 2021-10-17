using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
namespace GDG.ECS
{
    public class EntityPool
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
                beforeEnableCallback?.Invoke(tempEntity);
                tempEntity.OnEnable();
                return tempEntity;
            }
            
            var entity = new Entity();
            BaseWorld.Instance.EntityManager.EntityMaxIndexIncrease();
            entity.SetIndex(World.EntityManager.GetEntityMaxIndex());
            entity.Name = "Entity " + entity.Index;
            BaseWorld.Instance.EntityManager.InitEntityComponent(entity.Index);

            if (!BaseWorld.Instance.EntityManager.m_Index2EntityMapping.ContainsKey(entity.Index))
                BaseWorld.Instance.EntityManager.m_Index2EntityMapping.Add(entity.Index, entity);

            if(entity == null)
            
            beforeEnableCallback?.Invoke(entity);

            entity.OnInit();
            entity.OnEnable();
            return entity;
        }
        // public T PopEntity<T>(Action<T> beforeEnableCallback=null)where T:Entity,new()
        // {
        //     if(entityStack.Count!=0)
        //     {
        //         var tempEntity = entityStack.Pop() as T;
        //         if(beforeEnableCallback!=null)
        //             beforeEnableCallback(tempEntity);
        //         EnableEntity(tempEntity);

        //         return tempEntity;
        //     }
        //     var entity = new T();
        //     BaseWorld.Instance.EntityManager.EntityMaxIndexIncrease();
        //     entity.SetIndex(World.EntityManager.GetEntityMaxIndex());
        //     entity.Name = "Entity " + entity.Index;
        //     BaseWorld.Instance.EntityManager.InitEntityComponent(entity.Index);

        //     if (!BaseWorld.Instance.EntityManager.m_Index2EntityMapping.ContainsKey(entity.Index))
        //         BaseWorld.Instance.EntityManager.m_Index2EntityMapping.Add(entity.Index, entity);

        //     if(beforeEnableCallback!=null)
        //         beforeEnableCallback(entity);            
        //     entity.OnInit();
        //     entity.OnEnable();
        //     return entity;
        // }
        public void EnableEntity(Entity entity)
        {
            entity.OnEnable();
        }
    }
}